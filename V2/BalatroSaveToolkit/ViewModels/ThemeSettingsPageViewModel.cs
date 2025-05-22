using ReactiveUI;
using BalatroSaveToolkit.Core.Services;

namespace BalatroSaveToolkit.ViewModels
{    /// <summary>
    /// A routable ViewModel wrapper for ThemeSettingsViewModel.
    /// </summary>
    internal sealed class ThemeSettingsPageViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the ThemeSettingsViewModel that contains the actual functionality.
        /// </summary>
        public ThemeSettingsViewModel Content { get; }        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeSettingsPageViewModel"/> class.
        /// </summary>
        /// <param name="hostScreen">The screen that will host this ViewModel.</param>
        /// <param name="themeService">The theme service.</param>
        /// <param name="settingsService">The settings service.</param>
        public ThemeSettingsPageViewModel(IScreen hostScreen, IThemeService themeService, ISettingsService settingsService)
            : base(hostScreen)
        {
            Content = new ThemeSettingsViewModel(themeService, settingsService, hostScreen);
        }
    }
}
