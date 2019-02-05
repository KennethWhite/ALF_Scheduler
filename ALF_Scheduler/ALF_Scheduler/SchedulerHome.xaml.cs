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

namespace ALF_Scheduler {
    /// <summary>
    /// Interaction logic for SchedulerHome.xaml
    /// </summary>
    public partial class SchedulerHome : Page {
        public SchedulerHome() {
            InitializeComponent();

            // TODO import excel file, parse into facility object and db, bind db to grid
            // if load true then
            //Excel.Application app = new Excel.Application();
            //Excel.Workbook workbook = new Excel.Workbook();
            //if (ExcelImporterExporter.LoadExcelFromFile(
            //    "C:\\Users\\mical\\Source\\Repos\\KennethWhite\\ALF_Scheduler\\ALF_Scheduler\\TestData.xlsx",
            //    out app, out workbook)) {
            //    // parse data into facility objects

            //}



            List<Facility> items = new List<Facility>();

            // TODO connect Facility DB for display
            
            DetailsInit();
        }

        private void CreateFacilities() {
            /*
            fac1 = new Facility();
            DataParser dp = new DataParser(fac1);
            dp.Name("Lakeland Adult Family Home");
            dp.Licensee("Mendez, Catherine");
            dp.Unit("G");
            dp.LicenseNumber("725103");
            dp.ZipCode("98001");
            dp.City("Algona");
            dp.OneYearInspection("08/14/2015");
            dp.MostRecentInspection("02/23/2018");
            dp.NumberOfLicensors("2");
            dp.InspectionResult("NO");
            dp.ProposedDate("03/22/2020");
            items.Add(dp.GetFacility());
            
            Facility fac2 = new Facility();
            dp = new DataParser(fac2);
            dp.Name("Facility 2");
            dp.MostRecentInspection("08/25/2017");
            dp.InspectionResult("yes");
            dp.ProposedDate("03/02/2019");
            items.Add(dp.GetFacility());

            Facility fac3 = new Facility();
            dp = new DataParser(fac3);
            dp.Name("Facility 3");
            dp.MostRecentInspection("12/05/2017");
            dp.InspectionResult("enf");
            dp.ProposedDate("05/14/2019");
            items.Add(dp.GetFacility());

            FacilityList.ItemsSource = items;
            AddSelectedDates(items);*/
        }

        /// <summary>
        /// This method adds each facility's proposed date to the calendar object.
        /// </summary>
        /// <param name="facilities">The list of facilities.</param>
        private void AddSelectedDates(List<Facility> facilities) {
            //foreach (Facility facility in facilities) { 
            //    MonthlyCalendar.SelectedDates.Add(DateTime.Parse(facility.ProposedDate));
            //}
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
