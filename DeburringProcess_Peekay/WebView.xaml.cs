using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

namespace DeburringProcess_Peekay
{
	/// <summary>
	/// Interaction logic for WebView.xaml
	/// </summary>
	public partial class WebView : Window
	{
		public WebView()
		{
			InitializeComponent();
		}

		private string _url = string.Empty;
		public WebView(string Url)
		{
			_url = Url;
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(_url))
				{
					myUrl.Text = _url;
					webViewer.Navigate(_url);
				}

				else
				{
					DialogBox dlg = new DialogBox("Information", "Please Enter Work Order URL !!!", true);
					dlg.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
		}

		// Reload the current page function.
		private void BrowserRefresh_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				webViewer.Refresh();
				myUrl.Text = webViewer.Source.ToString();
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
		}
		// from one page to another.
		private void myBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
		{
			_url = myUrl.Text;
		}

		private void myBrowser_Navigated(object sender, NavigationEventArgs e)
		{
			try
			{
				myUrl.Text = webViewer.Source.ToString();
				SetSilent(webViewer, true); // make it silent
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
		}

		public static void SetSilent(WebBrowser browser, bool silent)
		{
			try
			{
				if (browser == null)
					throw new ArgumentNullException("browser");

				// get an IWebBrowser2 from the document
				IOleServiceProvider sp = browser.Document as IOleServiceProvider;
				if (sp != null)
				{
					Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
					Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

					object webBrowser;
					sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
					if (webBrowser != null)
					{
						webBrowser.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { silent });
					}
				}
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
		}


		[ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IOleServiceProvider
		{
			[PreserveSig]
			int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
		}

		private void myBrowser_LoadCompleted(object sender, NavigationEventArgs e)
		{
			try
			{
				myUrl.Text = webViewer.Source.ToString();
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.ToString());
			}
		}
	}
}
