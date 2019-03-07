using ALF_Scheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Reflection;
using ALF_Scheduler.Models;
using ScheduleGeneration;
using ALF_Scheduler.Utilities;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace WPF_Application
{
    /// <summary>
    ///     Interaction logic for SchedulerHome.xaml
    /// </summary>
    public partial class SchedulerHome : Page
    {
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private GridViewColumnHeader _lastHeaderClicked;
        private bool _detailsChanged;
        private Facility _currentDisplayedFacility;
        private double _globalAvg = -1;
        private double _desiredAvg = 15.99;

        private bool isDetailTabBuilt = false; // checks if details tab has alredy been built or not.

        private string[] labelContent = {
            "Facility Name",
            "Name of Licensee",
            "License Number",
            "Unit",
            "City",
            "ZipCode",
            "Proposed Date",
            "Inspection Result",
            "Enforcement Notes",
            "Schedule Interval",
            "Most Recent Full Inspection",
            "Previous Year Full Inspection",
            "Two Year Full Inspection",
            "Month 15",
            "Month 18",
            "Number Of Beds",
            "Special Info",
            "Licensors",
            "Complaints",
            "Dates Of SOD",
        };

        
        /// <summary>
        ///     This constructor initializes the Scheduler Home page.
        /// </summary>
        public SchedulerHome()
        {
            InitializeComponent();
            FacilityList.ItemsSource = App.Facilities;
            HelperMethods.DateSelection(MonthlyCalendar);
            TabItemDetails = DetailsInit();
            InspectionResultFormInit();
            FacilityList.LostFocus += (s, e) => FacilityList.SelectedItem = null;
        }

        private void InspectionResultFormInit()
        {            
            FacilityBox.ItemsSource = App.Facilities;
            List<Code> codes = Code.getCodes();
            List<string> codeStr = new List<string>();
            foreach (var item in codes)
            {
                codeStr.Add(item.Name);
            }
            ResultCodeCombo.ItemsSource = codeStr;  
        }

        /// <summary>
        ///     This method searches through the list of facilities displayed for the specific string provided by the user.
        /// </summary>
        private void SearchText_TextChanged(object sender, RoutedEventArgs e) {

            var keyword = SearchText.Text.ToUpper();
            if (!keyword.Equals("SEARCH..")) {
                FacilityList.ItemsSource = App.Facilities;

                if (!keyword.Equals("SEARCH..")) {
                    List<Facility> tempList = new List<Facility>();

                    foreach (Facility fac in App.Facilities) {
                        var datastr = "";
                        foreach (var data in fac.ReturnFacility()) {
                            datastr += data + ";";
                        }
                        if (datastr.ToUpper().Contains(keyword)) tempList.Add(fac);
                    }

                    FacilityList.ItemsSource = tempList;
                    TabItemFacilities.IsSelected = true;
                }
            }
        }

        private void SearchText_GotFocus(object sender, RoutedEventArgs e) {
            if (SearchText.Text.Equals("Search..")) SearchText.Text = "";
        }

        private void SearchText_LostFocus(object sender, RoutedEventArgs e) {
            if (SearchText.Text.Equals("")) SearchText.Text = "Search..";
        }

        /// <summary>
        ///     This method opens the specified facility in the details tab.
        /// </summary>
        /// <param name="facility">The facility to be displayed in the details tab.</param>
        internal void DisplayFacility(Facility facility) {
            OpenDetails(facility);
        }

        /// <summary>
        ///     This method initializes the Details tab with Facility object property names.
        /// </summary>
        private TabItem DetailsInit()
        {
            if(isDetailTabBuilt != true)
            {
                if (StackPanelInfo.Children.Count != 0) StackPanelInfo.Children.Clear();
                for (var x = 0; x < labelContent.Length; x++)
                {
                    Label label = new Label();
                    label.Content = labelContent[x];
                    StackPanelLabels.Children.Add(label);
                }
                _currentDisplayedFacility = App.Facilities[0];
                isDetailTabBuilt = true;
                return TabItemDetails;
            }
            return TabItemDetails;
        }

        /// <summary>
        ///     This method gathers data from the selected facility in the ListView and displays it next to its respective property
        ///     label.
        /// </summary>
        public void OpenDetails(Facility facToShow) {
            TabItemDetails.IsSelected = true;
            StackPanelInfo.Children.Clear();
            _currentDisplayedFacility = facToShow;
            List<string> facProperties = facToShow.ReturnFacility();
            for(int row = 0; row < labelContent.Length; row++)
            {
                TextBox txt = new TextBox();
                txt.Height = 20;
                txt.Margin = new Thickness(0, 3, 0, 3);
                txt.IsReadOnly = false;

                if(row >= 9 && row <= 14)
                {
                    //We don't want to be able to edit certain fields
                    txt.IsReadOnly = true;
                    txt.Background = Brushes.Silver;
                }

                var info = facProperties[row];
                if (row == 12 && (info.Equals(facProperties[10]) || info.Equals(facProperties[11]))) txt.Text = "";
                else txt.Text = info;

                StackPanelInfo.Children.Add(txt);
                txt.TextChanged += new TextChangedEventHandler(DetailsTextChanged);
            }
        }

        /// <summary>
        ///     This event is triggered when you switch tabs in the TabControl.
        /// </summary>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabItemFacilities.IsSelected || TabItemInspectionResult.IsSelected){
                if (StackPanelInfo != null) StackPanelInfo.Children.Clear();
                if (_detailsChanged)
                {
                    DetailsSubmitButton.Visibility = Visibility.Hidden;
                    DetailsRevertButton.Visibility = Visibility.Hidden;
                    _detailsChanged = false;
                }
            }
            else if(TabItemDetails.IsSelected)
            {
                if (isDetailTabBuilt != false)
                    OpenDetails(_currentDisplayedFacility);
                    
            }
        }

        /// <summary>
        ///     This event method opens the CalendarYear page when the 'Whole Year' button is clicked.
        /// </summary>
        private void CalendarYearButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(App.CalendarYearPage);
        }

        /// <summary>
        /// Triggers schedule generation for the facility list then loads them into the facility objects.
        /// </summary>
        private void GenerateSchedule_Click(object sender, RoutedEventArgs e)
        {
            if(TabItemFacilities.IsSelected)
            {
                MessageBoxResult msg = MessageBox.Show("This will overwrite ALL proposed dates, are you sure you want to continue?\nIf you want to generate just one date, go into the facility's detail tab and then click Generate Schedule.", "Data Overwrite Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (msg != MessageBoxResult.Yes)
                    return;
                ScheduleReturn schedule = ScheduleGeneration.ScheduleGeneration.GenerateSchedule(App.Facilities, _desiredAvg);

                foreach (KeyValuePair<Facility, DateTime> keyValue in schedule.FacilitySchedule)
                {
                    try
                    {
                        App.Facilities.Find(fac => fac.Equals(keyValue.Key)).ProposedDate = keyValue.Value;
                    }
                    catch (Exception notFound)
                    {
                        ErrorLogger.LogInfo("Couldn't find matching facility in App.Facilities, thus ProposedDate could not be set.", notFound);
                    }
                }

                _globalAvg = schedule.GlobalAvg;
            }
            else if (TabItemDetails.IsSelected)
            {
                if (_globalAvg == -1)
                {
                    _globalAvg = ScheduleGeneration.ScheduleGeneration.GetGlobalAverage(App.Facilities);
                }

                var newDate = ScheduleGeneration.ScheduleGeneration.GenerateSingleDate(_currentDisplayedFacility, App.Facilities);

                _currentDisplayedFacility.ProposedDate = newDate;

                _globalAvg = ScheduleGeneration.ScheduleGeneration.GetGlobalAverage(App.Facilities);
                OpenDetails(_currentDisplayedFacility);
            }
            MonthAvgLabel.Visibility = Visibility.Visible;
            MonthAvgVal.Visibility = Visibility.Visible;
            MonthAvgVal.Content = _globalAvg;
            FacilityList.Items.Refresh();
            HelperMethods.RefreshCalendars(MonthlyCalendar);
        }

        /// <summary>
        ///     This click event will update the specified facility's most recent inspection information.
        /// </summary>
        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null) {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding) {
                    if (headerClicked != _lastHeaderClicked) {
                        direction = ListSortDirection.Ascending;
                    } else {
                        if (_lastDirection == ListSortDirection.Ascending)
                            direction = ListSortDirection.Descending;
                        else {
                            CollectionViewSource.GetDefaultView(FacilityList.ItemsSource).SortDescriptions.Clear();
                            _lastHeaderClicked.Column.HeaderTemplate = null;
                            _lastHeaderClicked = null;
                            return;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                        headerClicked.Column.HeaderTemplate =
                            Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    else
                        headerClicked.Column.HeaderTemplate =
                            Resources["HeaderTemplateArrowDown"] as DataTemplate;

                    // Remove arrow from previously sorted header  
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                        _lastHeaderClicked.Column.HeaderTemplate = null;

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        /// <summary>
        /// Default Microsoft sort method.
        /// </summary>
        /// <param name="sortBy">The header name of the column to sort by.</param>
        /// <param name="direction">Sort direction of desired sort.</param>
        private void Sort(string sortBy, ListSortDirection direction)
        {
            //Not too sure if this will sort, but I think it should. Can't really test it though yet because excel file not coming in.
            var dataView =
                CollectionViewSource.GetDefaultView(FacilityList.ItemsSource);

            dataView.SortDescriptions.Clear();
            var sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        /// <summary>
        /// When a facility is double clicked in the list of facilities displayed, this method will open that facility in the details tab.
        /// </summary>
        private void FacilityList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(FacilityList.HasItems)
            { 
                var selected = FacilityList.SelectedItem;
                if(selected != null)
                {
                    StackPanelInfo.Children.Clear();
                    _currentDisplayedFacility = (Facility)selected;
                    OpenDetails(_currentDisplayedFacility);
                }
            }
        }

        private void SubmitForm_Click(object sender, RoutedEventArgs e)//Need to connect with Colton on how to build  Code, then Inspection, then add Inspection to corresponding Facility in App.Facilities.
        {
            string find = FacilityBox.Text;
            string code = ResultCodeCombo.SelectedItem.ToString();
            string date = dateBox.SelectedDate.Value.ToShortDateString();

            if (find != null && code != null && date != null)
            {
                var check = App.Facilities.Find(x => x.FacilityName.CompareTo(find) == 0);
                if (check != null)
                {
                    Inspection toAdd = CreateInspection(date, check.Licensors, code);
                    check.AddInspection(toAdd);
                }
                OpenDetails(check);

                FacilityBox.Text = null;
                ResultCodeCombo.SelectedItem = null;
                dateBox.Text = null;
            }
            else
            {
                TabItemInspectionResult.IsSelected = true;
            }

        }
        private static Inspection CreateInspection(string date, string licensor = "", string code = "")
        {
            var ret = new Inspection { InspectionDate = DateTime.Parse(date) };
            if (!string.IsNullOrEmpty(code)) ret.Code = Code.getCodeByName(code);
            ret.Licensor = licensor;
            return ret;
        }

        /// <summary>
        /// Text changed handler for Details tab text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailsTextChanged(object sender, EventArgs e)
        {
            _detailsChanged = true;
            TextBox txt = (TextBox)sender;
            Brush green = new SolidColorBrush(Color.FromArgb(90, 0, 255, 0));
            txt.Background = green;
            DetailsSubmitButton.Visibility = Visibility.Visible;
            DetailsRevertButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Will revert changes made to details tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailsRevertButton_Click(object sender, RoutedEventArgs e)
        {
            _detailsChanged = false;
            DetailsSubmitButton.Visibility = Visibility.Hidden;
            DetailsRevertButton.Visibility = Visibility.Hidden;

            StackPanelInfo.Children.Clear();
            OpenDetails(_currentDisplayedFacility);
        }

        /// <summary>
        /// Updates the current selected Faciltiy's details when this button is cliked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailsSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var info = StackPanelInfo.Children;
            int count = 0;
            var fac = _currentDisplayedFacility;
            TextBox selText = null;

            try
            {
                foreach (TextBox txt in info)
                {
                    selText = txt;
                    //TODO Maybe error checking?
                    switch (count) {
                        case 0:
                            fac.FacilityName = txt.Text;
                            break;
                        case 1:
                            var ara = txt.Text.Split(new char[] { ',' });
                            if (ara.Length >= 2)
                            {
                                fac.LicenseeLastName = ara[0].Trim();
                                fac.LicenseeFirstName = ara[1].Trim();
                            }
                            break;
                        case 2:
                            fac.LicenseNumber = txt.Text;
                            break;
                        case 3:
                            fac.Unit = txt.Text;
                            break;
                        case 4:
                            fac.City = txt.Text;
                            break;
                        case 5:
                            fac.ZipCode = txt.Text;
                            break;
                        case 6:
                            if (txt.Text.Any())
                                fac.ProposedDate = DateTime.Parse(txt.Text);
                            else
                                fac.ProposedDate = new DateTime();
                            break;
                        case 7:
                            break;
                        case 8:
                            fac.EnforcementNotes = txt.Text;
                            break;
                        case 9:
                            break;
                        case 10:
                            break;
                        case 11:
                            break;
                        case 12:
                            break;
                        case 13:
                            break;
                        case 14:
                            break;
                        case 15:
                            if (txt.Text.Any())
                                fac.NumberOfBeds = int.Parse(txt.Text);
                            else
                                fac.NumberOfBeds = 0;
                            break;
                        case 16:
                            fac.SpecialInfo = txt.Text;
                            break;
                        case 17:
                            fac.Licensors = txt.Text;
                            break;
                        case 18:
                            fac.Complaints = txt.Text;
                            break;
                        case 19:
                            var sods = txt.Text.Split(new char[] { ',' });
                            var dates = "";
                            if (sods.Length >= 2) {
                                foreach (string date in sods)
                                    dates += string.Format("{0}, ", DateTime.Parse(date.Trim()).ToShortDateString());
                                fac.DatesOfSOD = dates;
                            } 
                            else if (txt.Text.Any())
                                fac.DatesOfSOD += DateTime.Parse(txt.Text).ToShortDateString();
                            else
                                fac.DatesOfSOD = "";
                            break;
                        default:
                            break;
                    }
                    count++;
                }
                foreach (TextBox txt in info)
                {
                    txt.Background = Brushes.White;
                }
                _detailsChanged = true;
                DetailsSubmitButton.Visibility = Visibility.Hidden;
                DetailsRevertButton.Visibility = Visibility.Hidden;

                FacilityList.Items.Refresh();
                StackPanelInfo.Children.Clear();
                OpenDetails(_currentDisplayedFacility);
            }
            catch (Exception)
            {
                Brush red = new SolidColorBrush(Color.FromArgb(90, 255, 0, 0));
                selText.Background = red;
                MessageBoxResult msg = MessageBox.Show("The data: '" + selText.Text + "' is invalid for the data type.", "Data entry error", MessageBoxButton.OK, MessageBoxImage.Error);
            }    
        }
    }
}