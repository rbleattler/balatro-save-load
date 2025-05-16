namespace BalatroSaveToolkit.Models
{    public class SaveFileInfo
    {
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public long FileSize { get; set; }
        public bool IsSelected { get; set; }

        public SaveFileInfo(string name, string filePath, DateTime lastModified, long fileSize)
        {
            Name = name ?? string.Empty;
            FilePath = filePath ?? string.Empty;
            LastModified = lastModified;
            FileSize = fileSize;
        }
    }
}
