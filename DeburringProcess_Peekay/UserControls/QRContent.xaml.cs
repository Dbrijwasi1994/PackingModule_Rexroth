using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Helpers;
using DeburringProcess_Peekay.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace DeburringProcess_Peekay.UserControls
{
	/// <summary>
	/// Interaction logic for QRContent.xaml
	/// </summary>
	public partial class QRContent : Window
	{
		string Batchid;
		public QRContent()
		{
			InitializeComponent();
		}

		public QRContent(QRVals Qrdata)
		{
			InitializeComponent();
			this.DataContext = Qrdata;
		}

		public QRContent(QRVals Qrdata, string batchid)
		{
			InitializeComponent();
			this.DataContext = Qrdata;
			this.Batchid = batchid;
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				//DeburringProcess.GetQRData(this.DataContext as QRVals);
				if(Utility.Process.Equals("dispatch area", StringComparison.OrdinalIgnoreCase))
				{
					var data = this.DataContext as QRVals;
					DatabaseAccess.SaveDispatchdata(data, Batchid);
				}
				else
				{
					var data = this.DataContext as QRVals;
					Utility.Qrdata.BCNO = data.BCNO;
					Utility.Qrdata.CustomerName = data.CustomerName;
					Utility.Qrdata.DrawingNum = data.DrawingNum;
					Utility.Qrdata.FPNO = data.FPNO;
					Utility.Qrdata.Grade = data.Grade;
					Utility.Qrdata.HeatNo = data.HeatNo;
					Utility.Qrdata.MNO = data.MNO;
					Utility.Qrdata.PartNo = data.PartNo;
					Utility.Qrdata.PoNumber = data.PoNumber;
					Utility.Qrdata.Quantity = data.Quantity;
					Utility.Qrdata.SerialNumber = data.SerialNumber;
					Utility.Qrdata.Type = data.Type;
					Utility.Qrdata.UTNO = data.UTNO;
					Utility.Qrdata.Weight = data.Weight;
					Utility.Qrdata.WorkOrderNumber = data.WorkOrderNumber;
					Utility.Qrdata.FpDesc = data.FpDesc;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
			finally
			{
				this.Close();
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Utility.Qrdata.BCNO = string.Empty;
				Utility.Qrdata.CustomerName = string.Empty;
				Utility.Qrdata.DrawingNum = string.Empty;
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
				Utility.Qrdata.FpDesc = string.Empty;

			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
			finally
			{
				this.Close();
			}
		}
	}
}
