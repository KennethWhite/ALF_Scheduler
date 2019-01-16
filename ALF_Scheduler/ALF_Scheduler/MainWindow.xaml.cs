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

namespace ALF_Scheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
            List<Facility> items = new List<Facility>();

            //TODO connect Facility DB for display
            Facility fac1 = new Facility();
            DataParser dp = new DataParser(fac1);
            dp.Name("Facility 1");
            dp.MostRecentInspection("01/12/2019");
            dp.ProposedDate("03/22/2020");
            items.Add(dp.GetFacility());
            
            Facility fac2 = new Facility();
            dp = new DataParser(fac2);
            dp.Name("Facility 2");
            dp.MostRecentInspection("08/25/2017");
            dp.ProposedDate("03/02/2019");
            items.Add(dp.GetFacility());

            Facility fac3 = new Facility();
            dp = new DataParser(fac3);
            dp.Name("Facility 3");
            dp.MostRecentInspection("12/05/2017");
            dp.ProposedDate("05/14/2019");
            items.Add(dp.GetFacility());

            FacilityList.ItemsSource = items;
            AddSelectedDates(items);
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
            string facility = SearchText.Text;
            // TODO search for facility based on text content ^
        }
    }
}
