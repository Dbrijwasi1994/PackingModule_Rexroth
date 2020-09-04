using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Helpers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeburringProcess_Peekay.UserControls
{
	/// <summary>
	/// Interaction logic for DownTimeSummary.xaml
	/// </summary>
	public partial class DownTimeSummary : UserControl
	{
		public DownTimeSummary()
		{
			InitializeComponent();
		}

		private void btnView_Click(object sender, RoutedEventArgs e)
		{
			BindGrid();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			BindDates();

			BindGrid();
		}

		private void BindDates()
		{
			string toDt = string.Empty;
			fromDate.Text = DatabaseAccess.GetLogicalDayStartEnd(out toDt);
			toDate.Text = toDt;
		}

		private void BindGrid()
		{
			try
			{
				Mouse.OverrideCursor = Cursors.Wait;
				dgDownTimeSummary.ItemsSource = DatabaseAccess.GetDownSummary(fromDate.Value.Value, toDate.Value.Value);
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
