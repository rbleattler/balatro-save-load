using System;
using System.Runtime.InteropServices;
using BalatroSaveToolkit.Core.Services;

namespace BalatroSaveToolkit.Services.FileSystem
{
    /// <summary>
    /// Factory class for creating platform-specific file system services.
    /// </summary>
    public static class FileSystemServiceFactory
    {
        /// <summary>
        /// Creates the appropriate file system service for the current platform.
        /// </summary>
        /// <returns>A platform-specific implementation of IFileSystemService.</returns>
        public static IFileSystemService Create()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsFileSystemService();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new MacOsFileSystemService();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxFileSystemService();
            }

            throw new PlatformNotSupportedException("Unsupported operating system");
        }
    }
}
