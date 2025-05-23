using System;
using System.Threading.Tasks;
using ReactiveUI;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Interface for navigation service that provides navigation between views in the application
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigate to a ViewModel of type TViewModel
        /// </summary>
        /// <typeparam name="TViewModel">Type of ViewModel to navigate to</typeparam>
        /// <param name="parameter">Optional parameter to pass to the ViewModel</param>
        /// <returns>Task representing the navigation operation</returns>
        Task NavigateToAsync<TViewModel>(object? parameter = null)
            where TViewModel : class, IRoutableViewModel;

        /// <summary>
        /// Navigate back to the previous view
        /// </summary>
        /// <returns>Task representing the navigation operation</returns>
        Task NavigateBackAsync();

        /// <summary>
        /// Clear navigation history
        /// </summary>
        /// <returns>Task representing the operation</returns>
        Task ClearHistoryAsync();

        /// <summary>
        /// Navigate to the initial view, clearing any existing navigation history
        /// </summary>
        /// <returns>Task representing the navigation operation</returns>
        Task NavigateToInitialViewAsync();

        /// <summary>
        /// Observable for the current ViewModel
        /// </summary>
        IObservable<IRoutableViewModel> CurrentViewModel { get; }

        /// <summary>
        /// Observable that indicates whether back navigation is available
        /// </summary>
        IObservable<bool> CanNavigateBack { get; }
    }
}
