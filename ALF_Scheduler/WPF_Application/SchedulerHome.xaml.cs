using ALF_Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        //private bool _AddNewFacility = false; //When false, a New Facility is not trying to be created. When true, a new fac is trying to be created. On start false, after submiting a valid Facility, reset back to false.
        private GridViewColumnHeader _lastHeaderClicked;
        private Facility _currentDisplayedFacility;
        private double _globalAvg = -1;
        private double _desiredAvg = 15.99;
        private Details _details;


        /// <summary>
        ///     This constructor initializes the Scheduler Home page.
        /// </summary>
        public SchedulerHome()
        {
            InitializeComponent();
            _details = new Details(this);
            FacilityList.ItemsSource = App.Facilities;
            HelperMethods.DateSelection(MonthlyCalendar);
            InspectionResultFormInit();
            FacilityList.LostFocus += (s, e) => FacilityList.SelectedItem = null;
        }

        //This method will assign the global variable _currentDisplayedFacility to the facility that is trying to be shown. Setting it inside it's own method will make appropriate use of encapsulation.
        private Facility SetGlobal_currentDisplayedFacility(Facility facToShow)
        {
            return this._currentDisplayedFacility = facToShow;
        }
        public void InspectionResultFormInit()
        {
            FacilityBox.ItemsSource = App.Facilities;
            ObservableCollection<Code> codes = Code.getCodes();
            ResultCodeCombo.ItemsSource = codes;
        }

        public void RefreshFacilityList()
        {
            FacilityList.Items.Refresh();
        }

        public void AddItemToFacilityList(Facility fac)
        {
            App.Facilities.Add(fac);
            RefreshFacilityList();
        }

        /// <summary>
        ///     This method searches through the list of facilities displayed for the specific string provided by the user.
        /// </summary>
        private void SearchText_TextChanged(object sender, RoutedEventArgs e)
        {

            var keyword = SearchText.Text.ToUpper();
            if (!keyword.Equals("SEARCH.."))
            {
                FacilityList.ItemsSource = App.Facilities;

                if (!keyword.Equals("SEARCH.."))
                {
                    List<Facility> tempList = new List<Facility>();

                    foreach (Facility fac in App.Facilities)
                    {
                        var datastr = "";
                        foreach (var data in fac.ReturnFacility())
                        {
                            datastr += data + ";";
                        }
                        if (datastr.ToUpper().Contains(keyword)) tempList.Add(fac);
                    }

                    FacilityList.ItemsSource = tempList;
                    TabItemFacilities.IsSelected = true;
                }
            }
        }

        private void SearchText_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchText.Text.Equals("Search..")) SearchText.Text = "";
        }

        private void SearchText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchText.Text.Equals("")) SearchText.Text = "Search..";
        }

        /// <summary>
        ///     This method opens the specified facility in the details tab.
        /// </summary>
        /// <param name="facility">The facility to be displayed in the details tab.</param>
        internal void DisplayFacility(Facility facility)
        {
            _details.DisplayFacility(facility);
            TabItemDetails.IsSelected = true;
        }
        
        /// <summary>
        ///     This event is triggered when you switch tabs in the TabControl.
        /// </summary>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabItemFacilities.IsSelected || TabItemInspectionResult.IsSelected)
            {
                
            }
            else if (TabItemDetails.IsSelected)
            {
                _details.DisplayFacility(_currentDisplayedFacility);
            }
        }

        /// <summary>
        ///     This event method opens the CalendarYear page when the 'Whole Year' button is clicked.
        /// </summary>
        private void CalendarYearButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(App.CalendarYearPage);
        }

        private bool IsAnyProposedDate()
        {
            bool propDate = false;
            foreach (Facility facility in App.Facilities)
            {
                if (!facility.ProposedDate.Equals(new DateTime()))
                    propDate = true;
            }
            return propDate;
        }

        /// <summary>
        /// Triggers schedule generation for the facility list then loads them into the facility objects.
        /// </summary>
        private void GenerateSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (TabItemFacilities.IsSelected)
            {
                if (IsAnyProposedDate())
                {
                    MessageBoxResult msg = MessageBox.Show("This will overwrite ALL proposed dates, are you sure you want to continue?\nIf you want to generate just one date, go into the facility's detail tab and then click Generate Schedule.", "Data Overwrite Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (msg != MessageBoxResult.Yes)
                        return;
                }

                ScheduleReturn schedule = ScheduleGeneration.ScheduleGeneration.GenerateSchedule(App.Facilities, _desiredAvg);

                foreach (KeyValuePair<Facility, DateTime> keyValue in schedule.FacilitySchedule)
                {
                    try
                    {
                        App.Facilities.FirstOrDefault(fac => fac.Equals(keyValue.Key)).ProposedDate = keyValue.Value;
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
                _details.DisplayFacility(_currentDisplayedFacility);
            }
            MonthAvgLabel.Visibility = Visibility.Visible;
            MonthAvgVal.Visibility = Visibility.Visible;
            MonthAvgVal.Content = _globalAvg;
            RefreshFacilityList();
            HelperMethods.RefreshCalendars(MonthlyCalendar);
        }

        /// <summary>
        ///     This click event will update the specified facility's most recent inspection information.
        /// </summary>
        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                            direction = ListSortDirection.Descending;
                        else
                        {
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
            if (FacilityList.HasItems)
            {
                var selected = FacilityList.SelectedItem;
                if (selected != null)
                {
                    _currentDisplayedFacility = SetGlobal_currentDisplayedFacility((Facility)selected);
                    TabItemDetails.IsSelected = true;
                }
            }
        }

        private void SubmitForm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                    string find = FacilityBox.Text;
                    Code code = (Code) ResultCodeCombo.SelectedItem;
                    string date = dateBox.SelectedDate.Value.ToShortDateString();

                    var check = App.Facilities.FirstOrDefault(x => x.FacilityName.CompareTo(find) == 0);
                    if (check != null)
                    {
                        Inspection toAdd = CreateInspection(date, check.Licensors, code);
                        check.AddInspection(toAdd);
                    }
                FacilityBox.Text = "";
                ResultCodeCombo.SelectedItem = null;
                dateBox.Text = null;
                EnforcementBox.Text = "";
                _currentDisplayedFacility = SetGlobal_currentDisplayedFacility((Facility)check);
                _details.DisplayFacility(_currentDisplayedFacility);
                RefreshFacilityList();
                TabItemDetails.IsSelected = true;
            }
            catch(Exception)
            {
                MessageBox.Show("Must enter all fields before submitting.");
            }

        }
        private static Inspection CreateInspection(string date, string licensor = "", Code code = null)
        {
            var ret = new Inspection { InspectionDate = DateTime.Parse(date) };
            if (code != null) ret.Code = code;
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
            TextBox txt = (TextBox)sender;
            _details.TextChanged(txt);
        }

        /// <summary>
        /// Will revert changes made to details tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailsRevertButton_Click(object sender, RoutedEventArgs e)
        {
            _details.HideButtons();
            _details.RevertChanges(_currentDisplayedFacility);
        }

        /// <summary>
        /// Updates the current selected Faciltiy's details when this button is cliked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailsSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            _details.SubmitChanges(_currentDisplayedFacility);
            RefreshFacilityList();
        }       

        private void NewFacility_Btn(object sender, RoutedEventArgs e)
        {
            NewFacilityWindow newFacility = new NewFacilityWindow() { Owner = App.HomePage_Main};
            newFacility.Show();
        }
    }
}