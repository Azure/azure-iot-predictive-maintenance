using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Execution;
using Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob.Processors;

namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.EventProcessor.WebJob
{
    using System.IO;

    public static class Program
    {
        static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        static IContainer eventProcessorContainer;

        static void Main(string[] args)
        {
            try
            {
                BuildContainer();
                eventProcessorContainer.Resolve<IShutdownFileRunner>().Run(StartMLDataProcessorHost, cancellationTokenSource);
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
            builder.RegisterModule(new EventProcessorModule());
            eventProcessorContainer = builder.Build();
        }

        static void StartMLDataProcessorHost()
        {
            Trace.TraceInformation("Starting ML Data Processor");
            eventProcessorContainer.Resolve<IMLDataProcessorHost>().Start(cancellationTokenSource.Token);
        }
    }
}
