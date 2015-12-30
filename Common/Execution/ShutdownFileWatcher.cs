namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Execution
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class ShutdownFileWatcher : IShutdownFileWatcher
    {
        const string SHUTDOWN_FILE_ENV_VAR = "WEBJOBS_SHUTDOWN_FILE";
        string _shutdownFile;
        CancellationTokenSource _cancelTokenSource;

        public void Run(Action start, CancellationTokenSource cancellationTokenSource)
        {
            _cancelTokenSource = cancellationTokenSource;

            // Cloud deploys often get staged and started to warm them up, then get a shutdown
            // signal from the framework before being moved to the production slot. We don't want
            // to start initializing data if we have already gotten the shutdown message, so we'll
            // monitor it. This environment variable is reliable
            // http://blog.amitapple.com/post/2014/05/webjobs-graceful-shutdown/#.VhVYO6L8-B4
            _shutdownFile = Environment.GetEnvironmentVariable(SHUTDOWN_FILE_ENV_VAR);
            bool shutdownSignalReceived = false;

            // Setup a file system watcher on that file's directory to know when the file is created
            // First check for null, though. This does not exist on a localhost deploy, only cloud
            if (!string.IsNullOrWhiteSpace(_shutdownFile))
            {
                var fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(_shutdownFile));
                fileSystemWatcher.Created += OnShutdownFileChanged;
                fileSystemWatcher.Changed += OnShutdownFileChanged;
                fileSystemWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite;
                fileSystemWatcher.IncludeSubdirectories = false;
                fileSystemWatcher.EnableRaisingEvents = true;

                // In case the file had already been created before we started watching it.
                if (File.Exists(_shutdownFile))
                {
                    shutdownSignalReceived = true;
                }
            }

            if (!shutdownSignalReceived)
            {
                start();

                RunAsync().Wait();
            }
        }

        void OnShutdownFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.IndexOf(Path.GetFileName(_shutdownFile), StringComparison.OrdinalIgnoreCase) >= 0)
            {
                _cancelTokenSource.Cancel();
            }
        }

        async Task RunAsync()
        {
            while (!_cancelTokenSource.Token.IsCancellationRequested)
            {
                Trace.TraceInformation("Running");
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), _cancelTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }
    }
}