using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using BalatroSaveToolkit.ViewModels;

namespace BalatroSaveToolkit.Views
{
    public partial class SaveContentView : UserControl
    {
        private SaveContentViewModel ViewModel => DataContext as SaveContentViewModel;

        // Use accessor methods instead of properties to avoid name conflicts with auto-generated fields
        private TextBox GetContentTextBox() => this.FindControl<TextBox>("ContentTextBox");
        private TextBlock GetStatusTextBlock() => this.FindControl<TextBlock>("StatusTextBlock");

        public SaveContentView()
        {
            InitializeComponent();
            AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Handle Ctrl+C to copy selection or all text
            if (e.Key == Key.C && e.KeyModifiers == KeyModifiers.Control)
            {
                // If there's a selection, let the TextBox handle it
                // If there's no selection, copy all text
                var contentTextBox = GetContentTextBox();
                if (string.IsNullOrEmpty(contentTextBox?.SelectedText))
                {
                    CopyContentToClipboard(contentTextBox?.Text);
                    UpdateStatus("All content copied to clipboard");
                    e.Handled = true; // Prevent default handling
                }
            }
            // Handle Ctrl+F for find
            else if (e.Key == Key.F && e.KeyModifiers == KeyModifiers.Control)
            {
                Find_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        private void CopySelected_Click(object sender, RoutedEventArgs e)
        {
            var contentTextBox = GetContentTextBox();
            if (!string.IsNullOrEmpty(contentTextBox?.SelectedText))
            {
                CopyContentToClipboard(contentTextBox.SelectedText);
                UpdateStatus("Selected text copied to clipboard");
            }
        }

        private void CopyAll_Click(object sender, RoutedEventArgs e)
        {
            var contentTextBox = GetContentTextBox();
            if (ViewModel?.HasContent == true && !string.IsNullOrEmpty(contentTextBox?.Text))
            {
                CopyContentToClipboard(contentTextBox.Text);
                UpdateStatus("All content copied to clipboard");
            }
        }

        private async void CopyContentToClipboard(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            try
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel != null)
                {
                    await topLevel.Clipboard.SetTextAsync(text);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error copying to clipboard: {ex.Message}");
            }
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var contentTextBox = GetContentTextBox();
                if (int.TryParse(selectedItem.Content?.ToString(), out var fontSize) && contentTextBox != null)
                {
                    contentTextBox.FontSize = fontSize;
                }
            }
        }

        private async void Find_Click(object sender, RoutedEventArgs e)
        {
            var contentTextBox = GetContentTextBox();
            if (ViewModel?.HasContent != true || string.IsNullOrEmpty(contentTextBox?.Text))
                return;            // Create a find dialog
            var findDialog = new Window
            {
                Title = "Find Text",
                Width = 300,
                Height = 120,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.Height,
                CanResize = false
            };            // Set dialog properties
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
            {
                findDialog.ShowInTaskbar = false;
                findDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                // In Avalonia we don't need to set the owner explicitly for ShowDialog
            }

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var searchPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Margin = new Thickness(10)
            };

            var searchLabel = new TextBlock
            {
                Text = "Search for:",
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var searchTextBox = new TextBox
            {
                Width = 200,
                Margin = new Thickness(0, 0, 0, 0)
            };

            searchPanel.Children.Add(searchLabel);
            searchPanel.Children.Add(searchTextBox);

            var buttonPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                Margin = new Thickness(10)
            };

            var findButton = new Button
            {
                Content = "Find",
                Width = 80,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80
            };

            buttonPanel.Children.Add(findButton);
            buttonPanel.Children.Add(cancelButton);

            Grid.SetRow(searchPanel, 0);
            Grid.SetRow(buttonPanel, 1);

            grid.Children.Add(searchPanel);
            grid.Children.Add(buttonPanel);

            findDialog.Content = grid;            findButton.Click += (s, args) =>
            {
                var searchText = searchTextBox.Text;
                var contentTextBox = GetContentTextBox();
                if (!string.IsNullOrEmpty(searchText) && contentTextBox != null)
                {
                    var startIndex = contentTextBox.CaretIndex;
                    if (startIndex >= contentTextBox.Text.Length)
                        startIndex = 0;

                    var index = contentTextBox.Text.IndexOf(searchText, startIndex, StringComparison.OrdinalIgnoreCase);
                    if (index == -1 && startIndex > 0)
                    {
                        index = contentTextBox.Text.IndexOf(searchText, 0, StringComparison.OrdinalIgnoreCase);
                    }

                    if (index != -1)
                    {
                        contentTextBox.Focus();
                        contentTextBox.SelectionStart = index;
                        contentTextBox.SelectionEnd = index + searchText.Length;
                        contentTextBox.CaretIndex = index + searchText.Length;

                        // Ensure the selection is visible
                        contentTextBox.BringIntoView(new Rect(contentTextBox.CaretIndex, 0, 1, 1));

                        UpdateStatus($"Found '{searchText}'");
                        findDialog.Close();
                    }
                    else
                    {
                        UpdateStatus($"Text '{searchText}' not found");
                    }
                }
            };

            cancelButton.Click += (s, args) => findDialog.Close();

            searchTextBox.KeyDown += (s, args) =>
            {
                if (args.Key == Key.Enter)
                {
                    findButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    args.Handled = true;
                }
                else if (args.Key == Key.Escape)
                {
                    findDialog.Close();
                    args.Handled = true;
                }
            };

            // Set initial focus to the search box
            findDialog.Opened += (s, args) => searchTextBox.Focus();            var topLevel = GetTopLevel();
            if (topLevel is Window parentWindow)
            {
                await findDialog.ShowDialog(parentWindow);
            }
            else
            {
                // If we can't get a proper Window reference, try to get the main window
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                    desktop.MainWindow != null)
                {
                    await findDialog.ShowDialog(desktop.MainWindow);
                }
                else
                {
                    // Log error - no parent window found
                    throw new InvalidOperationException("Cannot show dialog: No parent window found");
                }
            }
        }        private async void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.HasContent != true)
                return;

            TopLevel topLevel = GetTopLevel();
            if (topLevel == null) return;

            var filePickerSaveOptions = new FilePickerSaveOptions
            {
                Title = "Save Balatro Save Data",
                DefaultExtension = "lua",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Lua Files (*.lua)") { Patterns = new[] { "*.lua" } },
                    new FilePickerFileType("Text Files (*.txt)") { Patterns = new[] { "*.txt" } },
                    new FilePickerFileType("All Files (*.*)") { Patterns = new[] { "*.*" } }
                }
            };

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(filePickerSaveOptions);
            if (file != null)
            {
                try
                {
                    using var stream = await file.OpenWriteAsync();
                    using var writer = new System.IO.StreamWriter(stream);

                    var contentTextBox = GetContentTextBox();
                    var content = string.IsNullOrEmpty(contentTextBox?.SelectedText)
                        ? contentTextBox?.Text
                        : contentTextBox?.SelectedText;

                    if (!string.IsNullOrEmpty(content))
                    {
                        await writer.WriteAsync(content);
                        UpdateStatus($"Content saved to {file.Name}");
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Error saving file: {ex.Message}");
                }
            }
        }        private void UpdateStatus(string message)
        {
            var statusTextBlock = GetStatusTextBlock();
            if (statusTextBlock != null)
            {
                statusTextBlock.Text = message;
            }
        }

        private TopLevel GetTopLevel()
        {
            return TopLevel.GetTopLevel(this);
        }
    }
}
