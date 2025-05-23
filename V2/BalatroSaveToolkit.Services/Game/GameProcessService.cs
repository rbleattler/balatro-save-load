using System;
using System.Threading;
using BalatroSaveToolkit.Core.Services;

namespace BalatroSaveToolkit.Services.Game
{
    /// <summary>
    /// Implementation of the game process service with cross-platform detection.
    /// </summary>
    public class GameProcessService : IGameProcessService, IDisposable
    {
        private Timer? _processCheckTimer;
        private bool _isBalatroRunning;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(2);
        private readonly GameProcessDetector _processDetector;/// <summary>
        /// Event fired when the Balatro process status changes.
        /// </summary>
        public event EventHandler<GameProcessStatusEventArgs> BalatroProcessStatusChanged = delegate { };

        /// <summary>
        /// Gets whether the Balatro process is currently running.
        /// </summary>
        public bool IsBalatroRunning => _isBalatroRunning;        /// <summary>
        /// Initializes a new instance of the <see cref="GameProcessService"/> class.
        /// </summary>
        public GameProcessService()
        {
            _processDetector = new GameProcessDetector();
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
        }        /// <summary>
        /// Disposes resources used by the service.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">Whether this is being called from Dispose() or the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopProcessCheck();
            }
        }        private void CheckProcess(object? state)
        {
            bool isRunning = false;

            try
            {
                isRunning = _processDetector.IsBalatroRunning();

                // Only raise the event if the status has changed
                if (isRunning != _isBalatroRunning)
                {
                    _isBalatroRunning = isRunning;
                    BalatroProcessStatusChanged?.Invoke(this, new GameProcessStatusEventArgs(isRunning));
                }
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception but don't crash the app
                System.Diagnostics.Debug.WriteLine($"Error checking process status: {ex.Message}");
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                // Catch specific exceptions but not critical ones
                System.Diagnostics.Debug.WriteLine($"Unexpected error checking process: {ex.Message}");
            }
        }
    }
}
