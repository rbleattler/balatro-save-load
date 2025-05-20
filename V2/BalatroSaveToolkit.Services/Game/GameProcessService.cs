using System;
using System.Runtime.InteropServices;
using System.Threading;
using BalatroSaveToolkit.Core.Services;

namespace BalatroSaveToolkit.Services.Game
{
    /// <summary>
    /// Implementation of the game process service with platform-specific detection.
    /// </summary>
    public class GameProcessService : IGameProcessService
    {
        private Timer _processCheckTimer;
        private bool _isBalatroRunning;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(2);

        private WindowsProcessDetector _windowsDetector;
        private MacOsProcessDetector _macOsDetector;
        private LinuxProcessDetector _linuxDetector;

        /// <summary>
        /// Event fired when the Balatro process status changes.
        /// </summary>
        public event EventHandler<bool> BalatroProcessStatusChanged;

        /// <summary>
        /// Gets whether the Balatro process is currently running.
        /// </summary>
        public bool IsBalatroRunning => _isBalatroRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameProcessService"/> class.
        /// </summary>
        public GameProcessService()
        {
            InitializePlatformDetector();
        }

        /// <summary>
        /// Starts checking for the Balatro process.
        /// </summary>
        public void StartProcessCheck()
        {
            StopProcessCheck(); // Ensure we don't have multiple timers
            _processCheckTimer = new Timer(CheckProcess, null, TimeSpan.Zero, _checkInterval);
        }

        /// <summary>
        /// Stops checking for the Balatro process.
        /// </summary>
        public void StopProcessCheck()
        {
            _processCheckTimer?.Dispose();
            _processCheckTimer = null;
        }

        private void InitializePlatformDetector()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _windowsDetector = new WindowsProcessDetector();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _macOsDetector = new MacOsProcessDetector();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _linuxDetector = new LinuxProcessDetector();
            }
            else
            {
                throw new PlatformNotSupportedException("The current platform is not supported.");
            }
        }

        private void CheckProcess(object state)
        {
            bool isRunning = false;

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    isRunning = _windowsDetector.IsBalatroRunning();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    isRunning = _macOsDetector.IsBalatroRunning();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    isRunning = _linuxDetector.IsBalatroRunning();
                }

                // Only raise the event if the status has changed
                if (isRunning != _isBalatroRunning)
                {
                    _isBalatroRunning = isRunning;
                    BalatroProcessStatusChanged?.Invoke(this, _isBalatroRunning);
                }
            }
            catch (Exception)
            {
                // Ignore exceptions during process checking
                // This ensures that temporary issues don't break the application
            }
        }
    }
}
