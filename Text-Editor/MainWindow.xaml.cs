using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using Text_Editor.Properties;

namespace Text_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _hasTextChanged = false;
        private string _fileName = "";
        private string _dialogFileTypes = "Text file (*.txt)|*.txt|All files|*.*|C# file (*.cs)|*.cs|C++ file (*.cpp)|*.cpp|" +
                "HTML file (*.html, *.htm)|*.html;*.htm|Java file (*.java)|*.java|Javascript file (*.js)|*.js|" +
                "Visual Basic file (*.vb)|*.vb|XML file (*.xml)|*.xml|PHP file (*.php)|*.php";

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
                    case MessageBoxResult.No:
                        NewFile();
                        break;
                    default:
                        return;
                }
            }

            TxtBoxDoc.Clear();
            _hasTextChanged = false;
        }

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            SaveBeforeClosing_Prompt();
            NewFile();
        }

        private void NewFile()
        {
            this.Title = "Text editor";
            _fileName = "";
            _hasTextChanged = false;
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            SaveBeforeClosing_Prompt();

            OpenFile();
        }

        private void DetectSyntaxAndChange()
        {
            string fileType;
            byte indexfileType;

            // Change syntax upon detecting file name
            switch (_fileName.Substring(_fileName.LastIndexOf('.') + 1))
            {
                case ("cs"):
                    fileType = "C#";
                    indexfileType = 1;
                    break;
                case ("cpp"):
                    fileType = "C++";
                    indexfileType = 2;
                    break;
                case ("html"):
                case ("htm"):
                    fileType = "HTML";
                    indexfileType = 3;
                    break;
                case ("java"):
                    fileType = "Java";
                    indexfileType = 4;
                    break;
                case ("js"):
                    fileType = "Javascript";
                    indexfileType = 5;
                    break;
                case ("php"):
                    fileType = "PHP";
                    indexfileType = 6;
                    break;
                case ("vb"):
                    fileType = "Visual Basic";
                    indexfileType = 7;
                    break;
                case ("xml"):
                    fileType = "XML";
                    indexfileType = 8;
                    break;
                default:
                    fileType = "Text";
                    indexfileType = 0;
                    break;
            }

            ChangeSyntax(fileType);
            syntaxComboBox.SelectedIndex = indexfileType;
        }

        private void OpenFile()
        {
            OpenFileDialog openDlg = new OpenFileDialog
            {
                // Postavke dijaloga
                Filter = _dialogFileTypes, // Da filtrira tipove podataka

                // Pocetni direktorij
                InitialDirectory = File.Exists(_fileName) ?
                    _fileName.Remove(_fileName.LastIndexOf('\\')) :
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (openDlg.ShowDialog() == true)
            {
                TxtBoxDoc.Text = File.ReadAllText(openDlg.FileName);
                _fileName = openDlg.FileName;
                this.Title = "Text editor - " + _fileName.Substring(_fileName.LastIndexOf('\\') + 1);
                DetectSyntaxAndChange();
                _hasTextChanged = false;
            }
        }

        public void OpenFile(string filePath)
        {
            TxtBoxDoc.Text = File.ReadAllText(filePath);
            _fileName = filePath;
            this.Title = "Text editor - " + _fileName.Substring(_fileName.LastIndexOf('\\') + 1);
            DetectSyntaxAndChange();
            _hasTextChanged = false;
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(true);
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
                _fileName = saveDlg.FileName;
                this.Title = "Text editor - " + _fileName.Substring(_fileName.LastIndexOf('\\') + 1);
                _hasTextChanged = false;
                DetectSyntaxAndChange();
            }
        }

        private SaveFileDialog ReturnSaveDialog()
        {
            SaveFileDialog saveDlg = new SaveFileDialog
            {
                // Postavke dijaloga
                Filter = _dialogFileTypes, // Da filtrira tipove podataka

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

        private void MenuNightMode_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.NightMode = !Properties.Settings.Default.NightMode;
        }
    }
}
