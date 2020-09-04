using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Models
{
    public class PumpQRData
    {
        public string PumpModel { get; set; }
        public string PumpSerialNum { get; set; }
    }

    public class CurrentShiftTime
    {
        public DateTime ShiftStartDate { get; set; }
        public string Shift { get; set; }
    }
}
