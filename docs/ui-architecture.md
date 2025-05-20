# Balatro Save and Load Tool - UI Architecture

This document details the UI architecture of the Balatro Save and Load Tool, covering the MVVM pattern implementation, ReactiveUI usage, and best practices for maintaining consistency across the application.

## MVVM Implementation

The application uses the Model-View-ViewModel (MVVM) pattern:

1. **Models**: Domain entities in the Core project
2. **Views**: Avalonia UI components in the UI project
3. **ViewModels**: Classes that expose data and commands to the views

### Folder Structure

The UI project follows this structure:

```
BalatroSaveToolkit.UI/
├── Assets/            # Static assets (images, fonts, etc.)
├── Converters/        # Value converters for binding
├── Resources/         # Resource dictionaries and styles
├── ViewModels/        # Contains all ViewModel classes
│   ├── Base/          # Base ViewModel classes
│   └── ...            # Feature-specific ViewModels
├── Views/             # Contains all View classes
│   ├── Controls/      # Reusable UI controls
│   └── ...            # Feature-specific Views
└── App.axaml          # Application entry point
```

## ReactiveUI Usage

The application leverages ReactiveUI for reactive programming:

1. **Reactive Properties**: Using `this.RaiseAndSetIfChanged()`
2. **Commands**: Using `ReactiveCommand<TParam, TResult>`
3. **Activation**: Using `WhenActivated` for view lifecycle
4. **Bindings**: Using ReactiveUI binding syntax

### ReactiveCommand Wrapper

Custom `ReactiveCommandWrapper` has been implemented to provide a consistent approach to command creation and error handling.

## Navigation System

The application uses a centralized navigation system:

1. **IViewLocator**: Maps ViewModels to Views
2. **IScreen**: Screen host that contains the routing state
3. **RoutingState**: Manages the navigation stack
4. **RoutableViewModel**: Base class for ViewModels that can be navigated to

### Key Components

- **NavigationService**: Single implementation that provides methods for navigation
- **MainViewModel**: Implements IScreen and hosts the RoutingState
- **ViewModelBase**: Implements IRoutableViewModel for navigation support

## Theming System

Theming is implemented through:

1. **Resource Dictionaries**: Contains theme resources
2. **ThemeService**: Manages theme switching
3. **Theme-aware controls**: Controls adapt to theme changes

## Best Practices

1. **View-First Navigation**: Views are created first, then linked to ViewModels
2. **ViewModel-First Routing**: Navigation is driven by ViewModels
3. **Dependency Injection**: ViewModels receive services via DI
4. **Disposable Management**: Using `WhenActivated` for proper cleanup
5. **Command Creation**: Use `ReactiveCommandWrapper` for consistent command handling

## Avoiding Duplicate Implementations

To avoid duplication in the UI layer:

1. **Use the ViewLocator**: Register views with the ViewLocator instead of creating custom mapping logic
2. **Leverage the NavigationService**: Always use the single NavigationService for navigation
3. **Extend Base Classes**: Derive from ViewModelBase and other base classes
4. **Share Resources**: Put common styles and resources in shared dictionaries
5. **Create Reusable Controls**: Extract common UI patterns into reusable controls

## View-ViewModel Communication

Communication between components should follow these patterns:

1. **Property Binding**: For simple data display
2. **Command Binding**: For user actions
3. **Reactive Extensions**: For complex interactions
4. **Message Bus**: For loosely coupled communication

## Testing Approach

The UI layer should be tested through:

1. **ViewModel Tests**: Testing ViewModel logic and commands
2. **Integration Tests**: Testing View-ViewModel binding
3. **Visual Verification**: Manual verification of UI appearance
