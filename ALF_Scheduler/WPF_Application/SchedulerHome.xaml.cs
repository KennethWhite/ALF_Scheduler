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
        /// <summary>
        ///     This constructor initializes the Scheduler Home page.
        /// </summary>
        public SchedulerHome()
        {
            InitializeComponent();
            FacilityList.ItemsSource = App.Facilities;
            CreateGridView();
            HelperMethods.DateSelection(MonthlyCalendar);
            TabItemDetails = DetailsInit();
        }

        public void CreateGridView()
        {
            /*
            <GridViewColumn Header="Name" Width="140" />
            <GridViewColumn Header="Last Inspection" Width="100" />
            <GridViewColumn Header="Result" Width="53" />
            <GridViewColumn Header="Month Interval" Width="100" />
            <GridViewColumn Header="Next Inspection" Width="103" />*/

            //FacilityList.View = facilityGridView;
            //g.Columns[0].DisplayMemberBinding = new Binding("FacilityName");
            //g.Columns[1].DisplayMemberBinding = new Binding("MostRecentFullInspectionString");
            //g.Columns[2].DisplayMemberBinding = new Binding("InspectionResult");
            //g.Columns[3].DisplayMemberBinding = new Binding("ScheduleInterval");
            //g.Columns[4].DisplayMemberBinding = new Binding("ProposedDateString");
        }

        /// <summary>
        ///     This method searches through the list of facilities displayed for the specific string provided by the user.
        /// </summary>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            //var toSearch = SearchText.Text;
            //if(App.Facilities.Contains())
            //{
                
            //}
            //TabItemDetails.IsSelected = true;
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
                txt.Margin = new Thickness(0, 3, 0, 3);
                txt.IsReadOnly = true;
                
                txt.Text = facProperties[row];
                StackPanelInfo.Children.Add(txt);
            }
        }

        /// <summary>
        ///     This event is triggered when you switch tabs in the TabControl.
        /// </summary>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabItemFacilities.IsSelected || TabItemInspectionResult.IsSelected){
                if (StackPanelInfo != null) StackPanelInfo.Children.Clear();
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
        ///     This click event will update the specified facility's most recent inspection information.
        /// </summary>

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

        private void FacilityList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(FacilityList.HasItems)
            { 
                TabItemDetails.IsSelected = true;
                StackPanelInfo.Children.Clear();
                OpenDetails((Facility)FacilityList.SelectedItem);
            }
        }

        private void SubmitForm_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}