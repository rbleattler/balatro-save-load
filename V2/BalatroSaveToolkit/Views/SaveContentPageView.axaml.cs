using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BalatroSaveToolkit.Views
{
    public partial class SaveContentPageView : UserControl
    {
        public SaveContentPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
