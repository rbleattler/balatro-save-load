using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace BalatroSaveToolkit.Services.FileSystem
{
    /// <summary>
    /// macOS implementation of the FileSystemService.
    /// </summary>
    public class MacOsFileSystemService : FileSystemService
    {
        private FileSystemWatcher _fileWatcher;
        private Action _onFileChanged;
        private string _watchedFilePath;

        /// <inheritdoc/>
        public override string BalatroSaveDirectory 
        {
            get
            {
                string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(
                    homeDirectory,
                    "Library", 
                    "Application Support", 
                    "Balatro",
                    "Saves");
            }
        }

        /// <inheritdoc/>
        protected override void OpenFileExplorerPlatformSpecific(string path)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "open",
                Arguments = $"\"{path}\"",
                UseShellExecute = true
            });
        }

        /// <inheritdoc/>
        public override void StartFileWatcher(int profileNumber, Action onChanged)
        {
            StopFileWatcher();
            
            _onFileChanged = onChanged;
            _watchedFilePath = GetCurrentSaveFilePath(profileNumber);
            var directory = Path.GetDirectoryName(_watchedFilePath);
            
            if (!Directory.Exists(directory))
            {
                return;
            }

            _fileWatcher = new FileSystemWatcher
            {
                Path = directory,
                Filter = Path.GetFileName(_watchedFilePath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime
            };

            _fileWatcher.Changed += FileWatcher_Changed;
            _fileWatcher.Created += FileWatcher_Changed;
            _fileWatcher.EnableRaisingEvents = true;
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            _onFileChanged?.Invoke();
        }

        /// <inheritdoc/>
        public override void StopFileWatcher()
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Changed -= FileWatcher_Changed;
                _fileWatcher.Created -= FileWatcher_Changed;
                _fileWatcher.Dispose();
                _fileWatcher = null;
            }
        }
    }
}
