using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Models
{
    class PumpInfoEntity : INotifyPropertyChanged
    {
        internal bool IsRowChanged;

        public PumpInfoEntity()
        {
            PerBoxPumpQty = 2;
            CustomerModel = "";
            CustomerName = "";
            SalesUnit = "";
            PackagingType = "";
            PackingBoxNumber = "";
            PumpType = "";
            BoxDestination = "";
        }

        private string _PumpModel;
        public string PumpModel
        {
            get { return _PumpModel; }
            set
            {
                _PumpModel = value;
                NotifyPropertyChanged("PumpModel");
            }
        }

        private string _CustomerModel;
        public string CustomerModel
        {
            get { return _CustomerModel; }
            set
            {
                _CustomerModel = value;
                NotifyPropertyChanged("CustomerModel");
            }
        }

        private string _CustomerName;
        public string CustomerName
        {
            get { return _CustomerName; }
            set
            {
                _CustomerName = value;
                NotifyPropertyChanged("CustomerName");
            }
        }

        private string _SalesUnit;
        public string SalesUnit
        {
            get { return _SalesUnit; }
            set
            {
                _SalesUnit = value;
                NotifyPropertyChanged("SalesUnit");
            }
        }

        private string _PackagingType;
        public string PackagingType
        {
            get { return _PackagingType; }
            set
            {
                _PackagingType = value;
                NotifyPropertyChanged("PackagingType");
            }
        }

        private string _PackingBoxNumber;
        public string PackingBoxNumber
        {
            get { return _PackingBoxNumber; }
            set
            {
                _PackingBoxNumber = value;
                NotifyPropertyChanged("PackingBoxNumber");
            }
        }

        private int _PerBoxPumpQty;
        public int PerBoxPumpQty
        {
            get { return _PerBoxPumpQty; }
            set
            {
                _PerBoxPumpQty = value;
                NotifyPropertyChanged("PerBoxPumpQty");
            }
        }

        private string _PumpType;
        public string PumpType
        {
            get { return _PumpType; }
            set
            {
                _PumpType = value;
                NotifyPropertyChanged("PumpType");
            }
        }

        private string _BoxDestination;
        public string BoxDestination
        {
            get { return _BoxDestination; }
            set
            {
                _BoxDestination = value;
                NotifyPropertyChanged("BoxDestination");
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
