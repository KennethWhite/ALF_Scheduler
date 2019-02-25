using System;
using System.Collections.Generic;
using System.Windows;
using ALF_Scheduler;
using ALF_Scheduler.Domain.Models;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Application = System.Windows.Application;
using Window = System.Windows.Window;

namespace WPF_Application
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ApplicationDbContext DbContext { get; }
        public Workbook XlWorkbook { get; }
        public Microsoft.Office.Interop.Excel.Application XlApp { get; }
        public static List<Facility> Facilities { get; }
        public static CalendarYear CalendarYearPage { get; set;}

        private void Application_Startup(object sender, StartupEventArgs e)
        {
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

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                var filename = dlg.FileName;
                init(filename);

                var home = new SchedulerHome();
                var mainWindow = new MainWindow();
                if (onStartup) sender.Close();
                mainWindow.Show();
            }
            else if (!onStartup)
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        ///     This method initializes the overall application and data/database with the specified excel file path
        ///     grabbed from the Open File Dialog Window in <see cref="App.OpenFile(Window, bool)" />
        /// </summary>
        /// <param name="path">The full path of the specified file to import.</param>
        public static void init(string path) {
            CalendarYearPage = new CalendarYear();

            // TODO @KENNY import excel file
            //            if (ExcelImporterExporter.LoadExcelFromFile(path, out XlApp, out XlWorkbook)) {
            //                DbContext = new ApplicationDbContext(new Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext>());
            //                CreateFacilities();
            //            }

            // This is where the DataParser should parse into facility object and db

            //List<Facility> items = FacilityService.FetchAll();
            var items = new List<Facility>(); //delete once I can do that ^

            //List<Facility> items = DataParser.FetchAll(path);
        }
    }
}