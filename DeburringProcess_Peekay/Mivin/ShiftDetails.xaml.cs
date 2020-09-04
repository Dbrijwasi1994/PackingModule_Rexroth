using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using DeburringProcess_Peekay.Models;
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

namespace DeburringProcess_Peekay.Mivin
{
    /// <summary>
    /// Interaction logic for ShiftDetails.xaml
    /// </summary>
    public partial class ShiftDetails : UserControl
    {
        List<ShiftDetailsEntity> gridShiftDetailsData = null;
        public ShiftDetails()
        {
            InitializeComponent();
        }

        private void ShiftDetailsControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadShiftID();
                LoadFromDayComboBox();
                LoadShiftDetailsGrid();
                GetDetailsForShiftId(comboShiftId.Text);
            }
            catch (Exception ex)
            {
                DialogBox dbox = new DialogBox("Error!!", ex.Message, true);
                dbox.ShowDialog();
            }
        }

        private void LoadShiftID()
        {
            List<string> shiftID = new List<string>();
            shiftID.Add("1");
            shiftID.Add("2");
            shiftID.Add("3");
            comboShiftId.ItemsSource = shiftID;
            comboShiftId.SelectedIndex = 0;
        }

        private void LoadFromDayComboBox()
        {
            List<string> fromDay = new List<string>();
            fromDay.Add("Today");
            fromDay.Add("Tomorrow");
            fromDay.Add("Yesterday");
            comboFromDay.ItemsSource = fromDay;
            comboToDay.ItemsSource = fromDay;
        }

        private void LoadShiftDetailsGrid()
        {
            try
            {
                timePickerFromTime.EnableMouseWheelEdit = false;
                timePickerToTime.EnableMouseWheelEdit = false;
                gridShiftDetailsData = new List<ShiftDetailsEntity>();
                dataGridShiftDetails.AutoGenerateColumns = false;
                gridShiftDetailsData = MivinDataBaseAccess.GetAllshiftDetails();
                if (gridShiftDetailsData != null && gridShiftDetailsData.Count > 0)
                {
                    dataGridShiftDetails.ItemsSource = gridShiftDetailsData;
                    dataGridShiftDetails.IsReadOnly = true;
                    SetValuesForDataColumn(gridShiftDetailsData);
                }
                else
                {
                    dataGridShiftDetails.ItemsSource = null;
                    DialogBox dbox = new DialogBox("Information!!", "No shift details available.", false);
                    dbox.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                DialogBox dbox = new DialogBox("Error!!", ex.Message, true);
                dbox.ShowDialog();
            }
        }

        private void SetValuesForDataColumn(List<ShiftDetailsEntity> gridShiftDetailsData)
        {
            if (gridShiftDetailsData != null)
            {
                foreach (ShiftDetailsEntity shiftEntity in gridShiftDetailsData)
                {
                    if (shiftEntity.FromDay.Equals("0"))
                    {
                        shiftEntity.FromDay = "Today";
                    }
                    else if (shiftEntity.FromDay.Equals("1"))
                    {
                        shiftEntity.FromDay = "Tomorrow";
                    }
                    else
                    {
                        shiftEntity.FromDay = "Yesterday";
                    }

                    if (shiftEntity.ToDay.Equals("0"))
                    {
                        shiftEntity.ToDay = "Today";
                    }
                    else if (shiftEntity.ToDay.Equals("1"))
                    {
                        shiftEntity.ToDay = "Tomorrow";
                    }
                    else
                    {
                        shiftEntity.ToDay = "Yesterday";
                    }
                }
            }
        }

        private void GetDetailsForShiftId(string currentShiftId)
        {
            var details = MivinDataBaseAccess.GetShiftDetails(currentShiftId);
            if (details != null)
            {
                if (details.FromDay.ToString().Equals("0")) comboFromDay.SelectedIndex = 0;
                else if (details.FromDay.ToString().Equals("1")) comboFromDay.SelectedIndex = 1;
                else if (details.FromDay.ToString().Equals("2")) comboFromDay.SelectedIndex = 2;

                if (details.ToDay.ToString().Equals("0")) comboToDay.SelectedIndex = 0;
                else if (details.ToDay.ToString().Equals("1")) comboToDay.SelectedIndex = 1;
                else if (details.ToDay.ToString().Equals("2")) comboToDay.SelectedIndex = 2;

                timePickerFromTime.DateTime = Convert.ToDateTime(details.FromTime);
                timePickerToTime.DateTime = Convert.ToDateTime(details.ToTime);
                txtShiftName.Text = details.ShiftName.ToString();
            }
            else
            {
                timePickerFromTime.DateTime = DateTime.Now;
                timePickerToTime.DateTime = DateTime.Now;
                txtShiftName.Text = string.Empty;
                comboFromDay.SelectedIndex = 0;
                comboToDay.SelectedIndex = 0;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (comboFromDay.Text == "Yesterday")
                {
                    if (timePickerFromTime.DateTime.Value >= timePickerFromTime.DateTime.Value)
                        timePickerFromTime.DateTime = timePickerFromTime.DateTime.Value.AddDays(-1);
                }

                if (comboToDay.Text == "Tomorrow")
                {
                    if (timePickerToTime.DateTime.Value <= timePickerFromTime.DateTime.Value)
                        timePickerToTime.DateTime = timePickerToTime.DateTime.Value.AddDays(1);
                }
                if (ValidateFormFields())
                {
                    bool isValidEntry = MivinDataBaseAccess.CheckShiftId(comboShiftId.Text);
                    if (isValidEntry)
                    {
                        MivinDataBaseAccess.CheckForTheTimeEntry(timePickerFromTime.DateTime.ToString(), timePickerToTime.DateTime.ToString());
                        MivinDataBaseAccess.UpdateShiftDetails(comboShiftId.Text, txtShiftName.Text, comboFromDay.Text, comboToDay.Text, timePickerFromTime.DateTime.Value, timePickerToTime.DateTime.Value);
                        ShiftDetailsControl_Loaded(null, null);
                        DialogBox dbox = new DialogBox("Saved Successfully", "Details Added Successfully.", false);
                        dbox.Show();
                    }
                    else
                    {
                        if (!MivinDataBaseAccess.CheckForShiftName(txtShiftName.Text, comboShiftId.Text))
                        {
                            MivinDataBaseAccess.InsertShiftDetails(comboShiftId.Text, txtShiftName.Text, comboFromDay.Text, comboToDay.Text, timePickerFromTime.DateTime.Value, timePickerToTime.DateTime.Value);
                            ShiftDetailsControl_Loaded(null, null);
                            DialogBox dbox = new DialogBox("Saved Successfully", "Details Added Successfully.", false);
                            dbox.Show();
                        }
                        else
                        {
                            DialogBox dbox = new DialogBox("Information", "Shift Name Already Exsits,.!! \n Please Enter Different Shift Name.", false);
                            dbox.Show();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DialogBox dbox = new DialogBox("Error!!", ex.Message, true);
                dbox.ShowDialog();
            }
        }

        private bool ValidateFormFields()
        {
            TimeSpan time1 = (TimeSpan)timePickerFromTime.DateTime.Value.TimeOfDay;
            TimeSpan time2 = (TimeSpan)timePickerToTime.DateTime.Value.TimeOfDay;

            if (string.IsNullOrEmpty(comboShiftId.Text))
            {
                DialogBox dbox = new DialogBox("Important Note", "Please Enter Valid Shift ID.", false);
                dbox.ShowDialog();
                return false;
            }

            if (string.IsNullOrEmpty(txtShiftName.Text))
            {
                DialogBox dbox = new DialogBox("Important Note", "Shift Name cannot be blank.\n Please enter Shift Name.", false);
                dbox.ShowDialog();
                return false;
            }

            if (((comboFromDay.Text).Equals("Tomorrow") && (comboToDay.Text).Equals("Today")) || ((comboFromDay.Text).Equals("Today") && (comboToDay.Text).Equals("Yesterday")) || ((comboFromDay.Text).Equals("Yesterday") && (comboToDay.Text).Equals("Tomorrow")) || ((comboFromDay.Text).Equals("Tomorrow") && (comboToDay.Text).Equals("Yesterday")) || ((comboFromDay.Text).Equals("Tomorrow") && (comboToDay.Text).Equals("Today")))
            {
                DialogBox dbox = new DialogBox("Important Note", "Please enter Valid Days.", false);
                dbox.Show();
                return false;
            }

            if (((comboFromDay.Text).Equals("Today") && (comboToDay.Text).Equals("Today")) && (time1 > time2))
            {
                DialogBox dbox = new DialogBox("Important Note", "Please enter Valid Timings. From Time Cannot Be Greater than To Time", false);
                dbox.Show();
                return false;
            }

            if ((timePickerFromTime.DateTime.Value.Equals(timePickerToTime.DateTime.Value)))
            {
                DialogBox dbox = new DialogBox("Important Note", "Please enter Valid Timings. To Time Cannot Be  Equal to  the From Time", false);
                dbox.Show();
                return false;
            }
            if (timePickerToTime.DateTime.Value < timePickerFromTime.DateTime.Value)
            {
                DialogBox dbox = new DialogBox("Important Note", "Please enter Valid Timings. From Time Cannot Be  Less Than the To Time", false);
                dbox.Show();
                return false;
            }

            if (((comboFromDay.Text).Equals("Today") && (comboToDay.Text).Equals("Tomorrow")))
            {
                if (time2 > time1)
                {
                    DialogBox dbox = new DialogBox("Important Note", "Please enter Valid Timings. Time Is Greater than 24 hrs. Please Check the End time.", false);
                    dbox.Show();
                    return false;
                }
            }

            if (((comboFromDay.Text).Equals("Today") && (comboToDay.Text).Equals("Today")) && (time1 == time2) || ((comboFromDay.Text).Equals("Tomorrow") && (comboToDay.Text).Equals("Tomorrow")) && (time1 == time2) || ((comboFromDay.Text).Equals("Yesterday") && (comboToDay.Text).Equals("Yesterday")) && (time1 == time2))
            {
                DialogBox dbox = new DialogBox("Error Message", "Please enter Valid Timings. From Time Cannot Be Equal To the To Time", false);
                dbox.Show();
                return false;
            }
            return true;
        }

        private void comboShiftId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox comboShiftID = sender as ComboBox;
                string selectedShiftId = comboShiftID.SelectedItem.ToString();
                ResetAllFields();
                GetDetailsForShiftId(selectedShiftId);
            }
            catch (Exception ex)
            {
                DialogBox dbox = new DialogBox("Error!!", ex.Message, true);
                dbox.ShowDialog();
            }
        }

        private void dataGridShiftDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid shiftGrid = sender as DataGrid;
                var shiftRow = shiftGrid.SelectedItem as ShiftDetailsEntity;
                comboShiftId.SelectedItem = shiftRow.ShiftID;
                comboFromDay.SelectedItem = shiftRow.FromDay;
                comboToDay.SelectedItem = shiftRow.ToDay;
                txtShiftName.Text = shiftRow.ShiftName;
                timePickerFromTime.DateTime = Convert.ToDateTime(shiftRow.FromTime);
                timePickerToTime.DateTime = Convert.ToDateTime(shiftRow.ToTime);
            }
            catch (Exception ex)
            {
                DialogBox dbox = new DialogBox("Error!!", ex.Message, true);
                dbox.ShowDialog();
            }
        }

        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridShiftDetails.ItemsSource != null)
            {
                DialogBoxYesNo res = new DialogBoxYesNo("Warning...", "This will remove all of your shift details. Click OK to Proceed.");
                res.ShowDialog();
                if (Utility.YesNoAnswer == true)
                {
                    MivinDataBaseAccess.RemoveAllShiftdata();
                    ShiftDetailsControl_Loaded(null, null);
                    ResetAllFields();
                }
            }
            else
            {
                MessageBox.Show("No Records To Delete.\n ", "Important", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetAllFields()
        {
            txtShiftName.Text = string.Empty;
            comboFromDay.SelectedIndex = 0;
            comboToDay.SelectedIndex = 0;
        }

        private void timePickerFromTime_DateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //DateTime dtToday = DateTime.Now;
            //DateTime dtTomorrow = DateTime.Now.AddDays(1);
            //DateTime dtYesterday = DateTime.Now.AddDays(-1);
            //if (comboFromDay.Text.Equals("Today"))
            //{
            //    timePickerFromTime.DateTime = timePickerFromTime.DateTime.Value.AddDays((dtToday - timePickerFromTime.DateTime.Value).Days);
            //}
        }
    }
}
