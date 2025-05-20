using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Reactive.Disposables;
using System;

namespace BalatroSaveToolkit.UI.ViewModels
{
    /// <summary>
    /// ViewModel for the home screen
    /// </summary>
    public class HomeViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Creates a new instance of HomeViewModel
        /// </summary>
        /// <param name="hostScreen">The screen that will host this ViewModel</param>
        /// <param name="navigationService">The navigation service</param>
        public HomeViewModel(IScreen hostScreen, INavigationService navigationService)
            : base(hostScreen)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            Activator = new ViewModelActivator();

            NavigateToSettingsCommand = ReactiveCommand.CreateFromTask(NavigateToSettingsAsync);

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                // Setup activations
                Disposable.Create(() => { /* Cleanup */ }).DisposeWith(disposables);
            });
        }

        /// <summary>
        /// Gets the activator for this ViewModel
        /// </summary>
        public ViewModelActivator Activator { get; }

        /// <summary>
        /// Gets the command to navigate to settings
        /// </summary>
        public ICommand NavigateToSettingsCommand { get; }

        private Task NavigateToSettingsAsync()
        {
            return _navigationService.NavigateToAsync<SettingsViewModel>();
        }
    }
}