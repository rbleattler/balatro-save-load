using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BalatroSaveToolkit.Views
{
    public partial class SaveContentView : UserControl
    {
        public SaveContentView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
