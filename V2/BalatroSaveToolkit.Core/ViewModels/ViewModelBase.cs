using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.Commands;
using ReactiveUI;

namespace BalatroSaveToolkit.Core.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels in the application.
    /// </summary>
    public abstract class ViewModelBase : ReactiveObject, INotifyPropertyChanged, INotifyDataErrorInfo, IValidatableObject, IDisposable, IActivatableViewModel
    {
        private bool _isDisposed;
        private readonly Dictionary<string, List<string>> _errors = new();
        private readonly ViewModelActivator _activator = new();

        /// <summary>
        /// Gets the activator used to activate and deactivate the view model.
        /// </summary>
        public ViewModelActivator Activator => _activator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        protected ViewModelBase()
        {
            // Set up the disposable for cleanup
            CompositeDisposable = new CompositeDisposable();
        }

        /// <summary>
        /// Gets the composite disposable for resource cleanup.
        /// </summary>
        protected CompositeDisposable CompositeDisposable { get; }

        /// <summary>
        /// Event fired when a property value changes.
        /// </summary>
        public new event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Event fired when the errors for a property change.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Gets a value indicating whether the entity has validation errors.
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        /// <summary>
        /// Called when a property is changed.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // Validate the property if it's not null
            if (!string.IsNullOrEmpty(propertyName))
            {
                ValidateProperty(propertyName);
            }
        }

        /// <summary>
        /// Sets a property value and raises the PropertyChanged event if the value has changed.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to the backing field.</param>
        /// <param name="value">New value for the property.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Creates a command using the ReactiveCommandFactory.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">Optional observable to determine if the command can execute.</param>
        /// <returns>A command.</returns>
        protected ReactiveCommandWrapper<Unit, Unit> CreateCommand(
            Action execute,
            IObservable<bool>? canExecute = null)
        {
            return ReactiveCommandFactory.Create(execute, canExecute);
        }

        /// <summary>
        /// Creates a command using the ReactiveCommandFactory.
        /// </summary>
        /// <typeparam name="T">The type of parameter.</typeparam>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">Optional observable to determine if the command can execute.</param>
        /// <returns>A command.</returns>
        protected ReactiveCommandWrapper<T, Unit> CreateCommand<T>(
            Action<T> execute,
            IObservable<bool>? canExecute = null)
        {
            return ReactiveCommandFactory.Create<T>(execute, canExecute);
        }

        /// <summary>
        /// Creates an async command using the ReactiveCommandFactory.
        /// </summary>
        /// <param name="execute">The function to execute.</param>
        /// <param name="canExecute">Optional observable to determine if the command can execute.</param>
        /// <returns>A command.</returns>
        protected ReactiveCommandWrapper<Unit, Unit> CreateAsyncCommand(
            Func<Task> execute,
            IObservable<bool>? canExecute = null)
        {
            return ReactiveCommandFactory.CreateAsync(execute, canExecute);
        }

        /// <summary>
        /// Creates an async command using the ReactiveCommandFactory.
        /// </summary>
        /// <typeparam name="T">The type of parameter.</typeparam>
        /// <param name="execute">The function to execute.</param>
        /// <param name="canExecute">Optional observable to determine if the command can execute.</param>
        /// <returns>A command.</returns>
        protected ReactiveCommandWrapper<T, Unit> CreateAsyncCommand<T>(
            Func<T, Task> execute,
            IObservable<bool>? canExecute = null)
        {
            return ReactiveCommandFactory.CreateAsync<T>(execute, canExecute);
        }

        /// <summary>
        /// Creates an async command using the ReactiveCommandFactory.
        /// </summary>
        /// <typeparam name="TParam">The type of parameter.</typeparam>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="execute">The function to execute.</param>
        /// <param name="canExecute">Optional observable to determine if the command can execute.</param>
        /// <returns>A command.</returns>
        protected ReactiveCommandWrapper<TParam, TResult> CreateAsyncCommand<TParam, TResult>(
            Func<TParam, Task<TResult>> execute,
            IObservable<bool>? canExecute = null)
        {
            return ReactiveCommandFactory.CreateAsync<TParam, TResult>(execute, canExecute);
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">Name of the property to get validation errors for, or null to get all errors.</param>
        /// <returns>The validation errors.</returns>
        public System.Collections.IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return _errors.Values.SelectMany(errors => errors).ToList();
            }

            return _errors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Validates a property and updates the errors collection.
        /// </summary>
        /// <param name="propertyName">Name of the property to validate.</param>
        protected virtual void ValidateProperty(string propertyName)
        {
            var validationContext = new ValidationContext(this)
            {
                MemberName = propertyName
            };

            var validationResults = new List<ValidationResult>();
            Validator.TryValidateProperty(GetType().GetProperty(propertyName)?.GetValue(this), validationContext, validationResults);

            // Clear previous errors
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
            }

            // Add new errors
            if (validationResults.Count > 0)
            {
                _errors[propertyName] = validationResults.Select(x => x.ErrorMessage).ToList()!;
            }

            // Notify that errors have changed
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Validates all the properties of the entity.
        /// </summary>
        /// <returns>True if the entity is valid, false otherwise.</returns>
        protected virtual bool ValidateAll()
        {
            var validationContext = new ValidationContext(this);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(this, validationContext, validationResults, true);

            // Clear all errors
            _errors.Clear();

            // Add new errors
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    if (!_errors.ContainsKey(memberName))
                    {
                        _errors[memberName] = new List<string>();
                    }

                    _errors[memberName].Add(validationResult.ErrorMessage!);
                }
            }

            // Notify that errors have changed for all properties
            foreach (var propertyName in _errors.Keys)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            return isValid;
        }

        /// <summary>
        /// Determines whether the object is valid.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>A collection that holds failed-validation information.</returns>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Enumerable.Empty<ValidationResult>();
        }

        /// <summary>
        /// Disposes the ViewModel.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the ViewModel.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                // Release managed resources
                CompositeDisposable.Dispose();
            }

            // Release unmanaged resources

            _isDisposed = true;
        }
    }
}
