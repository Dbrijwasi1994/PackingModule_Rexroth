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
    /// Interaction logic for CustomerInformation.xaml
    /// </summary>
    public partial class CustomerInformation : UserControl
    {
        ObservableCollection<CustomerInfoEntity> customerInformationData = null;
        public CustomerInformation()
        {
            InitializeComponent();
        }

        private void CustomerInformation_Control_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadCustomerInformationData();
                if (Utility.CurrentEmployeeRole.Equals("Admin"))
                {
                    CustomerInformationGrid.IsReadOnly = false;
                    btnDelete.Visibility = Visibility.Visible;
                    btnNew.Visibility = Visibility.Visible;
                    btnSave.Visibility = Visibility.Visible;
                }
                else
                {
                    CustomerInformationGrid.IsReadOnly = true;
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

        private void LoadCustomerInformationData()
        {
            customerInformationData = new ObservableCollection<CustomerInfoEntity>();
            try
            {
                customerInformationData = MivinDataBaseAccess.GetAllCustomerInformationData();
                if (customerInformationData != null && customerInformationData.Count > 0)
                {
                    CustomerInformationGrid.ItemsSource = customerInformationData;
                    CustomerInformationGrid.Columns[0].IsReadOnly = true;
                }
                else
                {
                    CustomerInformationGrid.ItemsSource = new ObservableCollection<CustomerInfoEntity>();
                    MessageBox.Show("No customer information available.", "Information!!", MessageBoxButton.OK, MessageBoxImage.Information);
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
                if (CustomerInformationGrid.SelectedItems.Count > 0)
                    btnDelete.IsEnabled = true;
                else
                    btnDelete.IsEnabled = false;
            }
        }

        private void txtCustomerID_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (customerInformationData != null && customerInformationData.Count > 0)
                {
                    if (!string.IsNullOrEmpty(txtCustomerID.Text))
                    {
                        if (btnNew.Content.Equals("Cancel"))
                        {
                            customerInformationData = MivinDataBaseAccess.GetAllCustomerInformationData();
                            if (customerInformationData != null && customerInformationData.Count > 0)
                                CustomerInformationGrid.ItemsSource = new ObservableCollection<CustomerInfoEntity>(customerInformationData.Where(x => x.CustomerID.Contains(txtCustomerID.Text)));
                            else
                                CustomerInformationGrid.ItemsSource = new ObservableCollection<CustomerInfoEntity>();
                        }
                        else
                        {
                            CustomerInformationGrid.ItemsSource = new ObservableCollection<CustomerInfoEntity>(customerInformationData.Where(x => x.CustomerID.Contains(txtCustomerID.Text)));
                        }
                    }
                    else
                    {
                        CustomerInformationGrid.ItemsSource = customerInformationData;
                        CustomerInformationGrid.Columns[0].IsReadOnly = true;
                    }
                }
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
            LoadCustomerInformationData();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerInformationGrid.IsReadOnly = false;
                if (btnNew.Content.ToString() == "New")
                {
                    CustomerInformationGrid.CanUserAddRows = true;
                    var datagridCellInfo = new DataGridCellInfo(CustomerInformationGrid.Items, CustomerInformationGrid.Columns[0]);
                    btnNew.Content = "Cancel";
                    int rowIndex = CustomerInformationGrid.Items.Count - 1;
                    CustomerInformationGrid.CurrentCell = datagridCellInfo;
                    CustomerInformationGrid.SelectedIndex = CustomerInformationGrid.Items.Count - 1;
                    if (CustomerInformationGrid.Items.Count > 0)
                    {
                        var border = VisualTreeHelper.GetChild(CustomerInformationGrid, 0) as Decorator;
                        if (border != null)
                        {
                            CustomerInformationGrid.Columns[0].IsReadOnly = false;
                            var scroll = border.Child as ScrollViewer;
                            if (scroll != null) scroll.ScrollToEnd();
                            CustomerInformationGrid.CurrentCell = new DataGridCellInfo(CustomerInformationGrid.SelectedIndex, CustomerInformationGrid.Columns[1]);
                            int x = CustomerInformationGrid.Items.Count - 1;
                            CustomerInformationGrid.SelectedItem = CustomerInformationGrid.Items[x];
                            CustomerInformationGrid.ScrollIntoView(CustomerInformationGrid.Items[x]);

                            if (customerInformationData.Count > 0)
                            {
                                DataGridRow dgrow = (DataGridRow)CustomerInformationGrid.ItemContainerGenerator.ContainerFromItem(CustomerInformationGrid.Items[x]);
                                dgrow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                            }
                            CustomerInformationGrid.BeginEdit();
                        }
                    }
                }
                else
                {
                    btnNew.Content = "New";
                    CustomerInformationGrid.CancelEdit();
                    CustomerInformationGrid.DataContext = null;
                    CustomerInformationGrid.ItemsSource = null;
                    var changed = CustomerInformationGrid.ItemsSource as DataTable;
                    if (changed != null && changed.Rows.Count > 0)
                    {
                        foreach (DataRow row in changed.Rows.Cast<DataRow>().ToList())
                        {
                            if (row.RowState == DataRowState.Added)
                            {
                                changed.Rows.Remove(row);
                            }
                        }
                        CustomerInformationGrid.DataContext = changed as DataTable;
                    }
                    if (changed == null)
                    {
                        LoadCustomerInformationData();
                    }
                    CustomerInformationGrid.CanUserAddRows = false;
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
                ObservableCollection<CustomerInfoEntity> custInfoData = (CustomerInformationGrid.ItemsSource as ObservableCollection<CustomerInfoEntity>);
                if (custInfoData == null || custInfoData.Count < 1)
                {
                    DialogBox dlgInfo = new DialogBox("Information!!!", "No Data to Save.", false);
                    dlgInfo.ShowDialog();
                    return;
                }
                foreach (CustomerInfoEntity custInfoDataRow in custInfoData)
                {
                    if (custInfoDataRow.IsRowChanged)
                    {
                        if (string.IsNullOrEmpty(custInfoDataRow.CustomerID))
                        {
                            DialogBox dlgInfo = new DialogBox("Information!!", "Customer ID cannot be empty.", false);
                            dlgInfo.ShowDialog();
                            return;
                        }
                        if (string.IsNullOrEmpty(custInfoDataRow.CustomerName))
                        {
                            DialogBox dlgInfo = new DialogBox("Information!!", "Customer Name cannot be empty.", false);
                            dlgInfo.ShowDialog();
                            return;
                        }
                        MivinDataBaseAccess.UpdateCustomerInformation(custInfoDataRow, out IsUpdated);
                    }
                }
                if (IsUpdated)
                {
                    DialogBox dlgInfo = new DialogBox("Success!!!", "Data updated successfully.", false);
                    dlgInfo.ShowDialog();
                    LoadCustomerInformationData();
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
            if (CustomerInformationGrid.SelectedItems.Count > 0)
            {
                DialogBoxYesNo dlgDeleteConfirmation = new DialogBoxYesNo("Delete Confirmation", "Are You Sure you want to delete this data ? Click Yes to continue...");
                dlgDeleteConfirmation.Owner = Window.GetWindow(this);
                dlgDeleteConfirmation.ShowDialog();
                if (Utility.YesNoAnswer)
                {
                    try
                    {
                        var selectedRow = CustomerInformationGrid.SelectedItem as CustomerInfoEntity;
                        if (selectedRow != null)
                        {
                            MivinDataBaseAccess.DeleteCustomerInformation(selectedRow.CustomerID, out IsDeleted);
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
                    LoadCustomerInformationData();
                }
            }
            else
            {
                DialogBox dlgInfo = new DialogBox("Information Message", "Select a record to delete.", false);
                dlgInfo.Owner = Window.GetWindow(this);
                dlgInfo.ShowDialog();
            }
        }

        private void CustomerInformationGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CustomerInformationGrid.SelectedItems.Count > 0 && btnNew.Content.Equals("New"))
                    btnDelete.IsEnabled = true;
                else
                    btnDelete.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void comboState_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboState = sender as ComboBox;
            comboState.ItemsSource = Locations.StatesList;
            comboState.SelectedIndex = 0;
        }

        private void comboCountry_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboCountries = sender as ComboBox;
            comboCountries.ItemsSource = Locations.CountriesList.Where(x => x.Trim().Equals("India")).ToList();
            comboCountries.SelectedIndex = 0;
        }

        private void CustomerInformationGrid_GotFocus(object sender, RoutedEventArgs e)
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

        private void CustomerInformationGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                e.Handled = true;
        }
    }
}
