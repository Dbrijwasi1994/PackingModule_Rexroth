using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
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
using System.Windows.Shapes;

namespace DeburringProcess_Peekay.UserControls
{
	/// <summary>
	/// Interaction logic for DownTime.xaml
	/// </summary>
	public partial class DownTime : Window
	{
		ObservableCollection<Downcode> downlist;
		private Downcode selectedDowncode;
		int dc = int.MinValue;

		public DownTime()
		{
			InitializeComponent();
		}

		private void cmbcatagory_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GetDownData(string.Empty, string.Empty);
		}

		private void listDC_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				var item = ItemsControl.ContainerFromElement(listDownCodes, e.OriginalSource as DependencyObject) as ListBoxItem;

				if (item != null)
				{
					selectedDowncode = item.Content as Downcode;
					item.IsSelected = true;
				}
			}
			catch (Exception)
			{

			}

		}

		private void BindDownCateories()
		{
			List<string> category = DatabaseAccess.GetdownCategory();
			if (category.Count > 0)
			{
				cmbcatagory.ItemsSource = category;
				cmbcatagory.SelectedIndex = 0;
			}
		}

		private void listM_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{ }

		private void Start_Down_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (selectedDowncode != null)
				{
					if (DatabaseAccess.InsertDownCodes("Start", selectedDowncode.InterfaceID))
					{
						dc = Convert.ToInt32(selectedDowncode.InterfaceID);
						lblDowncode.Content = selectedDowncode.DowncodeID;
						HideMenus();
						DialogBox dlgInfo = new DialogBox("Information ", "Down Code Updated Successfully.", false);
						dlgInfo.ShowDialog();
					}
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
		}

		private void HideMenus()
		{
			if (dc <= 0)
			{
				StartDownContextMenu.Visibility = Visibility.Visible;
				UpdateDownContextMenu.Visibility = Visibility.Collapsed;
				EndDownContextMenu.Visibility = Visibility.Collapsed;
			}
			else
			{
				StartDownContextMenu.Visibility = Visibility.Collapsed;
				UpdateDownContextMenu.Visibility = Visibility.Visible;
				EndDownContextMenu.Visibility = Visibility.Visible;
			}
		}

		private void BindSelectedDownCode()
		{
			try
			{
				dc = DatabaseAccess.GetCurrentDown();
				if (dc > 0)
				{
					string dwncd = downlist.Where(l => l.InterfaceID.Equals(dc.ToString())).Select(l => l.DowncodeID).FirstOrDefault();
					if (dwncd != null && !lblDowncode.Content.Equals(dwncd))
					{
						lblDowncode.Content = dwncd;
						try
						{
							int index = downlist.IndexOf(downlist.Where(p => p.InterfaceID == dc.ToString()).FirstOrDefault());
							listDownCodes.SelectedIndex = index;
						}
						catch (Exception)
						{
						}
					}
				}
				else
				{
					lblDowncode.Content = string.Empty;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
		}

		private void Update_Down_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (selectedDowncode != null)
				{
					DatabaseAccess.EndPrevousDownCodes("Close", dc.ToString());

					if (DatabaseAccess.InsertDownCodes("Start", selectedDowncode.InterfaceID))
					{
						lblDowncode.Content = selectedDowncode.DowncodeID;
						dc = Convert.ToInt32(selectedDowncode.InterfaceID);
						HideMenus();
						DialogBox dlgInfo = new DialogBox("Information ", "Down Code Updated Successfully.", false);
						dlgInfo.ShowDialog();
					}
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
		}

		private void End_Down_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (selectedDowncode != null)
				{
					if (DatabaseAccess.InsertDownCodes("Close", dc.ToString()))
					{
						dc = 0;
						lblDowncode.Content = string.Empty;
						HideMenus();
						DialogBox dlgInfo = new DialogBox("Information ", "Down Code Updated Successfully.", false);
						dlgInfo.ShowDialog();
					}
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			BindDownCateories();
			GetDownData(string.Empty, string.Empty);
			BindSelectedDownCode();
			HideMenus();
		}

		private void GetDownData(string downdescription, string downid)
		{
			try
			{
				string Value = string.Empty;

				Value = cmbcatagory.SelectedValue.ToString();

				if (Value == "ALL") Value = string.Empty;
				downlist = DatabaseAccess.GetDowncodeList(Value, downdescription, downid);
				listDownCodes.ItemsSource = downlist;
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);

			}
			finally
			{

			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Utility.CheckInDown())
				e.Cancel = true;
		}
	}
}
