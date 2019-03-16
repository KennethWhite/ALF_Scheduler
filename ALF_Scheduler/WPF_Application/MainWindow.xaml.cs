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
        public bool IsClosing { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            IsClosing = false;
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
            var saveDialog = new SaveWhereDialog() { Owner = this };
            saveDialog.Show();
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
        private void NavigationWindow_Closing(object sender, CancelEventArgs e) {
            SaveFile(); //TODO: This method finishes before the dialog fully opens. Needs logic to prevent closing if the cancel button is pressed.
            if (true) { //IsClosing) {
                ExcelImporterExporter.CloseExcelApp(App.XlApp, App.XlWorkbook);
                Environment.Exit(0);
            } //else e.Cancel = true;
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
    }
}