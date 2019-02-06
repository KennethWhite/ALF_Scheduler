using Excel_Import_Export;
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

        public SchedulerHome() { //string path) {
            InitializeComponent();

            // TODO import excel file, parse into facility object and db, bind db to grid
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

            List<Facility> items = new List<Facility>();
            DataParser dp = new DataParser(new Facility());
            dp.Name("Lakeland Adult Family Home");
            dp.Licensee("Mendez, Catherine");
            dp.Unit("G");
            dp.LicenseNumber("725103");
            dp.ZipCode("98001");
            dp.City("Algona");
            dp.PreviousInspection("08/14/2015");
            dp.MostRecentInspection("02/23/2018");
            dp.NumberOfLicensors("2");
            dp.InspectionResult("NO");
            dp.ProposedDate("03/22/2020");
            items.Add(dp.Facility);
            
            dp = new DataParser(new Facility());
            dp.Name("Facility 2");
            dp.MostRecentInspection("08/25/2017");
            dp.InspectionResult("yes");
            dp.ProposedDate("03/02/2019");
            items.Add(dp.Facility);

            dp = new DataParser(new Facility());
            dp.Name("Facility 3");
            dp.MostRecentInspection("12/05/2017");
            dp.InspectionResult("enf");
            dp.ProposedDate("05/14/2019");
            items.Add(dp.Facility);

            dp = new DataParser(new Facility());
            dp.Name("Facility 4");
            items.Add(dp.Facility);

            dp = new DataParser(new Facility());
            dp.Name("Facility 5");
            items.Add(dp.Facility);

            dp = new DataParser(new Facility());
            dp.Name("Facility 6");
            items.Add(dp.Facility);

            dp = new DataParser(new Facility());
            dp.Name("Facility 7");
            items.Add(dp.Facility);

            dp = new DataParser(new Facility());
            dp.Name("Facility 8");
            items.Add(dp.Facility);

            FacilityList.ItemsSource = items;
            AddSelectedDates(items);

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

        private Label CreateLabel(string content, double width = 175, HorizontalAlignment alignRight = HorizontalAlignment.Left) {
            Label label = new Label();
            label.Content = content;
            label.FontSize = 12;
            label.Padding = new Thickness(5);
            label.Width = width;
            label.HorizontalAlignment = alignRight;
            return label;
        }

        private void OpenDetails() {
            //TODO input specific facility info into the labels
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //if (TabItemFacilities.IsSelected) {
            //    Console.WriteLine("tab control selection changed" + TabItemFacilities);
            //} else if (TabItemDetails.IsSelected) {
            //    Console.WriteLine("tab control selection changed" + TabItemDetails);
            //    OpenDetails();
            //}
        }

        private void CalendarYearButton_Click(object sender, RoutedEventArgs e) {
            CalendarYear calendarYearPage = new CalendarYear();
            this.NavigationService.Navigate(calendarYearPage);
        }
    }
}
