using System.Runtime.Versioning;

namespace BalatroSaveToolkit.Tests.TestFixtures
{
    /// <summary>
    /// Test fixture for Windows path provider tests.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class WindowsPathProviderTestFixture : PathProviderTestBase
    {
        private const string UserName = "TestUser";

        public void SetupWindowsFileSystem()
        {
            SetupBase(pathSeparator: '\\', isCaseSensitive: false);

            // Setup Windows-specific paths and environment variables
            SetupWindowsPaths();
            SetupWindowsEnvironmentVariables();
            SetupBalatroSaveFiles();
            SetupSteamInstallation();
        }

        /// <summary>
        /// Sets up Windows-specific paths.
        /// </summary>
        private void SetupWindowsPaths()
        {
            // Add Windows-specific paths that aren't in the base setup
            FileSystem.SetupSpecialFolder(Environment.SpecialFolder.System, @"C:\Windows\System32");
            FileSystem.SetupSpecialFolder(Environment.SpecialFolder.Windows, @"C:\Windows");
            FileSystem.SetupSpecialFolder(Environment.SpecialFolder.ProgramFiles, @"C:\Program Files");
            FileSystem.SetupSpecialFolder(Environment.SpecialFolder.ProgramFilesX86, @"C:\Program Files (x86)");

            // Create additional directories
            FileSystem.CreateDirectory(@"C:\Windows");
            FileSystem.CreateDirectory(@"C:\Windows\System32");
            FileSystem.CreateDirectory(@"C:\Program Files");
            FileSystem.CreateDirectory(@"C:\Program Files (x86)");
            FileSystem.CreateDirectory(@"D:\Program Files (x86)");
            FileSystem.CreateDirectory(@"D:\Program Files");
            FileSystem.CreateDirectory(@"D:\Steam");
            FileSystem.CreateDirectory(@"E:\Steam");
        }

        /// <summary>
        /// Sets up Windows-specific environment variables.
        /// </summary>
        private void SetupWindowsEnvironmentVariables()
        {
            FileSystem.SetEnvironmentVariable("USERPROFILE", GetUserProfilePath());
            FileSystem.SetEnvironmentVariable("APPDATA", GetRoamingAppDataPath());
            FileSystem.SetEnvironmentVariable("LOCALAPPDATA", GetLocalAppDataPath());
            FileSystem.SetEnvironmentVariable("PROGRAMFILES", @"C:\Program Files");
            FileSystem.SetEnvironmentVariable("PROGRAMFILES(X86)", @"C:\Program Files (x86)");
            FileSystem.SetEnvironmentVariable("SYSTEMROOT", @"C:\Windows");
            FileSystem.SetEnvironmentVariable("WINDIR", @"C:\Windows");
        }

        /// <summary>
        /// Sets up Balatro save files in the mock file system.
        /// </summary>
        private void SetupBalatroSaveFiles()
        {
            var balatroSavePath = GetBalatroSavePath();

            // Create Balatro save directory and sample files
            FileSystem.CreateDirectory(balatroSavePath);
            FileSystem.CreateFile(Path.Combine(balatroSavePath, "save1.lua"), "-- Balatro save file 1");
            FileSystem.CreateFile(Path.Combine(balatroSavePath, "save2.lua"), "-- Balatro save file 2");
            FileSystem.CreateFile(Path.Combine(balatroSavePath, "save3.lua"), "-- Balatro save file 3");

            // Create a folder that might cause confusion
            FileSystem.CreateDirectory(Path.Combine(balatroSavePath, "SaveBackups"));
        }

        /// <summary>
        /// Sets up Steam installation in the mock file system.
        /// </summary>
        private void SetupSteamInstallation()
        {
            // Create Steam installation directories
            FileSystem.CreateDirectory(@"C:\Program Files (x86)\Steam");
            FileSystem.CreateDirectory(@"C:\Program Files (x86)\Steam\steamapps\common\Balatro");

            // Create Steam user data with Balatro save folder
            var steamBalatroSavePath = GetBalatroSteamSavePath();
            FileSystem.CreateDirectory(steamBalatroSavePath);

            // Add some save files
            FileSystem.CreateFile(Path.Combine(steamBalatroSavePath, "steam_save1.lua"), "-- Steam Balatro save file 1");
            FileSystem.CreateFile(Path.Combine(steamBalatroSavePath, "steam_save2.lua"), "-- Steam Balatro save file 2");

            // Create registry key simulation by setting special environment variable
            FileSystem.SetEnvironmentVariable("STEAM_PATH", @"C:\Program Files (x86)\Steam");
        }

        #region Path Implementations

        /// <inheritdoc />
        protected override string GetUserProfilePath() => @$"C:\Users\{UserName}";

        /// <inheritdoc />
        protected override string GetDocumentsPath() => @$"C:\Users\{UserName}\Documents";

        /// <inheritdoc />
        protected override string GetDesktopPath() => @$"C:\Users\{UserName}\Desktop";

        /// <inheritdoc />
        protected override string GetLocalAppDataPath() => @$"C:\Users\{UserName}\AppData\Local";

        /// <inheritdoc />
        protected override string GetRoamingAppDataPath() => @$"C:\Users\{UserName}\AppData\Roaming";

        /// <inheritdoc />
        protected override string GetCommonAppDataPath() => @"C:\ProgramData";

        /// <inheritdoc />
        protected override string GetTempPath() => @$"C:\Users\{UserName}\AppData\Local\Temp";

        /// <inheritdoc />
        protected override string GetBalatroSavePath() => @$"C:\Users\{UserName}\AppData\Local\Balatro";

        /// <inheritdoc />
        protected override string GetBalatroSteamSavePath() =>
            @$"C:\Program Files (x86)\Steam\userdata\12345\1804740\remote\SaveData";

        #endregion
    }
}
