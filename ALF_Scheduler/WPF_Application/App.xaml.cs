using System;
using System.Windows;
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

            // Show open file dialog box
            var result = dlg.ShowDialog();


            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                var filename = dlg.FileName;
                var home = new SchedulerHome(filename);

                var mainWindow = new MainWindow();
                if (onStartup) sender.Close();
                mainWindow.Show();
            }
            else if (!onStartup)
            {
                Environment.Exit(0);
            }
        }
    }
}