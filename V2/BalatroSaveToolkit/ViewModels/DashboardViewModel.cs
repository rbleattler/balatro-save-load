using System;
using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;

namespace BalatroSaveToolkit.ViewModels
{    /// <summary>
     /// ViewModel for the dashboard page.
     /// </summary>
  internal class DashboardViewModel : PageViewModelBase, IRoutableViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardViewModel"/> class.
        /// </summary>
        /// <param name="hostScreen">The host screen.</param>
        public DashboardViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? throw new ArgumentNullException(nameof(hostScreen));
            UrlPathSegment = "dashboard";
            Title = "Dashboard";
        }

        /// <summary>
        /// Gets the host screen.
        /// </summary>
        public IScreen HostScreen { get; }        /// <summary>
        /// Gets the URL path segment.
        /// </summary>
        public string UrlPathSegment { get; }
    }
}
