using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using BalatroSaveToolkit.Core.ViewModels;

namespace BalatroSaveToolkit;

/// <summary>
/// A data template that locates views for view models.
/// </summary>
internal class ViewLocator : IDataTemplate
{
    /// <summary>
    /// Builds a control for the specified view model.
    /// </summary>
    /// <param name="param">The view model.</param>
    /// <returns>The built control.</returns>
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var viewModelType = param.GetType();

        // Use conventional naming to find the view
        var viewTypeName = viewModelType.FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);

        // Look in the same assembly as the view model first
        var viewType = viewModelType.Assembly.GetType(viewTypeName);

        // If not found, try the executing assembly
        if (viewType == null)
        {
            viewType = Assembly.GetExecutingAssembly().GetType(viewTypeName);
        }

        if (viewType != null)
        {
            return (Control)Activator.CreateInstance(viewType)!;
        }

        return new TextBlock { Text = $"View not found for: {viewModelType.Name}" };
    }

    /// <summary>
    /// Determines if the data template matches the specified data.
    /// </summary>
    /// <param name="data">The data to check.</param>
    /// <returns>Whether the data template matches.</returns>
    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
