using DeburringProcess_Peekay.DB_Connection;
using DeburringProcess_Peekay.Dialogs;
using DeburringProcess_Peekay.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DeburringProcess_Peekay.Helpers
{
    public static class Utility
    {
        public static bool IsAdmin;
        public static bool YesNoAnswer;
        public static string LoggedInUserName;
        public static QRVals Qrdata;
        public static string Process;
        public static bool isAdmin;
        public static string CurrentEmployeeRole;
        public static string InstalledPCType;
        public static string EmployeeRoles = @"Operator,Supervisor,MasterPlanner,SchedulePlanner,ShiftPlanner,Quality Admin,Admin";
        public static string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static bool ShowFinalInspection = bool.Parse(ConfigurationManager.AppSettings["ShowFinalInspection"].ToString());
        public static bool DisableDispatchLogin = bool.Parse(ConfigurationManager.AppSettings["DisableDispatchLogin"].ToString());
        public static List<string> EmployeeRolesList = new List<string>(EmployeeRoles.Split(',').Select(x => x.Trim()));

        static Utility()
        {
            IsAdmin = false;
            YesNoAnswer = false;
            Qrdata = new QRVals();
            LoggedInUserName = string.Empty;
            Process = string.Empty;
            CurrentEmployeeRole = "Operator";
            InstalledPCType = "Operator";
        }

        public static bool CheckInDown()
        {
            bool res = false;
            try
            {
                if (DatabaseAccess.CheckInDown(Utility.Process))
                {
                    DialogBox dlgInfo = new DialogBox("Information ", "Please Close the Down !!!.", true);
                    dlgInfo.ShowDialog();
                    res = true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            return res;
        }

        internal static void OpenExe(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "\"" + path + "\"";
            System.Diagnostics.Process.Start(startInfo);
        }

        public static T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild((prop), i) as DependencyObject;
                if (child == null)
                    continue;

                T castedProp = child as T;
                if (castedProp != null)
                    return castedProp;

                castedProp = GetFirstChildByType<T>(child);

                if (castedProp != null)
                    return castedProp;
            }
            return null;
        }

        public static Dictionary<int, string> GetDaysOfMonth()
        {
            Dictionary<int, string> Days = new Dictionary<int, string>();
            Days.Add(1, "One");
            Days.Add(2, "Two");
            Days.Add(3, "Three");
            Days.Add(4, "Four");
            Days.Add(5, "Five");
            Days.Add(6, "Six");
            Days.Add(7, "Seven");
            Days.Add(8, "Eight");
            Days.Add(9, "Nine");
            Days.Add(10, "Ten");
            Days.Add(11, "Eleven");
            Days.Add(12, "Twelve");
            Days.Add(13, "Thirteen");
            Days.Add(14, "Fourteen");
            Days.Add(15, "Fifteen");
            Days.Add(16, "Sixteen");
            Days.Add(17, "Seventeen");
            Days.Add(18, "Eighteen");
            Days.Add(19, "Nineteen");
            Days.Add(20, "Twenty");
            Days.Add(21, "Twentyone");
            Days.Add(22, "Twentytwo");
            Days.Add(23, "Twentythree");
            Days.Add(24, "Twentyfour");
            Days.Add(25, "Twentyfive");
            Days.Add(26, "Twentysix");
            Days.Add(27, "Twentyseven");
            Days.Add(28, "Twentyeight");
            Days.Add(29, "Twentynine");
            Days.Add(30, "Thirty");
            Days.Add(31, "ThirtyOne");
            return Days;
        }

        public static int GetWeekNumberOfMonth(DateTime date)
        {
            date = date.Date;
            DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
            DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > date)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            }
            return (date - firstMonthMonday).Days / 7 + 1;
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }
}
