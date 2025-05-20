using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;
using Splat;

namespace BalatroSaveToolkit.ViewModels
{
    /// <summary>
    /// ViewModel for the dashboard page.
    /// </summary>
    internal class DashboardViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        private readonly IGameProcessService? _gameProcessService;
        private readonly IFileSystemService? _fileSystemService;
        private readonly ILoggingService? _loggingService;
        private readonly INotificationService? _notificationService;

        private bool _isGameRunning;
        private string _gameVersion = "Unknown";
        private ObservableCollection<SaveFileViewModel> _saveFiles = new();
        private SaveFileViewModel? _selectedSaveFile;
        private string _title = "Dashboard";

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardViewModel"/> class.
        /// </summary>
        /// <param name="hostScreen">The host screen.</param>
        public DashboardViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? throw new ArgumentNullException(nameof(hostScreen));

            // Get services from locator
            _gameProcessService = Locator.Current.GetService<IGameProcessService>();
            _fileSystemService = Locator.Current.GetService<IFileSystemService>();
            _loggingService = Locator.Current.GetService<ILoggingService>();
            _notificationService = Locator.Current.GetService<INotificationService>();

            UrlPathSegment = "dashboard";
            Activator = new ViewModelActivator();

            // Commands
            RefreshSaveFilesCommand = ReactiveCommand.CreateFromTask(RefreshSaveFilesAsync);
            CreateBackupCommand = ReactiveCommand.CreateFromTask(CreateBackupAsync,
                this.WhenAnyValue(x => x.SelectedSaveFile).Select(x => x != null));
            LoadSaveCommand = ReactiveCommand.CreateFromTask(LoadSaveAsync,
                this.WhenAnyValue(x => x.SelectedSaveFile).Select(x => x != null));

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                // Subscribe to game process status changes
                if (_gameProcessService != null)
                {
                    _gameProcessService.BalatroProcessStatusChanged += (sender, isRunning) =>
                    {
                        IsGameRunning = isRunning;
                        GameVersion = isRunning ? "Running" : "Not running";
                        _loggingService?.Info($"Game status changed: {(isRunning ? "Running" : "Not running")}");
                    };

                    // Check current status
                    IsGameRunning = _gameProcessService.IsBalatroRunning;
                    GameVersion = IsGameRunning ? "Running" : "Not running";
                }

                // Initial refresh of save files
                if (RefreshSaveFilesCommand != null)
                {
                    RefreshSaveFilesCommand.Execute().Subscribe().DisposeWith(disposables);
                }
            });
        }

        /// <summary>
        /// Gets the host screen.
        /// </summary>
        public IScreen HostScreen { get; }

        /// <summary>
        /// Gets the URL path segment.
        /// </summary>
        public string UrlPathSegment { get; }

        /// <summary>
        /// Gets the view model activator.
        /// </summary>
        public ViewModelActivator Activator { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the game is running.
        /// </summary>
        public bool IsGameRunning
        {
            get => _isGameRunning;
            set => this.RaiseAndSetIfChanged(ref _isGameRunning, value);
        }

        /// <summary>
        /// Gets or sets the game version.
        /// </summary>
        public string GameVersion
        {
            get => _gameVersion;
            set => this.RaiseAndSetIfChanged(ref _gameVersion, value);
        }

        /// <summary>
        /// Gets or sets the collection of save files.
        /// </summary>
        public ObservableCollection<SaveFileViewModel> SaveFiles
        {
            get => _saveFiles;
            set => this.RaiseAndSetIfChanged(ref _saveFiles, value);
        }
          /// <summary>
        /// Gets or sets the selected save file.
        /// </summary>
        public SaveFileViewModel? SelectedSaveFile
        {
            get => _selectedSaveFile;
            set => this.RaiseAndSetIfChanged(ref _selectedSaveFile, value);
        }
          /// <summary>
        /// Gets the command to refresh save files.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? RefreshSaveFilesCommand { get; }

        /// <summary>
        /// Gets the command to create a backup of the selected save file.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? CreateBackupCommand { get; }

        /// <summary>
        /// Gets the command to load the selected save file.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? LoadSaveCommand { get; }        private async Task RefreshSaveFilesAsync()
        {
            if (_fileSystemService == null)
                return;

            SaveFiles.Clear();

            try
            {
                var backupFiles = await _fileSystemService.GetSavedBackupFilesAsync().ConfigureAwait(false);

                if (backupFiles != null)
                {
                    foreach (var file in backupFiles)
                    {
                        var saveInfo = new SaveFileViewModel
                        {
                            FilePath = file,
                            Timestamp = File.GetLastWriteTime(file),
                            ProfileNumber = DetermineProfileNumber(file),
                            FileSize = new FileInfo(file).Length
                        };

                        SaveFiles.Add(saveInfo);
                    }
                }

                _loggingService?.Info($"Found {SaveFiles.Count} save files");
            }
            catch (IOException ex)
            {
                _loggingService?.Error("IO Error loading save files", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                _loggingService?.Error("Access error loading save files", ex);
            }
            catch (ArgumentException ex)
            {
                _loggingService?.Error("Invalid argument when loading save files", ex);
            }
            catch (System.Security.SecurityException ex)
            {
                _loggingService?.Error("Security exception when loading save files", ex);
            }
        }        private async Task CreateBackupAsync()
        {
            if (SelectedSaveFile == null || _fileSystemService == null)
                return;

            try
            {
                string backupPath = await Task.Run(() =>
                {
                    return _fileSystemService.BackupSaveFile(SelectedSaveFile.ProfileNumber);
                }).ConfigureAwait(false);

                _loggingService?.Info($"Backup created successfully at {backupPath}");
                _notificationService?.ShowSuccess("Backup Created",
                    $"Profile {SelectedSaveFile.ProfileNumber} save file backed up successfully.");

                await RefreshSaveFilesAsync().ConfigureAwait(false);
            }
            catch (IOException ex)
            {
                _loggingService?.Error("IO Error creating backup", ex);
                _notificationService?.ShowError("Backup Failed",
                    $"Failed to backup profile {SelectedSaveFile.ProfileNumber}: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                _loggingService?.Error("Access error creating backup", ex);
                _notificationService?.ShowError("Access Denied",
                    $"Access denied when backing up profile {SelectedSaveFile.ProfileNumber}: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                _loggingService?.Error("Invalid argument when creating backup", ex);
                _notificationService?.ShowError("Invalid Argument",
                    $"Invalid argument when backing up profile {SelectedSaveFile.ProfileNumber}: {ex.Message}");
            }
            catch (System.Security.SecurityException ex)
            {
                _loggingService?.Error("Security exception when creating backup", ex);
                _notificationService?.ShowError("Security Error",
                    $"Security error when backing up profile {SelectedSaveFile.ProfileNumber}: {ex.Message}");
            }
        }

        private async Task LoadSaveAsync()
        {
            if (SelectedSaveFile == null || _fileSystemService == null)
                return;            // Get confirmation before proceeding
            bool confirmed = _notificationService != null && await _notificationService.ShowConfirmationAsync(
                "Confirm Restore",
                $"Are you sure you want to restore the save file to Profile {SelectedSaveFile.ProfileNumber}?\n" +
                "This will overwrite your current save data in that profile.").ConfigureAwait(false);

            if (!confirmed)
            {
                _loggingService?.Info("Save restoration cancelled by user");
                return;
            }

            try
            {
                bool success = await Task.Run(() =>
                {
                    return _fileSystemService.RestoreSaveFile(SelectedSaveFile.FilePath, SelectedSaveFile.ProfileNumber);
                }).ConfigureAwait(false);

                if (success)
                {
                    _loggingService?.Info($"Save file restored successfully to profile {SelectedSaveFile.ProfileNumber}");
                    _notificationService?.ShowSuccess("Save Restored",
                        $"Save file restored successfully to profile {SelectedSaveFile.ProfileNumber}.");
                }
                else
                {
                    _loggingService?.Warning($"Failed to restore save file to profile {SelectedSaveFile.ProfileNumber}");
                    _notificationService?.ShowWarning("Restore Failed",
                        $"Failed to restore save file to profile {SelectedSaveFile.ProfileNumber}.");
                }
            }
            catch (IOException ex)
            {
                _loggingService?.Error("IO Error loading save file", ex);
                _notificationService?.ShowError("Restore Failed",
                    $"IO Error restoring save file to profile {SelectedSaveFile.ProfileNumber}: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                _loggingService?.Error("Access error loading save file", ex);
                _notificationService?.ShowError("Access Denied",
                    $"Access denied when restoring save file to profile {SelectedSaveFile.ProfileNumber}: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                _loggingService?.Error("Invalid argument when loading save file", ex);
                _notificationService?.ShowError("Invalid Argument",
                    $"Invalid argument when restoring save file: {ex.Message}");
            }
            catch (System.Security.SecurityException ex)
            {
                _loggingService?.Error("Security exception when loading save file", ex);
                _notificationService?.ShowError("Security Error",
                    $"Security error when restoring save file: {ex.Message}");
            }
        }

        private static int DetermineProfileNumber(string filePath)
        {
            // Extract profile number from filename, default to 1 if not found
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName.Contains("profile", StringComparison.OrdinalIgnoreCase))
            {
                var profilePart = fileName.Split('_')
                    .FirstOrDefault(p => p.StartsWith("profile", StringComparison.OrdinalIgnoreCase));

                if (profilePart != null && profilePart.Length > 7)
                {
                    ReadOnlySpan<char> profileNumSpan = profilePart.AsSpan(7);
                    if (int.TryParse(profileNumSpan, out int profileNum))
                    {
                        return profileNum;
                    }
                }
            }

            return 1; // Default to profile 1
        }
    }
}
