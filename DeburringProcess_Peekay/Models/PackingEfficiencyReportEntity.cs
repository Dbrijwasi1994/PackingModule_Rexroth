using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Models
{
    class PackingEfficiencyReportEntity
    {
        public string Date { get; set; }
        public string StationID { get; set; }
        public string Operator { get; set; }
        public string Shift { get; set; }
        public string WorkOrderNumber { get; set; }
        public string Customer { get; set; }
        public string PumpModel { get; set; }
        public string PackedQuantity { get; set; }
        public string ShiftTarget { get; set; }
        public string ShfitEfficiency { get; set; }
        public string Remarks { get; set; }
    }
}
