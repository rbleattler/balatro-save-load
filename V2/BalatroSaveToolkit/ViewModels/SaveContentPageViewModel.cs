using ReactiveUI;

namespace BalatroSaveToolkit.ViewModels
{    /// <summary>
    /// A routable ViewModel wrapper for SaveContentViewModel.
    /// </summary>
    internal sealed class SaveContentPageViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the SaveContentViewModel that contains the actual functionality.
        /// </summary>
        public SaveContentViewModel Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveContentPageViewModel"/> class.
        /// </summary>
        /// <param name="hostScreen">The screen that will host this ViewModel.</param>
        /// <param name="filePath">The path to the save file.</param>
        public SaveContentPageViewModel(IScreen hostScreen, string filePath)
            : base(hostScreen)
        {
            Content = new SaveContentViewModel { FilePath = filePath };
        }
    }
}
