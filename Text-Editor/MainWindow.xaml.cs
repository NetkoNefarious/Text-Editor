using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;

namespace Text_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _hasTextChanged = false;
        string _fileName = "";

        public MainWindow()
        {
            InitializeComponent();
            TxtBoxDoc.FontSize = 14;
        }

        private void SaveBeforeClosing_Prompt()
        {
            if (_hasTextChanged)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to save before closing?", "Closing", MessageBoxButton.YesNoCancel);

                switch (messageBoxResult)
                {
                    case MessageBoxResult.Yes:
                        SaveFile();
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }

            TxtBoxDoc.Clear();
            _hasTextChanged = false;
        }

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            SaveBeforeClosing_Prompt();
            this.Title = "Text editor";
            _fileName = "";
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            SaveBeforeClosing_Prompt();

            OpenFileDialog openDlg = new OpenFileDialog
            {
                // Postavke dijaloga
                Filter = "Text file (*.txt)|*.txt|All files|", // Da filtrira tipove podataka

                // Pocetni direktorij
                InitialDirectory = File.Exists(_fileName) ?
                    _fileName.Remove(_fileName.LastIndexOf('\\')) :
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (openDlg.ShowDialog() == true)
            {
                TxtBoxDoc.Text = File.ReadAllText(openDlg.FileName);
            }

            _fileName = openDlg.FileName;
            this.Title = "Text editor - " + _fileName.Substring(_fileName.LastIndexOf('\\') + 1);
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
            _hasTextChanged = false;

            this.Title = "Text editor - " + _fileName.Substring(_fileName.LastIndexOf('\\') + 1);
        }

        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(true);
            _hasTextChanged = false;

            this.Title = "Text editor - " + _fileName.Substring(_fileName.LastIndexOf('\\') + 1);
        }

        private void SaveFile(bool saveAs = false)
        {
            if (File.Exists(_fileName) && !saveAs)
            {
                File.WriteAllText(_fileName, TxtBoxDoc.Text);
                return;
            }

            SaveFileDialog saveDlg = ReturnSaveDialog();

            if (saveDlg.ShowDialog() == true)
            {
                File.WriteAllText(saveDlg.FileName, TxtBoxDoc.Text);
            }

            _fileName = saveDlg.FileName;
        }

        private SaveFileDialog ReturnSaveDialog()
        {
            SaveFileDialog saveDlg = new SaveFileDialog
            {
                // Postavke dijaloga
                Filter = "Text file (*.txt)|*.txt|All files|", // Da filtrira tipove podataka

                // Postavimo početni folder (prvi slučaj za Save As, drugi za ostalo)
                InitialDirectory = File.Exists(_fileName) ?
                    _fileName.Remove(_fileName.LastIndexOf('\\')) :
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),

                // Postavimo ovo ako je All Files i nema ekstenzije napisane
                DefaultExt = "txt",
                AddExtension = true,
                FileName = _fileName.LastIndexOf('\\') != -1 ? _fileName.Substring(_fileName.LastIndexOf('\\') + 1) : _fileName
            };
            return saveDlg;
        }

        private void TxtBoxDoc_TextChanged(object sender, EventArgs e)
        {
            _hasTextChanged = true;
            if (this.Title[this.Title.Length - 1] != '*')
                this.Title += '*';
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            OpenAboutWindow();
        }

        private static void OpenAboutWindow()
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        private void ComboFontSize_DropDownClosed(object sender, EventArgs e)
        {
            ChangeFontSize();
        }

        private void ComboFontSize_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ChangeFontSize();
        }

        private void ChangeFontSize()
        {
            if (comboFontSize.Text != null)
            {
                try
                {
                    TxtBoxDoc.FontSize = double.Parse(comboFontSize.Text);
                }

                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private void SyntaxComboBox_OnDropDownClosed(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
                ChangeSyntax(comboBox.Text);
        }

        private void ChangeSyntax(string syntax)
        {
            var typeConverter = new HighlightingDefinitionTypeConverter();
            var syntaxHighlighter = (IHighlightingDefinition)typeConverter.ConvertFrom(syntax);
            TxtBoxDoc.SyntaxHighlighting = syntaxHighlighter;
        }

        private void MenuLineNumbers_OnClick(object sender, RoutedEventArgs e)
        {
            TxtBoxDoc.ShowLineNumbers = !TxtBoxDoc.ShowLineNumbers;
            menuLineNumbers.IsChecked = TxtBoxDoc.ShowLineNumbers;
            Properties.Settings.Default.LineNumbers = TxtBoxDoc.ShowLineNumbers;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            SaveBeforeClosing_Prompt();

            if (_hasTextChanged)
                e.Cancel = true;

            Properties.Settings.Default.Save();
        }

        private void MenuTodayDate_OnClick(object sender, RoutedEventArgs e)
        {
            TxtBoxDoc.Text += DateTime.Now;
        }

        private void MenuFind_Click(object sender, RoutedEventArgs e)
        {
            SearchPanel.Install(TxtBoxDoc);
        }

        private void Replace_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindReplaceDialog.ShowForReplace(TxtBoxDoc);
        }

        private void Replace_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindReplaceDialog.ShowForFind(TxtBoxDoc);
        }

        private void Find_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
