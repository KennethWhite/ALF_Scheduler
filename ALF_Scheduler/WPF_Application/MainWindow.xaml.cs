using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Win32;
using Excel_Import_Export;
using System.Windows.Controls.Primitives;
using System.IO;

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
        ///     This helper method opens a save file dialog for saving the file.
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
                    var dlg = new SaveFileDialog {
                        FileName = "Document", // Default file name
                        DefaultExt = ".xlsx", // Default file extension
                        Filter = "Excel documents (.xlsx)|*.xlsx", // Filter files by extension
                        CheckFileExists = false,
                        CheckPathExists = true
                    };

                    // Show save file dialog box
                    var result = dlg.ShowDialog();

                    // Process save file dialog box results
                    if (result == true)
                    {
                        // Save document
                        var filename = dlg.FileName;
                        ALF_Scheduler.DataParser.SaveAllFacilitiesToWorkbook(App.Facilities, App.XlWorkbook);
                        ExcelImporterExporter.SaveWorkbookToSpecifiedFile(filename, App.XlWorkbook);
                    }

                    break;
                case MessageBoxResult.No:
                    ALF_Scheduler.DataParser.SaveAllFacilitiesToWorkbook(App.Facilities, App.XlWorkbook);
                    ExcelImporterExporter.SaveWorkbookToOriginalFile(App.XlWorkbook);
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
                        ExcelImporterExporter.CloseExcelApp(App.XlApp, App.XlWorkbook);
                        Environment.Exit(0);
                        break;
                    case MessageBoxResult.No:
                        ExcelImporterExporter.CloseExcelApp(App.XlApp, App.XlWorkbook);
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
        private void Menu_HomeView_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(App.HomePage);
            HelperMethods.RefreshCalendars(App.HomePage.MonthlyCalendar);
        }

        /// <summary>
        ///     This method handles the user clicking the Year button in the menu.
        /// </summary>
        private void Menu_YearView_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(App.CalendarYearPage);
        }

        /// <summary>
        ///     This method handles the user clicking the export button in the menu.
        /// </summary>
        private void Menu_Export_Click(object sender, RoutedEventArgs e) {
            Export export = new Export() { Owner = this };
            export.Show();
        }

        /// <summary>
        ///     This method handles the user clicking the New Code button in the menu.
        /// </summary>
        private void Menu_New_Code_Click(object sender, RoutedEventArgs e)
        {
            NewCodeWindow newCode = new NewCodeWindow() { Owner = this };
            newCode.Show();
        }

        /// <summary>
        ///     This method handles the user clicking the New Facility button in the menu.
        /// </summary>
        private void Menu_New_Facility_Click(object sender, RoutedEventArgs e)
        {
            NewFacilityWindow newFacility = new NewFacilityWindow() { Owner = this };
            newFacility.Show();
        }

        /// <summary>
        /// Triggers when Edit>Code item is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Edit_Code_Click(object sender, RoutedEventArgs e)
        {
            EditCodeWindow editCode = new EditCodeWindow() { Owner = this };
            editCode.Show();
        }
    }
}