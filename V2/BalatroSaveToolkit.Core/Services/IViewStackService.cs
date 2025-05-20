using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Interface for managing a stack of views/pages
    /// </summary>
    public interface IViewStackService
    {
        /// <summary>
        /// Gets an observable of the current page.
        /// </summary>
        IObservable<PageViewModelBase> CurrentPage { get; }

        /// <summary>
        /// Gets the navigation stack.
        /// </summary>
        IReadOnlyList<PageViewModelBase> NavigationStack { get; }

        /// <summary>
        /// Pushes a page to the stack.
        /// </summary>
        /// <param name="page">The page to push.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task PushPage(PageViewModelBase page);

        /// <summary>
        /// Pops a page from the stack.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task PopPage();

        /// <summary>
        /// Sets the stack to contain just the given page.
        /// </summary>
        /// <param name="page">The single page to display.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetStack(PageViewModelBase page);
    }
}
