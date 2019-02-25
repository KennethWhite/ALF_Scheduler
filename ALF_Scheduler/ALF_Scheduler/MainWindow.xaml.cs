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
using System.Reflection;

namespace ALF_Scheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window //
    {
        List<Facility> items = new List<Facility>();
        bool isDetailsTabBuilt = false;//bool used to make sure the Details tab only gets built once. Otherwise flicking between tabs causes it to rebuild over and over, making it unreadable.

        public MainWindow() {
            InitializeComponent();

            Facility fac1 = new Facility();
            DataParser dp = new DataParser(fac1);
            dp.Name("Lakeland Adult Family Home");
            dp.Licensee("Mendez, Catherine");
            dp.Unit("G");
            dp.LicenseNumber("725103");
            dp.ZipCode("98001");
            dp.City("Algona");
            dp.NumberOfBeds("25");
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
            AddSelectedDates(items);
            //DetailsInit();
        }

        /// <summary>
        /// This method adds each facility's proposed date to the calendar object.
        /// </summary>
        /// <param name="facilities">The list of facilities.</param>
        private void AddSelectedDates(List<Facility> facilities) {
            foreach (Facility facility in facilities) { 
                MonthlyCalendar.SelectedDates.Add(DateTime.Parse(facility.ProposedDate));
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) {
            string facilityToSearch = SearchText.Text;
            int facilityIndexNumber = searchBar(facilityToSearch);
            TabItemDetails.IsSelected = true;
            if(facilityIndexNumber != -1)//if the search results found something, then open deets tab.
                OpenDetails(items[facilityIndexNumber]);

        }

        //This method will accept a string to be compared to all the other facility objs in the total list. It will return the first index it can find that matches with the searched for text. If not found, it will return -1.
        private int searchBar(string toSearch)
        {          
           for(int j = 0; j < items.Count; j++)
            {
                string[] facProperties = items[j].returnFacility(items[j]);
                for(int i = 0; i < facProperties.Length; i++)
                {
                    if (facProperties[i] != null)
                    {
                        string toCheck = facProperties[i].ToUpper();
                        if (toCheck.CompareTo(toSearch.ToUpper()) == 0)
                            return j;//to get here means the search results found something.
                    }                   
                        
                }
            }
            return -1;//to get here means the search results found nothing.
        }
        // TODO ability to scroll
        private void DetailsInit()
        {
            if(isDetailsTabBuilt != true)
            {
                Grid grid = DetailsGrid;
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                string[] labelContent = { "Facility Name", "Name of Licensee", "License Number", "Unit", "City", "ZipCode", "Number Of Beds", "Most Recent Full Inspection",
                    "One Year Full Inspection", "Two Year Full Inspection", "Inspection Result", "Dates Of SOD", "Enforcement Notes", "Failed Follow Up", "Complaints",
                    "Proposed Date", "Schedule Interval", "Month 15", "Month 18", "Number Of Licensors", "Sample Size", "Special Info" };

                for (int x = 0; x < labelContent.Length; x++)
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
                isDetailsTabBuilt = true;
            }
            
        }

        private void OpenDetails(Facility facToShow)
        {
            string[] facProperties = facToShow.returnFacility(facToShow);
            for (int row = 0; row < DetailsGrid.RowDefinitions.Count; row++) {
                TextBox textbox = new TextBox();
                textbox.Height = 15;
                textbox.IsReadOnly = true;
                
                textbox.Text = facProperties[row];
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
                DetailsInit();
                //OpenDetails();
            }
        }

        private void CalendarYearButton_Click(object sender, RoutedEventArgs e) {
            CalendarYear calendarYearPage = new CalendarYear();
            this.Content = calendarYearPage;
        }

        private void FacilityList_MouseDoubleClick(object sender, MouseButtonEventArgs e)//Can now double click one of the Facility ListView Items, to be displayed in Details tab.
        {
            if(FacilityList.HasItems)//makes sure somethings in the list first
            {
                Facility selectedFac = (Facility)FacilityList.SelectedItem;
                TabItemDetails.IsSelected = true;
                OpenDetails(selectedFac);
            }
        }
    }
}
