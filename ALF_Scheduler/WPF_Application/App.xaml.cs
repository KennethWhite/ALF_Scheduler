using System;
using System.Collections.Generic;
using System.Windows;
using ALF_Scheduler;
using ALF_Scheduler.Domain.Models;
using Excel_Import_Export;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Application = System.Windows.Application;
using Window = System.Windows.Window;

namespace WPF_Application
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static ApplicationDbContext DbContext { get; set; }
        public static Workbook XlWorkbook { get; }
        public static Microsoft.Office.Interop.Excel.Application XlApp { get; }
        public static List<Facility> Facilities { get; }
        public static CalendarYear CalendarYearPage { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e) {
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
                Environment.Exit(0);
            }
        }

        /// <summary>
        ///     This method initializes the overall application and data/database with the specified excel file path
        ///     grabbed from the Open File Dialog Window in <see cref="App.OpenFile(Window, bool)" />
        /// </summary>
        /// <param name="path">The full path of the specified file to import.</param>
        public static void Init(string path) {
            CalendarYearPage = new CalendarYear();

            //if (ExcelImporterExporter.LoadExcelFromFile(path, out XlApp, out XlWorkbook)) {
            //    DbContext = new ApplicationDbContext(new Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext>());
            //    CreateFacilities
            //}

            // This is where the DataParser should parse into facility object and db

            //List<Facility> items = FacilityService.FetchAll();
            var items = new List<Facility>(); //delete once I can do that ^

            //List<Facility> items = DataParser.FetchAll(path);
        }

        //        public void ConfigureServices(IServiceCollection services)
        //        {
        //            services.AddScoped<FacilityService>();
        //            services.AddScoped<Inspection>();
        //
        //            var connection = new SqliteConnection("DataSource=:memory:");
        //            connection.Open();
        //            services.AddDbContext<ApplicationDbContext>(builder =>
        //            {
        //                builder.UseSqlite(connection);
        //            });
        //
        //            //var dependencyContext = DependencyContext.Default;
        //            //var assemblies = dependencyContext.RuntimeLibraries.SelectMany(lib =>
        //            //    lib.GetDefaultAssemblyNames(dependencyContext)
        //            //        .Where(a => a.Name.Contains("Scheduler")).Select(Assembly.Load)).ToArray();
        //            //services.AddAutoMapper(assemblies);
        //        }
    }
}