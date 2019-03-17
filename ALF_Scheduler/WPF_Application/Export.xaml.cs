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
using Microsoft.Win32;
using Excel_Import_Export;
using System.Windows.Controls.Primitives;
using System.IO;

namespace WPF_Application
{
    /// <summary>
    /// Interaction logic for Export.xaml
    /// </summary>
    public partial class Export : Window
    {
        /// <summary>
        ///     This initialies the Export window.
        /// </summary>
        public Export()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     This event handler exports all facilities with inspection dates into a new excel file 
        ///     if the date is within the specified number of months.
        /// </summary>
        private void Export_Click(object sender, RoutedEventArgs e) {
            string monthStr = ExportMonths.Text;
            int months;

            try {
                months = int.Parse(monthStr);
            } catch {
                MessageBox.Show("Months to export is not a number!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                ExportMonths.Background = new SolidColorBrush(Color.FromArgb(90, 255, 0, 0));
                return;
            }


            var dlg = new SaveFileDialog {
                FileName = "Document", // Default file name
                DefaultExt = ".xlsx", // Default file extension
                Filter = "Excel documents (.xlsx)|*.xlsx", // Filter files by extension
                CheckFileExists = false,
                CheckPathExists = true
            };

            // Show save file dialog box
            var result = dlg.ShowDialog();

            if (result == true) {
                App.SaveUpcomingInspectionsToExcel(dlg.FileName, months);
            }

            Close();
        }
    }
}
