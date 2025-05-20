using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;
using Splat;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Implementation of the navigation service using ReactiveUI.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IViewStackService _viewStackService;
        private readonly Dictionary<Type, Func<object?, PageViewModelBase>> _viewModelFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        /// <param name="viewStackService">The view stack service.</param>
        public NavigationService(IViewStackService viewStackService)
        {
            _viewStackService = viewStackService ?? throw new ArgumentNullException(nameof(viewStackService));
            _viewModelFactories = new Dictionary<Type, Func<object?, PageViewModelBase>>();
        }

        /// <inheritdoc/>
        public Task NavigateToAsync<TViewModel>(object? parameter = null) where TViewModel : PageViewModelBase
        {
            var viewModelType = typeof(TViewModel);

            if (!_viewModelFactories.TryGetValue(viewModelType, out var factory))
            {
                throw new InvalidOperationException($"No view model factory registered for {viewModelType}");
            }

            var viewModel = factory(parameter);
            return _viewStackService.PushPage(viewModel);
        }

        /// <inheritdoc/>
        public Task NavigateBackAsync()
        {
            return _viewStackService.PopPage();
        }

        /// <inheritdoc/>
        public void RegisterViewModel<TViewModel>(Func<object?, TViewModel> factory) where TViewModel : PageViewModelBase
        {
            _viewModelFactories[typeof(TViewModel)] = parameter => factory(parameter);
        }

        /// <inheritdoc/>
        public Task NavigateToInitialViewAsync()
        {
            // Clear the navigation stack and navigate to the initial view
            return _viewStackService.PushPage(_viewModelFactories.Values.First()(null));
        }
    }
}
