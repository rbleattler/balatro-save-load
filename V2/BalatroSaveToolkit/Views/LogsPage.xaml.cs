using BalatroSaveToolkit.Models;
using BalatroSaveToolkit.ViewModels;
using CommunityToolkit.Maui.Views;

namespace BalatroSaveToolkit.Views
{
    public partial class LogsPage : ContentPage
    {
        public LogsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is LogsViewModel vm)
            {
                vm.LoadLogsAsync();
            }
        }

        private void OnLogItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is ActivityLogItem item)
            {
                // Show the log details popup
                var popup = new LogDetailPopup(item);
                this.ShowPopup(popup);
            }
        }
    }
}
