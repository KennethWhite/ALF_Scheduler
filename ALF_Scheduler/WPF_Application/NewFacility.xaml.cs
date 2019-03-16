using System.Windows;
using ALF_Scheduler.Models;
using System.Windows.Media;
using System;
using System.Windows.Controls;

namespace WPF_Application
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NewFacilityWindow : Window
    {
        private Brush Red { get; set; }
        public NewFacilityWindow()
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
            this.Close();
        }

        /// <summary>
        /// Shows user a warning message in MessageBox.
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

        /// <summary>
        /// Triggers when Submit button is clicked. Adds Facility to Facility list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnSubmit(object sender, RoutedEventArgs e)
        {
            string facName = this.facName.Text;
            string fname = lic_fname.Text;
            string lname = lic_lname.Text;
            string licNum = this.licNum.Text;
            string unit = this.unit.Text;
            string city = this.city.Text;
            string zip = this.zip.Text;
            string bedCountStr = this.bedCount.Text;
            string info = this.info.Text;
            int bedCount;
            Facility newFac;

            if (string.IsNullOrWhiteSpace(facName))
            {
                ShowWarn("Facility Name can't be empty.");
                this.facName.Background = Red;
                return;
            }
            else if (string.IsNullOrWhiteSpace(fname))
            {
                ShowWarn("Licensee First Name can't be empty.");
                lic_fname.Background = Red;
                return;
            }
            else if (string.IsNullOrWhiteSpace(lname))
            {
                ShowWarn("Licensee Last Name can't be empty.");
                lic_lname.Background = Red;
                return;
            }
            else if (string.IsNullOrWhiteSpace(licNum))
            {
                ShowWarn("License Number can't be empty.");
                this.licNum.Background = Red;
                return;
            }
            else if (string.IsNullOrWhiteSpace(unit))
            {
                ShowWarn("Unit can't be empty.");
                this.unit.Background = Red;
                return;
            }
            else if (string.IsNullOrWhiteSpace(city))
            {
                ShowWarn("City can't be empty.");
                this.city.Background = Red;
                return;
            }
            else if (string.IsNullOrWhiteSpace(zip))
            {
                ShowWarn("Zip Code can't be empty.");
                this.zip.Background = Red;
                return;
            }
            else if (string.IsNullOrWhiteSpace(bedCountStr))
            {
                this.bedCount.Text = "0";
                bedCountStr = "0";
            }

            try
            {
                bedCount = int.Parse(bedCountStr);
            }
            catch
            {
                ShowWarn("Cannot convert data in 'Bed Count' field to number.");
                this.bedCount.Background = Red;
                return;
            }

            try
            {
                newFac = new Facility() { FacilityName = facName, LicenseeFirstName = fname, LicenseeLastName = lname,
                    LicenseNumber = licNum, Unit = unit, City = city, ZipCode = zip, NumberOfBeds = bedCount, SpecialInfo = info };
                newFac.ProposedDate = ScheduleGeneration.ScheduleGeneration.GenerateSingleDate(newFac, App.Facilities);
                App.HomePage.AddItemToFacilityList(newFac);
                Close();
                return;
            }
            catch
            {
                ShowWarn("There was an error creating the Facility.");
            }

            ShowWarn("An unknown error occured.");
        }
    }
}
