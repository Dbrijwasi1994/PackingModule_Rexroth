using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.DB_Connection
{
    public static class ConnectionManager
    {
        static string conString = ConfigurationManager.ConnectionStrings["DeburringProcessConnectionString"].ToString();
        static string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static bool timeOut = false;
        public static SqlConnection GetConnection()
        {
            DateTime dt = DateTime.Now;
            SqlConnection conn = new SqlConnection(conString);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                DialogBox frm = new DialogBox("Error Message", "Database Connection Error. Could not connect to database.", true);
                frm.ShowDialog();
                dt = DateTime.Now.AddSeconds(60);
                Logger.WriteErrorLog(ex.Message);
            }
            return conn;
        }
    }
}
