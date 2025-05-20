using Avalonia.ReactiveUI;
using BalatroSaveToolkit.ViewModels;
using ReactiveUI;

namespace BalatroSaveToolkit.Views
{
  /// <summary>
  /// Dashboard view for the application.
  /// </summary>
  internal partial class DashboardView : ReactiveUserControl<DashboardViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardView"/> class.
        /// </summary>
        public DashboardView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // Handle view activation/deactivation here
            });
        }
    }
}
