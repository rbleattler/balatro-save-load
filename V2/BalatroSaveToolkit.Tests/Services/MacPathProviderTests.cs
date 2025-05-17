using BalatroSaveToolkit.Services.Implementations.MacCatalyst;
using BalatroSaveToolkit.Services.Interfaces;
using BalatroSaveToolkit.Tests.Mocks;
using BalatroSaveToolkit.Tests.TestFixtures;
using Moq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;

namespace BalatroSaveToolkit.Tests.Services
{
    [SupportedOSPlatform("maccatalyst")]
    [TestClass]
    public class MacPathProviderTests
    {
        private MacPathProviderTestFixture _fixture;
        private MacPathProvider _pathProvider;
        private FileSystemMockOperations _fileSystemOps;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new MacPathProviderTestFixture();
            _fixture.SetupMacFileSystem();

            _fileSystemOps = _fixture.FileSystemOps;

            // Create the path provider with mocked dependencies
            _pathProvider = new MacPathProvider(
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
        public void ResolveMacOSPath_ShouldReplaceHomeTilde()
        {
            // Arrange
            string homePath = _fixture.GetUserProfilePath();
            string testPath = "~/Library/Application Support/Balatro";

            // Act
            string resolvedPath = _pathProvider.ResolveMacOSPath(testPath);

            // Assert
            Assert.IsFalse(resolvedPath.Contains('~'));
            Assert.IsTrue(resolvedPath.StartsWith(homePath));
            StringAssert.Contains(resolvedPath, "Library/Application Support/Balatro");
        }

        [TestMethod]
        public void ResolveMacOSPath_WithoutTilde_ShouldNotModifyPath()
        {
            // Arrange
            string testPath = "/Library/Application Support/Balatro";

            // Act
            string resolvedPath = _pathProvider.ResolveMacOSPath(testPath);

            // Assert
            Assert.AreEqual(testPath, resolvedPath);
        }

        [TestMethod]
        public void NormalizeMacPath_ShouldConvertBackslashesToForwardSlashes()
        {
            // Arrange
            string testPath = "Users\\Test\\Documents";

            // Act
            string normalizedPath = _pathProvider.NormalizeMacPath(testPath);

            // Assert
            Assert.IsFalse(normalizedPath.Contains('\\'));
            Assert.IsTrue(normalizedPath.Contains('/'));
        }

        [TestMethod]
        public void NormalizeMacPath_ShouldRemoveTrailingSlash()
        {
            // Arrange
            string testPath = "/Users/Test/Documents/";

            // Act
            string normalizedPath = _pathProvider.NormalizeMacPath(testPath);

            // Assert
            Assert.AreEqual("/Users/Test/Documents", normalizedPath);
        }

        [TestMethod]
        public void NormalizeMacPath_ShouldNotRemoveSlashForRootDir()
        {
            // Arrange
            string testPath = "/";

            // Act
            string normalizedPath = _pathProvider.NormalizeMacPath(testPath);

            // Assert
            Assert.AreEqual("/", normalizedPath);
        }

        [TestMethod]
        public void NormalizeMacPath_ShouldApplyNfdNormalization()
        {
            // Arrange
            // á can be represented as a single character (NFC) or as 'a' followed by combining accent (NFD)
            string testPathNfc = "/Users/Test/Café"; // Uses NFC form

            // Act
            string normalizedPath = _pathProvider.NormalizeMacPath(testPathNfc);

            // Assert
            // Should be normalized to NFD (more code points than original)
            Assert.AreEqual(testPathNfc.Length + 1, normalizedPath.Length);

            // The é should be decomposed to e + combining accent
            Assert.AreEqual(
                "/Users/Test/Caf" + "\u0065\u0301", // e + combining accent
                normalizedPath);
        }

        [TestMethod]
        public void NormalizeMacPath_WithMixedSeparators_ShouldNormalizeToForwardSlashes()
        {
            // Arrange
            string mixedPath = "/Users\\testuser/Library\\Application Support/Balatro";

            // Act - Find and use the NormalizeMacPath method via reflection
            var method = _pathProvider.GetType().GetMethod("NormalizeMacPath",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var normalizedPath = method.Invoke(_pathProvider, new object[] { mixedPath }) as string;

            // Assert
            Assert.AreEqual("/Users/testuser/Library/Application Support/Balatro", normalizedPath);
        }

        [TestMethod]
        public void NormalizeMacPath_WithTrailingSeparator_ShouldRemoveTrailingSeparator()
        {
            // Arrange
            string pathWithTrailingSeparator = "/Users/testuser/Library/Application Support/Balatro/";

            // Act - Find and use the NormalizeMacPath method via reflection
            var method = _pathProvider.GetType().GetMethod("NormalizeMacPath",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var normalizedPath = method.Invoke(_pathProvider, new object[] { pathWithTrailingSeparator }) as string;

            // Assert
            Assert.AreEqual("/Users/testuser/Library/Application Support/Balatro", normalizedPath);
        }

        [TestMethod]
        public void NormalizeMacPath_WithDoubleSeparators_ShouldNormalizeSeparators()
        {
            // Arrange
            string pathWithDoubleSeparators = "/Users//testuser/Library//Application Support/Balatro";

            // Act - Find and use the NormalizeMacPath method via reflection
            var method = _pathProvider.GetType().GetMethod("NormalizeMacPath",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var normalizedPath = method.Invoke(_pathProvider, new object[] { pathWithDoubleSeparators }) as string;

            // Assert
            Assert.AreEqual("/Users/testuser/Library/Application Support/Balatro", normalizedPath);
        }

        [TestMethod]
        public void HasResourceFork_WithNonExistentFile_ShouldReturnFalse()
        {
            // Arrange
            string nonExistentPath = "/Users/Test/Documents/nonexistent.file";

            // Act
            bool result = _pathProvider.HasResourceFork(nonExistentPath);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [SupportedOSPlatform("macos")]
        public async Task GetApplicationDataDirectoryAsync_ShouldReturnLibraryApplicationSupport()
        {
            // This test will only run on actual macOS systems
            if (!OperatingSystem.IsMacOS())
            {
                Assert.Inconclusive("This test only runs on macOS");
                return;
            }

            // Arrange
            const string testAppName = "TestApp";

            // Act
            string appDataDir = await _pathProvider.GetApplicationDataDirectoryAsync(testAppName);

            // Assert
            Assert.IsNotNull(appDataDir);
            Assert.IsTrue(appDataDir.Contains("Library/Application Support"));
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
            StringAssert.Contains(appDataDir, "Library/Application Support");
            StringAssert.Contains(appDataDir, "TestApp");
        }

        [TestMethod]
        public async Task GetApplicationDataDirectoryAsync_WithDefaultAppName_ShouldUseDefaultName()
        {
            // Arrange - Done in setup

            // Act
            var appDataDir = await _pathProvider.GetApplicationDataDirectoryAsync();

            // Assert
            StringAssert.Contains(appDataDir, "Library/Application Support");
            StringAssert.Contains(appDataDir, "BalatroSaveToolkit");
        }

        [TestMethod]
        public async Task GetTempDirectoryAsync_ShouldReturnTempFolder()
        {
            // Arrange - Done in setup

            // Act
            var tempDir = await _pathProvider.GetTempDirectoryAsync();

            // Assert
            StringAssert.Contains(tempDir.ToLower(), "temporaryitems");
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
        public async Task IsBalatroSaveFilePathAsync_WithResourceFork_ShouldReturnFalse()
        {
            // Arrange
            var resourceForkPath = Path.Combine(_fixture.GetBalatroSavePath(), "._save1.lua");
            _fixture.FileSystem.CreateFile(resourceForkPath, "Resource fork data");

            // Act
            var isBalatroSave = await _pathProvider.IsBalatroSaveFilePathAsync(resourceForkPath);

            // Assert
            Assert.IsFalse(isBalatroSave);
        }

        [TestMethod]
        public void IsResourceFork_WithResourceForkFile_ShouldReturnTrue()
        {
            // Arrange
            string resourceForkPath = "._save1.lua";

            // Act - Find and use the IsResourceFork method via reflection
            var method = _pathProvider.GetType().GetMethod("IsResourceFork",
                BindingFlags.NonPublic | BindingFlags.Static);
            var isResourceFork = (bool)method.Invoke(_pathProvider, new object[] { resourceForkPath });

            // Assert
            Assert.IsTrue(isResourceFork);
        }

        [TestMethod]
        public void IsResourceFork_WithNormalFile_ShouldReturnFalse()
        {
            // Arrange
            string normalPath = "save1.lua";

            // Act - Find and use the IsResourceFork method via reflection
            var method = _pathProvider.GetType().GetMethod("IsResourceFork",
                BindingFlags.NonPublic | BindingFlags.Static);
            var isResourceFork = (bool)method.Invoke(_pathProvider, new object[] { normalPath });

            // Assert
            Assert.IsFalse(isResourceFork);
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
        public void NormalizeUnicodeForMac_ShouldHandleUnicodeCorrectly()
        {
            // Arrange
            string unicodeString = "éñçödéd ßtring";

            // Act - Find and use the NormalizeUnicodeForMac method via reflection
            var method = _pathProvider.GetType().GetMethod("NormalizeUnicodeForMac",
                BindingFlags.NonPublic | BindingFlags.Static);
            var normalizedString = method.Invoke(_pathProvider, new object[] { unicodeString }) as string;

            // Assert
            Assert.IsNotNull(normalizedString);
            Assert.AreNotEqual(normalizedString, unicodeString);
        }
    }
}
