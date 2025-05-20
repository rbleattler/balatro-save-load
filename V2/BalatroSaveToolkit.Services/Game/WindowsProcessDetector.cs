using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace BalatroSaveToolkit.Services.Game
{
    /// <summary>
    /// Windows-specific implementation of game process detection.
    /// </summary>
    public class WindowsProcessDetector
    {
        private static readonly string[] KnownProcessNames = { "balatro", "balatro.exe", "love", "lovec" };

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsProcessDetector"/> class.
        /// </summary>
        public WindowsProcessDetector()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException("This detector is only supported on Windows.");
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
                var processes = Process.GetProcesses();

                // Check for known process names
                foreach (var processName in KnownProcessNames)
                {
                    var matchingProcesses = processes.Where(p =>
                        p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase) ||
                        p.ProcessName.Equals(System.IO.Path.GetFileNameWithoutExtension(processName),
                                          StringComparison.OrdinalIgnoreCase)).ToList();

                    if (matchingProcesses.Any())
                    {
                        // For LÖVE engine processes, check if they're running Balatro
                        if (processName.Equals("love", StringComparison.OrdinalIgnoreCase) ||
                            processName.Equals("lovec", StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (var process in matchingProcesses)
                            {
                                try
                                {
                                    // Check command line arguments or window title for Balatro
                                    if (IsLoveProcessRunningBalatro(process))
                                    {
                                        return true;
                                    }
                                }
                                catch
                                {
                                    // Ignore issues checking specific processes
                                }
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

        private bool IsLoveProcessRunningBalatro(Process process)
        {
            try
            {
                // Check window title
                if (!string.IsNullOrEmpty(process.MainWindowTitle) &&
                    process.MainWindowTitle.IndexOf("balatro", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }

                // Additional checks could be added here
                // For example, we could check command line arguments but that requires additional privileges

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
