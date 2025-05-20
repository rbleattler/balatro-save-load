using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Core.ViewModels;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Implementation of the view stack service using ReactiveUI.
    /// </summary>
    public class ViewStackService : IViewStackService, IDisposable
    {
        private readonly List<PageViewModelBase> _navigationStack;
        private readonly BehaviorSubject<PageViewModelBase> _currentPage;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewStackService"/> class.
        /// </summary>
        public ViewStackService()
        {
            _navigationStack = new List<PageViewModelBase>();
            _currentPage = new BehaviorSubject<PageViewModelBase>(null!);
        }

        /// <inheritdoc/>
        public IObservable<PageViewModelBase> CurrentPage => _currentPage.AsObservable();

        /// <inheritdoc/>
        public IReadOnlyList<PageViewModelBase> NavigationStack => _navigationStack.AsReadOnly();

        /// <inheritdoc/>
        public async Task PushPage(PageViewModelBase page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            // If we have a current page, navigate from it
            if (_navigationStack.Count > 0)
            {
                var currentPage = _navigationStack.Last();
                await currentPage.OnNavigatedFromAsync();
            }

            _navigationStack.Add(page);
            await page.OnNavigatedToAsync();
            _currentPage.OnNext(page);
        }

        /// <inheritdoc/>
        public async Task PopPage()
        {
            if (_navigationStack.Count <= 1)
                throw new InvalidOperationException("Cannot pop the last page from the stack");

            var currentPage = _navigationStack.Last();
            await currentPage.OnNavigatedFromAsync();

            _navigationStack.Remove(currentPage);

            var newCurrentPage = _navigationStack.Last();
            await newCurrentPage.OnNavigatedToAsync();
            _currentPage.OnNext(newCurrentPage);
        }

        /// <inheritdoc/>
        public async Task SetStack(PageViewModelBase page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            // Navigate from current page if it exists
            if (_navigationStack.Count > 0)
            {
                var currentPage = _navigationStack.Last();
                await currentPage.OnNavigatedFromAsync();
            }

            // Clear the stack and add the new page
            _navigationStack.Clear();
            _navigationStack.Add(page);

            await page.OnNavigatedToAsync();
            _currentPage.OnNext(page);
        }

        /// <summary>
        /// Disposes the stack service.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the stack service.
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
                _currentPage.Dispose();
            }

            _isDisposed = true;
        }
    }
}
