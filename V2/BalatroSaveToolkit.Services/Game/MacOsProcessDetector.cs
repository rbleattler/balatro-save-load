using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BalatroSaveToolkit.Services.Game
{
    /// <summary>
    /// macOS-specific implementation of game process detection.
    /// </summary>
    public class MacOsProcessDetector
    {
        private static readonly string[] KnownProcessNames = { "balatro", "Balatro", "love" };

        /// <summary>
        /// Initializes a new instance of the <see cref="MacOsProcessDetector"/> class.
        /// </summary>
        public MacOsProcessDetector()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new PlatformNotSupportedException("This detector is only supported on macOS.");
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
                // On macOS, we can use the 'ps' command line tool to check for processes
                // This approach works even for GUI applications in App bundles
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
            // Use the 'ps' command to check for processes
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
            // Use the 'ps' command to check command line arguments for LÖVE processes
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "ps",
                Arguments = "-ef | grep -i love | grep -i balatro",
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
