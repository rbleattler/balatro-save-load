using System.Collections.Generic;
using System.Threading.Tasks;
using BalatroSaveToolkit.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BalatroSaveToolkit.Tests.Services
{
    /// <summary>
    /// Tests for the MockFileSystemService to ensure it behaves as expected.
    /// </summary>
    [TestClass]
    public class MockFileSystemServiceTests    {
        [TestMethod]
        public async Task GetSavedBackupFilesAsync_ShouldReturn_ConfiguredFiles()
        {
            // Arrange
            var sut = new MockFileSystemService();
            var expectedFiles = new List<string>
            {
                "/mock/backups/profile1_20250520.sav",
                "/mock/backups/profile2_20250520.sav"
            };

            sut.SetupBackupFiles(expectedFiles);

            // Act
            var result = await sut.GetSavedBackupFilesAsync();

            // Assert
            Assert.AreEqual(expectedFiles.Count, result.Count);
            CollectionAssert.AreEqual(expectedFiles, result);
        }

        [TestMethod]
        public void BackupSaveFile_ShouldAdd_ToBackupFiles()
        {
            // Arrange
            var sut = new MockFileSystemService();
            sut.SetupBackupFiles(new List<string>());
            sut.SetupCurrentSaveFile(1, "/mock/saves/profile1.sav");
            sut.SetupFileContent("/mock/saves/profile1.sav", "test content");

            // Act
            var backupPath = sut.BackupSaveFile(1);

            // Assert
            Assert.IsNotNull(backupPath);
            Assert.IsTrue(backupPath.Length > 0);
            Assert.IsTrue(backupPath.Contains("/mock/balatro/backups/profile1_"));
        }

        [TestMethod]
        public async Task SaveBackupAsync_ShouldReturn_BackupPath()
        {
            // Arrange
            var sut = new MockFileSystemService();
            sut.SetupBackupFiles(new List<string>());
            sut.SetupCurrentSaveFile(2, "/mock/saves/profile2.sav");

            // Act
            var backupPath = await sut.SaveBackupAsync(2);

            // Assert
            Assert.IsNotNull(backupPath);
            Assert.IsTrue(backupPath.Length > 0);
            Assert.IsTrue(backupPath.Contains("/mock/balatro/backups/profile2_"));
        }

        [TestMethod]
        public void RestoreSaveFile_ShouldReturn_False_WhenBackupNotFound()
        {
            // Arrange
            var sut = new MockFileSystemService();
            sut.SetupBackupFiles(new List<string>());

            // Act
            var result = sut.RestoreSaveFile("/mock/backups/nonexistent.sav", 1);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetSaveFileContentAsync_ShouldReturn_ConfiguredContent()
        {
            // Arrange
            var sut = new MockFileSystemService();
            var expectedContent = "test save file content";
            sut.SetupFileContent("/mock/saves/profile1.sav", expectedContent);

            // Act
            var result = await sut.GetSaveFileContentAsync("/mock/saves/profile1.sav");

            // Assert
            Assert.AreEqual(expectedContent, result);
        }

        [TestMethod]
        public void OpenSaveDirectory_ShouldSet_SaveDirectoryOpened()
        {
            // Arrange
            var sut = new MockFileSystemService();

            // Act
            sut.OpenSaveDirectory();

            // Assert
            Assert.IsTrue(sut.SaveDirectoryOpened);
        }
    }
}
