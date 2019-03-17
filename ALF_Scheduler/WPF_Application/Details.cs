using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using ALF_Scheduler.Models;
using System.Windows.Media;

namespace WPF_Application
{
    class Details
    {
        private static Brush red = new SolidColorBrush(Color.FromArgb(90, 255, 0, 0));
        private static Brush green = new SolidColorBrush(Color.FromArgb(90, 0, 255, 0));

        private StackPanel inspecInfo;
        private StackPanel buttonArea;
        private StackPanel detailsViewer;
        private StackPanel noFacSel;

        private TextBox facName;
        private TextBox licName;
        private TextBox licNum;
        private TextBox unit;
        private TextBox city;
        private TextBox zip;
        private TextBox bedCount;
        private TextBox specInfo;
        private TextBox propNext;
        private TextBox mostRecentInspec;
        private TextBox prevInspecOne;
        private TextBox prevInspecTwo;
        private TextBox licensors;
        private TextBox complaints;
        private TextBox inspecResult;
        private TextBox enfNotes;

        private Label avgDatesLabel;
        private Label avgDatesVal;

        private List<TextBox> allText;

        private bool InspectionInfoVisible { get { return inspecInfo.Visibility.Equals(Visibility.Visible); } }
        private bool DetailsViewerVisible { get { return detailsViewer.Visibility.Equals(Visibility.Visible); } }

        public Details(SchedulerHome sh)
        {
            inspecInfo = sh.sp_InspecInfo;
            buttonArea = sh.sp_ButtonArea;
            detailsViewer = sh.sp_DetailsViewer;
            noFacSel = sh.sp_NoFacSel;

            avgDatesLabel = sh.MonthAvgLabel;
            avgDatesVal = sh.MonthAvgVal;

            facName = sh.tb_FacName;
            licName = sh.tb_NameLicensee;
            licNum = sh.tb_LicNum;
            unit = sh.tb_Unit;
            city = sh.tb_City;
            zip = sh.tb_Zip;
            bedCount = sh.tb_BedCount;
            specInfo = sh.tb_SpecInfo;
            propNext = sh.tb_ProposedNext;
            mostRecentInspec = sh.tb_MostRecentInspec;
            prevInspecOne = sh.tb_PrevInspecOne;
            prevInspecTwo = sh.tb_PrevInspecTwo;
            licensors = sh.tb_Licensors;
            complaints = sh.tb_Complaints;
            inspecResult = sh.tb_InspecResult;
            enfNotes = sh.tb_EnfNotes;

            allText = new List<TextBox>();
            allText.Add(facName);
            allText.Add(licName);
            allText.Add(licNum);
            allText.Add(unit);
            allText.Add(city);
            allText.Add(zip);
            allText.Add(bedCount);
            allText.Add(specInfo);
            allText.Add(propNext);
            allText.Add(mostRecentInspec);
            allText.Add(prevInspecOne);
            allText.Add(prevInspecTwo);
            allText.Add(licensors);
            allText.Add(complaints);
            allText.Add(inspecResult);
            allText.Add(enfNotes);
        }

        /// <summary>
        /// Collapses information relating to inspections
        /// </summary>
        public void InspectionInfo_Collapse()
        {
            inspecInfo.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Expands information relating to inspections
        /// </summary>
        public void InspectionInfo_Expand()
        {
            inspecInfo.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Toggles collapse/expand for inspection information
        /// </summary>
        public void InspectionInfo_Toggle()
        {
            if (!InspectionInfoVisible)
                inspecInfo.Visibility = Visibility.Visible;
            else
                inspecInfo.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Collapses detail labels and text boxes, and expands "No facility selected" label
        /// </summary>
        public void DetailsViewer_Collapse()
        {
            detailsViewer.Visibility = Visibility.Collapsed;
            noFacSel.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Expands detail labels and text boxes, and collapses "No facility selected" label
        /// </summary>
        public void DetailsViewer_Expand()
        {
            detailsViewer.Visibility = Visibility.Visible;
            noFacSel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Sets Details TextBoxes to empty.
        /// </summary>
        public void ClearDetails()
        {
            foreach (var tb in allText)
            {
                tb.Text = "";
            }
        }

        /// <summary>
        /// Updates Details textboxes and layout for given Facility. If Facility has no inspections, no inspection data will be shown.
        /// </summary>
        /// <param name="fac">Facility to load data from</param>
        public void DisplayFacility(Facility fac)
        {
            ClearDetails();

            if (!DetailsViewerVisible)
                DetailsViewer_Expand();
            if (fac == null)
            {
                DetailsViewer_Collapse();
                return;
            }

            facName.Text = fac.FacilityName;
            licName.Text = fac.NameOfLicensee;
            licNum.Text = fac.LicenseNumber;
            unit.Text = fac.Unit;
            city.Text = fac.City;
            zip.Text = fac.ZipCode;
            bedCount.Text = fac.NumberOfBeds.ToString();
            specInfo.Text = fac.SpecialInfo;
            propNext.Text = fac.ProposedDateString;
            
            if (fac.HasInspection())
            {
                InspectionInfo_Expand();
                inspecResult.Text = fac.InspectionResult;
                mostRecentInspec.Text = Facility.GetDateString(fac.MostRecentFullInspection.InspectionDate);
                prevInspecOne.Text = Facility.GetDateString(fac.PreviousFullInspection.InspectionDate);
                prevInspecTwo.Text = Facility.GetDateString(fac.TwoYearFullInspection.InspectionDate);
                licensors.Text = fac.Licensors;
                complaints.Text = fac.Complaints;
                enfNotes.Text = fac.EnforcementNotes;
            }
            else
            {
                InspectionInfo_Collapse();
            }
        }

        /// <summary>
        /// Shows submit and revert buttons.
        /// </summary>
        public void ShowButtons()
        {
            buttonArea.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Hides submit and revert buttons.
        /// </summary>
        public void HideButtons()
        {
            buttonArea.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Saves changes made to given Facility from Details information.
        /// </summary>
        /// <param name="fac">Facility to update.</param>
        public void SubmitChanges(Facility fac)
        {
            fac.FacilityName = facName.Text;
            var ara = licName.Text.Split(',');
            foreach(var txt in ara)
            {
                txt.Trim();
            }
            if (ara.Any())
            {
                if(ara.Count() >= 2)
                {
                    fac.LicenseeLastName = ara.ElementAt(0);
                    fac.LicenseeFirstName = ara.ElementAt(1);
                }
                else
                {
                    fac.LicenseeFirstName = ara.ElementAt(0);
                }
            }
            else
            {
                fac.LicenseeFirstName = "";
                fac.LicenseeLastName = "";
            }
            fac.LicenseNumber = licNum.Text;
            fac.Unit = unit.Text;
            fac.City = city.Text;
            fac.ZipCode = zip.Text;
            try
            {
                fac.NumberOfBeds = int.Parse(bedCount.Text);
            }
            catch
            {
                ShowWarn("Could not parse number of beds.");
                bedCount.Background = red;
                return;
            }
            fac.SpecialInfo = specInfo.Text;
            try
            {
                DateTime newDate = new DateTime();
                if (!string.IsNullOrWhiteSpace(propNext.Text)) newDate = DateTime.Parse(propNext.Text);

                if (!fac.ProposedDate.Day.Equals(newDate.Day))
                {
                    avgDatesLabel.Visibility = Visibility.Visible;
                    avgDatesVal.Visibility = Visibility.Visible;
                    avgDatesVal.Content = ScheduleGeneration.ScheduleGeneration.GetGlobalAverage(App.Facilities);
                    fac.ProposedDate = newDate;
                }
            }
            catch
            {
                ShowWarn("Could not parse Next Proposed Date.");
                propNext.Background = red;
                return;
            }

            if (fac.HasInspection())
            {
                fac.Licensors = licensors.Text;
                fac.Complaints = complaints.Text;
                fac.EnforcementNotes = enfNotes.Text;
            }

            HideButtons();
            foreach (var txt in allText)
            {
                txt.Background = null;
            }

            mostRecentInspec.Background = Brushes.LightGray;
            inspecResult.Background = Brushes.LightGray;
            prevInspecOne.Background = Brushes.LightGray;
            prevInspecTwo.Background = Brushes.LightGray;
        }
        /// <summary>
        /// Replaces data in Details tab with given Facility. Clears TextBox background colors.
        /// </summary>
        /// <param name="fac">Facility to load in.</param>
        public void RevertChanges(Facility fac)
        {
            HideButtons();
            DisplayFacility(fac);
            foreach (var txt in allText)
            {
                txt.Background = null;
            }
        }

        /// <summary>
        /// Displays simple warning dialog.
        /// </summary>
        /// <param name="msg">Message to present in dialog.</param>
        /// <returns>MessageBoxResult</returns>
        private MessageBoxResult ShowWarn(string msg)
        {
            return MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// When TextBox Text is changes, sets background color to an opaque green, and enables Submit and Revert buttons.
        /// </summary>
        /// <param name="sender">TextBox that triggered event.</param>
        public void TextChanged(TextBox sender)
        {
            if (!sender.IsFocused)
                return;
            sender.Background = green;
            ShowButtons();
        }

    }
}
