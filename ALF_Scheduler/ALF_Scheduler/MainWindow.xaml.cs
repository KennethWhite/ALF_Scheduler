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
            DataParser dp = new DataParser();
            dp.Name("Facility 1");
            dp.MostRecentInspection("01/12/2019");
            dp.ProposedDate("03/22/2020");
            Facility fac1 = dp.GetFacility();
            items.Add(fac1);
            FacilityList.ItemsSource = items;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) {
            string facility = SearchText.Text;
            // TODO search for facility based on text content ^
        }
    }
}
