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

        }

        /// <summary>
        /// This method handles the user clicking the save button in the menu.
        /// </summary>
        private void Menu_Save_Click(object sender, RoutedEventArgs e) {

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
            if (true /* any changes have been made to a facility or inspection form has been filled out */) {
                MessageBoxResult result = CreateWarningDialog();

                // Process message box results
                switch (result) {
                    case MessageBoxResult.Yes:
                        //save file
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        // User pressed Cancel button
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
        /// This method handles the user clicking the Facilities button in the menu.
        /// </summary>
        private void Menu_FacilityView_Click(object sender, RoutedEventArgs e) {

        }
        
        /// <summary>
        /// This method handles the user clicking the Details button in the menu.
        /// </summary>
        private void Menu_DetailView_Click(object sender, RoutedEventArgs e) {

        }

        /// <summary>
        /// This method handles the user clicking the Form button in the menu.
        /// </summary>
        private void Menu_FormView_Click(object sender, RoutedEventArgs e) {

        }

        /// <summary>
        /// This method handles the user clicking the Year button in the menu.
        /// </summary>
        private void Menu_YearView_Click(object sender, RoutedEventArgs e) {

        }

        /// <summary>
        /// This method handles the user clicking the Search button in the menu.
        /// </summary>
        private void Menu_Search_Click(object sender, RoutedEventArgs e) {

        }

        /// <summary>
        /// This method handles the user clicking the Filter button in the menu.
        /// </summary>
        private void Menu_Filter_Click(object sender, RoutedEventArgs e) {

        }
    }
}
