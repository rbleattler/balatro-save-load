using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BalatroSaveToolkit.UI.ViewModels
{
    /// <summary>
    /// ViewModel for the settings screen
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Creates a new instance of SettingsViewModel
        /// </summary>
        /// <param name="hostScreen">The screen that will host this ViewModel</param>
        /// <param name="navigationService">The navigation service</param>
        /// <param name="settingsService">The settings service</param>
        public SettingsViewModel(
            IScreen hostScreen,
            INavigationService navigationService,
            ISettingsService settingsService)
            : base(hostScreen)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

            Activator = new ViewModelActivator();
            NavigateBackCommand = ReactiveCommand.CreateFromTask(_navigationService.NavigateBackAsync);

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
        /// Gets the command to navigate back
        /// </summary>
        public ICommand NavigateBackCommand { get; }
    }
}