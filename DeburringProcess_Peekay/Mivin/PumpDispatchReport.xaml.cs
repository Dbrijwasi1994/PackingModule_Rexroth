using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using DeburringProcess_Peekay.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for PumpDispatchReport.xaml
    /// </summary>
    public partial class PumpDispatchReport : UserControl
    {
        List<PumpDispatchReportEntity> pumpDispatchReportData = null;
        List<string> PumpModel = null;
        string APP_PATH = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public PumpDispatchReport()
        {
            InitializeComponent();
        }

        private void PumpDispatchReport_Control_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dtpFromDate.SelectedDate = DateTime.Now.AddDays(-1);
                dtpToDate.SelectedDate = DateTime.Now;
                BindPumpModels();
                BindStationIDs();
                BindShifts();
                BindPumpDispatchReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BindPumpModels()
        {
            try
            {
                PumpModel = MivinDataBaseAccess.GetPumplist();
                if (PumpModel != null && PumpModel.Count > 0)
                {
                    if (!PumpModel.Contains("All")) PumpModel.Insert(0, "All");
                    cmbPumpModel.ItemsSource = null;
                    cmbPumpModel.ItemsSource = PumpModel;
                    cmbPumpModel.SelectedIndex = 0;
                }
                else
                {
                    cmbPumpModel.ItemsSource = new List<string>();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }

        private void BindStationIDs()
        {
            try
            {
                List<string> stationIDList = MivinDataBaseAccess.GetAllStationIDs();
                if (stationIDList != null && stationIDList.Count > 0)
                {
                    if (!stationIDList.Contains("All")) stationIDList.Insert(0, "All");
                    cmbStationID.ItemsSource = null;
                    cmbStationID.ItemsSource = stationIDList;
                    cmbStationID.SelectedIndex = 0;
                }
                else
                {
                    cmbStationID.ItemsSource = new List<string>();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }

        private void BindShifts()
        {
            try
            {
                List<string> ShiftList = MivinDataBaseAccess.GetShiftDetails();
                if (ShiftList != null && ShiftList.Count > 0)
                {
                    if (!ShiftList.Contains("All")) ShiftList.Insert(0, "All");
                    cmbShift.ItemsSource = ShiftList;
                    cmbShift.SelectedIndex = 0;
                }
                else
                {
                    cmbShift.ItemsSource = new List<string>();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }

        private void BindPumpDispatchReport()
        {
            try
            {
                if (cmbPumpModel.SelectedValue != null && cmbShift.SelectedValue != null)
                {
                    pumpDispatchReportData = MivinDataBaseAccess.GetPumpDispatchReportData(dtpFromDate.SelectedDate.Value, dtpToDate.SelectedDate.Value, cmbStationID.SelectedValue.ToString(), cmbPumpModel.SelectedValue.ToString(), "", cmbShift.SelectedValue.ToString());
                    if (pumpDispatchReportData != null && pumpDispatchReportData.Count > 0)
                    {
                        PumpDispatchReportGrid.ItemsSource = pumpDispatchReportData;
                    }
                    else
                    {
                        PumpDispatchReportGrid.ItemsSource = null;
                        DialogBox dlgInfo = new DialogBox("Information!!", "No data available.", false);
                        dlgInfo.Owner = Window.GetWindow(this);
                        dlgInfo.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            BindPumpDispatchReport();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string TEMPLATE = "PumpDispatchReport_Mivin";
                string templateFile = string.Empty;
                templateFile = System.IO.Path.Combine(APP_PATH, "Templates", TEMPLATE + ".xlsx");
                string GeneratedReportPath = System.IO.Path.Combine(APP_PATH, "Templates", "GeneratedReports");
                if (!File.Exists(GeneratedReportPath))
                {
                    Directory.CreateDirectory(GeneratedReportPath);
                }
                if (!File.Exists(templateFile))
                {
                    MessageBox.Show("Template is not found. Cannot export data to excel.", "Error Message");
                    return;
                }
                string excelFilePath = System.IO.Path.Combine(GeneratedReportPath, "PumpDispatchReport_Mivin_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
                File.Copy(templateFile, excelFilePath, true);
                FileInfo newFile = new FileInfo(excelFilePath);
                ExcelPackage pck = new ExcelPackage(newFile, true);
                var wsDt = pck.Workbook.Worksheets[1];
                int row = 5, col = 1, Qty = 0;
                wsDt.Cells["B1"].Value = dtpFromDate.SelectedDate.ToString().Split(' ')[0];
                wsDt.Cells["B3"].Value = cmbShift.Text.ToString();
                wsDt.Cells["B2"].Value = dtpToDate.SelectedDate.ToString().Split(' ')[0];
                if (pumpDispatchReportData != null && pumpDispatchReportData.Count > 0)
                {
                    foreach (PumpDispatchReportEntity entity in pumpDispatchReportData)
                    {
                        col = 1;
                        wsDt.Cells[row, col].Value = entity.Sl_No;
                        col++;
                        wsDt.Cells[row, col].Value = entity.StationID;
                        col++;
                        wsDt.Cells[row, col].Value = entity.PumpPart_No;
                        col++;
                        wsDt.Cells[row, col].Value = entity.Quantity;
                        col++;
                        wsDt.Cells[row, col].Value = entity.BoxType_No;
                        col++;
                        wsDt.Cells[row, col].Value = entity.PackageType;
                        col += 2;
                        wsDt.Cells[row, col].Value = entity.Remarks;
                        row++;
                        Qty += Convert.ToInt32(entity.Quantity);
                    }
                    wsDt.Cells[row, 3].Value = "Ready For Dispatch";
                    wsDt.Cells[row, 3].Style.Font.Bold = true;
                    wsDt.Cells[row, 3].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
                    wsDt.Cells[row, 3].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
                    wsDt.Cells[row, 4].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
                    wsDt.Cells[row, 4].Value = Qty;
                    wsDt.Cells[row, 1, row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);

                    wsDt.Cells[5, 1, row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    wsDt.Cells[5, 1, row, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    row--;
                    wsDt.Cells[5, 1, row, col].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    wsDt.Cells[5, 1, row, col].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    wsDt.Cells[5, 1, row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    wsDt.Cells[5, 1, row, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    wsDt.Cells[5, 1, row, col].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    wsDt.Cells[5, 1, row, col].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    wsDt.Cells[5, 1, row, col].Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    wsDt.Cells[5, 1, row, col].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);

                    pck.SaveAs(newFile);
                    DialogBox dlgInfo = new DialogBox("Information ", "Report Generated Successfully.", false);
                    dlgInfo.ShowDialog();
                    Utility.OpenExe(excelFilePath);
                }
                else
                {
                    DialogBox dlgInfo = new DialogBox("Information ", "No data to export.", false);
                    dlgInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
                DialogBox dlgInfo = new DialogBox("Error ", ex.ToString(), false);
                dlgInfo.ShowDialog();
            }
        }

        private void cmbPumpModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //BindWorkOrders();
        }

        private void cmbPumpModel_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                cmbPumpModel.IsDropDownOpen = true;
                string pump_model = !string.IsNullOrEmpty(cmbPumpModel.Text) ? cmbPumpModel.Text : "";
                if (PumpModel != null && PumpModel.Count > 0)
                {
                    if (!string.IsNullOrEmpty(pump_model) && !pump_model.Equals("All"))
                    {
                        cmbPumpModel.ItemsSource = PumpModel.Where(x => x.Contains(pump_model));
                    }
                    else
                    {
                        cmbPumpModel.ItemsSource = PumpModel;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
        }
    }
}
