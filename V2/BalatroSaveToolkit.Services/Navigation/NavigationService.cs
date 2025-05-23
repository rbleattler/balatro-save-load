using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;
using Splat;

namespace BalatroSaveToolkit.Services.Navigation
{
    /// <summary>
    /// Implementation of the navigation service using ReactiveUI routing
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IScreen _hostScreen;

        /// <summary>
        /// Creates a new instance of NavigationService
        /// </summary>
        /// <param name="hostScreen">The screen that will host the navigation</param>
        public NavigationService(IScreen hostScreen)
        {
            _hostScreen = hostScreen ?? throw new ArgumentNullException(nameof(hostScreen));
        }

        /// <inheritdoc/>
        public Task NavigateToAsync<TViewModel>(object? parameter = null)
            where TViewModel : class, IRoutableViewModel
        {
            var viewModel = Locator.Current.GetService<TViewModel>()
                ?? throw new InvalidOperationException($"Unable to resolve ViewModel of type {typeof(TViewModel).Name}");

            _hostScreen.Router.Navigate.Execute(viewModel);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task NavigateBackAsync()
        {
            _hostScreen.Router.NavigateBack.Execute();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task ClearHistoryAsync()
        {
            var currentViewModel = _hostScreen.Router.GetCurrentViewModel();
            if (currentViewModel != null)
            {
                _hostScreen.Router.NavigateAndReset.Execute(currentViewModel);
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task NavigateToInitialViewAsync()
        {
            // Get the initial view model (first one registered in the IoC container)
            var initialViewModel = Locator.Current.GetServices<IRoutableViewModel>().FirstOrDefault()
                ?? throw new InvalidOperationException("Unable to resolve any routable ViewModels");

            // Navigate and reset the stack to just this view
            _hostScreen.Router.NavigateAndReset.Execute(initialViewModel);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public IObservable<IRoutableViewModel> CurrentViewModel => _hostScreen.Router.CurrentViewModel;

        /// <inheritdoc/>
        public IObservable<bool> CanNavigateBack => _hostScreen.Router.NavigateBack.CanExecute;
    }
}