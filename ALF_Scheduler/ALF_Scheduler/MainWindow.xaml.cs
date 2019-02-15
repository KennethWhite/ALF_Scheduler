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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ALF_Scheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow() {
            InitializeComponent();
        }

        /// <summary>
        /// This method handles the user clicking the open button in the menu.
        /// </summary>
        private void Menu_Open_Click(object sender, RoutedEventArgs e) {
            App.OpenFile(this, true);
        }

        /// <summary>
        /// This method handles the user clicking the save button in the menu.
        /// </summary>
        private void Menu_Save_Click(object sender, RoutedEventArgs e) {
            SaveFile();
        }

        /// <summary>
        /// This is a helper method for saving the file.
        /// </summary>
        private void SaveFile() {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true) {
                // Save document
                string filename = dlg.FileName;
            }
        }

        /// <summary>
        /// This method handles the user clicking the exit button in the menu.
        /// </summary>
        /// <remarks>
        /// This activates the Closing event: <see cref="NavigationWindow_Closing(object, System.ComponentModel.CancelEventArgs)"/>
        /// </remarks>
        private void Menu_Exit_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        /// <summary>
        /// This method handles the closing event that occurs when the user clicks 
        /// the exit button in the menu or the 'X' in the window itself.
        /// </summary>
        private void NavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (true /* if any changes have been made to a facility or inspection form has been filled out */) {
                MessageBoxResult result = CreateWarningDialog();

                // Process message box results
                switch (result) {
                    case MessageBoxResult.Yes:
                        //save file
                        App.Current.Shutdown();
                        break;
                    case MessageBoxResult.No:
                        App.Current.Shutdown();
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        /// <summary>
        /// This is a helper method that creates and displays the MessageBox before closing the window.
        /// </summary>
        private MessageBoxResult CreateWarningDialog() {
            string messageBoxText = "Do you want to save changes?";
            string caption = "Word Processor";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            return MessageBox.Show(messageBoxText, caption, button, icon);
        }
        
        /// <summary>
        /// This method handles the user clicking the Year button in the menu.
        /// </summary>
        private void Menu_YearView_Click(object sender, RoutedEventArgs e) {
            //HelperMethods.OpenCalendar();
        }

        /// <summary>
        /// This method handles the user clicking the Filter button in the menu.
        /// </summary>
        private void Menu_Filter_Click(object sender, RoutedEventArgs e) {

        }
    }
}
