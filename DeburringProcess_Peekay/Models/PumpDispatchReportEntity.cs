using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Models
{
    class PumpDispatchReportEntity
    {
        public string Sl_No { get; set; }
        public string StationID { get; set; }
        public string PumpPart_No { get; set; }
        public string Quantity { get; set; }
        public string BoxType_No { get; set; }
        public string PackageType { get; set; }
        public string Remarks { get; set; }
    }
}
