using System;
using System.Reactive;
using BalatroSaveToolkit.Core.Services;
using ReactiveUI;
using Avalonia.Styling;
using Splat;

namespace BalatroSaveToolkit.ViewModels
{    /// <summary>
    /// View model for theme selection and management.
    /// </summary>
    class ThemeSettingsViewModel : ViewModelBase
    {
        private readonly IThemeService _themeService;
        private readonly ISettingsService _settingsService;
        private bool _useDarkTheme;
        private bool _followSystemTheme;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeSettingsViewModel"/> class.
    /// </summary>
    /// <param name="themeService">The theme service.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="hostScreen">The screen that will host this ViewModel.</param>
    public ThemeSettingsViewModel(IThemeService themeService, ISettingsService settingsService, IScreen hostScreen)
        : base(hostScreen)
    {
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

        // Initialize properties
        _useDarkTheme = _themeService.CurrentTheme == ThemeVariant.Dark;
        _followSystemTheme = _themeService.FollowSystemTheme;

        // Initialize command
        ApplyThemeCommand = ReactiveCommand.Create(() =>
        {
            ApplyTheme();
            return Unit.Default;
        });

        // Monitor property changes but don't auto-apply
        this.WhenAnyValue(x => x.UseDarkTheme)
            .Subscribe(value =>
            {
                _settingsService.UseDarkTheme = value;
            });

        this.WhenAnyValue(x => x.FollowSystemTheme)
            .Subscribe(value =>
            {
                _settingsService.UseSystemTheme = value;
            });
    }

        /// <summary>
        /// Gets or sets whether to use the dark theme.
        /// </summary>
        public bool UseDarkTheme
        {
            get => _useDarkTheme;
            set => this.RaiseAndSetIfChanged(ref _useDarkTheme, value);
        }

        /// <summary>
        /// Gets or sets whether to follow the system theme.
        /// </summary>
        public bool FollowSystemTheme
        {
            get => _followSystemTheme;
            set => this.RaiseAndSetIfChanged(ref _followSystemTheme, value);
        }

        /// <summary>
        /// Command to apply the theme settings.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ApplyThemeCommand { get; }

        private void ApplyTheme()
        {
            try
            {
                if (_followSystemTheme)
                {
                    _themeService.SetFollowSystemTheme(true);
                }
                else
                {
                    _themeService.SetFollowSystemTheme(false);
                    _themeService.SetTheme(_useDarkTheme);
                }
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme operation error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme argument error: {ex.Message}");
            }
        }
    }
}
