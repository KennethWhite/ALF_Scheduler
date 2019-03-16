using Excel_Import_Export;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_Application {
    /// <summary>
    /// Interaction logic for SaveWhereDialog.xaml
    /// </summary>
    public partial class SaveWhereDialog : Window {

        public SaveWhereDialog() {
            InitializeComponent();
        }

        private void NewFileButton_Click(object sender, RoutedEventArgs e) {
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
            if (result == true) {
                // Save document
                var filename = dlg.FileName;
                ALF_Scheduler.DataParser.SaveAllFacilitiesToWorkbook(App.Facilities, App.XlWorkbook);
                ExcelImporterExporter.SaveWorkbookToSpecifiedFile(filename, App.XlWorkbook);
            }
            Close();

            if (App.HomePage_Main.IsClosing) App.HomePage_Main.Close();
        }

        private void CurrentFileButton_Click(object sender, RoutedEventArgs e) {
            ALF_Scheduler.DataParser.SaveAllFacilitiesToWorkbook(App.Facilities, App.XlWorkbook);
            ExcelImporterExporter.SaveWorkbookToOriginalFile(App.XlWorkbook);
            Close();
            
            if (App.HomePage_Main.IsClosing) App.HomePage_Main.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
