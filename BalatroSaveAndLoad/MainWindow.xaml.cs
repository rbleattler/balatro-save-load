using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.IO.Compression;
using System.Windows.Threading;
using System.Linq;
using System.Collections.ObjectModel;

namespace BalatroSaveAndLoad {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _status = "Ready";

        public string Status {
            get => _status;
            set
                {
                    if (_status == value) return;
                    _status = value;
                    OnPropertyChanged();
                }
        }

        private string _countdownText = "";

        public string CountdownText {
            get => _countdownText;
            set
                {
                    if (_countdownText == value) return;
                    _countdownText = value;
                    OnPropertyChanged();
                }
        }

        private Visibility _countdownVisibility = Visibility.Collapsed;

        public Visibility CountdownVisibility {
            get => _countdownVisibility;
            set
                {
                    if (_countdownVisibility == value) return;
                    _countdownVisibility = value;
                    OnPropertyChanged();
                }
        }

        private readonly string _directoryPath = Path.Combine(
                                                              Environment.GetFolderPath(
                                                                   Environment.SpecialFolder.ApplicationData
                                                                  ),
                                                              "BalatroSaveAndLoad"
                                                             );

        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly DispatcherTimer _statusResetTimer = new DispatcherTimer();
        private readonly DispatcherTimer _errorCheckTimer = new DispatcherTimer();
        private readonly DispatcherTimer _countdownTimer = new DispatcherTimer();
        private readonly DispatcherTimer _cleanupTimer = new DispatcherTimer();
        private readonly DispatcherTimer _balatroCheckTimer = new DispatcherTimer();

        // Track the current error state
        private bool _hasActiveError = false;
        private string _currentErrorMessage = string.Empty;
        private Func<bool>? _errorConditionChecker = null;

        // For countdown timer
        private DateTime _nextAutoSaveTime;
        private double _autoSaveIntervalMinutes;

        // For auto-cleanup
        private TimeSpan _cleanupTimeSpan = TimeSpan.FromDays(7); // Default to 7 days
        private readonly Dictionary<int, TimeSpan> _cleanupOptions = new Dictionary<int, TimeSpan>();

        private bool _isBalatroRunning = false;

        bool IsBalatroRunning {
            get => _isBalatroRunning;
            set
                {
                    if (_isBalatroRunning == value) return;
                    _isBalatroRunning = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BalatroRunningStatus));
                    OnPropertyChanged(nameof(IsSaveEnabled));
                }
        }

        public string BalatroRunningStatus => IsBalatroRunning ? "Balatro: Running" : "Balatro: Not Running";
        public bool IsSaveEnabled => IsBalatroRunning;

        private DebugWindow? _debugWindow;
        private readonly ObservableCollection<string> _debugLog = [];

        public MainWindow() {
            InitializeComponent();
            DataContext = this;

            Directory.CreateDirectory(_directoryPath); // Ensure the directory exists

            _timer.Tick += Timer_Tick;

            // Configure status reset timer
            _statusResetTimer.Interval = TimeSpan.FromSeconds(5);
            _statusResetTimer.Tick += StatusResetTimer_Tick;

            // Configure error check timer
            _errorCheckTimer.Interval = TimeSpan.FromSeconds(2);
            _errorCheckTimer.Tick += ErrorCheckTimer_Tick;
            _errorCheckTimer.Start(); // Start the error check timer

            // Configure a countdown timer
            _countdownTimer.Interval = TimeSpan.FromSeconds(1);
            _countdownTimer.Tick += CountdownTimer_Tick;

            // Configure cleanup timer
            _cleanupTimer.Interval = TimeSpan.FromHours(1); // Check once per hour
            _cleanupTimer.Tick += CleanupTimer_Tick;

            // Set up Balatro running check timer
            _balatroCheckTimer.Interval = TimeSpan.FromSeconds(2);
            _balatroCheckTimer.Tick += BalatroCheckTimer_Tick;
            _balatroCheckTimer.Start();

            LoadList();

            for (var i = 1; i <= 10; i++) {
                ProfileComboBox.Items.Add($"Profile {i}");
                MinuteComboBox.Items.Add($"{i} minutes");
            }

            // Setup cleanup options
            _cleanupOptions.Add(0, TimeSpan.FromDays(1));
            _cleanupOptions.Add(1, TimeSpan.FromDays(3));
            _cleanupOptions.Add(2, TimeSpan.FromDays(7));
            _cleanupOptions.Add(3, TimeSpan.FromDays(14));
            _cleanupOptions.Add(4, TimeSpan.FromDays(30));

            CleanupTimeComboBox.Items.Add("1 day");
            CleanupTimeComboBox.Items.Add("3 days");
            CleanupTimeComboBox.Items.Add("7 days");
            CleanupTimeComboBox.Items.Add("14 days");
            CleanupTimeComboBox.Items.Add("30 days");

            ProfileComboBox.SelectedIndex = 0;
            MinuteComboBox.SelectedIndex = 0;
            CleanupTimeComboBox.SelectedIndex = 2; // Default to 7 days

            // Enable multiple selection for FileListBox
            FileListBox.SelectionMode = SelectionMode.Extended;

            UpdateStatusDisplay(false);
        }

        void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateStatusDisplay(bool isError) {
            if (isError) {
                MainStatusBar.Background = Brushes.DarkRed;
                MainStatusBar.Foreground = Brushes.White;
            } else {
                MainStatusBar.Background = (Brush)FindResource("StatusBarBackgroundBrush");
                MainStatusBar.Foreground = (Brush)FindResource("StatusBarForegroundBrush");
            }
        }

        private void SetErrorStatus(string errorMessage, Func<bool>? conditionChecker = null) {
            // Set the error state
            _hasActiveError = true;
            _currentErrorMessage = errorMessage;
            _errorConditionChecker = conditionChecker;

            // Update UI
            Status = $"Error: {errorMessage}";
            UpdateStatusDisplay(true);
            FlashWindow();

            // Reset status after a delay only if no condition checker is provided
            if (_errorConditionChecker != null) return;
            _statusResetTimer.Stop();
            _statusResetTimer.Start();
        }

        private void SetSuccessStatus(string message) {
            // Clear any error state
            _hasActiveError = false;
            _currentErrorMessage = string.Empty;
            _errorConditionChecker = null;

            // Update UI
            Status = message;
            UpdateStatusDisplay(false);

            // Reset status after a delay
            _statusResetTimer.Stop();
            _statusResetTimer.Start();
        }

        private void StatusResetTimer_Tick(object? sender, EventArgs e) {
            _statusResetTimer.Stop();

            // Only reset to "Ready" if there's no active error
            if (_hasActiveError) return;
            Status = "Ready";
            UpdateStatusDisplay(false);
        }

        private void ErrorCheckTimer_Tick(object? sender, EventArgs e) {
            // Check if there's an active error with a condition checker
            if (!_hasActiveError ||
                _errorConditionChecker == null)
                return;
            try {
                // Check if the error condition is resolved
                var isResolved = _errorConditionChecker();
                if (!isResolved) return;
                // Error is resolved, clear error state
                _hasActiveError = false;
                _currentErrorMessage = string.Empty;
                _errorConditionChecker = null;
                Status = "Ready";
                UpdateStatusDisplay(false);
            }
            catch {
                // If checking causes an exception, keep the error state
            }
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e) {
            if (AutoCheckBox.IsChecked != true) return;
            var timeLeft = _nextAutoSaveTime - DateTime.Now;
            if (timeLeft.TotalSeconds <= 0) {
                // We've reached the time - timer event will handle the save
                // Just update for next interval
                UpdateNextAutoSaveTime();
            } else { UpdateCountdownDisplay(timeLeft); }
        }

        private void UpdateCountdownDisplay(TimeSpan timeLeft) {
            if (timeLeft.TotalHours >= 1) {
                CountdownText = $"Next auto-save: {timeLeft.Hours}h {timeLeft.Minutes}m {timeLeft.Seconds}s";
            } else if (timeLeft.TotalMinutes >= 1) {
                CountdownText = $"Next auto-save: {timeLeft.Minutes}m {timeLeft.Seconds}s";
            } else { CountdownText = $"Next auto-save: {timeLeft.Seconds}s"; }
        }

        private void UpdateNextAutoSaveTime() { _nextAutoSaveTime = DateTime.Now.AddMinutes(_autoSaveIntervalMinutes); }

        private void FlashWindow() {
            // Flash the window in the taskbar
            FlashWindow(this);
        }

        // P/Invoke to flash the window in the taskbar
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        private static void FlashWindow(Window window) {
            // Get the window handle
            var windowHandle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            // Flash the window in the taskbar
            FlashWindow(windowHandle, true);
        }

        void LoadList() {
            var files = Directory.GetFileSystemEntries(_directoryPath, "*.jkr");
            var fileNames = files.Select(Path.GetFileName).OrderByDescending(file => file);
            FileListBox.ItemsSource = fileNames;
            FileListBox.SelectedIndex = -1; // Clear selection
        }

        string GetCurrentSaveFile() {
            var profileNumber = ProfileComboBox.SelectedIndex + 1;
            return Path.Combine(
                                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                "Balatro",
                                profileNumber.ToString(),
                                "save.jkr"
                               );
        }

        void Save() {
            if (!IsBalatroRunning) {
                SetErrorStatus("Cannot save: Balatro is not running.");
                DebugLog("Save blocked: Balatro not running.");
                return;
            }

            DebugLog("Attempting to save...");
            const string deckNameKey = "[\"BACK\"]={[\"name\"]=\"";
            const string roundKey = "[\"round\"]=";
            var saveFile = GetCurrentSaveFile();
            var profileNumber = ProfileComboBox.SelectedIndex + 1;
            try {
                if (!File.Exists(saveFile)) {
                    SetErrorStatus(
                                   $"Save file not found for Profile {profileNumber}",
                                   () => File.Exists(saveFile) // Condition will be true when file exists
                                  );
                    DebugLog($"Save file not found for Profile {profileNumber}");
                    return;
                }

                using (var compressedStream = new FileStream(saveFile, FileMode.Open, FileAccess.Read))
                    using (var outputStream = new MemoryStream())
                        using (var deflateStream =
                               new DeflateStream(compressedStream, CompressionMode.Decompress)) {
                            deflateStream.CopyTo(outputStream);
                            var decompressedBytes = outputStream.ToArray();
                            var result = Encoding.UTF8.GetString(decompressedBytes);
                            var deckNameStart = result.IndexOf(deckNameKey, StringComparison.Ordinal) +
                                                deckNameKey.Length;
                            var deckNameEnd = result.IndexOf('"', deckNameStart);
                            var deckName = result.Substring(deckNameStart, deckNameEnd - deckNameStart);
                            var roundStart = result.IndexOf(roundKey, StringComparison.Ordinal) + roundKey.Length;
                            var roundEnd = result.IndexOf(',', roundStart);
                            var round = result.Substring(roundStart, roundEnd - roundStart);
                            var time = File.GetLastWriteTime(saveFile);

                            var fileName = $"P{profileNumber} {time:yyyy-MM-dd HH-mm-ss} {deckName} Round {round}.jkr";
                            var filePath = Path.Combine(_directoryPath, fileName);

                            if (!File.Exists(filePath)) {
                                File.Copy(saveFile, filePath, false);
                                LoadList();
                                SetSuccessStatus($"Saved {fileName}");
                                DebugLog($"Saved {fileName}");
                            } else {
                                SetSuccessStatus($"File already exists: {fileName}");
                                DebugLog($"File already exists: {fileName}");
                            }
                        }

                // Reset auto-save countdown if this was an auto-save
                if (AutoCheckBox.IsChecked == true) { UpdateNextAutoSaveTime(); }
            }
            catch (Exception ex) {
                SetErrorStatus(ex.Message);
                DebugLog($"Error during save: {ex.Message}");
            }

            DebugLog("Save completed.");
        }

        void Load() {
            DebugLog("Attempting to load...");
            try {
                var saveFile = GetCurrentSaveFile();
                var selectedItem = FileListBox.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(selectedItem)) {
                    SetErrorStatus("No save file selected");
                    DebugLog("Load failed: No save file selected.");
                    return;
                }

                var filePath = Path.Combine(_directoryPath, selectedItem);

                // Check that the file exists first, then copy it
                if (!File.Exists(filePath)) {
                    SetErrorStatus(
                                   $"The selected file does not exist: {selectedItem}",
                                   () => File.Exists(filePath) // Condition will be true when file exists
                                  );
                    DebugLog($"Load failed: File does not exist: {selectedItem}");
                    return;
                }

                File.Copy(filePath, saveFile, true);
                SetSuccessStatus($"Loaded {selectedItem}");
                DebugLog($"Loaded {selectedItem}");
            }
            catch (Exception ex) {
                SetErrorStatus(ex.Message);
                DebugLog($"Error during load: {ex.Message}");
            }

            DebugLog("Load completed.");
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e) {
            if (IsBalatroRunning) Save();
        }

        private void Load_Button_Click(object sender, RoutedEventArgs e) {
            switch (FileListBox.SelectedItems.Count) {
                case 1: Load(); break;
                case 0:
                    SetErrorStatus("Please select a save file to load");
                    DebugLog("Load failed: No save file selected.");
                    break;
                default:
                    SetErrorStatus("Please select only one save file to load");
                    DebugLog("Load failed: Multiple save files selected.");
                    break;
            }
        }

        private void FileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            LoadButton.IsEnabled = FileListBox.SelectedItems.Count == 1;

            // If an error was about needing to select a file, check if it's resolved
            if (!_hasActiveError ||
                !_currentErrorMessage.Contains("select") ||
                FileListBox.SelectedItems.Count != 1)
                return;
            _hasActiveError = false;
            Status = "Ready";
            UpdateStatusDisplay(false);
        }

        void CheckBox_Checked(object sender, RoutedEventArgs e) { UpdateAutoSave(); }

        static string FormatMinutesLabel(double minutes) { return Math.Abs(minutes - 1) < 0 ? "minutes" : "minute"; }

        void UpdateAutoSave() {
            if (AutoCheckBox.IsChecked == true) {
                var minutes = GetSelectedMinutes();
                if (minutes > 0) {
                    _autoSaveIntervalMinutes = minutes;
                    if (IsBalatroRunning) {
                        _timer.Interval = TimeSpan.FromMinutes(minutes);
                        _timer.Start();
                        UpdateNextAutoSaveTime();
                        _countdownTimer.Start();
                        CountdownVisibility = Visibility.Visible;
                    } else {
                        _timer.Stop();
                        _countdownTimer.Stop();
                        CountdownVisibility = Visibility.Collapsed;
                    }

                    var minutesLabel = FormatMinutesLabel(minutes);
                    SetSuccessStatus($"Auto-save enabled: every {minutes} {minutesLabel}");
                    DebugLog($"Auto-save enabled: every {minutes} {minutesLabel}");
                } else {
                    AutoCheckBox.IsChecked = false;
                    _countdownTimer.Stop();
                    CountdownVisibility = Visibility.Collapsed;
                    SetErrorStatus("Invalid time interval. Please enter a positive number.");
                    DebugLog("Auto-save failed: Invalid time interval.");
                }
            } else {
                _timer.Stop();
                _countdownTimer.Stop();
                CountdownVisibility = Visibility.Collapsed;
                SetSuccessStatus("Auto-save disabled");
                DebugLog("Auto-save disabled.");
            }
        }

        private double GetSelectedMinutes() {
            // If an item is selected from the dropdown list
            if (MinuteComboBox.SelectedIndex >= 0 &&
                MinuteComboBox.SelectedIndex < 10) { return MinuteComboBox.SelectedIndex + 1; }

            // Try to parse custom input
            var input = MinuteComboBox.Text.Trim();

            // Extract number from potential format like "X minutes"
            var match = Regex.Match(input, @"^(\d+\.?\d*)\s*(?:minutes?)?$", RegexOptions.IgnoreCase);
            if (match.Success) {
                if (double.TryParse(match.Groups[1].Value, out var result) &&
                    result > 0) { return result; }
            } else if (double.TryParse(input, out var directResult) &&
                       directResult > 0) { return directResult; }

            return 0; // Invalid input
        }

        private void MinuteComboBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (AutoCheckBox.IsChecked == true) { UpdateAutoSave(); }

            // If there was an error about an invalid time interval, check if it's resolved
            if (!_hasActiveError ||
                !_currentErrorMessage.Contains("time interval"))
                return;
            var minutes = GetSelectedMinutes();
            if (!(minutes > 0)) return;
            _hasActiveError = false;
            Status = "Ready";
            UpdateStatusDisplay(false);
        }

        private void MinuteComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            // Allow only digits, decimal point, and backspace
            var regex = new Regex(@"^[0-9\.]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void MinuteComboBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            // Force update on an Enter key
            UpdateAutoSave();
            e.Handled = true;
        }

        private void Timer_Tick(object? sender, EventArgs e) { Save(); }

        private void MinuteComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (IsInitialized && AutoCheckBox.IsChecked == true) { UpdateAutoSave(); }
        }

        private void BalatroCheckTimer_Tick(object? sender, EventArgs e) {
            var running = IsBalatroProcessRunning();
            IsBalatroRunning = running;

            // If not running, stop autosave timer and hide countdown
            if (!running) {
                _timer.Stop();
                _countdownTimer.Stop();
                CountdownVisibility = Visibility.Collapsed;
            } else if (AutoCheckBox.IsChecked == true) {
                // If running and autosave enabled, ensure timer is running
                _timer.Start();
                _countdownTimer.Start();
                CountdownVisibility = Visibility.Visible;
            }
        }

        private bool IsBalatroProcessRunning() {
            try {
                // Check for process named "balatro" (case-insensitive, without extension)
                return Process.GetProcessesByName("balatro").Length != 0;
            }
            catch { return false; }
        }

        private void OpenSavesFolder_Click(object sender, RoutedEventArgs e) {
            DebugLog("OpenSavesFolder_Click triggered.");
            try {
                Process.Start("explorer.exe", _directoryPath);
                SetSuccessStatus("Opened saves folder");
                DebugLog("Opened saves folder.");
            }
            catch (Exception ex) {
                SetErrorStatus($"Failed to open saves folder: {ex.Message}");
                DebugLog($"Failed to open saves folder: {ex.Message}");
            }
        }

        private void RemoveSave_Click(object sender, RoutedEventArgs e) {
            DebugLog("RemoveSave_Click triggered.");
            DeleteSelectedSaves();
        }

        private void DeleteSelectedSaves() {
            var selectedItems = FileListBox.SelectedItems.Cast<string>().ToList();
            if (selectedItems.Count > 0) {
                var message = selectedItems.Count == 1
                                  ? $"Are you sure you want to delete the selected save?"
                                  : $"Are you sure you want to delete {selectedItems.Count} selected saves?";

                var result = MessageBox.Show(
                                             message,
                                             "Confirm Delete",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning
                                            );

                if (result == MessageBoxResult.Yes) {
                    try {
                        foreach (var selectedFile in selectedItems) {
                            var filePath = Path.Combine(_directoryPath, selectedFile);
                            if (File.Exists(filePath)) {
                                File.Delete(filePath);
                                DebugLog($"Deleted save file: {selectedFile}");
                            } else {
                                SetErrorStatus($"File not found: {selectedFile}");
                                DebugLog($"Delete failed: File not found: {selectedFile}");
                            }
                        }

                        LoadList();
                        SetSuccessStatus($"Deleted {selectedItems.Count} save(s)");
                        DebugLog($"Deleted {selectedItems.Count} save(s)");
                    }
                    catch (Exception ex) {
                        SetErrorStatus($"Error deleting file(s): {ex.Message}");
                        DebugLog($"Error deleting file(s): {ex.Message}");
                    }
                }
            }
        }

        private void FileListBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Delete) { DeleteSelectedSaves(); }
        }

        private void CleanupTimer_Tick(object? sender, EventArgs e) { CleanupOldFiles(); }


        private void CleanupOldFiles() {
            try {
                DebugLog("Starting auto-cleanup check...");

                if (!AutoCleanCheckBox.IsChecked == true) {
                    DebugLog("Auto-cleanup is disabled, skipping");
                    return;
                }

                DebugLog($"Cleaning files older than {_cleanupTimeSpan.TotalDays} days");
                var files = Directory.GetFiles(_directoryPath, "*.jkr");
                DebugLog($"Found {files.Length} total save files to check");

                var cleanedCount = 0;
                var autoSaveCount = 0;

                foreach (var file in files) {
                    var fileName = Path.GetFileName(file);

                    // Check if this is an auto-save by looking for the pattern
                    var isAutoSave = IsAutoSaveFile(fileName);

                    if (isAutoSave) {
                        autoSaveCount++;
                        var creationTime = File.GetCreationTime(file);
                        var now = DateTime.Now;
                        var fileAge = now - creationTime;

                        DebugLog($"Checking auto-save: {fileName}, Age: {fileAge.TotalDays:F1} days");

                        if (fileAge > _cleanupTimeSpan) {
                            File.Delete(file);
                            cleanedCount++;
                            DebugLog($"Auto-cleaned file: {fileName}");
                        } else { DebugLog($"Keeping file: {fileName} (not old enough)"); }
                    }
                }

                DebugLog($"Auto-cleanup summary: {autoSaveCount} auto-saves found, {cleanedCount} deleted");

                if (cleanedCount > 0) {
                    LoadList();
                    SetSuccessStatus($"Auto-cleaned {cleanedCount} old save(s)");
                    DebugLog($"Auto-cleaned {cleanedCount} old save(s)");
                } else { DebugLog("No files needed cleaning"); }
            }
            catch (Exception ex) {
                // Log error but don't display - this happens in the background
                Debug.WriteLine($"Error during auto-cleanup: {ex.Message}");
                DebugLog($"Error during auto-cleanup: {ex.Message}");
                DebugLog($"Stack trace: {ex.StackTrace}");
            }
        }


        private static bool IsAutoSaveFile(string fileName) {
            // An auto-save is identified by the pattern: P{profileNumber} {date-time} {deckName} Round {roundNumber}.jkr
            // We can check if it matches our file naming pattern
            var pattern = @"^P\d+ \d{4}-\d{2}-\d{2} \d{2}-\d{2}-\d{2}.*Round \d+\.jkr$";
            return Regex.IsMatch(fileName, pattern);
        }

        private void AutoCleanCheckBox_Checked(object sender, RoutedEventArgs e) { UpdateAutoClean(); }

        private void AutoCleanCheckBox_Unchecked(object sender, RoutedEventArgs e) { UpdateAutoClean(); }

        private void CleanupTimeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (IsInitialized && AutoCleanCheckBox.IsChecked == true) { UpdateAutoClean(); }
        }

        private void UpdateAutoClean() {
            if (AutoCleanCheckBox.IsChecked == true) {
                if (CleanupTimeComboBox.SelectedIndex >= 0 &&
                    _cleanupOptions.TryGetValue(CleanupTimeComboBox.SelectedIndex, out var selectedTimeSpan)) {
                    _cleanupTimeSpan = selectedTimeSpan;
                    _cleanupTimer.Start();

                    // Run cleanup immediately
                    CleanupOldFiles();

                    SetSuccessStatus($"Auto-clean enabled: files older than {CleanupTimeComboBox.Text}");
                    DebugLog($"Auto-clean enabled: files older than {CleanupTimeComboBox.Text}");
                } else {
                    AutoCleanCheckBox.IsChecked = false;
                    SetErrorStatus("Invalid cleanup time selection");
                    DebugLog("Auto-clean failed: Invalid cleanup time selection.");
                }
            } else {
                _cleanupTimer.Stop();
                SetSuccessStatus("Auto-clean disabled");
                DebugLog("Auto-clean disabled.");
            }
        }

        // Debug logging helper
        private void DebugLog(string message) {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var entry = $"[{timestamp}] {message}";
            _debugLog.Add(entry);
            if (_debugWindow != null) { _debugWindow.AppendLog(entry); }
        }

        // Custom title bar handlers
        private void MinimizeButton_Click(object sender, RoutedEventArgs e) { WindowState = WindowState.Minimized; }

        private void CloseButton_Click(object sender, RoutedEventArgs e) { Close(); }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                // Optional: maximize/restore on double-click
                // WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            } else { DragMove(); }
        }

        // Debug window checkbox handlers
        private void ShowDebugWindowCheckBox_Checked(object sender, RoutedEventArgs e) {
            if (_debugWindow == null) {
                _debugWindow = new DebugWindow();
                _debugWindow.Owner = this;
                _debugWindow.SetLog(_debugLog);
                _debugWindow.Closed += DebugWindow_Closed;
                _debugWindow.Show();
            } else {
                _debugWindow.Show();
                _debugWindow.Activate();
            }
        }

        private void ShowDebugWindowCheckBox_Unchecked(object sender, RoutedEventArgs e) {
            if (_debugWindow != null) {
                _debugWindow.Close();
                _debugWindow = null;
            }
        }

        private void DebugWindow_Closed(object? sender, EventArgs e) {
            ShowDebugWindowCheckBox.IsChecked = false;
            _debugWindow = null;
        }
    }
}