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
using ALF_Scheduler.Models;

namespace WPF_Application
{
    /// <summary>
    /// Interaction logic for EditCode.xaml
    /// </summary>
    public partial class EditCodeWindow : Window
    {
        private Code selectedCode { get; set; }
        private Brush red = new SolidColorBrush(Color.FromArgb(90, 255, 0, 0));
        private Brush green = new SolidColorBrush(Color.FromArgb(90, 0, 255, 0));
        public EditCodeWindow()
        {
            InitializeComponent();
            cb_Codes.ItemsSource = Code.CodesList;
        }

        /// <summary>
        /// Triggers when Code ComboBox selection changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Codes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCode = cb_Codes.SelectedItem as Code;

            if (selectedCode == null)
            {
                ClearValues();
                return;
            }

            LoadCode(selectedCode);            
        }

        /// <summary>
        /// Clears out data in TextBoxes
        /// </summary>
        private void ClearValues()
        {
            tb_Name.Text = "";
            tb_Desc.Text = "";
            tb_MinMonth.Text = "";
            tb_MaxMonth.Text = "";
        }

        /// <summary>
        /// Loads data into TextBoxes from given Code.
        /// </summary>
        /// <param name="code">Code to load data from.</param>
        private void LoadCode(Code code)
        {
            ClearValues();
            tb_Name.Text = code.Name;
            tb_Desc.Text = code.Description;
            tb_MinMonth.Text = code.MinMonth.ToString();
            tb_MaxMonth.Text = code.MaxMonth.ToString();
        }

        /// <summary>
        /// Triggers when submit button is clicked. Updates selected Code with new information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string name = tb_Name.Text;
            string desc = tb_Desc.Text;
            int minMonth;
            int maxMonth;

            if (string.IsNullOrWhiteSpace(name))
            {
                tb_Name.Background = red;
                ShowWarn("Code must be given a name");
                return;
            }

            try
            {
                minMonth = int.Parse(tb_MinMonth.Text);
            }
            catch
            {
                ShowWarn("There is invalid data in the 'Minimum Month' field.");
                tb_MinMonth.Background = red;
                return;
            }

            try
            {
                maxMonth = int.Parse(tb_MaxMonth.Text);
            }
            catch
            {
                ShowWarn("There is invalid data in the 'Maximum Month' field.");
                tb_MaxMonth.Background = red;
                return;
            }

            if (minMonth > maxMonth || maxMonth < minMonth)
            {
                var temp = minMonth;
                minMonth = maxMonth;
                maxMonth = temp;
            }

            Code newCode = new Code() { Name = name, Description = desc, MinMonth = minMonth, MaxMonth = maxMonth };

            tb_Name.Background = null;
            tb_Desc.Background = null;
            tb_MinMonth.Background = null;
            tb_MaxMonth.Background = null;

            try
            {
                Code.UpdateCode(selectedCode, newCode);
                Code.CodesList.Remove(selectedCode);
                Code.CodesList.Add(newCode);
                MessageBox.Show("Code successfully created.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                cb_Codes.SelectedItem = newCode;
            }
            catch
            {
                ShowWarn("Code not created. Unexpected error. Perhaps invalid data?");
            }            
        }

        /// <summary>
        /// Triggers when delete button is pressed. Deletes a code from file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Code.RemoveCode(selectedCode);
                Code.CodesList.Remove(selectedCode);
            }
            catch
            {
                ShowWarn("An unexpected error occured when trying to remove Code.");
            }
            
        }

        /// <summary>
        /// Triggers when cancel button is clicked. Closes window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Triggers when TextBox text is changed. Changes text to green.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if(tb.IsFocused) tb.Background = green;
        }

        /// <summary>
        /// Shows a warning MessageBox to user.
        /// </summary>
        /// <param name="msg">Message to display.</param>
        /// <returns></returns>
        private MessageBoxResult ShowWarn(string msg)
        {
            return MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
