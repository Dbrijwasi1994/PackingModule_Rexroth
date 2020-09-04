using DeburringProcess_Peekay.Dialogs;
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

namespace DeburringProcess_Peekay.PDFViewer
{
	/// <summary>
	/// Interaction logic for PdfViewer.xaml
	/// </summary>
	public partial class PdfViewer : UserControl
	{
		public PdfViewer()
		{
			InitializeComponent();
		}
		public PdfViewer(string path)
		{
			InitializeComponent();
			try
			{
				pdfWebViewer.Navigate(path);
			}
			catch (Exception ex)
			{
				DialogBox dlgInfo = new DialogBox("Error ", ex.Message, true);
				dlgInfo.ShowDialog();
			}
		}
	}
}
