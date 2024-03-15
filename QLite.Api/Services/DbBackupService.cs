using QLite.Data.CommonContext;
using Serilog;
using System.Diagnostics;

namespace QLiteDataApi.Services
{
    public class DbBackupService : IHostedService, IDisposable
    {
        private Timer _timer = null!;
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        static Process proc = Process.GetCurrentProcess();

        private void DoWork(object? state)
        {
            proc.Refresh();
            Log.Information($"mem usage:{proc.WorkingSet64}");

            var time = CommonCtx.Config.GetSection("DbBackupSettings:Time")?.Value;
            var fileLocation = CommonCtx.Config.GetSection("DbBackupSettings:FileLocation")?.Value;
            var backupLocation = CommonCtx.Config.GetSection("DbBackupSettings:BackupLocation")?.Value;
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

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}
