using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.ViewModels;
using ReactiveUI;
using Splat;

namespace BalatroSaveToolkit.Views
{
    /// <summary>
    /// View for theme settings.
    /// </summary>
    partial class ThemeSettingsView : ReactiveUserControl<ThemeSettingsViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeSettingsView"/> class.
        /// </summary>
        public ThemeSettingsView()
        {
            InitializeComponent();

            // Create view model if not provided
            if (DataContext == null)
            {
                var themeService = Locator.Current.GetService<IThemeService>();
                var settingsService = Locator.Current.GetService<ISettingsService>();                if (themeService != null && settingsService != null)
                {
                    // Get the host screen
                    var hostScreen = Locator.Current.GetService<IScreen>();
                    if (hostScreen != null)
                    {
                        DataContext = new ThemeSettingsViewModel(themeService, settingsService, hostScreen);
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
