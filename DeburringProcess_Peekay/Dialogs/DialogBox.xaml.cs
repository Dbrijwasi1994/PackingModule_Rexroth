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
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogBox : Window
    {
        BrushConverter brushConverter = new BrushConverter();
        public DialogBox()
        {
            InitializeComponent();
        }

        public DialogBox(string headerMsg, string msg, bool IsError)
        {
            InitializeComponent();

            lblHeader.Content = headerMsg;
            lblAccTxt.Text = msg;
            if (IsError)
            {
                gridHeader.Background = Brushes.Red;
                lblImg.Background = Brushes.Red;
                lblHeader.Background = Brushes.Red;
                lblCloseButton.Background = Brushes.Red;
                btnOK.Background = Brushes.Red;
                lbltext.Background = (Brush)brushConverter.ConvertFrom("#FFE0E0");
                lblAccTxt.Background = (Brush)brushConverter.ConvertFrom("#FFE0E0");
                gridContainer.Background = (Brush)brushConverter.ConvertFrom("#FFC0C0");
            }
            btnOK.Focus();
        }

        private void CloseButton_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                this.Close();
            }
        }
    }
}
