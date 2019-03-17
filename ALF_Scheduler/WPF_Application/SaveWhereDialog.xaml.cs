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

        /// <summary>
        ///     Save Dialog window constructor.
        /// </summary>
        public SaveWhereDialog() {
            InitializeComponent();
        }

        /// <summary>
        ///     This handles logic for saving to a new file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFileButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
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
                if (!App.HomePage_Main.HasOpened) //TODO @KENNY create new excel file at filename: ExcelImporterExporter.CreateWorkBookAtPath(filename, xlApp, xlWorkbook);
                ALF_Scheduler.DataParser.SaveAllFacilitiesToWorkbook(App.Facilities, App.XlWorkbook);
                ExcelImporterExporter.SaveWorkbookToSpecifiedFile(filename, App.XlWorkbook);
            }
            Close();
        }

        /// <summary>
        ///     This handles logic for saving to the current file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentFileButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            ALF_Scheduler.DataParser.SaveAllFacilitiesToWorkbook(App.Facilities, App.XlWorkbook);
            ExcelImporterExporter.SaveWorkbookToOriginalFile(App.XlWorkbook);
            Close();
        }

        /// <summary>
        ///     This handles the user canceling the save.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }
    }
}
