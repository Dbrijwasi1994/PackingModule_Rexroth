using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Helpers
{
    public static class Logger
    {
        private static string appPath;
        private static object _locker;

        static Logger()
        {
            string ProgTime = String.Format("{0:dd_MMM_yyyy}", DateTime.Now);
            appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); //Environment.CurrentDirectory;
            _locker = new object();

            string LogsFolderPath = appPath + @"\Logs";
            if (Directory.Exists(LogsFolderPath) == false)
            {
                Directory.CreateDirectory(LogsFolderPath);
            }
            appPath = Path.Combine(appPath, @"Logs\TrackAndTrace" + ProgTime + ".txt");
        }

        public static void WriteDebugLog(string str)
        {
            StreamWriter writer = null;
            if (Monitor.TryEnter(_locker, 1000))
            {
                try
                {
                    writer = new StreamWriter(appPath, true, Encoding.UTF8, 8195);
                    writer.WriteLine(string.Format("{0} : DEBUG - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"), str));
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                    writer = null;
                }
                catch { }
                finally
                {
                    Monitor.Exit(_locker);
                    if (writer != null)
                    {
                        writer.Close();
                        writer.Dispose();
                    }
                }
            }
        }

        public static void WriteErrorLog(string str)
        {
            StreamWriter writer = null;
            if (Monitor.TryEnter(_locker, 1000))
            {
                try
                {
                    writer = new StreamWriter(appPath, true, Encoding.UTF8, 8195);
                    writer.WriteLine(string.Format("{0} : EXCEPTION - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"), str));
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                    writer = null;
                }
                catch { }
                finally
                {
                    Monitor.Exit(_locker);
                    if (writer != null)
                    {
                        writer.Close();
                        writer.Dispose();
                    }
                }
            }
        }

        public static void WriteErrorLog(Exception ex)
        {
            StreamWriter writer = null;
            if (Monitor.TryEnter(_locker, 1000))
            {
                try
                {
                    writer = new StreamWriter(appPath, true, Encoding.UTF8, 8195);
                    writer.WriteLine(string.Format("{0} : Exception - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"), ex));
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                    writer = null;
                }
                catch { }
                finally
                {
                    Monitor.Exit(_locker);
                    if (writer != null)
                    {
                        writer.Close();
                        writer.Dispose();
                    }
                }
            }
        }
    }
}
