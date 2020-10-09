using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using DeburringProcess_Peekay.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Interaction logic for PumpInformation.xaml
    /// </summary>
    public partial class PumpInformation : UserControl
    {
        ObservableCollection<PumpInfoEntity> PumpInformationData = null;
        List<string> PackagingTypeList = null;
        public PumpInformation()
        {
            InitializeComponent();
        }

        private void PumpInformation_Control_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                BindPumpInformationGrid(string.Empty);
                PackagingTypeList = MivinDataBaseAccess.GetPackagingTypeData();
                SetControlsVisibility();
            }
            catch (Exception ex)
            {
                DialogBox dlgError = new DialogBox("Error!!", ex.Message, true);
                dlgError.ShowDialog();
            }
        }

        private void SetControlsVisibility()
        {
            try
            {
                if (Utility.CurrentEmployeeRole.Contains("Admin") || Utility.CurrentEmployeeRole.Equals("MasterPlanner"))
                {
                    PumpInfoGrid.IsReadOnly = false;
                    btnDelete.Visibility = Visibility.Visible;
                    btnNew.Visibility = Visibility.Visible;
                    btnSave.Visibility = Visibility.Visible;
                }
                else
                {
                    PumpInfoGrid.IsReadOnly = true;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnNew.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BindPumpInformationGrid(string pumpPartNumber)
        {
            PumpInformationData = new ObservableCollection<PumpInfoEntity>();
            try
            {
                PumpInformationData = MivinDataBaseAccess.GetAllPumpInformationData(pumpPartNumber);
                if (PumpInformationData != null && PumpInformationData.Count > 0)
                {
                    PumpInfoGrid.ItemsSource = PumpInformationData;
                    PumpInfoGrid.Columns[0].IsReadOnly = true;
                    PumpInfoGrid.Columns[4].IsReadOnly = false;
                }
                else
                {
                    PumpInfoGrid.ItemsSource = new ObservableCollection<PumpInfoEntity>();
                    DialogBox dlgInfo = new DialogBox("Information!!", "No Pump information available.", false);
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
                if (PumpInfoGrid.SelectedItems.Count > 0)
                    btnDelete.IsEnabled = true;
                else
                    btnDelete.IsEnabled = false;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BindPumpInformationGrid(txtPumpPartNumber.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PumpInfoGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (PumpInfoGrid.SelectedItems.Count > 0 && btnNew.Content.Equals("New"))
                    btnDelete.IsEnabled = true;
                else
                    btnDelete.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (btnNew.Content.Equals("Cancel"))
                btnNew_Click(null, null);
            txtPumpPartNumber.Text = string.Empty;
            BindPumpInformationGrid(string.Empty);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PumpInfoGrid.IsReadOnly = false;
                if (btnNew.Content.ToString() == "New")
                {
                    PumpInfoGrid.CanUserAddRows = true;
                    var datagridCellInfo = new DataGridCellInfo(PumpInfoGrid.Items, PumpInfoGrid.Columns[0]);
                    btnNew.Content = "Cancel";
                    int rowIndex = PumpInfoGrid.Items.Count - 1;
                    PumpInfoGrid.CurrentCell = datagridCellInfo;
                    PumpInfoGrid.SelectedIndex = PumpInfoGrid.Items.Count - 1;
                    if (PumpInfoGrid.Items.Count > 0)
                    {
                        var border = VisualTreeHelper.GetChild(PumpInfoGrid, 0) as Decorator;
                        if (border != null)
                        {
                            PumpInfoGrid.Columns[0].IsReadOnly = false;
                            PumpInfoGrid.Columns[4].IsReadOnly = false;
                            var scroll = border.Child as ScrollViewer;
                            if (scroll != null) scroll.ScrollToEnd();
                            PumpInfoGrid.CurrentCell = new DataGridCellInfo(PumpInfoGrid.SelectedIndex, PumpInfoGrid.Columns[1]);
                            int x = PumpInfoGrid.Items.Count - 1;
                            PumpInfoGrid.SelectedItem = PumpInfoGrid.Items[x];
                            PumpInfoGrid.ScrollIntoView(PumpInfoGrid.Items[x]);

                            if (PumpInformationData.Count > 0)
                            {
                                DataGridRow dgrow = (DataGridRow)PumpInfoGrid.ItemContainerGenerator.ContainerFromItem(PumpInfoGrid.Items[x]);
                                dgrow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                            }
                            PumpInfoGrid.BeginEdit();
                        }
                    }
                }
                else
                {
                    btnNew.Content = "New";
                    PumpInfoGrid.CancelEdit();
                    PumpInfoGrid.DataContext = null;
                    PumpInfoGrid.ItemsSource = null;
                    var changed = PumpInfoGrid.ItemsSource as DataTable;
                    if (changed != null && changed.Rows.Count > 0)
                    {
                        foreach (DataRow row in changed.Rows.Cast<DataRow>().ToList())
                        {
                            if (row.RowState == DataRowState.Added)
                            {
                                changed.Rows.Remove(row);
                            }
                        }
                        PumpInfoGrid.DataContext = changed as DataTable;
                    }
                    if (changed == null)
                    {
                        BindPumpInformationGrid(txtPumpPartNumber.Text);
                    }
                    PumpInfoGrid.CanUserAddRows = false;
                }
            }
            catch (Exception ex)
            {
                DialogBox dlgError = new DialogBox("Error!!", ex.Message, true);
                dlgError.ShowDialog();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool IsUpdated = false;
            try
            {
                ObservableCollection<PumpInfoEntity> pumpInfoData = (PumpInfoGrid.ItemsSource as ObservableCollection<PumpInfoEntity>);
                if (pumpInfoData == null || pumpInfoData.Count < 1)
                {
                    DialogBox dlgInfo = new DialogBox("Information!!!", "No data to save.", false);
                    dlgInfo.ShowDialog();
                    return;
                }
                foreach (PumpInfoEntity pumpInfoDataRow in pumpInfoData)
                {
                    if (pumpInfoDataRow.IsRowChanged)
                    {
                        if (string.IsNullOrEmpty(pumpInfoDataRow.PumpModel))
                        {
                            DialogBox dlgInfo = new DialogBox("Information!!", "Pump model cannot be empty.", false);
                            dlgInfo.ShowDialog();
                            return;
                        }
                        if (string.IsNullOrEmpty(pumpInfoDataRow.PackagingType))
                        {
                            DialogBox dlgInfo = new DialogBox("Information!!", "Packaging type cannot be empty.", false);
                            dlgInfo.ShowDialog();
                            return;
                        }
                        MivinDataBaseAccess.UpdatePumpInformation(pumpInfoDataRow, out IsUpdated);
                    }
                }
                if (IsUpdated)
                {
                    DialogBox dlgInfo = new DialogBox("Success!!!", "Data updated successfully.", false);
                    dlgInfo.ShowDialog();
                    BindPumpInformationGrid(txtPumpPartNumber.Text);
                }
                else
                {
                    DialogBox dlgInfo = new DialogBox("Error!!!", "Error. Data not updated.", false);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                DialogBox dlgError = new DialogBox("Error!!", ex.Message, true);
                dlgError.ShowDialog();
            }
            if (btnNew.Content.Equals("Cancel")) btnNew_Click(null, null);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            bool IsDeleted = false;
            if (PumpInfoGrid.SelectedItems.Count > 0)
            {
                DialogBoxYesNo dlgDeleteConfirmation = new DialogBoxYesNo("Delete Confirmation", "Are You Sure you want to delete this data ? Click Yes to continue...");
                dlgDeleteConfirmation.Owner = Window.GetWindow(this);
                dlgDeleteConfirmation.ShowDialog();
                if (Utility.YesNoAnswer)
                {
                    try
                    {
                        var selectedRow = PumpInfoGrid.SelectedItem as PumpInfoEntity;
                        if (selectedRow != null)
                        {
                            MivinDataBaseAccess.DeletePumpInformation(selectedRow.PumpModel, selectedRow.PackagingType, out IsDeleted);
                        }
                    }
                    catch (Exception ex)
                    {
                        DialogBox dlgError = new DialogBox("Error!!", ex.Message, true);
                        dlgError.ShowDialog();
                    }
                }
                if (IsDeleted)
                {
                    DialogBox dbSuccess = new DialogBox("Information Message", "Record deleted successfully.", false);
                    dbSuccess.Owner = Window.GetWindow(this);
                    dbSuccess.ShowDialog();
                    BindPumpInformationGrid(txtPumpPartNumber.Text);
                }
            }
            else
            {
                DialogBox dlgInfo = new DialogBox("Information Message", "Select a record to delete.", false);
                dlgInfo.Owner = Window.GetWindow(this);
                dlgInfo.ShowDialog();
            }
        }

        private void comboCustomerName_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBox cmbCustomerName = sender as ComboBox;
                if (cmbCustomerName != null)
                {
                    List<string> CustomerNameList = MivinDataBaseAccess.GetAllCustomerNames();
                    if (CustomerNameList != null && CustomerNameList.Count > 0)
                    {
                        cmbCustomerName.ItemsSource = CustomerNameList;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }

        private void comboCustomerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ComboBox cmbCustomerName = sender as ComboBox;
                if (cmbCustomerName != null && PumpInfoGrid.SelectedItem != null)
                {
                    PumpInfoEntity selectedPumpInfo = PumpInfoGrid.SelectedItem as PumpInfoEntity;
                    if (selectedPumpInfo != null)
                    {
                        selectedPumpInfo.CustomerName = cmbCustomerName.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }

        private void comboSalesUnit_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBox cmbSalesUnit = sender as ComboBox;
                if (cmbSalesUnit != null)
                {
                    List<string> salesUnitList = MivinDataBaseAccess.GetAllSalesUnits();
                    if (salesUnitList != null && salesUnitList.Count > 0)
                    {
                        cmbSalesUnit.ItemsSource = salesUnitList;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }

        private void comboPackagingType_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBox cmbPackagingType = sender as ComboBox;
                if (cmbPackagingType != null)
                {
                    if (PackagingTypeList != null && PackagingTypeList.Count > 0)
                        cmbPackagingType.ItemsSource = PackagingTypeList;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }

        private void comboPumpType_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBox cmbPumpType = sender as ComboBox;
                if (cmbPumpType != null)
                {
                    List<string> cmbPumpTypeList = new List<string>() { "Solo", "Tandem" };
                    cmbPumpType.ItemsSource = cmbPumpTypeList;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }

        private void comboBoxDestination_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBox cmbBoxDestination = sender as ComboBox;
                if (cmbBoxDestination != null)
                {
                    List<string> cmbBoxDestinationList = new List<string>() { "DOMESTIC", "EXPORT" };
                    cmbBoxDestination.ItemsSource = cmbBoxDestinationList;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }

        private void txtPumpPartNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearch_Click(null, null);
            }
        }

        private void PumpInfoGrid_GotFocus(object sender, RoutedEventArgs e)
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

        private void PumpInfoGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                e.Handled = true;
        }
    }
}
