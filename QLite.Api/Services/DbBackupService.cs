using QLiteDataApi.Context;
using Serilog;
using System.Diagnostics;

namespace QLiteDataApi.Services
{
    /// <summary>
    /// A service that periodically backs up the database to a specified location,
    /// ensuring only a fixed number of recent backups are retained.
    /// </summary>
    public class DbBackupService : IHostedService, IDisposable
    {
        private Timer _timer = null!;

        /// <summary>
        /// Initializes the timer and starts the periodic backup process when the service starts.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that can be used to stop the service.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        static Process proc = Process.GetCurrentProcess();

        /// <summary>
        /// Initializes the timer and starts the periodic backup process when the service starts.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that can be used to stop the service.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private void DoWork(object? state)
        {
            proc.Refresh();
            Log.Information($"mem usage:{proc.WorkingSet64}");

            var time = ApiContext.Config.GetSection("DbBackupSettings:Time")?.Value;
            var fileLocation = ApiContext.Config.GetSection("DbBackupSettings:FileLocation")?.Value;
            var backupLocation = ApiContext.Config.GetSection("DbBackupSettings:BackupLocation")?.Value;
            if (time == DateTime.Now.ToString("HH"))
            {
                if (!Directory.Exists(backupLocation))
                {
                    Log.Information($"Creating backup folder {backupLocation}");

                    Directory.CreateDirectory(backupLocation);
                }
                CopyFile(fileLocation, backupLocation);
            }
        }

        /// <summary>
        /// Copies the database file to the backup location with a timestamp. If more than a specified number 
        /// of backups exist, the oldest backups are deleted to maintain the limit.
        /// </summary>
        /// <param name="fileLocation">The location of the original database file.</param>
        /// <param name="backupLocation">The directory where the backups should be stored.</param>
        private void CopyFile(string fileLocation, string backupLocation)
        {
            string[] filePaths = Directory.GetFiles(fileLocation, "QLite.db");
            foreach (var file in filePaths)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                var d = DateTime.Now.ToString("d").Replace("/", "");
                string str = Path.Combine(backupLocation, $"{filename}_{d}.db");
                if (!File.Exists(str))
                {
                    File.Copy(file, str, false);
                    Log.Information($"Creating backup {fileLocation}");
                }
            }

            var removeFiles = Directory.GetFiles(backupLocation)?.Select(f => new FileInfo(f))?.OrderByDescending(c => c.CreationTime)?.ToList();
            while (removeFiles.Count > 5)
            {
                File.Delete(removeFiles[removeFiles.Count - 1].FullName);
                removeFiles.RemoveAt(removeFiles.Count - 1);
            }
        }

        /// <summary>
        /// Stops the timer and the periodic backup process when the service is stopped.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that can be used to stop the service.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes of resources used by the service, specifically the timer.
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}
