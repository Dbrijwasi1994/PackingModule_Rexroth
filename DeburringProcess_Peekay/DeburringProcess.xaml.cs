using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Models;
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
using System.Windows.Forms.Integration;
using Newtonsoft.Json;
using DeburringProcess_Peekay.Helpers;
using System.Configuration;
using DeburringProcess_Peekay.UserControls;

namespace DeburringProcess_Peekay
{
	/// <summary>
	/// Interaction logic for DeburringProcess.xaml
	/// </summary>
	public partial class DeburringProcess : UserControl
	{
		List<DeburringProcessEntity> dbentry = null;
		List<DeburringProcessEntity> dbentryNew = null;
		QRVals qrdata = new QRVals();
		QRVals datatt = new QRVals();
		string Process = ConfigurationManager.AppSettings["Process"].ToString();


		public DeburringProcess()
		{
			InitializeComponent();
		}

		private void Deburring_Process_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				dbentry = DatabaseAccess.recievedata("Default", Process);
				dgDeburringProcess.ItemsSource = dbentry;
				SetParamValues();
				Utility.Process = Process;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			ShowDownPagePopUp();
			Keyboard.Focus(dgDeburringProcess);
		}

		private void SetParamValues()
		{
			Utility.Qrdata.BCNO = string.Empty;
			Utility.Qrdata.CustomerName = string.Empty;
			Utility.Qrdata.DrawingNum = string.Empty;
			Utility.Qrdata.FpDesc = string.Empty;
			Utility.Qrdata.FPNO = string.Empty;
			Utility.Qrdata.Grade = string.Empty;
			Utility.Qrdata.HeatNo = string.Empty;
			Utility.Qrdata.MNO = string.Empty;
			Utility.Qrdata.PartNo = string.Empty;
			Utility.Qrdata.PoNumber = string.Empty;
			Utility.Qrdata.Quantity = string.Empty;
			Utility.Qrdata.SerialNumber = string.Empty;
			Utility.Qrdata.Type = string.Empty;
			Utility.Qrdata.UTNO = string.Empty;
			Utility.Qrdata.Weight = string.Empty;
			Utility.Qrdata.WorkOrderNumber = string.Empty;
			gridParams.DataContext = Utility.Qrdata;
			Keyboard.Focus(dgDeburringProcess);
		}

		private void ShowDownPagePopUp()
		{
			if (Utility.CheckInDown())
			{
				DownTime dt = new DownTime();
				dt.ShowDialog();
			}
		}

		private void btnStart_Click(object sender, RoutedEventArgs e)
		{
			if (Utility.CheckInDown()) return;
			DeburringProcessEntity data = new DeburringProcessEntity();
			data = dgDeburringProcess.SelectedItem as DeburringProcessEntity;
			data.StartendID++;
			DeburringProcessEntity currentDeburringdata = dgDeburringProcess.SelectedItem as DeburringProcessEntity;


			//if (Process.Equals("Dispatch Area", StringComparison.OrdinalIgnoreCase))
			if (Utility.DisableDispatchLogin)
			{
				#region Dispatch
				bool IsUpdated = false;
				data.Operator = Utility.LoggedInUserName;
				if (data != null)
				{
					if (data.Remarks == null)
					{
						data.Remarks = "";
					}
					DatabaseAccess.StartEndScanout(Process, data.FPNumber, data.UTNumber, data.Operator, "Start", DateTime.Now, data.Remarks, data.BatchId,data.StartendID, out IsUpdated);
				}
				currentDeburringdata.StartVisibility = "Collapsed";
				currentDeburringdata.TxtStartVisibility = "Visible";
				currentDeburringdata.StartDateTime = DateTime.Now;
				currentDeburringdata.IsEndEnabled = true;
				currentDeburringdata.IsStartEnabled = false;
				currentDeburringdata.TxtEndVisibility = "Collapsed";
				currentDeburringdata.EndVisibility = "Visible";
				currentDeburringdata.IsScanOutEnabled = false;
				currentDeburringdata.ScanOutVisibility = "Visible";
				currentDeburringdata.TxtScanOutVisibility = "Collapsed";
				#endregion
			}
			else
			{
				LoginDialog supLoginStart = new LoginDialog();
				bool result = (bool)supLoginStart.ShowDialog();
				if (result)
				{
					bool IsUpdated = false;
					data.Operator = Utility.LoggedInUserName;
					if (data != null)
					{
						if (data.Remarks == null)
						{
							data.Remarks = "";
						}
						DatabaseAccess.StartEndScanout(Process, data.FPNumber, data.UTNumber, data.Operator, "Start", DateTime.Now, data.Remarks, data.BatchId,data.StartendID, out IsUpdated);
					}
					currentDeburringdata.StartVisibility = "Collapsed";
					currentDeburringdata.TxtStartVisibility = "Visible";
					currentDeburringdata.StartDateTime = DateTime.Now;
					currentDeburringdata.IsEndEnabled = true;
					currentDeburringdata.IsStartEnabled = false;
					currentDeburringdata.TxtEndVisibility = "Collapsed";
					currentDeburringdata.EndVisibility = "Visible";
					currentDeburringdata.IsScanOutEnabled = false;
					currentDeburringdata.ScanOutVisibility = "Visible";
					currentDeburringdata.TxtScanOutVisibility = "Collapsed";
				}
			}


			Keyboard.Focus(dgDeburringProcess);
		}

		private void btnEnd_Click(object sender, RoutedEventArgs e)
		{
			if (Utility.CheckInDown()) return;

			DeburringProcessEntity data = new DeburringProcessEntity();
			data = dgDeburringProcess.SelectedItem as DeburringProcessEntity;
			bool IsUpdated = false;
			DeburringProcessEntity currentDeburringdata = dgDeburringProcess.SelectedItem as DeburringProcessEntity;
			//if (Process.Equals("Dispatch Area", StringComparison.OrdinalIgnoreCase))
			if (Utility.DisableDispatchLogin)
			{
				#region Dispatch
				data.Operator = Utility.LoggedInUserName;
				if (data != null)
				{
					if (data.Remarks == null)
					{
						data.Remarks = "";
					}
					DatabaseAccess.StartEndScanout(Process, data.FPNumber, data.UTNumber, data.Operator, "End", DateTime.Now, data.Remarks, data.BatchId, data.StartendID, out IsUpdated);
				}
				currentDeburringdata.EndVisibility = "Collapsed";
				currentDeburringdata.TxtEndVisibility = "Visible";
				currentDeburringdata.EndDateTime = DateTime.Now;
				currentDeburringdata.IsScanOutEnabled = true;
				currentDeburringdata.ScanOutVisibility = "Visible";
				currentDeburringdata.TxtScanOutVisibility = "Collapsed";
				currentDeburringdata.IsEndEnabled = false;
				currentDeburringdata.IsStartEnabled = true;
				currentDeburringdata.TxtStartVisibility = "Collapsed";
				currentDeburringdata.StartVisibility = "Visible";
				#endregion
			}
			else
			{
				LoginDialog supLoginEnd = new LoginDialog();
				bool result = (bool)supLoginEnd.ShowDialog();
				if (result)
				{
					data.Operator = Utility.LoggedInUserName;
					if (data != null)
					{
						if (data.Remarks == null)
						{
							data.Remarks = "";
						}
						DatabaseAccess.StartEndScanout(Process, data.FPNumber, data.UTNumber, data.Operator, "End", DateTime.Now, data.Remarks, data.BatchId, data.StartendID, out IsUpdated);
					}
					currentDeburringdata.EndVisibility = "Collapsed";
					currentDeburringdata.TxtEndVisibility = "Visible";
					currentDeburringdata.EndDateTime = DateTime.Now;
					currentDeburringdata.IsScanOutEnabled = true;
					currentDeburringdata.ScanOutVisibility = "Visible";
					currentDeburringdata.TxtScanOutVisibility = "Collapsed";
					currentDeburringdata.IsEndEnabled = false;
					currentDeburringdata.IsStartEnabled = true;
					currentDeburringdata.TxtStartVisibility = "Collapsed";
					currentDeburringdata.StartVisibility = "Visible";
				}
			}

			Keyboard.Focus(dgDeburringProcess);
		}

		internal static void RefreshData(QRVals data)
		{
			//if (DatabaseAccess.IsAlreadyScannedOut(data.UTNO, "Scanout"))
			//{
			//	DialogBox dlgInfo = new DialogBox("Information ", "This material has already been scanned-Out.", false);
			//	dlgInfo.ShowDialog();
			//	return;
			//}
			//else
			string process = string.Empty;
			try
			{
				process = DatabaseAccess.Checkforutnoprocessed(data.UTNO);
				if((string.IsNullOrEmpty(process)))
				{
					data = DatabaseAccess.search(data.UTNO);
					QRContent qrData = new QRContent(data);
					qrData.ShowDialog();
				}
				else
				{
					DialogBox dlgInfo = new DialogBox("Information ", "The UT-Number Scanned is already getinng processed in "+process + " Area.", false);
					dlgInfo.ShowDialog();
					return;
				}

			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}

			//if (DatabaseAccess.IsAlreadyScannedOut(data.UTNO, "Scanin"))
			//{
			//	DialogBox dlgInfo = new DialogBox("Information ", "This material has already been scanned-In.", false);
			//	dlgInfo.ShowDialog();
			//	return;
			//}
			//else
			//{

			//}

		}

		private void btnScanOut_Click(object sender, RoutedEventArgs e)
		{
			if (Utility.CheckInDown()) return;

			DeburringProcessEntity data = new DeburringProcessEntity();
			data = dgDeburringProcess.SelectedItem as DeburringProcessEntity;
			bool IsUpdated = false;
			DeburringProcessEntity currentDeburringdata = dgDeburringProcess.SelectedItem as DeburringProcessEntity;
			//if (Process.Equals("Dispatch Area", StringComparison.OrdinalIgnoreCase))
			if (Utility.DisableDispatchLogin)
			{
				data.Operator = Utility.LoggedInUserName;
				if (data != null)
				{
					if (data.Remarks == null)
					{
						data.Remarks = "";
					}
					DatabaseAccess.StartEndScanout(Process, data.FPNumber, data.UTNumber, data.Operator, "Scanout", DateTime.Now, data.Remarks, data.BatchId, data.StartendID, out IsUpdated);
				}
				//DatabaseAccess.SaveDispatchScanin(Process, data.FPNumber, data.UTNumber, data.Operator, "Scanout", DateTime.Now, data.Remarks, data.BatchId);
				currentDeburringdata.ScanOutVisibility = "Collapsed";
				currentDeburringdata.TxtScanOutVisibility = "Visible";
				currentDeburringdata.ScanOutTime = DateTime.Now;
				currentDeburringdata.IsScanOutEnabled = false;
				currentDeburringdata.IsEndEnabled = false;
				currentDeburringdata.IsStartEnabled = false;
				currentDeburringdata.EndVisibility = "Collapsed";
				currentDeburringdata.TxtEndVisibility = "Visible";
				currentDeburringdata.StartVisibility = "Collapsed";
				currentDeburringdata.TxtStartVisibility = "Visible";

			}
			else
			{
				LoginDialog supLoginScanOut = new LoginDialog();
				bool result = (bool)supLoginScanOut.ShowDialog();
				if (result)
				{
					data.Operator = Utility.LoggedInUserName;
					if (data != null)
					{
						if (data.Remarks == null)
						{
							data.Remarks = "";
						}
						DatabaseAccess.StartEndScanout(Process, data.FPNumber, data.UTNumber, data.Operator, "Scanout", DateTime.Now, data.Remarks, data.BatchId, data.StartendID, out IsUpdated);
					}

					currentDeburringdata.ScanOutVisibility = "Collapsed";
					currentDeburringdata.TxtScanOutVisibility = "Visible";
					currentDeburringdata.ScanOutTime = DateTime.Now;
					currentDeburringdata.IsScanOutEnabled = false;
					currentDeburringdata.IsEndEnabled = false;
					currentDeburringdata.IsStartEnabled = false;
					currentDeburringdata.EndVisibility = "Collapsed";
					currentDeburringdata.TxtEndVisibility = "Visible";
					currentDeburringdata.StartVisibility = "Collapsed";
					currentDeburringdata.TxtStartVisibility = "Visible";
				}
			}

			Keyboard.Focus(dgDeburringProcess);
		}

		private void btnReceive_Click(object sender, RoutedEventArgs e)
		{
			if (Utility.CheckInDown()) return;

			if (DatabaseAccess.IsAlreadyScannedOut(txtUTno.Content.ToString().Trim(), "Scanin"))
			{
				DialogBox dlgInfo = new DialogBox("Information ", "This material has already been scanned-In.", false);
				dlgInfo.ShowDialog();
				return;
			}

			if ((txtUTno.Content != null) && (txtFPno.Content != null) && (txtUTno.Content.ToString() != "") && (txtFPno.Content.ToString() != ""))
			{
				DatabaseAccess.dataentry(Process, txtUTno.Content.ToString(), txtFPno.Content.ToString(), "Scanin", DateTime.Now);
				RefreshDeburringData();
			}
			else
			{
				DialogBox dlgError = new DialogBox("Information", "No data to Scan-In.", false);
				dlgError.ShowDialog();
			}
			SetParamValues();
			btnReceive.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
			Keyboard.Focus(dgDeburringProcess);
		}

		private void RefreshDeburringData()
		{
			DateTime StartDate, EndDate;
			StartDate = DatabaseAccess.startdatetime();
			EndDate = DatabaseAccess.enddatetime();
			dbentryNew = DatabaseAccess.recievedata("", Process);
			dgDeburringProcess.ItemsSource = dbentryNew;
			Keyboard.Focus(dgDeburringProcess);
		}

		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			if (txtUtNum.Text != "")
			{
				qrdata = DatabaseAccess.search(txtUtNum.Text);
				string process = DatabaseAccess.Checkforutnoprocessed(txtUtNum.Text);
				if (!(string.IsNullOrEmpty(process)))
				{
					DialogBox dlgInfo = new DialogBox("Information ", "The UT-Number Scanned is already getting processed in " + process + ".", false);
					dlgInfo.ShowDialog();
					return;
				}
				else if (qrdata != null && !(string.IsNullOrEmpty(qrdata.FPNO)))
				{
					//if (DatabaseAccess.IsAlreadyScannedOut(qrdata.UTNO, "Scanout"))
					//{
					//	DialogBox dlgInfo = new DialogBox("Information", "This material has already been Scanned-Out.", false);
					//	dlgInfo.ShowDialog();
					//	return;
					//}
					//else 
					if (DatabaseAccess.IsAlreadyScannedOut(qrdata.UTNO, "Scanin"))
					{
						DialogBox dlgInfo = new DialogBox("Information", "This material has already been Scanned-In.", false);
						dlgInfo.ShowDialog();
						return;
					}
					else
					{
						txtMno.Content = qrdata.MNO;
						txtUTno.Content = qrdata.UTNO;
						txtFPno.Content = qrdata.FPNO;
						txtFPDesc.Content = qrdata.FpDesc;
						txtHeatNum.Content = qrdata.HeatNo;
						txtPartNumber.Content = qrdata.PartNo;
						txtBarcode.Content = qrdata.BCNO;
						txtUtNum.Text = string.Empty;
						txtdrawingNum.Content = qrdata.DrawingNum;
						txtCustName.Content = qrdata.CustomerName;
						txtWOno.Content = qrdata.WorkOrderNumber;
					}
				}
				else
				{
					DialogBox dlg = new DialogBox("ERROR", "No data for entered NDT number", true);
					dlg.ShowDialog();
					txtUtNum.Text = string.Empty;
				}
				Keyboard.Focus(dgDeburringProcess);
			}
			else
			{
				DialogBox dlg = new DialogBox("ERROR", "Enter NDT number", true);
				dlg.ShowDialog();
				txtUtNum.Text = string.Empty;
				Keyboard.Focus(dgDeburringProcess);
			}
			txtUtNum.Text = string.Empty;
			Keyboard.Focus(dgDeburringProcess);
		}

		private void btnreportSave_Click(object sender, RoutedEventArgs e)
		{
			if (Utility.CheckInDown()) return;

			bool IsUpdated = false;
			DeburringProcessEntity data = new DeburringProcessEntity();
			data = dgDeburringProcess.SelectedItem as DeburringProcessEntity;
			if (data != null)
			{
				DatabaseAccess.StartEndScanout(Process, data.FPNumber, data.UTNumber, data.Operator, "Remarks", DateTime.Now, data.Remarks, data.BatchId, data.StartendID, out IsUpdated);
			}
			if (IsUpdated)
			{
				DialogBox db = new DialogBox("Information", "Updated Successfully", false);
				db.ShowDialog();
				RefreshDeburringData();
			}
			Keyboard.Focus(dgDeburringProcess);
		}

		private void txtUtNum_TextChanged(object sender, TextChangedEventArgs e)
		{
			QRVals data = new QRVals();
			if (txtUtNum.Text.Contains("{") && txtUtNum.Text.Contains("}"))
			{
				string msg = txtUtNum.Text;
				txtUtNum.Text = string.Empty;
				data = JsonConvert.DeserializeObject<QRVals>(msg);
				txtUtNum.Text = data.UTNO.ToString();
				Keyboard.Focus(dgDeburringProcess);
			}
			if (txtUtNum.Text == "" && txtUtNum.Text == null)
			{
				Keyboard.Focus(dgDeburringProcess);
			}
		}

		private void WOlink_Click(object sender, RoutedEventArgs e)
		{
			Hyperlink link = (Hyperlink)e.OriginalSource;
			Mouse.OverrideCursor = Cursors.Wait;
			try
			{
				var pd = DatabaseAccess.GetAllPaths();
				if (pd == null) return;
				WebView WebVw = new WebView(string.Format(pd.WO_URL, link.NavigateUri.OriginalString));
				WebVw.Owner = Window.GetWindow(this);
				WebVw.ShowDialog();
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
			finally
			{
				Mouse.OverrideCursor = null;
			}
		}
	}
}
