using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using DeburringProcess_Peekay.Models;
using PackingModule_Rexroth.Mivin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeburringProcess_Peekay.Mivin
{
    /// <summary>
    /// Interaction logic for ReprintPackingDetails.xaml
    /// </summary>
    public partial class ReprintPackingDetails : Window
    {
        RunningModelStatusEntity runningModelData = null;
        public ReprintPackingDetails()
        {
            InitializeComponent();
        }

        public ReprintPackingDetails(RunningModelStatusEntity currentRunningModelData)
        {
            this.runningModelData = currentRunningModelData;
            InitializeComponent();
        }

        private void ReprintPackingDetails_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.runningModelData != null && !string.IsNullOrEmpty(runningModelData.RunningPumpModel) && !string.IsNullOrEmpty(runningModelData.PackagingType))
                {
                    lblPumpModel.Content = runningModelData.RunningPumpModel;
                    lblPackagingType.Content = runningModelData.PackagingType;
                }
                else
                {
                    DialogBox dlgInfo = new DialogBox("Information!!", "No running schedule found", false);
                    dlgInfo.Owner = Window.GetWindow(this);
                    dlgInfo.ShowDialog();
                }
                //LoadAllPumpModels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //private void LoadAllPumpModels()
        //{
        //    List<string> PumpModelsList = new List<string>();
        //    try
        //    {
        //        PumpModelsList = MivinDataBaseAccess.GetAllPumpModels();
        //        if (PumpModelsList != null && PumpModelsList.Count > 0)
        //        {
        //            cmbPumpModel.ItemsSource = PumpModelsList;
        //            cmbPumpModel.SelectedIndex = 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtNumOfPrints.Text))
                {
                    int quantity = Convert.ToInt32(txtNumOfPrints.Text);
                    if (quantity > 0)
                    {
                        string PumpModel = lblPumpModel.Content != null ? lblPumpModel.Content.ToString() : "";
                        string PackagingType = lblPackagingType.Content != null ? lblPackagingType.Content.ToString() : "";
                        if (!string.IsNullOrEmpty(PumpModel) && !string.IsNullOrEmpty(PackagingType))
                        {
                            PrintPackedBoxDetails(runningModelData, quantity);
                        }
                        else
                        {
                            DialogBox dlgInfo = new DialogBox("Information!!", "No pump model selected for reprint.", false);
                            dlgInfo.Owner = Window.GetWindow(this);
                            dlgInfo.ShowDialog();
                        }
                    }
                    else
                    {
                        DialogBox dlgInfo = new DialogBox("Information!!", "Print quantity cannot be zero.", false);
                        dlgInfo.Owner = Window.GetWindow(this);
                        dlgInfo.ShowDialog();
                    }
                }
                else
                {
                    DialogBox dlgInfo = new DialogBox("Information!!", "No. of prints cannot be empty.", false);
                    dlgInfo.Owner = Window.GetWindow(this);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintPackedBoxDetails(RunningModelStatusEntity runningModelStatusEntity, int quantity)
        {
            bool IsPrinterValid = false;
            string DestPath = string.Empty;
            try
            {
                string PrinterServer = ConfigurationManager.AppSettings["PrinterServer"].ToString();
                string PrinterName = ConfigurationManager.AppSettings["PrinterName"].ToString();
                string templateFileName = System.IO.Path.Combine(Utility.appPath, "Mivin", "PrintPRNFile", "Rexroth.prn");
                DestPath = PackingMainDashboard.GenerateDestinationPrintFile(runningModelStatusEntity, templateFileName);
                if (!string.IsNullOrEmpty(DestPath) && File.Exists(DestPath))
                {
                    try
                    {
                        PrintDocument pd = new PrintDocument();
                        pd.PrinterSettings.PrinterName = @PrinterName;
                        IsPrinterValid = pd.PrinterSettings.IsValid;
                        if (IsPrinterValid)
                        {
                            for (int i = 0; i < quantity; i++)
                            {
                                bool success = PrinterHelper.SendFileToPrinter(PrinterName, DestPath);
                                if (success)
                                    Logger.WriteDebugLog(string.Format("Printing Successfull for Pump Model :{0}", runningModelStatusEntity.RunningPumpModel));
                                Thread.Sleep(500);
                            }
                            DialogBox dlgInfo = new DialogBox("Success", "Print completed successfully.", false);
                            dlgInfo.Owner = Window.GetWindow(this);
                            dlgInfo.ShowDialog();
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
                        if (File.Exists(DestPath)) File.Delete(DestPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Printing Error : " + ex.Message);
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtNumOfPrints_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
