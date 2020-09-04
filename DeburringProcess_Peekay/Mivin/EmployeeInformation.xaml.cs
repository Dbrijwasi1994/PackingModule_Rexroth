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
    /// Interaction logic for EmployeeInformation.xaml
    /// </summary>
    public partial class EmployeeInformation : UserControl
    {
        ObservableCollection<EmpInfoEntity> EmpInfoList = null;
        int insert = 0;
        public EmployeeInformation()
        {
            InitializeComponent();
        }

        private void EmployeeInformation_Control_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                BindEmployeeInformationGrid();
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
                if (Utility.CurrentEmployeeRole.Contains("Admin"))
                {
                    EmpInfoGrid.IsReadOnly = false;
                    btnDelete.Visibility = Visibility.Visible;
                    btnNew.Visibility = Visibility.Visible;
                    btnSave.Visibility = Visibility.Visible;
                }
                else if (Utility.CurrentEmployeeRole.Contains("Planner") || Utility.CurrentEmployeeRole.Equals("Operator") || Utility.CurrentEmployeeRole.Equals("Supervisor"))
                {
                    EmpInfoGrid.IsReadOnly = false;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnNew.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Visible;
                    EmpInfoGrid.Columns[3].IsReadOnly = true;
                    EmpInfoGrid.Columns[6].IsReadOnly = true;
                }
                else
                {
                    EmpInfoGrid.IsReadOnly = true;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnNew.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("NewItemPlaceholderPosition"))
                {
                    return;
                }
            }
        }

        private void BindEmployeeInformationGrid()
        {
            try
            {
                EmpInfoList = new ObservableCollection<EmpInfoEntity>();
                EmpInfoList = MivinDataBaseAccess.GetEmployeeInformation();
                if (EmpInfoList != null && EmpInfoList.Count > 0)
                {
                    if (Utility.CurrentEmployeeRole.Contains("Admin"))
                        EmpInfoGrid.ItemsSource = EmpInfoList;
                    else
                        EmpInfoGrid.ItemsSource = new ObservableCollection<EmpInfoEntity>(EmpInfoList.Where(x => x.EmpId.Equals(Utility.LoggedInUserName, StringComparison.OrdinalIgnoreCase)).ToList());
                }
                else
                {
                    EmpInfoGrid.ItemsSource = new ObservableCollection<EmpInfoEntity>();
                    DialogBox dlgInfo = new DialogBox("Information!!", "No employee information available.", false);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Errorr!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (btnNew.Content.Equals("Cancel"))
                    btnNew_Click(null, null);
                if (EmpInfoGrid.SelectedItems.Count > 0)
                    btnDelete.IsEnabled = true;
                else
                    btnDelete.IsEnabled = false;
            }
        }

        private void EmpInfoGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
                Control control = GetFirstChildByType<Control>(e.OriginalSource as DataGridCell);
                if (control != null)
                {
                    control.Focus();
                }
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EmpInfoGrid.IsReadOnly = false;
                if (btnNew.Content.ToString() == "New")
                {
                    EmpInfoGrid.CanUserAddRows = true;
                    insert = 1;
                    var datagridCellInfo = new DataGridCellInfo(EmpInfoGrid.Items, EmpInfoGrid.Columns[0]);
                    btnNew.Content = "Cancel";
                    int rowIndex = EmpInfoGrid.Items.Count - 1;
                    EmpInfoGrid.CurrentCell = datagridCellInfo;
                    EmpInfoGrid.SelectedIndex = EmpInfoGrid.Items.Count - 1;
                    if (EmpInfoGrid.Items.Count > 0)
                    {
                        var border = VisualTreeHelper.GetChild(EmpInfoGrid, 0) as Decorator;
                        if (border != null)
                        {
                            var scroll = border.Child as ScrollViewer;
                            if (scroll != null) scroll.ScrollToEnd();
                            EmpInfoGrid.CurrentCell = new DataGridCellInfo(EmpInfoGrid.SelectedIndex, EmpInfoGrid.Columns[1]);
                            int x = EmpInfoGrid.Items.Count - 1;
                            EmpInfoGrid.SelectedItem = EmpInfoGrid.Items[x];
                            EmpInfoGrid.ScrollIntoView(EmpInfoGrid.Items[x]);

                            EmpInfoGrid.Columns[0].IsReadOnly = false;
                            if (EmpInfoList.Count > 0)
                            {
                                DataGridRow dgrow = (DataGridRow)EmpInfoGrid.ItemContainerGenerator.ContainerFromItem(EmpInfoGrid.Items[x]);
                                //  dgrow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                            }
                            EmpInfoGrid.BeginEdit();
                        }
                    }
                }
                else
                {
                    btnNew.Content = "New";
                    EmpInfoGrid.Columns[0].IsReadOnly = true;
                    EmpInfoGrid.CancelEdit();
                    EmpInfoGrid.DataContext = null;
                    EmpInfoGrid.ItemsSource = null;
                    insert = 0;
                    var changed = EmpInfoGrid.ItemsSource as DataTable;
                    if (changed != null && changed.Rows.Count > 0)
                    {
                        foreach (DataRow row in changed.Rows.Cast<DataRow>().ToList())
                        {
                            if (row.RowState == DataRowState.Added)
                            {
                                changed.Rows.Remove(row);
                            }
                        }
                        EmpInfoGrid.DataContext = changed as DataTable;
                    }
                    if (changed == null)
                    {
                        BindEmployeeInformationGrid();
                    }
                    SetControlsVisibility();
                    EmpInfoGrid.CanUserAddRows = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DialogBoxYesNo dlgYN = new DialogBoxYesNo("Warning Message", "Are you sure you want to delete the data ?");
            dlgYN.ShowDialog();
            if (Utility.YesNoAnswer)
            {
                if (EmpInfoGrid.SelectedItems.Count > 0)
                {
                    EmpInfoEntity SelectedEmpInfo = EmpInfoGrid.SelectedItem as EmpInfoEntity;
                    try
                    {
                        if (SelectedEmpInfo.EmpId.Equals(Utility.LoggedInUserName))
                        {
                            DialogBox dlgInfo = new DialogBox("Information Message", "This Account Is in Use.\n Cannot Delete Right Now.", false);
                            dlgInfo.ShowDialog();
                            return;
                        }
                        MivinDataBaseAccess.DeleteEmployeeInformation(SelectedEmpInfo.EmpId);
                        DialogBox dlgSuccess = new DialogBox("Success!!", "Employee Record Successfully Deleted.", false);
                        dlgSuccess.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error!!!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    BindEmployeeInformationGrid();
                }
                else
                {
                    MessageBox.Show("Select an Employee record to delete", "Warning!!!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool IsUpdated = false;
                ObservableCollection<EmpInfoEntity> employeeInfoList = (EmpInfoGrid.ItemsSource as ObservableCollection<EmpInfoEntity>);

                if (employeeInfoList == null) return;
                foreach (EmpInfoEntity row in employeeInfoList)
                {
                    if (row.IsRowchnaged == true || insert == 1)
                    {
                        if (string.IsNullOrEmpty(row.EmpId) || string.IsNullOrEmpty(row.Name) || string.IsNullOrEmpty(row.EmployeeRole))
                        {
                            var errorDialog = new DialogBox("Error !!", "Warning!!!  Please fill all the required fields.", false);
                            errorDialog.ShowDialog();
                            return;
                        }
                        if (row.IsRowchnaged)
                        {
                            MivinDataBaseAccess.UpdateEmployeeInformation(row.EmpId, row.Name, row.Phone, row.IsAdmin, row.Email, row.Password, row.EmployeeRole, out IsUpdated);
                        }
                    }
                }
                if (IsUpdated)
                {
                    DialogBox dbSuccess = new DialogBox("Information Message", "Employee information Updated Successfully.", false);
                    dbSuccess.ShowDialog();
                    BindEmployeeInformationGrid();
                }
            }
            catch (Exception ex)
            {
                DialogBox dlg = new DialogBox("Error!!!", ex.Message, true);
                dlg.ShowDialog();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (btnNew.Content.Equals("Cancel"))
                btnNew_Click(null, null);
            SetControlsVisibility();
            BindEmployeeInformationGrid();
        }

        private void EmpInfoGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            (sender as DataGrid).CellEditEnding -= EmpInfoGrid_CellEditEnding;
            (sender as DataGrid).CommitEdit();
            (sender as DataGrid).CommitEdit();
            (sender as DataGrid).CellEditEnding += EmpInfoGrid_CellEditEnding;
        }

        private void EmpInfoGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (Utility.isAdmin.Equals("0"))
            {
                if (EmpInfoGrid.SelectedIndex == EmpInfoGrid.Items.Count - 2)
                {
                    DataGridRow dgrow = (DataGridRow)EmpInfoGrid.ItemContainerGenerator.ContainerFromItem(EmpInfoGrid.Items[EmpInfoGrid.Items.Count - 1]);
                    // dgrow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }

        private void EmpInfoGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            string headerName = string.Empty;
            var vv = e.Row.DataContext as EmpInfoEntity;
            if (vv != null)
            {
                if (Utility.isAdmin.Equals("0"))
                {
                    if (e.Row != null)
                    {
                        headerName = e.Column.Header.ToString();
                        if (headerName == "Employee ID")
                        { e.Cancel = true; }
                    }

                    if ((string.IsNullOrEmpty(vv.EmpId) || string.IsNullOrEmpty(vv.Email) || string.IsNullOrEmpty(vv.Name) || string.IsNullOrEmpty(vv.Password) || string.IsNullOrEmpty(vv.Phone)))
                    { vv.IsNewItem = true; }

                    if (vv.IsNewItem == false)
                    {
                        e.Cancel = true;
                    }
                    else { e.Cancel = false; }
                }
                if (Utility.isAdmin.Equals("1"))
                {
                    if (e.Row != null)
                    {
                        headerName = e.Column.Header.ToString();
                        if (headerName == "Employee ID")
                        { e.Cancel = true; }
                        if ((string.IsNullOrEmpty(vv.EmpId) || string.IsNullOrEmpty(vv.Email) || string.IsNullOrEmpty(vv.Name) || string.IsNullOrEmpty(vv.Password) || string.IsNullOrEmpty(vv.Phone)))
                        { e.Cancel = false; }
                    }
                }
            }
        }

        private void chkIsAdmin_Click(object sender, RoutedEventArgs e)
        {
            var vv = sender as CheckBox;
            var row = vv.DataContext as EmpInfoEntity;
            if (Utility.CurrentEmployeeRole.Contains("Admin"))
            {
                if (((CheckBox)sender).IsChecked == true)
                    row.IsAdmin = true;
                else
                    row.IsAdmin = false;
            }
            else
            {
                ((CheckBox)sender).IsChecked = false;
            }
        }

        private T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild((prop), i) as DependencyObject;
                if (child == null)
                    continue;

                T castedProp = child as T;
                if (castedProp != null)
                    return castedProp;

                castedProp = GetFirstChildByType<T>(child);

                if (castedProp != null)
                    return castedProp;
            }
            return null;
        }

        private void EmpInfoGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                e.Handled = true;
        }

        private void comboEmployeeRole_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBox comboBoxEmpRole = sender as ComboBox;
                if (comboBoxEmpRole != null)
                    comboBoxEmpRole.ItemsSource = Utility.EmployeeRolesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EmpInfoGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (EmpInfoGrid.SelectedItems.Count > 0 && btnNew.Content.Equals("New"))
                    btnDelete.IsEnabled = true;
                else
                    btnDelete.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class DataGridNumericColumn : DataGridTextColumn
    {
        protected override object PrepareCellForEdit(System.Windows.FrameworkElement editingElement, System.Windows.RoutedEventArgs editingEventArgs)
        {
            TextBox edit = editingElement as TextBox;
            edit.HorizontalContentAlignment = HorizontalAlignment.Center;
            edit.Margin = new Thickness(5, 0, 5, 0);
            edit.PreviewTextInput += OnPreviewTextInput;
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }
        void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }
    }

    public static class PasswordHelper
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordHelper), new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordHelper));

        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;
            if (!(bool)GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox == null) return;
            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }
            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}
