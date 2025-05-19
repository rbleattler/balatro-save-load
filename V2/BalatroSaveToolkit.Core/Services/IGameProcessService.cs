using System;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Service interface for game process management.
    /// </summary>
    public interface IGameProcessService
    {
        /// <summary>
        /// Event fired when the Balatro process status changes.
        /// </summary>
        event EventHandler<bool> BalatroProcessStatusChanged;

        /// <summary>
        /// Gets whether the Balatro process is currently running.
        /// </summary>
        bool IsBalatroRunning { get; }

        /// <summary>
        /// Starts checking for the Balatro process.
        /// </summary>
        void StartProcessCheck();

        /// <summary>
        /// Stops checking for the Balatro process.
        /// </summary>
        void StopProcessCheck();
    }
}
