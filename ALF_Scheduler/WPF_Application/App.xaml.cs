using System;
using System.Collections.Generic;
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

        public static Workbook XlWorkbook { get; set; }
        public static Worksheet XlWorksheet { get; set; }
        public static Microsoft.Office.Interop.Excel.Application XlApp { get; set; }

        public static ApplicationDbContext DbContext { get; set; }
        public static List<Facility> Facilities { get; set; }
        public static CalendarYear CalendarYearPage { get; set; }
        public static SchedulerHome HomePage { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            XML_Utils.XML_Utils.Init(); //This needs to be run to set up initial code file and folders
            OpenFile(new MainWindow());
        }

        public static void OpenFile(Window sender, bool onStartup = false)
        {
            // Configure open file dialog box
            var dlg = new OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;


            // Process open file dialog box results
            if (dlg.ShowDialog() == true)
            {
                // Open document
                var filename = dlg.FileName;
                Init(filename);

                HomePage = new SchedulerHome();
                var mainWindow = (MainWindow)sender;
                mainWindow.Frame.Source = new Uri("pack://application:,,,/SchedulerHome.xaml");
                CalendarYearPage = new CalendarYear();

                if (onStartup) sender.Close();
                mainWindow.Show();
            }
            else if (!onStartup)
            {
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

        public static void SaveFacilitiesToExcel(string filePath)
        {
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = XlApp.Workbooks.Add(Type.Missing);
            xlWorkBook.Worksheets.Add();

            //Need to retrieve desired number of months from user, default is 6
            var facList = ScheduleGeneration.ScheduleGeneration.RetrieveFacilitiesWithInspectionsInXMonths(App.Facilities);
            ALF_Scheduler.DataParser.WriteFacilitiesToWorkbook(facList, xlWorkBook);
            ExcelImporterExporter.SaveWorkbookToSpecifiedFile(filePath, xlWorkBook);
            xlWorkBook.Close();
            Marshal.ReleaseComObject(xlWorkBook);
        }
    }
}