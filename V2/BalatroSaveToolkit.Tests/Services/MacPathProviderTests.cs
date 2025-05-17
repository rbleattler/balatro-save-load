using BalatroSaveToolkit.Services.Implementations.MacCatalyst;
using BalatroSaveToolkit.Services.Interfaces;
using Moq;
using System.Runtime.Versioning;
using System.Text;

namespace BalatroSaveToolkit.Tests.Services
{
    [SupportedOSPlatform("maccatalyst")]
    [TestClass]
    public class MacPathProviderTests
    {
        private Mock<ILogService> _mockLogService;
        private Mock<IErrorHandlingService> _mockErrorHandler;
        private MacPathProvider _pathProvider;

        [TestInitialize]
        public void Setup()
        {
            _mockLogService = new Mock<ILogService>();
            _mockErrorHandler = new Mock<IErrorHandlingService>();
            _pathProvider = new MacPathProvider(_mockLogService.Object, _mockErrorHandler.Object);
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
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string testPath = "~/Library/Application Support/Balatro";

            // Act
            string resolvedPath = _pathProvider.ResolveMacOSPath(testPath);

            // Assert
            Assert.IsFalse(resolvedPath.Contains('~'));
            Assert.IsTrue(resolvedPath.StartsWith(homePath));
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
    }
}
