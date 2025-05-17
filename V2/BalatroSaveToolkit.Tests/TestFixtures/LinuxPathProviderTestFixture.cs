using System.Runtime.Versioning;

namespace BalatroSaveToolkit.Tests.TestFixtures
{
    /// <summary>
    /// Test fixture for Linux path provider tests.
    /// </summary>
    [SupportedOSPlatform("linux")]
    public class LinuxPathProviderTestFixture : PathProviderTestBase
    {
        private const string UserName = "testuser";

        public void SetupLinuxFileSystem()
        {
            SetupBase(pathSeparator: '/', isCaseSensitive: true);

            // Setup Linux-specific paths and environment variables
            SetupLinuxPaths();
            SetupLinuxEnvironmentVariables();
            SetupBalatroSaveFiles();
            SetupSteamInstallation();
            SetupFlatpak();
            SetupProton();
        }

        /// <summary>
        /// Sets up Linux-specific paths.
        /// </summary>
        private void SetupLinuxPaths()
        {
            // Create Linux system directories
            FileSystem.CreateDirectory("/bin");
            FileSystem.CreateDirectory("/boot");
            FileSystem.CreateDirectory("/dev");
            FileSystem.CreateDirectory("/etc");
            FileSystem.CreateDirectory("/home");
            FileSystem.CreateDirectory($"/home/{UserName}");
            FileSystem.CreateDirectory("/lib");
            FileSystem.CreateDirectory("/media");
            FileSystem.CreateDirectory("/mnt");
            FileSystem.CreateDirectory("/opt");
            FileSystem.CreateDirectory("/proc");
            FileSystem.CreateDirectory("/root");
            FileSystem.CreateDirectory("/run");
            FileSystem.CreateDirectory("/sbin");
            FileSystem.CreateDirectory("/srv");
            FileSystem.CreateDirectory("/sys");
            FileSystem.CreateDirectory("/tmp");
            FileSystem.CreateDirectory("/usr");
            FileSystem.CreateDirectory("/usr/bin");
            FileSystem.CreateDirectory("/usr/lib");
            FileSystem.CreateDirectory("/usr/local");
            FileSystem.CreateDirectory("/usr/share");
            FileSystem.CreateDirectory("/var");

            // Create XDG directories
            FileSystem.CreateDirectory($"/home/{UserName}/.config");
            FileSystem.CreateDirectory($"/home/{UserName}/.local");
            FileSystem.CreateDirectory($"/home/{UserName}/.local/share");
            FileSystem.CreateDirectory($"/home/{UserName}/.cache");
            FileSystem.CreateDirectory($"/home/{UserName}/.local/state");

            // Create user directories
            FileSystem.CreateDirectory($"/home/{UserName}/Documents");
            FileSystem.CreateDirectory($"/home/{UserName}/Desktop");
            FileSystem.CreateDirectory($"/home/{UserName}/Pictures");
            FileSystem.CreateDirectory($"/home/{UserName}/Music");
            FileSystem.CreateDirectory($"/home/{UserName}/Videos");
            FileSystem.CreateDirectory($"/home/{UserName}/Downloads");
        }

        /// <summary>
        /// Sets up Linux-specific environment variables.
        /// </summary>
        private void SetupLinuxEnvironmentVariables()
        {
            FileSystem.SetEnvironmentVariable("USER", UserName);
            FileSystem.SetEnvironmentVariable("HOME", GetUserProfilePath());
            FileSystem.SetEnvironmentVariable("TEMP", "/tmp");
            FileSystem.SetEnvironmentVariable("TMPDIR", "/tmp");

            // XDG variables
            FileSystem.SetEnvironmentVariable("XDG_CONFIG_HOME", $"/home/{UserName}/.config");
            FileSystem.SetEnvironmentVariable("XDG_DATA_HOME", $"/home/{UserName}/.local/share");
            FileSystem.SetEnvironmentVariable("XDG_CACHE_HOME", $"/home/{UserName}/.cache");
            FileSystem.SetEnvironmentVariable("XDG_STATE_HOME", $"/home/{UserName}/.local/state");
            FileSystem.SetEnvironmentVariable("XDG_RUNTIME_DIR", $"/run/user/1000");

            // Desktop environment variables
            FileSystem.SetEnvironmentVariable("XDG_CURRENT_DESKTOP", "GNOME");
            FileSystem.SetEnvironmentVariable("DESKTOP_SESSION", "gnome");
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

            // Create symlink-like entry (actual symlinks not supported in mock)
            FileSystem.CreateFile(Path.Combine(balatroSavePath, "symlink_save.lua"),
                "This represents a symlink to save1.lua");
        }

        /// <summary>
        /// Sets up Steam installation in the mock file system.
        /// </summary>
        private void SetupSteamInstallation()
        {
            // Create Steam native installation directories
            FileSystem.CreateDirectory($"/home/{UserName}/.steam");
            FileSystem.CreateDirectory($"/home/{UserName}/.steam/steam");
            FileSystem.CreateDirectory($"/home/{UserName}/.local/share/Steam");

            // Create Steam user data with Balatro save folder
            var steamBalatroSavePath = GetBalatroSteamSavePath();
            FileSystem.CreateDirectory(steamBalatroSavePath);

            // Add some save files
            FileSystem.CreateFile(Path.Combine(steamBalatroSavePath, "steam_save1.lua"), "-- Steam Balatro save file 1");
            FileSystem.CreateFile(Path.Combine(steamBalatroSavePath, "steam_save2.lua"), "-- Steam Balatro save file 2");
        }

        /// <summary>
        /// Sets up Flatpak environment in the mock file system.
        /// </summary>
        private void SetupFlatpak()
        {
            // Create Flatpak directories
            FileSystem.CreateDirectory($"/home/{UserName}/.var");
            FileSystem.CreateDirectory($"/home/{UserName}/.var/app");
            FileSystem.CreateDirectory($"/home/{UserName}/.var/app/com.valvesoftware.Steam");
            FileSystem.CreateDirectory($"/home/{UserName}/.var/app/com.valvesoftware.Steam/.local/share");
            FileSystem.CreateDirectory($"/home/{UserName}/.var/app/com.valvesoftware.Steam/.steam/steam");

            // Create Flatpak Steam user data with Balatro save folder
            var flatpakSteamPath = $"/home/{UserName}/.var/app/com.valvesoftware.Steam/.steam/steam";
            FileSystem.CreateDirectory($"{flatpakSteamPath}/userdata/12345/1804740/remote/SaveData");

            // Add some save files
            FileSystem.CreateFile(
                $"{flatpakSteamPath}/userdata/12345/1804740/remote/SaveData/flatpak_save1.lua",
                "-- Flatpak Steam Balatro save file 1");
        }

        /// <summary>
        /// Sets up Proton/Wine environment in the mock file system.
        /// </summary>
        private void SetupProton()
        {
            // Create Proton prefix directories
            FileSystem.CreateDirectory($"/home/{UserName}/.local/share/Steam/steamapps/compatdata/1804740");
            FileSystem.CreateDirectory($"/home/{UserName}/.local/share/Steam/steamapps/compatdata/1804740/pfx");
            FileSystem.CreateDirectory($"/home/{UserName}/.local/share/Steam/steamapps/compatdata/1804740/pfx/drive_c");
            FileSystem.CreateDirectory($"/home/{UserName}/.local/share/Steam/steamapps/compatdata/1804740/pfx/drive_c/users");
            FileSystem.CreateDirectory($"/home/{UserName}/.local/share/Steam/steamapps/compatdata/1804740/pfx/drive_c/users/steamuser");
            FileSystem.CreateDirectory($"/home/{UserName}/.local/share/Steam/steamapps/compatdata/1804740/pfx/drive_c/users/steamuser/AppData");
            FileSystem.CreateDirectory($"/home/{UserName}/.local/share/Steam/steamapps/compatdata/1804740/pfx/drive_c/users/steamuser/AppData/Local");
            FileSystem.CreateDirectory($"/home/{UserName}/.local/share/Steam/steamapps/compatdata/1804740/pfx/drive_c/users/steamuser/AppData/Local/Balatro");

            // Add some save files
            FileSystem.CreateFile(
                $"/home/{UserName}/.local/share/Steam/steamapps/compatdata/1804740/pfx/drive_c/users/steamuser/AppData/Local/Balatro/proton_save.lua",
                "-- Proton Balatro save file");

            // Create Wine prefix directories
            FileSystem.CreateDirectory($"/home/{UserName}/.wine");
            FileSystem.CreateDirectory($"/home/{UserName}/.wine/drive_c");
            FileSystem.CreateDirectory($"/home/{UserName}/.wine/drive_c/users");
            FileSystem.CreateDirectory($"/home/{UserName}/.wine/drive_c/users/{UserName}");
            FileSystem.CreateDirectory($"/home/{UserName}/.wine/drive_c/users/{UserName}/AppData");
            FileSystem.CreateDirectory($"/home/{UserName}/.wine/drive_c/users/{UserName}/AppData/Local");
            FileSystem.CreateDirectory($"/home/{UserName}/.wine/drive_c/users/{UserName}/AppData/Local/Balatro");

            // Add some save files
            FileSystem.CreateFile(
                $"/home/{UserName}/.wine/drive_c/users/{UserName}/AppData/Local/Balatro/wine_save.lua",
                "-- Wine Balatro save file");
        }

        #region Path Implementations

        /// <inheritdoc />
        protected override string GetUserProfilePath() => $"/home/{UserName}";

        /// <inheritdoc />
        protected override string GetDocumentsPath() => $"/home/{UserName}/Documents";

        /// <inheritdoc />
        protected override string GetDesktopPath() => $"/home/{UserName}/Desktop";

        /// <inheritdoc />
        protected override string GetLocalAppDataPath() => $"/home/{UserName}/.local/share";

        /// <inheritdoc />
        protected override string GetRoamingAppDataPath() => $"/home/{UserName}/.config";

        /// <inheritdoc />
        protected override string GetCommonAppDataPath() => "/usr/local/share";

        /// <inheritdoc />
        protected override string GetTempPath() => "/tmp";

        /// <inheritdoc />
        protected override string GetBalatroSavePath() => $"/home/{UserName}/.local/share/Balatro";

        /// <inheritdoc />
        protected override string GetBalatroSteamSavePath() =>
            $"/home/{UserName}/.steam/steam/userdata/12345/1804740/remote/SaveData";

        #endregion
    }
}
