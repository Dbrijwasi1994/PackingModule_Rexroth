using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Helpers;
using DeburringProcess_Peekay.Models;
using Microsoft.Win32;
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
using DeburringProcess_Peekay.Dialogs;
using OfficeOpenXml;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DeburringProcess_Peekay
{
	public partial class InspectionProcess : System.Windows.Controls.UserControl
	{
		int countbefore = 0, countafter = 0;
		List<string> dataop = new List<string>();
		List<string> datatype = new List<string>();
		List<string> data = new List<string>();
		public static Boolean rowAdded = false, rowModified = false;
		string value = string.Empty;
		string values = string.Empty;
		string prevComponent = string.Empty;
		string prevOperation = string.Empty;
		string prevSampleSize = string.Empty;
		string prevInterval = string.Empty;
		string prevDrawing = string.Empty;
		List<GetInspectionData> InspectionData = new List<GetInspectionData>();
		string APP_PATH = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

		public InspectionProcess()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			GetDropdown();
		}

		private void GetDropdown()
		{
			dataop = DatabaseAccess.GetCODataList("");
			cmbfbno.ItemsSource = dataop;
			cmbfbno_SelectionChanged(null, null);
			cmbfbno.Text = dataop[0].ToString();
		}

		private void cmbfbno_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				value = cmbfbno.SelectedItem.ToString();
				data = DatabaseAccess.GetCODataList(cmbfbno.SelectedItem.ToString());
				cmbop.ItemsSource = data;
				cmbop.Text = data[0].ToString();
				value = cmbfbno.SelectedItem.ToString();
				btnview_Click(null, null);
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}
		}

		private void cmbop_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			values = cmbop.SelectedItem.ToString();
		}

		private void btnview_Click(object sender, RoutedEventArgs e)
		{
			InspectionData=DatabaseAccess.GetSPCCharecteristicData(value, values);
			if(InspectionData.Count!=0)
			{
				
				dgInspectionprocess.ItemsSource = InspectionData;
				countbefore = dgInspectionprocess.Items.Count;
				DataType();
				txtdrawing.Text = InspectionData[0].inspectionDrawing.ToString();
				txtinterval.Text = InspectionData[0].interval.ToString();
				txtsample.Text = InspectionData[0].sampleSize.ToString();
			}
			else
			{
				txtdrawing.Text = "-";
				txtinterval.Text = "-";
				txtsample.Text = "-";
			}
		}

		private void DataType( )
		{
			datatype = DatabaseAccess.datatype(cmbfbno.SelectedItem.ToString(), cmbop.SelectedItem.ToString());
			cmddata.ItemsSource = datatype;
		}

		private void btnimportdata_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
			Nullable<bool> result =dialog.ShowDialog();
			if(result==true)
			{
				try
				{
					string filename = dialog.FileName;
					ImportData(filename);
				}
				catch (Exception ex)
				{
					Logger.WriteErrorLog("Error - \n" + ex.ToString());
					DialogBox frm = new DialogBox("Error Message", ex.Message.ToString(), true);
					frm.ShowDialog();
				}
			}
			
		}

		private void ImportData(string filename)
		{
			string names = filename;
			var excelFileToImport = System.IO.Path.GetFileNameWithoutExtension(names) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + System.IO.Path.GetExtension(names);
			var excelFileFullPath = System.IO.Path.Combine(APP_PATH, "ImportTemplates", "GeneratedReports", excelFileToImport);
			if (!Directory.Exists(System.IO.Path.GetDirectoryName(excelFileFullPath)))
			{
				Directory.CreateDirectory(System.IO.Path.GetDirectoryName(excelFileFullPath));
			}
			if (File.Exists(names))
			{
				File.Copy(names, System.IO.Path.Combine(APP_PATH, "ImportTemplates", "GeneratedReports", excelFileToImport));
			}

			GenerateDataSetFromExcelData(excelFileFullPath);

			cmbfbno.ItemsSource = DatabaseAccess.GetCODataList("");
			cmbfbno_SelectionChanged(null, null);
			btnview_Click(null, null);
		}

		public static void GenerateDataSetFromExcelData(string excelFileFullPath)
		{
			DataTable tbl = null;
			using (var pck = new ExcelPackage())
			{
				using (var stream = File.OpenRead(excelFileFullPath))
				{
					pck.Load(stream);
				}
				var workBook = pck.Workbook;
				if (workBook != null)
				{
					try
					{
						var ws = workBook.Worksheets[1];
						string ComponentID, OperationNo, SampleSize, Interval, InspectionDrawing;
						ComponentID = ws.Cells[2, 1].Value.ToString();
						OperationNo = ws.Cells[2, 3].Value.ToString();
						InspectionDrawing = ws.Cells[2, 4].Value.ToString();
						SampleSize = ws.Cells[2, 5].Value.ToString();
						Interval = ws.Cells[2, 6].Value.ToString();
						tbl = new DataTable();
						tbl.Columns.Add("MachineID");
						tbl.Columns.Add("ComponentID");
						tbl.Columns.Add("OperationNo");
						tbl.Columns.Add("SampleSize");
						tbl.Columns.Add("Interval");
						tbl.Columns.Add("InspectionDrawing");
						tbl.Columns.Add("CharacteristicID");
						tbl.Columns.Add("CharacteristicCode");
						tbl.Columns.Add("Specification");
						tbl.Columns.Add("SpecificationMean");
						tbl.Columns.Add("LSL");
						tbl.Columns.Add("USL");
						tbl.Columns.Add("UOM");
						tbl.Columns.Add("InstrumentType");
						tbl.Columns.Add("DataType");
						tbl.Columns.Add("InProcessInterval");
						tbl.Columns.Add("SetupApprovalInterval");
						var startRow = 5;
						int lastRow = GetLastUsedRow(ws);
						for (var rowNum = startRow; rowNum <= lastRow; rowNum++)
						{
							var wsRow = ws.Cells[rowNum, 1, rowNum, 9];
							var row = tbl.NewRow();
							row[0] = "MachineID";
							row[1] = ComponentID;
							row[2] = OperationNo;
							row[3] = SampleSize;
							row[4] = Interval;
							row[5] = InspectionDrawing;
							row[6] = rowNum - startRow + 1;
							foreach (var cell in wsRow)
							{
								try
								{
									if (!string.IsNullOrEmpty(cell.Text))
									{
										row[cell.Start.Column + 6] = cell.Text.Trim();
									}
									else if (cell.Address.Contains("G"))
									{
										row[cell.Start.Column + 6] = "2";
									}
								}
								catch (Exception ex)
								{
									Logger.WriteErrorLog("Error - \n" + ex.ToString());
									DialogBox frm = new DialogBox("Error Message", ex.Message.ToString(),true);
									frm.ShowDialog();
								}
							}
							tbl.Rows.Add(row);
						}
						//Validate rows before import
						if (string.IsNullOrEmpty(ComponentID))
						{
							DialogBox frm = new DialogBox("Error Message", "Please enter component ID in excel.",true);
							frm.ShowDialog();
							return;
						}
						if (string.IsNullOrEmpty(OperationNo))
						{
							DialogBox frm = new DialogBox("Error Message", "Please enter Operation No.  in excel", true);
							frm.ShowDialog();
							return;
						}
						if (string.IsNullOrEmpty(Interval))
						{
							DialogBox frm = new DialogBox("Error Message", "Please enter Interval in excel", true);
							frm.ShowDialog();
							return;
						}
						var CharacteristicCodeVal = tbl.AsEnumerable().Where(t => t.Field<string>("CharacteristicCode") == null).ToList();
						if (CharacteristicCodeVal.Count > 0)
						{
							DialogBox frm = new DialogBox("Error Message", "Please enter the Characteristic Code in all the rows", true);
							frm.ShowDialog();
							return;
						}
						DataTable dt = DatabaseAccess.GetMasterDataExistence(ComponentID, OperationNo);
						if (dt.Rows.Count > 0)
						{
							DialogResult res = System.Windows.Forms.MessageBox.Show("Data Already exists for the Component / Operation. \n  Are you sure to Delete existence Inspection Data ?", "Delete Data", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
							if (res == System.Windows.Forms.DialogResult.OK)
							{
								DatabaseAccess.DeleteSPCMasterData(ComponentID, OperationNo);
							}
							else
							{
								return;
							}
						}
						ExcelReader(tbl, excelFileFullPath);
						DialogBox frmz = new DialogBox("Error Message", "Data imported Successfully." + ws.ToString(),true);
						frmz.ShowDialog();
					}
					catch (Exception ex)
					{
						Logger.WriteErrorLog("Error - \n" + ex.ToString());
						DialogBox frm = new DialogBox("Error Message", ex.Message.ToString(), true);
						frm.ShowDialog();
					}
				}
			}
		}

		private static int GetLastUsedRow(ExcelWorksheet sheet)
		{
			var row = sheet.Dimension.End.Row;
			while (row >= 1)
			{
				var range = sheet.Cells[row, 1, row, sheet.Dimension.End.Column];
				if (range.Any(c => !string.IsNullOrEmpty(c.Text)))
				{
					break;
				}
				row--;
			}
			return row;
		}

		private static void ExcelReader(DataTable tbl, string excelFileFullPath)
		{
			SqlConnection ConSql = null;
			ConSql = ConnectionManager.GetConnection();

			using (SqlBulkCopy bulk1 = new SqlBulkCopy(ConSql))
			{
				try
				{
					bulk1.ColumnMappings.Add("MachineID", "MachineID");
					bulk1.ColumnMappings.Add("ComponentID", "ComponentID");
					bulk1.ColumnMappings.Add("OperationNo", "OperationNo");
					bulk1.ColumnMappings.Add("CharacteristicCode", "CharacteristicCode");
					bulk1.ColumnMappings.Add("CharacteristicID", "CharacteristicID");
					bulk1.ColumnMappings.Add("SpecificationMean", "SpecificationMean");
					bulk1.ColumnMappings.Add("LSL", "LSL");
					bulk1.ColumnMappings.Add("USL", "USL");
					bulk1.ColumnMappings.Add("UOM", "UOM");

					bulk1.ColumnMappings.Add("SampleSize", "SampleSize");
					bulk1.ColumnMappings.Add("Interval", "Interval");
					bulk1.ColumnMappings.Add("InstrumentType", "InstrumentType");
					bulk1.ColumnMappings.Add("InspectionDrawing", "InspectionDrawing");
					bulk1.ColumnMappings.Add("DataType", "Datatype");
					bulk1.ColumnMappings.Add("InProcessInterval", "InProcessInterval");
					bulk1.ColumnMappings.Add("SetupApprovalInterval", "SetupApprovalInterval");
					bulk1.ColumnMappings.Add("Specification", "Specification");

					bulk1.DestinationTableName = "SPC_Characteristic";
					bulk1.WriteToServer(tbl);
				}
				catch (Exception ex)
				{
					Logger.WriteErrorLog("Error - \n" + ex.ToString());
					throw;
					//CustomDialogBox frm = new CustomDialogBox("Error Message", ex.Message.ToString());
					//frm.ShowDialog(); 
				}
			}

		}

		private void btnopentem_Click(object sender, RoutedEventArgs e)
		{
			string templateFile = string.Empty;

			templateFile = System.IO.Path.Combine(APP_PATH, @"Templates\" + "spc_characteristics_.xlsx");

			if (!File.Exists(templateFile))
			{
				DialogBox msg = new DialogBox("Error Message", "Template is not found. Cannot Export Data to Excel.", true);
				msg.Show();
				return;
			}
			else
			{
				ProcessStartInfo startInfo = new ProcessStartInfo();
				startInfo.FileName = "\"" + templateFile + "\"";
				Process.Start(startInfo);
			}
		}

		private void btnnew_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (btnnew.Content.ToString() == "New")
				{
					dgInspectionprocess.CanUserAddRows= true;
					btnnew.Content = "Cancel";
					int rowIndex = dgInspectionprocess.Items.Count - 1;
					countafter = rowIndex;
					dgInspectionprocess.CurrentCell = new DataGridCellInfo(dgInspectionprocess.SelectedItem, dgInspectionprocess.Columns[0]);
				}
				else
				{
					btnnew.Content = "New";
					dgInspectionprocess.CanUserAddRows = false;
					btnview_Click(null, null);
				}
			}
			catch(Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}
		}

		private void btnsave_Click(object sender, RoutedEventArgs e)
		{
			GetInspectionData Data = new GetInspectionData();
			Data = dgInspectionprocess.SelectedItem as GetInspectionData;
			string successFailure = string.Empty;
			try
			{
				Data.sampleSize = txtsample.Text;
				Data.inspectionDrawing = txtdrawing.Text;
				Data.interval= txtinterval.Text;
				if (countafter!=0)
				{
					Data.machineID = "MachineID";
					DatabaseAccess.InspecDataInsertUpdateDelete(Data.machineID, cmbfbno.Text, cmbop.Text, Data.characteristicCode, Data.characteristicID, Data.specificationMean, Data._LSL, Data._USL, Data._UOM, txtsample.Text, Data.inProcessInterval, Data.instrumentType, txtdrawing.Text, Data.dataType, Data.setupApprovalInterval, txtinterval.Text, Data.specification, Data.macroLocation, Data._ID, "Insert", out successFailure);
					if (successFailure.Equals("Successfull"))
					{
						rowAdded = true;
					}
					else
					{
						btnnew.Content = "Cancel";
						DialogBox dialog = new DialogBox("Error Message", successFailure + "", true);
						dialog.ShowDialog();
					}
				}
				else if(countafter==0)
				{
					Data.machineID = "MachineID";
					DatabaseAccess.InspecDataInsertUpdateDelete(Data.machineID, Data.componentID, Data.operationNo, Data.characteristicCode, Data.characteristicID, Data.specificationMean, Data._LSL, Data._USL, Data._UOM, Data.sampleSize, Data.inProcessInterval, Data.instrumentType, Data.inspectionDrawing, Data.dataType, Data.setupApprovalInterval, Data.interval, Data.specification, Data.macroLocation, Data._ID, "Update", out successFailure);
					if (successFailure.Equals("Successfull"))
					{
						rowModified = true;
					}
					else
					{
						btnnew.Content = "Cancel";
						DialogBox dialog = new DialogBox("Error Message", successFailure + "", true);
						dialog.ShowDialog();
					}
				}
				if (rowAdded || rowModified)
				{
					DialogBox dialog = new DialogBox("Information Message", "Details added/Updated successfully.", false);
					dialog.ShowDialog();
					rowAdded = false;
					rowModified = false;
					btnview_Click(null, null);
					countafter = 0;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog("Error - \n" + ex.ToString());
				DialogBox frm = new DialogBox("Error Message", ex.Message.ToString(),true);
				frm.ShowDialog();
			}
		}

		private void btncopy_Click(object sender, RoutedEventArgs e)
		{
			if(btncopy.Content.Equals("Copy Inspection Data"))
			{
				//cmbfbno.DropDownStyle = ComboBoxStyle.DropDown;
				//cmbOperation.DropDownStyle = ComboBoxStyle.DropDown;

				btncopy.Content = "Cancel Inspection Data";

				DialogBox frm = new DialogBox("Information Message", "Enter New Component,Operation. Update Data on the Grid and Click on Save to create a Copy of Inspection Data.",false);
				frm.ShowDialog();
				cmbfbno.Focus();
			}
			else
			{
			//	cmbComponent.DropDownStyle = ComboBoxStyle.DropDownList;
			//	cmbOperation.DropDownStyle = ComboBoxStyle.DropDownList;

				btncopy.Content = "Copy Inspection Data";
			}
		}
	}
}
