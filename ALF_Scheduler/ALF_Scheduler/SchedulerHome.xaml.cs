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

namespace ALF_Scheduler {
    /// <summary>
    /// Interaction logic for SchedulerHome.xaml
    /// </summary>
    public partial class SchedulerHome : Page {

        private ApplicationDbContext DbContext { get; }
        private Excel.Workbook XlWorkbook;
        private Excel.Application XlApp;

        public SchedulerHome(string path) {
            InitializeComponent();

            // TODO import excel file, parse into facility object and db, bind db to grid
            if (ExcelImporterExporter.LoadExcelFromFile(path, out XlApp, out XlWorkbook)) {
                /*
                  System.InvalidOperationException
                  HResult=0x80131509
                  Message=No database provider has been configured for this DbContext. A provider can be configured by overriding the DbContext.OnConfiguring method or by using AddDbContext 
                  on the application service provider. If AddDbContext is used, then also ensure that your DbContext type accepts a DbContextOptions<TContext> object in its constructor and 
                  passes it to the base constructor for DbContext.
                  Source=Microsoft.EntityFrameworkCore
                */
                DbContext = new ApplicationDbContext(new Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext>());
                CreateFacilities();
            }



            List<Facility> items = new List<Facility>();

            // TODO connect Facility DB for display
            
            DetailsInit();
        }

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

            AddSelectedDates(facilityService.FetchAll());
        }

        /// <summary>
        /// This method adds each facility's proposed date to the calendar object.
        /// </summary>
        /// <param name="facilities">The list of facilities.</param>
        private void AddSelectedDates(List<Facility> facilities) {
            foreach (Facility facility in facilities) {
                MonthlyCalendar.SelectedDates.Add(DateTime.Parse(facility.ProposedDate.ToString()));
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) {
            string facility = SearchText.Text;
            // TODO search for facility based on text content ^ and open it in details page
            TabItemDetails.IsSelected = true;
        }

        // TODO formatting and add ability to scroll
        private void DetailsInit() {
            Grid grid = DetailsGrid;
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            string[] labelContent = { "Facility Name", "Name of Licensee", "License Number", "Unit", "City", "ZipCode", "Number Of Beds", "Most Recent Full Inspection",
                    "One Year Full Inspection", "Two Year Full Inspection", "Inspection Result", "Dates Of SOD", "Enforcement Notes", "Failed Follow Up", "Complaints",
                    "Proposed Date", "Schedule Interval", "Month 15", "Month 18", "Number Of Licensors", "Sample Size", "Special Info" };

            for (int x = 0; x < labelContent.Length; x++) {
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
        }

        private void OpenDetails() {
            for (int row = 0; row < DetailsGrid.RowDefinitions.Count; row++) {
                TextBox textbox = new TextBox();
                textbox.Height = 15;
                textbox.IsReadOnly = true;
                // TODO connect to facility
                //textbox.Text = fac1.Name;
                Grid.SetColumn(textbox, 1);
                Grid.SetRow(textbox, row);
                DetailsGrid.Children.Add(textbox);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (TabItemFacilities.IsSelected) {
                Console.WriteLine("tab control selection changed" + TabItemFacilities);
            } else if (TabItemDetails.IsSelected) {
                Console.WriteLine("tab control selection changed" + TabItemDetails);
                OpenDetails();
            }
        }

        private void CalendarYearButton_Click(object sender, RoutedEventArgs e) {
            CalendarYear calendarYearPage = new CalendarYear();
            this.NavigationService.Navigate(calendarYearPage);
        }
    }
}
