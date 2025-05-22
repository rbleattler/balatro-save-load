using System;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using BalatroSaveToolkit.Core.Services;
using Splat;

namespace BalatroSaveToolkit.ViewModels
{
    /// <summary>
    /// ViewModel for viewing the content of a save file.
    /// </summary>
    public class SaveContentViewModel : ReactiveObject
    {
        [Reactive]
        public string FilePath { get; set; } = string.Empty;

        [Reactive]
        public string RawContent { get; set; } = string.Empty;

        [Reactive]
        public string DisplayContent { get; set; } = string.Empty;

        [Reactive]
        public bool IsLoading { get; set; }

        [Reactive]
        public string? ErrorMessage { get; set; }

        [Reactive]
        public string StatusInfo { get; set; } = string.Empty;

        [Reactive]
        public bool HasContent { get; set; }

        [Reactive]
        public SaveFileParser.GameStatistics? GameStats { get; set; }

        [Reactive]
        public bool IsStatisticsViewActive { get; set; }

        // This shouldn't be marked as [Reactive] since it has no setter and is derived from another property
        public bool HasGameStats => GameStats != null;

        private readonly IFileSystemService _fileSystemService;
        private readonly SaveFileParser _saveFileParser;

        public ReactiveCommand<Unit, Unit> LoadContentCommand { get; }
        public ReactiveCommand<Unit, Unit> CopyContentCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleViewCommand { get; }

        // Static readonly field for line separators to avoid repeated array allocations
        private static readonly char[] LineSeparators = { '\n', '\r' };

        public SaveContentViewModel()
        {
            _fileSystemService = Locator.Current.GetService<IFileSystemService>()!;
            _saveFileParser = new SaveFileParser();
            LoadContentCommand = ReactiveCommand.CreateFromTask(LoadContentAsync);
            CopyContentCommand = ReactiveCommand.Create(CopyContentToClipboard);
            ToggleViewCommand = ReactiveCommand.Create(ToggleView);
        }

        private void ToggleView()
        {
            IsStatisticsViewActive = !IsStatisticsViewActive;
            UpdateStatusInfo();
        }

        private async Task LoadContentAsync()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                ErrorMessage = "No file selected.";
                return;
            }
            IsLoading = true;
            ErrorMessage = null;
            GameStats = null;  // Clear previous stats
            IsStatisticsViewActive = false;  // Default to raw view when loading new content

            try
            {
                RawContent = await _fileSystemService.GetSaveFileContentAsync(FilePath).ConfigureAwait(false);
                DisplayContent = FormatLuaContent(RawContent);
                HasContent = !string.IsNullOrWhiteSpace(DisplayContent);

                // Parse game statistics
                try
                {
                    GameStats = await _saveFileParser.ParseAsync(RawContent).ConfigureAwait(false);

                    // Set profile number if detected from file path
                    if (GameStats.ProfileNumber == 0)
                    {
                        GameStats.ProfileNumber = SaveFileParser.DetermineProfileNumber(FilePath);
                    }

                    // Add file metadata
                    if (GameStats.Metadata == null)
                    {
                        GameStats.Metadata = new System.Collections.Generic.Dictionary<string, string>();
                    }
                    GameStats.Metadata["File Path"] = FilePath;
                    GameStats.Metadata["File Size"] = $"{RawContent.Length:N0} bytes";
                    GameStats.Timestamp = DateTime.Now; // Set current timestamp if none was found in the file

                    // Auto-switch to statistics view if we have valid stats
                    if (GameStats.Jokers.Count > 0 || GameStats.Cards.Count > 0)
                    {
                        IsStatisticsViewActive = true;
                    }
                }
                catch (FormatException ex)
                {
                    // Don't fail the whole operation if parsing fails, just log the error
                    GameStats = null;
                    // Add the error to status info but don't show it as the main error
                    ErrorMessage = $"Failed to parse game statistics: {ex.Message}";
                }
                catch (ArgumentException ex)
                {
                    GameStats = null;
                    ErrorMessage = $"Failed to parse game statistics: {ex.Message}";
                }

                UpdateStatusInfo();
            }
            catch (System.IO.IOException ex)
            {
                ErrorMessage = $"Failed to load save content: {ex.Message}";
                HasContent = false;
                GameStats = null;
            }
            catch (UnauthorizedAccessException ex)
            {
                ErrorMessage = $"Access denied: {ex.Message}";
                HasContent = false;
                GameStats = null;
            }
            // For specific security exceptions
            catch (System.Security.SecurityException ex)
            {
                ErrorMessage = $"Security error: {ex.Message}";
                HasContent = false;
                GameStats = null;
            }
            // For all other exceptions - we need this to handle potentially unknown errors
            catch (Exception ex) when (
                // Filter to exclude exceptions we've already caught
                !(ex is System.IO.IOException) &&
                !(ex is UnauthorizedAccessException) &&
                !(ex is System.Security.SecurityException))
            {
                // Log the exception or send to a logging service
                System.Diagnostics.Debug.WriteLine($"Unexpected error in LoadContentAsync: {ex}");
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                HasContent = false;
                GameStats = null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateStatusInfo()
        {
            if (IsStatisticsViewActive)
            {
                StatusInfo = HasGameStats ? "Game statistics view active" : "No game statistics available";
                return;
            }

            if (string.IsNullOrWhiteSpace(DisplayContent))
            {
                StatusInfo = "No content available";
                return;
            }

            var lineCount = DisplayContent.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries).Length;
            StatusInfo = $"Lua dictionary format - {lineCount} lines, {DisplayContent.Length} characters";
        }

        private void CopyContentToClipboard()
        {
            // In Avalonia, clipboard operations are handled in the view code-behind
            // This is a placeholder that will trigger the command binding
            HasContent = !string.IsNullOrWhiteSpace(DisplayContent);
        }        /// <summary>
        /// Formats Lua-style dictionary content for better readability
        /// </summary>
        private static string FormatLuaContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            var luaSource = content.Trim();
            if (luaSource.StartsWith("return", StringComparison.OrdinalIgnoreCase))
            {
                var returnStatementEnd = "return".Length;
                while (returnStatementEnd < luaSource.Length && char.IsWhiteSpace(luaSource[returnStatementEnd]))
                {
                    returnStatementEnd++;
                }
                luaSource = luaSource[returnStatementEnd..];
            }

            var sb = new StringBuilder();
            var indentLevel = 0;
            var i = 0;
            var inString = false;
            var stringDelim = '\0';
            var lastCharWasNewline = true;

            while (i < luaSource.Length)
            {
                var c = luaSource[i];

                if (inString)
                {
                    sb.Append(c);
                    if (c == stringDelim)
                    {
                        if (i == 0 || luaSource[i - 1] != '\\' || (i > 1 && luaSource[i - 2] == '\\'))
                        {
                            inString = false;
                        }
                    }
                    i++;
                    lastCharWasNewline = false;
                    continue;
                }

                switch (c)
                {
                    case '{':
                    case '[':
                        sb.Append(c);
                        lastCharWasNewline = false;

                        var nextTokenIdx = i + 1;
                        while (nextTokenIdx < luaSource.Length && char.IsWhiteSpace(luaSource[nextTokenIdx]))
                        {
                            nextTokenIdx++;
                        }

                        var isEmptyBlock = false;
                        if (nextTokenIdx < luaSource.Length)
                        {
                            if (c == '{' && luaSource[nextTokenIdx] == '}') isEmptyBlock = true;
                            else if (c == '[' && luaSource[nextTokenIdx] == ']') isEmptyBlock = true;
                        }

                        if (isEmptyBlock)
                        {
                            i++;
                            while (i < nextTokenIdx)
                            {
                                sb.Append(luaSource[i++]);
                            }
                            sb.Append(luaSource[i]);
                        }
                        else
                        {
                            sb.AppendLine();
                            lastCharWasNewline = true;
                            indentLevel++;
                            AppendIndent(sb, indentLevel);
                        }
                        break;

                    case '}':
                    case ']':
                        if (!lastCharWasNewline) sb.AppendLine();
                        lastCharWasNewline = true;
                        indentLevel = Math.Max(0, indentLevel - 1);
                        AppendIndent(sb, indentLevel);
                        sb.Append(c);
                        lastCharWasNewline = false;
                        break;

                    case ',':
                        sb.Append(c);
                        sb.AppendLine();
                        lastCharWasNewline = true;
                        AppendIndent(sb, indentLevel);
                        break;

                    case '"':
                    case '\'':
                        inString = true;
                        stringDelim = c;
                        sb.Append(c);
                        lastCharWasNewline = false;
                        break;

                    default:
                        if (char.IsWhiteSpace(c))
                        {
                            if (!lastCharWasNewline && (sb.Length > 0 && sb[sb.Length - 1] != ' '))
                            {
                                sb.Append(' ');
                            }
                        }
                        else
                        {
                            sb.Append(c);
                            lastCharWasNewline = false;
                        }
                        break;
                }
                i++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Appends indentation to a StringBuilder
        /// </summary>
        private static void AppendIndent(StringBuilder sb, int indentLevel)
        {
            for (var j = 0; j < indentLevel; j++)
            {
                sb.Append("  ");
            }
        }
    }
}
