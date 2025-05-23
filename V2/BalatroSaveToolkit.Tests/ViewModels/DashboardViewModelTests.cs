using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Tests.Mocks;
using BalatroSaveToolkit.ViewModels;
using ReactiveUI;
using Splat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BalatroSaveToolkit.Tests.ViewModels
{    /// <summary>
    /// Tests for the DashboardViewModel.
    /// </summary>
    [TestClass]
    public class DashboardViewModelTests
    {
        private MockFileSystemService _fileSystemService;
        private MockNotificationService _notificationService;
        private IMutableDependencyResolver _originalResolver;
        private IDependencyResolver _originalDependencyResolver;

        [TestInitialize]
        public void TestInitialize()
        {
            // Store original Splat resolver for cleanup
            _originalResolver = Locator.CurrentMutable;
            _originalDependencyResolver = (IDependencyResolver?)Locator.Current!;

            // Create a new dependency resolver for testing
            var resolver = new ModernDependencyResolver();
            Locator.SetLocator(resolver);

            // Set up mock services
            _fileSystemService = new MockFileSystemService();
            _notificationService = new MockNotificationService();

            // Register mock services with the locator
            Locator.CurrentMutable.RegisterConstant<IFileSystemService>(_fileSystemService);
            Locator.CurrentMutable.RegisterConstant<INotificationService>(_notificationService);
            Locator.CurrentMutable.RegisterConstant<IScreen>(new TestScreen());

            // Register logging service
            Locator.CurrentMutable.RegisterConstant<ILoggingService>(new MockLoggingService());

            // Set up some test data
            _fileSystemService.SetupBackupFiles(new List<string>
            {
                "/mock/balatro/backups/profile1_20250520_120000.sav",
                "/mock/balatro/backups/profile2_20250520_120000.sav"
            });            _fileSystemService.SetupCurrentSaveFile(1, "/mock/balatro/saves/profile1.sav");
            _fileSystemService.SetupCurrentSaveFile(2, "/mock/balatro/saves/profile2.sav");
        }

        [TestMethod]
        public async Task RefreshSaveFilesCommand_Should_PopulateSaveFiles()
        {
            // Arrange
            var sut = new DashboardViewModel(Locator.Current.GetService<IScreen>());

            // Act
            await sut.RefreshSaveFilesCommand.Execute();            // Assert
            Assert.AreEqual(2, sut.SaveFiles.Count);
        }

        [TestMethod]
        public async Task CreateBackupCommand_Should_CreateBackup_AndNotifyUser()
        {
            // Arrange
            var sut = new DashboardViewModel(Locator.Current.GetService<IScreen>());
            await sut.RefreshSaveFilesCommand.Execute();
            sut.SelectedSaveFile = sut.SaveFiles[0];

            // Act
            await sut.CreateBackupCommand.Execute();

            // Assert
            Assert.AreEqual(1, _notificationService.Notifications.Count);
            Assert.AreEqual("Success", _notificationService.Notifications[0].Type);            Assert.IsTrue(_notificationService.Notifications[0].Title.Contains("Backup Created"));
        }

        [TestMethod]
        public async Task LoadSaveCommand_Should_PromptForConfirmation()
        {
            // Arrange
            var sut = new DashboardViewModel(Locator.Current.GetService<IScreen>());
            await sut.RefreshSaveFilesCommand.Execute();
            sut.SelectedSaveFile = sut.SaveFiles[0];

            // Configure mock to simulate user confirmation
            _notificationService.ConfirmationResult = true;

            // Act
            await sut.LoadSaveCommand.Execute();

            // Assert
            Assert.IsTrue(_notificationService.Notifications.Any(                n => n.Type == "Confirmation" && n.Title == "Confirm Restore"));
        }

        [TestMethod]
        public async Task LoadSaveCommand_Should_NotRestoreSave_WhenUserCancels()
        {
            // Arrange
            var sut = new DashboardViewModel(Locator.Current.GetService<IScreen>());
            await sut.RefreshSaveFilesCommand.Execute();
            sut.SelectedSaveFile = sut.SaveFiles[0];

            // Configure mock to simulate user cancellation
            _notificationService.ConfirmationResult = false;

            // Act
            await sut.LoadSaveCommand.Execute();

            // Assert
            Assert.IsFalse(_notificationService.Notifications.Any(                n => n.Type == "Success" && n.Title == "Save Restored"));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Restore original Splat resolver
            Locator.SetLocator(_originalDependencyResolver);
        }
    }

    internal class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new RoutingState();
    }
}
