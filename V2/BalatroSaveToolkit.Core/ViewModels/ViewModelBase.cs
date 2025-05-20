using System;
using ReactiveUI;
using Splat;

namespace BalatroSaveToolkit.Core.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels in the application that support navigation
    /// </summary>
    public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel
    {
        /// <summary>
        /// Creates a new instance of ViewModelBase
        /// </summary>
        /// <param name="hostScreen">The screen that will host this ViewModel</param>
        protected ViewModelBase(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? throw new ArgumentNullException(nameof(hostScreen));
        }

        /// <summary>
        /// Gets the screen that is hosting this ViewModel
        /// </summary>
        public IScreen HostScreen { get; }

        /// <summary>
        /// Gets a unique identifier for this instance
        /// </summary>
        public string UrlPathSegment => GetType().Name.Replace("ViewModel", string.Empty, StringComparison.Ordinal);
    }
}
