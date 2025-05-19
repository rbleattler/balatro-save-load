using System.Threading.Tasks;
using System.Windows.Input;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;

namespace BalatroSaveToolkit.ViewModels
{
    /// <summary>
    /// ViewModel for the main window.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private string _windowTitle = "Balatro Save Toolkit";
        private bool _isGameRunning;

        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string WindowTitle
        {
            get => _windowTitle;
            private set => SetProperty(ref _windowTitle, value);
        }

        /// <summary>
        /// Gets a value indicating whether the game is running.
        /// </summary>
        public bool IsGameRunning
        {
            get => _isGameRunning;
            private set => SetProperty(ref _isGameRunning, value);
        }

        /// <summary>
        /// Gets the command to toggle the theme.
        /// </summary>
        public ICommand ToggleThemeCommand { get; }

        /// <summary>
        /// Gets the command to open the settings dialog.
        /// </summary>
        public ICommand OpenSettingsCommand { get; }

        /// <summary>
        /// Gets the command to open the about dialog.
        /// </summary>
        public ICommand OpenAboutCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            // Initialize commands
            ToggleThemeCommand = CreateCommand(() => ToggleTheme());
            OpenSettingsCommand = CreateCommand(() => OpenSettings());
            OpenAboutCommand = CreateCommand(() => OpenAbout());
        }

        /// <summary>
        /// Initializes the ViewModel.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InitializeAsync()
        {
            // Initialize resources, check game status, etc.
            await Task.CompletedTask;
        }

        private void ToggleTheme()
        {
            // Toggle theme logic will be implemented later
        }

        private void OpenSettings()
        {
            // Open settings dialog logic will be implemented later
        }

        private void OpenAbout()
        {
            // Open about dialog logic will be implemented later
        }
    }
}
