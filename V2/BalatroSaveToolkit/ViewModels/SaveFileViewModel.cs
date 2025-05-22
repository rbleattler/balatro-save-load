using System;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BalatroSaveToolkit.ViewModels
{
    /// <summary>
    /// ViewModel for a save file.
    /// </summary>
    public class SaveFileViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string> _displayName;
        private readonly ObservableAsPropertyHelper<string> _formattedFileSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileViewModel"/> class.
        /// </summary>
        public SaveFileViewModel()
        {
            // Initialize required properties with default values
            FilePath = string.Empty;
            Timestamp = DateTime.Now;
            ProfileNumber = 1;
            FileSize = 0;

            // Set up property change notifications for derived properties
            this.WhenAnyValue(x => x.ProfileNumber, x => x.Timestamp)
                .Select(_ => $"Profile {ProfileNumber} - {Timestamp:yyyy-MM-dd HH:mm:ss}")
                .ToProperty(this, x => x.DisplayName, out _displayName);

            this.WhenAnyValue(x => x.FileSize)
                .Select(size => FormatFileSize(size))
                .ToProperty(this, x => x.FormattedFileSize, out _formattedFileSize);
        }

        /// <summary>
        /// Gets or sets the full path to the save file.
        /// </summary>
        [Reactive]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the save was created.
        /// </summary>
        [Reactive]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the profile number (1-4) the save belongs to.
        /// </summary>
        [Reactive]
        public int ProfileNumber { get; set; }

        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        [Reactive]
        public long FileSize { get; set; }

        /// <summary>
        /// Gets the formatted display name for the save file.
        /// </summary>
        public string DisplayName => _displayName?.Value ?? $"Profile {ProfileNumber} - {Timestamp:yyyy-MM-dd HH:mm:ss}";

        /// <summary>
        /// Gets the formatted file size string.
        /// </summary>
        public string FormattedFileSize => _formattedFileSize?.Value ?? FormatFileSize(FileSize);

        /// <summary>
        /// Formats a file size in bytes to a human-readable string.
        /// </summary>
        /// <param name="size">The size in bytes.</param>
        /// <returns>A formatted string representation of the file size.</returns>
        private static string FormatFileSize(long size)
        {
            if (size < 1024)
                return $"{size} B";
            else if (size < 1024 * 1024)
                return $"{size / 1024.0:F2} KB";
            else
                return $"{size / (1024.0 * 1024.0):F2} MB";
        }
    }
}
