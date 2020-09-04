using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Models
{
    class RunningShiftSchedulesEntity : INotifyPropertyChanged
    {
        public RunningShiftSchedulesEntity()
        {
            QuantityPerBox = 0;
            TotalScannedQuantity = 0;
            IsSetButtonEnabled = false;
        }

        public RunningShiftSchedulesEntity(string stationid, string schDate, string woNumber, string customer, string pumpmodel, string custModel, int packingTrgt, int qtyPerBox, int totalScannedQty, int packedboxCount, string pckType, string woStatus, bool isSetEnabled)
        {
            this.ScheduleDate = schDate;
            this.StationID = stationid;
            this.WorkOrderNumber = woNumber;
            this.Customer = customer;
            this.PumpModel = pumpmodel;
            this.CustomerModel = custModel;
            this.PackingTarget = packingTrgt;
            this.QuantityPerBox = qtyPerBox;
            this.TotalScannedQuantity = totalScannedQty;
            this.PackedBoxCount = packedboxCount;
            this.PackagingType = pckType;
            this.WorkOrderStatus = woStatus;
            this.IsSetButtonEnabled = isSetEnabled;
        }
        public bool IsRowChanged { get; private set; }

        private string scheduleDate;
        public string ScheduleDate
        {
            get { return scheduleDate; }
            set
            {
                scheduleDate = value;
                NotifyPropertyChanged("ScheduleDate");
            }
        }

        private string stationID;
        public string StationID
        {
            get { return stationID; }
            set
            {
                stationID = value;
                NotifyPropertyChanged("StationID");
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

        private string customer;
        public string Customer
        {
            get { return customer; }
            set
            {
                customer = value;
                NotifyPropertyChanged("Customer");
            }
        }

        private string pumpModel;
        public string PumpModel
        {
            get { return pumpModel; }
            set
            {
                pumpModel = value;
                NotifyPropertyChanged("PumpModel");
            }
        }

        private string customerModel;
        public string CustomerModel
        {
            get { return customerModel; }
            set
            {
                customerModel = value;
                NotifyPropertyChanged("CustomerModel");
            }
        }

        private int packingTarget;
        public int PackingTarget
        {
            get { return packingTarget; }
            set
            {
                packingTarget = value;
                NotifyPropertyChanged("PackingTarget");
            }
        }

        private int quantityPerBox;
        public int QuantityPerBox
        {
            get { return quantityPerBox; }
            set
            {
                quantityPerBox = value;
                NotifyPropertyChanged("QuantityPerBox");
            }
        }

        private int totalScannedQuantity;
        public int TotalScannedQuantity
        {
            get { return totalScannedQuantity; }
            set
            {
                totalScannedQuantity = value;
                NotifyPropertyChanged("TotalScannedQuantity");
            }
        }

        private int packedBoxCount;
        public int PackedBoxCount
        {
            get { return packedBoxCount; }
            set
            {
                packedBoxCount = value;
                NotifyPropertyChanged("PackedBoxCount");
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

        private string workOrderStatus;
        public string WorkOrderStatus
        {
            get { return workOrderStatus; }
            set
            {
                workOrderStatus = value;
                NotifyPropertyChanged("WorkOrderStatus");
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

        private bool isSetButtonEnabled;
        public bool IsSetButtonEnabled
        {
            get { return isSetButtonEnabled; }
            set
            {
                isSetButtonEnabled = value;
                NotifyPropertyChanged("IsSetButtonEnabled");
            }
        }

        private string pumpInfo;
        public string PumpInfo
        {
            get { return pumpInfo; }
            set
            {
                pumpInfo = value;
                NotifyPropertyChanged("PumpInfo");
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

    public class RunningModelStatusEntity : INotifyPropertyChanged
    {
        public RunningModelStatusEntity()
        {
            QuantityPerBox = 0;
            ScannedQuantity = 0;
            IsHoldEnabled = false;
        }

        public RunningModelStatusEntity(string stationid, string woNumber, string runningpumpmodel, string custmodel, int qtyPerBox, int scannedQty, string pckType, string opr, string lastScannedSlNum, string workOrderStatus, bool isHoldEnabled)
        {
            this.StationID = stationid;
            this.WorkOrderNumber = woNumber;
            this.RunningPumpModel = runningpumpmodel;
            this.CustomerModel = custmodel;
            this.QuantityPerBox = qtyPerBox;
            this.ScannedQuantity = scannedQty;
            this.PackagingType = pckType;
            this.Operator = opr;
            this.LastScannedSerialNum = lastScannedSlNum;
            this.WorkOrderStatus = workOrderStatus;
            this.IsHoldEnabled = isHoldEnabled;
        }

        public bool IsRowChanged { get; private set; }
        private string scheduleDate;
        public string ScheduleDate
        {
            get { return scheduleDate; }
            set
            {
                scheduleDate = value;
                NotifyPropertyChanged("ScheduleDate");
            }
        }

        private string stationID;
        public string StationID
        {
            get { return stationID; }
            set
            {
                stationID = value;
                NotifyPropertyChanged("StationID");
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

        private string runningPumpModel;
        public string RunningPumpModel
        {
            get { return runningPumpModel; }
            set
            {
                runningPumpModel = value;
                NotifyPropertyChanged("RunningPumpModel");
            }
        }

        private string customerModel;
        public string CustomerModel
        {
            get { return customerModel; }
            set
            {
                customerModel = value;
                NotifyPropertyChanged("CustomerModel");
            }
        }

        private int quantityPerBox;
        public int QuantityPerBox
        {
            get { return quantityPerBox; }
            set
            {
                quantityPerBox = value;
                NotifyPropertyChanged("QuantityPerBox");
            }
        }

        private int scannedQuantity;
        public int ScannedQuantity
        {
            get { return scannedQuantity; }
            set
            {
                scannedQuantity = value;
                NotifyPropertyChanged("ScannedQuantity");
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

        private string _operator;
        public string Operator
        {
            get { return _operator; }
            set
            {
                _operator = value;
                NotifyPropertyChanged("Operator");
            }
        }

        private string lastScannedSerialNum;
        public string LastScannedSerialNum
        {
            get { return lastScannedSerialNum; }
            set
            {
                lastScannedSerialNum = value;
                NotifyPropertyChanged("LastScannedSerialNum");
            }
        }

        private string workOrderStatus;
        public string WorkOrderStatus
        {
            get { return workOrderStatus; }
            set
            {
                workOrderStatus = value;
                NotifyPropertyChanged("WorkOrderStatus");
            }
        }

        private int totalScannedQuantity;
        public int TotalScannedQuantity
        {
            get { return totalScannedQuantity; }
            set
            {
                totalScannedQuantity = value;
                NotifyPropertyChanged("TotalScannedQuantity");
            }
        }

        private bool isHoldEnabled;
        public bool IsHoldEnabled
        {
            get { return isHoldEnabled; }
            set
            {
                isHoldEnabled = value;
                NotifyPropertyChanged("IsHoldEnabled");
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

    class RunningPumpModelEntity : INotifyPropertyChanged
    {
        public RunningPumpModelEntity()
        {
            WorkOrderNumber = "";
            StationID = "";
            CustomerModel = "";
            DailyTarget = 0;
            DailyActual = 0;
            MonthlyTarget = 0;
            MonthlyActual = 0;
            PackedBoxCount = 0;
            DailyPending = 0;
            MonthlyPending = 0;
        }

        public bool IsRowChanged { get; private set; }

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

        private string stationID;
        public string StationID
        {
            get { return stationID; }
            set
            {
                stationID = value;
                NotifyPropertyChanged("StationID");
            }
        }

        private string customerModel;
        public string CustomerModel
        {
            get { return customerModel; }
            set
            {
                customerModel = value;
                NotifyPropertyChanged("CustomerModel");
            }
        }

        private string scheduleDate;
        public string ScheduleDate
        {
            get { return scheduleDate; }
            set
            {
                scheduleDate = value;
                NotifyPropertyChanged("ScheduleDate");
            }
        }

        private string runningPumpModel;
        public string RunningPumpModel
        {
            get { return runningPumpModel; }
            set
            {
                runningPumpModel = value;
                NotifyPropertyChanged("RunningPumpModel");
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

        private int dailyTarget;
        public int DailyTarget
        {
            get { return dailyTarget; }
            set
            {
                dailyTarget = value;
                NotifyPropertyChanged("DailyTarget");
            }
        }

        private int dailyActual;
        public int DailyActual
        {
            get { return dailyActual; }
            set
            {
                dailyActual = value;
                NotifyPropertyChanged("DailyActual");
            }
        }

        private int monthlyTarget;
        public int MonthlyTarget
        {
            get { return monthlyTarget; }
            set
            {
                monthlyTarget = value;
                NotifyPropertyChanged("MonthlyTarget");
            }
        }

        private int monthlyActual;
        public int MonthlyActual
        {
            get { return monthlyActual; }
            set
            {
                monthlyActual = value;
                NotifyPropertyChanged("MonthlyActual");
            }
        }

        private int packedBoxCount;
        public int PackedBoxCount
        {
            get { return packedBoxCount; }
            set
            {
                packedBoxCount = value;
                NotifyPropertyChanged("PackedBoxCount");
            }
        }

        private int dailyPending;
        public int DailyPending
        {
            get { return dailyPending; }
            set
            {
                dailyPending = value;
                NotifyPropertyChanged("DailyPending");
            }
        }

        private int monthlyPending;
        public int MonthlyPending
        {
            get { return monthlyPending; }
            set
            {
                monthlyPending = value;
                NotifyPropertyChanged("MonthlyPending");
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

    class ShiftWiseEnergyInfo
    {
        public string StationID { get; set; }
        public int ShiftTarget { get; set; }
        public int ShiftActual { get; set; }
        public double ShiftEfficiency { get; set; }
        public string ShiftwiseTarget { get; set; }
        public string ShiftwiseActual { get; set; }
    }
}
