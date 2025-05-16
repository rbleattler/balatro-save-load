using BalatroSaveToolkit.Views;

namespace BalatroSaveToolkit;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for navigation
		Routing.RegisterRoute("Settings", typeof(SettingsPage));
		Routing.RegisterRoute("Dashboard", typeof(DashboardPage));
		Routing.RegisterRoute("SaveViewer", typeof(SaveViewerPage));
	}
}
