using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Models
{
    class ScheduleMasterEntity : INotifyPropertyChanged
    {
        public ScheduleMasterEntity()
        {
            DispatchQty = 0;
        }

        public ScheduleMasterEntity(int slNo, string woNumber, string pumpPartNo, string custName, string packaging_Type, int dispatchQuantity, int priority, DateTime date, string customermodel)
        {
            this.SerialNum = slNo;
            this.WorkOrderNumber = woNumber;
            this.PumpPartNumber = pumpPartNo;
            this.CustomerName = custName;
            this.PackagingType = packaging_Type;
            this.DispatchQty = dispatchQuantity;
            this.Priority = priority;
            this.Date = date;
            this.CustomerModel = customermodel;
        }

        internal bool IsRowChanged = false;
        private int serialNum;
        public int SerialNum
        {
            get { return serialNum; }
            set
            {
                serialNum = value;
                NotifyPropertyChanged("SerialNum");
            }
        }

        private string workOrderNumber;
        public string WorkOrderNumber
        {
            get { return workOrderNumber; }
            set
            {
                workOrderNumber = value;
                NotifyPropertyChanged("WorkOrderNumber");
            }
        }

        private string pumpPartNumber;
        public string PumpPartNumber
        {
            get { return pumpPartNumber; }
            set
            {
                pumpPartNumber = value;
                NotifyPropertyChanged("PumpPartNumber");
            }
        }

        private string customerName;
        public string CustomerName
        {
            get { return customerName; }
            set
            {
                customerName = value;
                NotifyPropertyChanged("CustomerName");
            }
        }

        private string packagingType;
        public string PackagingType
        {
            get { return packagingType; }
            set
            {
                packagingType = value;
                NotifyPropertyChanged("PackagingType");
            }
        }

        private int dispatchQty;
        public int DispatchQty
        {
            get { return dispatchQty; }
            set
            {
                dispatchQty = value;
                NotifyPropertyChanged("DispatchQty");
            }
        }

        private string customermodel;
        public string CustomerModel
        {
            get { return customermodel; }
            set
            {
                customermodel = value;
                NotifyPropertyChanged("CustomerModel");
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                NotifyPropertyChanged("Date");
            }
        }

        private int priority;
        public int Priority
        {
            get { return priority; }
            set
            {
                priority = value;
                NotifyPropertyChanged("Priority");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                IsRowChanged = true;
            }
        }
    }
}
