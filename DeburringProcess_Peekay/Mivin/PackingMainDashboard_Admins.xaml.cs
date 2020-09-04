using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using DeburringProcess_Peekay.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using System.Windows.Threading;

namespace PackingModule_Rexroth.Mivin
{
    /// <summary>
    /// Interaction logic for PackingMainDashboard_Admins.xaml
    /// </summary>
    public partial class PackingMainDashboard_Admins : UserControl
    {
        ObservableCollection<RunningPumpModelEntity> runningPumpModelData = null;
        ObservableCollection<RunningShiftSchedulesEntity> runningShiftSchedulesData = null;
        ObservableCollection<RunningModelStatusEntity> runningModelDetails = null;
        DispatcherTimer dataRefreshTimer = null;
        public PackingMainDashboard_Admins()
        {
            InitializeComponent();
        }

        private void PackingDashboardAdmins_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRunningShiftStatus();
            int data_refresh_interval = Convert.ToInt32(ConfigurationManager.AppSettings["DataRefreshTimeInterval"].ToString());
            dataRefreshTimer = new DispatcherTimer();
            dataRefreshTimer.Interval = TimeSpan.FromSeconds(data_refresh_interval);
            dataRefreshTimer.Tick += DataRefreshTimer_Tick; ;
            if (dataRefreshTimer != null && !dataRefreshTimer.IsEnabled) dataRefreshTimer.Start();
        }

        private void LoadRunningShiftStatus()
        {
            ShiftWiseEnergyInfo shiftWiseEnergy = new ShiftWiseEnergyInfo();
            runningModelDetails = new ObservableCollection<RunningModelStatusEntity>();
            runningShiftSchedulesData = new ObservableCollection<RunningShiftSchedulesEntity>();
            runningPumpModelData = new ObservableCollection<RunningPumpModelEntity>();
            try
            {
                runningShiftSchedulesData = MivinDataBaseAccess.GetRunningShiftSchedulesData("", "", out runningModelDetails, out shiftWiseEnergy, out runningPumpModelData);
                if (runningModelDetails != null && runningModelDetails.Count > 0)
                {
                    RunningModelDetailsGrid.ItemsSource = runningModelDetails;
                }
                else
                {
                    RunningModelDetailsGrid.ItemsSource = new ObservableCollection<RunningModelStatusEntity>();
                }
                if (runningPumpModelData != null && runningPumpModelData.Count > 0)
                {
                    this.dgMonthlyScannedPumpDetails.DataContext = null;
                    this.dgDailyScannedPumpDetails.DataContext = null;
                    this.dgMonthlyScannedPumpDetails.ItemsSource = runningPumpModelData;
                    this.dgDailyScannedPumpDetails.ItemsSource = runningPumpModelData;
                }
                else
                {
                    this.dgMonthlyScannedPumpDetails.ItemsSource = new ObservableCollection<RunningPumpModelEntity>();
                    this.dgDailyScannedPumpDetails.ItemsSource = new ObservableCollection<RunningPumpModelEntity>();
                }
                this.gaugeShiftTargetVsActual.DataContext = shiftWiseEnergy;
                scaleShiftTargetVsActual.Interval = shiftWiseEnergy.ShiftTarget < 100 ? 20 : shiftWiseEnergy.ShiftTarget / 5;
                this.gaugeShiftEfficiency.DataContext = shiftWiseEnergy;
                scaleShiftEfficiency.Interval = 20;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteErrorLog("In LoadRunningShiftStatus" + ex.Message);
            }
        }

        private void DataRefreshTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                LoadRunningShiftStatus();
            }
            catch (Exception) { }
        }

        private void PackingDashboardAdmins_Unloaded(object sender, RoutedEventArgs e)
        {
            if (dataRefreshTimer != null && dataRefreshTimer.IsEnabled) dataRefreshTimer.Stop();
        }
    }
}
