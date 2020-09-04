using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Models
{
    class ShiftDetailsEntity
    {
        public string ShiftID { get; set; }
        public string ShiftName { get; set; }
        public string FromDay { get; set; }
        public string FromTime { get; set; }
        public string ToDay { get; set; }
        public string ToTime { get; set; }
    }
}
