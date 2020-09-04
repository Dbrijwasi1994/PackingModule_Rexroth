using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeburringProcess_Peekay.UserControls
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : UserControl
	{
		public Settings()
		{
			InitializeComponent();
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				DatabaseAccess.FolderPathDefinition("WorkInsNonMachininig", txtbrowseWI.Text, "pdf");
				DatabaseAccess.FolderPathDefinition("ProcDocsNonMachininig", txtbrowsePD.Text, "pdf");
			}
			catch (Exception ex)
			{
				DialogBox db = new DialogBox("Error", ex.Message, true);
				db.ShowDialog();
			}
		}


		private void browse_Click(object sender, RoutedEventArgs e)
		{
			setPathVals(txtbrowseWI);
		}

		private void browsePD_Click(object sender, RoutedEventArgs e)
		{
			setPathVals(txtbrowsePD);
		}

		private void setPathVals(TextBox txtBox)
		{
			string path = DefinePath();
			if (!string.IsNullOrEmpty(path))
			{
				txtBox.Text = path;
			}
		}

		private string DefinePath()
		{
			System.Windows.Forms.FolderBrowserDialog Dialog = new System.Windows.Forms.FolderBrowserDialog();
			Dialog.ShowDialog();
			//while (Dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			//{
			//	Dialog.Reset();
			//}
			return Dialog.SelectedPath;
		}

		private void BindTextBoxes()
		{
			var pd = GetAllPaths();
			if (pd != null)
			{
				txtbrowsePD.Text = pd.PROC_DOC_PATH;
				txtbrowseWI.Text = pd.WORK_INST_PATH;
			}
		}

		private PathDetails GetAllPaths()
		{
			return DatabaseAccess.GetAllPaths();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			BindTextBoxes();
		}
	}
}
