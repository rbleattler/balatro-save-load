namespace BalatroSaveToolkit.Models
{
    public class AppSettings
    {
        // Game Configuration
        public string GamePath { get; set; } = string.Empty;
        public string SavesPath { get; set; } = string.Empty;
        public bool AutoDetectGame { get; set; } = true;
        
        // Backup Configuration
        public string BackupPath { get; set; } = string.Empty;
        public bool AutoBackupOnStart { get; set; } = true;
        public bool AutoBackupOnClose { get; set; } = true;
        public int MaxBackupsToKeep { get; set; } = 20;
        
        // Application Settings
        public bool StartWithWindows { get; set; } = false;
        public bool MinimizeToTray { get; set; } = true;
        public string LogLevel { get; set; } = "Info";
    }
}
