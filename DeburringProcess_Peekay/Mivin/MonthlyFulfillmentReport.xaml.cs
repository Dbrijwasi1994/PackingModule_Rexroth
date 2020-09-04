using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using OfficeOpenXml;
using PackingModule_Rexroth.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace PackingModule_Rexroth.Mivin
{
    /// <summary>
    /// Interaction logic for MonthlyFulfillmentReport.xaml
    /// </summary>
    public partial class MonthlyFulfillmentReport : UserControl
    {
        List<MonthlyfulfilmentReportEntity> monthlyFulfilmentReportData = null;
        string APP_PATH = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public MonthlyFulfillmentReport()
        {
            InitializeComponent();
        }

        private void MonthlyFulfillmentReport_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                BindMonths();
                BindYears();
                BindMonthlyFulfilmentReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BindMonths()
        {
            try
            {
                var months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
                string currentMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
                if (months != null && months.Length > 0)
                {
                    cmbMonth.ItemsSource = months;
                    if (!string.IsNullOrEmpty(currentMonthName))
                        cmbMonth.SelectedIndex = Array.IndexOf(months, currentMonthName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BindYears()
        {
            try
            {
                List<int> lstYears = Enumerable.Range(DateTime.Now.Year - 9, 10).Reverse().ToList();
                if (lstYears != null && lstYears.Count > 0)
                {
                    cmbYear.ItemsSource = lstYears;
                    cmbYear.SelectedValue = DateTime.Now.Year;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BindMonthlyFulfilmentReport()
        {
            try
            {
                if (cmbMonth.SelectedValue != null && cmbYear.SelectedValue != null)
                {
                    monthlyFulfilmentReportData = MivinDataBaseAccess.GetMonthlyFulfilmentReportData(cmbMonth.SelectedValue.ToString(), cmbYear.SelectedValue.ToString());
                    if (monthlyFulfilmentReportData != null && monthlyFulfilmentReportData.Count > 0)
                    {
                        GridMonthlyFulfilmentReport.ItemsSource = monthlyFulfilmentReportData;
                    }
                    else
                    {
                        GridMonthlyFulfilmentReport.ItemsSource = null;
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
            BindMonthlyFulfilmentReport();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string TEMPLATE = "MonthlyFulfilmentReport_Mivin";
                string templateFile = string.Empty;
                templateFile = System.IO.Path.Combine(APP_PATH, "Templates", TEMPLATE + ".xlsx");
                string GeneratedReportPath = System.IO.Path.Combine(APP_PATH, "Templates", "GeneratedReports");
                string mnth = cmbMonth.SelectedItem != null ? cmbMonth.SelectedValue.ToString() : "";
                string yr = cmbYear.SelectedItem != null ? cmbYear.SelectedValue.ToString() : "";
                string fullMonth = $"{mnth}-{yr}";
                if (!File.Exists(GeneratedReportPath))
                {
                    Directory.CreateDirectory(GeneratedReportPath);
                }
                if (!File.Exists(templateFile))
                {
                    MessageBox.Show("Template is not found. Cannot export data to excel.", "Error Message");
                    return;
                }
                string excelFilePath = System.IO.Path.Combine(GeneratedReportPath, "MonthlyFulfilmentReport_Mivin_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
                File.Copy(templateFile, excelFilePath, true);
                FileInfo newFile = new FileInfo(excelFilePath);
                ExcelPackage pck = new ExcelPackage(newFile, true);
                var wsDt = pck.Workbook.Worksheets[1];
                int row = 4, col = 18;
                wsDt.Cells["C2"].Value = fullMonth;
                if (monthlyFulfilmentReportData != null && monthlyFulfilmentReportData.Count > 0)
                {
                    foreach (MonthlyfulfilmentReportEntity entity in monthlyFulfilmentReportData)
                    {
                        wsDt.Cells[row, 2].Value = entity.Type;
                        wsDt.Cells[row, 3].Value = entity.Customer;
                        wsDt.Cells[row, 4].Value = entity.PumpPartNumber;
                        wsDt.Cells[row, 5].Value = entity.MonthRequirement;
                        wsDt.Cells[row, 6].Value = entity.ActualPackedQty;
                        wsDt.Cells[row, 7].Value = entity.PendingForPacking;
                        wsDt.Cells[row, 8].Value = entity.ActualDispatch;
                        wsDt.Cells[row, 9].Value = entity.CW1;
                        wsDt.Cells[row, 10].Value = entity.CW1Actual;
                        wsDt.Cells[row, 11].Value = entity.CW2;
                        wsDt.Cells[row, 12].Value = entity.CW2Actual;
                        wsDt.Cells[row, 13].Value = entity.CW3;
                        wsDt.Cells[row, 14].Value = entity.CW3Actual;
                        wsDt.Cells[row, 15].Value = entity.CW4;
                        wsDt.Cells[row, 16].Value = entity.CW4Actual;
                        wsDt.Cells[row, 17].Value = entity.CW5;
                        wsDt.Cells[row, 18].Value = entity.CW5Actual;
                        row++;
                    }
                    row--;
                    wsDt.Cells[4, 2, row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    wsDt.Cells[4, 2, row, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    wsDt.Cells[4, 2, row, col].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    wsDt.Cells[4, 2, row, col].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    wsDt.Cells[4, 2, row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    wsDt.Cells[4, 2, row, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    wsDt.Cells[4, 2, row, col].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    wsDt.Cells[4, 2, row, col].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    wsDt.Cells[4, 2, row, col].Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    wsDt.Cells[4, 2, row, col].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
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
    }
}
