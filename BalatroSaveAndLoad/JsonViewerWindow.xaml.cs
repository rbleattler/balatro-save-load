using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BalatroSaveAndLoad
{
    public partial class JsonViewerWindow : Window
    {
        private string _currentContent = string.Empty; // Store current content

        public JsonViewerWindow()
        {
            InitializeComponent();
            DataContext = this;
            Title = "Balatro Save File Viewer - Lua Format"; // Updated title to clarify format

            // Add a keyboard shortcut for copy (Ctrl+C)
            JsonTextBox.PreviewKeyDown += JsonTextBox_PreviewKeyDown;

            // Initialize UI elements
            StatusTextBlock.Text = "Lua dictionary format shown. This is not JSON but the native Balatro save format.";
        }

        private void JsonTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Handle Ctrl+C to copy selection or all text
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                // If there's a selection, it will be copied by the default behavior
                // If there's no selection, copy all text
                if (string.IsNullOrEmpty(JsonTextBox.SelectedText))
                {
                    Clipboard.SetText(JsonTextBox.Text);
                    StatusTextBlock.Text = "All content copied to clipboard";
                    e.Handled = true; // Prevent default handling
                }
            }
        }

        private void FontSizeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (JsonTextBox != null && FontSizeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (int.TryParse(selectedItem.Content.ToString(), out var fontSize))
                {
                    JsonTextBox.FontSize = fontSize;
                }
            }
        }

        public void SetJsonContent(string content)
        {
            _currentContent = content;
            JsonTextBox.Text = FormatLuaContent(content);
            UpdateStatusInfo(JsonTextBox.Text);
        }

        public void UpdateJsonContent(string content)
        {
            // Update the content and scroll to the top
            _currentContent = content; // Store original content
            JsonTextBox.Text = FormatLuaContent(content); // Display formatted content
            JsonTextBox.ScrollToHome();
            UpdateStatusInfo(JsonTextBox.Text); // Update status based on formatted content
        }

        private void UpdateStatusInfo(string displayedContent) // Parameter changed to reflect displayed content
        {
            // Update the status bar with info about the content
            if (!string.IsNullOrEmpty(displayedContent))
            {
                // Count lines in the displayed (formatted) content
                var lineCount = displayedContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                StatusTextBlock.Text = $"Lua dictionary format - {lineCount} lines, {displayedContent.Length} characters";
            }
            else
            {
                StatusTextBlock.Text = "No content available";
            }
        }

        /// <summary>
        /// Formats Lua-style dictionary content for better readability
        /// </summary>
        private string FormatLuaContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            var luaSource = content.Trim();
            if (luaSource.StartsWith("return", StringComparison.OrdinalIgnoreCase))
            {
                var returnStatementEnd = "return".Length;
                while (returnStatementEnd < luaSource.Length && char.IsWhiteSpace(luaSource[returnStatementEnd]))
                {
                    returnStatementEnd++;
                }
                luaSource = luaSource[returnStatementEnd..];
            }

            var sb = new StringBuilder();
            var indentLevel = 0;
            var i = 0;
            var inString = false;
            var stringDelim = '\0';
            var lastCharWasNewline = true;

            while (i < luaSource.Length)
            {
                var c = luaSource[i];

                if (inString)
                {
                    sb.Append(c);
                    if (c == stringDelim)
                    {
                        if (i == 0 || luaSource[i - 1] != '\\' || (i > 1 && luaSource[i - 2] == '\\'))
                        {
                            inString = false;
                        }
                    }
                    i++;
                    lastCharWasNewline = false;
                    continue;
                }

                switch (c)
                {
                    case '{':
                    case '[':
                        sb.Append(c);
                        lastCharWasNewline = false;

                        var nextTokenIdx = i + 1;
                        while (nextTokenIdx < luaSource.Length && char.IsWhiteSpace(luaSource[nextTokenIdx]))
                        {
                            nextTokenIdx++;
                        }

                        var isEmptyBlock = false;
                        if (nextTokenIdx < luaSource.Length)
                        {
                            if (c == '{' && luaSource[nextTokenIdx] == '}') isEmptyBlock = true;
                            else if (c == '[' && luaSource[nextTokenIdx] == ']') isEmptyBlock = true;
                        }

                        if (isEmptyBlock)
                        {
                            i++;
                            while (i < nextTokenIdx)
                            {
                                sb.Append(luaSource[i++]);
                            }
                            sb.Append(luaSource[i]);
                        }
                        else
                        {
                            sb.AppendLine();
                            lastCharWasNewline = true;
                            indentLevel++;
                            AppendIndent(sb, indentLevel);
                        }
                        break;

                    case '}':
                    case ']':
                        if (!lastCharWasNewline) sb.AppendLine();
                        lastCharWasNewline = true;
                        indentLevel = Math.Max(0, indentLevel - 1);
                        AppendIndent(sb, indentLevel);
                        sb.Append(c);
                        lastCharWasNewline = false;
                        break;

                    case ',':
                        sb.Append(c);
                        sb.AppendLine();
                        lastCharWasNewline = true;
                        AppendIndent(sb, indentLevel);
                        break;

                    case '"':
                    case '\'':
                        inString = true;
                        stringDelim = c;
                        sb.Append(c);
                        lastCharWasNewline = false;
                        break;

                    default:
                        if (char.IsWhiteSpace(c))
                        {
                            if (!lastCharWasNewline && (sb.Length > 0 && sb[sb.Length - 1] != ' '))
                            {
                                sb.Append(' ');
                            }
                        }
                        else
                        {
                            sb.Append(c);
                            lastCharWasNewline = false;
                        }
                        break;
                }
                i++;
            }
            return sb.ToString();
        }

        private void AppendIndent(StringBuilder sb, int indentLevel)
        {
            for (var j = 0; j < indentLevel; j++)
            {
                sb.Append("  ");
            }
        }

        private void CopySelected_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(JsonTextBox.SelectedText))
            {
                Clipboard.SetText(JsonTextBox.SelectedText);
                StatusTextBlock.Text = "Selected text copied to clipboard";
            }
        }

        private void CopyAll_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(JsonTextBox.Text))
            {
                Clipboard.SetText(JsonTextBox.Text);
                StatusTextBlock.Text = "All content copied to clipboard";
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            var findDialog = new Window
            {
                Title = "Find Text",
                Width = 300,
                Height = 120,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                Background = (System.Windows.Media.Brush)FindResource("WindowBackgroundBrush"),
                Foreground = (System.Windows.Media.Brush)FindResource("TextForegroundBrush")
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var searchPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10)
            };

            var searchLabel = new TextBlock
            {
                Text = "Search for:",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                Foreground = (System.Windows.Media.Brush)FindResource("TextForegroundBrush")
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
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10)
            };

            var findButton = new Button
            {
                Content = "Find",
                Width = 80,
                Margin = new Thickness(0, 0, 10, 0),
                Style = (Style)FindResource("ThemedButtonStyle")
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Style = (Style)FindResource("ThemedButtonStyle")
            };

            buttonPanel.Children.Add(findButton);
            buttonPanel.Children.Add(cancelButton);

            Grid.SetRow(searchPanel, 0);
            Grid.SetRow(buttonPanel, 1);

            grid.Children.Add(searchPanel);
            grid.Children.Add(buttonPanel);

            findDialog.Content = grid;

            findButton.Click += (s, args) =>
            {
                var searchText = searchTextBox.Text;
                if (!string.IsNullOrEmpty(searchText))
                {
                    var startIndex = JsonTextBox.CaretIndex;
                    if (startIndex >= JsonTextBox.Text.Length)
                        startIndex = 0;

                    var index = JsonTextBox.Text.IndexOf(searchText, startIndex, StringComparison.OrdinalIgnoreCase);
                    if (index == -1 && startIndex > 0)
                    {
                        index = JsonTextBox.Text.IndexOf(searchText, 0, StringComparison.OrdinalIgnoreCase);
                    }

                    if (index != -1)
                    {
                        JsonTextBox.Focus();
                        JsonTextBox.Select(index, searchText.Length);
                        JsonTextBox.ScrollToLine(JsonTextBox.GetLineIndexFromCharacterIndex(index));
                        StatusTextBlock.Text = $"Found '{searchText}'";
                        findDialog.Close();
                    }
                    else
                    {
                        StatusTextBlock.Text = $"Text '{searchText}' not found";
                    }
                }
            };

            cancelButton.Click += (s, args) => findDialog.Close();

            findDialog.ShowDialog();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Lua files (*.lua)|*.lua|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = ".lua",
                Title = "Save Balatro Save Data"
            };

            var result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    var content = _currentContent;
                    if (!string.IsNullOrEmpty(content))
                    {
                        if (!string.IsNullOrEmpty(JsonTextBox.SelectedText))
                        {
                            System.IO.File.WriteAllText(saveFileDialog.FileName, JsonTextBox.SelectedText);
                            StatusTextBlock.Text = $"Selected content saved to {System.IO.Path.GetFileName(saveFileDialog.FileName)}";
                        }
                        else
                        {
                            System.IO.File.WriteAllText(saveFileDialog.FileName, JsonTextBox.Text);
                            StatusTextBlock.Text = $"Formatted content saved to {System.IO.Path.GetFileName(saveFileDialog.FileName)}";
                        }
                    }
                    else
                    {
                        StatusTextBlock.Text = "No content to save";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    StatusTextBlock.Text = "Error saving file";
                }
            }
        }
    }
}
