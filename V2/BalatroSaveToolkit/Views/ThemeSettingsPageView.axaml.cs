using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BalatroSaveToolkit.Views
{
    public partial class ThemeSettingsPageView : UserControl
    {
        public ThemeSettingsPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
