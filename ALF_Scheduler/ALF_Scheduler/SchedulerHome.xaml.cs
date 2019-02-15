﻿using Excel_Import_Export;
using Excel = Microsoft.Office.Interop.Excel;
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
using ALF_Scheduler.Domain.Models;
using ALF_Scheduler.Services;

namespace ALF_Scheduler {
    /// <summary>
    /// Interaction logic for SchedulerHome.xaml
    /// </summary>
    public partial class SchedulerHome : Page {

        private ApplicationDbContext DbContext { get; }
        private Excel.Workbook XlWorkbook;
        private Excel.Application XlApp;

        CalendarYear CalendarYearPage { get; }

        /// <summary>
        /// This constructor initializes the overall application and data/database with the specified excel file path 
        /// grabbed from the Open File Dialog Window in <see cref="App.OpenFile(Window, bool)"/>
        /// </summary>
        /// <param name="path">The full path of the specified file to import.</param>
        public SchedulerHome() { //string path) {
            InitializeComponent();
            CalendarYearPage = new CalendarYear();

            // TODO @KENNY import excel file
            //if (ExcelImporterExporter.LoadExcelFromFile(path, out XlApp, out XlWorkbook)) {
            //    /*
            //      System.InvalidOperationException
            //      HResult=0x80131509
            //      Message=No database provider has been configured for this DbContext. A provider can be configured by overriding the DbContext.OnConfiguring method or by using AddDbContext 
            //      on the application service provider. If AddDbContext is used, then also ensure that your DbContext type accepts a DbContextOptions<TContext> object in its constructor and 
            //      passes it to the base constructor for DbContext.
            //      Source=Microsoft.EntityFrameworkCore
            //    */
            //    DbContext = new ApplicationDbContext(new Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext>());
            //    CreateFacilities();
            //}

            // This is where the DataParser should parse into facility object and db
            //List<Facility> items = FacilityService.FetchAll();
            List<Facility> items = new List<Facility>(); //delete once I can do that ^
            FacilityList.ItemsSource = items;
            HelperMethods.DateSelection(items, MonthlyCalendar); //delete this if you use the CreateFacilities method below

            DetailsInit();
        }

        //TODO @KENNY is this already dealt with in your new dataparser? This and next method was just my attempt, feel free to delete or change.
        /// <summary>
        /// This helper method creates facilities for each row in the excel file. 
        /// </summary>
        private void CreateFacilities() {
            Services.FacilityService facilityService = new Services.FacilityService(DbContext);
            for (int row = 1; row < XlWorkbook.Worksheets.Count; row++) {
                DataParser dp = new DataParser(new Facility());
                int column = 0;
                Excel.Worksheet item = XlWorkbook.Worksheets.Item[row];
                dp.Name(item.Cells[row, column++]);
                dp.Licensee(item.Cells[row, column++]);
                dp.Unit(item.Cells[row, column++]);
                dp.LicenseNumber(item.Cells[row, column++]);
                dp.ZipCode(item.Cells[row, column++]);
                dp.City(item.Cells[row, column++]);
                dp.PreviousInspection(item.Cells[row, column++]);
                dp.MostRecentInspection(item.Cells[row, column++]);
                column++; column++; //don't need to parse intervals
                dp.ProposedDate(item.Cells[row, column++]);
                column++; column++; //don't need to parse 17th/18th month deadline
                dp.LicensorList(item.Cells[row, column++]);
                dp.InspectionResult(item.Cells[row, column++]);
                dp.EnforcementNotes(item.Cells[row, column++]);
                facilityService.AddOrUpdateFacility(dp.Facility);
            }

            HelperMethods.DateSelection(facilityService.FetchAll(), MonthlyCalendar);
        }

        /// <summary>
        /// This method searches through the list of facilities displayed for the specific string provided by the user.
        /// </summary>
        private void SearchButton_Click(object sender, RoutedEventArgs e) {
            string facility = SearchText.Text;
            // TODO search for facility based on text content ^ and open it in details page
            TabItemDetails.IsSelected = true;
        }

        /// <summary>
        /// This method initializes the Details tab with Facility object property names.
        /// </summary>
        private void DetailsInit() {
            ListView list = DetailsView;

            string[] labelContent = { "Facility Name", "Name of Licensee", "License Number", "Unit", "City", "ZipCode", "Number Of Beds", "Most Recent Full Inspection",
                    "Previous Year Full Inspection", "Two Year Full Inspection", "Inspection Result", "Dates Of SOD", "Enforcement Notes", "Failed Follow Up", "Complaints",
                    "Proposed Date", "Schedule Interval", "Month 15", "Month 18", "Number Of Licensors", "Sample Size", "Special Info" };

            for (int x = 0; x < labelContent.Length; x++) {
                StackPanel stack = new StackPanel();
                stack.HorizontalAlignment = HorizontalAlignment.Stretch;
                stack.Orientation = Orientation.Horizontal;
                Label label = CreateLabel(labelContent[x]);
                stack.Children.Add(label);
                label = CreateLabel("", 240, HorizontalAlignment.Right);
                stack.Children.Add(label);

                if (x == 0) {
                    Button editButton = new Button();
                    editButton.Content = "Edit Facility";
                    editButton.Width = 70;
                    stack.Children.Add(editButton);
                }
            
                list.Items.Add(stack);
            }
            
            TabItemDetails.Content = list;
        }

        /// <summary>
        /// This method creates a new label for each item in the details tab.
        /// </summary>
        /// <param name="content">The string content to be displayed.</param>
        /// <param name="width">The width of the label, default is 175px.</param>
        /// <param name="alignRight">The horizontal text alignment, default left.</param>
        private Label CreateLabel(string content, double width = 175, HorizontalAlignment alignRight = HorizontalAlignment.Left) {
            Label label = new Label();
            label.Content = content;
            label.FontSize = 12;
            label.Padding = new Thickness(5);
            label.Width = width;
            label.HorizontalAlignment = alignRight;
            return label;
        }

        /// <summary>
        /// This method gathers data from the selected facility in the ListView and displays it next to its respective property label.
        /// </summary>
        public void OpenDetails() {
            //TODO input specific facility info into the labels, needs to be accessible based on tab switch as well (see below)
        }

        /// <summary>
        /// This event is triggered when you switch tabs in the TabControl.
        /// </summary>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //if (TabItemDetails.IsSelected) {
            //    OpenDetails(); //send specific facility based on what's selected
            //} 
        }

        /// <summary>
        /// This event method opens the CalendarYear page when the 'Whole Year' button is clicked.
        /// </summary>
        private void CalendarYearButton_Click(object sender, RoutedEventArgs e) {
            HelperMethods.OpenCalendar(CalendarYearPage);
        }

        /// <summary>
        /// This click event will update the specified facility's most recent inspection information.
        /// </summary>
        private void SubmitForm_Click(object sender, RoutedEventArgs e) {

        }
    }
}
