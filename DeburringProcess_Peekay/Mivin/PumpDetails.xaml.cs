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

namespace DeburringProcess_Peekay.Mivin
{
    /// <summary>
    /// Interaction logic for PumpDetails.xaml
    /// </summary>
    public partial class PumpDetails : Window
    {
        public static PumpQRData SelectedPumpDetails = null;

        public PumpDetails()
        {
            InitializeComponent();
            SelectedPumpDetails = new PumpQRData();
        }

        private void EnterPumpDetails_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPumpModel.Text) || string.IsNullOrEmpty(txtPumpSlNo.Text))
            {
                MessageBox.Show("Both fields are mandatory.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                SelectedPumpDetails.PumpModel = txtPumpModel.Text;
                SelectedPumpDetails.PumpSerialNum = txtPumpSlNo.Text;
                if (!string.IsNullOrEmpty(SelectedPumpDetails.PumpModel) && !string.IsNullOrEmpty(SelectedPumpDetails.PumpSerialNum))
                    this.Close();
            }
        }
    }
}
