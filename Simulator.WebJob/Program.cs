// ---------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Common.Repository;
    using Configurations;
    using WebJob;
    using WebJob.DataInitialization;
    using WebJob.Engine.Devices.Factory;
    using WebJob.Engine.Telemetry.Factory;
    using WebJob.SimulatorCore.Devices.Factory;
    using WebJob.SimulatorCore.Logging;
    using WebJob.SimulatorCore.Repository;
    using WebJob.SimulatorCore.Serialization;
    using WebJob.SimulatorCore.Transport.Factory;

    public static class Program
    {
        static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        static IContainer simulatorContainer;

        const string ShutdownFileEnvVar = "WEBJOBS_SHUTDOWN_FILE";
        static string shutdownFile;
        static Timer timer;

        static void Main(string[] args)
        {
            try
            {
                // Cloud deploys often get staged and started to warm them up, then get a shutdown
                // signal from the framework before being moved to the production slot. We don't want 
                // to start initializing data if we have already gotten the shutdown message, so we'll 
                // monitor it. This environment variable is reliable
                // http://blog.amitapple.com/post/2014/05/webjobs-graceful-shutdown/#.VhVYO6L8-B4
                shutdownFile = Environment.GetEnvironmentVariable(ShutdownFileEnvVar);
                bool shutdownSignalReceived = false;

                // Setup a file system watcher on that file's directory to know when the file is created
                // First check for null, though. This does not exist on a localhost deploy, only cloud
                if (!string.IsNullOrWhiteSpace(shutdownFile))
                {
                    var fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(shutdownFile));
                    fileSystemWatcher.Created += OnShutdownFileChanged;
                    fileSystemWatcher.Changed += OnShutdownFileChanged;
                    fileSystemWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite;
                    fileSystemWatcher.IncludeSubdirectories = false;
                    fileSystemWatcher.EnableRaisingEvents = true;

                    // In case the file had already been created before we started watching it.
                    if (File.Exists(shutdownFile))
                    {
                        shutdownSignalReceived = true;
                    }
                }

                if (!shutdownSignalReceived)
                {
                    BuildContainer();

                    StartDataInitializationAsNeeded();
                    StartSimulator();

                    RunAsync().Wait();
                }
            }
            catch (Exception ex)
            {
                CancellationTokenSource.Cancel();
                Trace.TraceError("Webjob terminating: {0}", ex.ToString());
            }
        }

        static void BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SimulatorModule());
            simulatorContainer = builder.Build();
        }

        static void OnShutdownFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.IndexOf(Path.GetFileName(shutdownFile), StringComparison.OrdinalIgnoreCase) >= 0)
            {
                CancellationTokenSource.Cancel();
            }
        }

        static void CreateInitialDataAsNeeded(object state)
        {
            timer.Dispose();
            if (!CancellationTokenSource.Token.IsCancellationRequested)
            {
                Trace.TraceInformation("Preparing to add initial data");
                var creator = simulatorContainer.Resolve<IDataInitializer>();
                creator.CreateInitialDataIfNeeded();
            }
        }

        static void StartDataInitializationAsNeeded()
        {
            //We have observed that Azure reliably starts the web job twice on a fresh deploy. The second start
            //is reliably about 7 seconds after the first start (under current conditions -- this is admittedly
            //not a perfect solution, but absent visibility into the black box of Azure this is what works at
            //the time) with a shutdown command being received on the current instance in the interim. We want
            //to further bolster our guard against starting a data initialization process that may be aborted
            //in the middle of its work. So we want to delay the data initialization for about 10 seconds to
            //give ourselves the best chance of receiving the shutdown command if it is going to come in. After
            //this delay there is an extremely good chance that we are on a stable start that will remain in place.
            timer = new Timer(CreateInitialDataAsNeeded, null, 10000, Timeout.Infinite);
        }

        static void StartSimulator()
        {
            // Dependencies to inject into the Bulk Device Tester
            var logger = new TraceLogger();
            var configProvider = new ConfigurationProvider();
            var telemetryFactory = new EngineTelemetryFactory(logger, configProvider);

            var serializer = new JsonSerialize();
            var transportFactory = new IotHubTransportFactory(serializer, logger, configProvider);

            IVirtualDeviceStorage deviceStorage;
            var useConfigforDeviceList = Convert.ToBoolean(configProvider.GetConfigurationSettingValueOrDefault("UseConfigForDeviceList", "False"), CultureInfo.InvariantCulture);

            if (useConfigforDeviceList)
            {
                deviceStorage = new AppConfigRepository(configProvider, logger);
            }
            else
            {
                deviceStorage = new VirtualDeviceTableStorage(configProvider);
            }

            IDeviceFactory deviceFactory = new EngineDeviceFactory();

            // Start Simulator
            Trace.TraceInformation("Starting Simulator");
            var tester = new BulkDeviceTester(transportFactory, logger, configProvider, telemetryFactory, deviceFactory, deviceStorage);
            Task.Run(() => tester.ProcessDevicesAsync(CancellationTokenSource.Token), CancellationTokenSource.Token);
        }

        static async Task RunAsync()
        {
            while (!CancellationTokenSource.Token.IsCancellationRequested)
            {
                Trace.TraceInformation("Running");
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), CancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }
    }
}