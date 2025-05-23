using System;
using System.Threading;
using BalatroSaveToolkit.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BalatroSaveToolkit.Tests.ViewModels
{
    /// <summary>
    /// Unit tests for the SaveFileViewModel.
    /// </summary>
    [TestClass]
    public class SaveFileViewModelTests
    {        [TestMethod]
        public void DisplayName_ShouldFormat_ProfileNumberAndTimestamp()
        {
            // Arrange
            var sut = new SaveFileViewModel
            {
                ProfileNumber = 2,
                Timestamp = new DateTime(2025, 5, 20, 13, 45, 30)
            };

            // Wait for observable property to update
            Thread.Sleep(50);

            // Act
            var displayName = sut.DisplayName;

            // Assert
            Assert.AreEqual("Profile 2 - 2025-05-20 13:45:30", displayName);
        }

        [TestMethod]
        public void FormattedFileSize_ShouldFormat_Bytes()
        {
            // Arrange
            var sut = new SaveFileViewModel
            {
                FileSize = 512
            };

            // Wait for observable property to update
            Thread.Sleep(50);

            // Act
            var formattedSize = sut.FormattedFileSize;

            // Assert
            Assert.AreEqual("512 B", formattedSize);
        }

        [TestMethod]
        public void FormattedFileSize_ShouldFormat_KiloBytes()
        {
            // Arrange
            var sut = new SaveFileViewModel
            {
                FileSize = 2048
            };

            // Wait for observable property to update
            Thread.Sleep(50);

            // Act
            var formattedSize = sut.FormattedFileSize;

            // Assert
            Assert.AreEqual("2.00 KB", formattedSize);
        }

        [TestMethod]
        public void FormattedFileSize_ShouldFormat_MegaBytes()
        {
            // Arrange
            var sut = new SaveFileViewModel
            {
                FileSize = 3 * 1024 * 1024
            };

            // Wait for observable property to update
            Thread.Sleep(50);

            // Act
            var formattedSize = sut.FormattedFileSize;

            // Assert
            Assert.AreEqual("3.00 MB", formattedSize);
        }

        [TestMethod]
        public void ChangingProperties_ShouldUpdate_DerivedProperties()
        {
            // Arrange
            var sut = new SaveFileViewModel
            {
                ProfileNumber = 1,
                Timestamp = new DateTime(2025, 5, 1, 10, 0, 0),
                FileSize = 1024
            };

            // Wait for observable property to update
            Thread.Sleep(50);

            var initialDisplayName = sut.DisplayName;
            var initialFormattedSize = sut.FormattedFileSize;

            // Act
            sut.ProfileNumber = 3;
            sut.Timestamp = new DateTime(2025, 5, 20, 15, 30, 0);
            sut.FileSize = 3 * 1024 * 1024;

            // Wait for observable property to update
            Thread.Sleep(50);

            // Assert
            Assert.AreNotEqual(initialDisplayName, sut.DisplayName);
            Assert.AreNotEqual(initialFormattedSize, sut.FormattedFileSize);
            Assert.AreEqual("Profile 3 - 2025-05-20 15:30:00", sut.DisplayName);
            Assert.AreEqual("3.00 MB", sut.FormattedFileSize);
        }
    }
}
