using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ALF_Scheduler;
using ALF_Scheduler.Utilities;
using Excel_Import_Export;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Application = System.Windows.Application;
using Window = System.Windows.Window;
using XML_Utils;
using ALF_Scheduler.Models;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using ScheduleGeneration;

namespace WPF_Application
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <value>Excel Workbook</value>
        public static Workbook XlWorkbook { get; set; }

        /// <value>Excel Worksheet</value>
        public static Worksheet XlWorksheet { get; set; }

        /// <value>Excel Application</value>
        public static Excel.Application XlApp { get; set; }
        
        /// <value>ApplicationDbContext</value>
        public static ApplicationDbContext DbContext { get; set; }

        /// <value>ObservableCollection of all facilities</value>
        public static ObservableCollection<Facility> Facilities { get; set; }

        /// <value>CalendarYear page reference</value>
        public static CalendarYear CalendarYearPage { get; set; }

        /// <value>SchedulerHome page reference</value>
        public static SchedulerHome HomePage { get; set; }

        /// <value>MainWindow reference</value>
        public static MainWindow HomePage_Main { get; set; }

        /// <summary>
        ///     This method runs in startup. It initializes all of our settings, pages, and calendars.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            XML_Utils.XML_Utils.Init();
            Code.CodesList = Code.GetCodes();
            HomePage_Main = new MainWindow();
            
            Facilities = new ObservableCollection<Facility>();
            HomePage = new SchedulerHome();
            HomePage_Main.Frame.Source = new Uri("pack://application:,,,/SchedulerHome.xaml");
            CalendarYearPage = new CalendarYear();
            HomePage_Main.Show();
        }

        /// <summary>
        ///     This method opens a file dialog for the user to choose an excel file to open in our program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="onStartup"></param>
        public static bool OpenFile(Window sender, bool onStartup = false) {
            // Configure open file dialog box
            var dlg = new OpenFileDialog {
                FileName = "Document", // Default file name
                DefaultExt = ".xlsx", // Default file extension
                Filter = "Excel documents (.xlsx)|*.xlsx", // Filter files by extension
                CheckFileExists = true,
                CheckPathExists = true
            };


            // Process open file dialog box results
            if (dlg.ShowDialog() == true) {
                // Open document
                var filename = dlg.FileName;
                Init(filename);
                HomePage = new SchedulerHome();
                HomePage_Main.Frame.Navigate(HomePage);
                
                if (onStartup) sender.Close();
                return true;
            } else if (!onStartup) {
                ExcelImporterExporter.CloseExcelApp(XlApp, XlWorkbook);
                Environment.Exit(0);
            }
            return false;
        }

        /// <summary>
        ///     This method initializes the data with the specified excel file path
        ///     grabbed from the Open File Dialog Window in <see cref="App.OpenFile(Window, bool)" />
        /// </summary>
        /// <param name="path">The full path of the specified file to import.</param>
        public static void Init(string path)
        {
            if (!ExcelImporterExporter.LoadExcelFromFile(path, out Excel.Application xApp, out Workbook xBook)) {
                ErrorLogger.LogInfo($"Failed to load the file at {path}.");
            }

            XlWorkbook = xBook;
            XlApp = xApp;
            XlWorksheet = (Worksheet)XlWorkbook.Worksheets[1];
            Facilities = new ObservableCollection<Facility>();

            //This will cause Excel to clear formats in empty cells so the count is correct
            var WorksheetLastRow = XlWorksheet.Cells.Find(
                What: "*",
                SearchOrder: Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                SearchDirection: Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious,
                MatchCase: false
            ).Row;

            // get last column
            var WorksheetLastCol = XlWorksheet.Cells.Find(
                What: "*",
                SearchOrder: Excel.XlSearchOrder.xlByColumns,
                SearchDirection: Excel.XlSearchDirection.xlPrevious,
                MatchCase: false
            ).Column;

            int totalRows = XlWorksheet.UsedRange.Rows.Count;

            //Excel objects are indexed starting at 1, and first row is the header
            for (int index = 2; index <= WorksheetLastRow; index++)
            {
                Facilities.Add(DataParser.ParseFacility(XlWorksheet.UsedRange.Cells[index, 1] as Range));
            }
        }

        /// <summary>
        ///     This method exports the facilites with inspections within 
        ///     a specified number of months to a separate excel file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="months"></param>
        public static void SaveUpcomingInspectionsToExcel(string filePath, int months = 6)
        {
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = XlApp.Workbooks.Add(Type.Missing);
            xlWorkBook.Worksheets.Add();

            var facList = ScheduleGeneration.ScheduleGeneration.RetrieveFacilitiesWithInspectionsInXMonths(Facilities, months);
            ALF_Scheduler.DataParser.WriteFacilitiesToWorkbookForInspectionSchedule(facList, xlWorkBook);
            ExcelImporterExporter.SaveWorkbookToSpecifiedFile(filePath, xlWorkBook);
            xlWorkBook.Close();
            Marshal.ReleaseComObject(xlWorkBook);
        }
    }
}