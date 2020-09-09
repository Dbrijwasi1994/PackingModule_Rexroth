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
using System.Windows.Shapes;

namespace DeburringProcess_Peekay.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogBoxYesNo.xaml
    /// </summary>
    public partial class DialogBoxYesNo : Window
    {
        public DialogBoxYesNo()
        {
            InitializeComponent();
        }

        public DialogBoxYesNo(string headerMsg, string msg)
        {
            InitializeComponent();
            lblHeader.Content = headerMsg;
            lblAccTxt.Text = msg;
            btnNo.Focus();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            Utility.YesNoAnswer = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            Utility.YesNoAnswer = false;
            this.Close();
        }
    }
}
