using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;

namespace BalatroSaveToolkit.ViewModels
{
  /// <summary>
  /// Base class for all ViewModels in the application.
  /// Inherits from the core ViewModelBase.
  /// </summary>
  internal class ViewModelBase : Core.ViewModels.ViewModelBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
    /// </summary>
    /// <param name="hostScreen">The screen that will host this ViewModel</param>
    protected ViewModelBase(IScreen hostScreen) : base(hostScreen)
    {
    }
  }
}
