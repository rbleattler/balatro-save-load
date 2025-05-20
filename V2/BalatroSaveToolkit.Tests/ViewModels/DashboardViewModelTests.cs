using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Tests.Mocks;
using BalatroSaveToolkit.ViewModels;
using ReactiveUI;
using Splat;
using Xunit;

namespace BalatroSaveToolkit.Tests.ViewModels
{
    /// <summary>
    /// Tests for the DashboardViewModel.
    /// </summary>
    public class DashboardViewModelTests : IDisposable
    {
        private readonly MockFileSystemService _fileSystemService;
        private readonly MockNotificationService _notificationService;
        private readonly IMutableDependencyResolver _originalResolver;
        private readonly IDependencyResolver _originalDependencyResolver;

        public DashboardViewModelTests()
        {
            // Store original Splat resolver for cleanup
            _originalResolver = Locator.CurrentMutable;
            _originalDependencyResolver = Locator.Current;

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
            });

            _fileSystemService.SetupCurrentSaveFile(1, "/mock/balatro/saves/profile1.sav");
            _fileSystemService.SetupCurrentSaveFile(2, "/mock/balatro/saves/profile2.sav");
        }

        [Fact]
        public async Task RefreshSaveFilesCommand_Should_PopulateSaveFiles()
        {
            // Arrange
            var sut = new DashboardViewModel(Locator.Current.GetService<IScreen>());

            // Act
            await sut.RefreshSaveFilesCommand.Execute();

            // Assert
            Assert.Equal(2, sut.SaveFiles.Count);
        }

        [Fact]
        public async Task CreateBackupCommand_Should_CreateBackup_AndNotifyUser()
        {
            // Arrange
            var sut = new DashboardViewModel(Locator.Current.GetService<IScreen>());
            await sut.RefreshSaveFilesCommand.Execute();
            sut.SelectedSaveFile = sut.SaveFiles[0];

            // Act
            await sut.CreateBackupCommand.Execute();

            // Assert
            Assert.Single(_notificationService.Notifications);
            Assert.Equal("Success", _notificationService.Notifications[0].Type);
            Assert.Contains("Backup Created", _notificationService.Notifications[0].Title);
        }

        [Fact]
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
            Assert.Contains(_notificationService.Notifications,
                n => n.Type == "Confirmation" && n.Title == "Confirm Restore");
        }

        [Fact]
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
            Assert.DoesNotContain(_notificationService.Notifications,
                n => n.Type == "Success" && n.Title == "Save Restored");
        }

        public void Dispose()
        {
            // Restore original Splat resolver
            Locator.SetLocator(_originalDependencyResolver);
        }
    }

    internal class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new RoutingState();
    }

    internal class MockLoggingService : ILoggingService
    {
        public void Debug(string message) { }
        public void Debug(string message, Exception exception) { }
        public void Info(string message) { }
        public void Info(string message, Exception exception) { }
        public void Warning(string message) { }
        public void Warning(string message, Exception exception) { }
        public void Error(string message) { }
        public void Error(string message, Exception exception) { }
    }
}
