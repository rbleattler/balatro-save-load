using BalatroSaveToolkit.ViewModels;

namespace BalatroSaveToolkit.Views;

public partial class SaveViewerPage : ContentPage
{
    private readonly SaveViewerViewModel _viewModel;
    
    public SaveViewerPage(SaveViewerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSavesAsync();
    }
}
