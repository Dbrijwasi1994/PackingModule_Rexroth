using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeburringProcess_Peekay.DB_Connection;

namespace PackingModule_Rexroth.Models
{
    class ShiftTargetMasterEntity : INotifyPropertyChanged
    {
        public ShiftTargetMasterEntity()
        {
            IsRowChanged = false;
            ScheduleDate = DateTime.Now;
            StationID = ConfigurationManager.AppSettings["OperatorStation"] != null ? ConfigurationManager.AppSettings["OperatorStation"].ToString().ToUpper() : "";
            Shift = MivinDataBaseAccess.GetCurrentShiftTime().Shift;
            NumOfPersons = 3;
            ShiftTarget = 900;
        }

        public bool IsRowChanged { get; private set; }
        public DateTime ScheduleDate { get; set; }
        public string StationID { get; set; }
        public string Shift { get; set; }

        private int _NumOfPersons;
        public int NumOfPersons
        {
            get { return _NumOfPersons; }
            set
            {
                _NumOfPersons = value;
                NotifyPropertyChanged("NumOfPersons");
            }
        }

        private int _ShiftTarget;
        public int ShiftTarget
        {
            get { return _ShiftTarget; }
            set
            {
                _ShiftTarget = value;
                NotifyPropertyChanged("ShiftTarget");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                IsRowChanged = true;
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
