using BalatroSaveToolkit.Services.Implementations.Linux;
using BalatroSaveToolkit.Services.Interfaces;
using BalatroSaveToolkit.Tests.Mocks;
using BalatroSaveToolkit.Tests.TestFixtures;
using Moq;
using System.Reflection;
using System.Runtime.Versioning;

namespace BalatroSaveToolkit.Tests.Services
{
    [SupportedOSPlatform("linux")]
    [TestClass]
    public class LinuxPathProviderTests
    {
        private LinuxPathProviderTestFixture _fixture;
        private LinuxPathProvider _pathProvider;
        private FileSystemMockOperations _fileSystemOps;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new LinuxPathProviderTestFixture();
            _fixture.SetupLinuxFileSystem();

            _fileSystemOps = _fixture.FileSystemOps;

            // Create the path provider with mocked dependencies
            _pathProvider = new LinuxPathProvider(
                _fixture.MockLogService.Object,
                _fixture.MockErrorHandler.Object);

            // Use reflection to inject the mocked file system utilities
            InjectMock(_pathProvider, "GetFileExists", typeof(File), nameof(File.Exists), _fileSystemOps.File);
            InjectMock(_pathProvider, "GetDirectoryExists", typeof(Directory), nameof(Directory.Exists), _fileSystemOps.Directory);
            InjectMock(_pathProvider, "GetEnvironmentGetFolderPath", typeof(Environment), nameof(Environment.GetFolderPath), _fileSystemOps.Environment);
            InjectMock(_pathProvider, "GetEnvironmentGetEnvironmentVariable", typeof(Environment), nameof(Environment.GetEnvironmentVariable), _fileSystemOps.Environment);
            InjectMock(_pathProvider, "GetDirectoryCreateDirectory", typeof(Directory), nameof(Directory.CreateDirectory), _fileSystemOps.Directory);
            InjectMock(_pathProvider, "GetDirectoryGetFiles", typeof(Directory), nameof(Directory.GetFiles), _fileSystemOps.Directory);
            InjectMock(_pathProvider, "GetDirectoryGetDirectories", typeof(Directory), nameof(Directory.GetDirectories), _fileSystemOps.Directory);
        }

        /// <summary>
        /// Helper method to inject a mock via reflection.
        /// </summary>
        private static void InjectMock(object target, string fieldName, Type originalType, string methodName, object mockObject)
        {
            // Get the field from the target object
            var field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                // Get the method from the mock object
                var methodInfo = mockObject.GetType().GetMethod(methodName);
                if (methodInfo != null)
                {
                    // Create a delegate of the appropriate type
                    var delegateType = field.FieldType;
                    var delegateInstance = methodInfo.CreateDelegate(delegateType, mockObject);

                    // Set the field to the delegate
                    field.SetValue(target, delegateInstance);
                }
            }
        }

        [TestMethod]
        public void IsCaseSensitive_ShouldBeTrue()
        {
            // Arrange & Act & Assert
            Assert.IsTrue(_pathProvider.IsCaseSensitive);
        }

        [TestMethod]
        public void PathSeparator_ShouldBeForwardSlash()
        {
            // Arrange & Act & Assert
            Assert.AreEqual('/', _pathProvider.PathSeparator);
        }

        [TestMethod]
        public void ResolveLinuxPath_ShouldReplaceHomeTilde()
        {
            // Arrange
            string homePath = _fixture.GetUserProfilePath();
            string testPath = "~/.local/share/Balatro";

            // Act
            string resolvedPath = _pathProvider.ResolveLinuxPath(testPath);

            // Assert
            Assert.IsFalse(resolvedPath.Contains('~'));
            Assert.IsTrue(resolvedPath.StartsWith(homePath));
            StringAssert.Contains(resolvedPath, ".local/share/Balatro");
        }

        [TestMethod]
        public void ResolveLinuxPath_WithoutTilde_ShouldNotModifyPath()
        {
            // Arrange
            string testPath = "/usr/share/Balatro";

            // Act
            string resolvedPath = _pathProvider.ResolveLinuxPath(testPath);

            // Assert
            Assert.AreEqual(testPath, resolvedPath);
        }

        [TestMethod]
        public void NormalizeLinuxPath_ShouldConvertBackslashesToForwardSlashes()
        {
            // Arrange
            string testPath = "home\\user\\Documents";

            // Act
            string normalizedPath = _pathProvider.NormalizeLinuxPath(testPath);

            // Assert
            Assert.IsFalse(normalizedPath.Contains('\\'));
            Assert.IsTrue(normalizedPath.Contains('/'));
            Assert.AreEqual("home/user/Documents", normalizedPath);
        }

        [TestMethod]
        public void NormalizeLinuxPath_ShouldRemoveTrailingSlash()
        {
            // Arrange
            string testPath = "/home/user/Documents/";

            // Act
            string normalizedPath = _pathProvider.NormalizeLinuxPath(testPath);

            // Assert
            Assert.AreEqual("/home/user/Documents", normalizedPath);
        }

        [TestMethod]
        public void NormalizeLinuxPath_ShouldNotRemoveSlashForRootDir()
        {
            // Arrange
            string testPath = "/";

            // Act
            string normalizedPath = _pathProvider.NormalizeLinuxPath(testPath);

            // Assert
            Assert.AreEqual("/", normalizedPath);
        }

        [TestMethod]
        public void NormalizeLinuxPath_ShouldRemoveConsecutiveSlashes()
        {
            // Arrange
            string testPath = "/home//user///Documents";

            // Act
            string normalizedPath = _pathProvider.NormalizeLinuxPath(testPath);

            // Assert
            Assert.AreEqual("/home/user/Documents", normalizedPath);
        }

        [TestMethod]
        public void NormalizeLinuxPath_WithMixedSeparators_ShouldNormalizeToForwardSlashes()
        {
            // Arrange
            string mixedPath = "/home\\testuser/.local\\share/Balatro";

            // Act - Find and use the NormalizeLinuxPath method via reflection
            var method = _pathProvider.GetType().GetMethod("NormalizeLinuxPath",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var normalizedPath = method.Invoke(_pathProvider, new object[] { mixedPath }) as string;

            // Assert
            Assert.AreEqual("/home/testuser/.local/share/Balatro", normalizedPath);
        }

        [TestMethod]
        public void NormalizeLinuxPath_WithTrailingSeparator_ShouldRemoveTrailingSeparator()
        {
            // Arrange
            string pathWithTrailingSeparator = "/home/testuser/.local/share/Balatro/";

            // Act - Find and use the NormalizeLinuxPath method via reflection
            var method = _pathProvider.GetType().GetMethod("NormalizeLinuxPath",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var normalizedPath = method.Invoke(_pathProvider, new object[] { pathWithTrailingSeparator }) as string;

            // Assert
            Assert.AreEqual("/home/testuser/.local/share/Balatro", normalizedPath);
        }

        [TestMethod]
        public void NormalizeLinuxPath_WithDoubleSeparators_ShouldNormalizeSeparators()
        {
            // Arrange
            string pathWithDoubleSeparators = "/home//testuser/.local//share/Balatro";

            // Act - Find and use the NormalizeLinuxPath method via reflection
            var method = _pathProvider.GetType().GetMethod("NormalizeLinuxPath",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var normalizedPath = method.Invoke(_pathProvider, new object[] { pathWithDoubleSeparators }) as string;

            // Assert
            Assert.AreEqual("/home/testuser/.local/share/Balatro", normalizedPath);
        }

        [TestMethod]
        public void IsSymbolicLink_WithNonExistentFile_ShouldReturnFalse()
        {
            // Arrange
            string nonExistentPath = "/home/user/Documents/nonexistent.file";

            // Act
            bool result = _pathProvider.IsSymbolicLink(nonExistentPath);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [SupportedOSPlatform("linux")]
        public async Task GetApplicationDataDirectoryAsync_ShouldReturnXdgDataHome()
        {
            // This test will only run on actual Linux systems
            if (!OperatingSystem.IsLinux())
            {
                Assert.Inconclusive("This test only runs on Linux");
                return;
            }

            // Arrange
            const string testAppName = "TestApp";

            // Act
            string appDataDir = await _pathProvider.GetApplicationDataDirectoryAsync(testAppName);

            // Assert
            Assert.IsNotNull(appDataDir);
            Assert.IsTrue(appDataDir.Contains(".local/share") || appDataDir.Contains("XDG_DATA_HOME"));
            Assert.IsTrue(appDataDir.EndsWith(testAppName));

            // Cleanup - attempt to remove test directory if created
            try
            {
                if (Directory.Exists(appDataDir))
                {
                    Directory.Delete(appDataDir);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        [TestMethod]
        public async Task GetBalatroSaveDirectoryAsync_ShouldReturnCorrectDirectory()
        {
            // Arrange - Done in setup

            // Act
            var saveDir = await _pathProvider.GetBalatroSaveDirectoryAsync();

            // Assert
            Assert.AreEqual(_fixture.GetBalatroSavePath(), saveDir);
        }

        [TestMethod]
        public async Task GetBalatroSaveDirectoryAsync_WithSteamInstallation_ShouldReturnSteamDirectory()
        {
            // Arrange - Done in setup

            // Act
            var saveDir = await _pathProvider.GetBalatroSaveDirectoryAsync(steamInstallation: true);

            // Assert
            StringAssert.Contains(saveDir.ToLower(), "steam");
            StringAssert.Contains(saveDir.ToLower(), "userdata");
            StringAssert.Contains(saveDir.ToLower(), "1804740");
            StringAssert.Contains(saveDir.ToLower(), "remote");
            StringAssert.Contains(saveDir.ToLower(), "savedata");
        }

        [TestMethod]
        public async Task GetApplicationDataDirectoryAsync_ShouldReturnCorrectPath()
        {
            // Arrange - Done in setup

            // Act
            var appDataDir = await _pathProvider.GetApplicationDataDirectoryAsync("TestApp");

            // Assert
            StringAssert.Contains(appDataDir, ".local/share");
            StringAssert.Contains(appDataDir, "TestApp");
        }

        [TestMethod]
        public async Task GetApplicationDataDirectoryAsync_WithDefaultAppName_ShouldUseDefaultName()
        {
            // Arrange - Done in setup

            // Act
            var appDataDir = await _pathProvider.GetApplicationDataDirectoryAsync();

            // Assert
            StringAssert.Contains(appDataDir, ".local/share");
            StringAssert.Contains(appDataDir, "BalatroSaveToolkit");
        }

        [TestMethod]
        public async Task GetTempDirectoryAsync_ShouldReturnTempFolder()
        {
            // Arrange - Done in setup

            // Act
            var tempDir = await _pathProvider.GetTempDirectoryAsync();

            // Assert
            StringAssert.Contains(tempDir.ToLower(), "/tmp");
            Assert.IsTrue(_fileSystemOps.Directory.Exists(tempDir));
        }

        [TestMethod]
        public async Task GetDocumentsDirectoryAsync_ShouldReturnDocumentsFolder()
        {
            // Arrange - Done in setup

            // Act
            var documentsDir = await _pathProvider.GetDocumentsDirectoryAsync();

            // Assert
            StringAssert.Contains(documentsDir.ToLower(), "documents");
            Assert.IsTrue(_fileSystemOps.Directory.Exists(documentsDir));
        }

        [TestMethod]
        public async Task GetDesktopDirectoryAsync_ShouldReturnDesktopFolder()
        {
            // Arrange - Done in setup

            // Act
            var desktopDir = await _pathProvider.GetDesktopDirectoryAsync();

            // Assert
            StringAssert.Contains(desktopDir.ToLower(), "desktop");
            Assert.IsTrue(_fileSystemOps.Directory.Exists(desktopDir));
        }

        [TestMethod]
        public async Task GetUserHomeDirectoryAsync_ShouldReturnUserProfileFolder()
        {
            // Arrange - Done in setup

            // Act
            var homeDir = await _pathProvider.GetUserHomeDirectoryAsync();

            // Assert
            StringAssert.Contains(homeDir.ToLower(), "/home");
            Assert.IsTrue(_fileSystemOps.Directory.Exists(homeDir));
        }

        [TestMethod]
        public async Task GetSaveBackupsDirectoryAsync_ShouldCreateAndReturnBackupsFolder()
        {
            // Arrange - Done in setup

            // Act
            var backupsDir = await _pathProvider.GetSaveBackupsDirectoryAsync();

            // Assert
            StringAssert.Contains(backupsDir.ToLower(), "savebackups");
            Assert.IsTrue(_fileSystemOps.Directory.Exists(backupsDir));
        }

        [TestMethod]
        public async Task GetLogsDirectoryAsync_ShouldCreateAndReturnLogsFolder()
        {
            // Arrange - Done in setup

            // Act
            var logsDir = await _pathProvider.GetLogsDirectoryAsync();

            // Assert
            StringAssert.Contains(logsDir.ToLower(), "logs");
            Assert.IsTrue(_fileSystemOps.Directory.Exists(logsDir));
        }

        [TestMethod]
        public async Task GetExportDirectoryAsync_ShouldCreateAndReturnExportsFolder()
        {
            // Arrange - Done in setup

            // Act
            var exportsDir = await _pathProvider.GetExportDirectoryAsync();

            // Assert
            StringAssert.Contains(exportsDir.ToLower(), "exports");
            Assert.IsTrue(_fileSystemOps.Directory.Exists(exportsDir));
        }

        [TestMethod]
        public async Task GetSpecialFolderPathAsync_ShouldReturnCorrectPath()
        {
            // Arrange - Done in setup

            // Act
            var personalDir = await _pathProvider.GetSpecialFolderPathAsync(Environment.SpecialFolder.Personal);

            // Assert
            StringAssert.Contains(personalDir.ToLower(), "documents");
            Assert.IsTrue(_fileSystemOps.Directory.Exists(personalDir));
        }

        [TestMethod]
        public async Task IsBalatroSaveFilePathAsync_WithValidPath_ShouldReturnTrue()
        {
            // Arrange
            var savePath = Path.Combine(_fixture.GetBalatroSavePath(), "save1.lua");
            _fixture.FileSystem.CreateFile(savePath, "-- Balatro save file 1");

            // Act
            var isBalatroSave = await _pathProvider.IsBalatroSaveFilePathAsync(savePath);

            // Assert
            Assert.IsTrue(isBalatroSave);
        }

        [TestMethod]
        public async Task IsBalatroSaveFilePathAsync_WithInvalidPath_ShouldReturnFalse()
        {
            // Arrange
            var nonSavePath = Path.Combine(_fixture.GetBalatroSavePath(), "not_a_save_file.txt");
            _fixture.FileSystem.CreateFile(nonSavePath, "This is not a Balatro save file");

            // Act
            var isBalatroSave = await _pathProvider.IsBalatroSaveFilePathAsync(nonSavePath);

            // Assert
            Assert.IsFalse(isBalatroSave);
        }

        [TestMethod]
        public async Task IsBalatroSaveFilePathAsync_WithSymlinkPath_ShouldHandleSymlinks()
        {
            // Arrange
            var symlinkPath = Path.Combine(_fixture.GetBalatroSavePath(), "symlink_save.lua");

            // Act
            var isBalatroSave = await _pathProvider.IsBalatroSaveFilePathAsync(symlinkPath);

            // Assert
            // In our mock, we can't fully test symlinks, but we ensure the method doesn't fail
            Assert.IsFalse(isBalatroSave);
        }

        [TestMethod]
        public void TryFindSteamInstallation_ShouldReturnValidSteamPath()
        {
            // Arrange - Done in setup

            // Act - Find and use the TryFindSteamInstallation method via reflection
            var method = _pathProvider.GetType().GetMethod("TryFindSteamInstallation",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Create a parameter for the out parameter
            object[] parameters = new object[1];
            var result = method.Invoke(_pathProvider, parameters);
            bool success = (bool)result;
            string steamPath = parameters[0] as string;

            // Assert
            Assert.IsTrue(success);
            Assert.IsNotNull(steamPath);
            StringAssert.Contains(steamPath.ToLower(), "steam");
        }

        [TestMethod]
        public void TryFindFlatpakSteamPath_ShouldReturnValidFlatpakPath()
        {
            // Arrange - Done in setup

            // Act - Find and use the TryFindFlatpakSteamPath method via reflection
            var method = _pathProvider.GetType().GetMethod("TryFindFlatpakSteamPath",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Create a parameter for the out parameter
            object[] parameters = new object[1];
            var result = method.Invoke(_pathProvider, parameters);
            bool success = (bool)result;
            string flatpakPath = parameters[0] as string;

            // Assert
            Assert.IsTrue(success);
            Assert.IsNotNull(flatpakPath);
            StringAssert.Contains(flatpakPath.ToLower(), ".var/app/com.valvesoftware.steam");
        }

        [TestMethod]
        public void TryFindProtonPrefixes_ShouldFindProtonPrefix()
        {
            // Arrange
            string steamPath = _fixture.GetUserProfilePath() + "/.steam/steam";

            // Act - Find and use the TryFindProtonPrefixes method via reflection
            var method = _pathProvider.GetType().GetMethod("TryFindProtonPrefixes",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var protonPaths = method.Invoke(_pathProvider, new object[] { steamPath }) as List<string>;

            // Assert
            Assert.IsNotNull(protonPaths);
            Assert.IsTrue(protonPaths.Count > 0);

            var protonPath = protonPaths[0];
            StringAssert.Contains(protonPath.ToLower(), "compatdata");
            StringAssert.Contains(protonPath.ToLower(), "1804740");
            StringAssert.Contains(protonPath.ToLower(), "pfx");
        }

        [TestMethod]
        public void GetXdgDataHome_ShouldReturnCorrectPath()
        {
            // Arrange - Done in setup

            // Act - Find and use the GetXdgDataHome method via reflection
            var method = _pathProvider.GetType().GetMethod("GetXdgDataHome",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var xdgDataHome = method.Invoke(_pathProvider, null) as string;

            // Assert
            Assert.IsNotNull(xdgDataHome);
            Assert.AreEqual(_fixture.GetUserProfilePath() + "/.local/share", xdgDataHome);
        }

        [TestMethod]
        public void GetXdgConfigHome_ShouldReturnCorrectPath()
        {
            // Arrange - Done in setup

            // Act - Find and use the GetXdgConfigHome method via reflection
            var method = _pathProvider.GetType().GetMethod("GetXdgConfigHome",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var xdgConfigHome = method.Invoke(_pathProvider, null) as string;

            // Assert
            Assert.IsNotNull(xdgConfigHome);
            Assert.AreEqual(_fixture.GetUserProfilePath() + "/.config", xdgConfigHome);
        }

        [TestMethod]
        public void GetXdgCacheHome_ShouldReturnCorrectPath()
        {
            // Arrange - Done in setup

            // Act - Find and use the GetXdgCacheHome method via reflection
            var method = _pathProvider.GetType().GetMethod("GetXdgCacheHome",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var xdgCacheHome = method.Invoke(_pathProvider, null) as string;

            // Assert
            Assert.IsNotNull(xdgCacheHome);
            Assert.AreEqual(_fixture.GetUserProfilePath() + "/.cache", xdgCacheHome);
        }
    }
}
