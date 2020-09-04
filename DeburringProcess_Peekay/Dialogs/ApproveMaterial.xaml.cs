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

namespace DeburringProcess_Peekay.Dialogs
{
	/// <summary>
	/// Interaction logic for ApproveMaterial.xaml
	/// </summary>
	public partial class ApproveMaterial : Window
	{
		public ApproveMaterial()
		{
			InitializeComponent();
		}

		private void lblCloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Close();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void btnAccept_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnReject_Click_1(object sender, RoutedEventArgs e)
		{
			if(string.IsNullOrEmpty(txtRejectionReason.Text.ToString()))
			{
				DialogBox Db = new DialogBox("Information","Please enter Rejection Reason.",false);
				Db.ShowDialog();
			}
			//Mouse.OverrideCursor = Cursors.Wait;
			
		}
	}
}
