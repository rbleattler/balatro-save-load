using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BalatroSaveAndLoad
{
    public partial class DebugWindow : Window
    {
        public ObservableCollection<string> DebugLog { get; } = new ObservableCollection<string>();
        public ICommand ToggleThemeCommand { get; private set; }

        public DebugWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Initialize the theme toggle command
            ToggleThemeCommand = new RelayCommand(() => App.ToggleTheme());
        }

        public void SetLog(ObservableCollection<string> log)
        {
            DebugLog.Clear();
            foreach (var entry in log)
                DebugLog.Add(entry);
        }

        public void AppendLog(string entry)
        {
            DebugLog.Add(entry);
            DebugListBox.ScrollIntoView(entry);
        }
    }
}
