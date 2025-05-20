using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace BalatroSaveToolkit.Core.ViewModels
{
    /// <summary>
    /// Main ViewModel that serves as the host screen for navigation
    /// </summary>
    public class MainViewModel : ReactiveObject, IScreen, IActivatableViewModel
    {
        /// <summary>
        /// Creates a new instance of MainViewModel
        /// </summary>
        public MainViewModel()
        {
            Router = new RoutingState();
            Activator = new ViewModelActivator();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                // Setup any activations here
                Disposable.Create(() => { /* Cleanup */ }).DisposeWith(disposables);
            });
        }

        /// <summary>
        /// Gets the router used for navigation
        /// </summary>
        public RoutingState Router { get; }

        /// <summary>
        /// Gets the activator for this ViewModel
        /// </summary>
        public ViewModelActivator Activator { get; }
    }
}