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
            calendarYearLabel.Content += DateTime.Now.Year.ToString();
            CreateCalendars();
        }

        public void CreateCalendars() {
            Calendar calendar;
            string month;
            int year = DateTime.Now.Year;

            int row = 1;
            int column = 0;
            for (int x = 1; x <= 12; x++) {
                if (x < 10) month = string.Format("0{0}", x);
                else month = x.ToString();

                calendar = new Calendar();
                calendar.DisplayDateStart = DateTime.Parse(string.Format("{0}/01/{1}", month, year));
                calendar.DisplayDateEnd = DateTime.Parse(string.Format("{0}/{1}/{2}", month, DateTime.DaysInMonth(year, x), year));
                calendar.IsTodayHighlighted = true;
                calendar.Margin = new Thickness(10);
                calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
                calendar.CalendarButtonStyle = (Style)(Resources["CalendarButtonStyle"]);

                //TODO add blackout and selected dates
                Grid.SetRow(calendar, row);
                Grid.SetColumn(calendar, column);
                calendarGrid.Children.Add(calendar);

                if (x == 4 || x == 8) {
                    row++;
                    column = 0;
                } else column++;
            }
        }

        private void CalendarPage_SizeChanged(object sender, SizeChangedEventArgs e) {
            //myCanvas.Width = e.NewSize.Width;
            //myCanvas.Height = e.NewSize.Height;

            //double xChange = 1, yChange = 1;

            //if (e.PreviousSize.Width != 0)
            //    xChange = (e.NewSize.Width / e.PreviousSize.Width);

            //if (e.PreviousSize.Height != 0)
            //    yChange = (e.NewSize.Height / e.PreviousSize.Height);

            //foreach (FrameworkElement fe in calendarGrid.Children) {
            //    //because I didn't want to resize the grid I'm having inside the canvas in this particular instance. (doing that from xaml)           
            //    if (fe is Grid == false) {
            //        fe.Height = fe.ActualHeight * yChange;
            //        fe.Width = fe.ActualWidth * xChange;

            //        Canvas.SetTop(fe, Canvas.GetTop(fe) * yChange);
            //        Canvas.SetLeft(fe, Canvas.GetLeft(fe) * xChange);

                    
            //    }
            //}
        }
    }
}
