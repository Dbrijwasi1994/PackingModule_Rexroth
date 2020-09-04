using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeburringProcess_Peekay.Models
{
	class DeburringProcessEntity : INotifyPropertyChanged
	{
		internal bool IsRowchnaged;

		public DeburringProcessEntity()
		{
			IsRowchnaged = false;
		}

		public DeburringProcessEntity(string process, string fpNum, string utNum, DateTime receivedDateTime, DateTime startDateTime, string Oprator, DateTime endDateTime, DateTime scanOut, string remarks, int batchId,int startendID, string woNo, string heatNo )
		{
			Process = process;
			FPNumber = fpNum;
			UTNumber = utNum;
			ReceivedDateTime = receivedDateTime;
			StartDateTime = startDateTime;
			Operator = Oprator;
			EndDateTime = endDateTime;
			ScanOutTime = scanOut;
			Remarks = remarks;
			IsStartEnabled = true;
			IsEndEnabled = false;
			IsScanOutEnabled = false;
			StartVisibility = "Visible";
			EndVisibility = "Visible";
			ScanOutVisibility = "Visible";
			TxtStartVisibility = "Collapsed";
			TxtEndVisibility = "Collapsed";
			TxtScanOutVisibility = "Collapsed";
			BatchId = batchId;
			StartendID = startendID;
			WoNum = woNo;
			heatNum = heatNo;
		}

		private string _Process;
		public string Process
		{
			get { return _Process; }
			set
			{
				_Process = value;
				NotifyPropertyChanged("Process");
			}
		}

		private int _batchId;
		public int BatchId
		{
			get { return _batchId; }
			set
			{
				_batchId = value;
				NotifyPropertyChanged("BatchId");
			}
		}

		private int _startendID;
		public int StartendID
		{
			get { return _startendID; }
			set
			{
				_startendID = value;
				NotifyPropertyChanged("StartendID");
			}
		}


		private string _FPNumber;
		public string FPNumber
		{
			get { return _FPNumber; }
			set
			{
				_FPNumber = value;
				NotifyPropertyChanged("FPNumber");
			}
		}

		private string _UTNumber;
		public string UTNumber
		{
			get { return _UTNumber; }
			set
			{
				_UTNumber = value;
				NotifyPropertyChanged("UTNumber");
			}
		}

		private DateTime _ReceivedDateTime;
		public DateTime ReceivedDateTime
		{
			get { return _ReceivedDateTime; }
			set
			{
				_ReceivedDateTime = value;
				NotifyPropertyChanged("ReceivedDateTime");
			}
		}

		private DateTime _StartDateTime;
		public DateTime StartDateTime
		{
			get { return _StartDateTime; }
			set
			{
				_StartDateTime = value;
				NotifyPropertyChanged("StartDateTime");
			}
		}

		private string _Operator;
		public string Operator
		{
			get { return _Operator; }
			set
			{
				_Operator = value;
				NotifyPropertyChanged("Operator");
			}
		}

		private DateTime _EndDateTime;
		public DateTime EndDateTime
		{
			get { return _EndDateTime; }
			set
			{
				_EndDateTime = value;
				NotifyPropertyChanged("EndDateTime");
			}
		}

		private DateTime _ScanOutTime;
		public DateTime ScanOutTime
		{
			get { return _ScanOutTime; }
			set
			{
				_ScanOutTime = value;
				NotifyPropertyChanged("ScanOutTime");
			}
		}

		private string _Remarks;
		public string Remarks
		{
			get { return _Remarks; }
			set
			{
				_Remarks = value;
				NotifyPropertyChanged("Remarks");
			}
		}

		private bool _IsStartEnabled;
		public bool IsStartEnabled
		{
			get { return _IsStartEnabled; }
			set
			{
				_IsStartEnabled = value;
				NotifyPropertyChanged("IsStartEnabled");
			}
		}

		private bool _IsEndEnabled;
		public bool IsEndEnabled
		{
			get { return _IsEndEnabled; }
			set
			{
				_IsEndEnabled = value;
				NotifyPropertyChanged("IsEndEnabled");
			}
		}

		private bool _IsScanOutEnabled;
		public bool IsScanOutEnabled
		{
			get { return _IsScanOutEnabled; }
			set
			{
				_IsScanOutEnabled = value;
				NotifyPropertyChanged("IsScanOutEnabled");
			}
		}

		private string _StartVisibility;
		public string StartVisibility
		{
			get { return _StartVisibility; }
			set
			{
				_StartVisibility = value;
				NotifyPropertyChanged("StartVisibility");
			}
		}

		private string _EndVisibility;
		public string EndVisibility
		{
			get { return _EndVisibility; }
			set
			{
				_EndVisibility = value;
				NotifyPropertyChanged("EndVisibility");
			}
		}

		private string _ScanOutVisibility;
		public string ScanOutVisibility
		{
			get { return _ScanOutVisibility; }
			set
			{
				_ScanOutVisibility = value;
				NotifyPropertyChanged("ScanOutVisibility");
			}
		}

		private string _TxtStartVisibility;
		public string TxtStartVisibility
		{
			get { return _TxtStartVisibility; }
			set
			{
				_TxtStartVisibility = value;
				NotifyPropertyChanged("TxtStartVisibility");
			}
		}

		private string _TxtEndVisibility;
		public string TxtEndVisibility
		{
			get { return _TxtEndVisibility; }
			set
			{
				_TxtEndVisibility = value;
				NotifyPropertyChanged("TxtEndVisibility");
			}
		}

		private string _TxtScanOutVisibility;
		public string TxtScanOutVisibility
		{
			get { return _TxtScanOutVisibility; }
			set
			{
				_TxtScanOutVisibility = value;
				NotifyPropertyChanged("TxtScanOutVisibility");
			}
		}

		private string _WoNum;
		public string WoNum
		{
			get { return _WoNum; }
			set
			{
				_WoNum = value;
				NotifyPropertyChanged("WoNum");
			}
		}

		private string _heatNum;
		public string heatNum
		{
			get { return _heatNum; }
			set
			{
				_heatNum = value;
				NotifyPropertyChanged("heatNum");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				IsRowchnaged = true;
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	public class QRVals : INotifyPropertyChanged
	{

		private string type;
		public string Type
		{
			get { return type; }
			set
			{
				type = value;
				NotifyPropertyChanged("Type");
			}
		}
		private string mNo;
		public string MNO
		{
			get { return mNo; }
			set
			{
				mNo = value;
				NotifyPropertyChanged("MNO");
			}
		}
		private string utNo;
		public string UTNO
		{
			get { return utNo; }
			set
			{
				utNo = value;
				NotifyPropertyChanged("UTNO");
			}
		}
		private string fpNo;
		public string FPNO
		{
			get { return fpNo; }
			set
			{
				fpNo = value;
				NotifyPropertyChanged("FPNO");
			}
		}
		private string partNo;
		public string PartNo
		{
			get { return partNo; }
			set
			{
				partNo = value;
				NotifyPropertyChanged("PartNo");
			}
		}
		private string bcNo;
		public string BCNO
		{
			get { return bcNo; }
			set
			{
				bcNo = value;
				NotifyPropertyChanged("BCNO");
			}
		}
		private string heatNo;
		public string HeatNo
		{
			get { return heatNo; }
			set
			{
				heatNo = value;
				NotifyPropertyChanged("HeatNo");
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
		private string poNumber;
		public string PoNumber
		{
			get { return poNumber; }
			set
			{
				poNumber = value;
				NotifyPropertyChanged("PoNumber");
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
		private string quantity;
		public string Quantity
		{
			get { return quantity; }
			set
			{
				quantity = value;
				NotifyPropertyChanged("Quantity");
			}
		}
		private string weight;
		public string Weight
		{
			get { return weight; }
			set
			{
				weight = value;
				NotifyPropertyChanged("Weight");
			}
		}
		private string serialNumber;
		public string SerialNumber
		{
			get { return serialNumber; }
			set
			{
				serialNumber = value;
				NotifyPropertyChanged("SerialNumber");
			}
		}
		private string grade;
		public string Grade
		{
			get { return grade; }
			set
			{
				grade = value;
				NotifyPropertyChanged("Grade");
			}
		}
		private string fpDesc;
		public string FpDesc
		{
			get { return fpDesc; }
			set
			{
				fpDesc = value;
				NotifyPropertyChanged("FpDesc");
			}
		}
		private string drawingNum;
		public string DrawingNum
		{
			get { return drawingNum; }
			set
			{
				drawingNum = value;
				NotifyPropertyChanged("DrawingNum");
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	class InspectionTransitionEntity : INotifyPropertyChanged
	{
		internal bool IsRowchnaged = false;
		private int _Id;
		public int Id
		{
			get { return _Id; }
			set
			{
				_Id = value;
				NotifyPropertyChanged("Id");
			}
		}

		private string _DrawingSpec;
		public string DrawingSpec
		{
			get { return _DrawingSpec; }
			set
			{
				_DrawingSpec = value;
				NotifyPropertyChanged("DrawingSpec");
			}
		}

		private string _Specification;
		public string Specification
		{
			get { return _Specification; }
			set
			{
				_Specification = value;
				NotifyPropertyChanged("Specification");
			}
		}

		private string _SpecificationMean;
		public string SpecificationMean
		{
			get { return _SpecificationMean; }
			set
			{
				_SpecificationMean = value;
				NotifyPropertyChanged("SpecificationMean");
			}
		}

		private string _LSL;
		public string LSL
		{
			get { return _LSL; }
			set
			{
				_LSL = value;
				NotifyPropertyChanged("LSL");
			}
		}

		private string _USL;
		public string USL
		{
			get { return _USL; }
			set
			{
				_USL = value;
				NotifyPropertyChanged("USL");
			}
		}

		private string _MethodOfInspection;
		public string MethodOfInspection
		{
			get { return _MethodOfInspection; }
			set
			{
				_MethodOfInspection = value;
				NotifyPropertyChanged("MethodOfInspection");
			}
		}

		private string _Observation;
		public string Observation
		{
			get { return _Observation; }
			set
			{
				_Observation = value;
				NotifyPropertyChanged("Observation");
			}
		}

		private string _OperatorRemarks;
		public string OperatorRemarks
		{
			get { return _OperatorRemarks; }
			set
			{
				_OperatorRemarks = value;
				NotifyPropertyChanged("OperatorRemarks");
			}
		}

		private string _SupervisorRemarks;
		public string SupervisorRemarks
		{
			get { return _SupervisorRemarks; }
			set
			{
				_SupervisorRemarks = value;
				NotifyPropertyChanged("SupervisorRemarks");
			}
		}

		private string _BackgroundObservation;
		public string BackgroundObservation
		{
			get { return _BackgroundObservation; }
			set
			{
				_BackgroundObservation = value;
				NotifyPropertyChanged("BackgroundObservation");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				IsRowchnaged = true;
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	class GetInspectionData : INotifyPropertyChanged
	{
		private string MachineID;
		public string machineID
		{
			get { return MachineID; }
			set
			{
				MachineID = value;
				NotifyPropertyChanged("machineID");
			}
		}

		private string ComponentID;
		public string componentID
		{
			get { return ComponentID; }
			set
			{
				ComponentID = value;
				NotifyPropertyChanged("componentID");
			}
		}

		private string OperationNo;
		public string operationNo
		{
			get { return OperationNo; }
			set
			{
				OperationNo = value;
				NotifyPropertyChanged("operationNo");
			}
		}

		private string CharacteristicCode;
		public string characteristicCode
		{
			get { return CharacteristicCode; }
			set
			{
				CharacteristicCode = value;
				NotifyPropertyChanged("characteristicCode");
			}
		}

		private string CharacteristicID;
		public string characteristicID
		{
			get { return CharacteristicID; }
			set
			{
				CharacteristicID = value;
				NotifyPropertyChanged("characteristicID");
			}
		}

		private string SpecificationMean;
		public string specificationMean
		{
			get { return SpecificationMean; }
			set
			{
				SpecificationMean = value;
				NotifyPropertyChanged("specificationMean");
			}
		}

		private string LSL;
		public string _LSL
		{
			get { return LSL; }
			set
			{
				LSL = value;
				NotifyPropertyChanged("_LSL");
			}
		}

		private string USL;
		public string _USL
		{
			get { return USL; }
			set
			{
				USL = value;
				NotifyPropertyChanged("_USL");
			}
		}

		private string UOM;
		public string _UOM
		{
			get { return UOM; }
			set
			{
				UOM = value;
				NotifyPropertyChanged("_UOM");
			}
		}

		private string SampleSize;
		public string sampleSize
		{
			get { return SampleSize; }
			set
			{
				SampleSize = value;
				NotifyPropertyChanged("sampleSize");
			}
		}

		private string InProcessInterval;
		public string inProcessInterval
		{
			get { return InProcessInterval; }
			set
			{
				InProcessInterval = value;
				NotifyPropertyChanged("inProcessInterval");
			}
		}

		private string InstrumentType;
		public string instrumentType
		{
			get { return InstrumentType; }
			set
			{
				InstrumentType = value;
				NotifyPropertyChanged("instrumentType");
			}
		}

		private string InspectionDrawing;
		public string inspectionDrawing
		{
			get { return InspectionDrawing; }
			set
			{
				InspectionDrawing = value;
				NotifyPropertyChanged("inspectionDrawing");
			}
		}

		private string DataType;
		public string dataType
		{
			get { return DataType; }
			set
			{
				DataType = value;
				NotifyPropertyChanged("dataType");
			}
		}

		private string SetupApprovalInterval;
		public string setupApprovalInterval
		{
			get { return SetupApprovalInterval; }
			set
			{
				SetupApprovalInterval = value;
				NotifyPropertyChanged("setupApprovalInterval");
			}
		}

		private string Interval;
		public string interval
		{
			get { return Interval; }
			set
			{
				Interval = value;
				NotifyPropertyChanged("interval");
			}
		}

		private string Specification;
		public string specification
		{
			get { return Specification; }
			set
			{
				Specification = value;
				NotifyPropertyChanged("specification");
			}
		}

		private string MacroLocation;
		public string macroLocation
		{
			get { return MacroLocation; }
			set
			{
				MacroLocation = value;
				NotifyPropertyChanged("macroLocation");
			}
		}

		private string ID;
		public string _ID
		{
			get { return ID; }
			set
			{
				ID = value;
				NotifyPropertyChanged("_ID");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				//IsRowchnaged = true;
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	public class InspectionReportEntity
	{
		public InspectionParameters insparams { get; set; }
		public ObservableCollection<InspectionReportParameters> insReportParamsList { get; set; }
	}

	public class InspectionParameters
	{
		public string UTNumber { get; set; }
		public string FPNumber { get; set; }
		public string MPPNumber { get; set; }
		public string MPINumber { get; set; }
		public string MachineID { get; set; }
		public string FPDescription { get; set; }
		public string MaterialGrade { get; set; }
		public string DrawingNumber { get; set; }
		public string HeatNumber { get; set; }
		public string CustomerName { get; set; }
		public string ReportNo { get; set; }
		public string PartName { get; set; }
		public string PartNumber { get; set; }
		public string WOnoNDate { get; set; }
		public string InspDate { get; set; }
		public string PONum { get; set; }
		public string Quantity { get; set; }
		public string PODate { get; set; }
		public string NDEReq { get; set; }
		public string SupplierName { get; set; }
		public string HydoTestReqd { get; set; }
		public string WONum { get; set; }
	}

	public class InspectionReportParameters : INotifyPropertyChanged
	{
		internal bool IsRowchnaged = false;
		public string BLNo { get; set; }
		public string OperationNo { get; set; }
		public string DrawingSpec { get; set; }
		public string Specification { get; set; }
		public string SpecificationMean { get; set; }
		public string LSL { get; set; }
		public string USL { get; set; }
		public string MethodOfInspection { get; set; }

		public string _Observation { get; set; }
		public string Observation
		{
			get { return _Observation; }
			set
			{
				_Observation = value;
				NotifyPropertyChanged("Observation");
			}
		}

		public string _Operator { get; set; }
		public string Operator
		{
			get { return _Operator; }
			set
			{
				_Operator = value;
				NotifyPropertyChanged("Operator");
			}
		}

		public string _OperatorRemarks { get; set; }
		public string OperatorRemarks
		{
			get { return _OperatorRemarks; }
			set
			{
				_OperatorRemarks = value;
				NotifyPropertyChanged("OperatorRemarks");
			}
		}

		public string _Supervisor { get; set; }
		public string Supervisor
		{
			get { return _Supervisor; }
			set
			{
				_Supervisor = value;
				NotifyPropertyChanged("Supervisor");
			}
		}

		public string _SupervisorRemarks { get; set; }
		public string SupervisorRemarks
		{
			get { return _SupervisorRemarks; }
			set
			{
				_SupervisorRemarks = value;
				NotifyPropertyChanged("SupervisorRemarks");
			}
		}

		public string _BackgroundObservation { get; set; }
		public string BackgroundObservation
		{
			get { return _BackgroundObservation; }
			set
			{
				_BackgroundObservation = value;
				NotifyPropertyChanged("BackgroundObservation");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				IsRowchnaged = true;
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	class Downcode
	{
		public string DowncodeID { get; set; }
		public string InterfaceID { get; set; }
	}

	public class PathDetails
	{
		public string WORK_INST_PATH { get; set; }
		public string PROC_DOC_PATH { get; set; }
		public string WO_URL { get; set; }
	}

	public class DownSummary
	{
		public string DOWN_ID { get; set; }
		public DateTime? START_TIME { get; set; }
		public DateTime? END_TIME { get; set; }
		public string DOWN_TIME { get; set; }
		public string OPERATOR { get; set; }
	}
	public class ProductionDataEntity
	{
		public string Process { get; set; }
		public string FPNumber { get; set; }
		public string WONumber { get; set; }
		public string UTNumber { get; set; }
		public string HeatNumber { get; set; }
		public string BatchID { get; set; }
		public string ScanIn { get; set; }
		public string Start { get; set; }
		public string End { get; set; }
		public string ScanOut { get; set; }
		public string Operator { get; set; }
		public string Remarks { get; set; }
	}

	public class ComboboxEntity
	{
		public string UTNumber { get; set; }
		public string HeatNumber { get; set; }
		public string MPINumber { get; set; }
	}
	public class ComboboxInspectionEntity
	{
		public string UTNumber { get; set; }
		public string HeatNumber { get; set; }
		public string FPNumber { get; set; }
	}
}


