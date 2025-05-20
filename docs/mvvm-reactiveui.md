# MVVM and ReactiveUI Implementation

This document explains the MVVM (Model-View-ViewModel) architecture implementation in the Balatro Save and Load Tool, including how ReactiveUI is used to enhance the development experience and application responsiveness.

## MVVM Pattern Overview

The application follows the MVVM pattern with the following components:

1. **Models** - Data structures and business logic
2. **Views** - UI components in XAML
3. **ViewModels** - Classes that expose data and commands to the views

## ReactiveUI Integration

ReactiveUI is a functional reactive MVVM library that extends the traditional MVVM pattern with reactive programming concepts.

### Key ReactiveUI Features Used

1. **Reactive Properties** - Using `ObservableAsPropertyHelper<T>` for derived properties
2. **Commands** - Using `ReactiveCommand<TParam, TResult>` for all user actions
3. **Activation** - Using `WhenActivated` to manage subscriptions and resources
4. **Routing** - Using ReactiveUI's routing system for screen navigation
5. **Binding** - Using type-safe bindings to connect Views and ViewModels

## Base Classes

### ReactiveViewModelBase

All ViewModels inherit from `ReactiveViewModelBase`, which provides common functionality:

```csharp
public abstract class ReactiveViewModelBase : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new ViewModelActivator();

    protected ReactiveViewModelBase()
    {
        this.WhenActivated((disposables) =>
        {
            HandleActivation();

            Disposable.Create(HandleDeactivation)
                .DisposeWith(disposables);
        });
    }

    protected virtual void HandleActivation() { }
    protected virtual void HandleDeactivation() { }
}
```

### ReactiveView

Views inherit from `ReactiveView<T>` to provide type-safe ViewModel binding:

```csharp
public abstract class ReactiveUserControl<TViewModel> : UserControl, IViewFor<TViewModel>
    where TViewModel : class
{
    public static readonly StyledProperty<TViewModel> ViewModelProperty =
        AvaloniaProperty.Register<ReactiveUserControl<TViewModel>, TViewModel>(nameof(ViewModel));

    public TViewModel ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel)value;
    }

    protected ReactiveUserControl()
    {
        this.WhenActivated(disposables => { });
    }
}
```

## Command Infrastructure

Commands are implemented using a ReactiveCommand wrapper for consistent error handling and loading state:

```csharp
public class ReactiveCommandWrapper<TParam, TResult>
{
    private readonly ReactiveCommand<TParam, TResult> _command;

    public ReactiveCommand<TParam, TResult> Command => _command;

    public IObservable<bool> IsExecuting => _command.IsExecuting;

    public IObservable<bool> CanExecute => _command.CanExecute;

    public IObservable<Exception> ThrownExceptions => _command.ThrownExceptions;

    public ReactiveCommandWrapper(
        Func<TParam, Task<TResult>> executeAsync,
        IObservable<bool> canExecute = null,
        IScheduler outputScheduler = null)
    {
        _command = ReactiveCommand.CreateFromTask(
            executeAsync,
            canExecute,
            outputScheduler ?? RxApp.MainThreadScheduler);

        // Handle exceptions by default
        _command.ThrownExceptions
            .Subscribe(HandleCommandException);
    }

    private void HandleCommandException(Exception ex)
    {
        // Log the exception
        Debug.WriteLine($"Command execution error: {ex.Message}");

        // Additional error handling as needed
    }
}
```

### Command Usage Example

```csharp
public class SaveViewModel : ReactiveViewModelBase
{
    private readonly IFileSystemService _fileSystemService;

    // Command declaration
    public ReactiveCommandWrapper<string, bool> SaveGameCommand { get; }

    // Loading indicator
    private readonly ObservableAsPropertyHelper<bool> _isSaving;
    public bool IsSaving => _isSaving.Value;

    public SaveViewModel(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;

        // Create command
        SaveGameCommand = new ReactiveCommandWrapper<string, bool>(
            ExecuteSaveGameAsync,
            this.WhenAnyValue(vm => vm.CanSaveGame)
        );

        // Create loading indicator from command execution
        _isSaving = SaveGameCommand.IsExecuting
            .ToProperty(this, vm => vm.IsSaving);
    }

    private async Task<bool> ExecuteSaveGameAsync(string saveName)
    {
        // Command implementation
        await _fileSystemService.SaveGameAsync(saveName);
        return true;
    }

    private bool CanSaveGame => /* some condition */;
}
```

## Navigation System

The application uses ReactiveUI's routing system for navigation:

```csharp
public class NavigationService : INavigationService
{
    private readonly RoutingState _router;

    public IObservable<IRoutableViewModel> CurrentViewModel => _router.CurrentViewModel;

    public NavigationService(RoutingState router)
    {
        _router = router;
    }

    public IObservable<Unit> Navigate<TViewModel>(object parameter = null)
        where TViewModel : IRoutableViewModel
    {
        var viewModel = Locator.Current.GetService<TViewModel>();
        return _router.Navigate.Execute(viewModel);
    }

    public IObservable<Unit> NavigateBack() => _router.NavigateBack.Execute();
}
```

### RoutableViewModel Example

```csharp
public class SaveListViewModel : ReactiveViewModelBase, IRoutableViewModel
{
    // IRoutableViewModel implementation
    public string UrlPathSegment => "saves";
    public IScreen HostScreen { get; }

    private readonly INavigationService _navigationService;

    public ReactiveCommandWrapper<SaveInfo, Unit> ViewSaveDetailsCommand { get; }

    public SaveListViewModel(IScreen hostScreen, INavigationService navigationService)
    {
        HostScreen = hostScreen;
        _navigationService = navigationService;

        ViewSaveDetailsCommand = new ReactiveCommandWrapper<SaveInfo, Unit>(
            save => _navigationService.Navigate<SaveDetailViewModel>(save).ToTask()
        );
    }
}
```

## Property Change Notifications

ReactiveUI provides several ways to notify about property changes:

### Basic Property

```csharp
private string _saveName;
public string SaveName
{
    get => _saveName;
    set => this.RaiseAndSetIfChanged(ref _saveName, value);
}
```

### Derived Property

```csharp
private readonly ObservableAsPropertyHelper<bool> _isValid;
public bool IsValid => _isValid.Value;

public SaveViewModel()
{
    // Create derived property based on another property
    _isValid = this.WhenAnyValue(vm => vm.SaveName)
        .Select(name => !string.IsNullOrEmpty(name))
        .ToProperty(this, vm => vm.IsValid);
}
```

### Property Changed Reactions

```csharp
this.WhenAnyValue(vm => vm.SelectedSave)
    .WhereNotNull()
    .Subscribe(save => LoadSaveDetails(save))
    .DisposeWith(disposables);
```

## ViewLocator

To connect Views with ViewModels, the application uses a ViewLocator:

```csharp
public class ViewLocator : IDataTemplate
{
    public IControl Build(object data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = $"View not found: {name}" };
    }

    public bool Match(object data)
    {
        return data is ReactiveViewModelBase;
    }
}
```

## Dependency Injection

ReactiveUI is integrated with the Splat dependency injection container:

```csharp
public static class AppBootstrapper
{
    public static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        // Register services
        services.RegisterConstant(new RoutingState(), typeof(RoutingState));
        services.Register(() => new NavigationService(resolver.GetService<RoutingState>()), typeof(INavigationService));

        // Register platform-specific services

        // Register ViewModels
        services.Register(() => new MainViewModel(
            resolver.GetService<INavigationService>(),
            resolver.GetService<ISettingsService>()
        ), typeof(MainViewModel));

        // Additional registrations
    }
}
```

## Testing ViewModels

ReactiveUI's design facilitates ViewModel testing:

```csharp
[Fact]
public void SaveCommand_CanExecute_WhenSaveNameIsValid()
{
    // Arrange
    var fileSystemService = Substitute.For<IFileSystemService>();
    var viewModel = new SaveViewModel(fileSystemService);

    // Push property value
    viewModel.SaveName = "TestSave";

    // Act - Get command can execute
    var canExecute = viewModel.SaveGameCommand.CanExecute.First();

    // Assert
    Assert.True(canExecute);
}
```
