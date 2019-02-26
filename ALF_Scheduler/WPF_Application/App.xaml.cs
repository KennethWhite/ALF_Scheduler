using System;
using System.Collections.Generic;
using System.Windows;
using ALF_Scheduler;
using ALF_Scheduler.Domain.Models;
using ALF_Scheduler.Utilities;
using Excel_Import_Export;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Application = System.Windows.Application;
using Window = System.Windows.Window;
using XML_Utils;

namespace WPF_Application
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public static Workbook XlWorkbook { get; set; }
        public static Worksheet XlWorksheet { get; set; }
        public static Microsoft.Office.Interop.Excel.Application XlApp { get; set; }

        public static ApplicationDbContext DbContext { get; set; }
        public static List<Facility> Facilities { get; set; }
        public static CalendarYear CalendarYearPage { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e) {
            XML_Utils.XML_Utils.Init(); //This needs to be run to set up initial code file and folders
            OpenFile(new MainWindow());
        }

        public static void OpenFile(Window sender, bool onStartup = false) {
            // Configure open file dialog box
            var dlg = new OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true) {
                // Open document
                var filename = dlg.FileName;
                Init(filename);

                var home = new SchedulerHome();
                var mainWindow = new MainWindow();
                if (onStartup) sender.Close();
                mainWindow.Show();
            } else if (!onStartup) {
                ExcelImporterExporter.CloseExcelApp(XlApp, XlWorkbook);
                Environment.Exit(0);
            }
        }

        /// <summary>
        ///     This method initializes the overall application and data/database with the specified excel file path
        ///     grabbed from the Open File Dialog Window in <see cref="App.OpenFile(Window, bool)" />
        /// </summary>
        /// <param name="path">The full path of the specified file to import.</param>
        public static void Init(string path)
        {
            CalendarYearPage = new CalendarYear();

            Workbook xBook;
            Microsoft.Office.Interop.Excel.Application xApp;
            if (!ExcelImporterExporter.LoadExcelFromFile(path, out xApp, out xBook))
            {
                ErrorLogger.LogInfo($"Failed to load the file at {path}.");
            }

            XlWorkbook = xBook;
            XlApp = xApp;
            XlWorksheet = (Worksheet)XlWorkbook.Worksheets[1];
            Facilities = new List<Facility>();

            //This will cause Excel to clear formats in empty cells so the count is correct
            XlWorksheet.Columns.ClearFormats();
            XlWorksheet.Rows.ClearFormats();

            int totalRows = XlWorksheet.UsedRange.Rows.Count;

            //Excel objects are indexed starting at 1, and first row is the header
            for (int index = 2; index <= totalRows; index++)
            {
                Facilities.Add(DataParser.ParseFacility(XlWorksheet.UsedRange.Cells[index, 1] as Range));
            }

        }

   
    }
}