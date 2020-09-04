using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Helpers
{
    class QRScannerInfo
    {
        public static string IpAddress { get; set; }
        public static string Port { get; set; }
        public static string RegisterAddress { get; set; }
        public static string NumOfRegistersToRead { get; set; }
    }
}
