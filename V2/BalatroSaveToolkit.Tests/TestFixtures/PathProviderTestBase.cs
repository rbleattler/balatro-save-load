using BalatroSaveToolkit.Services.Interfaces;
using BalatroSaveToolkit.Tests.Mocks;
using Moq;

namespace BalatroSaveToolkit.Tests.TestFixtures
{
    /// <summary>
    /// Base class for path provider tests with shared functionality.
    /// </summary>
    public abstract class PathProviderTestBase
    {
        protected Mock<ILogService> MockLogService;
        protected Mock<IErrorHandlingService> MockErrorHandler;
        protected FileSystemMock FileSystem;
        protected FileSystemMockOperations FileSystemOps;

        /// <summary>
        /// Sets up the common test fixture.
        /// </summary>
        /// <param name="pathSeparator">The path separator character to use.</param>
        /// <param name="isCaseSensitive">Whether paths should be case-sensitive.</param>
        public void SetupBase(char pathSeparator = '\\', bool isCaseSensitive = false)
        {
            MockLogService = new Mock<ILogService>();
            MockErrorHandler = new Mock<IErrorHandlingService>();
            FileSystem = new FileSystemMock(pathSeparator, isCaseSensitive);
            FileSystemOps = FileSystem.SetupFileSystemMocks();

            // Setup common paths and environment variables
            SetupCommonPaths();
            SetupCommonEnvironmentVariables();
        }

        /// <summary>
        /// Sets up common paths for all platforms.
        /// </summary>
        protected virtual void SetupCommonPaths()
        {
            // Set up basic special folders that all platforms would have
            FileSystem.SetupSpecialFolder(Environment.SpecialFolder.UserProfile, GetUserProfilePath());
            FileSystem.SetupSpecialFolder(Environment.SpecialFolder.Personal, GetDocumentsPath());
            FileSystem.SetupSpecialFolder(Environment.SpecialFolder.Desktop, GetDesktopPath());
            FileSystem.SetupSpecialFolder(Environment.SpecialFolder.LocalApplicationData, GetLocalAppDataPath());
            FileSystem.SetupSpecialFolder(Environment.SpecialFolder.ApplicationData, GetRoamingAppDataPath());
            FileSystem.SetupSpecialFolder(Environment.SpecialFolder.CommonApplicationData, GetCommonAppDataPath());
        }

        /// <summary>
        /// Sets up common environment variables for all platforms.
        /// </summary>
        protected virtual void SetupCommonEnvironmentVariables()
        {
            FileSystem.SetEnvironmentVariable("TEMP", GetTempPath());
            FileSystem.SetEnvironmentVariable("TMP", GetTempPath());
            FileSystem.SetEnvironmentVariable("HOME", GetUserProfilePath());
        }

        #region Abstract Path Methods (Platform-specific)

        /// <summary>
        /// Gets the user profile path for the platform.
        /// </summary>
        protected abstract string GetUserProfilePath();

        /// <summary>
        /// Gets the documents path for the platform.
        /// </summary>
        protected abstract string GetDocumentsPath();

        /// <summary>
        /// Gets the desktop path for the platform.
        /// </summary>
        protected abstract string GetDesktopPath();

        /// <summary>
        /// Gets the local application data path for the platform.
        /// </summary>
        protected abstract string GetLocalAppDataPath();

        /// <summary>
        /// Gets the roaming application data path for the platform.
        /// </summary>
        protected abstract string GetRoamingAppDataPath();

        /// <summary>
        /// Gets the common application data path for the platform.
        /// </summary>
        protected abstract string GetCommonAppDataPath();

        /// <summary>
        /// Gets the temporary path for the platform.
        /// </summary>
        protected abstract string GetTempPath();

        /// <summary>
        /// Gets the Balatro save path for the platform.
        /// </summary>
        protected abstract string GetBalatroSavePath();

        /// <summary>
        /// Gets the Balatro Steam save path for the platform.
        /// </summary>
        protected abstract string GetBalatroSteamSavePath();

        #endregion
    }
}
