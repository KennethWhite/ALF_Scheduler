﻿using ALF_Scheduler.Domain.Models;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ALF_Scheduler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public ApplicationDbContext DbContext { get; }
        public Excel.Workbook XlWorkbook { get; }
        public Excel.Application XlApp { get; }

        private void Application_Startup(object sender, StartupEventArgs e) {
            OpenFile(new MainWindow());
        }

        public static void OpenFile(Window sender, bool onStartup = false) {
            // Configure open file dialog box
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();


            // Process open file dialog box results
            if (result == true) {
                // Open document
                string filename = dlg.FileName;
                SchedulerHome home = new SchedulerHome();//filename);

                MainWindow mainWindow = new MainWindow();
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
