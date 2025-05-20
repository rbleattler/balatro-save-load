using System;
using ReactiveUI;

namespace BalatroSaveToolkit.UI.Infrastructure
{
    /// <summary>
    /// Maps ViewModels to Views for ReactiveUI
    /// </summary>
    public class ViewLocator : IViewLocator
    {
        /// <summary>
        /// Resolves a View for a ViewModel
        /// </summary>
        /// <typeparam name="T">Type of ViewModel</typeparam>
        /// <param name="viewModel">ViewModel instance</param>
        /// <param name="contract">Optional contract</param>
        /// <returns>The View for the ViewModel</returns>
        public IViewFor? ResolveView<T>(T? viewModel, string? contract = null) where T : class
        {
            if (viewModel == null)
                return null;

            var viewModelName = viewModel.GetType().Name;
            var viewName = viewModelName.Replace("ViewModel", "View", StringComparison.Ordinal);

            var viewTypeName = $"BalatroSaveToolkit.UI.Views.{viewName}";
            var viewType = Type.GetType(viewTypeName) ??
                           AppDomain.CurrentDomain.GetAssemblies()
                               .SelectMany(x => x.GetTypes())
                               .FirstOrDefault(t => t.FullName == viewTypeName || t.Name == viewName);

            if (viewType == null)
            {
                return null;
            }

            try
            {
                return (IViewFor)Activator.CreateInstance(viewType)!;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unable to create view for {viewModelName}: {ex.Message}", ex);
            }
        }
    }
}