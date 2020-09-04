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


namespace DeburringProcess_Peekay.UserControls
{
	/// <summary>
	/// Interaction logic for InspectionRpt.xaml
	/// </summary>
	public partial class InspectionRpt : UserControl
	{
		List<ComboboxInspectionEntity> ComboboxLists = null;
		string _appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		private InspectionReportEntity inspectionReport = null;
		public InspectionRpt()
		{
			InitializeComponent();
		}


		private void LoadFPNumberComboBox()
		{
			List<string> FpNumberList = new List<string>();
			ComboboxLists = DatabaseAccess.GetFPNumberList();
			var FPNumberList = ComboboxLists.Select(o => o.FPNumber).Distinct().ToList();
			if (FPNumberList != null)
			{
				cmbFpNo.ItemsSource = FPNumberList;
			}
			var HeatNumberList = ComboboxLists.Select(o => o.HeatNumber).Distinct().ToList();
			if (HeatNumberList != null)
			{
				cmbHeatNo.ItemsSource = HeatNumberList;
			}
			var UTnumberList = ComboboxLists.Select(o => o.UTNumber).Distinct().ToList();
			if (UTnumberList != null)
			{
				cmbUtNo.ItemsSource = UTnumberList;
			}
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			LoadFPNumberComboBox();
			LoadInspectionReportParameters();
		}

		private void cmbFpNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (cmbFpNo.SelectedValue == null) return;
				var results = ComboboxLists.Where(x => x.FPNumber == cmbFpNo.SelectedValue.ToString()).Distinct().ToList();
				//cmbutnumber.ItemsSource = results.Select(o => o.UTNumber).Distinct().ToList();
				if (results.Count > 0)
				{
					cmbHeatNo.SelectedItem = results[0].HeatNumber;
					cmbUtNo.SelectedItem = results[0].UTNumber;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}
		}

		private void SetUTNumber(string FPNumber)
		{
			if (!string.IsNullOrEmpty(FPNumber))
			{
				List<string> UTNumberList = new List<string>();
				UTNumberList = DatabaseAccess.GetUTNumberForFPNumber(FPNumber);
				if (UTNumberList != null && UTNumberList.Count > 0)
				{
					cmbUtNo.ItemsSource = UTNumberList;
					cmbUtNo.SelectedIndex = 0;
				}
			}
		}

		private void LoadInspectionReportParameters()
		{
			if (cmbFpNo.SelectedItem == null && cmbUtNo.SelectedItem == null ) return;
			if (cmbFpNo.SelectedValue == null && cmbUtNo.SelectedValue == null ) return;
			inspectionReport = new InspectionReportEntity();
			inspectionReport = DatabaseAccess.GetAllInspectionReportData("FinalView", "", cmbFpNo.SelectedItem.ToString(), cmbUtNo.SelectedItem.ToString(), "");
			if (inspectionReport != null)
			{
				txtMpiNo.Text = inspectionReport.insparams.MPINumber;
				txtDrwingNo.Text = inspectionReport.insparams.DrawingNumber;
				txtMatGrade.Text = inspectionReport.insparams.MaterialGrade;
				txtFpDecs.Text = inspectionReport.insparams.FPDescription;
				txtPOnum.Text = inspectionReport.insparams.PONum;

				txtWOnum.Text = inspectionReport.insparams.WONum;

				if (inspectionReport.insReportParamsList != null && inspectionReport.insReportParamsList.Count > 0)
				{
					dgInspectionRpt.ItemsSource = inspectionReport.insReportParamsList;
				}
				else
				{
					DialogBox db = new DialogBox("Information Message", "No data available for current FP Number and UT number", false);
					db.ShowDialog();
					dgInspectionRpt.ItemsSource = new List<InspectionReportParameters>();
				}
			}
			else
			{
				DialogBox db = new DialogBox("Information Message", "No data available for current FP Number and UT number", false);
				db.ShowDialog();
				dgInspectionRpt.ItemsSource = new List<InspectionReportParameters>();
			}
		}

		private void btnView_Click(object sender, RoutedEventArgs e)
		{
			LoadInspectionReportParameters();
		}

		private void btnExport_Click(object sender, RoutedEventArgs e)
		{
			if (inspectionReport.insReportParamsList != null && inspectionReport.insReportParamsList.Count > 0)
			{
				string generatedFilePath = Path.Combine(_appPath, "Templates", "GeneratedReports");
				string FilePath = string.Empty;
				ExportInspectionDataReport(Path.Combine(_appPath, "Templates", "InspectionDataReport.xlsx"), generatedFilePath, "", out FilePath);
				if (FilePath != string.Empty)
				{
					DialogBox db = new DialogBox("Information", "Your Inspection Data Report Generated Successfully." + Environment.NewLine + "Opening Report....", false);
					db.ShowDialog();
					OpenExe(FilePath);
				}
			}
			else
			{
				DialogBox db = new DialogBox("Information", "No Inspection data to export. Select another FP and UT number.", false);
				db.ShowDialog();
				return;
			}
		}

		private void ExportInspectionDataReport(string reportTemplatePath, string generatedFilePath, string ExportedReportFile, out string filePath)
		{
			filePath = string.Empty;
			try
			{
				if (!File.Exists(reportTemplatePath))
				{
					DialogBox db = new DialogBox("Error", "Template is not found on " + reportTemplatePath, true);
					db.ShowDialog();
				}
				if (!Directory.Exists(generatedFilePath))
				{
					Directory.CreateDirectory(generatedFilePath);
				}
				ExportedReportFile = Path.Combine(generatedFilePath, "InspectionDataReport" + string.Format("{0:ddMMMyyyy_HHmmss}", DateTime.Now) + ".xlsx");
				filePath = ExportedReportFile;
				if (File.Exists(ExportedReportFile))
				{
					var dirInfo = new DirectoryInfo(ExportedReportFile);
					dirInfo.Attributes &= ~FileAttributes.ReadOnly;
					File.Delete(ExportedReportFile);
				}
				File.Copy(reportTemplatePath, ExportedReportFile, true);
				FillInspectionDataReport(ref ExportedReportFile, reportTemplatePath);
				filePath = ExportedReportFile;
			}
			catch (Exception ex)
			{
				DialogBox db = new DialogBox("Error", ex.Message, true);
				db.ShowDialog();
			}
		}

		private void FillInspectionDataReport(ref string exportedReportFile, string reportTemplatePath)
		{
			List<InspectionReportParameters> insParamList = new List<InspectionReportParameters>();
			FileInfo newFile = new FileInfo(exportedReportFile);
			ExcelPackage excelPackage = new ExcelPackage(newFile, true);
			ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];

			workSheet.Cells["A3"].Value = workSheet.Cells["A3"].Value + " " + inspectionReport.insparams.CustomerName;
			workSheet.Cells["E3"].Value = workSheet.Cells["E3"].Value + " " + inspectionReport.insparams.MaterialGrade;
			workSheet.Cells["G3"].Value = workSheet.Cells["G3"].Value + " " + inspectionReport.insparams.ReportNo;

			workSheet.Cells["A4"].Value = workSheet.Cells["A4"].Value + " " + inspectionReport.insparams.PartName;
			workSheet.Cells["E4"].Value = workSheet.Cells["E4"].Value + " " + inspectionReport.insparams.WOnoNDate;
			workSheet.Cells["G4"].Value = workSheet.Cells["G4"].Value + " " + inspectionReport.insparams.InspDate;

			workSheet.Cells["A5"].Value = workSheet.Cells["A5"].Value + " " + inspectionReport.insparams.DrawingNumber;
			workSheet.Cells["E5"].Value = workSheet.Cells["E5"].Value + " " + inspectionReport.insparams.PONum;
			workSheet.Cells["G5"].Value = workSheet.Cells["G5"].Value + " " + inspectionReport.insparams.Quantity;

			workSheet.Cells["A6"].Value = workSheet.Cells["A6"].Value + " " + inspectionReport.insparams.PartNumber;
			workSheet.Cells["E6"].Value = workSheet.Cells["E6"].Value + " " + inspectionReport.insparams.PODate;
			workSheet.Cells["G6"].Value = workSheet.Cells["G6"].Value + " " + inspectionReport.insparams.NDEReq;

			workSheet.Cells["A7"].Value = workSheet.Cells["A7"].Value + " " + inspectionReport.insparams.FPNumber;
			workSheet.Cells["E7"].Value = workSheet.Cells["E7"].Value + " " + inspectionReport.insparams.SupplierName;
			workSheet.Cells["G7"].Value = workSheet.Cells["G7"].Value + " " + inspectionReport.insparams.HydoTestReqd;

			workSheet.Cells["F9"].Value = workSheet.Cells["F9"].Value + " " + inspectionReport.insparams.HeatNumber + " : " + inspectionReport.insparams.MPINumber;
			workSheet.Cells["A4"].Value = workSheet.Cells["A4"].Value + " " + inspectionReport.insparams.FPDescription;
		

			workSheet.Cells["G3"].Value = workSheet.Cells["G3"].Value + " " + cmbUtNo.Text.TrimStart('U');

			List<string> opNumberList = inspectionReport.insReportParamsList.Select(x => x.OperationNo).Distinct().ToList();

			int row = 10;
			int rowLimit = 39;
			int i = 0;
			int rowsToAdd = inspectionReport.insReportParamsList.Count - (rowLimit - row + 1) + (opNumberList.Count - 1) * 3;
			workSheet.InsertRow(row + 2, rowsToAdd + 3, 11);
			for (int j = row + 2; j <= rowsToAdd + row + 1; j++)
			{
				workSheet.Row(j).Height = 25.50;
			}
			foreach (string opNum in opNumberList)
			{
				insParamList = inspectionReport.insReportParamsList.Where(x => x.OperationNo == opNum).ToList();
				//if (i == 0)
				//{
				//	EnterNewOperationTitle(workSheet, opNum, row);
				//	row = row + 2;
				//	foreach (InspectionReportParameters insParam in insParamList)
				//	{
				//		FillExcelRow(workSheet, insParam, row);
				//		row++;
				//	}
				//}
				//else
				{
					row++;
					EnterNewOperationTitle(workSheet, opNum, row);
					row = row + 2;
					foreach (InspectionReportParameters insParam in insParamList)
					{
						FillExcelRow(workSheet, insParam, row);
						row++;
					}
				}
				i++;
			}
			excelPackage.SaveAs(newFile);
		}

		private void FillExcelRow(ExcelWorksheet workSheet, InspectionReportParameters insParam, int row)
		{
			workSheet.Cells[row, 1].Value = insParam.BLNo;
			workSheet.Cells[row, 2].Value = insParam.DrawingSpec;
			workSheet.Cells[row, 3].Value = insParam.LSL;
			workSheet.Cells[row, 4].Value = insParam.USL;
			workSheet.Cells[row, 5].Value = insParam.MethodOfInspection;
			workSheet.Cells[row, 6].Value = insParam.Observation;
			workSheet.Cells[row, 7].Value = insParam.SupervisorRemarks;
			workSheet.Cells[row, 1, row, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
			workSheet.Cells[row, 1, row, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
		}

		private void EnterNewOperationTitle(ExcelWorksheet workSheet, string opNum, int row)
		{
			workSheet.Cells[row, 1, row, 7].Merge = true;
			workSheet.Cells[row, 1].Style.Font.Bold = true;
			workSheet.Cells[row, 1].Style.Font.Size = 14;
			workSheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
			workSheet.Cells[row, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
			workSheet.Cells[row, 1].Value = "OPERATION NUMBER : " + opNum;
		}

		internal static void OpenExe(string path)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = "\"" + path + "\"";
			Process.Start(startInfo);
		}

		private void cmbHeatNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (cmbHeatNo.SelectedValue == null) return;
				var results = ComboboxLists.Where(x => x.HeatNumber == cmbHeatNo.SelectedValue.ToString()).Distinct().ToList();
				//cmbutnumber.ItemsSource = results.Select(o => o.UTNumber).Distinct().ToList();
				if (results.Count > 0)
				{
					cmbFpNo.SelectedItem = results[0].FPNumber;
					cmbUtNo.SelectedItem = results[0].UTNumber;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}
		}

		private void cmbUtNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (cmbUtNo.SelectedValue == null) return;
				var results = ComboboxLists.Where(x => x.UTNumber == cmbUtNo.SelectedValue.ToString()).Distinct().ToList();
				//cmbutnumber.ItemsSource = results.Select(o => o.UTNumber).Distinct().ToList();
				if (results.Count > 0)
				{
					cmbFpNo.SelectedItem = results[0].FPNumber;
					cmbHeatNo.SelectedItem = results[0].UTNumber;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}
		}
	}
}
