using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace WPF_Application
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     This method handles the user clicking the open button in the menu.
        /// </summary>
        private void Menu_Open_Click(object sender, RoutedEventArgs e)
        {
            App.OpenFile(this, true);
        }

        /// <summary>
        ///     This method handles the user clicking the save button in the menu.
        /// </summary>
        private void Menu_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        /// <summary>
        ///     This is a helper method for saving the file.
        /// </summary>
        private void SaveFile()
        {
            var save = CreateSaveDialog();
            var app = (App) Application.Current;

            // Process message box results
            switch (save)
            {
                case MessageBoxResult.Yes:

                    // Configure save file dialog box
                    var dlg = new SaveFileDialog();
                    dlg.FileName = "Document"; // Default file name
                    dlg.DefaultExt = ".xlsx"; // Default file extension
                    dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension

                    // Show save file dialog box
                    var result = dlg.ShowDialog();

                    // Process save file dialog box results
                    if (result == true)
                    {
                        // Save document
                        var filename = dlg.FileName;
                        //Excel_Import_Export.ExcelImporterExporter.SaveWorkbookToSpecifiedFile(filename, app.XlWorkbook);
                    }

                    break;
                case MessageBoxResult.No:
                    //Excel_Import_Export.ExcelImporterExporter.SaveWorkbookToOriginalFile(app.XlWorkbook);
                    break;
            }
        }

        /// <summary>
        ///     This is a helper method that creates and displays the MessageBox before closing the window.
        /// </summary>
        private MessageBoxResult CreateSaveDialog()
        {
            var messageBoxText = "Do you want to save changes to a new file?";
            var caption = "Word Processor";
            var button = MessageBoxButton.YesNo;
            var icon = MessageBoxImage.Question;
            return MessageBox.Show(messageBoxText, caption, button, icon);
        }


        /// <summary>
        ///     This method handles the user clicking the exit button in the menu.
        /// </summary>
        /// <remarks>
        ///     This activates the Closing event:
        ///     <see cref="NavigationWindow_Closing(object, System.ComponentModel.CancelEventArgs)" />
        /// </remarks>
        private void Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     This method handles the closing event that occurs when the user clicks
        ///     the exit button in the menu or the 'X' in the window itself.
        /// </summary>
        private void NavigationWindow_Closing(object sender, CancelEventArgs e)
        {
            if (true /* if any changes have been made to a facility or inspection form has been filled out */)
            {
                var result = CreateWarningDialog();

                // Process message box results
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveFile();
                        Environment.Exit(0);
                        break;
                    case MessageBoxResult.No:
                        Environment.Exit(0);
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        /// <summary>
        ///     This is a helper method that creates and displays the MessageBox before closing the window.
        /// </summary>
        private MessageBoxResult CreateWarningDialog()
        {
            var messageBoxText = "Do you want to save changes?";
            var caption = "Word Processor";
            var button = MessageBoxButton.YesNoCancel;
            var icon = MessageBoxImage.Warning;
            return MessageBox.Show(messageBoxText, caption, button, icon);
        }

        /// <summary>
        ///     This method handles the user clicking the Year button in the menu.
        /// </summary>
        private void Menu_YearView_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(App.CalendarYearPage);
        }

        /// <summary>
        ///     This method handles the user clicking the Filter button in the menu.
        /// </summary>
        private void Menu_Filter_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}