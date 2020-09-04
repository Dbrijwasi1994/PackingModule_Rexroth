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
	public partial class ProductionData : UserControl
	{
		List<ProductionDataEntity> dbentry = null;
		//List<ProductionDataEntity> dbentryNew = null;
		List<ComboboxEntity> ComboboxLists = null;
		QRVals qrdata = new QRVals();
		QRVals datatt = new QRVals();
		string Process = ConfigurationManager.AppSettings["Process"].ToString();


		public ProductionData()
		{
			InitializeComponent();
		}

		private void ProductionData_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				if (Process.Equals("Dispatch Area", StringComparison.OrdinalIgnoreCase))
				{
					productiondatadatagrid.Columns[5].Visibility = Visibility.Hidden;
					productiondatadatagrid.Columns[7].Visibility = Visibility.Hidden;
				}
				else
				{
					productiondatadatagrid.Columns[5].Visibility = Visibility.Visible;
					productiondatadatagrid.Columns[7].Visibility = Visibility.Visible;
				}
				Mouse.OverrideCursor = null;
				fromdate.SelectedDate = DateTime.Now.AddDays(-2);
				todate.SelectedDate = DateTime.Now;
				bindcombobox();
				string FromDate = DatabaseAccess.GetLogicalDay(fromdate.SelectedDate.Value);
				string ToDate = DatabaseAccess.GetLogicalDayEnd(todate.SelectedDate.Value);
				dbentry = DatabaseAccess.GetProductiondatabydate(Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd HH:mm:ss"));
				productiondatadatagrid.ItemsSource = dbentry;
				Utility.Process = Process;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			ShowDownPagePopUp();
			Keyboard.Focus(productiondatadatagrid);
		}

		private void bindcombobox()
		{
			try
			{
				ComboboxLists = DatabaseAccess.GetcomboboxLists();
				var HeatNumberList = ComboboxLists.Select(o => o.HeatNumber).Distinct().ToList();
				if (HeatNumberList != null)
				{
					cmbheatnumber.ItemsSource = HeatNumberList;
				}
				var MpiNumberList = ComboboxLists.Where(o => o.MPINumber != null).Select(o => o.MPINumber).Distinct().ToList();
				if (MpiNumberList != null)
				{
					cmbMpinumber.ItemsSource = MpiNumberList;
				}
				var UTNumberList = ComboboxLists.Select(o => o.UTNumber).Distinct().ToList();
				if (UTNumberList != null)
				{
					cmbutnumber.ItemsSource = UTNumberList;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}
		}

		private void ShowDownPagePopUp()
		{
			if (Utility.CheckInDown())
			{
				DownTime dt = new DownTime();
				dt.ShowDialog();
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

		private void btnSearchbyparameter_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if(cmbutnumber.SelectedValue !=null)
				{
					if (!(string.IsNullOrEmpty(cmbutnumber.SelectedValue.ToString())))
					{
						dbentry = DatabaseAccess.GetProductiondatabydate(cmbutnumber.SelectedValue.ToString());
						if (dbentry != null && dbentry.Count > 0)
							productiondatadatagrid.ItemsSource = dbentry;
						else
						{
							DialogBox dlgInfo = new DialogBox("Information ", "No Data For Selected NDT Number.", false);
							dlgInfo.ShowDialog();
							productiondatadatagrid.ItemsSource = new List<ProductionDataEntity>();
							return;
						}
					}
					else
					{
						DialogBox dlgInfo = new DialogBox("Information ", "NDT Number Not Found.", false);
						dlgInfo.ShowDialog();
						productiondatadatagrid.ItemsSource = new List<ProductionDataEntity>();
						return;
					}
				}
				else
				{
					DialogBox dlgInfo = new DialogBox("Information ", "Please Select Valid NDT Number.", false);
					dlgInfo.ShowDialog();
					return;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}
		}

		private void btnSearchbydate_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string FromDate = DatabaseAccess.GetLogicalDay(fromdate.SelectedDate.Value);
				string ToDate = DatabaseAccess.GetLogicalDayEnd(todate.SelectedDate.Value);
				dbentry = DatabaseAccess.GetProductiondatabydate(Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd HH:mm:ss"));
				if (dbentry != null && dbentry.Count > 0)
					productiondatadatagrid.ItemsSource = dbentry;
				else
				{
					DialogBox dlgInfo = new DialogBox("Information ", "No Data For Selected Date.", false);
					dlgInfo.ShowDialog();
					productiondatadatagrid.ItemsSource = new List<ProductionDataEntity>();
					return;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}
		}

		private void cmbMpinumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (cmbMpinumber.SelectedValue == null) return;
				var results = ComboboxLists.Where(x => x.MPINumber == cmbMpinumber.SelectedValue.ToString()).Distinct().ToList();
				//cmbutnumber.ItemsSource = results.Select(o => o.UTNumber).Distinct().ToList();
				if (results.Count > 0)
				{
					cmbheatnumber.SelectedItem = results[0].HeatNumber;
					cmbutnumber.SelectedItem = results[0].UTNumber;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}
		}

		private void cmbheatnumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (cmbheatnumber.SelectedValue == null) return;
				var results = ComboboxLists.Where(x => x.HeatNumber == cmbheatnumber.SelectedValue.ToString()).Distinct().ToList();
				//cmbutnumber.ItemsSource = results.Select(o => o.UTNumber).Distinct().ToList();
				if (results.Count > 0)
				{
					cmbMpinumber.SelectedItem = results[0].MPINumber;
					cmbutnumber.SelectedItem = results[0].UTNumber;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}

		}

		private void cmbutnumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (cmbheatnumber.SelectedValue == null) return;
				var results = ComboboxLists.Where(x => x.UTNumber == cmbutnumber.SelectedValue.ToString()).Distinct().ToList();
				//cmbutnumber.ItemsSource = results.Select(o => o.UTNumber).Distinct().ToList();
				if (results.Count > 0)
				{
					cmbMpinumber.SelectedItem = results[0].MPINumber;
					cmbheatnumber.SelectedItem = results[0].HeatNumber;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}
		}
	}
}
