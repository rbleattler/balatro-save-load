namespace BalatroSaveToolkit.Views.Controls;

public partial class LoadingPage : ContentPage
{
    public LoadingPage(string message = "Loading...")
    {
        InitializeComponent();
        MessageLabel.Text = message;
    }
    
    public void UpdateMessage(string message)
    {
        MessageLabel.Text = message;
    }
}
