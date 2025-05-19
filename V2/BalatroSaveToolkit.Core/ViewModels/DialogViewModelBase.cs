using System;
using System.Reactive;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.Commands;
using ReactiveUI;

namespace BalatroSaveToolkit.Core.ViewModels
{
    /// <summary>
    /// Base class for all dialog ViewModels in the application.
    /// </summary>
    public abstract class DialogViewModelBase : ViewModelBase
    {
        private string _title = string.Empty;
        private bool _isOpen;

        /// <summary>
        /// Gets or sets the title of the dialog.
        /// </summary>
        public string Title
        {
            get => _title;
            protected set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog is open.
        /// </summary>
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }

        /// <summary>
        /// Gets the command to close the dialog.
        /// </summary>
        public ReactiveCommandWrapper<Unit, Unit> CloseCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogViewModelBase"/> class.
        /// </summary>
        protected DialogViewModelBase()
        {
            CloseCommand = CreateCommand(() => Close());
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual Task ShowAsync()
        {
            IsOpen = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        public virtual void Close()
        {
            IsOpen = false;
        }
    }
}
