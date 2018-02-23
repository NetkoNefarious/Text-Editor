﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Text_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool hasTextChanged = false;
        string fileName = "";

        public MainWindow()
        {
            InitializeComponent();
            txtBoxDoc.FontSize = 14;
        }

        private void CloseWithoutSaving_Prompt()
        {
            if (hasTextChanged)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to save before closing?", "Closing", MessageBoxButton.YesNoCancel);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    SaveFile();
                }

                else if (messageBoxResult == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            txtBoxDoc.Clear();
            hasTextChanged = false;
        }

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            CloseWithoutSaving_Prompt();
            this.Title = "Text editor";
            fileName = "";
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            CloseWithoutSaving_Prompt();

            OpenFileDialog openDlg = new OpenFileDialog
            {
                // Postavke dijaloga
                Filter = "Text file (*.txt)|*.txt|All files|", // Da filtrira tipove podataka

                // Pocetni direktorij
                InitialDirectory = File.Exists(fileName) ?
                    fileName.Remove(fileName.LastIndexOf('\\')) :
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (openDlg.ShowDialog() == true)
            {
                txtBoxDoc.Text = File.ReadAllText(openDlg.FileName);
            }

            fileName = openDlg.FileName;
            this.Title = "Text editor - " + fileName.Substring(fileName.LastIndexOf('\\') + 1);
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
            hasTextChanged = false;

            this.Title = "Text editor - " + fileName.Substring(fileName.LastIndexOf('\\') + 1);
        }

        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(true);
            hasTextChanged = false;

            this.Title = "Text editor - " + fileName.Substring(fileName.LastIndexOf('\\') + 1);
        }

        private void SaveFile(bool saveAs = false)
        {
            if (File.Exists(fileName) && !saveAs)
            {
                File.WriteAllText(fileName, txtBoxDoc.Text);
                return;
            }

            SaveFileDialog saveDlg = ReturnSaveDialog();

            if (saveDlg.ShowDialog() == true)
            {
                File.WriteAllText(saveDlg.FileName, txtBoxDoc.Text);
            }

            fileName = saveDlg.FileName;
        }

        private SaveFileDialog ReturnSaveDialog()
        {
            SaveFileDialog saveDlg = new SaveFileDialog
            {
                // Postavke dijaloga
                Filter = "Text file (*.txt)|*.txt|All files|", // Da filtrira tipove podataka

                // Postavimo početni folder (prvi slučaj za Save As, drugi za ostalo)
                InitialDirectory = File.Exists(fileName) ?
                    fileName.Remove(fileName.LastIndexOf('\\')) :
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),

                // Postavimo ovo ako je All Files i nema ekstenzije napisane
                DefaultExt = "txt",
                AddExtension = true,
                FileName = fileName.LastIndexOf('\\') != -1 ? fileName.Substring(fileName.LastIndexOf('\\') + 1) : fileName
            };
            return saveDlg;
        }

        private void TxtBoxDoc_TextChanged(object sender, TextChangedEventArgs e)
        {
            hasTextChanged = true;
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
                    txtBoxDoc.FontSize = double.Parse(comboFontSize.Text);
                }

                catch (Exception)
                { }
            }
        }
    }
}
