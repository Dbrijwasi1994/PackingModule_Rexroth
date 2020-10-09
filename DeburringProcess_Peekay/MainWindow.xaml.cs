using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DeburringProcess_Peekay.Models;
using DeburringProcess_Peekay.Helpers;
using System.Configuration;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Mivin;
using PackingModule_Rexroth.Mivin;

namespace DeburringProcess_Peekay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DateTime last = new DateTime(0);
        List<string> _barcode = new List<string>(10);
        List<string> code = new List<string>(10);
        string msg = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                lblLoginUser.Content = LoginPage.LoginUserName.ToUpper();
                txtEmpName.Content = LoginPage.LoginUserName;
                txtDate.Text = DateTime.Now.ToShortDateString();
                GridLength grd = new GridLength(240, GridUnitType.Pixel);
                grdClmn_menu.Width = grd;
                txtMasterData.Visibility = Visibility.Visible;
                txtReports.Visibility = Visibility.Visible;
                lblPackingDashboard.Visibility = Visibility.Visible;
                lbl_ShiftDetails.Visibility = Visibility.Visible;
                lbl_EmpInfo.Visibility = Visibility.Visible;
                lbl_CustomerInfo.Visibility = Visibility.Visible;
                lbl_PumpInformation.Visibility = Visibility.Visible;
                lbl_ScheduleMasterImport.Visibility = Visibility.Visible;
                lbl_ModelTargetMaster.Visibility = Visibility.Visible;
                lbl_PumpDispatchReport.Visibility = Visibility.Visible;
                lbl_PackingEfficiencyReport.Visibility = Visibility.Visible;
                txtUser.Width = 100;
                btn_hamburgerMenu.Width = 100;
                btn_hamburgerMenu.Height = 100;
                brdCanvas.Visibility = Visibility.Visible;
                if (Utility.CurrentEmployeeRole.Contains("Admin"))
                {
                    btnShiftDetails.Visibility = Visibility.Visible;
                }
                else
                {
                    btnShiftDetails.Visibility = Visibility.Collapsed;
                }
                if (Utility.CurrentEmployeeRole.Equals("Operator"))
                    reportsMenu.Visibility = Visibility.Collapsed;
                else
                    reportsMenu.Visibility = Visibility.Visible;
                btnPackingDashboard_Click(null, null);
                btnCustomerInfo.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private PathDetails GetAllPaths()
        {
            return DatabaseAccess.GetAllPaths();
        }

        private void btnLogout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginPage log = new LoginPage();
            log.Show();
            this.Close();
        }

        private void btn_hamburgerMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (btn_hamburgerMenu.Width != 60)
            {
                GridLength grd = new GridLength(80, GridUnitType.Pixel);
                grdClmn_menu.Width = grd;
                txtMasterData.Visibility = Visibility.Hidden;
                txtReports.Visibility = Visibility.Hidden;
                lblPackingDashboard.Visibility = Visibility.Hidden;
                lbl_ShiftDetails.Visibility = Visibility.Hidden;
                lbl_EmpInfo.Visibility = Visibility.Hidden;
                lbl_CustomerInfo.Visibility = Visibility.Hidden;
                lbl_PumpInformation.Visibility = Visibility.Hidden;
                lbl_ScheduleMasterImport.Visibility = Visibility.Hidden;
                lbl_ModelTargetMaster.Visibility = Visibility.Hidden;
                lbl_PumpDispatchReport.Visibility = Visibility.Hidden;
                lbl_PackingEfficiencyReport.Visibility = Visibility.Hidden;
                txtUser.Width = 0;
                btn_hamburgerMenu.Width = 60;
                btn_hamburgerMenu.Height = 60;
                brdCanvas.Visibility = Visibility.Hidden;
                menu_altlabel.Visibility = Visibility.Hidden;
                lblLoginUser.Visibility = lblLoginUser.Content.ToString().Length > 4 ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                GridLength grd = new GridLength(240, GridUnitType.Pixel);
                grdClmn_menu.Width = grd;
                txtMasterData.Visibility = Visibility.Visible;
                txtReports.Visibility = Visibility.Visible;
                lblPackingDashboard.Visibility = Visibility.Visible;
                lbl_ShiftDetails.Visibility = Visibility.Visible;
                lbl_EmpInfo.Visibility = Visibility.Visible;
                lbl_CustomerInfo.Visibility = Visibility.Visible;
                lbl_PumpInformation.Visibility = Visibility.Visible;
                lbl_ScheduleMasterImport.Visibility = Visibility.Visible;
                lbl_ModelTargetMaster.Visibility = Visibility.Visible;
                lbl_PumpDispatchReport.Visibility = Visibility.Visible;
                lbl_PackingEfficiencyReport.Visibility = Visibility.Visible;
                txtUser.Width = 100;
                btn_hamburgerMenu.Width = 100;
                btn_hamburgerMenu.Height = 100;
                brdCanvas.Visibility = Visibility.Visible;
                menu_altlabel.Visibility = Visibility.Visible;
                lblLoginUser.Visibility = Visibility.Visible;
            }
        }

        private void btnPackingDashboard_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            //PackingDashboard packingDashhboard = new PackingDashboard();
            UIPanel.Children.Clear();
            if (Utility.CurrentEmployeeRole.Equals("Operator"))
            {
                string currentStation = ConfigurationManager.AppSettings["OperatorStation"] != null ? ConfigurationManager.AppSettings["OperatorStation"].ToString().ToUpper() : "";
                if(!string.IsNullOrEmpty(currentStation))
                {
                    txtStation.Text = currentStation;
                    PackingMainDashboard packingDashhboard = new PackingMainDashboard();
                    UIPanel.Children.Add(packingDashhboard);
                }
                else
                {
                    DialogBox dlgInfo = new DialogBox("Error!!", "No station configured for the operator.", true);
                    dlgInfo.ShowDialog();
                    return;
                }
            }
            else
            {
                PackingMainDashboard_Admins packingDashhboard = new PackingMainDashboard_Admins();
                UIPanel.Children.Add(packingDashhboard);
            }
            txtTitle.Text = "Packing Dashboard".ToUpper();
            stkPackingDashboard.Background = Brushes.DarkSalmon;
            stkShiftDetails.Background = Brushes.Transparent;
            stkEmpInfo.Background = Brushes.Transparent;
            stkCustomerInfo.Background = Brushes.Transparent;
            stkPumpInformation.Background = Brushes.Transparent;
            stkScheduleMasterImport.Background = Brushes.Transparent;
            stkModelTargetMaster.Background = Brushes.Transparent;
            stkPumpDispatchReport.Background = Brushes.Transparent;
            stkPackingEfficiencyReport.Background = Brushes.Transparent;
            stkMonthlyFulfilmentReport.Background = Brushes.Transparent;
            Mouse.OverrideCursor = null;
        }

        private void btnShiftDetails_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            ShiftDetails shiftdetails = new ShiftDetails();
            UIPanel.Children.Clear();
            UIPanel.Children.Add(shiftdetails);
            txtTitle.Text = "Shift Details".ToUpper();
            stkPackingDashboard.Background = Brushes.Transparent;
            stkShiftDetails.Background = Brushes.DarkSalmon;
            stkEmpInfo.Background = Brushes.Transparent;
            stkCustomerInfo.Background = Brushes.Transparent;
            stkPumpInformation.Background = Brushes.Transparent;
            stkScheduleMasterImport.Background = Brushes.Transparent;
            stkModelTargetMaster.Background = Brushes.Transparent;
            stkPumpDispatchReport.Background = Brushes.Transparent;
            stkPackingEfficiencyReport.Background = Brushes.Transparent;
            stkMonthlyFulfilmentReport.Background = Brushes.Transparent;
            Mouse.OverrideCursor = null;
        }

        private void btnEmpInfo_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            EmployeeInformation empInfo = new EmployeeInformation();
            UIPanel.Children.Clear();
            UIPanel.Children.Add(empInfo);
            txtTitle.Text = "Employee Information".ToUpper();
            stkPackingDashboard.Background = Brushes.Transparent;
            stkShiftDetails.Background = Brushes.Transparent;
            stkEmpInfo.Background = Brushes.DarkSalmon;
            stkCustomerInfo.Background = Brushes.Transparent;
            stkPumpInformation.Background = Brushes.Transparent;
            stkScheduleMasterImport.Background = Brushes.Transparent;
            stkModelTargetMaster.Background = Brushes.Transparent;
            stkPumpDispatchReport.Background = Brushes.Transparent;
            stkPackingEfficiencyReport.Background = Brushes.Transparent;
            stkMonthlyFulfilmentReport.Background = Brushes.Transparent;
            Mouse.OverrideCursor = null;
        }

        private void btnCustomerInfo_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            CustomerInformation customerInfo = new CustomerInformation();
            UIPanel.Children.Clear();
            UIPanel.Children.Add(customerInfo);
            txtTitle.Text = "Customer Information".ToUpper();
            stkPackingDashboard.Background = Brushes.Transparent;
            stkShiftDetails.Background = Brushes.Transparent;
            stkEmpInfo.Background = Brushes.Transparent;
            stkCustomerInfo.Background = Brushes.DarkSalmon;
            stkPumpInformation.Background = Brushes.Transparent;
            stkScheduleMasterImport.Background = Brushes.Transparent;
            stkModelTargetMaster.Background = Brushes.Transparent;
            stkPumpDispatchReport.Background = Brushes.Transparent;
            stkPackingEfficiencyReport.Background = Brushes.Transparent;
            stkMonthlyFulfilmentReport.Background = Brushes.Transparent;
            Mouse.OverrideCursor = null;
        }

        private void btnPumpInformation_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            PumpInformation pumpInfo = new PumpInformation();
            UIPanel.Children.Clear();
            UIPanel.Children.Add(pumpInfo);
            txtTitle.Text = "Pump Information".ToUpper();
            stkPackingDashboard.Background = Brushes.Transparent;
            stkShiftDetails.Background = Brushes.Transparent;
            stkEmpInfo.Background = Brushes.Transparent;
            stkCustomerInfo.Background = Brushes.Transparent;
            stkPumpInformation.Background = Brushes.DarkSalmon;
            stkScheduleMasterImport.Background = Brushes.Transparent;
            stkModelTargetMaster.Background = Brushes.Transparent;
            stkPumpDispatchReport.Background = Brushes.Transparent;
            stkPackingEfficiencyReport.Background = Brushes.Transparent;
            stkMonthlyFulfilmentReport.Background = Brushes.Transparent;
            Mouse.OverrideCursor = null;
        }

        private void btnScheduleMasterImport_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            //ScheduleMasterImport scheduleMasterInfo = new ScheduleMasterImport();
            MonthlyScheduleMaster scheduleMasterInfo = new MonthlyScheduleMaster();
            UIPanel.Children.Clear();
            UIPanel.Children.Add(scheduleMasterInfo);
            txtTitle.Text = "Schedule Master".ToUpper();
            stkPackingDashboard.Background = Brushes.Transparent;
            stkShiftDetails.Background = Brushes.Transparent;
            stkEmpInfo.Background = Brushes.Transparent;
            stkCustomerInfo.Background = Brushes.Transparent;
            stkPumpInformation.Background = Brushes.Transparent;
            stkScheduleMasterImport.Background = Brushes.DarkSalmon;
            stkModelTargetMaster.Background = Brushes.Transparent;
            stkPumpDispatchReport.Background = Brushes.Transparent;
            stkPackingEfficiencyReport.Background = Brushes.Transparent;
            stkMonthlyFulfilmentReport.Background = Brushes.Transparent;
            Mouse.OverrideCursor = null;
        }

        private void btnModelTargetMaster_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            ShiftTargetMaster modelTargetMasterInfo = new ShiftTargetMaster();
            UIPanel.Children.Clear();
            UIPanel.Children.Add(modelTargetMasterInfo);
            txtTitle.Text = "Model Target Master".ToUpper();
            stkPackingDashboard.Background = Brushes.Transparent;
            stkShiftDetails.Background = Brushes.Transparent;
            stkEmpInfo.Background = Brushes.Transparent;
            stkCustomerInfo.Background = Brushes.Transparent;
            stkPumpInformation.Background = Brushes.Transparent;
            stkScheduleMasterImport.Background = Brushes.Transparent;
            stkModelTargetMaster.Background = Brushes.DarkSalmon;
            stkPumpDispatchReport.Background = Brushes.Transparent;
            stkPackingEfficiencyReport.Background = Brushes.Transparent;
            stkMonthlyFulfilmentReport.Background = Brushes.Transparent;
            Mouse.OverrideCursor = null;
        }

        private void btnPumpDispatchReport_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            PumpDispatchReport pumpDispatchReport = new PumpDispatchReport();
            UIPanel.Children.Clear();
            UIPanel.Children.Add(pumpDispatchReport);
            txtTitle.Text = "PUMP DISPATCH REPORT".ToUpper();
            stkPackingDashboard.Background = Brushes.Transparent;
            stkShiftDetails.Background = Brushes.Transparent;
            stkEmpInfo.Background = Brushes.Transparent;
            stkCustomerInfo.Background = Brushes.Transparent;
            stkPumpInformation.Background = Brushes.Transparent;
            stkScheduleMasterImport.Background = Brushes.Transparent;
            stkModelTargetMaster.Background = Brushes.Transparent;
            stkPumpDispatchReport.Background = Brushes.DarkSalmon;
            stkPackingEfficiencyReport.Background = Brushes.Transparent;
            stkMonthlyFulfilmentReport.Background = Brushes.Transparent;
            Mouse.OverrideCursor = null;
        }

        private void btnPackingEfficiencyReport_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            PackingEfficiencyReport packingEfficiencyReport = new PackingEfficiencyReport();
            UIPanel.Children.Clear();
            UIPanel.Children.Add(packingEfficiencyReport);
            txtTitle.Text = "Packing Efficiency Report".ToUpper();
            stkPackingDashboard.Background = Brushes.Transparent;
            stkShiftDetails.Background = Brushes.Transparent;
            stkEmpInfo.Background = Brushes.Transparent;
            stkCustomerInfo.Background = Brushes.Transparent;
            stkPumpInformation.Background = Brushes.Transparent;
            stkScheduleMasterImport.Background = Brushes.Transparent;
            stkModelTargetMaster.Background = Brushes.Transparent;
            stkPumpDispatchReport.Background = Brushes.Transparent;
            stkPackingEfficiencyReport.Background = Brushes.DarkSalmon;
            stkMonthlyFulfilmentReport.Background = Brushes.Transparent;
            Mouse.OverrideCursor = null;
        }

        private void btnMonthlyFulfilmentReport_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            MonthlyFulfillmentReport monthlyFulfilmentReport = new MonthlyFulfillmentReport();
            UIPanel.Children.Clear();
            UIPanel.Children.Add(monthlyFulfilmentReport);
            txtTitle.Text = "Monthly Fulfilment Report".ToUpper();
            stkPackingDashboard.Background = Brushes.Transparent;
            stkShiftDetails.Background = Brushes.Transparent;
            stkEmpInfo.Background = Brushes.Transparent;
            stkCustomerInfo.Background = Brushes.Transparent;
            stkPumpInformation.Background = Brushes.Transparent;
            stkScheduleMasterImport.Background = Brushes.Transparent;
            stkModelTargetMaster.Background = Brushes.Transparent;
            stkPumpDispatchReport.Background = Brushes.Transparent;
            stkPackingEfficiencyReport.Background = Brushes.Transparent;
            stkMonthlyFulfilmentReport.Background = Brushes.DarkSalmon;
            Mouse.OverrideCursor = null;
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            QRVals data = new QRVals();
            TimeSpan elapsed = (DateTime.Now - last);
            if (elapsed.TotalMilliseconds > 100)
                _barcode.Clear();
            _barcode.Add(e.Text);
            last = DateTime.Now;
            if (e.Text == "}" && _barcode.Count > 0)
            {
                last = new DateTime(0);
                msg = string.Join("", _barcode.ToArray());
                _barcode.Clear();
                try
                {
                    data = JsonConvert.DeserializeObject<QRVals>(msg);
                    _barcode.Clear();
                    //if (Utility.Process.Equals("dispatch area", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    DispatchProcessCtrl.RefreshData(data);
                    //}
                    //else
                    //{
                    //    DeburringProcess.RefreshData(data);
                    //}
                }
                catch (Exception ex)
                {
                    Logger.WriteDebugLog(ex.Message);
                }
            }
        }
    }
}
