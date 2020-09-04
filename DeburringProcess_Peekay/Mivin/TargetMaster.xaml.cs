using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for TargetMaster.xaml
    /// </summary>
    public partial class TargetMaster : UserControl
    {
        DataTable dtTargetMasterData = null;
        public TargetMaster()
        {
            InitializeComponent();
        }

        private void TargetMaster_Control_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dtpDate.SelectedDate = DateTime.Now;
                BindTargetMasterGrid();
            }
            catch (Exception ex)
            {
                DialogBox dlgError = new DialogBox("Error!!", ex.Message, true);
                dlgError.ShowDialog();
            }
        }

        private void BindTargetMasterGrid()
        {
            try
            {
                dtTargetMasterData = MivinDataBaseAccess.GetAllTargetMasterData((DateTime)dtpDate.SelectedDate);
                string[] columnNames = null;
                if (dtTargetMasterData != null && dtTargetMasterData.Rows.Count > 0)
                {
                    columnNames = dtTargetMasterData.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                    ModelTargetMasterGrid.ItemsSource = null;
                    ModelTargetMasterGrid.Columns.Clear();
                    ModelTargetMasterGrid.Items.Clear();
                    ModelTargetMasterGrid.Items.Refresh();
                    GenerateDataGridColumns(columnNames);
                    ModelTargetMasterGrid.ItemsSource = dtTargetMasterData.DefaultView;
                }
                else
                {
                    ModelTargetMasterGrid.ItemsSource = null;
                    ModelTargetMasterGrid.Columns.Clear();
                    ModelTargetMasterGrid.Items.Clear();
                    ModelTargetMasterGrid.Items.Refresh();
                    DialogBox dlgInfo = new DialogBox("Information!!", "No data available for selected dates.", false);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateDataGridColumns(string[] columnNames)
        {
            try
            {
                var col = new DataGridTextColumn();
                if (columnNames == null || columnNames.Length.Equals(0)) return;
                foreach (var column in columnNames)
                {
                    var ElementStyle = new Style(typeof(TextBlock));
                    if (column.Equals("Date") || column.Equals("Shift"))
                    {
                        col = new DataGridTextColumn();
                        col.IsReadOnly = true;
                        ElementStyle.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(8)));
                        ElementStyle.Setters.Add(new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center));
                        col.Header = column;
                        if (column.Equals("Date"))
                            col.Binding = new Binding(column) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay, StringFormat = "{0:yyyy-MM-dd}" };
                        else
                            col.Binding = new Binding(column) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                        col.MinWidth = 120;
                        col.MaxWidth = 250;
                    }
                    else
                    {
                        col = new DataGridNumericColumn();
                        col.IsReadOnly = column.Equals("TotalShiftTarget", StringComparison.OrdinalIgnoreCase) ? true : false;
                        ElementStyle.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(8)));
                        ElementStyle.Setters.Add(new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center));
                        col.Header = column.Equals("TotalShiftTarget", StringComparison.OrdinalIgnoreCase) ? "Total Shift Target" : column;
                        col.Binding = new Binding(column) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                        col.MinWidth = column.Equals("TotalShiftTarget", StringComparison.OrdinalIgnoreCase) ? 200 : 150;
                        col.MaxWidth = column.Equals("TotalShiftTarget", StringComparison.OrdinalIgnoreCase) ? 350 : 250;
                    }
                    col.ElementStyle = ElementStyle;
                    ModelTargetMasterGrid.Columns.Add(col);
                }
                ModelTargetMasterGrid.FrozenColumnCount = 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            BindTargetMasterGrid();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            BindTargetMasterGrid();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool IsUpdated = false;
            try
            {
                DataTable dtTargetMaster = ((DataView)ModelTargetMasterGrid.ItemsSource).ToTable();
                if (dtTargetMaster != null)
                {
                    DataTable dtChangedTargetMaster = dtTargetMaster.GetChanges();
                    if (dtChangedTargetMaster != null && dtChangedTargetMaster.Rows.Count > 0)
                    {
                        foreach (DataRow dataRowTargetMaster in dtChangedTargetMaster.Rows)
                        {
                            for (int i = 2; i < dataRowTargetMaster.ItemArray.Length - 1; i++)
                            {
                                string PumpModel = dtChangedTargetMaster.Columns[i].ColumnName;
                                MivinDataBaseAccess.UpdatePumpModelTargetInfo(Convert.ToDateTime(dataRowTargetMaster["Date"]).ToString("yyyy-MM-dd HH:mm:ss"), dataRowTargetMaster["Shift"].ToString(), PumpModel, dataRowTargetMaster[i].ToString(), out IsUpdated);
                            }
                        }
                        if (IsUpdated)
                        {
                            DialogBox dlgSuccess = new DialogBox("Information Message", "Data updated successfully.", false);
                            dlgSuccess.ShowDialog();
                            BindTargetMasterGrid();
                        }
                        else
                        {
                            DialogBox dlgError = new DialogBox("Error Message", "Error. Data not updated.", true);
                            dlgError.ShowDialog();
                        }
                    }
                }
                else
                {
                    DialogBox dlgError = new DialogBox("Error Message", "Error. Data not updated.", true);
                    dlgError.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ModelTargetMasterGrid_GotFocus(object sender, RoutedEventArgs e)
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

        private void ModelTargetMasterGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                e.Handled = true;
        }
    }
}
