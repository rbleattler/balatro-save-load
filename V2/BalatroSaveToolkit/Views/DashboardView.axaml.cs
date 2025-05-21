using Avalonia.ReactiveUI;
using Avalonia.Input;
using Avalonia.Controls;
using BalatroSaveToolkit.ViewModels;
using BalatroSaveToolkit.Views;
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

        private void SaveFilesListBox_DoubleTapped(object? sender, RoutedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is SaveFileViewModel saveFile)
            {
                ViewModel?.ViewSaveContentCommand.Execute(saveFile).Subscribe();
            }
        }
    }
}
