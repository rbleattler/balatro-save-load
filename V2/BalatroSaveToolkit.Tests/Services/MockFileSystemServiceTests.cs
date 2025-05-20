using System.Collections.Generic;
using System.Threading.Tasks;
using BalatroSaveToolkit.Tests.Mocks;
using Xunit;

namespace BalatroSaveToolkit.Tests.Services
{
    /// <summary>
    /// Tests for the MockFileSystemService to ensure it behaves as expected.
    /// </summary>
    public class MockFileSystemServiceTests
    {
        [Fact]
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
            Assert.Equal(expectedFiles.Count, result.Count);
            Assert.Equal(expectedFiles, result);
        }

        [Fact]
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
            Assert.NotNull(backupPath);
            Assert.NotEmpty(backupPath);
            Assert.Contains("/mock/balatro/backups/profile1_", backupPath);
        }

        [Fact]
        public async Task SaveBackupAsync_ShouldReturn_BackupPath()
        {
            // Arrange
            var sut = new MockFileSystemService();
            sut.SetupBackupFiles(new List<string>());
            sut.SetupCurrentSaveFile(2, "/mock/saves/profile2.sav");

            // Act
            var backupPath = await sut.SaveBackupAsync(2);

            // Assert
            Assert.NotNull(backupPath);
            Assert.NotEmpty(backupPath);
            Assert.Contains("/mock/balatro/backups/profile2_", backupPath);
        }

        [Fact]
        public void RestoreSaveFile_ShouldReturn_False_WhenBackupNotFound()
        {
            // Arrange
            var sut = new MockFileSystemService();
            sut.SetupBackupFiles(new List<string>());

            // Act
            var result = sut.RestoreSaveFile("/mock/backups/nonexistent.sav", 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetSaveFileContentAsync_ShouldReturn_ConfiguredContent()
        {
            // Arrange
            var sut = new MockFileSystemService();
            var expectedContent = "test save file content";
            sut.SetupFileContent("/mock/saves/profile1.sav", expectedContent);

            // Act
            var result = await sut.GetSaveFileContentAsync("/mock/saves/profile1.sav");

            // Assert
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public void OpenSaveDirectory_ShouldSet_SaveDirectoryOpened()
        {
            // Arrange
            var sut = new MockFileSystemService();

            // Act
            sut.OpenSaveDirectory();

            // Assert
            Assert.True(sut.SaveDirectoryOpened);
        }
    }
}
