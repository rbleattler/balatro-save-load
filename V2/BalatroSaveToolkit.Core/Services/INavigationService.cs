using System;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.ViewModels;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Interface for navigation service.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to a view for the specified view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to navigate to.</typeparam>
        /// <param name="parameter">Optional parameter to pass to the view model.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task NavigateToAsync<TViewModel>(object? parameter = null) where TViewModel : PageViewModelBase;

        /// <summary>
        /// Navigates back to the previous view.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task NavigateBackAsync();
        
        /// <summary>
        /// Registers a view model factory for a view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to register.</typeparam>
        /// <param name="factory">The factory function that creates the view model.</param>
        void RegisterViewModel<TViewModel>(Func<object?, TViewModel> factory) where TViewModel : PageViewModelBase;

        /// <summary>
        /// Navigates to the initial view.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task NavigateToInitialViewAsync();
    }
}
