using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BalatroSaveToolkit.Services.Game
{
    /// <summary>
    /// Linux-specific implementation of game process detection.
    /// </summary>
    public class LinuxProcessDetector
    {
        private static readonly string[] KnownProcessNames = { "balatro", "Balatro", "love" };

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxProcessDetector"/> class.
        /// </summary>
        public LinuxProcessDetector()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new PlatformNotSupportedException("This detector is only supported on Linux.");
            }
        }

        /// <summary>
        /// Checks if the Balatro game is running.
        /// </summary>
        /// <returns>True if Balatro is running, false otherwise.</returns>
        public bool IsBalatroRunning()
        {
            try
            {
                // On Linux, we use the 'ps' and 'pgrep' commands to detect processes
                foreach (var processName in KnownProcessNames)
                {
                    if (IsProcessRunning(processName))
                    {
                        // For the LÖVE runtime, we need to check if it's actually running Balatro
                        if (processName.Equals("love", StringComparison.OrdinalIgnoreCase))
                        {
                            if (IsLoveRunningBalatro())
                            {
                                return true;
                            }
                        }
                        else
                        {
                            // Direct match for Balatro process
                            return true;
                        }
                    }
                }

                return false;
            }
            catch
            {
                // Default to not running if detection fails
                return false;
            }
        }

        private bool IsProcessRunning(string processName)
        {
            // Use pgrep to check for matching processes
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "pgrep",
                Arguments = $"-i {processName}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return !string.IsNullOrEmpty(output);
        }

        private bool IsLoveRunningBalatro()
        {
            // Check command line arguments for LÖVE processes for Balatro
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = "-c \"ps -ef | grep -i love | grep -i balatro | grep -v grep\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return !string.IsNullOrEmpty(output) && output.IndexOf("balatro", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
