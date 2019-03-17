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
        /// <value>A boolean for if an excel file is currently being opened.</value>
        public bool IsOpening { get; set; }
        /// <value>A boolean for if an excel file has being opened.</value>
        public bool HasOpened { get; set; }

        /// <summary>
        ///     The Main Navigation Window constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            IsOpening = false;
            HasOpened = false;
        }

        /// <summary>
        ///     This method handles the user clicking the open button in the menu.
        /// </summary>
        private void Menu_Open_Click(object sender, RoutedEventArgs e)
        {
            IsOpening = true;
            App.OpenFile(this, true);
            HasOpened = true;
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
        private bool? SaveFile()
        {
            var saveDialog = new SaveWhereDialog() { Owner = this };
            return saveDialog.ShowDialog();
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
        /// <remarks>
        ///     It will close automatically if no excel file has been opened, otherwise it checks
        ///     to see if you want to save your file, and where.
        /// </remarks>
        private void NavigationWindow_Closing(object sender, CancelEventArgs e) {
            bool? save = true;
            if (HasOpened) save = SaveMessageBox();

            if (save == null && !IsOpening) {
                save = true;
            }
            else if (save == null && IsOpening) {
                save = false;
                IsOpening = false;
            }

            if (save.Value && !IsOpening) {
                ExcelImporterExporter.CloseExcelApp(App.XlApp, App.XlWorkbook);
                Environment.Exit(0);
            } else {
                e.Cancel = true;
                IsOpening = false;
            }
        }

        /// <summary>
        ///     Helper method that opens a message box asking them to save or cancel. If they choose yes,
        ///     it will open a window to help them decide where to save.
        /// </summary>
        /// <returns>Nullable boolean based on the message box result.</returns>
        private bool? SaveMessageBox() {
            var messageBoxText = "Would you like to save your changes?";
            var caption = "Word Processor";
            var button = MessageBoxButton.YesNo;
            var icon = MessageBoxImage.Question;
            var result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result) {
                case MessageBoxResult.Yes:
                    return SaveFile();
                case MessageBoxResult.No:
                    return null;
                case MessageBoxResult.Cancel:
                    return false;
            }
            return false;
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