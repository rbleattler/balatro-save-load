using System;
using System.Threading.Tasks;

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
            protected set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page is busy.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            protected set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            protected set => SetProperty(ref _statusMessage, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page has an error.
        /// </summary>
        public bool HasError
        {
            get => _hasError;
            protected set => SetProperty(ref _hasError, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageViewModelBase"/> class.
        /// </summary>
        protected PageViewModelBase()
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
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            ClearStatus();

            try
            {
                await operation();
            }
            catch (Exception ex)
            {
                SetErrorStatus($"Error: {ex.Message}");
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
            if (IsBusy)
            {
                return default!;
            }

            IsBusy = true;
            ClearStatus();

            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                SetErrorStatus($"Error: {ex.Message}");
                return default!;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
