using DeburringProcess_Peekay;
using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using DeburringProcess_Peekay.Mivin;
using DeburringProcess_Peekay.Models;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using Path = System.IO.Path;

namespace PackingModule_Rexroth.Mivin
{
    /// <summary>
    /// Interaction logic for PackingMainDashboard.xaml
    /// </summary>
    public partial class PackingMainDashboard : UserControl
    {
        int ScanResetTimeInterval = 300;
        string oldScanDataString = string.Empty;
        string currentStation = string.Empty;
        DateTime scanResetTime = DateTime.Now;
        Ping netMon = default(Ping);
        TcpClient tcpClient = default(TcpClient);
        ModbusIpMaster master = default(ModbusIpMaster);
        DispatcherTimer pumpScanTimer = null;
        DispatcherTimer dataRefreshTimer = null;
        RunningPumpModelEntity runningPumpModelData = null;
        ObservableCollection<RunningShiftSchedulesEntity> runningShiftSchedulesData = null;
        ObservableCollection<RunningModelStatusEntity> runningModelDetails = null;
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(parentWindow: Application.Current.MainWindow, corner: Corner.TopRight, offsetX: 10, offsetY: 10);
            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(notificationLifetime: TimeSpan.FromSeconds(2), maximumNotificationCount: MaximumNotificationCount.FromCount(5));
            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        public PackingMainDashboard()
        {
            InitializeComponent();
            currentStation = ConfigurationManager.AppSettings["OperatorStation"].ToString();
            if (ConfigurationManager.AppSettings["ScanResetTimeInterval"] != null)
                ScanResetTimeInterval = Convert.ToInt32(ConfigurationManager.AppSettings["ScanResetTimeInterval"]);
        }

        private void PackingDashboard_Control_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadRunningShiftStatus();
                if (!Utility.InstalledPCType.Equals("Client"))
                {
                    pumpScanTimer = new DispatcherTimer();
                    pumpScanTimer.Interval = TimeSpan.FromSeconds(1);
                    pumpScanTimer.Tick += PumpScanTimer_Tick;
                    if (dataRefreshTimer != null && dataRefreshTimer.IsEnabled) dataRefreshTimer.Stop();
                    btnForceClose.IsEnabled = true;
                    chkAutoPrint.IsEnabled = true;
                    if (pumpScanTimer != null && !pumpScanTimer.IsEnabled) pumpScanTimer.Start();
                }
                else
                {
                    int data_refresh_interval = Convert.ToInt32(ConfigurationManager.AppSettings["DataRefreshTimeInterval"].ToString());
                    dataRefreshTimer = new DispatcherTimer();
                    dataRefreshTimer.Interval = TimeSpan.FromSeconds(data_refresh_interval);
                    dataRefreshTimer.Tick += DataRefreshTimer_Tick;
                    if (pumpScanTimer != null && pumpScanTimer.IsEnabled) pumpScanTimer.Stop();
                    btnForceClose.IsEnabled = false;
                    chkAutoPrint.IsEnabled = false;
                    imgScan.IsEnabled = false;
                    if (dataRefreshTimer != null && !dataRefreshTimer.IsEnabled) dataRefreshTimer.Start();
                }
                if (Utility.CurrentEmployeeRole.Equals("Operator"))
                {
                    btnForceClose.Visibility = Visibility.Visible;
                    btnReprint.Visibility = Visibility.Visible;
                }
                else
                {
                    btnForceClose.Visibility = Visibility.Collapsed;
                    btnReprint.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteErrorLog("In PackingDashboard_Control_Loaded" + ex.Message);
            }
        }

        private void LoadRunningShiftStatus()
        {
            ShiftWiseEnergyInfo shiftWiseEnergy = new ShiftWiseEnergyInfo();
            List<ShiftWiseEnergyInfo> shiftWiseEnergyStationWise = new List<ShiftWiseEnergyInfo>();
            runningPumpModelData = new RunningPumpModelEntity();
            List<RunningPumpModelEntity> runningPumpModelDataStationWise = new List<RunningPumpModelEntity>();
            runningShiftSchedulesData = new ObservableCollection<RunningShiftSchedulesEntity>();
            runningModelDetails = new ObservableCollection<RunningModelStatusEntity>();
            try
            {
                runningShiftSchedulesData = MivinDataBaseAccess.GetRunningShiftSchedulesData(currentStation, "", out runningModelDetails, out shiftWiseEnergy, out runningPumpModelData);
                if (runningShiftSchedulesData != null && runningShiftSchedulesData.Count > 0)
                {
                    this.cmbPumpModelDetails.SelectionChanged -= new SelectionChangedEventHandler(this.cmbPumpModelDetails_SelectionChanged);
                    cmbPumpModelDetails.ItemsSource = runningShiftSchedulesData.Where(x => !x.PackingTarget.Equals(0)).OrderByDescending(x => Convert.ToDateTime(x.ScheduleDate));
                    cmbPumpModelDetails.DisplayMemberPath = "PumpInfo";
                    cmbPumpModelDetails.SelectedValuePath = "PumpModel";
                    if (runningModelDetails != null && runningModelDetails.Count > 0)
                    {
                        RunningModelDetailsGrid.ItemsSource = runningModelDetails;
                        cmbPumpModelDetails.SelectedValue = runningModelDetails.First().RunningPumpModel;
                        scanResetTime = DateTime.Now.AddSeconds(ScanResetTimeInterval);
                    }
                    else
                    {
                        cmbPumpModelDetails.SelectedIndex = -1;
                        RunningModelDetailsGrid.ItemsSource = new ObservableCollection<RunningModelStatusEntity>();
                    }
                    if (runningPumpModelData != null && !string.IsNullOrEmpty(runningPumpModelData.RunningPumpModel) && !string.IsNullOrEmpty(runningPumpModelData.ScheduleDate) && !string.IsNullOrEmpty(runningPumpModelData.PackagingType))
                    {
                        this.packingQuantityGrid.DataContext = null;
                        this.packingQuantityGrid.Visibility = Visibility.Visible;
                        this.packingQuantityGrid.DataContext = runningPumpModelData;
                    }
                    else
                    {
                        this.packingQuantityGrid.Visibility = Visibility.Collapsed;
                        this.packingQuantityGrid.DataContext = null;
                    }
                    this.gaugeShiftTargetVsActual.DataContext = shiftWiseEnergy;
                    scaleShiftTargetVsActual.Interval = shiftWiseEnergy.ShiftTarget < 100 ? 20 : shiftWiseEnergy.ShiftTarget / 5;
                    this.gaugeShiftEfficiency.DataContext = shiftWiseEnergy;
                    scaleShiftEfficiency.Interval = 20;
                    this.cmbPumpModelDetails.SelectionChanged += new SelectionChangedEventHandler(this.cmbPumpModelDetails_SelectionChanged);
                }
                else
                {
                    this.packingQuantityGrid.Visibility = Visibility.Collapsed;
                    DialogBox dlgInfo = new DialogBox("Information!!", "No running schedules available for shift.", false);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                this.packingQuantityGrid.Visibility = Visibility.Collapsed;
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteErrorLog("In LoadRunningShiftStatus" + ex.Message);
            }
        }

        private async void SetQrScannerStatus()
        {
            try
            {
                netMon = new Ping();
                PingReply reply = await netMon.SendPingAsync(QRScannerInfo.IpAddress, 10000);
                if (reply.Status == IPStatus.Success)
                {
                    try
                    {
                        tcpClient = new TcpClient(QRScannerInfo.IpAddress, int.Parse(QRScannerInfo.Port));
                        tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                        master = ModbusIpMaster.CreateIp(tcpClient);
                        master.Transport.Retries = 4;
                        master.Transport.ReadTimeout = 4000;
                        master.Transport.WriteTimeout = 4000;
                        master.Transport.WaitToRetryMilliseconds = 2000;
                        master.Transport.SlaveBusyUsesRetryCount = true;
                        if (tcpClient.Connected)
                        {
                            lblScannerStatus.Content = "QR SCANNER : CONNECTED";
                            lblScannerStatus.Foreground = System.Windows.Media.Brushes.DarkSeaGreen;
                            imgScannerStatus.Source = new BitmapImage(new Uri(Path.Combine(Utility.appPath, "Images", "ConnectionStatus", "NetworkOk.png")));
                        }
                        else
                        {
                            lblScannerStatus.Content = "QR SCANNER : NOT CONNECTED";
                            lblScannerStatus.Foreground = System.Windows.Media.Brushes.DarkRed;
                            imgScannerStatus.Source = new BitmapImage(new Uri(Path.Combine(Utility.appPath, "Images", "ConnectionStatus", "NetworkNotOk.png")));
                        }
                    }
                    catch (Exception)
                    {
                        lblScannerStatus.Content = "QR SCANNER : NOT CONNECTED";
                        lblScannerStatus.Foreground = System.Windows.Media.Brushes.DarkRed;
                        imgScannerStatus.Source = new BitmapImage(new Uri(Path.Combine(Utility.appPath, "Images", "ConnectionStatus", "NetworkNotOk.png")));
                    }
                }
                else
                {
                    lblScannerStatus.Content = "QR SCANNER : NOT CONNECTED";
                    lblScannerStatus.Foreground = System.Windows.Media.Brushes.DarkRed;
                    imgScannerStatus.Source = new BitmapImage(new Uri(Path.Combine(Utility.appPath, "Images", "ConnectionStatus", "NetworkNotOk.png")));
                    //MessageBox.Show(@"Not able to connect to QR scanner. Ping failed..", "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if (netMon != null) netMon.Dispose();
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void PumpScanTimer_Tick(object sender, EventArgs e)
        {
            ushort[] scanDataArray = null;
            string outputString = string.Empty;
            string scanDataString = string.Empty;
            bool IsPumpSlNoValid = false;
            PumpQRData scannedPumpData = null;
            pumpScanTimer.IsEnabled = false;
            try
            {
                SetQrScannerStatus();
                if (tcpClient != null && tcpClient.Connected)
                {
                    //scanDataArray = await master.ReadHoldingRegistersAsync(ushort.Parse(QRScannerInfo.RegisterAddress), ushort.Parse(QRScannerInfo.NumOfRegistersToRead));
                    scanDataArray = master.ReadHoldingRegisters(ushort.Parse(QRScannerInfo.RegisterAddress), ushort.Parse(QRScannerInfo.NumOfRegistersToRead));
                    scanDataString = GetString(scanDataArray).Trim(char.MinValue).Trim();
                    if (!string.IsNullOrEmpty(scanDataString.Trim()))
                    {
                        scanResetTime = DateTime.Now.AddSeconds(ScanResetTimeInterval);
                        if (scanDataString.Contains("-"))
                        {
                            string[] QrScanData = scanDataString.Split('-');
                            if (QrScanData.Length >= 2)
                            {
                                if (string.IsNullOrEmpty(oldScanDataString))
                                {
                                    //Logger.WriteDebugLog("Scanned data = " + scanDataString);
                                    string pumpModel = QrScanData[0];
                                    string pumpSlNo = QrScanData[QrScanData.Length - 1];
                                    IsPumpSlNoValid = MivinDataBaseAccess.ValidatePumpSerialNumber(pumpModel, pumpSlNo);
                                    if (!IsPumpSlNoValid) oldScanDataString = scanDataString;
                                }
                                if (scanDataString != oldScanDataString)
                                {
                                    Logger.WriteDebugLog("Scanned data = " + scanDataString);
                                    scannedPumpData = new PumpQRData();
                                    scannedPumpData.PumpModel = QrScanData[0];
                                    scannedPumpData.PumpSerialNum = QrScanData[QrScanData.Length - 1];
                                    ProcessScannedQRData(scannedPumpData);
                                    oldScanDataString = scanDataString;
                                }
                            }
                            else
                            {
                                if (scanDataString != oldScanDataString)
                                {
                                    DialogBox dlgError = new DialogBox("Error!!", "Wrong QR code scanned. QR data is not valid.", true);
                                    dlgError.Owner = Window.GetWindow(this);
                                    dlgError.ShowDialog();
                                    //notifier.ShowError("Wrong QR code scanned. QR data is not valid.");
                                    oldScanDataString = scanDataString;
                                }
                            }
                        }
                        else
                        {
                            if (scanDataString != oldScanDataString)
                            {
                                DialogBox dlgError = new DialogBox("Error!!", "Wrong QR code scanned. QR data is not valid.", true);
                                dlgError.Owner = Window.GetWindow(this);
                                dlgError.ShowDialog();
                                //notifier.ShowError("Wrong QR code scanned. QR data is not valid.");
                                oldScanDataString = scanDataString;
                            }
                        }
                    }
                }
                if (!pumpScanTimer.IsEnabled) pumpScanTimer.IsEnabled = true;
                if (scanResetTime <= DateTime.Now)
                {
                    var currentRunningModelData = RunningModelDetailsGrid.ItemsSource as ObservableCollection<RunningModelStatusEntity>;
                    if (currentRunningModelData != null && currentRunningModelData.Count > 0)
                    {
                        int scannedQty = currentRunningModelData.First().ScannedQuantity;
                        if (!scannedQty.Equals(0))
                        {
                            MivinDataBaseAccess.DeleteScannedPumpDetails(currentRunningModelData.First());
                        }
                        LoadRunningShiftStatus();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
            finally
            {
                if (tcpClient != null)
                {
                    tcpClient.Client.Close();
                    tcpClient.Dispose();
                }
                if (!pumpScanTimer.IsEnabled) pumpScanTimer.Start();
            }
        }

        private void imgScan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool IsPumpScanned = false;
            int QtyPerBox = 0, ScannedQty = 0, TotalScannedQty = 0, PackingTarget = 0;
            PumpQRData scannedPumpData = new PumpQRData();
            try
            {
                var currentRunningModelData = RunningModelDetailsGrid.ItemsSource as ObservableCollection<RunningModelStatusEntity>;
                if (currentRunningModelData != null && currentRunningModelData.Count > 0)
                {
                    PackingTarget = GetPackingTargetForWorkOrder(currentRunningModelData.First());
                    if (PackingTarget.Equals(0))
                    {
                        DialogBox dlgError = new DialogBox("Error!!", "Unknown error. Could not get packing target.", true);
                        dlgError.Owner = Window.GetWindow(this);
                        dlgError.ShowDialog();
                        return;
                    }
                    if (string.IsNullOrEmpty(currentRunningModelData.First().WorkOrderStatus) || currentRunningModelData.First().WorkOrderStatus.Equals("Hold", StringComparison.OrdinalIgnoreCase))
                    {
                        DialogBox dlgError = new DialogBox("Error!!", "Cannot scan QR data when running pump model is on hold.", true);
                        dlgError.Owner = Window.GetWindow(this);
                        dlgError.ShowDialog();
                        return;
                    }
                    QtyPerBox = currentRunningModelData.First().QuantityPerBox;
                    ScannedQty = currentRunningModelData.First().ScannedQuantity;
                    TotalScannedQty = currentRunningModelData.First().TotalScannedQuantity;
                    if(QtyPerBox.Equals(ScannedQty))
                    {
                        MivinDataBaseAccess.UpdateScannedPumpDetails(currentRunningModelData.First().StationID, QtyPerBox, ScannedQty);
                    }
                    PumpDetails pumpDetails = new PumpDetails();
                    pumpDetails.Owner = Window.GetWindow(this);
                    pumpDetails.ShowDialog();
                    scannedPumpData = PumpDetails.SelectedPumpDetails;
                    if (string.IsNullOrEmpty(scannedPumpData.PumpModel) || string.IsNullOrEmpty(scannedPumpData.PumpSerialNum))
                    {
                        DialogBox dlgError = new DialogBox("Error!!", "Wrong QR code scanned. No data in QR code.", true);
                        dlgError.Owner = Window.GetWindow(this);
                        dlgError.ShowDialog();
                        return;
                    }
                    if (!string.IsNullOrEmpty(currentRunningModelData.First().RunningPumpModel))
                    {
                        if (!currentRunningModelData.First().RunningPumpModel.Equals(scannedPumpData.PumpModel))
                        {
                            DialogBox dlgError = new DialogBox("Error!!", "Wrong QR code scanned. Pump model number is not same as running model.", true);
                            dlgError.Owner = Window.GetWindow(this);
                            dlgError.ShowDialog();
                            return;
                        }
                        if (!MivinDataBaseAccess.ValidatePumpSerialNumber(scannedPumpData.PumpModel, scannedPumpData.PumpSerialNum))
                        {
                            DialogBox dlgError = new DialogBox("Error!!", "Duplicate serial number. This serial number is already scanned for running model.", true);
                            dlgError.Owner = Window.GetWindow(this);
                            dlgError.ShowDialog();
                            return;
                        }
                        if (ScannedQty < QtyPerBox)
                        {
                            IsPumpScanned = MivinDataBaseAccess.UpdateScannedPumpDetails(currentRunningModelData.First(), 1, LoginPage.LoginUserName, 0, scannedPumpData.PumpSerialNum, "");
                        }
                        if (ScannedQty.Equals(QtyPerBox))
                        {
                            IsPumpScanned = MivinDataBaseAccess.UpdateScannedPumpDetails(currentRunningModelData.First(), 1, LoginPage.LoginUserName, 0, scannedPumpData.PumpSerialNum, "");
                        }

                        if (IsPumpScanned)
                        {
                            if (ScannedQty.Equals(QtyPerBox - 1))
                            {
                                if (!(bool)chkAutoPrint.IsChecked)
                                {
                                    DialogBoxYesNo dlgConfirm = new DialogBoxYesNo("Print Confirmation", "Box is full. Do you want to print the data ?");
                                    dlgConfirm.Owner = Window.GetWindow(this);
                                    dlgConfirm.ShowDialog();
                                    if (Utility.YesNoAnswer)
                                    {
                                        PrintPackedBoxDetails(currentRunningModelData.First(), true);
                                    }
                                }
                                if ((bool)chkAutoPrint.IsChecked)
                                {
                                    PrintPackedBoxDetails(currentRunningModelData.First(), true);
                                }
                            }
                            if (TotalScannedQty.Equals(PackingTarget - 1))
                            {
                                bool IsWorkOrderCompleted = MivinDataBaseAccess.UpdateScheduleStatus(currentRunningModelData.First(), "Completed");
                                if (IsWorkOrderCompleted)
                                {
                                    DialogBox dlgInfo = new DialogBox("Inforrmation!!", "Work order completed. All boxes have been packed for work order.", false);
                                    dlgInfo.Owner = Window.GetWindow(this);
                                    dlgInfo.ShowDialog();
                                }
                            }
                            LoadRunningShiftStatus();
                        }
                        else
                        {
                            DialogBox dlgError = new DialogBox("Error!!", "Pump scan data not saved.", true);
                            dlgError.Owner = Window.GetWindow(this);
                            dlgError.ShowDialog();
                        }
                    }
                    else
                    {
                        DialogBox dlgInfo = new DialogBox("Information", "No current running pump model found.", false);
                        dlgInfo.Owner = Window.GetWindow(this);
                        dlgInfo.ShowDialog();
                    }
                }
                else
                {
                    DialogBox dlgInfo = new DialogBox("Information", "No current running schedule found.", false);
                    dlgInfo.Owner = Window.GetWindow(this);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteErrorLog("In imgScan_MouseDown" + ex.Message);
            }
        }

        void ProcessScannedQRData(PumpQRData scannedPumpData)
        {
            bool IsPumpScanned = false;
            int QtyPerBox = 0, ScannedQty = 0, TotalScannedQty = 0, PackingTarget = 0;
            try
            {
                var currentRunningModelData = RunningModelDetailsGrid.ItemsSource as ObservableCollection<RunningModelStatusEntity>;
                if (currentRunningModelData != null && currentRunningModelData.Count > 0)
                {
                    PackingTarget = GetPackingTargetForWorkOrder(currentRunningModelData.FirstOrDefault());
                    if (PackingTarget.Equals(0))
                    {
                        DialogBox dlgError = new DialogBox("Error!!", "Unknown error. Could not get packing target.", true);
                        dlgError.Owner = Window.GetWindow(this);
                        dlgError.ShowDialog();
                        return;
                    }
                    if (string.IsNullOrEmpty(currentRunningModelData.First().WorkOrderStatus) || currentRunningModelData.First().WorkOrderStatus.Equals("Hold", StringComparison.OrdinalIgnoreCase))
                    {
                        DialogBox dlgError = new DialogBox("Error!!", "Cannot scan QR data when running pump model is on hold.", true);
                        dlgError.Owner = Window.GetWindow(this);
                        dlgError.ShowDialog();
                        return;
                    }
                    QtyPerBox = currentRunningModelData.First().QuantityPerBox;
                    ScannedQty = currentRunningModelData.First().ScannedQuantity;
                    TotalScannedQty = currentRunningModelData.First().TotalScannedQuantity;
                    if (QtyPerBox.Equals(ScannedQty))
                    {
                        MivinDataBaseAccess.UpdateScannedPumpDetails(currentRunningModelData.First().StationID, QtyPerBox, ScannedQty);
                    }
                    if (string.IsNullOrEmpty(scannedPumpData.PumpModel) || string.IsNullOrEmpty(scannedPumpData.PumpSerialNum))
                    {
                        DialogBox dlgError = new DialogBox("Error!!", "Wrong QR code scanned. No data in QR code.", true);
                        dlgError.Owner = Window.GetWindow(this);
                        dlgError.ShowDialog();
                        return;
                    }
                    if (!string.IsNullOrEmpty(currentRunningModelData.First().RunningPumpModel))
                    {
                        if (!currentRunningModelData.First().RunningPumpModel.Equals(scannedPumpData.PumpModel))
                        {
                            DialogBox dlgError = new DialogBox("Error!!", "Wrong QR code scanned. Pump model number is not same as running model.", true);
                            dlgError.Owner = Window.GetWindow(this);
                            dlgError.ShowDialog();
                            //notifier.ShowError("Wrong QR code scanned. Pump model number is not same as running model.");
                            return;
                        }
                        if (!MivinDataBaseAccess.ValidatePumpSerialNumber(scannedPumpData.PumpModel, scannedPumpData.PumpSerialNum))
                        {
                            DialogBox dlgError = new DialogBox("Error!!", "Duplicate serial number. This serial number is already scanned for running model.", true);
                            dlgError.Owner = Window.GetWindow(this);
                            dlgError.ShowDialog();
                            //notifier.ShowError("Duplicate serial number. This serial number is already scanned for running model.");
                            return;
                        }
                        if (ScannedQty < QtyPerBox)
                        {
                            IsPumpScanned = MivinDataBaseAccess.UpdateScannedPumpDetails(currentRunningModelData.First(), 1, LoginPage.LoginUserName, 0, scannedPumpData.PumpSerialNum, "");
                        }
                        if (ScannedQty.Equals(QtyPerBox))
                        {
                            IsPumpScanned = MivinDataBaseAccess.UpdateScannedPumpDetails(currentRunningModelData.First(), 1, LoginPage.LoginUserName, 0, scannedPumpData.PumpSerialNum, "");
                        }

                        if (IsPumpScanned)
                        {
                            if (ScannedQty.Equals(QtyPerBox - 1))
                            {
                                if (!(bool)chkAutoPrint.IsChecked)
                                {
                                    DialogBoxYesNo dlgConfirm = new DialogBoxYesNo("Print Confirmation", "Box is full. Do you want to print the data ?");
                                    dlgConfirm.Owner = Window.GetWindow(this);
                                    dlgConfirm.ShowDialog();
                                    if (Utility.YesNoAnswer)
                                    {
                                        PrintPackedBoxDetails(currentRunningModelData.First(), true);
                                    }
                                }
                                if ((bool)chkAutoPrint.IsChecked)
                                {
                                    PrintPackedBoxDetails(currentRunningModelData.First(), true);
                                }
                            }
                            if (TotalScannedQty.Equals(PackingTarget - 1))
                            {
                                bool IsWorkOrderCompleted = MivinDataBaseAccess.UpdateScheduleStatus(currentRunningModelData.First(), "Completed");
                                if (IsWorkOrderCompleted)
                                {
                                    MivinDataBaseAccess.DeleteRunningScheduleDetails(currentRunningModelData.First().StationID);
                                    DialogBox dlgInfo = new DialogBox("Inforrmation!!", "Schedule completed. All boxes have been packed for work order.", false);
                                    dlgInfo.Owner = Window.GetWindow(this);
                                    dlgInfo.ShowDialog();
                                }
                            }
                        }
                        else
                        {
                            DialogBox dlgError = new DialogBox("Error!!", "Pump scan data not saved.", true);
                            dlgError.Owner = Window.GetWindow(this);
                            dlgError.ShowDialog();
                        }
                    }
                    else
                    {
                        DialogBox dlgInfo = new DialogBox("Information", "No current running pump model found.", false);
                        dlgInfo.Owner = Window.GetWindow(this);
                        dlgInfo.ShowDialog();
                    }
                }
                else
                {
                    DialogBox dlgInfo = new DialogBox("Information", "No current running schedule found.", false);
                    dlgInfo.Owner = Window.GetWindow(this);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteErrorLog("In ProcessScannedQRData 1 : " + ex.ToString());
                Logger.WriteErrorLog("In ProcessScannedQRData2 : " + ex.StackTrace != null ? ex.StackTrace.ToString() : ex.Source.ToString());
            }
            finally
            {
                LoadRunningShiftStatus();
            }
        }

        private int GetPackingTargetForWorkOrder(RunningModelStatusEntity runningModelStatusEntity)
        {
            int PckTarget = 0;
            try
            {
                ObservableCollection<RunningShiftSchedulesEntity> CurrentShiftSchedules = runningShiftSchedulesData as ObservableCollection<RunningShiftSchedulesEntity>;
                if (CurrentShiftSchedules != null && CurrentShiftSchedules.Count > 0)
                {
                    PckTarget = CurrentShiftSchedules.Where(x => x.PumpModel.Equals(runningModelStatusEntity.RunningPumpModel) && x.PackagingType.Equals(runningModelStatusEntity.PackagingType) && x.ScheduleDate.Equals(runningModelStatusEntity.ScheduleDate)).First().PackingTarget;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteErrorLog("In GetPackingTargetForWorkOrder" + ex.Message);
            }
            return PckTarget;
        }

        private void btnHold_Click(object sender, RoutedEventArgs e)
        {
            bool IsHoldSuccessful = false;
            try
            {
                var currentRunningModelData = RunningModelDetailsGrid.SelectedItem as RunningModelStatusEntity;
                if (currentRunningModelData != null)
                {
                    if (!currentRunningModelData.WorkOrderStatus.Equals("Hold", StringComparison.OrdinalIgnoreCase))
                    {
                        DialogBoxYesNo dialogBoxYesNo = new DialogBoxYesNo("Confirmation Dialog!!", "Are you sure you really want to put this schedule on hold ?");
                        dialogBoxYesNo.Owner = Window.GetWindow(this);
                        dialogBoxYesNo.ShowDialog();
                        if (Utility.YesNoAnswer)
                        {
                            IsHoldSuccessful = MivinDataBaseAccess.UpdateScheduleStatus(currentRunningModelData, "Hold");
                            if (IsHoldSuccessful)
                            {
                                MivinDataBaseAccess.DeleteRunningScheduleDetails(currentRunningModelData.StationID);
                                DialogBox dlgSuccess = new DialogBox("Success!!", "Schedule successfully put on hold.", false);
                                dlgSuccess.Owner = Window.GetWindow(this);
                                dlgSuccess.ShowDialog();
                                LoadRunningShiftStatus();
                            }
                            else
                            {
                                DialogBox dlgError = new DialogBox("Error!!", "Error. Can't put this running model on hold.", true);
                                dlgError.Owner = Window.GetWindow(this);
                                dlgError.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        DialogBox dlgInfo = new DialogBox("Information", "Cannot hold a running model which is already in hold.", false);
                        dlgInfo.Owner = Window.GetWindow(this);
                        dlgInfo.ShowDialog();
                    }
                }
                else
                {
                    DialogBox dlgInfo = new DialogBox("Information", "No current running pump model found.", false);
                    dlgInfo.Owner = Window.GetWindow(this);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnForceClose_Click(object sender, RoutedEventArgs e)
        {
            string PackagingType = string.Empty;
            try
            {
                if (RunningModelDetailsGrid.ItemsSource is ObservableCollection<RunningModelStatusEntity> currentRunningModelData && currentRunningModelData.Count > 0)
                {
                    if (currentRunningModelData.First().WorkOrderStatus.Equals("In Progress", StringComparison.OrdinalIgnoreCase))
                    {
                        int PackingTarget = GetPackingTargetForWorkOrder(currentRunningModelData.First());
                        DialogBoxYesNo dialogBoxYesNo = new DialogBoxYesNo("Confirmation Dialog!!", "Are you sure you really want to force close this box ?");
                        dialogBoxYesNo.Owner = Window.GetWindow(this);
                        dialogBoxYesNo.ShowDialog();
                        if (Utility.YesNoAnswer)
                        {
                            if (currentRunningModelData.First().ScannedQuantity != 0)
                            {
                                bool IsSuccessfullyClosed = MivinDataBaseAccess.UpdatePumpBoxForceCloseStatus(currentRunningModelData.First(), 1);
                                if (IsSuccessfullyClosed)
                                {
                                    DialogBox dlgSuccess = new DialogBox("Success!!", "Box successfully closed.", false);
                                    dlgSuccess.Owner = Window.GetWindow(this);
                                    dlgSuccess.ShowDialog();
                                    if (!(bool)chkAutoPrint.IsChecked)
                                    {
                                        DialogBoxYesNo dlgConfirm = new DialogBoxYesNo("Print Confirmation", "Box is full. Do you want to print the data ?");
                                        dlgConfirm.Owner = Window.GetWindow(this);
                                        dlgConfirm.ShowDialog();
                                        if (Utility.YesNoAnswer)
                                        {
                                            PrintPackedBoxDetails(currentRunningModelData.First(), false);
                                        }
                                    }
                                    else
                                    {
                                        PrintPackedBoxDetails(currentRunningModelData.First(), false);
                                    }
                                    LoadRunningShiftStatus();
                                }
                                else
                                {
                                    DialogBox dlgError = new DialogBox("Error!!", "Error. Can't close this box.", true);
                                    dlgError.Owner = Window.GetWindow(this);
                                    dlgError.ShowDialog();
                                }
                            }
                            else
                            {
                                DialogBox dlgError = new DialogBox("Error!!", "Error. Can't close an empty box with no pumps.", true);
                                dlgError.Owner = Window.GetWindow(this);
                                dlgError.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        DialogBox dlgInfo = new DialogBox("Information", "Only in progress box can be force closed.", false);
                        dlgInfo.Owner = Window.GetWindow(this);
                        dlgInfo.ShowDialog();
                    }
                }
                else
                {
                    DialogBox dlgInfo = new DialogBox("Information", "No current running pump model found.", false);
                    dlgInfo.Owner = Window.GetWindow(this);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintPackedBoxDetails(RunningModelStatusEntity runningModelStatusEntity, bool auto_print = false)
        {
            bool IsPrinterValid = false;
            string DestPath = string.Empty;
            try
            {
                string PrinterServer = ConfigurationManager.AppSettings["PrinterServer"].ToString();
                string PrinterName = ConfigurationManager.AppSettings["PrinterName"].ToString();
                string templateFileName = System.IO.Path.Combine(Utility.appPath, "Mivin", "PrintPRNFile", "Rexroth.prn");
                DestPath = GenerateDestinationPrintFile(runningModelStatusEntity, templateFileName, auto_print);
                if (!string.IsNullOrEmpty(DestPath) && File.Exists(DestPath))
                {
                    try
                    {
                        PrintDocument pd = new PrintDocument();
                        pd.PrinterSettings.PrinterName = @PrinterName;
                        IsPrinterValid = pd.PrinterSettings.IsValid;
                        if (IsPrinterValid)
                        {
                            bool success = PrinterHelper.SendFileToPrinter(PrinterName, DestPath);
                            if (success)
                                Logger.WriteDebugLog(string.Format("Printing Successfull for Pump Model :{0}", runningModelStatusEntity.RunningPumpModel));
                        }
                        else
                        {
                            DialogBox dlgInfo = new DialogBox("Information", "Can't connect to printer. Printer is either invalid or offline.", false);
                            dlgInfo.Owner = Window.GetWindow(this);
                            dlgInfo.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteErrorLog("Printing Error : " + ex.Message);
                    }
                    finally
                    {
                        //if (File.Exists(DestPath)) File.Delete(DestPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Printing Error : " + ex.Message);
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string GenerateDestinationPrintFile(RunningModelStatusEntity runningModelData, string sourcePath, bool autoprint = false)
        {
            string DestFileName = string.Empty;
            string destFolder = string.Empty;
            string destFilePath = string.Empty;
            try
            {
                DestFileName = "Rexroth" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".prn";
                destFolder = System.IO.Path.Combine(Utility.appPath, "Mivin", "PrintPRNFile", "GeneratedFiles");
                if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                destFilePath = System.IO.Path.Combine(destFolder, DestFileName);
                string[] fileContent = File.ReadAllLines(sourcePath);
                for (int i = 0; i < fileContent.Length; i++)
                {
                    if (fileContent[i].Contains("803402048A")) fileContent[i] = fileContent[i].Replace("803402048A", runningModelData.CustomerModel);
                    else if (fileContent[i].Contains("06Nos")) fileContent[i] = fileContent[i].Replace("06Nos", (autoprint ? (runningModelData.ScannedQuantity + 1) : runningModelData.ScannedQuantity) + "Nos.");
                    else if (fileContent[i].Contains("AG")) fileContent[i] = fileContent[i].Replace("AG", runningModelData.PackagingType);
                    else if (fileContent[i].Contains("R983035458")) fileContent[i] = fileContent[i].Replace("R983035458", runningModelData.RunningPumpModel);
                    else if (fileContent[i].Contains("02-11-20")) fileContent[i] = fileContent[i].Replace("02-11-20", DateTime.Now.ToString("dd-MM-yy"));
                }
                File.WriteAllLines(destFilePath, fileContent);
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while generating print prn file : " + ex.Message);
            }
            return destFilePath;
        }

        private static string GetString(ushort[] data)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                foreach (ushort i in data)
                {
                    byte[] byteArray = BitConverter.GetBytes(i);
                    byte temp = byteArray[0];
                    byteArray[0] = byteArray[1];
                    byteArray[1] = temp;
                    str.Append(Encoding.UTF8.GetString(byteArray));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog("Error while reading string value from QR scanner - " + ex.Message);
            }
            return str.ToString();
        }

        private void btnReprint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentRunningModelData = RunningModelDetailsGrid.ItemsSource as ObservableCollection<RunningModelStatusEntity>;
                if (currentRunningModelData != null && currentRunningModelData.Count > 0)
                {
                    ReprintPackingDetails reprintDetails = new ReprintPackingDetails(currentRunningModelData.First());
                    reprintDetails.Owner = Window.GetWindow(this);
                    reprintDetails.ShowDialog();
                }
                else
                {
                    DialogBox dlgInfo = new DialogBox("Information!!", "No running schedule found", false);
                    dlgInfo.Owner = Window.GetWindow(this);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PackingDashboardMainGrid_GotFocus(object sender, RoutedEventArgs e)
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

        private void PackingDashboardMainGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                e.Handled = true;
        }

        private void PackingDashboard_Control_Unloaded(object sender, RoutedEventArgs e)
        {
            if (pumpScanTimer != null && pumpScanTimer.IsEnabled) pumpScanTimer.Stop();
            if (dataRefreshTimer != null && dataRefreshTimer.IsEnabled) dataRefreshTimer.Stop();
        }

        private void cmbPumpModelDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool success = false;
            RunningShiftSchedulesEntity selectedPumpModelData = null;
            try
            {
                if (cmbPumpModelDetails.SelectedItem != null)
                {
                    var currentRunningModelData = RunningModelDetailsGrid.ItemsSource as ObservableCollection<RunningModelStatusEntity>;
                    DialogBoxYesNo dialogBoxYesNo = new DialogBoxYesNo("Confirmation Dialog!!", "Do you really want to set this as running pump model ?");
                    dialogBoxYesNo.Owner = Window.GetWindow(this);
                    dialogBoxYesNo.ShowDialog();
                    if (Utility.YesNoAnswer)
                    {
                        selectedPumpModelData = cmbPumpModelDetails.SelectedItem as RunningShiftSchedulesEntity;
                        if (currentRunningModelData != null && currentRunningModelData.Count > 0 && selectedPumpModelData != null)
                        {
                            if (currentRunningModelData.First().WorkOrderStatus.Equals("In Progress", StringComparison.OrdinalIgnoreCase))
                            {
                                if (currentRunningModelData.First().RunningPumpModel.Equals(selectedPumpModelData.PumpModel) && currentRunningModelData.First().PackagingType.Equals(selectedPumpModelData.PackagingType) && currentRunningModelData.First().ScheduleDate.Equals(selectedPumpModelData.ScheduleDate))
                                {
                                    this.cmbPumpModelDetails.SelectionChanged -= new SelectionChangedEventHandler(this.cmbPumpModelDetails_SelectionChanged);
                                    cmbPumpModelDetails.SelectedValue = currentRunningModelData.First().RunningPumpModel;
                                    MessageBox.Show("Selected schedule is already running.", "Information!!", MessageBoxButton.OK, MessageBoxImage.Information);
                                    this.cmbPumpModelDetails.SelectionChanged += new SelectionChangedEventHandler(this.cmbPumpModelDetails_SelectionChanged);
                                    return;
                                }
                                else
                                {
                                    this.cmbPumpModelDetails.SelectionChanged -= new SelectionChangedEventHandler(this.cmbPumpModelDetails_SelectionChanged);
                                    cmbPumpModelDetails.SelectedValue = currentRunningModelData.First().RunningPumpModel;
                                    MessageBox.Show("Cannot set new running schedule when a schedule is already in progress.", "Information!!", MessageBoxButton.OK, MessageBoxImage.Information);
                                    this.cmbPumpModelDetails.SelectionChanged += new SelectionChangedEventHandler(this.cmbPumpModelDetails_SelectionChanged);
                                    return;
                                }
                            }
                        }
                        MivinDataBaseAccess.SetNewRunningSchedule(selectedPumpModelData, out success);
                        if (success)
                        {
                            LoadRunningShiftStatus();
                        }
                        else
                        {
                            MessageBox.Show("Error setting new running schedule.", "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        if (currentRunningModelData != null && currentRunningModelData.Count > 0)
                        {
                            this.cmbPumpModelDetails.SelectionChanged -= new SelectionChangedEventHandler(this.cmbPumpModelDetails_SelectionChanged);
                            cmbPumpModelDetails.SelectedValue = currentRunningModelData.First().RunningPumpModel;
                            this.cmbPumpModelDetails.SelectionChanged += new SelectionChangedEventHandler(this.cmbPumpModelDetails_SelectionChanged);
                        }
                        else
                            cmbPumpModelDetails.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dtpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtpDate.SelectedDate.Value != null)
                {
                    cmbPumpModelDetails.ItemsSource = runningShiftSchedulesData.Where(x => x.ScheduleDate.Equals(dtpDate.SelectedDate.Value.ToString("yyyy-MM-dd HH:mm:ss"))).Where(x => !x.PackingTarget.Equals(0)).OrderByDescending(x => Convert.ToDateTime(x.ScheduleDate));
                    cmbPumpModelDetails.DisplayMemberPath = "PumpInfo";
                    cmbPumpModelDetails.SelectedValuePath = "PumpModel";
                }
                else
                {
                    cmbPumpModelDetails.ItemsSource = runningShiftSchedulesData.Where(x => !x.PackingTarget.Equals(0)).OrderByDescending(x => Convert.ToDateTime(x.ScheduleDate));
                    cmbPumpModelDetails.DisplayMemberPath = "PumpInfo";
                    cmbPumpModelDetails.SelectedValuePath = "PumpModel";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadRunningShiftStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
