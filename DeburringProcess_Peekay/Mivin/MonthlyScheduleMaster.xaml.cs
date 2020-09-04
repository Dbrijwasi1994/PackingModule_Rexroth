using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using Microsoft.Win32;
using OfficeOpenXml;
using PackingModule_Rexroth.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
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

namespace PackingModule_Rexroth.Mivin
{
    /// <summary>
    /// Interaction logic for MonthlyScheduleMaster.xaml
    /// </summary>
    public partial class MonthlyScheduleMaster : UserControl
    {
        string FileName = string.Empty;
        ObservableCollection<MonthlyScheduleMasterEntity> scheduleMasterDataList = null;
        Dictionary<int, string> DaysList = new Dictionary<int, string>();
        List<string> PackagingTypeList = new List<string>();
        public MonthlyScheduleMaster()
        {
            InitializeComponent();
        }

        private void MonthlyScheduleMaster_Control_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Utility.CurrentEmployeeRole.Contains("Admin") || Utility.CurrentEmployeeRole.Equals("SchedulePlanner"))
                {
                    ScheduleMasterImportGrid.IsReadOnly = false;
                    btnDelete.Visibility = Visibility.Visible;
                    btnNew.Visibility = Visibility.Visible;
                    btnUpdate.Visibility = Visibility.Visible;
                }
                else
                {
                    ScheduleMasterImportGrid.IsReadOnly = true;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnNew.Visibility = Visibility.Collapsed;
                    btnUpdate.Visibility = Visibility.Collapsed;
                    txtFileName.Visibility = Visibility.Collapsed;
                    btnOpen.Visibility = Visibility.Collapsed;
                    btnImport.Visibility = Visibility.Collapsed;
                }
                var yearList = Enumerable.Range(DateTime.Now.AddYears(-3).Year, 4).ToList();
                if (yearList != null && yearList.Count > 0)
                {
                    cmbYear.ItemsSource = yearList;
                    cmbYear.SelectedItem = DateTime.Now.Year;
                }
                BindMonths();
                DaysList = Utility.GetDaysOfMonth();
                PackagingTypeList = MivinDataBaseAccess.GetPackagingTypeData();
                if (cmbYear.SelectedValue != null && cmbMonth.SelectedValue != null)
                {
                    DateTime SelectedDate = Convert.ToDateTime("01-" + cmbMonth.SelectedValue.ToString() + "-" + cmbYear.SelectedValue.ToString());
                    BindScheduleMasterData(SelectedDate);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BindMonths()
        {
            List<MonthEntity> MonthList = new List<MonthEntity>();
            DateTime Month = DateTime.Now;
            try
            {
                Month = Convert.ToDateTime("01-01-" + cmbYear.SelectedValue);
                for (int i = 0; i <= 12; i++)
                {
                    MonthEntity Entity = new MonthEntity();
                    Entity.MonthName = Month.ToString("MMM");
                    Entity.MonthVal = Month.Month;
                    MonthList.Add(Entity);
                    Month = Month.AddMonths(1);
                }
                cmbMonth.DisplayMemberPath = "MonthName";
                cmbMonth.SelectedValuePath = "MonthVal";
                cmbMonth.ItemsSource = MonthList;
                cmbMonth.SelectedIndex = DateTime.Now.Month - 1;
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog("Binding Month Combobox");
                Logger.WriteErrorLog(ex.ToString());
            }
        }

        private void BindScheduleMasterData(DateTime SelectedDate)
        {
            scheduleMasterDataList = new ObservableCollection<MonthlyScheduleMasterEntity>();
            try
            {
                int week = 0;
                scheduleMasterDataList = MivinDataBaseAccess.GetMonthlyScheduleMasterData(SelectedDate);
                week = Utility.GetWeekNumberOfMonth(SelectedDate);

                DateTime CurrentMonth = DateTime.Now;
                CurrentMonth = Convert.ToDateTime("01-" + cmbMonth.SelectedValue.ToString() + "-" + cmbYear.SelectedValue.ToString());
                if (ScheduleMasterImportGrid.Columns.Count < 13)
                {
                    for (int i = 1; i <= DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month); i++)
                    {
                        DataGridTextColumn Grid = new DataGridTextColumn();
                        System.Windows.Data.Binding bnd = new System.Windows.Data.Binding(DaysList[CurrentMonth.Day]);
                        bnd.Mode = BindingMode.TwoWay;
                        bnd.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        Grid.Binding = bnd;
                        Grid.Header = i;
                        CurrentMonth = CurrentMonth.AddDays(1);
                        ScheduleMasterImportGrid.Columns.Add(Grid);
                    }
                }
                if (scheduleMasterDataList != null && scheduleMasterDataList.Count > 0)
                {
                    if (!string.IsNullOrEmpty(txtPumpModelSearch.Text))
                        ScheduleMasterImportGrid.ItemsSource = new ObservableCollection<MonthlyScheduleMasterEntity>(scheduleMasterDataList.Where(x => x.PumpPartNumber.Contains(txtPumpModelSearch.Text)));
                    else
                        ScheduleMasterImportGrid.ItemsSource = scheduleMasterDataList;
                    ScheduleMasterImportGrid.Columns[0].IsReadOnly = true;
                    ScheduleMasterImportGrid.Columns[1].IsReadOnly = true;
                    ScheduleMasterImportGrid.Columns[4].IsReadOnly = true;
                }
                else
                {
                    ScheduleMasterImportGrid.ItemsSource = new ObservableCollection<MonthlyScheduleMasterEntity>();
                    DialogBox dlgInfo = new DialogBox("Information!!", "No schedule data available.", false);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (btnNew.Content.Equals("Cancel"))
                    btnNew_Click(null, null);
                if (ScheduleMasterImportGrid.SelectedItems.Count > 0)
                    btnDelete.IsEnabled = true;
                else
                    btnDelete.IsEnabled = false;
            }
        }

        private void ScheduleMasterImportGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ScheduleMasterImportGrid.SelectedItems.Count > 0)
                    btnDelete.IsEnabled = true;
                else
                    btnDelete.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtFileName.Text = openFileDialog.SafeFileName;
                FileName = openFileDialog.FileName;
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                try
                {
                    FileInfo fileinfo = new FileInfo(FileName);
                    bool Updated = false;
                    DialogBoxYesNo dlgConfirm = new DialogBoxYesNo("Confirm Import ?", "Are you sure you want to import schedule master data ?");
                    dlgConfirm.Owner = Window.GetWindow(this);
                    dlgConfirm.ShowDialog();
                    if (Utility.YesNoAnswer)
                    {
                        if (File.Exists(FileName))
                        {
                            if (System.IO.Path.GetExtension(FileName) != ".xlsx")
                            {
                                DialogBox frm = new DialogBox("Error Message", "Unsupported file type. Please choose a .xlsx excel file.", true);
                                frm.Owner = Window.GetWindow(this);
                                frm.ShowDialog();
                                return;
                            }
                            else if (Utility.IsFileLocked(fileinfo))
                            {
                                DialogBox frm = new DialogBox("Error Message", "The file is already open. Please close the file and try again.", true);
                                frm.Owner = Window.GetWindow(this);
                                frm.ShowDialog();
                                return;
                            }
                            else
                            {
                                List<MonthlyScheduleMasterEntity> ScheduleMasterImportData = new List<MonthlyScheduleMasterEntity>();
                                ExcelPackage Excelpck = new ExcelPackage(fileinfo);
                                ExcelWorksheet worksheet = Excelpck.Workbook.Worksheets[21];
                                DataTable dt = new DataTable();
                                string Monthyear = worksheet.Cells["C3"].Text.ToString();
                                DateTime MonthYear = DateTime.Now;
                                DateTime.TryParseExact(Monthyear, "MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out MonthYear);
                                DateTime Dateforimport = MonthYear;
                                for (int i = 6; i <= worksheet.Dimension.End.Row; i++)
                                {
                                    if (worksheet.Cells[i, 1].Value != null && !(string.IsNullOrEmpty(worksheet.Cells[i, 1].Value.ToString()) || worksheet.Cells[i, 1].Value.ToString().Equals("Total", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        for (int j = 10; j <= worksheet.Dimension.End.Column; j++)
                                        {
                                            MonthlyScheduleMasterEntity ImportEntity = new MonthlyScheduleMasterEntity();
                                            if (worksheet.Cells[i, 1].Value != null)
                                            {
                                                ImportEntity.PackagingType = worksheet.Cells[i, 1].Value.ToString();
                                            }
                                            if (worksheet.Cells[i, 2].Value != null)
                                            {
                                                ImportEntity.CustomerName = worksheet.Cells[i, 2].Value.ToString();
                                            }
                                            if (worksheet.Cells[i, 3].Value != null)
                                            {
                                                ImportEntity.PumpPartNumber = worksheet.Cells[i, 3].Value.ToString().Replace(" ", "");
                                            }
                                            if (worksheet.Cells[i, 4].Value != null)
                                            {
                                                ImportEntity.MonthSchedule = Convert.ToInt32(worksheet.Cells[i, 4].Value.ToString());
                                            }
                                            if (worksheet.Cells[i, 5].Value != null)
                                            {
                                                ImportEntity.ScheduleWeek1 = Convert.ToInt32(worksheet.Cells[i, 5].Value.ToString());
                                            }
                                            if (worksheet.Cells[i, 6].Value != null)
                                            {
                                                ImportEntity.ScheduleWeek2 = Convert.ToInt32(worksheet.Cells[i, 6].Value.ToString());
                                            }
                                            if (worksheet.Cells[i, 7].Value != null)
                                            {
                                                ImportEntity.ScheduleWeek3 = Convert.ToInt32(worksheet.Cells[i, 7].Value.ToString());
                                            }
                                            if (worksheet.Cells[i, 8].Value != null)
                                            {
                                                ImportEntity.ScheduleWeek4 = Convert.ToInt32(worksheet.Cells[i, 8].Value.ToString());
                                            }
                                            if (worksheet.Cells[i, 9].Value != null)
                                            {
                                                ImportEntity.ScheduleWeek5 = Convert.ToInt32(worksheet.Cells[i, 9].Value.ToString());
                                            }
                                            ImportEntity.WorkOrderNumber = "";
                                            ImportEntity.CustomerModel = "";
                                            ImportEntity.Date = MonthYear;
                                            MonthYear = MonthYear.AddDays(1);
                                            if (worksheet.Cells[i, j].Value != null && !string.IsNullOrEmpty(worksheet.Cells[i, j].Value.ToString()))
                                            {
                                                ImportEntity.DispatchQty = Convert.ToInt32(worksheet.Cells[i, j].Value.ToString());
                                                ScheduleMasterImportData.Add(ImportEntity);
                                            }
                                        }
                                        MonthYear = Dateforimport;
                                    }
                                }
                                if (ScheduleMasterImportData != null && ScheduleMasterImportData.Count > 0)
                                {
                                    if (ScheduleMasterImportData.Any(x => string.IsNullOrEmpty(x.PackagingType)))
                                    {
                                        DialogBox dlgError = new DialogBox("Error!!", "One or more Packing type in the file is empty. Please enter type and import again.", true);
                                        dlgError.Owner = Window.GetWindow(this);
                                        dlgError.ShowDialog();
                                        return;
                                    }
                                    if (ScheduleMasterImportData.Any(x => !PackagingTypeList.Contains(x.PackagingType)))
                                    {
                                        DialogBox dlgError = new DialogBox("Error!!", "One or more Packing type does not match with the allowed types in the table.", true);
                                        dlgError.Owner = Window.GetWindow(this);
                                        dlgError.ShowDialog();
                                        return;
                                    }
                                    if (ScheduleMasterImportData.Any(x => string.IsNullOrEmpty(x.PumpPartNumber)))
                                    {
                                        DialogBox dlgError = new DialogBox("Error!!", "One or more Part no. in the file is empty. Please enter the data and import again.", true);
                                        dlgError.Owner = Window.GetWindow(this);
                                        dlgError.ShowDialog();
                                        return;
                                    }
                                    foreach (MonthlyScheduleMasterEntity Entity in ScheduleMasterImportData)
                                    {
                                        if ((Entity.PumpPartNumber.Contains("(") && Entity.PumpPartNumber.Contains(")")))
                                        {
                                            Updated = MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                                            Updated = MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                                        }
                                        else if (!Entity.PumpPartNumber.Contains("(") && !Entity.PumpPartNumber.Contains(")"))
                                        {
                                            Updated = MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                                            Updated = MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                                        }
                                    }
                                }
                                if (Updated)
                                {
                                    DialogBox dlgError = new DialogBox("Information!!", "Imported Successfully.", false);
                                    dlgError.Owner = Window.GetWindow(this);
                                    dlgError.ShowDialog();
                                    txtFileName.Text = "";
                                }
                            }
                            if (cmbYear.SelectedValue != null && cmbMonth.SelectedValue != null)
                            {
                                DateTime SelectedDate = Convert.ToDateTime("01-" + cmbMonth.SelectedValue.ToString() + "-" + cmbYear.SelectedValue.ToString());
                                BindScheduleMasterData(SelectedDate);
                            }
                        }
                        else
                        {
                            DialogBox dlgError = new DialogBox("Error!!", "File does not exists.", true);
                            dlgError.Owner = Window.GetWindow(this);
                            dlgError.ShowDialog();
                        }
                    }
                    else
                    {
                        DialogBox dlgError = new DialogBox("Information!!", "Please select an excel file to import.", false);
                        dlgError.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnNew.Content.Equals("Cancel"))
                    btnNew_Click(null, null);
                if (cmbYear.SelectedValue != null && cmbMonth.SelectedValue != null)
                {
                    DateTime SelectedDate = Convert.ToDateTime("01-" + cmbMonth.SelectedValue.ToString() + "-" + cmbYear.SelectedValue.ToString());
                    BindScheduleMasterData(SelectedDate);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ScheduleMasterImportGrid.IsReadOnly = false;
                var rr = btnNew.Content;
                if (btnNew.Content.ToString() == "New")
                {
                    ScheduleMasterImportGrid.CanUserAddRows = true;
                    var datagridCellInfo = new DataGridCellInfo(ScheduleMasterImportGrid.Items, ScheduleMasterImportGrid.Columns[0]);
                    btnNew.Content = "Cancel";
                    int rowIndex = ScheduleMasterImportGrid.Items.Count - 1;
                    ScheduleMasterImportGrid.CurrentCell = datagridCellInfo;
                    ScheduleMasterImportGrid.SelectedIndex = ScheduleMasterImportGrid.Items.Count - 1;
                    ScheduleMasterImportGrid.Columns[0].IsReadOnly = false;
                    ScheduleMasterImportGrid.Columns[1].IsReadOnly = false;
                    ScheduleMasterImportGrid.Columns[4].IsReadOnly = false;
                    if (ScheduleMasterImportGrid.Items.Count > 0)
                    {
                        var border = VisualTreeHelper.GetChild(ScheduleMasterImportGrid, 0) as Decorator;
                        if (border != null)
                        {
                            var scroll = border.Child as ScrollViewer;
                            if (scroll != null) scroll.ScrollToEnd();
                            ScheduleMasterImportGrid.CurrentCell = new DataGridCellInfo(ScheduleMasterImportGrid.SelectedIndex, ScheduleMasterImportGrid.Columns[1]);
                            int x = ScheduleMasterImportGrid.Items.Count - 1;
                            ScheduleMasterImportGrid.SelectedItem = ScheduleMasterImportGrid.Items[x];
                            ScheduleMasterImportGrid.ScrollIntoView(ScheduleMasterImportGrid.Items[x]);
                            if (scheduleMasterDataList.Count > 0)
                            {
                                DataGridRow dgrow = (DataGridRow)ScheduleMasterImportGrid.ItemContainerGenerator.ContainerFromItem(ScheduleMasterImportGrid.Items[x]);
                                if (dgrow != null)
                                {
                                    dgrow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                                    ScheduleMasterImportGrid.CurrentCell = new DataGridCellInfo(ScheduleMasterImportGrid.Items[x], ScheduleMasterImportGrid.Columns[6]);
                                    ScheduleMasterImportGrid.BeginEdit();
                                }
                            }
                        }
                    }
                }
                else if (btnNew.Content.ToString() == "Cancel")
                {
                    btnNew.Content = "New";
                    ScheduleMasterImportGrid.CancelEdit();
                    ScheduleMasterImportGrid.CanUserAddRows = false;
                    if (cmbYear.SelectedValue != null && cmbMonth.SelectedValue != null)
                    {
                        DateTime SelectedDate = Convert.ToDateTime("01-" + cmbMonth.SelectedValue.ToString() + "-" + cmbYear.SelectedValue.ToString());
                        BindScheduleMasterData(SelectedDate);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool saved = false; int count = 0;
                ObservableCollection<MonthlyScheduleMasterEntity> ScheduleMasterUpdateData = ScheduleMasterImportGrid.ItemsSource as ObservableCollection<MonthlyScheduleMasterEntity>;
                if (ScheduleMasterUpdateData != null && ScheduleMasterUpdateData.Count > 0)
                {
                    foreach (MonthlyScheduleMasterEntity Entity in ScheduleMasterUpdateData)
                    {
                        if (Entity.IsRowChanged)
                        {
                            if (string.IsNullOrEmpty(Entity.PumpPartNumber))
                            {
                                DialogBox dlgError = new DialogBox("Information!!", "Please add Pump Model.", false);
                                dlgError.ShowDialog();
                                return;
                            }
                            DateTime scheduleMonth = DateTime.Now;
                            if (Entity.Date == null || Entity.Date.Year.Equals(1))
                            {
                                string date = $"{cmbYear.SelectedValue.ToString()}-{cmbMonth.SelectedValue.ToString()}-01 00:00:00";
                                scheduleMonth = Convert.ToDateTime(date);
                            }
                            else
                            {
                                scheduleMonth = Convert.ToDateTime(Entity.Date.ToString("yyyy-MM-01 00:00:00"));
                            }
                            int days = DateTime.DaysInMonth(scheduleMonth.Year, scheduleMonth.Month);
                            SaveDailySchedule(Entity, scheduleMonth, days);
                        }
                        count++;
                    }
                    saved = true;
                }
                if (saved)
                {
                    btnNew.Content = "New";
                    ScheduleMasterImportGrid.CancelEdit();
                    ScheduleMasterImportGrid.CanUserAddRows = false;
                    if (cmbYear.SelectedValue != null && cmbMonth.SelectedValue != null)
                    {
                        DateTime SelectedDate = Convert.ToDateTime("01-" + cmbMonth.SelectedValue.ToString() + "-" + cmbYear.SelectedValue.ToString());
                        BindScheduleMasterData(SelectedDate);
                    }
                    DialogBox dlgError = new DialogBox("Information!!", "Data Updated Successfully.", false);
                    dlgError.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (btnNew.Content.Equals("Cancel")) btnNew_Click(null, null);
        }

        private bool SaveDailySchedule(MonthlyScheduleMasterEntity Entity, DateTime CurrentMonth, int Totaldays)
        {
            bool success = false;
            try
            {
                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.One;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Two;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Three;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Four;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Five;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Six;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Seven;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Eight;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Nine;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Ten;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Eleven;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Twelve;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Thirteen;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Fourteen;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Fifteen;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Sixteen;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Seventeen;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Eighteen;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Nineteen;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Twenty;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Twentyone;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);

                CurrentMonth = CurrentMonth.AddDays(1);
                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Twentytwo;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Twentythree;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Twentyfour;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);


                CurrentMonth = CurrentMonth.AddDays(1);
                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Twentyfive;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Twentysix;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Twentyseven;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                Entity.Date = CurrentMonth;
                Entity.DispatchQty = Entity.Twentyeight;
                MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                CurrentMonth = CurrentMonth.AddDays(1);

                if (Totaldays > 28)
                {
                    Entity.Date = CurrentMonth;
                    Entity.DispatchQty = Entity.Twentynine;
                    MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                    MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                    CurrentMonth = CurrentMonth.AddDays(1);
                    if (Totaldays > 29)
                    {
                        Entity.Date = CurrentMonth;
                        Entity.DispatchQty = Entity.Thirty;
                        MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                        MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                        CurrentMonth = CurrentMonth.AddDays(1);
                    }
                    if (Totaldays > 30)
                    {
                        Entity.Date = CurrentMonth;
                        Entity.DispatchQty = Entity.ThirtyOne;
                        MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                        MivinDataBaseAccess.SaveScheduleMonthReportMaster(Entity);
                    }
                }
                success = true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            return success;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            bool delete = false;
            try
            {
                MonthlyScheduleMasterEntity Entity = ScheduleMasterImportGrid.SelectedValue as MonthlyScheduleMasterEntity;
                if (Entity != null)
                {
                    for (int i = 1; i <= DateTime.DaysInMonth(Entity.Date.Year, Entity.Date.Month); i++)
                    {
                        delete = MivinDataBaseAccess.DeleteScheduleMaster(Entity.Date, Entity.PumpPartNumber, Entity.PackagingType, Entity.CustomerName);
                        delete = MivinDataBaseAccess.DeleteMonthlyScheduleMaster(Entity.Date, Entity.PumpPartNumber, Entity.PackagingType, Entity.CustomerName);
                        Entity.Date = Entity.Date.AddDays(1);
                    }
                }
                if (delete)
                {
                    DialogBox dlgError = new DialogBox("Information!!", "Record Deleted Successfully.", false);
                    dlgError.Owner = Window.GetWindow(this);
                    dlgError.ShowDialog();
                }
                if (cmbYear.SelectedValue != null && cmbMonth.SelectedValue != null)
                {
                    DateTime SelectedDate = Convert.ToDateTime("01-" + cmbMonth.SelectedValue.ToString() + "-" + cmbYear.SelectedValue.ToString());
                    BindScheduleMasterData(SelectedDate);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbYear.SelectedValue != null && cmbMonth.SelectedValue != null)
                {
                    DateTime SelectedDate = Convert.ToDateTime("01-" + cmbMonth.SelectedValue.ToString() + "-" + cmbYear.SelectedValue.ToString());
                    BindScheduleMasterData(SelectedDate);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtPumpModelSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearch_Click(null, null);
            }
        }

        private void cmbpackagingtype_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox cmbpackagingType = sender as ComboBox;
            if (PackagingTypeList != null && PackagingTypeList.Count > 0)
            {
                cmbpackagingType.ItemsSource = PackagingTypeList;
            }
        }

        private void cmbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BindMonths();
        }

        private void ScheduleMasterImportGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                e.Handled = true;
        }

        private void ScheduleMasterImportGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);

                Control control = Utility.GetFirstChildByType<Control>(e.OriginalSource as DataGridCell);
                if (control != null)
                {
                    control.Focus();
                }
            }
        }
    }
}
