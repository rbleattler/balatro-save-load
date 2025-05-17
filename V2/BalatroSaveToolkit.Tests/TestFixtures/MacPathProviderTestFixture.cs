using System.Runtime.Versioning;

namespace BalatroSaveToolkit.Tests.TestFixtures
{
    /// <summary>
    /// Test fixture for macOS path provider tests.
    /// </summary>
    [SupportedOSPlatform("maccatalyst")]
    public class MacPathProviderTestFixture : PathProviderTestBase
    {
        private const string UserName = "testuser";

        public void SetupMacFileSystem()
        {
            SetupBase(pathSeparator: '/', isCaseSensitive: true);

            // Setup macOS-specific paths and environment variables
            SetupMacPaths();
            SetupMacEnvironmentVariables();
            SetupBalatroSaveFiles();
            SetupSteamInstallation();
        }

        /// <summary>
        /// Sets up macOS-specific paths.
        /// </summary>
        private void SetupMacPaths()
        {
            // Create macOS system directories
            FileSystem.CreateDirectory("/System");
            FileSystem.CreateDirectory("/Library");
            FileSystem.CreateDirectory("/Applications");
            FileSystem.CreateDirectory("/Users");
            FileSystem.CreateDirectory($"/Users/{UserName}");
            FileSystem.CreateDirectory($"/Users/{UserName}/Library");
            FileSystem.CreateDirectory($"/Users/{UserName}/Library/Application Support");
            FileSystem.CreateDirectory($"/Users/{UserName}/Library/Preferences");
            FileSystem.CreateDirectory($"/Users/{UserName}/Library/Caches");
        }

        /// <summary>
        /// Sets up macOS-specific environment variables.
        /// </summary>
        private void SetupMacEnvironmentVariables()
        {
            FileSystem.SetEnvironmentVariable("USER", UserName);
            FileSystem.SetEnvironmentVariable("HOME", GetUserProfilePath());
            FileSystem.SetEnvironmentVariable("TMPDIR", GetTempPath());
            FileSystem.SetEnvironmentVariable("XDG_CONFIG_HOME", $"/Users/{UserName}/.config");
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

            // Create resource fork file (macOS specific)
            FileSystem.CreateFile(Path.Combine(balatroSavePath, "._save1.lua"), "Resource fork data");

            // Create nested directories for testing path handling
            FileSystem.CreateDirectory(Path.Combine(balatroSavePath, "SaveBackups"));
        }

        /// <summary>
        /// Sets up Steam installation in the mock file system.
        /// </summary>
        private void SetupSteamInstallation()
        {
            // Create Steam installation directories
            FileSystem.CreateDirectory("/Applications/Steam.app");
            FileSystem.CreateDirectory("/Applications/Steam.app/Contents/MacOS");
            FileSystem.CreateDirectory($"/Users/{UserName}/Library/Application Support/Steam");

            // Create Steam user data with Balatro save folder
            var steamBalatroSavePath = GetBalatroSteamSavePath();
            FileSystem.CreateDirectory(steamBalatroSavePath);

            // Add some save files
            FileSystem.CreateFile(Path.Combine(steamBalatroSavePath, "steam_save1.lua"), "-- Steam Balatro save file 1");
            FileSystem.CreateFile(Path.Combine(steamBalatroSavePath, "steam_save2.lua"), "-- Steam Balatro save file 2");
        }

        #region Path Implementations

        /// <inheritdoc />
        protected override string GetUserProfilePath() => $"/Users/{UserName}";

        /// <inheritdoc />
        protected override string GetDocumentsPath() => $"/Users/{UserName}/Documents";

        /// <inheritdoc />
        protected override string GetDesktopPath() => $"/Users/{UserName}/Desktop";

        /// <inheritdoc />
        protected override string GetLocalAppDataPath() => $"/Users/{UserName}/Library/Application Support";

        /// <inheritdoc />
        protected override string GetRoamingAppDataPath() => $"/Users/{UserName}/Library/Application Support";

        /// <inheritdoc />
        protected override string GetCommonAppDataPath() => "/Library/Application Support";

        /// <inheritdoc />
        protected override string GetTempPath() => $"/Users/{UserName}/Library/Caches/TemporaryItems";

        /// <inheritdoc />
        protected override string GetBalatroSavePath() => $"/Users/{UserName}/Library/Application Support/Balatro";

        /// <inheritdoc />
        protected override string GetBalatroSteamSavePath() =>
            $"/Users/{UserName}/Library/Application Support/Steam/userdata/12345/1804740/remote/SaveData";

        #endregion
    }
}
