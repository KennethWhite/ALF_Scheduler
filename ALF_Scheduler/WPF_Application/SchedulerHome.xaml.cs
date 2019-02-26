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

namespace WPF_Application
{
    /// <summary>
    ///     Interaction logic for SchedulerHome.xaml
    /// </summary>
    public partial class SchedulerHome : Page
    {
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private GridViewColumnHeader _lastHeaderClicked;

        private bool isDetailTabBuilt = false; // checks if details tab has alredy been built or not.

        private string[] labelContent =
                    {
                "Facility Name", "Name of Licensee", "License Number", "Unit", "City", "ZipCode", "Number Of Beds",
                "Most Recent Full Inspection",
                "Previous Year Full Inspection", "Two Year Full Inspection", "Inspection Result", "Dates Of SOD",
                "Enforcement Notes", "Complaints",
                "Proposed Date", "Schedule Interval", "Month 15", "Month 18",
                "Special Info" };
        Facility fac1 = new Facility();
        /// <summary>
        ///     This constructor initializes the Scheduler Home page.
        /// </summary>
        public SchedulerHome()
        {
            InitializeComponent();
            FacilityList.ItemsSource = App.Facilities;
            HelperMethods.DateSelection(MonthlyCalendar);
            TabItemDetails = DetailsInit();

        }

        /// <summary>
        ///     This method searches through the list of facilities displayed for the specific string provided by the user.
        /// </summary>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var toSearch = SearchText.Text;
            Facility found = new Facility();//assign the found Facility here when found
            TabItemDetails.IsSelected = true;

        }

        /// <summary>
        ///     This method initializes the Details tab with Facility object property names.
        /// </summary>
        private TabItem DetailsInit()
        {
            if(isDetailTabBuilt != true)
            {
                //var list = DetailsView;
                Grid grid = DetailsGrid;
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                


                for (var x = 0; x < labelContent.Length; x++)
                {

                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    grid.RowDefinitions.Add(rowDefinition);
                    Label label = new Label();
                    label.Content = labelContent[x];

                    Grid.SetColumn(label, 0);
                    Grid.SetRow(label, x);
                    grid.Children.Add(label);
                }

                TabItemDetails.Content = grid;
                isDetailTabBuilt = true;
                return TabItemDetails;
            }
            return TabItemDetails;
        }

        /// <summary>
        ///     This method creates a new label for each item in the details tab.
        /// </summary>
        /// <param name="content">The string content to be displayed.</param>
        /// <param name="width">The width of the label, default is 175px.</param>
        /// <param name="alignRight">The horizontal text alignment, default left.</param>
        private Label CreateLabel(string content, double width = 175,
            HorizontalAlignment alignRight = HorizontalAlignment.Left)
        {
            var label = new Label();
            label.Content = content;
            label.FontSize = 12;
            label.Padding = new Thickness(5);
            label.Width = width;
            label.HorizontalAlignment = alignRight;
            return label;
        }

        /// <summary>
        ///     This method gathers data from the selected facility in the ListView and displays it next to its respective property
        ///     label.
        /// </summary>
        public void OpenDetails(Facility facToShow)
        {
            List<string> facProperties = facToShow.returnFacility(facToShow);
            for(int row = 0; row < labelContent.Length; row++)
            {
                TextBox txt = new TextBox();
                txt.Height = 20;
                txt.IsReadOnly = true;


                txt.Text = facProperties[row];
                Grid.SetColumn(txt, 1);
                Grid.SetRow(txt, row);
                DetailsGrid.Children.Add(txt);
            }
        }

        /// <summary>
        ///     This event is triggered when you switch tabs in the TabControl.
        /// </summary>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabItemFacilities.IsSelected){
                ; //had to temporariy do nothing here, otherwise it wouldn't run
            }
            else if(TabItemDetails.IsSelected)
            {
                if (isDetailTabBuilt != false)
                    OpenDetails(App.Facilities[0]);
                    
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
            ScheduleReturn schedule = ScheduleGeneration.ScheduleGeneration.GenerateSchedule(App.Facilities, 15.99);

            foreach (KeyValuePair<Facility,DateTime> keyValue in schedule.FacilitySchedule)
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

            MonthAvgLabel.Visibility = Visibility.Visible;
            MonthAvgVal.Visibility = Visibility.Visible;
            MonthAvgVal.Content = schedule.GlobalAvg;
        }

        /// <summary>
        ///     This click event will update the specified facility's most recent inspection information.
        /// </summary>
        private void SubmitForm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
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
                            direction = ListSortDirection.Ascending;
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
    }
}