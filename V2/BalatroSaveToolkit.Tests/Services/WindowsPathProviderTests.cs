using BalatroSaveToolkit.Services.Implementations.Windows;
using BalatroSaveToolkit.Services.Interfaces;
using Moq;
using System.Runtime.Versioning;

namespace BalatroSaveToolkit.Tests.Services
{
    [SupportedOSPlatform("windows")]
    [TestClass]
    public class WindowsPathProviderTests
    {
        private Mock<ILogService> _mockLogService;
        private Mock<IErrorHandlingService> _mockErrorHandler;
        private WindowsPathProvider _pathProvider;

        [TestInitialize]
        public void Setup()
        {
            _mockLogService = new Mock<ILogService>();
            _mockErrorHandler = new Mock<IErrorHandlingService>();
            _pathProvider = new WindowsPathProvider(_mockLogService.Object, _mockErrorHandler.Object);
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
        public async Task GetBalatroSaveDirectoryAsync_ShouldReturnValidDirectory()
        {
            // Arrange
            // Act
            string saveDir = await _pathProvider.GetBalatroSaveDirectoryAsync();

            // Assert
            Assert.IsNotNull(saveDir);
            Assert.IsTrue(Directory.Exists(saveDir));
            StringAssert.Contains(saveDir.ToLower(), "balatro");
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
