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
        private Brush red { get; set; }
        public NewCodeWindow()
        {
            InitializeComponent();
            red = new SolidColorBrush(Color.FromArgb(90, 255, 0, 0));
        }

        public void onCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void onSubmit(object sender, RoutedEventArgs e)
        {
            string fileName = this.fileName.Text;
            string name = codeName.Text;
            string desc = this.desc.Text;
            int minMonth;
            int maxMonth;

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
                this.minMonth.Background = red;
                return;
            }

            try
            {
                maxMonth = int.Parse(this.maxMonth.Text);
            }
            catch
            {
                ShowWarn("There is invalid data in the 'Maximum Month' field.");
                this.maxMonth.Background = red;
                return;
            }

            Code code = new Code() { Name = name, Description = desc, MinMonth = minMonth, MaxMonth = maxMonth };

            try
            {
                Code.AddCodeToFile(code, fileName);
            }
            catch (FileFormatException)
            {
                ShowWarn("Current file name not accepted.");
                this.fileName.Background = red;
            }
            catch
            {
                ShowWarn("Something unexpected happened.");
            }

        }

        private MessageBoxResult ShowWarn(string msg)
        {
            return MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void TextChanged(object sender, RoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.Background = null;
        }
    }
}
