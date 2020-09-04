using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using PackingModule_Rexroth.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ShiftTargetMaster.xaml
    /// </summary>
    public partial class ShiftTargetMaster : UserControl
    {
        ObservableCollection<ShiftTargetMasterEntity> shiftTargetData = null;
        public ShiftTargetMaster()
        {
            InitializeComponent();
        }

        private void ShiftTargetMaster_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dtpDate.SelectedDate = DateTime.Now;
                BindShiftTargetMasterGrid((DateTime)dtpDate.SelectedDate);
                if (Utility.CurrentEmployeeRole.Contains("Admin") || Utility.CurrentEmployeeRole.Equals("ShiftPlanner") || Utility.CurrentEmployeeRole.Equals("Supervisor"))
                {
                    ShiftTargetMasterGrid.IsReadOnly = false;
                    btnUpdate.Visibility = Visibility.Visible;
                }
                else
                {
                    ShiftTargetMasterGrid.IsReadOnly = true;
                    btnUpdate.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                DialogBox dlgError = new DialogBox("Error!!", ex.Message, true);
                dlgError.ShowDialog();
            }
        }

        private void BindShiftTargetMasterGrid(DateTime selectedDate)
        {
            shiftTargetData = new ObservableCollection<ShiftTargetMasterEntity>();
            try
            {
                shiftTargetData = MivinDataBaseAccess.GetShiftTargetMasterData(selectedDate);
                if (shiftTargetData != null && shiftTargetData.Count > 0)
                {
                    ShiftTargetMasterGrid.ItemsSource = shiftTargetData;
                }
                else
                {
                    ShiftTargetMasterGrid.ItemsSource = new ObservableCollection<ShiftTargetMasterEntity>();
                    DialogBox dlgInfo = new DialogBox("Information!!", "No shift target data available.", false);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Errorr!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            BindShiftTargetMasterGrid((DateTime)dtpDate.SelectedDate);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            dtpDate.SelectedDate = DateTime.Now;
            BindShiftTargetMasterGrid((DateTime)dtpDate.SelectedDate);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool IsUpdated = false;
            try
            {
                ObservableCollection<ShiftTargetMasterEntity> shiftTargetData = (ShiftTargetMasterGrid.ItemsSource as ObservableCollection<ShiftTargetMasterEntity>);
                if (shiftTargetData == null || shiftTargetData.Count < 1)
                {
                    DialogBox dlgInfo = new DialogBox("Information!!!", "No data to save.", false);
                    dlgInfo.ShowDialog();
                    return;
                }
                foreach (ShiftTargetMasterEntity shiftTargetDataRow in shiftTargetData)
                {
                    if (shiftTargetDataRow.IsRowChanged)
                    {
                        MivinDataBaseAccess.UpdateShiftTargetData(shiftTargetDataRow, out IsUpdated);
                    }
                }
                if (IsUpdated)
                {
                    DialogBox dlgInfo = new DialogBox("Success!!!", "Data updated successfully.", false);
                    dlgInfo.ShowDialog();
                    BindShiftTargetMasterGrid((DateTime)dtpDate.SelectedDate);
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
        }

        private void ShiftTargetMasterGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                ShiftTargetMasterEntity selectedRow = e.Row.DataContext as ShiftTargetMasterEntity;
                if (selectedRow != null)
                {
                    if (e.Column.Header.Equals("No. Of Persons"))
                    {
                        selectedRow.ShiftTarget = selectedRow.NumOfPersons * 300;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }

        private void comboNumOfPersons_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBox cmbNumOfPersons = sender as ComboBox;
                if (cmbNumOfPersons != null)
                {
                    List<int> numOfPersonList = new List<int>() { 1, 2, 3, 4, 5 };
                    cmbNumOfPersons.ItemsSource = numOfPersonList;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }
    }
}
