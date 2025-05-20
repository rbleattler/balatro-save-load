# Balatro Save and Load Tool - Navigation System

This document provides detailed information about the navigation system in the Balatro Save and Load Tool, which should help prevent duplicate implementations in the future.

## Overview

The application uses a **single, centralized navigation system** based on ReactiveUI's routing capabilities. This navigation system is defined by the `INavigationService` interface in the Core project and implemented by the `NavigationService` class in the Services project.

## Architecture

### Key Components

1. **INavigationService Interface**:
   - Located in: `BalatroSaveToolkit.Core.Services`
   - Defines the contract for navigation operations

2. **NavigationService Implementation**:
   - Located in: `BalatroSaveToolkit.Services.Navigation`
   - Implements the navigation interface
   - Uses ReactiveUI's RoutingState to manage navigation

3. **IScreen Implementation**:
   - Located in: `BalatroSaveToolkit.UI.ViewModels`
   - The main ViewModel implements IScreen to host the RoutingState

4. **RoutableViewModels**:
   - All ViewModels that can be navigated to implement IRoutableViewModel
   - Derive from ViewModelBase which provides navigation support

## Navigation Service Interface

```csharp
public interface INavigationService
{
    // Navigate to a ViewModel
    Task NavigateToAsync<TViewModel>(object parameter = null)
        where TViewModel : class, IRoutableViewModel;

    // Navigate back
    Task NavigateBackAsync();

    // Clear navigation history
    Task ClearHistoryAsync();

    // Observable for current ViewModel
    IObservable<IRoutableViewModel> CurrentViewModel { get; }

    // Can navigate back
    IObservable<bool> CanNavigateBack { get; }
}
```

## Standard Implementation

The standard NavigationService implements these methods using ReactiveUI's RoutingState:

```csharp
public class NavigationService : INavigationService
{
    private readonly IScreen _hostScreen;

    public NavigationService(IScreen hostScreen)
    {
        _hostScreen = hostScreen;
    }

    public async Task NavigateToAsync<TViewModel>(object parameter = null)
        where TViewModel : class, IRoutableViewModel
    {
        var viewModel = Locator.Current.GetService<TViewModel>();
        await _hostScreen.Router.Navigate.Execute(viewModel);
    }

    public async Task NavigateBackAsync()
    {
        await _hostScreen.Router.NavigateBack.Execute();
    }

    public async Task ClearHistoryAsync()
    {
        await _hostScreen.Router.NavigateAndReset.Execute(
            _hostScreen.Router.GetCurrentViewModel());
    }

    public IObservable<IRoutableViewModel> CurrentViewModel =>
        _hostScreen.Router.CurrentViewModel;

    public IObservable<bool> CanNavigateBack =>
        _hostScreen.Router.NavigateBack.CanExecute;
}
```

## ViewLocator

The ViewLocator maps ViewModels to Views:

```csharp
public class ViewLocator : IViewLocator
{
    public IViewFor ResolveView<T>(T viewModel, string contract = null) where T : class
    {
        var viewModelName = viewModel.GetType().Name;
        var viewName = viewModelName.Replace("ViewModel", "View");
        var viewType = Type.GetType($"BalatroSaveToolkit.UI.Views.{viewName}");

        if (viewType != null)
        {
            return (IViewFor)Activator.CreateInstance(viewType);
        }

        return null;
    }
}
```

## Using the Navigation Service

ViewModels should use constructor injection to get the navigation service:

```csharp
public class SomeViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public SomeViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        NavigateCommand = ReactiveCommand.CreateFromTask<Type>(Navigate);
    }

    public ReactiveCommand<Type, Unit> NavigateCommand { get; }

    private async Task Navigate(Type viewModelType)
    {
        if (viewModelType == typeof(OtherViewModel))
        {
            await _navigationService.NavigateToAsync<OtherViewModel>();
        }
    }
}
```

## Dependency Registration

The navigation system should be registered as follows in the dependency injection container:

```csharp
// Register IScreen implementation
services.AddSingleton<IScreen, MainViewModel>();

// Register NavigationService as a singleton
services.AddSingleton<INavigationService, NavigationService>();

// Register ViewLocator
services.AddSingleton<IViewLocator, ViewLocator>();

// Register all routable ViewModels
services.AddTransient<HomeViewModel>();
services.AddTransient<SettingsViewModel>();
// etc.
```

## Why a Single Navigation Service is Important

Having a single implementation of the navigation service offers several benefits:

1. **Consistency**: Navigation behaves the same way throughout the application
2. **Maintainability**: Changes to navigation logic only need to be made in one place
3. **Testability**: Easier to mock a single navigation service for testing
4. **History Management**: Centralized control of the navigation stack
5. **Parameter Passing**: Consistent method for passing parameters between views

## Common Mistakes to Avoid

1. **Creating Multiple Navigation Services**: Only one NavigationService implementation should exist
2. **Direct Router Access**: Always use the NavigationService instead of accessing the Router directly
3. **Manual View Creation**: Let the ViewLocator handle View creation instead of manually creating Views
4. **Bypassing the Navigation System**: All navigation should go through the NavigationService
