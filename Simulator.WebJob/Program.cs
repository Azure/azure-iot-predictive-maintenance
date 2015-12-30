namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Common.Configurations;
    using Common.Execution;
    using Common.Repository;
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
        static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        static IContainer simulatorContainer;
        static Timer _timer;

        static void Main(string[] args)
        {
            try
            {
                BuildContainer();
                simulatorContainer.Resolve<IShutdownFileWatcher>().Run(() =>
                {
                    StartDataInitializationAsNeeded();
                    StartSimulator();
                }, cancellationTokenSource);
            }
            catch (Exception ex)
            {
                cancellationTokenSource.Cancel();
                Trace.TraceError("Webjob terminating: {0}", ex.ToString());
            }
        }

        static void BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SimulatorModule());
            simulatorContainer = builder.Build();
        }

        static void CreateInitialDataAsNeeded(object state)
        {
            _timer.Dispose();
            if (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                Trace.TraceInformation("Preparing to add initial data");
                simulatorContainer.Resolve<IDataInitializer>().CreateInitialDataIfNeeded();
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
            _timer = new Timer(CreateInitialDataAsNeeded, null, 10000, Timeout.Infinite);
        }

        static void StartSimulator()
        {
            // Dependencies to inject into the Bulk Device Tester
            var logger = new TraceLogger();
            var configProvider = new ConfigurationProvider();
            var telemetryFactory = new EngineTelemetryFactory(logger, configProvider);

            var serializer = new JsonSerialize();
            var transportFactory = new IotHubTransportFactory(serializer, logger, configProvider);

            IVirtualDeviceStorage deviceStorage = null;
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
            Task.Run(() => tester.ProcessDevicesAsync(cancellationTokenSource.Token), cancellationTokenSource.Token);
        }
    }
}