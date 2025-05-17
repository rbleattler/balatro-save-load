using BalatroSaveToolkit.Services.Implementations.Linux;
using BalatroSaveToolkit.Services.Interfaces;
using Moq;
using System.Runtime.Versioning;

namespace BalatroSaveToolkit.Tests.Services
{
    [SupportedOSPlatform("linux")]
    [TestClass]
    public class LinuxPathProviderTests
    {
        private Mock<ILogService> _mockLogService;
        private Mock<IErrorHandlingService> _mockErrorHandler;
        private LinuxPathProvider _pathProvider;

        [TestInitialize]
        public void Setup()
        {
            _mockLogService = new Mock<ILogService>();
            _mockErrorHandler = new Mock<IErrorHandlingService>();
            _pathProvider = new LinuxPathProvider(_mockLogService.Object, _mockErrorHandler.Object);
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
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string testPath = "~/.local/share/Balatro";

            // Act
            string resolvedPath = _pathProvider.ResolveLinuxPath(testPath);

            // Assert
            Assert.IsFalse(resolvedPath.Contains('~'));
            Assert.IsTrue(resolvedPath.StartsWith(homePath));
            StringAssert.Contains(resolvedPath, ".local/share/Balatro");
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
    }
}
