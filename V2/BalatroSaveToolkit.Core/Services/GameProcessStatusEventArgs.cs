using System;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Event arguments for game process status changes.
    /// </summary>
    public class GameProcessStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value indicating whether the game process is running.
        /// </summary>
        public bool IsRunning { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameProcessStatusEventArgs"/> class.
        /// </summary>
        /// <param name="isRunning">Whether the game process is running.</param>
        public GameProcessStatusEventArgs(bool isRunning)
        {
            IsRunning = isRunning;
        }
    }
}
