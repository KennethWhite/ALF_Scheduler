using System.Windows;
using ALF_Scheduler.Models;
using System.Windows.Media;
using System;
using System.IO;
using System.Windows.Controls;

namespace WPF_Application
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NewCodeWindow : Window
    {
        private Brush Red { get; set; }

        /// <summary>
        ///     This constructor makes a new code window and intializes a red brush for errors.
        /// </summary>
        public NewCodeWindow()
        {
            Red = new SolidColorBrush(Color.FromArgb(90, 255, 0, 0));
            InitializeComponent();
        }

        /// <summary>
        /// Triggers when Cancel button is clicked. Closes window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Triggers when Submit button is clicked. Adds new code to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnSubmit(object sender, RoutedEventArgs e)
        {
            string fileName = this.fileName.Text;
            string name = codeName.Text;
            string desc = this.desc.Text;
            int minMonth;
            int maxMonth;

            if (string.IsNullOrWhiteSpace(name))
            {
                codeName.Background = Red;
                ShowWarn("Code must be given a name");
                return;
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "Codes_" + DateTime.Now.ToString("yyyy'_'MM'_'dd'T'HHmmss") + ".xml";
                this.fileName.Text = fileName;
            }
            else if (!fileName.EndsWith(".xml", System.StringComparison.InvariantCultureIgnoreCase))
            {
                fileName = this.fileName.Text + ".xml";
            }

            try
            {
                minMonth = int.Parse(this.minMonth.Text);
            }
            catch
            {
                ShowWarn("There is invalid data in the 'Minimum Month' field.");
                this.minMonth.Background = Red;
                return;
            }

            try
            {
                maxMonth = int.Parse(this.maxMonth.Text);
            }
            catch
            {
                ShowWarn("There is invalid data in the 'Maximum Month' field.");
                this.maxMonth.Background = Red;
                return;
            }

            if (minMonth > maxMonth || maxMonth < minMonth)
            {
                var temp = minMonth;
                minMonth = maxMonth;
                maxMonth = temp;
            }

            Code code = new Code() { Name = name, Description = desc, MinMonth = minMonth, MaxMonth = maxMonth };

            try
            {
                Code.AddCodeToFile(code, fileName);
                Code.CodesList.Add(code);
                Close();
                return;
            }
            catch (FileFormatException)
            {
                ShowWarn("Current file name not accepted.");
                this.fileName.Background = Red;
            }
            catch
            {
                ShowWarn("Something unexpected happened.");
            }

            ShowWarn("An unknown error occured.");
        }

        /// <summary>
        /// Shows user a MessageBox with warning message in it.
        /// </summary>
        /// <param name="msg">Message to display.</param>
        private void ShowWarn(string msg)
        {
            MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Triggers when TextBox text is changed. Clears background.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChanged(object sender, RoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.Background = null;
        }
    }
}
