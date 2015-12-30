namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Autofac;
    using Common.Execution;
    using Processors;

    public static class Program
    {
        static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        static IContainer eventProcessorContainer;

        static void Main(string[] args)
        {
            try
            {
                BuildContainer();
                eventProcessorContainer
                    .Resolve<IShutdownFileWatcher>()
                    .Run(StartMLDataProcessorHost, CancellationTokenSource);
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
            builder.RegisterModule(new EventProcessorModule());
            eventProcessorContainer = builder.Build();
        }

        static void StartMLDataProcessorHost()
        {
            Trace.TraceInformation("Starting ML Data Processor");
            eventProcessorContainer.Resolve<IMLDataProcessorHost>().Start(CancellationTokenSource.Token);
        }
    }
}