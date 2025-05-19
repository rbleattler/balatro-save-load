using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace BalatroSaveToolkit.Core.Commands
{
  /// <summary>
  /// A wrapper around ReactiveCommand to provide a common interface for commands.
  /// </summary>
  /// <typeparam name="TParam">The type of parameter that the command accepts.</typeparam>
  /// <typeparam name="TResult">The type of result that the command produces.</typeparam>
  public class ReactiveCommandWrapper<TParam, TResult> : ICommand
  {
    private readonly ReactiveCommand<TParam, TResult> _command;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCommandWrapper{TParam, TResult}"/> class.
    /// </summary>
    /// <param name="command">The underlying command.</param>
    public ReactiveCommandWrapper(ReactiveCommand<TParam, TResult> command)
    {
      _command = command ?? throw new ArgumentNullException(nameof(command));
      _command.CanExecute.Subscribe(canExecute => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
    }

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Gets an observable that fires when the command's execution state changes.
    /// </summary>
    public IObservable<bool> CanExecuteObservable => _command.CanExecute;

    /// <summary>
    /// Gets an observable that fires when the command is executing.
    /// </summary>
    public IObservable<bool> IsExecuting => _command.IsExecuting;

    /// <summary>
    /// Gets an observable that fires when the command throws an exception.
    /// </summary>
    public IObservable<Exception> ThrownExceptions => _command.ThrownExceptions;

    /// <summary>
    /// Gets the underlying ReactiveCommand.
    /// </summary>
    public ReactiveCommand<TParam, TResult> Command => _command;

    /// <summary>
    /// Executes the command with the specified parameter.
    /// </summary>
    /// <param name="param">The parameter to pass to the command.</param>
    /// <returns>An observable of the result.</returns>
    public IObservable<TResult> Execute(TParam param) => _command.Execute(param);

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="param">The parameter to pass to the command.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<TResult> ExecuteAsync(TParam param)
    {
      return await _command.Execute(param).FirstAsync();
    }

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="param">Data used by the command.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public bool CanExecuteNow(TParam param) => _command.CanExecute.FirstAsync().Wait();

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data, this object can be set to null.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public bool CanExecute(object? parameter)
    {
      return _command.CanExecute.FirstAsync().Wait();
    }

  /// <summary>
  /// Defines the method to be called when the command is invoked.
  /// </summary>
  /// <param name="parameter">Data used by the command. If the command does not require data, this object can be set to null.</param>
  public void Execute(object? parameter)
  {
    _command.Execute((TParam)parameter!).Subscribe();
  }

    /// <summary>
    /// Implicitly converts a ReactiveCommandWrapper to a ReactiveCommand.
    /// </summary>
    /// <param name="wrapper">The wrapper to convert.</param>
    public static implicit operator ReactiveCommand<TParam, TResult>(ReactiveCommandWrapper<TParam, TResult> wrapper)
    {
      return wrapper.Command;
    }
  }

  /// <summary>
  /// Factory methods for creating reactive commands.
  /// </summary>
  public static class ReactiveCommandFactory
  {
    /// <summary>
    /// Creates a command that takes no parameters and returns no results.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">Observable that determines if the command can execute.</param>
    /// <returns>A command wrapper.</returns>
    public static ReactiveCommandWrapper<Unit, Unit> Create(
        Action execute,
        IObservable<bool>? canExecute = null)
    {
      return new ReactiveCommandWrapper<Unit, Unit>(ReactiveCommand.Create(
          () => { execute(); return Unit.Default; },
          canExecute ?? Observable.Return(true)));
    }

    /// <summary>
    /// Creates a command that takes a parameter and returns no results.
    /// </summary>
    /// <typeparam name="T">The type of parameter.</typeparam>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">Observable that determines if the command can execute.</param>
    /// <returns>A command wrapper.</returns>
    public static ReactiveCommandWrapper<T, Unit> Create<T>(
        Action<T> execute,
        IObservable<bool>? canExecute = null)
    {
      return new ReactiveCommandWrapper<T, Unit>(ReactiveCommand.Create<T, Unit>(
          param =>
          {
            execute(param);
            return Unit.Default;
          },
          canExecute ?? Observable.Return(true)));
    }

    /// <summary>
    /// Creates a command that takes no parameters and returns a result asynchronously.
    /// </summary>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="execute">The function to execute.</param>
    /// <param name="canExecute">Observable that determines if the command can execute.</param>
    /// <returns>A command wrapper.</returns>
    public static ReactiveCommandWrapper<Unit, TResult> CreateFromTask<TResult>(
        Func<Task<TResult>> execute,
        IObservable<bool>? canExecute = null)
    {
      return new ReactiveCommandWrapper<Unit, TResult>(ReactiveCommand.CreateFromTask(
          execute,
          canExecute ?? Observable.Return(true)));
    }

    /// <summary>
    /// Creates a command that takes a parameter and returns a result asynchronously.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="execute">The function to execute.</param>
    /// <param name="canExecute">Observable that determines if the command can execute.</param>
    /// <returns>A command wrapper.</returns>
    public static ReactiveCommandWrapper<TParam, TResult> CreateFromTask<TParam, TResult>(
        Func<TParam, Task<TResult>> execute,
        IObservable<bool>? canExecute = null)
    {
      return new ReactiveCommandWrapper<TParam, TResult>(ReactiveCommand.CreateFromTask(
          execute,
          canExecute ?? Observable.Return(true)));
    }

    /// <summary>
    /// Creates a command that takes no parameters and performs an action asynchronously.
    /// </summary>
    /// <param name="execute">The function to execute.</param>
    /// <param name="canExecute">Observable that determines if the command can execute.</param>
    /// <returns>A command wrapper.</returns>
    public static ReactiveCommandWrapper<Unit, Unit> CreateFromTask(
        Func<Task> execute,
        IObservable<bool>? canExecute = null)
    {
      return new ReactiveCommandWrapper<Unit, Unit>(ReactiveCommand.CreateFromTask(
          execute,
          canExecute ?? Observable.Return(true)));
    }

    /// <summary>
    /// Creates a command that takes a parameter and performs an action asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of parameter.</typeparam>
    /// <param name="execute">The function to execute.</param>
    /// <param name="canExecute">Observable that determines if the command can execute.</param>
    /// <returns>A command wrapper.</returns>
    public static ReactiveCommandWrapper<T, Unit> CreateFromTask<T>(
        Func<T, Task> execute,
        IObservable<bool>? canExecute = null)
    {
      return new ReactiveCommandWrapper<T, Unit>(ReactiveCommand.CreateFromTask<T>(
          execute,
          canExecute ?? Observable.Return(true)));
    }

    /// <summary>
    /// Creates an asynchronous reactive command with no parameter.
    /// Alias for CreateFromTask for backward compatibility.
    /// </summary>
    /// <param name="execute">The function to execute when the command is invoked.</param>
    /// <param name="canExecute">The observable that determines when the command can execute.</param>
    /// <returns>A reactive command.</returns>
    public static ReactiveCommandWrapper<Unit, Unit> CreateAsync(Func<Task> execute, IObservable<bool>? canExecute = null)
    {
      return CreateFromTask(execute, canExecute);
    }

    /// <summary>
    /// Creates an asynchronous reactive command with a parameter.
    /// Alias for CreateFromTask for backward compatibility.
    /// </summary>
    /// <typeparam name="T">The type of parameter passed to the command.</typeparam>
    /// <param name="execute">The function to execute when the command is invoked.</param>
    /// <param name="canExecute">The observable that determines when the command can execute.</param>
    /// <returns>A reactive command.</returns>
    public static ReactiveCommandWrapper<T, Unit> CreateAsync<T>(Func<T, Task> execute, IObservable<bool>? canExecute = null)
    {
      return CreateFromTask<T>(execute, canExecute);
    }

    /// <summary>
    /// Creates an asynchronous reactive command with a parameter and result.
    /// Alias for CreateFromTask for backward compatibility.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter passed to the command.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the command.</typeparam>
    /// <param name="execute">The function to execute when the command is invoked.</param>
    /// <param name="canExecute">The observable that determines when the command can execute.</param>
    /// <returns>A reactive command.</returns>
    public static ReactiveCommandWrapper<TParam, TResult> CreateAsync<TParam, TResult>(Func<TParam, Task<TResult>> execute, IObservable<bool>? canExecute = null)
    {
      return CreateFromTask<TParam, TResult>(execute, canExecute);
    }
  }
}
