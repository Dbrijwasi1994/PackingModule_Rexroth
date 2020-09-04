using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeburringProcess_Peekay.Models;
using System.Configuration;
using System.Data.SqlClient;
using DeburringProcess_Peekay.Helpers;
using System.Data;
using DeburringProcess_Peekay.Dialogs;
using System.IO;
using PackingModule_Rexroth.Models;

namespace DeburringProcess_Peekay.DB_Connection
{
    class DatabaseAccess
    {
        #region process
        public static void dataentry(string Process, string UTNumber, string FPNumber, string Activity, DateTime date)
        {
            SqlConnection conn = ConnectionManager.GetConnection();
            //string Query = @"if not exists(select * from [PostMachiningProcessInfo_Peekay] where  [UTNumber]=@UTNumber and [Process]=@Process)
            //	INSERT INTO [dbo].[PostMachiningProcessInfo_Peekay]
            //             ([Process],[FPNumber], [UTNumber],[Activity],[ActivityTS])
            //             VALUES
            //             (@Process, @FPNumber, @UTNumber, @Activity, @ActivityTS)";

            var batchId = GetMaxBatchId(UTNumber, FPNumber);

            string Query = @"if not exists(select * from [PostMachiningProcessInfo_Peekay] where not exists(select * from [PostMachiningProcessInfo_Peekay] where [UTNumber] = @UTNumber and [Process]= @Process and Activity = 'Scanout' and activityts >  (select max(ActivityTS) from PostMachiningProcessInfo_Peekay where activity = 'Scanin' and [UTNumber]= @UTNumber and [Process]= @Process)) And [UTNumber] = @UTNumber and [Process] = @Process ) INSERT INTO [dbo].[PostMachiningProcessInfo_Peekay]
             ([Process],[FPNumber], [UTNumber],[Activity],[ActivityTS],[BatchId])
				VALUES
				(@Process, @FPNumber, @UTNumber, @Activity, @ActivityTS,@BatchId)";

            SqlCommand cmd = new SqlCommand(Query, conn);
            try
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Process", Process);
                cmd.Parameters.AddWithValue("@FPNumber", FPNumber);
                cmd.Parameters.AddWithValue("@UTNumber", UTNumber);
                cmd.Parameters.AddWithValue("@Activity", Activity);
                cmd.Parameters.AddWithValue("@ActivityTS", date);
                cmd.Parameters.AddWithValue("@BatchId", batchId);
                //cmd.Parameters.AddWithValue("@StNdBatchid", 1);


                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        internal static bool mainlogin(string Name, string upassword, out bool isAdmin, out string empRole, out bool IsConnected)
        {
            bool ok = false;
            isAdmin = false;
            empRole = string.Empty;
            IsConnected = false;
            SqlConnection conn = null;
            try
            {
                conn = ConnectionManager.GetConnection();
                if (conn != null && !(conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken))
                {
                    IsConnected = true;
                    string Query = "Select * from employeeinformation where employeeid=@Name and upassword=@upassword";
                    SqlDataReader rdr = null;
                    SqlCommand cmd = new SqlCommand(Query, conn);
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@upassword", upassword);
                    cmd.ExecuteNonQuery();
                    rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        ok = true;
                        if (rdr.Read())
                        {
                            if (!Convert.IsDBNull(rdr["isadmin"]) && !string.IsNullOrEmpty(rdr["isadmin"].ToString()))
                                isAdmin = rdr["isadmin"].ToString() == "1" ? true : false;
                            if (!Convert.IsDBNull(rdr["EmployeeRole"]) && !string.IsNullOrEmpty(rdr["EmployeeRole"].ToString()))
                                empRole = rdr["EmployeeRole"].ToString();
                        }
                    }
                    else
                        ok = false;

                }
                else
                    IsConnected = false;
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return ok;
        }

        public static DateTime startdatetime()
        {
            DateTime StartDate = DateTime.Now;
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlDataReader rdr = null;
            string query = "select [dbo].[f_GetLogicalDayStart] ('" + DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss") + "')";
            SqlCommand cmd = new SqlCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        StartDate = Convert.ToDateTime(rdr[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
                if (rdr != null) rdr.Close();
            }
            return StartDate;
        }

        public static DateTime enddatetime()
        {
            DateTime EndDate = DateTime.Now;
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlDataReader rdr = null;
            string query = "select [dbo].[f_GetLogicalDayEnd] ('" + DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss") + "')";
            SqlCommand cmd = new SqlCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        EndDate = Convert.ToDateTime(rdr[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
                if (rdr != null) rdr.Close();
            }
            return EndDate;
        }

        //public static List<DeburringProcessEntity> recieveolddata(string Default, string process)
        //{
        //	SqlConnection conn = ConnectionManager.GetConnection();
        //	SqlDataReader rdr = null;
        //	List<DeburringProcessEntity> dbentry_info = new List<DeburringProcessEntity>();
        //	DeburringProcessEntity dbentry_data = null;
        //	try
        //	{
        //		SqlCommand cmd = new SqlCommand("S_GetPostMachiningProcessInfo_Peekay", conn);
        //		cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //		cmd.Parameters.AddWithValue("@ReportType", Default);
        //		cmd.Parameters.AddWithValue("@process", process);
        //		rdr = cmd.ExecuteReader();
        //		if (rdr.HasRows)
        //		{
        //			while (rdr.Read())
        //			{
        //				dbentry_data = new DeburringProcessEntity();
        //				dbentry_data.Process = rdr["process"].ToString();
        //				dbentry_data.FPNumber = rdr["FPNUMBER"].ToString();
        //				dbentry_data.UTNumber = rdr["UTNumber"].ToString();
        //				dbentry_data.Operator = rdr["Operator"].ToString();
        //				if (rdr[Scanin] != DBNull.Value)
        //					dbentry_data.ReceivedDateTime = Convert.ToDateTime(rdr[Scanin]);
        //				if (rdr[Start] != DBNull.Value)
        //				{
        //					dbentry_data.StartVisibility = "Collapsed";
        //					dbentry_data.TxtStartVisibility = "Visible";
        //					dbentry_data.StartDateTime = Convert.ToDateTime(rdr[Start]);
        //					dbentry_data.IsStartEnabled = false;
        //					dbentry_data.IsEndEnabled = true;
        //					dbentry_data.EndVisibility = "Visible";
        //					dbentry_data.IsScanOutEnabled = false;
        //					dbentry_data.ScanOutVisibility = "Visible";
        //					dbentry_data.TxtScanOutVisibility = "Collapsed";
        //				}
        //				else
        //				{
        //					dbentry_data.StartVisibility = "Visible";
        //					dbentry_data.TxtStartVisibility = "Collapsed";
        //					dbentry_data.IsStartEnabled = true;
        //				}	
        //				dbentry_data.Operator = rdr["Operator"].ToString();
        //				if (rdr["End"] != DBNull.Value)
        //				{
        //					dbentry_data.EndVisibility = "Collapsed";
        //					dbentry_data.TxtEndVisibility = "Visible";
        //					dbentry_data.EndDateTime = Convert.ToDateTime(rdr["End"]);
        //					dbentry_data.IsEndEnabled = false;
        //					dbentry_data.IsStartEnabled = true;
        //					dbentry_data.TxtStartVisibility = "Collapsed";
        //					dbentry_data.StartVisibility = "Visible";
        //					dbentry_data.IsScanOutEnabled = true;
        //					dbentry_data.ScanOutVisibility = "Visible";
        //					dbentry_data.TxtScanOutVisibility = "Collapsed";

        //				}
        //				else
        //				{
        //					dbentry_data.EndVisibility = "Visible";
        //					dbentry_data.TxtEndVisibility = "Collapsed";
        //					dbentry_data.IsEndEnabled = true;
        //				}

        //				if (rdr["Scanout"] != DBNull.Value)
        //				{
        //					dbentry_data.ScanOutVisibility = "Collapsed";
        //					dbentry_data.TxtScanOutVisibility = "Visible";
        //					dbentry_data.ScanOutTime = Convert.ToDateTime(rdr["Scanout"]);
        //					dbentry_data.IsStartEnabled = false;
        //					dbentry_data.IsScanOutEnabled = false;
        //					dbentry_data.IsEndEnabled = false;
        //					dbentry_data.EndVisibility = "Collapsed";
        //					dbentry_data.TxtEndVisibility = "Visible";
        //					dbentry_data.StartVisibility = "Collapsed";
        //					dbentry_data.TxtStartVisibility = "Visible";
        //				}
        //				else
        //				{
        //					dbentry_data.ScanOutVisibility = "Visible";
        //					dbentry_data.TxtScanOutVisibility = "Collapsed";
        //					dbentry_data.IsScanOutEnabled = true;
        //				}
        //				dbentry_data.Remarks = rdr["Remarks"].ToString();
        //				dbentry_info.Add(dbentry_data);
        //			}
        //		}
        //	}
        //	catch (Exception ex)
        //	{
        //		Logger.WriteDebugLog(ex.Message);
        //	}
        //	finally
        //	{
        //		if (conn != null) conn.Close();
        //		if (rdr != null) rdr.Close();
        //	}
        //	return dbentry_info;
        //}

        public static List<DeburringProcessEntity> recievedata(string Default, string process)
        {
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlDataReader rdr = null;
            List<DeburringProcessEntity> dbentry_info = new List<DeburringProcessEntity>();
            DeburringProcessEntity dbentry_data = null;
            try
            {
                SqlCommand cmd = new SqlCommand("S_GetPostMachiningProcessInfo_Peekay", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ReportType", Default);
                cmd.Parameters.AddWithValue("@process", process);
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        dbentry_data = new DeburringProcessEntity();
                        dbentry_data.Process = rdr["process"].ToString();
                        dbentry_data.FPNumber = rdr["FPNUMBER"].ToString();
                        dbentry_data.UTNumber = rdr["UTNumber"].ToString();
                        if (rdr["Scanin"] != DBNull.Value)
                            dbentry_data.ReceivedDateTime = Convert.ToDateTime(rdr["Scanin"]);
                        if (rdr["Start"] != DBNull.Value)
                        {
                            dbentry_data.StartVisibility = "Collapsed";
                            dbentry_data.TxtStartVisibility = "Visible";
                            dbentry_data.StartDateTime = Convert.ToDateTime(rdr["Start"]);
                            dbentry_data.IsStartEnabled = false;
                            dbentry_data.IsEndEnabled = true;
                            dbentry_data.EndVisibility = "Visible";
                            dbentry_data.IsScanOutEnabled = false;
                            dbentry_data.ScanOutVisibility = "Visible";
                            dbentry_data.TxtScanOutVisibility = "Collapsed";
                        }
                        else
                        {
                            dbentry_data.StartVisibility = "Visible";
                            dbentry_data.TxtStartVisibility = "Collapsed";
                            dbentry_data.IsStartEnabled = true;
                        }
                        dbentry_data.Operator = rdr["Operator"].ToString();
                        if (rdr["End"] != DBNull.Value)
                        {
                            dbentry_data.EndVisibility = "Collapsed";
                            dbentry_data.TxtEndVisibility = "Visible";
                            dbentry_data.EndDateTime = Convert.ToDateTime(rdr["End"]);
                            dbentry_data.IsEndEnabled = false;
                            dbentry_data.IsStartEnabled = true;
                            dbentry_data.TxtStartVisibility = "Collapsed";
                            dbentry_data.StartVisibility = "Visible";
                            dbentry_data.IsScanOutEnabled = true;
                            dbentry_data.ScanOutVisibility = "Visible";
                            dbentry_data.TxtScanOutVisibility = "Collapsed";
                        }
                        else
                        {
                            dbentry_data.EndVisibility = "Visible";
                            dbentry_data.TxtEndVisibility = "Collapsed";
                            dbentry_data.IsEndEnabled = true;
                        }

                        if (rdr["Scanout"] != DBNull.Value)
                        {
                            dbentry_data.ScanOutVisibility = "Collapsed";
                            dbentry_data.TxtScanOutVisibility = "Visible";
                            dbentry_data.ScanOutTime = Convert.ToDateTime(rdr["Scanout"]);
                            dbentry_data.IsStartEnabled = false;
                            dbentry_data.IsScanOutEnabled = false;
                            dbentry_data.IsEndEnabled = false;
                            dbentry_data.EndVisibility = "Collapsed";
                            dbentry_data.TxtEndVisibility = "Visible";
                            dbentry_data.StartVisibility = "Collapsed";
                            dbentry_data.TxtStartVisibility = "Visible";
                        }
                        else
                        {
                            dbentry_data.ScanOutVisibility = "Visible";
                            dbentry_data.TxtScanOutVisibility = "Collapsed";
                            if (dbentry_data.IsEndEnabled == true || dbentry_data.IsStartEnabled == false)
                            {
                                dbentry_data.IsScanOutEnabled = false;
                            }
                            else
                            {
                                dbentry_data.IsScanOutEnabled = true;
                            }
                        }
                        if (dbentry_data.IsScanOutEnabled == false && dbentry_data.IsStartEnabled == true)
                        {
                            dbentry_data.IsEndEnabled = false;
                        }
                        dbentry_data.Remarks = rdr["Remarks"].ToString();
                        if (!Convert.IsDBNull(rdr["BatchId"]) && !string.IsNullOrEmpty(rdr["BatchId"].ToString()))
                            dbentry_data.BatchId = Convert.ToInt32(rdr["BatchId"].ToString());
                        if (!Convert.IsDBNull(rdr["StNdBatchid"]) && !string.IsNullOrEmpty(rdr["StNdBatchid"].ToString()))
                            dbentry_data.StartendID = Convert.ToInt32(rdr["StNdBatchid"].ToString());
                        else
                            dbentry_data.StartendID = 0;
                        dbentry_data.WoNum = rdr["WorkOrderNumber"].ToString();
                        dbentry_data.heatNum = rdr["HeatNumber"].ToString();
                        dbentry_info.Add(dbentry_data);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
                if (rdr != null) rdr.Close();
            }
            return dbentry_info;
        }

        internal static DataTable GetMasterDataExistence(string componentID, string operationNo)
        {
            SqlDataReader sdr = null;
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            List<string> list = new List<string>();
            try
            {
                cmd = new SqlCommand("select * from SPC_Characteristic where ComponentID = @ComponentID and OperationNo = @OperationNo", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ComponentID", componentID);
                cmd.Parameters.AddWithValue("@OperationNo", operationNo);
                cmd.CommandTimeout = 120;
                sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    dt.Load(sdr);
                    dt.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error in retriving Machine - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (conn != null) conn.Close();
            }
            return dt;
        }

        public static void StartEndScanout(string Process, string FPNumber, string UTNumber, string Operator, string Activity, DateTime ActivityTS, string Remark, int batchId, int startendid, out bool isUpdated)
        {
            isUpdated = false;
            //startendid++;
            SqlConnection conn = ConnectionManager.GetConnection();
            string query = @"INSERT INTO [dbo].[PostMachiningProcessInfo_Peekay] ([Process],[FPNumber],[UTNumber],[Activity],[ActivityTS],[Remark],[Operator],[BatchId],[StNdBatchid])
				VALUES
				(@Process,@FPNumber, @UTNumber, @Activity,@ActivityTS,@Remark,@Operator,@BatchId,@StNdBatchid) ";
            SqlCommand cmd = new SqlCommand(query, conn);
            try
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Process", Process);
                cmd.Parameters.AddWithValue("@FPNumber", FPNumber);
                cmd.Parameters.AddWithValue("@UTNumber", UTNumber);
                cmd.Parameters.AddWithValue("@Operator", Operator);
                cmd.Parameters.AddWithValue("@Activity", Activity);
                cmd.Parameters.AddWithValue("@ActivityTS", ActivityTS.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@BatchId", batchId);
                cmd.Parameters.AddWithValue("@Remark", Remark);
                cmd.Parameters.AddWithValue("@StNdBatchid", startendid);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    isUpdated = true;
                else
                    isUpdated = false;
                if (Activity.Equals("ScanOut", StringComparison.OrdinalIgnoreCase) && Process.Equals(ConfigurationManager.AppSettings["packingname"].ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    query = @"INSERT INTO [dbo].[PostMachiningProcessInfo_Peekay] ([Process],[FPNumber],[UTNumber],[Activity],[ActivityTS],[Remark],[Operator],[BatchId],[StNdBatchid])
					VALUES (@Process,@FPNumber, @UTNumber, @Activity,@ActivityTS,@Remark,@Operator,@BatchId,@StNdBatchid) ";
                    cmd = new SqlCommand(query, conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Process", ConfigurationManager.AppSettings["savedispatchprocess"].ToString());
                    cmd.Parameters.AddWithValue("@FPNumber", FPNumber);
                    cmd.Parameters.AddWithValue("@UTNumber", UTNumber);
                    cmd.Parameters.AddWithValue("@Operator", Operator);
                    cmd.Parameters.AddWithValue("@Activity", "Scanin");
                    cmd.Parameters.AddWithValue("@ActivityTS", ActivityTS.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@BatchId", batchId);
                    cmd.Parameters.AddWithValue("@Remark", Remark);
                    cmd.Parameters.AddWithValue("@StNdBatchid", 1);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
                Logger.WriteDebugLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        public static QRVals search(string UTNumber)
        {
            SqlConnection conn = ConnectionManager.GetConnection();
            QRVals dbentry_data = new QRVals();
            string query = "Select * from [dbo].[MaterialInwardingProcessPeekay_Main] Where UTNumber=@UTNumber";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader rdr = null;
            try
            {
                cmd.Parameters.AddWithValue("@UTNumber", UTNumber.Trim());
                cmd.ExecuteNonQuery();
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        dbentry_data.UTNO = rdr["UTNumber"].ToString();
                        dbentry_data.FPNO = rdr["FPNUMBER"].ToString();
                        dbentry_data.MNO = rdr["MPINumber"].ToString();
                        dbentry_data.FpDesc = rdr["FPDescription"].ToString();
                        dbentry_data.HeatNo = rdr["HeatNumber"].ToString();
                        dbentry_data.PartNo = rdr["PartNumber"].ToString();
                        dbentry_data.BCNO = rdr["BarcodeNumber"].ToString();
                        dbentry_data.CustomerName = rdr["CustomerName"].ToString();
                        dbentry_data.DrawingNum = rdr["DrawingNumber"].ToString();

                        dbentry_data.WorkOrderNumber = rdr["WorkOrderNumber"].ToString();
                        dbentry_data.PoNumber = rdr["PONumber"].ToString();
                        dbentry_data.Grade = rdr["MaterialGrade"].ToString();

                        dbentry_data.Quantity = rdr["Quantity"].ToString();
                        dbentry_data.SerialNumber = rdr["SerialNumber"].ToString();
                        dbentry_data.Weight = rdr["Weight"].ToString();
                        //data_info.Add(dbentry_data);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
                if (rdr != null) rdr.Close();
            }
            return dbentry_data;
        }

        public static bool login(string Name, string upassword)
        {
            bool ok = false;
            SqlConnection conn = ConnectionManager.GetConnection();
            string Query = "Select * from employeeinformation where employeeid=@Name and upassword=@upassword";
            SqlDataReader rdr = null;
            SqlCommand cmd = new SqlCommand(Query, conn);
            try
            {
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@upassword", upassword);
                cmd.ExecuteNonQuery();
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    ok = true;
                }
                else
                    ok = false;
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return ok;
        }

        internal static bool IsAlreadyScannedOut(string UTNumber, string Activity)
        {
            string Pro = ConfigurationManager.AppSettings["Process"].ToString();
            bool ok = false;
            SqlConnection conn = ConnectionManager.GetConnection();
            //string Query = "Select * from PostMachiningProcessInfo_Peekay Where [UTNumber]=@UTNumber And [Process]=@Process And [Activity]=@Activity";
            string Query = @"select * from [PostMachiningProcessInfo_Peekay] where not exists 
(select * from [PostMachiningProcessInfo_Peekay] where [UTNumber] = @UTNumber and [Process] = @Process and  Activity = 'Scanout' and activityts >  (select max(ActivityTS) from PostMachiningProcessInfo_Peekay where activity = 'Scanin' and [UTNumber]= @UTNumber and [Process]= @Process) And [UTNumber] = @UTNumber and [Process] = @Process) and [UTNumber] = @UTNumber and [Process] = @Process and
Activity = @Activity";

            SqlDataReader rdr = null;
            SqlCommand cmd = new SqlCommand(Query, conn);
            try
            {
                cmd.Parameters.AddWithValue("@UTNumber", UTNumber);
                cmd.Parameters.AddWithValue("@Process", Pro);
                cmd.Parameters.AddWithValue("@Activity", Activity);
                cmd.ExecuteNonQuery();
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    ok = true;
                }
            }
            catch (Exception ex)
            {
                ok = false;
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
                if (rdr != null) rdr.Close();
            }
            return ok;
        }

        internal static List<string> GetCODataList(string param)
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            SqlDataReader rdr = null;
            List<string> list = new List<string>();
            try
            {

                if (param.Equals(""))
                {
                    cmd = new SqlCommand(@"select distinct ComponentId from SPC_Characteristic", sqlConn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandTimeout = 120;
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        list.Add(rdr["ComponentId"].ToString());
                    }
                }
                else
                {
                    cmd = new SqlCommand(@"select distinct OperationNo from SPC_Characteristic where ComponentId = @ComponentId", sqlConn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue(@"ComponentId", param);
                    cmd.CommandTimeout = 120;
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        list.Add(rdr["OperationNo"].ToString());
                    }
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return list;
        }
        #endregion

        #region inseption
        internal static List<GetInspectionData> GetSPCCharecteristicData(string componentId, string operationNo)
        {
            SqlDataReader sdr = null;
            SqlConnection conn = ConnectionManager.GetConnection();
            List<GetInspectionData> datainfo = new List<GetInspectionData>();
            GetInspectionData data = null;
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            List<string> list = new List<string>();
            object obj = new object();
            try
            {
                cmd = new SqlCommand("select * from dbo.SPC_Characteristic where ComponentId = @componentId and OperationNo = @operationNo ", conn); //where ComponentId = @componentId and OperationNo = @operationNo
                cmd.CommandType = CommandType.Text;
                // cmd.Parameters.AddWithValue("@MachineId", "");
                cmd.Parameters.AddWithValue("@componentId", componentId);
                cmd.Parameters.AddWithValue("@operationNo", operationNo);
                cmd.CommandTimeout = 120;
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        data = new GetInspectionData();
                        data.machineID = sdr["MachineId"].ToString();
                        data.componentID = sdr["ComponentId"].ToString();
                        data.operationNo = sdr["OperationNo"].ToString();
                        data.characteristicID = sdr["CharacteristicID"].ToString();
                        data.characteristicCode = sdr["CharacteristicCode"].ToString();
                        data.specificationMean = sdr["SpecificationMean"].ToString();
                        data._LSL = sdr["LSL"].ToString();
                        data._USL = sdr["USL"].ToString();
                        data._UOM = sdr["UOM"].ToString();
                        data.sampleSize = sdr["SampleSize"].ToString();
                        data.inProcessInterval = sdr["InProcessInterval"].ToString();
                        data.instrumentType = sdr["InstrumentType"].ToString();
                        data.inspectionDrawing = sdr["InspectionDrawing"].ToString();
                        data.dataType = sdr["DataType"].ToString();
                        data.interval = sdr["Interval"].ToString();
                        data.specification = sdr["Specification"].ToString();
                        data.macroLocation = sdr["MacroLocation"].ToString();
                        data._ID = sdr["ID"].ToString();
                        datainfo.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error in retriving Machine - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (conn != null) conn.Close();
            }
            return datainfo;
        }

        internal static void InspecDataInsertUpdateDelete(string MachineID, string ComponentID, string OperationNo, string CharacteristicCode, string CharacteristicID, string SpecificationMean, string LSL, string USL, string UOM, string sampleSize, string InprocessInterval, string InstrumentType, string InspectionDrawing, string dataType, string setupApproval, string interval, string Specification,
        string MacroLocation, string Id, string param, out string successFailure)
        {
            successFailure = string.Empty;
            SqlDataReader sdr = null;
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            List<string> list = new List<string>();
            try
            {
                cmd = new SqlCommand("[s_GetBFLInspectionMaster_Peekay]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MachineID", MachineID);
                cmd.Parameters.AddWithValue("@ComponentID", ComponentID);
                cmd.Parameters.AddWithValue("@OperationNo", OperationNo);
                cmd.Parameters.AddWithValue("@CharacteristicCode", CharacteristicCode);
                cmd.Parameters.AddWithValue("@CharacteristicID", CharacteristicID);
                cmd.Parameters.AddWithValue("@SpecificationMean", string.IsNullOrEmpty(SpecificationMean) ? DBNull.Value : (object)SpecificationMean);
                cmd.Parameters.AddWithValue("@LSL", string.IsNullOrEmpty(LSL) ? DBNull.Value : (object)LSL);
                cmd.Parameters.AddWithValue("@USL", string.IsNullOrEmpty(USL) ? DBNull.Value : (object)USL);
                cmd.Parameters.AddWithValue("@UOM", UOM);
                cmd.Parameters.AddWithValue("@sampleSize", (sampleSize));
                cmd.Parameters.AddWithValue("@InprocessInterval", (InprocessInterval));
                cmd.Parameters.AddWithValue("@InstrumentType", InstrumentType);
                cmd.Parameters.AddWithValue("@InspectionDrawing", InspectionDrawing);
                cmd.Parameters.AddWithValue("@dataType", (dataType));
                cmd.Parameters.AddWithValue("@setupApprovalinterval", (setupApproval));
                cmd.Parameters.AddWithValue("@interval", (interval));
                cmd.Parameters.AddWithValue("@Specification", Specification);
                cmd.Parameters.AddWithValue("@MacroLocation", 0);
                cmd.Parameters.AddWithValue("@param", param);
                cmd.CommandTimeout = 120;
                //cmd.Parameters.AddWithValue("@ID", Id);
                int x = cmd.ExecuteNonQuery();
                successFailure = "Successfull";

            }
            catch (Exception ex)
            {
                successFailure = ex.Message;
                Logger.WriteErrorLog("Error in retriving Machine - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (conn != null) conn.Close();
            }
        }

        internal static void DeleteSPCMasterData(string component, string operation)
        {
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string sqlQuery = "Delete from SPC_Characteristic where ComponentID=@ComponentID and OperationNo = @OperationNo";
            try
            {
                cmd = new SqlCommand(sqlQuery, conn);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@ComponentID", component);
                cmd.Parameters.AddWithValue("@OperationNo", operation);
                cmd.CommandTimeout = 120;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error in DeleteSPCMasterData m Nos - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        internal static void UpdateIntervalSampleSizeDrawing(string componentId, string operationNo, string inspectionDrawing, string Interval, string sampleSize, out string successFailure)
        {
            successFailure = string.Empty;
            SqlDataReader sdr = null;
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            List<string> list = new List<string>();
            object obj = null;
            try
            {
                cmd = new SqlCommand("Update SPC_Characteristic set InspectionDrawing = @InspectionDrawing,Interval = @Interval,sampleSize = @sampleSize where ComponentId = @ComponentId and operationNo = @operationNo", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@InspectionDrawing", inspectionDrawing);
                cmd.Parameters.AddWithValue("@Interval", Interval);
                cmd.Parameters.AddWithValue("@sampleSize", sampleSize);
                cmd.Parameters.AddWithValue("@operationNo", operationNo);
                cmd.Parameters.AddWithValue("@componentId", componentId);
                cmd.CommandTimeout = 120;
                obj = cmd.ExecuteScalar();
                successFailure = "Successfull";
            }
            catch (Exception ex)
            {
                successFailure = ex.Message;
                Logger.WriteErrorLog("Error in retriving Machine - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (conn != null) conn.Close();
            }
            //return obj;
        }

        internal static List<string> datatype(string componentId, string operationNo)
        {
            SqlDataReader sdr = null;
            SqlConnection conn = ConnectionManager.GetConnection();
            List<string> datainfo = new List<string>();
            SqlCommand cmd = null;
            try
            {
                cmd = new SqlCommand("select distinct DataType from SPC_Characteristic where ComponentId = @componentId and OperationNo = @operationNo ", conn); //where ComponentId = @componentId and OperationNo = @operationNo
                cmd.CommandType = CommandType.Text;
                // cmd.Parameters.AddWithValue("@MachineId", "");
                cmd.Parameters.AddWithValue("@componentId", componentId);
                cmd.Parameters.AddWithValue("@operationNo", operationNo);
                cmd.CommandTimeout = 120;
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        datainfo.Add(sdr["DataType"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
                if (sdr != null) sdr.Close();
            }
            return datainfo;
        }
        #endregion

        internal static List<ComboboxInspectionEntity> GetFPNumberList()
        {
            List<ComboboxInspectionEntity> lst = new List<ComboboxInspectionEntity>();
            List<string> FPNoList = new List<string>();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlDataReader rdr = null;
            try
            {
                SqlCommand cmd = new SqlCommand(@"select distinct FPNumber,HeatNumber,UTNumber from MaterialInwardingProcessPeekay_Main MIP inner join dbo.componentoperationpricing COP ON MIP.FPNumber=COP.Componentid where  PartInDateTime is not null order by FPNumber", sqlConn);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ComboboxInspectionEntity TTData = new ComboboxInspectionEntity();
                    if (!Convert.IsDBNull(rdr["UTNumber"]) && !string.IsNullOrEmpty(rdr["UTNumber"].ToString()))
                        TTData.UTNumber = rdr["UTNumber"].ToString();
                    if (!Convert.IsDBNull(rdr["HeatNumber"]) && !string.IsNullOrEmpty(rdr["HeatNumber"].ToString()))
                        TTData.HeatNumber = rdr["HeatNumber"].ToString();
                    if (!Convert.IsDBNull(rdr["FPNumber"]) && !string.IsNullOrEmpty(rdr["FPNumber"].ToString()))
                        TTData.FPNumber = rdr["FPNumber"].ToString();
                    lst.Add(TTData);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (rdr != null) rdr.Close();
            }
            return lst;
        }

        internal static List<string> GetUTNumberForFPNumber(string FPNumber)
        {
            List<string> UTNumberList = new List<string>();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlDataReader rdr = null;
            try
            {
                SqlCommand cmd = new SqlCommand(@"select distinct UTNumber from MaterialInwardingProcessPeekay_Main MIP inner join dbo.componentoperationpricing COP ON MIP.FPNumber=COP.Componentid where FPNumber=@FPNumber and PartInDateTime is not null order by UTNumber", sqlConn);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@FPNumber", FPNumber);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UTNumberList.Add(rdr["UTNumber"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (rdr != null) rdr.Close();
            }
            return UTNumberList;
        }

        internal static InspectionReportEntity GetAllInspectionReportData(string param, string selectedMachine, string FpNumber, string UTNumber, string operationNumber)
        {
            InspectionReportEntity inspectionReportData = new InspectionReportEntity();
            InspectionParameters InsParams = new InspectionParameters();
            ObservableCollection<InspectionReportParameters> InsReportParamsList = new ObservableCollection<InspectionReportParameters>();
            InspectionReportParameters insReportParam = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlDataReader rdr = null;
            try
            {
                SqlCommand cmd = new SqlCommand("s_ViewInspectionDetails_Peekay", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Param", param);
                cmd.Parameters.AddWithValue("@machineid", selectedMachine);
                cmd.Parameters.AddWithValue("@FPNumber", FpNumber);
                cmd.Parameters.AddWithValue("@UTNo", UTNumber);
                cmd.Parameters.AddWithValue("@operationNo", operationNumber);
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        InsParams.UTNumber = rdr["UTNumber"].ToString();
                        InsParams.FPNumber = rdr["FPNumber"].ToString();
                        InsParams.MPPNumber = rdr["MPPNo"].ToString();
                        InsParams.MPINumber = rdr["MPINumber"].ToString();
                        //InsParams.MachineID = rdr["MachineID"].ToString();
                        InsParams.FPDescription = rdr["FPDescription"].ToString();
                        InsParams.MaterialGrade = rdr["MaterialGrade"].ToString();
                        InsParams.DrawingNumber = rdr["DrawingNumber"].ToString();
                        InsParams.HeatNumber = rdr["HeatNumber"].ToString();

                        InsParams.CustomerName = rdr["CustomerName"].ToString();
                        InsParams.ReportNo = rdr["ReportNo"].ToString();
                        InsParams.PartName = rdr["Partname"].ToString();
                        InsParams.PartNumber = rdr["PartNumber"].ToString();
                        InsParams.WOnoNDate = rdr["WONoAndDate"].ToString();

                        InsParams.PONum = rdr["PONumber"].ToString();
                        InsParams.Quantity = rdr["Quantity"].ToString();
                        InsParams.PODate = rdr["PODate"].ToString();

                        InsParams.NDEReq = rdr["NDEReqd"].ToString();
                        InsParams.SupplierName = rdr["SupplierName"].ToString();
                        InsParams.HydoTestReqd = rdr["HydroTestReqd"].ToString();

                        try
                        {
                            if (!Convert.IsDBNull(rdr["InspDate"]) && !string.IsNullOrEmpty(rdr["InspDate"].ToString()))
                                InsParams.InspDate = Convert.ToDateTime(rdr["InspDate"].ToString()).ToString("dd-MMM-yyyy hh:mm:ss tt");
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteErrorLog(ex.ToString());
                        }
                        try
                        {
                            if (!Convert.IsDBNull(rdr["WONoAndDate"]) && !string.IsNullOrEmpty(rdr["WONoAndDate"].ToString()))
                                InsParams.WONum = rdr["WONoAndDate"].ToString().Split(new char[] { '&' })[0];
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteErrorLog(ex.ToString());
                        }
                    }

                    if (rdr.NextResult())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                insReportParam = new InspectionReportParameters();
                                insReportParam.BLNo = rdr["BLNo"].ToString();
                                insReportParam.OperationNo = rdr["OperationNo"].ToString();
                                insReportParam.DrawingSpec = rdr["DrawingSpec"].ToString();
                                insReportParam.Specification = rdr["Specification"].ToString();
                                insReportParam.SpecificationMean = rdr["SpecificationMean"].ToString();
                                insReportParam.LSL = rdr["LSL"].ToString();
                                insReportParam.USL = rdr["USL"].ToString();
                                insReportParam.MethodOfInspection = rdr["MethodOfInspection"].ToString();
                                insReportParam.Observation = rdr["Observation"].ToString();
                                insReportParam.Operator = rdr["Operator"].ToString();
                                insReportParam.OperatorRemarks = rdr["OperatorRemarks"].ToString();
                                insReportParam.Supervisor = rdr["Supervisor"].ToString();
                                insReportParam.SupervisorRemarks = rdr["SupervisorRemarks"].ToString();
                                if (!string.IsNullOrEmpty(rdr["LSL"].ToString()) && !string.IsNullOrEmpty(rdr["USL"].ToString()))
                                {
                                    double valLsl = 0.00, valUsl = 0.00;
                                    if (Double.TryParse(rdr["LSL"].ToString(), out valLsl) && Double.TryParse(rdr["USL"].ToString(), out valUsl))
                                    {
                                        double valObs = 0.00;
                                        if (Double.TryParse(rdr["Observation"].ToString(), out valObs))
                                        {
                                            if (Convert.ToDouble(rdr["Observation"].ToString()) < Convert.ToDouble(rdr["LSL"].ToString()) || Convert.ToDouble(rdr["Observation"].ToString()) > Convert.ToDouble(rdr["USL"].ToString()))
                                            {
                                                insReportParam.BackgroundObservation = "#FF0000";
                                            }
                                            else
                                            {
                                                insReportParam.BackgroundObservation = "#051235";
                                            }
                                        }
                                        else
                                        {
                                            insReportParam.BackgroundObservation = "#051235";
                                        }
                                    }
                                    else
                                    {
                                        insReportParam.BackgroundObservation = "#051235";
                                    }
                                }
                                else
                                {
                                    insReportParam.BackgroundObservation = "#051235";
                                }
                                InsReportParamsList.Add(insReportParam);
                            }
                        }
                    }
                }
                inspectionReportData.insparams = InsParams;
                inspectionReportData.insReportParamsList = InsReportParamsList;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (rdr != null) rdr.Close();
            }
            return inspectionReportData;
        }

        internal static List<string> GetdownCategory()
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            List<string> list = new List<string>();
            try
            {
                SqlCommand cmd = new SqlCommand(@"select distinct BreakDownType from downcodeinformation where BreakDownType Not In('MACHINING')", sqlConn);
                cmd.CommandType = System.Data.CommandType.Text;
                SqlDataReader rdr = cmd.ExecuteReader();
                list.Add("ALL");
                while (rdr.Read())
                {
                    if (!string.IsNullOrEmpty(rdr["BreakDownType"].ToString()))
                        list.Add(rdr["BreakDownType"].ToString());
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return list;
        }

        internal static ObservableCollection<Downcode> GetDowncodeList(string catagory = "", string downdescription = "", string interfaceid = "")
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            ObservableCollection<Downcode> list = new ObservableCollection<Downcode>();
            try
            {
                //SqlCommand cmd = new SqlCommand(@"SELECT downid as downcodeid, interfaceid FROM downcodeinformation
                //            left outer join [dbo].[DownCategoryInformation] on [DownCategoryInformation].[DownCategory]=downcodeinformation.[Catagory]
                //            where  ([Catagory]=@catagory  or @catagory='') and  isnumeric(interfaceid) = 1	and interfaceid not in('9995','9996','9997','9998','9999') order by downid", sqlConn);

                //cmd.CommandType = System.Data.CommandType.Text;
                //cmd.Parameters.AddWithValue("@catagory", catagory);
                //cmd.Parameters.AddWithValue("@downdescription", downdescription);
                //cmd.Parameters.AddWithValue("@interfaceid", interfaceid);


                SqlCommand cmd = new SqlCommand(@"select distinct  downid as downcodeid ,interfaceid from downcodeinformation where BreakDownType Not In('MACHINING') and  ([BreakDownType]=@BreakDownType  or @BreakDownType='') order by downid", sqlConn);

                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@BreakDownType", catagory);

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new Downcode() { DowncodeID = rdr["downcodeid"].ToString().Trim(), InterfaceID = rdr["interfaceid"].ToString().Trim() });
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return list;
        }

        internal static void FolderPathDefinition(string item, string path, string extension)
        {
            SqlDataReader sdr = null;
            string currentShift = string.Empty;
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            string sqlQuery = string.Empty;
            try
            {
                sqlQuery = "[s_InsertUpdateFolderPathDefinition]";
                cmd = new SqlCommand(sqlQuery, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FolderType", item);
                cmd.Parameters.AddWithValue("@FolderPath", path);
                cmd.Parameters.AddWithValue("@FolderExtension", extension);
                cmd.Parameters.AddWithValue("@param", "Save");
                cmd.CommandTimeout = 120;
                sdr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error in retriving all ISOItem Nos - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (conn != null) conn.Close();
            }
        }

        internal static PathDetails GetAllPaths()
        {
            PathDetails pd = new PathDetails();
            SqlDataReader rdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(@"select * from [dbo].[FolderPathDefinition] where FolderType IN ('WorkInsNonMachininig','ProcDocsNonMachininig','WorkOrderURLPath')", sqlConn);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["FolderType"].ToString().Equals("WorkInsNonMachininig", StringComparison.OrdinalIgnoreCase))
                        pd.WORK_INST_PATH = rdr["FolderPath"].ToString();
                    if (rdr["FolderType"].ToString().Equals("ProcDocsNonMachininig", StringComparison.OrdinalIgnoreCase))
                        pd.PROC_DOC_PATH = rdr["FolderPath"].ToString();
                    if (rdr["FolderType"].ToString().Equals("WorkOrderURLPath", StringComparison.OrdinalIgnoreCase))
                        pd.WO_URL = rdr["FolderPath"].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return pd;
        }



        internal static bool InsertDownCodes(string eventName, string downInterfaceId)
        {
            bool res = false;
            SqlCommand cmd = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            string sqlQry = string.Empty;
            try
            {
                if (eventName.Equals("Start", StringComparison.OrdinalIgnoreCase))
                    sqlQry = @"
								If Exists(select * from PostMachiningDownCodeDetails_Peekay where DownInterfaceID=@DownInterfaceID and DownEvent='Start')
								Begin
                                       Update PostMachiningDownCodeDetails_Peekay set DownEvent=@DownEvent,Operator=@Operator where DownInterfaceID=@DownInterfaceID
								End
								Else	
                                Begin							
                                      Insert into PostMachiningDownCodeDetails_Peekay (Process,DownStartTime,DownInterfaceID,DownEvent,Operator) values(@Process,@DownTime,@DownInterfaceID,@DownEvent,@Operator)
				                End";
                else
                    sqlQry = @"
								If Exists(select * from PostMachiningDownCodeDetails_Peekay where DownInterfaceID=@DownInterfaceID and DownEvent='Start')
								Begin
                                       Update PostMachiningDownCodeDetails_Peekay set DownEvent=@DownEvent,DownEndTime=@DownTime,Operator=@Operator where DownInterfaceID=@DownInterfaceID
								End
								Else	
                                Begin							
                                      Insert into PostMachiningDownCodeDetails_Peekay (Process,DownEndTime,DownInterfaceID,DownEvent,Operator) values(@Process,@DownTime,@DownInterfaceID,@DownEvent,@Operator)
				                End";

                cmd = new SqlCommand(sqlQry, sqlConn);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Process", Utility.Process);
                cmd.Parameters.AddWithValue("@DownTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@DownInterfaceID", downInterfaceId);
                cmd.Parameters.AddWithValue("@Operator", LoginPage.LoginUserName);
                cmd.Parameters.AddWithValue("@DownEvent", eventName);
                cmd.ExecuteNonQuery();
                res = true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return res;
        }

        internal static bool EndPrevousDownCodes(string eventName, string downInterfaceId)
        {
            bool res = false;
            SqlCommand cmd = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            string sqlQry = string.Empty;
            try
            {

                sqlQry = @"Update PostMachiningDownCodeDetails_Peekay set DownEvent='Close',DownEndTime=@DownTime,Operator=@Opr where DownInterfaceID=@DownInterfaceID and DownEvent='Start'";
                cmd = new SqlCommand(sqlQry, sqlConn);
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@DownTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@DownInterfaceID", downInterfaceId);
                cmd.Parameters.AddWithValue("@DownEvent", eventName);
                cmd.Parameters.AddWithValue("@Opr", LoginPage.LoginUserName);
                cmd.ExecuteNonQuery();
                res = true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return res;
        }

        internal static int GetCurrentDown()
        {
            int dc = 0;
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(@"SELECT TOP 1 DownInterfaceID FROM PostMachiningDownCodeDetails_Peekay where DownEvent = 'Start' ORDER BY ID DESC", sqlConn);
                sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    if (!Convert.IsDBNull(sdr["DownInterfaceID"]) && !string.IsNullOrEmpty(sdr["DownInterfaceID"].ToString()))
                        dc = Convert.ToInt32(sdr["DownInterfaceID"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return dc;
        }

        internal static bool CheckInDown(string process)
        {
            bool inDown = false;
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(@"SELECT* FROM PostMachiningDownCodeDetails_Peekay WHERE DownStartTime IN (SELECT max(DownStartTime) FROM PostMachiningDownCodeDetails_Peekay where Process = @process)", sqlConn);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@process", process);
                sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    inDown = sdr["DownEvent"].ToString().Equals("Close", StringComparison.OrdinalIgnoreCase) ? false : true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return inDown;
        }

        internal static List<DownSummary> GetDownSummary(DateTime startTime, DateTime endTime)
        {
            List<DownSummary> dsRes = new List<DownSummary>();
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            try
            {
                string sqlQry = @"(select DownStartTime, DownEndTime, Operator, DCI.downid, dbo.f_FormatTime(DateDiff(s, DownStartTime, DownEndTime),'hh:mm:ss') as 
									DownTime FROM[PostMachiningDownCodeDetails_Peekay] DT inner join downcodeinformation DCI on DT.DownInterfaceID = DCI.interfaceid 
									where process = @process and[DownStartTime] >= @startDate and [DownEndTime] <= @EndDate) order by DownStartTime desc";

                cmd = new SqlCommand(sqlQry, sqlConn);
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@startDate", startTime.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@EndDate", endTime.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@process", Utility.Process);
                sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    DownSummary ds = new DownSummary();
                    ds.DOWN_ID = sdr["downid"].ToString();
                    if (!Convert.IsDBNull(sdr["DownStartTime"]) && !string.IsNullOrEmpty(sdr["DownStartTime"].ToString()))
                        ds.START_TIME = Convert.ToDateTime(sdr["DownStartTime"].ToString());
                    if (!Convert.IsDBNull(sdr["DownEndTime"]) && !string.IsNullOrEmpty(sdr["DownEndTime"].ToString()))
                        ds.END_TIME = Convert.ToDateTime(sdr["DownEndTime"].ToString());
                    ds.OPERATOR = sdr["Operator"].ToString();
                    ds.DOWN_TIME = sdr["DownTime"].ToString();
                    dsRes.Add(ds);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return dsRes;
        }

        internal static string GetLogicalDayStartEnd(out string logicalDayEnd)
        {
            string logicalDayStart = DateTime.Now.AddDays(-1).ToString("dd-MMM-yyyy hh:mm:ss tt");
            logicalDayEnd = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(@"SELECT  [dbo].[f_GetLogicalDayStart](@FromDate) as stTime ;SELECT  [dbo].[f_GetLogicalDayEnd](@FromDate) as ndTime ", sqlConn);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@FromDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    if (!Convert.IsDBNull(sdr["stTime"]) && !string.IsNullOrEmpty(sdr["stTime"].ToString()))
                        logicalDayStart = Convert.ToDateTime(sdr["stTime"].ToString()).ToString("dd-MMM-yyyy hh:mm:ss tt");
                }
                sdr.NextResult();
                if (sdr.Read())
                {
                    if (!Convert.IsDBNull(sdr["ndTime"]) && !string.IsNullOrEmpty(sdr["ndTime"].ToString()))
                        logicalDayEnd = Convert.ToDateTime(sdr["ndTime"].ToString()).ToString("dd-MMM-yyyy hh:mm:ss tt");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return logicalDayStart;
        }

        internal static int GetMaxBatchId(string UTNumber, string FPNumber)
        {
            int BatchId = 0;
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            try
            {
                string sqlQry = @"select max(isnull(batchid, 0)) as BatchId from[PostMachiningProcessInfo_Peekay] where [UTNumber] = @UTNumber and[Process] = @Process and FPNumber = @FPNumber";

                cmd = new SqlCommand(sqlQry, sqlConn);
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.AddWithValue("@UTNumber", UTNumber);
                cmd.Parameters.AddWithValue("@Process", Utility.Process);
                cmd.Parameters.AddWithValue("@FPNumber", FPNumber);
                sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    if (!Convert.IsDBNull(sdr["BatchId"]) && !string.IsNullOrEmpty(sdr["BatchId"].ToString()))
                        BatchId = Convert.ToInt32(sdr["BatchId"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return BatchId + 1;
        }

        #region Dispatch Process
        internal static bool checkUTNUmberscanned(string UTNumber, out string batchid)
        {
            SqlDataReader sdr = null;
            batchid = "";
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            bool present = false;
            try
            {
                string query = "Select * from PostMachiningProcessInfo_Peekay where UTNumber=@UTNumber and Process = @Process";
                cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.AddWithValue("@UTNumber", UTNumber);
                cmd.Parameters.AddWithValue("@Process", Utility.Process);
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    present = true;
                    while (sdr.Read())
                    {
                        batchid = sdr["BatchId"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return present;
        }

        internal static void SaveDispatchdata(QRVals data, string batchid)
        {
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            try
            {
                string query = @"INSERT INTO PostMachiningProcessInfo_Peekay([Process],[FPNumber],[UTNUMBER],[Operator],[Activity],[ActivityTS],[BatchId])
									VALUES(@Process, @FPNUMBER, @UTNUMBER, @Operator, @Activity, @ActivityTS, @BatchId) ";
                cmd = new SqlCommand(query, sqlConn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Process", Utility.Process);
                cmd.Parameters.AddWithValue("@FPNUMBER", data.FPNO);
                cmd.Parameters.AddWithValue("@UTNUMBER", data.UTNO);
                cmd.Parameters.AddWithValue("@Operator", Utility.LoggedInUserName);
                cmd.Parameters.AddWithValue("@Activity", "Scanout");
                cmd.Parameters.AddWithValue("@ActivityTS", DateTime.Now);
                cmd.Parameters.AddWithValue("@BatchId", batchid);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
        }

        internal static List<DeburringProcessEntity> GetDisaptchData(string Default, string process)
        {
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            List<DeburringProcessEntity> dbentry_info = new List<DeburringProcessEntity>();
            DeburringProcessEntity dbentry_data = null;
            try
            {
                cmd = new SqlCommand("S_GetPostMachiningProcessInfo_Peekay", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ReportType", Default);
                cmd.Parameters.AddWithValue("@process", process);
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        dbentry_data = new DeburringProcessEntity();
                        dbentry_data.FPNumber = sdr["FPNUMBER"].ToString();
                        dbentry_data.WoNum = sdr["WorkOrderNumber"].ToString();
                        dbentry_data.UTNumber = sdr["UTNumber"].ToString();
                        dbentry_data.heatNum = sdr["HeatNumber"].ToString();
                        dbentry_data.Operator = sdr["Operator"].ToString();
                        dbentry_data.Remarks = sdr["Remarks"].ToString();
                        if (!sdr.IsDBNull(sdr.GetOrdinal("Scanin")))
                            dbentry_data.ReceivedDateTime = Convert.ToDateTime(sdr["Scanin"]);
                        if ((sdr["Scanout"] != (DBNull.Value)))
                            dbentry_data.ScanOutTime = Convert.ToDateTime(sdr["Scanout"]);
                        //dbentry_data.ScanOutTime = DateTime.Now;
                        dbentry_info.Add(dbentry_data);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return dbentry_info;
        }


        internal static bool Dispatchscanout(string UTNumber, string Activity)
        {
            string Pro = ConfigurationManager.AppSettings["Process"].ToString();
            bool ok = false;
            SqlConnection conn = ConnectionManager.GetConnection();
            //string Query = "Select * from PostMachiningProcessInfo_Peekay Where [UTNumber]=@UTNumber And [Process]=@Process And [Activity]=@Activity";
            string Query = @"Select * from [PostMachiningProcessInfo_Peekay] where Process=@Process and UTNumber=@UTNumber and Activity= @Activity";

            SqlDataReader rdr = null;
            SqlCommand cmd = new SqlCommand(Query, conn);
            try
            {
                cmd.Parameters.AddWithValue("@UTNumber", UTNumber);
                cmd.Parameters.AddWithValue("@Process", Pro);
                cmd.Parameters.AddWithValue("@Activity", Activity);
                cmd.ExecuteNonQuery();
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    ok = true;
                }
            }
            catch (Exception ex)
            {
                ok = false;
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
                if (rdr != null) rdr.Close();
            }
            return ok;
        }


        #endregion

        #region PRODUCTION DATA
        internal static List<ProductionDataEntity> GetProductiondatabydate(string FromDate, string ToDate)
        {
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            List<ProductionDataEntity> ProductiondataList = new List<ProductionDataEntity>();
            ProductionDataEntity ProductionDataset = null;
            try
            {
                cmd = new SqlCommand("S_GetNonMachiningProductionData_Peekay ", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@process", Utility.Process);
                cmd.Parameters.AddWithValue("startdate", FromDate);
                cmd.Parameters.AddWithValue("Enddate", ToDate);
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        ProductionDataset = new ProductionDataEntity();
                        ProductionDataset.FPNumber = sdr["FPNUMBER"].ToString();
                        ProductionDataset.WONumber = sdr["WorkOrderNumber"].ToString();
                        ProductionDataset.UTNumber = sdr["UTNumber"].ToString();
                        ProductionDataset.HeatNumber = sdr["HeatNumber"].ToString();
                        ProductionDataset.BatchID = sdr["BatchId"].ToString();
                        ProductionDataset.ScanIn = sdr["Scanin"].ToString();
                        ProductionDataset.Start = sdr["Start"].ToString();
                        ProductionDataset.Operator = sdr["Operator"].ToString();
                        ProductionDataset.End = sdr["End"].ToString();
                        ProductionDataset.ScanOut = sdr["Scanout"].ToString();
                        ProductionDataset.Remarks = sdr["Remarks"].ToString();
                        ProductiondataList.Add(ProductionDataset);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return ProductiondataList;
        }

        internal static List<ProductionDataEntity> GetProductiondatabydate(string UTNumber)
        {
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            List<ProductionDataEntity> ProductiondataList = new List<ProductionDataEntity>();
            ProductionDataEntity ProductionDataset = null;
            try
            {
                cmd = new SqlCommand("S_GetNonMachiningProductionData_Peekay ", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UTNumber", UTNumber);
                cmd.Parameters.AddWithValue("@Process", Utility.Process);
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        ProductionDataset = new ProductionDataEntity();
                        ProductionDataset.FPNumber = sdr["FPNUMBER"].ToString();
                        ProductionDataset.WONumber = sdr["WorkOrderNumber"].ToString();
                        ProductionDataset.UTNumber = sdr["UTNumber"].ToString();
                        ProductionDataset.HeatNumber = sdr["HeatNumber"].ToString();
                        ProductionDataset.BatchID = sdr["BatchId"].ToString();
                        ProductionDataset.ScanIn = sdr["Scanin"].ToString();
                        ProductionDataset.Start = sdr["Start"].ToString();
                        ProductionDataset.Operator = sdr["Operator"].ToString();
                        ProductionDataset.End = sdr["End"].ToString();
                        ProductionDataset.ScanOut = sdr["Scanout"].ToString();
                        ProductionDataset.Remarks = sdr["Remarks"].ToString();
                        ProductiondataList.Add(ProductionDataset);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return ProductiondataList;
        }

        internal static List<ProductionDataEntity> GetProductiondata(string Default)
        {
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            List<ProductionDataEntity> ProductiondataList = new List<ProductionDataEntity>();
            ProductionDataEntity ProductionDataset = null;
            try
            {
                cmd = new SqlCommand("S_GetNonMachiningProductionData_Peekay ", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", Default);
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        ProductionDataset = new ProductionDataEntity();
                        ProductionDataset.FPNumber = sdr["FPNUMBER"].ToString();
                        ProductionDataset.WONumber = sdr["WorkOrderNumber"].ToString();
                        ProductionDataset.UTNumber = sdr["UTNumber"].ToString();
                        ProductionDataset.HeatNumber = sdr["HeatNumber"].ToString();
                        ProductionDataset.BatchID = sdr["BatchId"].ToString();
                        ProductionDataset.ScanIn = sdr["Scanin"].ToString();
                        ProductionDataset.Start = sdr["Start"].ToString();
                        ProductionDataset.Operator = sdr["Operator"].ToString();
                        ProductionDataset.End = sdr["End"].ToString();
                        ProductionDataset.ScanOut = sdr["Scanout"].ToString();
                        ProductionDataset.Remarks = sdr["Remarks"].ToString();
                        ProductiondataList.Add(ProductionDataset);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return ProductiondataList;
        }

        internal static List<ComboboxEntity> GetcomboboxLists()
        {
            List<ComboboxEntity> lst = new List<ComboboxEntity>();
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlDataReader sdr = null;
            SqlCommand cmd = null;
            try
            {
                cmd = new SqlCommand("SELECT distinct [UTNumber] ,[HeatNumber],[MPINumber] FROM [dbo].[MaterialInwardingProcessPeekay_Main]", conn);
                sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    ComboboxEntity TTData = new ComboboxEntity();
                    if (!Convert.IsDBNull(sdr["UTNumber"]) && !string.IsNullOrEmpty(sdr["UTNumber"].ToString()))
                        TTData.UTNumber = sdr["UTNumber"].ToString();
                    if (!Convert.IsDBNull(sdr["MPINumber"]) && !string.IsNullOrEmpty(sdr["MPINumber"].ToString()))
                        TTData.MPINumber = sdr["MPINumber"].ToString();
                    if (!Convert.IsDBNull(sdr["HeatNumber"]) && !string.IsNullOrEmpty(sdr["HeatNumber"].ToString()))
                        TTData.HeatNumber = sdr["HeatNumber"].ToString();
                    lst.Add(TTData);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error Log - \n " + ex.ToString());
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (conn != null) conn.Close();
            }
            return lst;
        }

        public static string GetLogicalDay(DateTime fromdate)
        {
            string list = string.Empty;
            SqlDataReader sdr = null;
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string sqlQuery = string.Empty;
            try
            {
                sqlQuery = "select [dbo].[f_GetLogicalDayStart](' " + fromdate.ToString("yyyy-MM-dd 13:00:00") + "') as logicalDay";
                cmd = new SqlCommand(sqlQuery, conn);
                //cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 120;
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (!Convert.IsDBNull(sdr["logicalDay"]))
                        {
                            list = DateTime.Parse((sdr["logicalDay"]).ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
                else
                {
                    list = null;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error in Retriving Machines - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (conn != null) conn.Close();
            }
            return list;
        }

        public static string GetLogicalDayEnd(DateTime Todate)
        {
            string list = string.Empty;
            SqlDataReader sdr = null;
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string sqlQuery = string.Empty;
            try
            {
                sqlQuery = "select [dbo].[f_GetLogicalDayEnd](' " + Todate.ToString("yyyy-MM-dd 13:00:00") + "') as logicalDay";
                cmd = new SqlCommand(sqlQuery, conn);
                //cmd.CommandType = System.Data.CommandType.Text;
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (!Convert.IsDBNull(sdr["logicalDay"]))
                        {
                            list = DateTime.Parse((sdr["logicalDay"]).ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
                else
                {
                    list = null;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error in Retriving Machines - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (conn != null) conn.Close();
            }
            return list;
        }
        #endregion


        internal static string Checkforutnoprocessed(string uTNO)
        {
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string process = "";
            try
            {
                cmd = new SqlCommand(@"select p.process,p.utnumber from PostMachiningProcessInfo_Peekay p
										where p.activity = 'Scanin'
										and p.UTNumber = @utnumber
										group by p.process,p.UTNumber
										having  max(p.activityts) > 
											(select isnull(max(activityTS),'1991-01-01') from PostMachiningProcessInfo_Peekay
												where activity = 'Scanout'
												and utnumber = p.UTNumber
												and process=p.process
											)", sqlConn);
                cmd.Parameters.AddWithValue("@utnumber", uTNO);
                cmd.CommandType = CommandType.Text;
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        process = sdr["process"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return process;
        }
    }
}
