using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace PiStellwerk.BackgroundServices
{
    /// <summary>
    /// The Hardware Status Service 
    /// </summary>
    public class HardwareStatusService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("HardwareStatusService is starting.");

            stoppingToken.Register(() => Console.WriteLine("HardwareStatusService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        PrintLinuxHardwareStatus();
                    }

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        PrintWindowsHardwareStatus();
                    }

                    //Console.WriteLine($"HardwareStatusService: {Math.Round(usagePercent)}% Memory usage (total:{stats.Item1 / 1024} MB / available:{stats.Item2 / 1024} MB)");
                }

                catch (Exception e)
                {
                    Console.WriteLine("HardwareStatusService has crashed: " + e.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }

            Console.WriteLine("HardwareStatusService background task is stopping.");
        }

        private void PrintLinuxHardwareStatus()
        {
            using (var stream = new FileStream("/proc/meminfo", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    var line = reader.ReadLine();
                    var totalMemory = long.Parse(line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
                    _ = reader.ReadLine();
                    line = reader.ReadLine();
                    var availableMemory = long.Parse(line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
                    double memoryPercent = (totalMemory - availableMemory) * 100 / totalMemory;
                    Console.WriteLine($"Memory Usage: {Math.Round(memoryPercent)}% total:{totalMemory / 1000}MB available:{availableMemory / 1000}MB");
                }
            }

            using (var stream = new FileStream("/sys/class/thermal/thermal_zone0/temp", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    var line = long.Parse(reader.ReadLine());
                    Console.WriteLine($"Temperature: {Math.Round(line/1000d,1)}C°");
                }
            }
        }

        private void PrintWindowsHardwareStatus()
        {
            string output;

            var info = new ProcessStartInfo
            {
                FileName = "wmic",
                Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
                RedirectStandardOutput = true
            };

            using var process = Process.Start(info);
            output = process.StandardOutput.ReadToEnd();
            

            var lines = output.Trim().Split("\n");
            var freeMemory = long.Parse(lines[0].Split("=").Last());
            var totalMemory = long.Parse(lines[1].Split('=').Last());

            double memoryPercent = (totalMemory - freeMemory) * 100  / totalMemory;
            Console.WriteLine($"Memory Usage: {Math.Round(memoryPercent)}% total:{totalMemory / 1000}MB available:{freeMemory / 1000}MB");
        }
    }
}
