using DeburringProcess_Peekay.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using DeburringProcess_Peekay.Dialogs;
using System.Configuration;

namespace DeburringProcess_Peekay
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        private bool relogin = false;
        public static string LoginUserName = string.Empty;
        private string EmpRole = string.Empty;
        string name, pw;
        public LoginPage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                QRScannerInfo.IpAddress = ConfigurationManager.AppSettings["QRScannerIP"].ToString();
                QRScannerInfo.Port = ConfigurationManager.AppSettings["QRScannerPort"].ToString();
                QRScannerInfo.RegisterAddress = ConfigurationManager.AppSettings["RegisterAddress"].ToString();
                QRScannerInfo.NumOfRegistersToRead = ConfigurationManager.AppSettings["NoOfRegistersToRead"].ToString();
                Utility.InstalledPCType = ConfigurationManager.AppSettings["InstallingPCType"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

        private void btnOK_Clicked(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            getLogindata();
            Mouse.OverrideCursor = null;
        }

        private void getLogindata()
        {
            name = txtName.Text;
            LoginUserName = name;
            pw = txtPassword.Password;
            relogin = DatabaseAccess.mainlogin(name, pw, out bool isAdmin, out EmpRole, out bool IsConnected);
            if (relogin)
            {
                Utility.isAdmin = isAdmin;
                Utility.LoggedInUserName = name;
                Utility.CurrentEmployeeRole = EmpRole;
                Window win = new MainWindow();
                win.Show();
                this.Close();
            }
            else
            {
                if (IsConnected)
                {
                    Mouse.OverrideCursor = null;
                    DialogBox dlg = new DialogBox("Access Denied", "Invalid Credentials", true);
                    dlg.ShowDialog();
                    txtPassword.Password = string.Empty;
                }
            }
        }

        private void txtpass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                getLogindata();
                Mouse.OverrideCursor = null;
            }
        }
    }
}
