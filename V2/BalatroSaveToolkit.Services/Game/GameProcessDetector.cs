using System;
using System.Diagnostics;
using System.Linq;

namespace BalatroSaveToolkit.Services.Game
{
    /// <summary>
    /// Cross-platform implementation of game process detection using System.Diagnostics.Process.
    /// </summary>
    public class GameProcessDetector
    {
        private static readonly string[] KnownProcessNames = { "balatro", "love", "lovec" };
        private static readonly int CurrentProcessId = Environment.ProcessId;

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
                        p.Id != CurrentProcessId && // Don't detect our own process
                        !IsOwnToolkitProcess(p) && // Don't detect other instances of our toolkit
                        (p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase) ||
                         p.ProcessName.Equals(System.IO.Path.GetFileNameWithoutExtension(processName),
                                              StringComparison.OrdinalIgnoreCase))).ToList();

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
                                    // Check if this LÖVE process is running Balatro
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

        /// <summary>
        /// Checks if a process is our own toolkit to avoid false positives.
        /// </summary>
        /// <param name="process">The process to check.</param>
        /// <returns>True if this is our toolkit process, false otherwise.</returns>
        private static bool IsOwnToolkitProcess(Process process)
        {
            try
            {
                // Check if the process name contains indicators of our toolkit
                var processName = process.ProcessName.ToLowerInvariant();
                return processName.Contains("balatrosave") ||
                       processName.Contains("toolkit") ||
                       (!string.IsNullOrEmpty(process.MainWindowTitle) &&
                        process.MainWindowTitle.IndexOf("BalatroSaveToolkit", StringComparison.OrdinalIgnoreCase) >= 0);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a LÖVE process is running Balatro by examining its window title.
        /// </summary>
        /// <param name="process">The LÖVE process to check.</param>
        /// <returns>True if the process is running Balatro, false otherwise.</returns>
        private static bool IsLoveProcessRunningBalatro(Process process)
        {
            try
            {
                // Check window title for Balatro
                if (!string.IsNullOrEmpty(process.MainWindowTitle) &&
                    process.MainWindowTitle.IndexOf("balatro", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
