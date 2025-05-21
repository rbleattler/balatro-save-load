using System;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Represents information about a saved game file.
    /// </summary>
    public class SaveFileInfo
    {
        /// <summary>
        /// Gets or sets the full path to the save file.
        /// </summary>
        public required string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the save was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the profile number (1-4) the save belongs to.
        /// </summary>
        public int ProfileNumber { get; set; }

        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Gets the formatted display name for the save file.
        /// </summary>
        public string DisplayName => $"{Timestamp:yyyy-MM-dd HH:mm:ss}";

        /// <summary>
        /// Gets the formatted file size for display.
        /// </summary>
        public string FormattedSize
        {
            get
            {
                if (FileSize < 1024)
                {
                    return $"{FileSize} B";
                }
                if (FileSize < 1024 * 1024)
                {
                    return $"{FileSize / 1024.0:F1} KB";
                }
                return $"{FileSize / (1024.0 * 1024):F1} MB";
            }
        }
    }
}
