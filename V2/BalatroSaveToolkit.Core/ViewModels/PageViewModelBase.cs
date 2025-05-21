using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using ReactiveUI;

namespace BalatroSaveToolkit.Core.ViewModels
{
    /// <summary>
    /// Base class for all page ViewModels in the application.
    /// </summary>
    public abstract class PageViewModelBase : ViewModelBase
    {
        private string _title = string.Empty;
        private bool _isBusy;
        private string _statusMessage = string.Empty;
        private bool _hasError;

        /// <summary>
        /// Gets or sets the title of the page.
        /// </summary>
        public string Title
        {
            get => _title;
            protected set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page is busy.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            protected set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            protected set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page has an error.
        /// </summary>
        public bool HasError
        {
            get => _hasError;
            protected set => this.RaiseAndSetIfChanged(ref _hasError, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageViewModelBase"/> class.
        /// </summary>
        /// <param name="hostScreen">The screen that will host this ViewModel</param>
        protected PageViewModelBase(IScreen hostScreen) : base(hostScreen)
        {
        }

        /// <summary>
        /// Called when the page is navigated to.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual Task OnNavigatedToAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when the page is navigated from.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual Task OnNavigatedFromAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the status message.
        /// </summary>
        /// <param name="message">The message to set.</param>
        /// <param name="isError">Whether the message is an error.</param>
        protected void SetStatus(string message, bool isError = false)
        {
            StatusMessage = message;
            HasError = isError;
        }

        /// <summary>
        /// Sets the status message to an error.
        /// </summary>
        /// <param name="errorMessage">The error message to set.</param>
        protected void SetErrorStatus(string errorMessage)
        {
            SetStatus(errorMessage, true);
        }

        /// <summary>
        /// Clears the status message.
        /// </summary>
        protected void ClearStatus()
        {
            SetStatus(string.Empty);
        }

        /// <summary>
        /// Executes an operation with busy state handling.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected async Task ExecuteWithBusyStateAsync(Func<Task> operation)
        {
            ArgumentNullException.ThrowIfNull(operation);

            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            ClearStatus();

            try
            {
                await operation().ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                SetErrorStatus($"Operation error: {ex.Message}");
            }
            catch (IOException ex)
            {
                SetErrorStatus($"File error: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                SetErrorStatus($"Access error: {ex.Message}");
            }
            catch (SecurityException ex)
            {
                SetErrorStatus($"Security error: {ex.Message}");
            }
            catch (Exception ex)
            {
                SetErrorStatus($"Error: {ex.Message}");
                throw; // Rethrow other exceptions
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Executes an operation with busy state handling.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>The result of the operation.</returns>
        protected async Task<T> ExecuteWithBusyStateAsync<T>(Func<Task<T>> operation)
        {
            ArgumentNullException.ThrowIfNull(operation);

            if (IsBusy)
            {
                return default!;
            }

            IsBusy = true;
            ClearStatus();

            try
            {
                return await operation().ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                SetErrorStatus($"Operation error: {ex.Message}");
                return default!;
            }
            catch (IOException ex)
            {
                SetErrorStatus($"File error: {ex.Message}");
                return default!;
            }
            catch (UnauthorizedAccessException ex)
            {
                SetErrorStatus($"Access error: {ex.Message}");
                return default!;
            }
            catch (SecurityException ex)
            {
                SetErrorStatus($"Security error: {ex.Message}");
                return default!;
            }
            catch (Exception ex)
            {
                SetErrorStatus($"Error: {ex.Message}");
                throw; // Rethrow other exceptions
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
