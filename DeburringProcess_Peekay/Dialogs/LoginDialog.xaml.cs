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
using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Helpers;

namespace DeburringProcess_Peekay.Dialogs
{
	/// <summary>
	/// Interaction logic for LoginDialog.xaml
	/// </summary>
	public partial class LoginDialog : Window
	{
		public LoginDialog()
		{
			InitializeComponent();
			this.txtUsername.Focus();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			this.Close();
		}

		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{
			bool ok;
			ok = DatabaseAccess.login(txtUsername.Text, txtPassword.Password);
			if (ok)
			{
				Utility.LoggedInUserName = txtUsername.Text;
				DialogResult = true;
				this.Close();
			}
			else
			{
				DialogBox dlgError = new DialogBox("Error!", "The user name or password you entered isn't correct. Try entering it again.", true);
				dlgError.ShowDialog();
				this.txtUsername.Focus();
			}
		}

		private void txtPassword_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				btnLogin_Click(null, null);
			}
			if (e.Key == Key.Tab)
			{
				btnLogin.Focus();
			}
		}

		private void txtUsername_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Tab)
			{
				txtPassword.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
			}
		}
	}
}
