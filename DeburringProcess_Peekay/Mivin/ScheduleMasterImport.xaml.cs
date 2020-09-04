using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using DeburringProcess_Peekay.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using OfficeOpenXml;
using System.Globalization;

namespace DeburringProcess_Peekay.Mivin
{
    /// <summary>
    /// Interaction logic for ScheduleMasterImport.xaml
    /// </summary>
    public partial class ScheduleMasterImport : UserControl
    {
        string FileName = string.Empty;
        ObservableCollection<ScheduleMasterEntity> scheduleMasterDataList = null;
        List<string> PackagingTypeList = new List<string>();
        public ScheduleMasterImport()
        {
            InitializeComponent();
        }

        private void ScheduleMasterImport_Control_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Utility.CurrentEmployeeRole.Equals("Admin") || Utility.CurrentEmployeeRole.Equals("Planner") || Utility.CurrentEmployeeRole.Equals("Supervisor"))
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
                }
                BindPumpModel();
                PackagingTypeList = MivinDataBaseAccess.GetPackagingTypeData();
                if (comboWorkOrder.Text != null)
                    BindWorkOrder(comboWorkOrder.Text.ToString());
                if (comboWorkOrder.Text != null && comboPumpPartNumber.Text != null)
                    BindScheduleMasterData(comboWorkOrder.Text.ToString(), comboPumpPartNumber.Text.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BindPumpModel()
        {
            try
            {
                List<string> PumpModelList = MivinDataBaseAccess.GetPumplist();
                PumpModelList.Insert(0, "All");
                if (PumpModelList != null)
                {
                    comboPumpPartNumber.ItemsSource = PumpModelList;
                    comboPumpPartNumber.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
        }

        private void BindWorkOrder(string Pumbmodel)
        {
            try
            {
                Pumbmodel = Pumbmodel.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : Pumbmodel;

                List<string> WorkOrderList = MivinDataBaseAccess.GetWorkOrder(Pumbmodel, "Schedule");
                WorkOrderList.Insert(0, "All");
                if (WorkOrderList != null)
                {
                    comboWorkOrder.ItemsSource = WorkOrderList;
                    comboWorkOrder.SelectedIndex = 0;
                }

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
        }

        private void BindScheduleMasterData(string WorkOrder, string PumpModel)
        {
            scheduleMasterDataList = new ObservableCollection<ScheduleMasterEntity>();
            try
            {
                WorkOrder = WorkOrder.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : WorkOrder;
                PumpModel = PumpModel.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : PumpModel;
                scheduleMasterDataList = MivinDataBaseAccess.GetScheduleMasterData(WorkOrder, PumpModel);
                if (scheduleMasterDataList != null && scheduleMasterDataList.Count > 0)
                {
                    ScheduleMasterImportGrid.ItemsSource = scheduleMasterDataList;
                }
                else
                {
                    ScheduleMasterImportGrid.ItemsSource = new ObservableCollection<ScheduleMasterEntity>();
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
                            else if (IsFileLocked(fileinfo))
                            {
                                DialogBox frm = new DialogBox("Error Message", "The file is already open. Please close the file and try again.", true);
                                frm.Owner = Window.GetWindow(this);
                                frm.ShowDialog();
                                return;
                            }
                            else
                            {
                                List<ScheduleMasterEntity> ScheduleMasterImportData = new List<ScheduleMasterEntity>();
                                ExcelPackage Excelpck = new ExcelPackage(fileinfo);
                                ExcelWorksheet worksheet = Excelpck.Workbook.Worksheets[21];

                                DataTable dt = new DataTable();
                                string Monthyear = worksheet.Cells["E3"].Text.ToString();
                                DateTime MonthYear = DateTime.Now;
                                DateTime.TryParseExact(Monthyear, "MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out MonthYear);
                                DateTime Dateforimport = MonthYear;
                                for (int i = 6; i <= worksheet.Dimension.End.Row; i++)
                                {

                                    if (worksheet.Cells[i, 1].Value != null && !(string.IsNullOrEmpty(worksheet.Cells[i, 1].Value.ToString()) || worksheet.Cells[i, 1].Value.ToString().Equals("Total", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        for (int j = 11; j <= worksheet.Dimension.End.Column; j++)
                                        {
                                            ScheduleMasterEntity ImportEntity = new ScheduleMasterEntity();
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
                                                ImportEntity.WorkOrderNumber = worksheet.Cells[i, 3].Value.ToString();
                                            }
                                            if (worksheet.Cells[i, 4].Value != null)
                                            {
                                                ImportEntity.CustomerModel = worksheet.Cells[i, 4].Value.ToString();
                                            }
                                            if (worksheet.Cells[i, 5].Value != null)
                                            {
                                                ImportEntity.PumpPartNumber = worksheet.Cells[i, 5].Value.ToString();
                                            }
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
                                    foreach (ScheduleMasterEntity Entity in ScheduleMasterImportData)
                                    {
                                        if ((Entity.PumpPartNumber.Contains("(") && Entity.PumpPartNumber.Contains(")")))
                                        {
                                            Updated = MivinDataBaseAccess.SavScheduleReportMaster(Entity);
                                        }
                                        else if (!Entity.PumpPartNumber.Contains("(") && !Entity.PumpPartNumber.Contains(")"))
                                        {
                                            Updated = MivinDataBaseAccess.SavScheduleReportMaster(Entity);
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
                            if (comboWorkOrder.Text != null && comboPumpPartNumber.Text != null)
                                BindScheduleMasterData(comboWorkOrder.Text.ToString(), comboPumpPartNumber.Text);
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
                    Logger.WriteErrorLog(ex.Message);
                }

            }
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnNew.Content.Equals("Cancel"))
                    btnNew_Click(null, null);
                if (comboWorkOrder.Text != null && comboPumpPartNumber.Text != null)
                    BindScheduleMasterData(comboWorkOrder.Text.ToString(), comboPumpPartNumber.Text);
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
                    ScheduleMasterImportGrid.Columns[1].IsReadOnly = false;
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
                    if (comboWorkOrder.Text != null && comboPumpPartNumber.Text != null)
                        BindScheduleMasterData(comboWorkOrder.Text.ToString(), comboPumpPartNumber.Text);
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
                bool saved = false;
                ObservableCollection<ScheduleMasterEntity> ScheduleMasterUpdateData = ScheduleMasterImportGrid.ItemsSource as ObservableCollection<ScheduleMasterEntity>;
                if (ScheduleMasterUpdateData != null && ScheduleMasterUpdateData.Count > 0)
                {
                    foreach (ScheduleMasterEntity Entity in ScheduleMasterUpdateData)
                    {

                        if (Entity.IsRowChanged)
                        {
                            if (string.IsNullOrEmpty(Entity.WorkOrderNumber))
                            {
                                DialogBox dlgError = new DialogBox("Information!!", "Please add Workorder.", false);
                                dlgError.ShowDialog();
                                return;
                            }
                            if (string.IsNullOrEmpty(Entity.PumpPartNumber))
                            {
                                DialogBox dlgError = new DialogBox("Information!!", "Please add Pump Model.", false);
                                dlgError.ShowDialog();
                                return;
                            }
                            if ((Entity.Date).Equals(DateTime.MinValue))
                            {
                                DialogBox dlgError = new DialogBox("Information!!", "Please select the Date.", false);
                                dlgError.ShowDialog();
                                return;
                            }
                            saved = MivinDataBaseAccess.SavScheduleReportMaster(Entity);

                        }
                    }
                }
                if (saved)
                {
                    btnNew.Content = "New";
                    ScheduleMasterImportGrid.CancelEdit();
                    ScheduleMasterImportGrid.CanUserAddRows = false;
                    if (comboWorkOrder.Text != null && comboPumpPartNumber.Text != null)
                        BindScheduleMasterData(comboWorkOrder.Text.ToString(), comboPumpPartNumber.Text);
                    DialogBox dlgError = new DialogBox("Information!!", "Data Updated Successfully.", false);
                    dlgError.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            if (btnNew.Content.Equals("Cancel")) btnNew_Click(null, null);
        }

        private void Datetimepicker_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker datepick = sender as DatePicker;
            try
            {
                if (datepick.SelectedDate == DateTime.MinValue)
                {
                    datepick.SelectedDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            bool delete = false;
            try
            {
                ScheduleMasterEntity Entity = ScheduleMasterImportGrid.SelectedValue as ScheduleMasterEntity;
                if (Entity != null)
                {
                    delete = MivinDataBaseAccess.DeleteScheduleMaster(Entity.Date, Entity.WorkOrderNumber, Entity.PumpPartNumber);
                }
                if (delete)
                {
                    DialogBox dlgError = new DialogBox("Information!!", "Record Deleted Successfully.", false);
                    dlgError.Owner = Window.GetWindow(this);
                    dlgError.ShowDialog();
                }
                BindScheduleMasterData(comboWorkOrder.Text.ToString(), comboPumpPartNumber.Text.ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (comboWorkOrder.Text != null && comboPumpPartNumber.Text != null)
                BindScheduleMasterData(comboWorkOrder.Text.ToString(), comboPumpPartNumber.Text);
        }

        private void cmbpackagingtype_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox cmbpackagingType = sender as ComboBox;
            if (PackagingTypeList != null && PackagingTypeList.Count > 0)
            {
                cmbpackagingType.ItemsSource = PackagingTypeList;
            }
        }

        private void comboPumpPartNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboPumpPartNumber.SelectedValue != null)
                BindWorkOrder(comboPumpPartNumber.SelectedValue.ToString());
        }
    }
}
