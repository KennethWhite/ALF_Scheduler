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
    /// Interaction logic for CalendarYear.xaml
    /// </summary>
    public partial class CalendarYear : Page {
        public CalendarYear() {
            InitializeComponent();
            CreateCalendars();
        }

        public void CreateCalendars() {
            Calendar calendar;
            string month;
            int year = DateTime.Now.Year;
            for (int x = 1; x <= 12; x++) {
                if (x < 10) month = String.Format("0{0}", x);
                else month = x.ToString();
                calendar = new Calendar();
                calendar.DisplayDateStart = DateTime.Parse(String.Format("{0}/01/{1}", month, year));
                calendar.DisplayDateEnd = DateTime.Parse(String.Format("{0}/{1}/{2}", month, DateTime.DaysInMonth(year, x), year));
                calendar.IsTodayHighlighted = true;
                calendar.Margin = new Thickness(10);
                calendar.ClipToBounds = true;
                calendar.SelectionMode = CalendarSelectionMode.MultipleRange;

                wrapPanel.Children.Add(calendar);
            }
        }
    }
}
