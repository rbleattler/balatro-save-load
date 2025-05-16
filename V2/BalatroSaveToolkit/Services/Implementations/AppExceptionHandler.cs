using System.Diagnostics;
using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Implementations
{
    /// <summary>
    /// Global exception handler for the application
    /// </summary>
    public class AppExceptionHandler
    {
        private readonly IErrorHandlingService _errorHandlingService;
        
        public AppExceptionHandler(IErrorHandlingService errorHandlingService)
        {
            _errorHandlingService = errorHandlingService;
        }
        
        /// <summary>
        /// Set up global exception handlers for the application
        /// </summary>
        public void Initialize()
        {
            // Handle exceptions in the UI thread
            Application.Current.UnhandledException += OnUnhandledException;
            
            // Handle exceptions in any thread
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            
            // Handle exceptions in tasks
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            
            Debug.WriteLine("Global exception handlers initialized");
        }
        
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            _errorHandlingService.HandleException(
                exception ?? new Exception("Unknown exception"),
                "Application",
                "An unexpected error occurred",
                ErrorSeverity.Critical);
        }
        
        private void OnAppDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            _errorHandlingService.HandleException(
                exception ?? new Exception("Unknown exception"),
                "AppDomain",
                "A critical application error occurred",
                ErrorSeverity.Critical);
            
            // Log that the app might be terminating
            if (e.IsTerminating)
            {
                _errorHandlingService.LogError("AppDomain", "Application is terminating due to an unhandled exception", ErrorSeverity.Critical);
            }
        }
        
        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _errorHandlingService.HandleException(
                e.Exception,
                "Task",
                "An error occurred in a background task",
                ErrorSeverity.Error);
            
            // Mark as observed so the process doesn't terminate
            e.SetObserved();
        }
    }
}
