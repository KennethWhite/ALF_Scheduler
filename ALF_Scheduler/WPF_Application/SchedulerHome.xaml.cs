﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPF_Application
{
    /// <summary>
    ///     Interaction logic for SchedulerHome.xaml
    /// </summary>
    public partial class SchedulerHome : Page
    {
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private GridViewColumnHeader _lastHeaderClicked;

        /// <summary>
        ///     This constructor initializes the Scheduler Home page.
        /// </summary>
        public SchedulerHome()
        {
            InitializeComponent();
            FacilityList.ItemsSource = App.Facilities;
            HelperMethods.DateSelection(MonthlyCalendar);

            DetailsInit();
        }

        /// <summary>
        ///     This method searches through the list of facilities displayed for the specific string provided by the user.
        /// </summary>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var facility = SearchText.Text;
            // TODO search for facility based on text content ^ and open it in details page
            TabItemDetails.IsSelected = true;
        }

        /// <summary>
        ///     This method initializes the Details tab with Facility object property names.
        /// </summary>
        private void DetailsInit()
        {
            var list = DetailsView;

            string[] labelContent =
            {
                "Facility Name", "Name of Licensee", "License Number", "Unit", "City", "ZipCode", "Number Of Beds",
                "Most Recent Full Inspection",
                "Previous Year Full Inspection", "Two Year Full Inspection", "Inspection Result", "Dates Of SOD",
                "Enforcement Notes", "Failed Follow Up", "Complaints",
                "Proposed Date", "Schedule Interval", "Month 15", "Month 18", "Number Of Licensors", "Sample Size",
                "Special Info"
            };

            for (var x = 0; x < labelContent.Length; x++)
            {
                var stack = new StackPanel();
                stack.HorizontalAlignment = HorizontalAlignment.Stretch;
                stack.Orientation = Orientation.Horizontal;
                var label = CreateLabel(labelContent[x]);
                stack.Children.Add(label);
                label = CreateLabel("", 240, HorizontalAlignment.Right);
                stack.Children.Add(label);

                if (x == 0)
                {
                    var editButton = new Button();
                    editButton.Content = "Edit Facility";
                    editButton.Width = 70;
                    stack.Children.Add(editButton);
                }

                list.Items.Add(stack);
            }

            TabItemDetails.Content = list;
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
        public void OpenDetails()
        {
            //TODO input specific facility info into the labels, needs to be accessible based on tab switch as well (see below)
        }

        /// <summary>
        ///     This event is triggered when you switch tabs in the TabControl.
        /// </summary>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (TabItemDetails.IsSelected) {
            //    OpenDetails(); //send specific facility based on what's selected
            //} 
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