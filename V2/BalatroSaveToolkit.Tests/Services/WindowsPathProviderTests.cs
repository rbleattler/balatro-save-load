using BalatroSaveToolkit.Services.Implementations.Windows;
using BalatroSaveToolkit.Services.Interfaces;
using BalatroSaveToolkit.Tests.Mocks;
using BalatroSaveToolkit.Tests.TestFixtures;
using Moq;
using System.Reflection;
using System.Runtime.Versioning;

namespace BalatroSaveToolkit.Tests.Services
{
    [SupportedOSPlatform("windows")]
    [TestClass]
    public class WindowsPathProviderTests
    {
        private WindowsPathProviderTestFixture _fixture;
        private WindowsPathProvider _pathProvider;
        private FileSystemMockOperations _fileSystemOps;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new WindowsPathProviderTestFixture();
            _fixture.SetupWindowsFileSystem();

            _fileSystemOps = _fixture.FileSystemOps;

            // Create the path provider with mocked dependencies
            _pathProvider = new WindowsPathProvider(
                _fixture.MockLogService.Object,
                _fixture.MockErrorHandler.Object);

            // Use reflection to inject the mocked file system utilities
            // (since the path provider uses static File, Directory, and Environment methods)
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
        public void IsCaseSensitive_ShouldBeFalse()
        {
            // Arrange & Act & Assert
            Assert.IsFalse(_pathProvider.IsCaseSensitive);
        }

        [TestMethod]
        public void PathSeparator_ShouldBeBackslash()
        {
            // Arrange & Act & Assert
            Assert.AreEqual('\\', _pathProvider.PathSeparator);
        }

        [TestMethod]
        public async Task GetBalatroSaveDirectoryAsync_ShouldReturnCorrectDirectory()
        {
            // Arrange - Done in setup

            // Act
            var saveDir = await _pathProvider.GetBalatroSaveDirectoryAsync();

            // Assert
            Assert.AreEqual(_fixture.GetUserProfilePath() + @"\AppData\Local\Balatro", saveDir);
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
        public void NormalizeWindowsPath_ShouldCorrectlyFormatPaths()
        {
            // Arrange
            var testCases = new Dictionary<string, string>
            {
                { "c:/test/path", "C:\\test\\path" },
                { "C:\\test\\path\\", "C:\\test\\path" },
                { "c:\\test\\PATH", "C:\\test\\PATH" }, // Case preserved for path components
                { "C:\\", "C:\\" }, // Drive root preserved
            };

            // Act & Assert
            foreach (var testCase in testCases)
            {
                string normalized = _pathProvider.NormalizeWindowsPath(testCase.Key);
                Assert.AreEqual(testCase.Value, normalized);
            }
        }

        [TestMethod]
        public async Task GetApplicationDataDirectoryAsync_ShouldReturnLocalAppDataPath()
        {
            // Arrange
            const string testAppName = "TestApp";

            // Act
            string appDataDir = await _pathProvider.GetApplicationDataDirectoryAsync(testAppName);

            // Assert
            Assert.IsNotNull(appDataDir);
            Assert.IsTrue(Directory.Exists(appDataDir));
            StringAssert.Contains(appDataDir.ToLower(), testAppName.ToLower());

            // Cleanup
            try { Directory.Delete(appDataDir); }
            catch { /* Ignore cleanup errors */ }
        }

        [TestMethod]
        public async Task GetApplicationDataDirectoryAsync_ShouldReturnCorrectPath()
        {
            // Arrange - Done in setup

            // Act
            var appDataDir = await _pathProvider.GetApplicationDataDirectoryAsync("TestApp");

            // Assert
            StringAssert.Contains(appDataDir, "AppData");
            StringAssert.Contains(appDataDir, "TestApp");
        }

        [TestMethod]
        public async Task GetApplicationDataDirectoryAsync_WithDefaultAppName_ShouldUseDefaultName()
        {
            // Arrange - Done in setup

            // Act
            var appDataDir = await _pathProvider.GetApplicationDataDirectoryAsync();

            // Assert
            StringAssert.Contains(appDataDir, "AppData");
            StringAssert.Contains(appDataDir, "BalatroSaveToolkit");
        }

        [TestMethod]
        public async Task GetTempDirectoryAsync_ShouldReturnValidDirectory()
        {
            // Arrange
            // Act
            string tempDir = await _pathProvider.GetTempDirectoryAsync();

            // Assert
            Assert.IsNotNull(tempDir);
            Assert.IsTrue(Directory.Exists(tempDir));
        }

        [TestMethod]
        public async Task GetTempDirectoryAsync_ShouldReturnTempFolder()
        {
            // Arrange - Done in setup

            // Act
            var tempDir = await _pathProvider.GetTempDirectoryAsync();

            // Assert
            StringAssert.Contains(tempDir.ToLower(), "temp");
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
            StringAssert.Contains(homeDir.ToLower(), "users");
            Assert.IsTrue(_fileSystemOps.Directory.Exists(homeDir));
        }

        [TestMethod]
        public async Task GetSaveBackupsDirectoryAsync_ShouldCreateDirectoryIfNotExists()
        {
            // Arrange
            // Act
            string backupDir = await _pathProvider.GetSaveBackupsDirectoryAsync();

            // Assert
            Assert.IsNotNull(backupDir);
            Assert.IsTrue(Directory.Exists(backupDir));
            StringAssert.Contains(backupDir.ToLower(), "savebackups");
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
            string saveDir = await _pathProvider.GetBalatroSaveDirectoryAsync();
            string testFilePath = Path.Combine(saveDir, "test_save.save");

            // Create a test file with basic Balatro save structure
            string content = "{\"seed\": 123, \"deck\": [], \"jokers\": []}";
            File.WriteAllText(testFilePath, content);

            try
            {
                // Act
                bool result = await _pathProvider.IsBalatroSaveFilePathAsync(testFilePath);

                // Assert
                Assert.IsTrue(result);
            }
            finally
            {
                // Cleanup
                if (File.Exists(testFilePath))
                {
                    File.Delete(testFilePath);
                }
            }
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
            string nonExistentPath = "Z:\\this\\path\\should\\not\\exist\\balatro.save";

            // Act
            bool result = await _pathProvider.IsBalatroSaveFilePathAsync(nonExistentPath);

            // Assert
            Assert.IsFalse(result);
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
        public void NormalizeWindowsPath_WithMixedSeparators_ShouldNormalizeToWindowsFormat()
        {
            // Arrange
            string mixedPath = @"C:/Users\TestUser/AppData\Local/Balatro";

            // Act - Find and use the NormalizeWindowsPath method via reflection
            var method = _pathProvider.GetType().GetMethod("NormalizeWindowsPath",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var normalizedPath = method.Invoke(_pathProvider, new object[] { mixedPath }) as string;

            // Assert
            Assert.AreEqual(@"C:\Users\TestUser\AppData\Local\Balatro", normalizedPath);
        }

        [TestMethod]
        public void NormalizeWindowsPath_WithTrailingSeparator_ShouldRemoveTrailingSeparator()
        {
            // Arrange
            string pathWithTrailingSeparator = @"C:\Users\TestUser\AppData\Local\Balatro\";

            // Act - Find and use the NormalizeWindowsPath method via reflection
            var method = _pathProvider.GetType().GetMethod("NormalizeWindowsPath",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var normalizedPath = method.Invoke(_pathProvider, new object[] { pathWithTrailingSeparator }) as string;

            // Assert
            Assert.AreEqual(@"C:\Users\TestUser\AppData\Local\Balatro", normalizedPath);
        }

        [TestMethod]
        public void NormalizeWindowsPath_WithDoubleSeparators_ShouldNormalizeSeparators()
        {
            // Arrange
            string pathWithDoubleSeparators = @"C:\\Users\\TestUser\\AppData\\Local\\Balatro";

            // Act - Find and use the NormalizeWindowsPath method via reflection
            var method = _pathProvider.GetType().GetMethod("NormalizeWindowsPath",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var normalizedPath = method.Invoke(_pathProvider, new object[] { pathWithDoubleSeparators }) as string;

            // Assert
            Assert.AreEqual(@"C:\Users\TestUser\AppData\Local\Balatro", normalizedPath);
        }

        [TestMethod]
        public async Task GetBalatroSaveDirectoryAsync_DirectoryDoesNotExist_ShouldThrowException()
        {
            // Arrange - Setup a scenario where the Balatro directory doesn't exist
            var originalPath = _fixture.GetBalatroSavePath();

            // Remove the directory from our mock
            var directoryMethod = _fixture.FileSystemOps.DirectoryMock.Setup(d => d.Exists(originalPath))
                .Returns(false);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<DirectoryNotFoundException>(() =>
                _pathProvider.GetBalatroSaveDirectoryAsync());
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
        public void FindSteamUserData_WithValidSteamPath_ShouldReturnSaveDataPath()
        {
            // Arrange
            string steamPath = @"C:\Program Files (x86)\Steam";

            // Act - Find and use the FindSteamUserData method via reflection
            var method = _pathProvider.GetType().GetMethod("FindSteamUserData",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var saveDataPath = method.Invoke(_pathProvider, new object[] { steamPath }) as string;

            // Assert
            Assert.IsNotNull(saveDataPath);
            StringAssert.Contains(saveDataPath.ToLower(), "steam");
            StringAssert.Contains(saveDataPath.ToLower(), "userdata");
            StringAssert.Contains(saveDataPath.ToLower(), "1804740");
        }

        [TestMethod]
        public void IsLongPath_WithLongPath_ShouldReturnTrue()
        {
            // Arrange
            string longPath = new string('a', 248) + @"\file.txt"; // 248 + 9 = 257 chars (> MAX_PATH)

            // Act - Find and use the IsLongPath method via reflection
            var method = _pathProvider.GetType().GetMethod("IsLongPath",
                BindingFlags.NonPublic | BindingFlags.Static);
            var isLongPath = (bool)method.Invoke(_pathProvider, new object[] { longPath });

            // Assert
            Assert.IsTrue(isLongPath);
        }

        [TestMethod]
        public void IsLongPath_WithShortPath_ShouldReturnFalse()
        {
            // Arrange
            string shortPath = @"C:\Users\TestUser\file.txt";

            // Act - Find and use the IsLongPath method via reflection
            var method = _pathProvider.GetType().GetMethod("IsLongPath",
                BindingFlags.NonPublic | BindingFlags.Static);
            var isLongPath = (bool)method.Invoke(_pathProvider, new object[] { shortPath });

            // Assert
            Assert.IsFalse(isLongPath);
        }

        [TestMethod]
        public void IsOnRemovableDrive_WithCDriveFolder_ShouldReturnFalse()
        {
            // Arrange
            string systemDrivePath = "C:\\Windows\\System32";

            // Act
            bool result = _pathProvider.IsOnRemovableDrive(systemDrivePath);

            // Assert
            Assert.IsFalse(result); // C: is typically not removable
        }
    }
}
