using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackingModule_Rexroth.Models
{
    class MonthlyfulfilmentReportEntity
    {
        public string Type { get; set; }
        public string Customer { get; set; }
        public string PumpPartNumber { get; set; }
        public int MonthRequirement { get; set; }
        public int ActualPackedQty { get; set; }
        public int PendingForPacking { get; set; }
        public string ActualDispatch { get; set; }
        public int CW1 { get; set; }
        public int CW1Actual { get; set; }
        public int CW2 { get; set; }
        public int CW2Actual { get; set; }
        public int CW3 { get; set; }
        public int CW3Actual { get; set; }
        public int CW4 { get; set; }
        public int CW4Actual { get; set; }
        public int CW5 { get; set; }
        public int CW5Actual { get; set; }
    }
}
