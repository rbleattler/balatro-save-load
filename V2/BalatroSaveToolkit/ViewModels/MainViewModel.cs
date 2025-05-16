using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BalatroSaveToolkit.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _welcomeMessage;

        public MainViewModel()
        {
            Title = "Balatro Save Toolkit";
            WelcomeMessage = "Welcome to Balatro Save Toolkit!";
        }

        [RelayCommand]
        private async Task Initialize()
        {
            // Future initialization logic will go here
            await Task.CompletedTask;
        }
    }
}
