using DeburringProcess_Peekay.Helpers;
using DeburringProcess_Peekay.Models;
using PackingModule_Rexroth.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DeburringProcess_Peekay.DB_Connection
{
    class MivinDataBaseAccess
    {
        internal static CurrentShiftTime GetCurrentShiftTime()
        {
            SqlConnection conn = null;
            SqlDataReader dataReader = null;
            CurrentShiftTime currentShiftTime = new CurrentShiftTime();
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(@"s_GetCurrentShiftTime", conn);
                sqlCommand.Parameters.AddWithValue("@StartDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sqlCommand.Parameters.AddWithValue("@Param", "");
                sqlCommand.CommandType = CommandType.StoredProcedure;
                dataReader = sqlCommand.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        currentShiftTime.ShiftStartDate = Convert.ToDateTime(dataReader["Startdate"]);
                        currentShiftTime.Shift = dataReader["shiftname"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting current shift time in method GetCurrentShiftTime - \n" + ex.ToString());
            }
            finally
            {
                if (dataReader != null) dataReader.Close();
                if (conn != null) conn.Close();
            }
            return currentShiftTime;
        }

        internal static List<ShiftDetailsEntity> GetAllshiftDetails()
        {
            List<ShiftDetailsEntity> list = new List<ShiftDetailsEntity>();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            DataTable table = new DataTable();
            SqlDataReader rdr = null;
            try
            {
                SqlCommand cmd = new SqlCommand(@"select * from shiftdetails where running=1 order by shiftid", sqlConn);
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ShiftDetailsEntity shftVals = new ShiftDetailsEntity();
                        if (!Convert.IsDBNull(rdr["ShiftId"]))
                        {
                            shftVals.ShiftID = Convert.ToString(rdr["ShiftId"]);
                        }
                        if (!Convert.IsDBNull(rdr["ShiftName"]))
                        {
                            shftVals.ShiftName = Convert.ToString(rdr["ShiftName"]);
                        }
                        if (!Convert.IsDBNull(rdr["FromDay"]))
                        {
                            shftVals.FromDay = Convert.ToString(rdr["FromDay"]);
                        }
                        if (!Convert.IsDBNull(rdr["FromTime"]))
                        {
                            DateTime dt = Convert.ToDateTime(rdr["FromTime"]);
                            shftVals.FromTime = dt.ToString("hh:mm:ss tt");
                        }
                        if (!Convert.IsDBNull(rdr["ToDay"]))
                        {
                            shftVals.ToDay = Convert.ToString(rdr["ToDay"]);
                        }
                        if (!Convert.IsDBNull(rdr["ToTime"]))
                        {
                            DateTime dt = Convert.ToDateTime(rdr["ToTime"]);
                            shftVals.ToTime = dt.ToString("hh:mm:ss tt");
                        }
                        list.Add(shftVals);
                    }
                }
                else
                {
                    list = null;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting Shift Details : " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (rdr != null) rdr.Close();
            }
            return list;
        }

        internal static List<string> GetShiftDetails()
        {
            ShiftDetailsEntity shftVals = new ShiftDetailsEntity();
            SqlConnection sqlConn = null;
            DataTable table = new DataTable();
            List<string> shiftList = new List<string>();
            SqlDataReader rdr = null;
            try
            {
                sqlConn = ConnectionManager.GetConnection();
                SqlCommand cmd = new SqlCommand(@"select * from shiftdetails where running = 1", sqlConn);
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        shiftList.Add(rdr["ShiftName"].ToString());
                    }
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
            return shiftList;
        }

        internal static ShiftDetailsEntity GetShiftDetails(string shiftId)
        {
            ShiftDetailsEntity shftVals = new ShiftDetailsEntity();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            DataTable table = new DataTable();
            SqlDataReader rdr = null;
            try
            {
                SqlCommand cmd = new SqlCommand(@"select * from shiftdetails where running=1 and ShiftId = @ShiftId ", sqlConn);
                cmd.Parameters.AddWithValue("@shiftId", shiftId);
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        if (!Convert.IsDBNull(rdr["ShiftId"]))
                        {
                            shftVals.ShiftID = Convert.ToString(rdr["ShiftId"]);
                        }
                        if (!Convert.IsDBNull(rdr["ShiftName"]))
                        {
                            shftVals.ShiftName = Convert.ToString(rdr["ShiftName"]);
                        }
                        if (!Convert.IsDBNull(rdr["FromDay"]))
                        {
                            shftVals.FromDay = Convert.ToString(rdr["FromDay"]);
                        }
                        if (!Convert.IsDBNull(rdr["FromTime"]))
                        {
                            shftVals.FromTime = Convert.ToString(rdr["FromTime"]);
                        }
                        if (!Convert.IsDBNull(rdr["ToDay"]))
                        {
                            shftVals.ToDay = Convert.ToString(rdr["ToDay"]);
                        }
                        if (!Convert.IsDBNull(rdr["ToTime"]))
                        {
                            shftVals.ToTime = Convert.ToString(rdr["ToTime"]);
                        }
                    }
                }
                else
                {
                    shftVals = null;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting Shift Details : " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (rdr != null) rdr.Close();
            }
            return shftVals;
        }

        internal static bool CheckShiftId(string shiftId)
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            bool alreadyPresent = false;
            object obj = null;
            SqlCommand cmd = null;
            try
            {
                string sqlQuery = "select shiftId from shiftDetails where shiftId = @shiftId and Running = 1";
                cmd = new SqlCommand(sqlQuery, sqlConn);
                cmd.Parameters.AddWithValue("@shiftId", shiftId);
                obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    alreadyPresent = true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error Log - \n " + ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return alreadyPresent;
        }

        internal static bool CheckForTheTimeEntry(string fromTime, string toTime)
        {
            bool isPresent = false;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            DataTable table = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand(@"s_GetCurrentShiftTime", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StartDate", fromTime);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        isPresent = true;
                    }
                }
                if (rdr != null) rdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return isPresent;
        }

        internal static void UpdateShiftDetails(string shiftId, string shiftName, string fromDay, string toDay, DateTime fromTime, DateTime toTime)
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            int rowsAffected = 0, fDay = 0, tDay = 0;
            SqlCommand cmd = null;
            string sqlQuery = string.Empty;
            sqlQuery = "update ShiftDetails set shiftName = @shiftName ,fromDay =@fromDay, toDay= @toDay, fromTime= @fromTime,Totime= @Totime where shiftId= @shiftId ";
            try
            {
                cmd = new SqlCommand(sqlQuery, sqlConn);
                cmd.Parameters.AddWithValue("@shiftId", shiftId);
                cmd.Parameters.AddWithValue("@shiftName", shiftName);
                if (fromDay.Equals("Tomorrow")) fDay = 1;
                else if (fromDay.Equals("Yesterday")) fDay = 2;

                cmd.Parameters.AddWithValue("@fromDay", fDay);
                if (toDay.Equals("Tomorrow")) tDay = 1;
                else if (toDay.Equals("Yesterday")) tDay = 2;

                cmd.Parameters.AddWithValue("@toDay", tDay);
                cmd.Parameters.AddWithValue("@fromTime", fromTime);
                cmd.Parameters.AddWithValue("@toTime", toTime);
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error Log - \n " + ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
        }

        internal static bool CheckForShiftName(string shiftName, string shiftId)
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            object obj = null;
            string valz = string.Empty;
            bool isPresent = false;
            try
            {
                SqlCommand cmd = new SqlCommand("Select shiftId from ShiftDetails where  shiftName = @shiftName and shiftId != @shiftId ", sqlConn);
                cmd.Parameters.AddWithValue("@shiftId", shiftId);
                cmd.Parameters.AddWithValue("@shiftName", shiftName);
                obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    isPresent = true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error Log - \n " + ex.ToString());
            }
            finally
            {
                sqlConn.Close();
            }
            return isPresent;
        }

        internal static void InsertShiftDetails(string shiftId, string shiftName, string fromDay, string toDay, DateTime fromTime, DateTime toTime)
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            int rowsAffected = 0, fDay = 0, tDay = 0;
            string sqlQuery = "Insert into ShiftDetails ([shiftName],[fromDay] ,[toDay],[fromTime] ,[toTime], [shiftId],Running) values  (@shiftName , @fromDay, @toDay,  @fromTime, @Totime,@shiftId,1 ) ";
            try
            {
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn);
                cmd.Parameters.AddWithValue("@shiftId", shiftId);
                cmd.Parameters.AddWithValue("@shiftName", shiftName);
                if (fromDay.Equals("Tomorrow")) fDay = 1;
                else if (fromDay.Equals("Yesterday")) fDay = 2;
                cmd.Parameters.AddWithValue("@fromDay", fDay);
                if (toDay.Equals("Tomorrow")) tDay = 1;
                else if (toDay.Equals("Yesterday")) tDay = 2;
                cmd.Parameters.AddWithValue("@toDay", tDay);
                cmd.Parameters.AddWithValue("@fromTime", fromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@toTime", toTime.ToString("yyyy-MM-dd HH:mm:ss"));
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error Log - \n " + ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
        }

        internal static void RemoveAllShiftdata()
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            int rowsAffected = 0;
            SqlCommand cmd = null;
            string sqlQuery = "Update shiftDetails SET Running = 0 where Running = 1";
            try
            {
                cmd = new SqlCommand(sqlQuery, sqlConn);
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error Log - \n " + ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
        }

        internal static ObservableCollection<EmpInfoEntity> GetEmployeeInformation()
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            ObservableCollection<EmpInfoEntity> EmpInfoList = new ObservableCollection<EmpInfoEntity>();
            EmpInfoEntity employeeInfo = null;
            try
            {
                string sqlQuery = "select * from [EmployeeInformation]";
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        employeeInfo = new EmpInfoEntity();
                        employeeInfo.EmpId = rdr["Employeeid"].ToString();
                        employeeInfo.Name = rdr["Name"].ToString();
                        employeeInfo.Phone = rdr["phone"].ToString();
                        employeeInfo.IsAdmin = Convert.ToInt32(rdr["isadmin"]) == 1 ? true : false;
                        employeeInfo.Email = rdr["Email"].ToString();
                        employeeInfo.Password = !string.IsNullOrEmpty(rdr["upassword"].ToString()) ? rdr["upassword"].ToString() : "";
                        employeeInfo.EmployeeRole = rdr["EmployeeRole"].ToString();
                        EmpInfoList.Add(employeeInfo);
                    }
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(" Error Occured!!!\n" + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return EmpInfoList;
        }

        internal static void DeleteEmployeeInformation(string employeeId)
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            try
            {
                string query = "delete from EmployeeInformation where EmployeeID = @EmployeeID";
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(" Error Occured!!!\n" + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
        }

        internal static void UpdateEmployeeInformation(string empId, string name, string phone, bool isAdmin, string email, string password, string employeeRole, out bool isUpdated)
        {
            isUpdated = false;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            string query = @"if not exists(select * from [EmployeeInformation] where [EmployeeID]=@EmployeeID)
            BEGIN  
            INSERT INTO [dbo].[EmployeeInformation]([EmployeeID], [Name], [Phone], [IsAdmin], [Email], [upassword], [EmployeeRole])
            VALUES(@EmployeeID, @Name, @Phone, @IsAdmin, @Email, @Password, @EmployeeRole)
            END ELSE BEGIN
            update [EmployeeInformation] set [Name]=@Name, [Phone]=@Phone, [IsAdmin]=@IsAdmin, [Email]=@Email, [upassword]=@Password, [EmployeeRole]=@EmployeeRole where [EmployeeID] = @EmployeeID END";
            try
            {
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@EmployeeID", empId);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@IsAdmin", isAdmin == true ? 1 : 0);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@EmployeeRole", employeeRole);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    isUpdated = true;
                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
                if (ex.Message.Contains("UNIQUE KEY") || ex.Message.Contains("PRIMARY KEY"))
                {
                    MessageBox.Show("Same Employee ID already exists", "Information Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(ex.Message.ToString(), "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
        }

        internal static ObservableCollection<CustomerInfoEntity> GetAllCustomerInformationData()
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            ObservableCollection<CustomerInfoEntity> CustomerInfoList = new ObservableCollection<CustomerInfoEntity>();
            CustomerInfoEntity customerInfo = null;
            try
            {
                string sqlQuery = @"select * from [customerinformation]";
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        customerInfo = new CustomerInfoEntity();
                        customerInfo.CustomerID = rdr["customerid"].ToString();
                        customerInfo.CustomerName = rdr["customername"].ToString();
                        customerInfo.Address = rdr["address1"].ToString();
                        customerInfo.Place = rdr["place"].ToString();
                        customerInfo.State = rdr["state"].ToString();
                        customerInfo.Country = rdr["country"].ToString();
                        customerInfo.Pin = rdr["pin"].ToString();
                        customerInfo.Phone = rdr["phone"].ToString();
                        customerInfo.Email = rdr["email"].ToString();
                        customerInfo.ContactPerson = rdr["contactperson"].ToString();
                        CustomerInfoList.Add(customerInfo);
                    }
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(" Error Occured!!!\n" + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return CustomerInfoList;
        }

        internal static ObservableCollection<PumpInfoEntity> GetAllPumpInformationData(string pumpPartNumber)
        {
            int val = 0;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            ObservableCollection<PumpInfoEntity> pumpInfoDataList = new ObservableCollection<PumpInfoEntity>();
            PumpInfoEntity pumpInfoData = null;
            try
            {
                string sqlQuery = $"select * from [PumpMasterData] where PumpModel like '%{pumpPartNumber}%'";
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        pumpInfoData = new PumpInfoEntity();
                        pumpInfoData.PumpModel = rdr["PumpModel"].ToString();
                        pumpInfoData.CustomerModel = rdr["CustomerModel"].ToString();
                        pumpInfoData.CustomerName = rdr["CustomerName"].ToString();
                        pumpInfoData.SalesUnit = rdr["SalesUnit"].ToString();
                        pumpInfoData.PackagingType = rdr["PackagingType"].ToString();
                        pumpInfoData.PackingBoxNumber = rdr["PackingBoxNumber"].ToString();
                        pumpInfoData.PerBoxPumpQty = int.TryParse(rdr["PerBoxPumpQty"].ToString(), out val) ? val : 2;
                        pumpInfoData.PumpType = rdr["PumpType"].ToString();
                        pumpInfoData.BoxDestination = rdr["BoxDestination"].ToString();
                        pumpInfoDataList.Add(pumpInfoData);
                    }
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error occured while getting pump information data - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return pumpInfoDataList;
        }

        internal static void UpdateCustomerInformation(CustomerInfoEntity custInfoDataRow, out bool isUpdated)
        {
            isUpdated = false;
            SqlConnection sqlConn = null;
            string query = @"if exists(select * from customerinformation where customerid = @CustomerId) begin update customerinformation set customername = @CustomerName, address1 = @Address, place = @Place, state = @State, country = @Country, pin = @Pin, phone = @Phone, email = @Email, contactperson = @ContactPerson where customerid = @CustomerId end else begin insert into customerinformation(customerid, customername, address1, place, state, country, pin, phone, email, contactperson) values(@CustomerId, @CustomerName, @Address, @Place, @State, @Country, @Pin, @Phone, @Email, @ContactPerson) end";
            try
            {
                if (custInfoDataRow != null)
                {
                    sqlConn = ConnectionManager.GetConnection();
                    SqlCommand cmd = new SqlCommand(query, sqlConn);
                    cmd.Parameters.AddWithValue("@CustomerId", custInfoDataRow.CustomerID);
                    cmd.Parameters.AddWithValue("@CustomerName", custInfoDataRow.CustomerName);
                    if (!string.IsNullOrEmpty(custInfoDataRow.Address))
                        cmd.Parameters.AddWithValue("@Address", custInfoDataRow.Address);
                    else
                        cmd.Parameters.AddWithValue("@Address", System.DBNull.Value);
                    if (!string.IsNullOrEmpty(custInfoDataRow.Place))
                        cmd.Parameters.AddWithValue("@Place", custInfoDataRow.Place);
                    else
                        cmd.Parameters.AddWithValue("@Place", System.DBNull.Value);
                    if (!string.IsNullOrEmpty(custInfoDataRow.State))
                        cmd.Parameters.AddWithValue("@State", custInfoDataRow.State);
                    else
                        cmd.Parameters.AddWithValue("@State", System.DBNull.Value);
                    if (!string.IsNullOrEmpty(custInfoDataRow.Country))
                        cmd.Parameters.AddWithValue("@Country", custInfoDataRow.Country);
                    else
                        cmd.Parameters.AddWithValue("@Country", System.DBNull.Value);
                    if (!string.IsNullOrEmpty(custInfoDataRow.Pin))
                        cmd.Parameters.AddWithValue("@Pin", custInfoDataRow.Pin);
                    else
                        cmd.Parameters.AddWithValue("@Pin", System.DBNull.Value);
                    if (!string.IsNullOrEmpty(custInfoDataRow.Phone))
                        cmd.Parameters.AddWithValue("@Phone", custInfoDataRow.Phone);
                    else
                        cmd.Parameters.AddWithValue("@Phone", System.DBNull.Value);
                    if (!string.IsNullOrEmpty(custInfoDataRow.Email))
                        cmd.Parameters.AddWithValue("@Email", custInfoDataRow.Email);
                    else
                        cmd.Parameters.AddWithValue("@Email", System.DBNull.Value);
                    if (!string.IsNullOrEmpty(custInfoDataRow.ContactPerson))
                        cmd.Parameters.AddWithValue("@ContactPerson", custInfoDataRow.ContactPerson);
                    else
                        cmd.Parameters.AddWithValue("@ContactPerson", System.DBNull.Value);
                    cmd.CommandType = CommandType.Text;
                    int rows_affected = cmd.ExecuteNonQuery();
                    if (rows_affected > 0)
                        isUpdated = true;
                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
                Logger.WriteErrorLog("Error in updating Data in [dbo].[customerinformation] - " + ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
        }

        internal static void DeleteCustomerInformation(string customerID, out bool isDeleted)
        {
            isDeleted = false;
            SqlConnection conn = null;
            string query = @"Delete from [dbo].[customerinformation] where customerid = @CustomerID";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@CustomerID", customerID);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    isDeleted = true;
                }
            }
            catch (Exception ex)
            {
                isDeleted = false;
                Logger.WriteErrorLog("Error in deleting data from [dbo].[customerinformation] - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        internal static void UpdatePumpInformation(PumpInfoEntity pumpInfoDataRow, out bool isUpdated)
        {
            isUpdated = false;
            SqlConnection sqlConn = null;
            string query = @"if exists(select * from PumpMasterData where PumpModel = @PumpModel) begin update PumpMasterData set CustomerModel = @CustomerModel, CustomerName = @CustomerName, SalesUnit = @SalesUnit, PackagingType = @PackagingType, PackingBoxNumber = @PackingBoxNumber, PerBoxPumpQty = @PerBoxPumpQty, PumpType = @PumpType, BoxDestination = @BoxDestination where PumpModel = @PumpModel end else begin insert into PumpMasterData(PumpModel, CustomerModel, CustomerName, SalesUnit, PackagingType, PackingBoxNumber, PerBoxPumpQty, PumpType, BoxDestination) values(@PumpModel, @CustomerModel, @CustomerName, @SalesUnit, @PackagingType, @PackingBoxNumber, @PerBoxPumpQty, @PumpType, @BoxDestination) end";
            try
            {
                if (pumpInfoDataRow != null)
                {
                    sqlConn = ConnectionManager.GetConnection();
                    SqlCommand cmd = new SqlCommand(query, sqlConn);
                    cmd.Parameters.AddWithValue("@PumpModel", string.IsNullOrEmpty(pumpInfoDataRow.PumpModel) ? "" : pumpInfoDataRow.PumpModel);
                    cmd.Parameters.AddWithValue("@CustomerModel", string.IsNullOrEmpty(pumpInfoDataRow.CustomerModel) ? "" : pumpInfoDataRow.CustomerModel);
                    cmd.Parameters.AddWithValue("@CustomerName", string.IsNullOrEmpty(pumpInfoDataRow.CustomerName) ? "" : pumpInfoDataRow.CustomerName);
                    cmd.Parameters.AddWithValue("@SalesUnit", string.IsNullOrEmpty(pumpInfoDataRow.SalesUnit) ? "" : pumpInfoDataRow.SalesUnit);
                    cmd.Parameters.AddWithValue("@PackagingType", string.IsNullOrEmpty(pumpInfoDataRow.PackagingType) ? "" : pumpInfoDataRow.PackagingType);
                    cmd.Parameters.AddWithValue("@PackingBoxNumber", string.IsNullOrEmpty(pumpInfoDataRow.PackingBoxNumber) ? "" : pumpInfoDataRow.PackingBoxNumber);
                    cmd.Parameters.AddWithValue("@PerBoxPumpQty", pumpInfoDataRow.PerBoxPumpQty);
                    cmd.Parameters.AddWithValue("@PumpType", string.IsNullOrEmpty(pumpInfoDataRow.PumpType) ? "" : pumpInfoDataRow.PumpType);
                    cmd.Parameters.AddWithValue("@BoxDestination", string.IsNullOrEmpty(pumpInfoDataRow.BoxDestination) ? "" : pumpInfoDataRow.BoxDestination);
                    cmd.CommandType = CommandType.Text;
                    int rows_affected = cmd.ExecuteNonQuery();
                    if (rows_affected > 0)
                        isUpdated = true;
                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
                Logger.WriteErrorLog("Error in updating data in [dbo].[PumpMasterData] - " + ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
        }

        internal static void DeletePumpInformation(string pumpModel, string pckType, out bool isDeleted)
        {
            isDeleted = false;
            SqlConnection conn = null;
            string query = @"Delete from PumpMasterData where PumpModel = @PumpModel and PackagingType = @PackagingType";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PumpModel", pumpModel);
                cmd.Parameters.AddWithValue("@PackagingType", pckType);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    isDeleted = true;
                }
            }
            catch (Exception ex)
            {
                isDeleted = false;
                Logger.WriteErrorLog("Error in deleting data from [dbo].[PumpMasterData] - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        internal static void DeleteScannedPumpDetails(RunningModelStatusEntity runningModelStatusEntity)
        {
            SqlConnection conn = null;
            string query = $"delete from ScannedDetails where IDD in(select top {runningModelStatusEntity.ScannedQuantity} IDD from ScannedDetails where PumpModel = @PumpModel and PackagingType = @PackagingType order by ScannedTS desc)";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PumpModel", runningModelStatusEntity.RunningPumpModel);
                cmd.Parameters.AddWithValue("@PackagingType", runningModelStatusEntity.PackagingType);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error in deleting data from [dbo].[ScannedDetails] - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        internal static DataTable GetAllTargetMasterData(DateTime selectedDate)
        {
            DataTable dtTargetMasterData = new DataTable();
            SqlConnection sqlConnection = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                sqlConnection = ConnectionManager.GetConnection();
                SqlCommand command = new SqlCommand(@"s_GetModelwiseTarget_Mivin", sqlConnection);
                command.Parameters.AddWithValue("@StartDate", selectedDate);
                command.CommandType = CommandType.StoredProcedure;
                sqlDataReader = command.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    dtTargetMasterData.Load(sqlDataReader);
                    foreach (DataColumn dataColumn in dtTargetMasterData.Columns)
                        dataColumn.ReadOnly = false;
                    dtTargetMasterData.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error in fetching data from [dbo].[s_GetModelwiseTarget_Mivin] - \n" + ex.ToString());
            }
            finally
            {
                if (sqlDataReader != null) sqlDataReader.Close();
                if (sqlConnection != null) sqlConnection.Close();
            }
            return dtTargetMasterData;
        }

        internal static void UpdatePumpModelTargetInfo(string date, string shift, string pumpModel, string target, out bool isUpdated)
        {
            isUpdated = false;
            SqlConnection conn = null;
            string query = @"if exists(select * from PumpwiseTargetMaster where convert(nvarchar(10), [Date], 120) = convert(nvarchar(10), @Date, 120) and Shift = @Shift and PumpModel = @PumpModel) begin update PumpwiseTargetMaster set Target = @Target where convert(nvarchar(10), [Date], 120) = convert(nvarchar(10), @Date, 120) and Shift = @Shift and PumpModel = @PumpModel end else begin insert into PumpwiseTargetMaster([Date], Shift, PumpModel, Target) values(convert(nvarchar(10), @Date, 120), @Shift, @PumpModel, @Target) end";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Date", date);
                command.Parameters.AddWithValue("@Shift", shift);
                command.Parameters.AddWithValue("@PumpModel", pumpModel);
                command.Parameters.AddWithValue("@Target", target);
                command.CommandType = CommandType.Text;
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                    isUpdated = true;
            }
            catch (Exception ex)
            {
                isUpdated = false;
                Logger.WriteErrorLog("Error updating data in [dbo].[PumpwiseTargetMaster] - \n" + ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        internal static ObservableCollection<RunningShiftSchedulesEntity> GetRunningShiftSchedulesData(out ObservableCollection<RunningModelStatusEntity> runningModelDetails, out ShiftWiseEnergyInfo shiftWiseEnergy)
        {
            double val = 0.0; int value = 0;
            shiftWiseEnergy = new ShiftWiseEnergyInfo();
            ObservableCollection<RunningShiftSchedulesEntity> runningShiftSchedulesDetails = new ObservableCollection<RunningShiftSchedulesEntity>();
            runningModelDetails = new ObservableCollection<RunningModelStatusEntity>();
            RunningModelStatusEntity runningModelStatus = null;
            RunningShiftSchedulesEntity runningShiftSchedule = null;
            SqlConnection conn = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(@"s_GetAndonDetails_Mivin", conn);
                sqlCommand.Parameters.AddWithValue("@Starttime", DateTime.Now);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        runningModelStatus = new RunningModelStatusEntity();
                        runningModelStatus.ScheduleDate = Convert.ToDateTime(sqlDataReader["Sdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                        runningModelStatus.WorkOrderNumber = sqlDataReader["WONumber"].ToString();
                        runningModelStatus.RunningPumpModel = sqlDataReader["PumpModel"].ToString();
                        runningModelStatus.CustomerModel = sqlDataReader["CustomerModel"].ToString();
                        runningModelStatus.QuantityPerBox = (int)sqlDataReader["QtyPerbox"];
                        runningModelStatus.ScannedQuantity = (int)sqlDataReader["ScannedQty"];
                        runningModelStatus.Operator = sqlDataReader["operator"].ToString();
                        runningModelStatus.LastScannedSerialNum = sqlDataReader["LastScannedSlno"].ToString();
                        runningModelStatus.WorkOrderStatus = sqlDataReader["WorkOrderStatus"].ToString();
                        runningModelStatus.TotalScannedQuantity = sqlDataReader["TotalScannedQty"] != System.DBNull.Value ? (int)sqlDataReader["TotalScannedQty"] : 0;
                        runningModelStatus.IsHoldEnabled = Utility.InstalledPCType.Equals("Client") ? false : ((Utility.CurrentEmployeeRole.Equals("Supervisor") || Utility.CurrentEmployeeRole.Equals("Admin")) ? true : false);
                        runningModelDetails.Add(runningModelStatus);
                    }
                }

                if (sqlDataReader.NextResult())
                {
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            runningShiftSchedule = new RunningShiftSchedulesEntity();
                            runningShiftSchedule.ScheduleDate = Convert.ToDateTime(sqlDataReader["Sdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                            runningShiftSchedule.WorkOrderNumber = sqlDataReader["WONumber"].ToString();
                            runningShiftSchedule.Customer = sqlDataReader["Customer"].ToString();
                            runningShiftSchedule.PumpModel = sqlDataReader["PumpModel"].ToString();
                            runningShiftSchedule.CustomerModel = sqlDataReader["CustomerModel"].ToString();
                            runningShiftSchedule.PackingTarget = (int)sqlDataReader["PackingTarget"];
                            runningShiftSchedule.QuantityPerBox = (int)sqlDataReader["QtyPerbox"];
                            runningShiftSchedule.TotalScannedQuantity = sqlDataReader["TotalScannedQty"] != System.DBNull.Value ? (int)sqlDataReader["TotalScannedQty"] : 0;
                            runningShiftSchedule.PackedBoxCount = sqlDataReader["PackedBoxCount"] != System.DBNull.Value ? (int)sqlDataReader["PackedBoxCount"] : 0;
                            runningShiftSchedule.PackagingType = sqlDataReader["PackagingType"].ToString();
                            runningShiftSchedule.WorkOrderStatus = sqlDataReader["WorkOrderStatus"].ToString();
                            runningShiftSchedule.Priority = sqlDataReader["Priority"] != System.DBNull.Value ? (int)sqlDataReader["Priority"] : 0;
                            runningShiftSchedule.IsSetButtonEnabled = Utility.InstalledPCType.Equals("Client") ? false : true;
                            runningShiftSchedule.PumpInfo = runningShiftSchedule.PumpModel + " - " + runningShiftSchedule.PackagingType + " - " + runningShiftSchedule.ScheduleDate;
                            runningShiftSchedulesDetails.Add(runningShiftSchedule);
                        }
                    }
                }

                if (sqlDataReader.NextResult())
                {
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            if (sqlDataReader["shifttarget"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftTarget = int.TryParse(sqlDataReader["shifttarget"].ToString(), out value) ? value : 0;
                            else
                                shiftWiseEnergy.ShiftTarget = 0;
                            if (sqlDataReader["Shiftactual"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftActual = int.TryParse(sqlDataReader["Shiftactual"].ToString(), out value) ? value : 0;
                            else
                                shiftWiseEnergy.ShiftActual = 0;
                            if (sqlDataReader["Efficiency"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftEfficiency = double.TryParse(sqlDataReader["Efficiency"].ToString(), out val) ? Math.Round(val, 2) : 0;
                            else
                                shiftWiseEnergy.ShiftEfficiency = 0;
                            shiftWiseEnergy.ShiftwiseTarget = "Shift Target - " + shiftWiseEnergy.ShiftTarget;
                            shiftWiseEnergy.ShiftwiseActual = "Shift Actual - " + shiftWiseEnergy.ShiftActual;
                        }
                    }
                    else
                    {
                        shiftWiseEnergy.ShiftTarget = 0;
                        shiftWiseEnergy.ShiftActual = 0;
                        shiftWiseEnergy.ShiftEfficiency = 0;
                        shiftWiseEnergy.ShiftwiseTarget = "Shift Target - " + shiftWiseEnergy.ShiftTarget;
                        shiftWiseEnergy.ShiftwiseActual = "Shift Actual - " + shiftWiseEnergy.ShiftActual;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error fetching running shift schedules data from procedure [dbo].[s_GetAndonDetails_Mivin] - \n" + ex.ToString());
            }
            finally
            {
                if (sqlDataReader != null) sqlDataReader.Close();
                if (conn != null) conn.Close();
            }
            return runningShiftSchedulesDetails;
        }

        internal static ObservableCollection<RunningShiftSchedulesEntity> GetRunningShiftSchedulesData(string PumpModel, out ObservableCollection<RunningModelStatusEntity> runningModelDetails, out ShiftWiseEnergyInfo shiftWiseEnergy, out RunningPumpModelEntity runningPumpModelData)
        {
            double val = 0.0; int value = 0;
            shiftWiseEnergy = new ShiftWiseEnergyInfo();
            runningPumpModelData = new RunningPumpModelEntity();
            ObservableCollection<RunningShiftSchedulesEntity> runningShiftSchedulesDetails = new ObservableCollection<RunningShiftSchedulesEntity>();
            runningModelDetails = new ObservableCollection<RunningModelStatusEntity>();
            RunningModelStatusEntity runningModelStatus = null;
            RunningShiftSchedulesEntity runningShiftSchedule = null;
            SqlConnection conn = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(@"s_GetShiftwiseAndonDetails_Mivin", conn);
                sqlCommand.Parameters.AddWithValue("@Starttime", DateTime.Now);
                sqlCommand.Parameters.AddWithValue("@PumpModel", !string.IsNullOrEmpty(PumpModel) ? PumpModel : "");
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        runningModelStatus = new RunningModelStatusEntity();
                        runningModelStatus.ScheduleDate = Convert.ToDateTime(sqlDataReader["Sdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                        runningModelStatus.WorkOrderNumber = sqlDataReader["WONumber"].ToString();
                        runningModelStatus.RunningPumpModel = sqlDataReader["PumpModel"].ToString();
                        runningModelStatus.CustomerModel = sqlDataReader["CustomerModel"].ToString();
                        runningModelStatus.QuantityPerBox = (int)sqlDataReader["QtyPerbox"];
                        runningModelStatus.ScannedQuantity = (int)sqlDataReader["ScannedQty"];
                        runningModelStatus.PackagingType = sqlDataReader["PackagingType"].ToString();
                        runningModelStatus.Operator = sqlDataReader["operator"].ToString();
                        runningModelStatus.LastScannedSerialNum = sqlDataReader["LastScannedSlno"].ToString();
                        runningModelStatus.WorkOrderStatus = sqlDataReader["WorkOrderStatus"].ToString();
                        runningModelStatus.TotalScannedQuantity = sqlDataReader["TotalScannedQty"] != System.DBNull.Value ? (int)sqlDataReader["TotalScannedQty"] : 0;
                        runningModelStatus.IsHoldEnabled = Utility.InstalledPCType.Equals("Client") ? false : ((Utility.CurrentEmployeeRole.Equals("Operator")) ? true : false);
                        runningModelDetails.Add(runningModelStatus);
                    }
                }

                if (sqlDataReader.NextResult())
                {
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            runningShiftSchedule = new RunningShiftSchedulesEntity();
                            runningShiftSchedule.ScheduleDate = Convert.ToDateTime(sqlDataReader["Sdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                            runningShiftSchedule.WorkOrderNumber = sqlDataReader["WONumber"].ToString();
                            runningShiftSchedule.Customer = sqlDataReader["Customer"].ToString();
                            runningShiftSchedule.PumpModel = sqlDataReader["PumpModel"].ToString();
                            runningShiftSchedule.CustomerModel = sqlDataReader["CustomerModel"].ToString();
                            runningShiftSchedule.PackingTarget = (int)sqlDataReader["PackingTarget"];
                            runningShiftSchedule.QuantityPerBox = (int)sqlDataReader["QtyPerbox"];
                            runningShiftSchedule.TotalScannedQuantity = sqlDataReader["TotalScannedQty"] != System.DBNull.Value ? (int)sqlDataReader["TotalScannedQty"] : 0;
                            runningShiftSchedule.PackedBoxCount = sqlDataReader["PackedBoxCount"] != System.DBNull.Value ? (int)sqlDataReader["PackedBoxCount"] : 0;
                            runningShiftSchedule.PackagingType = sqlDataReader["PackagingType"].ToString();
                            runningShiftSchedule.WorkOrderStatus = sqlDataReader["WorkOrderStatus"].ToString();
                            runningShiftSchedule.Priority = sqlDataReader["Priority"] != System.DBNull.Value ? (int)sqlDataReader["Priority"] : 0;
                            runningShiftSchedule.IsSetButtonEnabled = Utility.InstalledPCType.Equals("Client") ? false : true;
                            runningShiftSchedule.PumpInfo = runningShiftSchedule.PumpModel + " - " + runningShiftSchedule.PackagingType + " - " + runningShiftSchedule.ScheduleDate;
                            runningShiftSchedulesDetails.Add(runningShiftSchedule);
                        }
                    }
                }

                if (sqlDataReader.NextResult())
                {
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            if (sqlDataReader["shifttarget"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftTarget = int.TryParse(sqlDataReader["shifttarget"].ToString(), out value) ? value : 0;
                            else
                                shiftWiseEnergy.ShiftTarget = 0;
                            if (sqlDataReader["Shiftactual"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftActual = int.TryParse(sqlDataReader["Shiftactual"].ToString(), out value) ? value : 0;
                            else
                                shiftWiseEnergy.ShiftActual = 0;
                            if (sqlDataReader["Efficiency"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftEfficiency = double.TryParse(sqlDataReader["Efficiency"].ToString(), out val) ? Math.Round(val, 2) : 0;
                            else
                                shiftWiseEnergy.ShiftEfficiency = 0;
                            shiftWiseEnergy.ShiftwiseTarget = "Shift Target - " + shiftWiseEnergy.ShiftTarget;
                            shiftWiseEnergy.ShiftwiseActual = "Shift Actual - " + shiftWiseEnergy.ShiftActual;
                        }
                    }
                    else
                    {
                        shiftWiseEnergy.ShiftTarget = 0;
                        shiftWiseEnergy.ShiftActual = 0;
                        shiftWiseEnergy.ShiftEfficiency = 0;
                        shiftWiseEnergy.ShiftwiseTarget = "Shift Target - " + shiftWiseEnergy.ShiftTarget;
                        shiftWiseEnergy.ShiftwiseActual = "Shift Actual - " + shiftWiseEnergy.ShiftActual;
                    }
                }

                if (sqlDataReader.NextResult())
                {
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            runningPumpModelData.ScheduleDate = Convert.ToDateTime(sqlDataReader["Sdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                            runningPumpModelData.RunningPumpModel = sqlDataReader["Pumpmodel"].ToString();
                            runningPumpModelData.PackagingType = sqlDataReader["PackagingType"].ToString();
                            runningPumpModelData.MonthlyTarget = int.TryParse(sqlDataReader["MonthlyTotalTarget"].ToString(), out value) ? value : 0;
                            runningPumpModelData.MonthlyActual = int.TryParse(sqlDataReader["MonthlyTotalScannedQty"].ToString(), out value) ? value : 0;
                            runningPumpModelData.MonthlyPending = int.TryParse(sqlDataReader["MonthlyPendingQty"].ToString(), out value) ? value : 0;
                            runningPumpModelData.PackedBoxCount = int.TryParse(sqlDataReader["DailyNoOfPackedBoxes"].ToString(), out value) ? value : 0;
                            runningPumpModelData.DailyTarget = int.TryParse(sqlDataReader["DailyTotalTarget"].ToString(), out value) ? value : 0;
                            runningPumpModelData.DailyActual = int.TryParse(sqlDataReader["DailyActual"].ToString(), out value) ? value : 0;
                            runningPumpModelData.DailyPending = int.TryParse(sqlDataReader["DailyPendingQty"].ToString(), out value) ? value : 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error fetching running shift schedules data from procedure [dbo].[s_GetShiftwiseAndonDetails_Mivin] - \n" + ex.ToString());
            }
            finally
            {
                if (sqlDataReader != null) sqlDataReader.Close();
                if (conn != null) conn.Close();
            }
            return runningShiftSchedulesDetails;
        }

        internal static ObservableCollection<RunningShiftSchedulesEntity> GetRunningShiftSchedulesData(string stationID, string PumpModel, out ObservableCollection<RunningModelStatusEntity> runningModelDetails, out ShiftWiseEnergyInfo shiftWiseEnergy, out RunningPumpModelEntity runningPumpModelData)
        {
            double val = 0.0; int value = 0;
            shiftWiseEnergy = new ShiftWiseEnergyInfo();
            runningPumpModelData = new RunningPumpModelEntity();
            ObservableCollection<RunningShiftSchedulesEntity> runningShiftSchedulesDetails = new ObservableCollection<RunningShiftSchedulesEntity>();
            runningModelDetails = new ObservableCollection<RunningModelStatusEntity>();
            RunningModelStatusEntity runningModelStatus = null;
            RunningShiftSchedulesEntity runningShiftSchedule = null;
            SqlConnection conn = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(@"s_GetShiftwiseAndonDetails_Mivin", conn);
                sqlCommand.Parameters.AddWithValue("@StationID", stationID);
                sqlCommand.Parameters.AddWithValue("@Starttime", DateTime.Now);
                sqlCommand.Parameters.AddWithValue("@PumpModel", !string.IsNullOrEmpty(PumpModel) ? PumpModel : "");
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        runningModelStatus = new RunningModelStatusEntity();
                        runningModelStatus.ScheduleDate = Convert.ToDateTime(sqlDataReader["Sdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                        runningModelStatus.StationID = sqlDataReader["StationID"].ToString();
                        runningModelStatus.WorkOrderNumber = sqlDataReader["WONumber"].ToString();
                        runningModelStatus.RunningPumpModel = sqlDataReader["PumpModel"].ToString();
                        runningModelStatus.CustomerModel = sqlDataReader["CustomerModel"].ToString();
                        runningModelStatus.QuantityPerBox = (int)sqlDataReader["QtyPerbox"];
                        runningModelStatus.ScannedQuantity = (int)sqlDataReader["ScannedQty"];
                        runningModelStatus.PackagingType = sqlDataReader["PackagingType"].ToString();
                        runningModelStatus.Operator = sqlDataReader["operator"].ToString();
                        runningModelStatus.LastScannedSerialNum = sqlDataReader["LastScannedSlno"].ToString();
                        runningModelStatus.WorkOrderStatus = sqlDataReader["WorkOrderStatus"].ToString();
                        runningModelStatus.TotalScannedQuantity = sqlDataReader["TotalScannedQty"] != System.DBNull.Value ? (int)sqlDataReader["TotalScannedQty"] : 0;
                        runningModelStatus.IsHoldEnabled = Utility.InstalledPCType.Equals("Client") ? false : ((Utility.CurrentEmployeeRole.Equals("Operator")) ? true : false);
                        runningModelDetails.Add(runningModelStatus);
                    }
                }

                if (sqlDataReader.NextResult())
                {
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            runningShiftSchedule = new RunningShiftSchedulesEntity();
                            runningShiftSchedule.ScheduleDate = Convert.ToDateTime(sqlDataReader["Sdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                            runningShiftSchedule.StationID = sqlDataReader["StationID"].ToString();
                            runningShiftSchedule.WorkOrderNumber = sqlDataReader["WONumber"].ToString();
                            runningShiftSchedule.Customer = sqlDataReader["Customer"].ToString();
                            runningShiftSchedule.PumpModel = sqlDataReader["PumpModel"].ToString();
                            runningShiftSchedule.CustomerModel = sqlDataReader["CustomerModel"].ToString();
                            runningShiftSchedule.PackingTarget = (int)sqlDataReader["PackingTarget"];
                            runningShiftSchedule.QuantityPerBox = (int)sqlDataReader["QtyPerbox"];
                            runningShiftSchedule.TotalScannedQuantity = sqlDataReader["TotalScannedQty"] != System.DBNull.Value ? (int)sqlDataReader["TotalScannedQty"] : 0;
                            runningShiftSchedule.PackedBoxCount = sqlDataReader["PackedBoxCount"] != System.DBNull.Value ? (int)sqlDataReader["PackedBoxCount"] : 0;
                            runningShiftSchedule.PackagingType = sqlDataReader["PackagingType"].ToString();
                            runningShiftSchedule.WorkOrderStatus = sqlDataReader["WorkOrderStatus"].ToString();
                            runningShiftSchedule.Priority = sqlDataReader["Priority"] != System.DBNull.Value ? (int)sqlDataReader["Priority"] : 0;
                            runningShiftSchedule.IsSetButtonEnabled = Utility.InstalledPCType.Equals("Client") ? false : true;
                            runningShiftSchedule.PumpInfo = runningShiftSchedule.PumpModel + " - " + runningShiftSchedule.PackagingType + " - " + runningShiftSchedule.ScheduleDate;
                            runningShiftSchedulesDetails.Add(runningShiftSchedule);
                        }
                    }
                }

                if (sqlDataReader.NextResult())
                {
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            if (sqlDataReader["shifttarget"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftTarget = int.TryParse(sqlDataReader["shifttarget"].ToString(), out value) ? value : 0;
                            else
                                shiftWiseEnergy.ShiftTarget = 0;
                            if (sqlDataReader["Shiftactual"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftActual = int.TryParse(sqlDataReader["Shiftactual"].ToString(), out value) ? value : 0;
                            else
                                shiftWiseEnergy.ShiftActual = 0;
                            if (sqlDataReader["Efficiency"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftEfficiency = double.TryParse(sqlDataReader["Efficiency"].ToString(), out val) ? Math.Round(val, 2) : 0;
                            else
                                shiftWiseEnergy.ShiftEfficiency = 0;
                            shiftWiseEnergy.ShiftwiseTarget = "Shift Target - " + shiftWiseEnergy.ShiftTarget;
                            shiftWiseEnergy.ShiftwiseActual = "Shift Actual - " + shiftWiseEnergy.ShiftActual;
                        }
                    }
                    else
                    {
                        shiftWiseEnergy.ShiftTarget = 0;
                        shiftWiseEnergy.ShiftActual = 0;
                        shiftWiseEnergy.ShiftEfficiency = 0;
                        shiftWiseEnergy.ShiftwiseTarget = "Shift Target - " + shiftWiseEnergy.ShiftTarget;
                        shiftWiseEnergy.ShiftwiseActual = "Shift Actual - " + shiftWiseEnergy.ShiftActual;
                    }
                }

                if (sqlDataReader.NextResult())
                {
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            runningPumpModelData.ScheduleDate = Convert.ToDateTime(sqlDataReader["Sdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                            runningPumpModelData.StationID = sqlDataReader["StationID"].ToString();
                            runningPumpModelData.RunningPumpModel = sqlDataReader["Pumpmodel"].ToString();
                            runningPumpModelData.PackagingType = sqlDataReader["PackagingType"].ToString();
                            runningPumpModelData.MonthlyTarget = int.TryParse(sqlDataReader["MonthlyTotalTarget"].ToString(), out value) ? value : 0;
                            runningPumpModelData.MonthlyActual = int.TryParse(sqlDataReader["MonthlyTotalScannedQty"].ToString(), out value) ? value : 0;
                            runningPumpModelData.MonthlyPending = int.TryParse(sqlDataReader["MonthlyPendingQty"].ToString(), out value) ? value : 0;
                            runningPumpModelData.PackedBoxCount = int.TryParse(sqlDataReader["DailyNoOfPackedBoxes"].ToString(), out value) ? value : 0;
                            runningPumpModelData.DailyTarget = int.TryParse(sqlDataReader["DailyTotalTarget"].ToString(), out value) ? value : 0;
                            runningPumpModelData.DailyActual = int.TryParse(sqlDataReader["DailyActual"].ToString(), out value) ? value : 0;
                            runningPumpModelData.DailyPending = int.TryParse(sqlDataReader["DailyPendingQty"].ToString(), out value) ? value : 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error fetching running shift schedules data from procedure [dbo].[s_GetShiftwiseAndonDetails_Mivin] - \n" + ex.ToString());
            }
            finally
            {
                if (sqlDataReader != null) sqlDataReader.Close();
                if (conn != null) conn.Close();
            }
            return runningShiftSchedulesDetails;
        }

        internal static ObservableCollection<RunningShiftSchedulesEntity> GetRunningShiftSchedulesData(string stationID, string PumpModel, out ObservableCollection<RunningModelStatusEntity> runningModelDetails, out ShiftWiseEnergyInfo shiftWiseEnergy, out ObservableCollection<RunningPumpModelEntity> runningPumpModelData)
        {
            double val = 0.0; int value = 0;
            shiftWiseEnergy = new ShiftWiseEnergyInfo();
            runningPumpModelData = new ObservableCollection<RunningPumpModelEntity>();
            ObservableCollection<RunningShiftSchedulesEntity> runningShiftSchedulesDetails = new ObservableCollection<RunningShiftSchedulesEntity>();
            runningModelDetails = new ObservableCollection<RunningModelStatusEntity>();
            RunningModelStatusEntity runningModelStatus = null;
            RunningShiftSchedulesEntity runningShiftSchedule = null;
            SqlConnection conn = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(@"s_GetShiftwiseAndonDetails_Mivin", conn);
                sqlCommand.Parameters.AddWithValue("@StationID", stationID);
                sqlCommand.Parameters.AddWithValue("@Starttime", DateTime.Now);
                sqlCommand.Parameters.AddWithValue("@PumpModel", !string.IsNullOrEmpty(PumpModel) ? PumpModel : "");
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        runningModelStatus = new RunningModelStatusEntity();
                        runningModelStatus.ScheduleDate = Convert.ToDateTime(sqlDataReader["Sdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                        runningModelStatus.StationID = sqlDataReader["StationID"].ToString();
                        runningModelStatus.WorkOrderNumber = sqlDataReader["WONumber"].ToString();
                        runningModelStatus.RunningPumpModel = sqlDataReader["PumpModel"].ToString();
                        runningModelStatus.CustomerModel = sqlDataReader["CustomerModel"].ToString();
                        runningModelStatus.QuantityPerBox = (int)sqlDataReader["QtyPerbox"];
                        runningModelStatus.ScannedQuantity = (int)sqlDataReader["ScannedQty"];
                        runningModelStatus.PackagingType = sqlDataReader["PackagingType"].ToString();
                        runningModelStatus.Operator = sqlDataReader["operator"].ToString();
                        runningModelStatus.LastScannedSerialNum = sqlDataReader["LastScannedSlno"].ToString();
                        runningModelStatus.WorkOrderStatus = sqlDataReader["WorkOrderStatus"].ToString();
                        runningModelStatus.TotalScannedQuantity = sqlDataReader["TotalScannedQty"] != System.DBNull.Value ? (int)sqlDataReader["TotalScannedQty"] : 0;
                        runningModelStatus.IsHoldEnabled = Utility.InstalledPCType.Equals("Client") ? false : ((Utility.CurrentEmployeeRole.Equals("Operator")) ? true : false);
                        runningModelDetails.Add(runningModelStatus);
                    }
                }

                if (sqlDataReader.NextResult())
                {
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            runningShiftSchedule = new RunningShiftSchedulesEntity();
                            runningShiftSchedule.ScheduleDate = Convert.ToDateTime(sqlDataReader["Sdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                            runningShiftSchedule.StationID = sqlDataReader["StationID"].ToString();
                            runningShiftSchedule.WorkOrderNumber = sqlDataReader["WONumber"].ToString();
                            runningShiftSchedule.Customer = sqlDataReader["Customer"].ToString();
                            runningShiftSchedule.PumpModel = sqlDataReader["PumpModel"].ToString();
                            runningShiftSchedule.CustomerModel = sqlDataReader["CustomerModel"].ToString();
                            runningShiftSchedule.PackingTarget = (int)sqlDataReader["PackingTarget"];
                            runningShiftSchedule.QuantityPerBox = (int)sqlDataReader["QtyPerbox"];
                            runningShiftSchedule.TotalScannedQuantity = sqlDataReader["TotalScannedQty"] != System.DBNull.Value ? (int)sqlDataReader["TotalScannedQty"] : 0;
                            runningShiftSchedule.PackedBoxCount = sqlDataReader["PackedBoxCount"] != System.DBNull.Value ? (int)sqlDataReader["PackedBoxCount"] : 0;
                            runningShiftSchedule.PackagingType = sqlDataReader["PackagingType"].ToString();
                            runningShiftSchedule.WorkOrderStatus = sqlDataReader["WorkOrderStatus"].ToString();
                            runningShiftSchedule.Priority = sqlDataReader["Priority"] != System.DBNull.Value ? (int)sqlDataReader["Priority"] : 0;
                            runningShiftSchedule.IsSetButtonEnabled = Utility.InstalledPCType.Equals("Client") ? false : true;
                            runningShiftSchedule.PumpInfo = runningShiftSchedule.PumpModel + " - " + runningShiftSchedule.PackagingType + " - " + runningShiftSchedule.ScheduleDate;
                            runningShiftSchedulesDetails.Add(runningShiftSchedule);
                        }
                    }
                }

                if (sqlDataReader.NextResult())
                {
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            if (sqlDataReader["shifttarget"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftTarget = int.TryParse(sqlDataReader["shifttarget"].ToString(), out value) ? value : 0;
                            else
                                shiftWiseEnergy.ShiftTarget = 0;
                            if (sqlDataReader["Shiftactual"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftActual = int.TryParse(sqlDataReader["Shiftactual"].ToString(), out value) ? value : 0;
                            else
                                shiftWiseEnergy.ShiftActual = 0;
                            if (sqlDataReader["Efficiency"] != System.DBNull.Value)
                                shiftWiseEnergy.ShiftEfficiency = double.TryParse(sqlDataReader["Efficiency"].ToString(), out val) ? Math.Round(val, 2) : 0;
                            else
                                shiftWiseEnergy.ShiftEfficiency = 0;
                            shiftWiseEnergy.ShiftwiseTarget = "Shift Target - " + shiftWiseEnergy.ShiftTarget;
                            shiftWiseEnergy.ShiftwiseActual = "Shift Actual - " + shiftWiseEnergy.ShiftActual;
                        }
                    }
                    else
                    {
                        shiftWiseEnergy.ShiftTarget = 0;
                        shiftWiseEnergy.ShiftActual = 0;
                        shiftWiseEnergy.ShiftEfficiency = 0;
                        shiftWiseEnergy.ShiftwiseTarget = "Shift Target - " + shiftWiseEnergy.ShiftTarget;
                        shiftWiseEnergy.ShiftwiseActual = "Shift Actual - " + shiftWiseEnergy.ShiftActual;
                    }
                }

                if (sqlDataReader.NextResult())
                {
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            RunningPumpModelEntity runningPumpModelEntity = new RunningPumpModelEntity();
                            runningPumpModelEntity.ScheduleDate = Convert.ToDateTime(sqlDataReader["Sdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                            runningPumpModelEntity.StationID = sqlDataReader["StationID"].ToString();
                            runningPumpModelEntity.RunningPumpModel = sqlDataReader["Pumpmodel"].ToString();
                            runningPumpModelEntity.PackagingType = sqlDataReader["PackagingType"].ToString();
                            runningPumpModelEntity.MonthlyTarget = int.TryParse(sqlDataReader["MonthlyTotalTarget"].ToString(), out value) ? value : 0;
                            runningPumpModelEntity.MonthlyActual = int.TryParse(sqlDataReader["MonthlyTotalScannedQty"].ToString(), out value) ? value : 0;
                            runningPumpModelEntity.MonthlyPending = int.TryParse(sqlDataReader["MonthlyPendingQty"].ToString(), out value) ? value : 0;
                            runningPumpModelEntity.PackedBoxCount = int.TryParse(sqlDataReader["DailyNoOfPackedBoxes"].ToString(), out value) ? value : 0;
                            runningPumpModelEntity.DailyTarget = int.TryParse(sqlDataReader["DailyTotalTarget"].ToString(), out value) ? value : 0;
                            runningPumpModelEntity.DailyActual = int.TryParse(sqlDataReader["DailyActual"].ToString(), out value) ? value : 0;
                            runningPumpModelEntity.DailyPending = int.TryParse(sqlDataReader["DailyPendingQty"].ToString(), out value) ? value : 0;
                            runningPumpModelData.Add(runningPumpModelEntity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error fetching running shift schedules data from procedure [dbo].[s_GetShiftwiseAndonDetails_Mivin] - \n" + ex.ToString());
            }
            finally
            {
                if (sqlDataReader != null) sqlDataReader.Close();
                if (conn != null) conn.Close();
            }
            return runningShiftSchedulesDetails;
        }

        internal static void SetCurrentRunningSchedule(RunningShiftSchedulesEntity selectedScheduleStatus, out bool success)
        {
            success = false;
            SqlConnection conn = null;
            string query = @"Delete from RunningScheduleDetails
                             Insert into RunningScheduleDetails(WONumber, PumpModel, CustomerModel, ScheduledDate) Values(@WONumber, @PumpModel, @CustomerModel, @ScheduledDate)
                             Update ScheduleDetails set WorkOrderStatus='In Progress' where PumpModel=@PumpModel and WONumber=@WONumber and Date=@ScheduledDate";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@WONumber", selectedScheduleStatus.WorkOrderNumber);
                sqlCommand.Parameters.AddWithValue("@PumpModel", selectedScheduleStatus.PumpModel);
                sqlCommand.Parameters.AddWithValue("@CustomerModel", selectedScheduleStatus.CustomerModel);
                sqlCommand.Parameters.AddWithValue("@ScheduledDate", selectedScheduleStatus.ScheduleDate);
                sqlCommand.CommandType = CommandType.Text;
                int rows_affected = sqlCommand.ExecuteNonQuery();
                if (rows_affected > 0)
                    success = true;
                else
                    success = false;
            }
            catch (Exception ex)
            {
                success = false;
                Logger.WriteErrorLog("Error while setting current running schedule in method SetCurrentRunningSchedule - \n" + ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        internal static void SetNewRunningSchedule(RunningShiftSchedulesEntity selectedScheduleStatus, out bool success)
        {
            success = false;
            SqlConnection conn = null;
            string query = @"Delete from RunningScheduleDetails where StationID = @StationID
                             Insert into RunningScheduleDetails(StationID, WONumber, PumpModel, CustomerModel, ScheduledDate, PackagingType) Values(@StationID, @WONumber, @PumpModel, @CustomerModel, @ScheduledDate, @PackagingType)
                             Update ScheduleDetails set WorkOrderStatus='In Progress' where PumpModel=@PumpModel and PackagingType=@PackagingType and Date=@ScheduledDate";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@StationID", selectedScheduleStatus.StationID);
                sqlCommand.Parameters.AddWithValue("@WONumber", selectedScheduleStatus.WorkOrderNumber);
                sqlCommand.Parameters.AddWithValue("@PumpModel", selectedScheduleStatus.PumpModel);
                sqlCommand.Parameters.AddWithValue("@CustomerModel", selectedScheduleStatus.CustomerModel);
                sqlCommand.Parameters.AddWithValue("@ScheduledDate", selectedScheduleStatus.ScheduleDate);
                sqlCommand.Parameters.AddWithValue("@PackagingType", selectedScheduleStatus.PackagingType);
                sqlCommand.CommandType = CommandType.Text;
                int rows_affected = sqlCommand.ExecuteNonQuery();
                if (rows_affected > 0)
                    success = true;
                else
                    success = false;
            }
            catch (Exception ex)
            {
                success = false;
                Logger.WriteErrorLog("Error while setting current running schedule in method SetNewRunningSchedule - \n" + ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        internal static void SetCurrentRunningSchedule(RunningPumpModelEntity selectedScheduleStatus, out bool success)
        {
            success = false;
            SqlConnection conn = null;
            string query = @"Delete from RunningScheduleDetails
                             Insert into RunningScheduleDetails(WONumber, PumpModel, CustomerModel, ScheduledDate, PackagingType) Values(@WONumber, @PumpModel, @CustomerModel, @ScheduledDate, @PackagingType)
                             Update ScheduleDetails set WorkOrderStatus='In Progress' where PumpModel=@PumpModel and PackagingType=@PackagingType and Date=@ScheduledDate";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@WONumber", selectedScheduleStatus.WorkOrderNumber);
                sqlCommand.Parameters.AddWithValue("@PumpModel", selectedScheduleStatus.RunningPumpModel);
                sqlCommand.Parameters.AddWithValue("@CustomerModel", selectedScheduleStatus.CustomerModel);
                sqlCommand.Parameters.AddWithValue("@ScheduledDate", selectedScheduleStatus.ScheduleDate);
                sqlCommand.Parameters.AddWithValue("@PackagingType", selectedScheduleStatus.PackagingType);
                sqlCommand.CommandType = CommandType.Text;
                int rows_affected = sqlCommand.ExecuteNonQuery();
                if (rows_affected > 0)
                    success = true;
                else
                    success = false;
            }
            catch (Exception ex)
            {
                success = false;
                Logger.WriteErrorLog("Error while setting current running schedule in method SetCurrentRunningSchedule - \n" + ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        internal static bool ValidatePumpSerialNumber(string scheduleDate, string pumpModel, string pumpSerialNum)
        {
            bool IsValidated = false;
            SqlConnection conn = null;
            SqlDataReader sqlDataReader = null;
            string query = @"select * from ScannedDetails where PumpModel=@PumpModel and ScannedSlNo=@ScannedSlNo and ScheduledDate=@ScheduledDate";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@PumpModel", pumpModel);
                sqlCommand.Parameters.AddWithValue("@ScannedSlNo", pumpSerialNum);
                sqlCommand.Parameters.AddWithValue("@ScheduledDate", scheduleDate);
                sqlCommand.CommandType = CommandType.Text;
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                    IsValidated = false;
                else
                    IsValidated = true;
            }
            catch (Exception ex)
            {
                IsValidated = false;
                Logger.WriteErrorLog("Error while validating scanned pump serial number in method ValidatePumpSerialNumber - \n" + ex.ToString());
            }
            finally
            {
                if (sqlDataReader != null) sqlDataReader.Close();
                if (conn != null) conn.Close();
            }
            return IsValidated;
        }

        internal static bool ValidatePumpSerialNumber(string pumpModel, string pumpSerialNum)
        {
            bool IsValidated = false;
            SqlConnection conn = null;
            SqlDataReader sqlDataReader = null;
            string query = @"select * from ScannedDetails where PumpModel=@PumpModel and ScannedSlNo=@ScannedSlNo";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@PumpModel", pumpModel);
                sqlCommand.Parameters.AddWithValue("@ScannedSlNo", pumpSerialNum);
                sqlCommand.CommandType = CommandType.Text;
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                    IsValidated = false;
                else
                    IsValidated = true;
            }
            catch (Exception ex)
            {
                IsValidated = false;
                Logger.WriteErrorLog("Error while validating scanned pump serial number in method ValidatePumpSerialNumber - \n" + ex.ToString());
            }
            finally
            {
                if (sqlDataReader != null) sqlDataReader.Close();
                if (conn != null) conn.Close();
            }
            return IsValidated;
        }

        internal static void UpdateScannedPumpDetails(string stationID, int qtyPerBox, int scannedQty)
        {
            SqlConnection conn = null;
            string query = @"declare @idd as int select @idd=max(idd) from ScannedDetails where StationID=@StationID
                   if @QtyPerBox = @ScannedQty begin Update ScannedDetails set ScannedOrForceCloseStatus=1 where idd=@idd and ScannedOrForceCloseStatus=0 end";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@StationID", stationID);
                sqlCommand.Parameters.AddWithValue("@QtyPerBox", stationID);
                sqlCommand.Parameters.AddWithValue("@ScannedQty", stationID);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while updating scanned pump details in method UpdatePumpScannedDetails - \n" + ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        internal static bool UpdatePumpScannedDetails(RunningModelStatusEntity currentRunningModelData, int scannedQty, string loginUserName, int scanStatus, string pumpSerialNum, string remarks)
        {
            bool IsUpdated = false;
            SqlConnection conn = null;
            CurrentShiftTime currentShiftTime = new CurrentShiftTime();
            string query = @"Insert into ScannedDetails(Date, Shift, WONumber, PumpModel, CustomerModel, ScannedQty, Operator, ScannedSlNo, ScannedOrForceCloseStatus, ScannedTS, Remarks, ScheduledDate) Values(@Date, @Shift, @WONumber, @PumpModel, @CustomerModel, @ScannedQty, @Operator, @ScannedSlNo, @ScannedOrForceCloseStatus, @ScannedTS, @Remarks, @ScheduledDate)";
            try
            {
                currentShiftTime = GetCurrentShiftTime();
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@Date", currentShiftTime.ShiftStartDate.ToString("yyyy-MM-dd HH:mm:ss"));
                sqlCommand.Parameters.AddWithValue("@Shift", currentShiftTime.Shift);
                sqlCommand.Parameters.AddWithValue("@WONumber", currentRunningModelData.WorkOrderNumber);
                sqlCommand.Parameters.AddWithValue("@PumpModel", currentRunningModelData.RunningPumpModel);
                sqlCommand.Parameters.AddWithValue("@CustomerModel", currentRunningModelData.CustomerModel);
                sqlCommand.Parameters.AddWithValue("@ScannedQty", scannedQty);
                sqlCommand.Parameters.AddWithValue("@Operator", loginUserName);
                sqlCommand.Parameters.AddWithValue("@ScannedSlNo", pumpSerialNum);
                sqlCommand.Parameters.AddWithValue("@ScannedOrForceCloseStatus", scanStatus);
                sqlCommand.Parameters.AddWithValue("@ScannedTS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sqlCommand.Parameters.AddWithValue("@Remarks", "");
                sqlCommand.Parameters.AddWithValue("@ScheduledDate", currentRunningModelData.ScheduleDate);
                sqlCommand.CommandType = CommandType.Text;
                int rows_affected = sqlCommand.ExecuteNonQuery();
                if (rows_affected > 0)
                    IsUpdated = true;
                else
                    IsUpdated = false;
            }
            catch (Exception ex)
            {
                IsUpdated = false;
                Logger.WriteErrorLog("Error while updating scanned pump details in method UpdatePumpScannedDetails - \n" + ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return IsUpdated;
        }

        internal static bool UpdateScannedPumpDetails(RunningModelStatusEntity currentRunningModelData, int scannedQty, string loginUserName, int scanStatus, string pumpSerialNum, string remarks)
        {
            bool IsUpdated = false;
            SqlConnection conn = null;
            CurrentShiftTime currentShiftTime = new CurrentShiftTime();
            string query = @"Insert into ScannedDetails(Date, StationID, Shift, WONumber, PumpModel, CustomerModel, ScannedQty, Operator, ScannedSlNo, ScannedOrForceCloseStatus, ScannedTS, Remarks, ScheduledDate, PackagingType) Values(@Date, @StationID, @Shift, @WONumber, @PumpModel, @CustomerModel, @ScannedQty, @Operator, @ScannedSlNo, @ScannedOrForceCloseStatus, @ScannedTS, @Remarks, @ScheduledDate, @PackagingType)";
            try
            {
                currentShiftTime = GetCurrentShiftTime();
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@Date", currentShiftTime.ShiftStartDate.ToString("yyyy-MM-dd HH:mm:ss"));
                sqlCommand.Parameters.AddWithValue("@StationID", currentRunningModelData.StationID);
                sqlCommand.Parameters.AddWithValue("@Shift", currentShiftTime.Shift);
                sqlCommand.Parameters.AddWithValue("@WONumber", currentRunningModelData.WorkOrderNumber);
                sqlCommand.Parameters.AddWithValue("@PumpModel", currentRunningModelData.RunningPumpModel);
                sqlCommand.Parameters.AddWithValue("@CustomerModel", currentRunningModelData.CustomerModel);
                sqlCommand.Parameters.AddWithValue("@ScannedQty", scannedQty);
                sqlCommand.Parameters.AddWithValue("@Operator", loginUserName);
                sqlCommand.Parameters.AddWithValue("@ScannedSlNo", pumpSerialNum);
                sqlCommand.Parameters.AddWithValue("@ScannedOrForceCloseStatus", scanStatus);
                sqlCommand.Parameters.AddWithValue("@ScannedTS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sqlCommand.Parameters.AddWithValue("@Remarks", "");
                sqlCommand.Parameters.AddWithValue("@ScheduledDate", currentRunningModelData.ScheduleDate);
                sqlCommand.Parameters.AddWithValue("@PackagingType", currentRunningModelData.PackagingType);
                sqlCommand.CommandType = CommandType.Text;
                int rows_affected = sqlCommand.ExecuteNonQuery();
                if (rows_affected > 0)
                    IsUpdated = true;
                else
                    IsUpdated = false;
            }
            catch (Exception ex)
            {
                IsUpdated = false;
                Logger.WriteErrorLog("Error while updating scanned pump details in method UpdateScannedPumpDetails - \n" + ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return IsUpdated;
        }

        internal static void DeleteRunningScheduleDetails(string stationId)
        {
            SqlConnection conn = null;
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(@"Delete from RunningScheduleDetails where StationID = @StationID", conn);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddWithValue("@StationID", stationId);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while deleting data from table RunningScheduleDetails in method DeleteRunningScheduleDetails - \n" + ex.ToString());
            }
        }

        internal static bool UpdateWorkOrderStatus(RunningModelStatusEntity runningModelStatusEntity, string status)
        {
            bool IsUpdated = false;
            SqlConnection conn = null;
            string query = @"Update ScheduleDetails set WorkOrderStatus=@WorkOrderStatus where PumpModel=@PumpModel and WONumber=@WONumber and [Date]=@ScheduleDate";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@WONumber", runningModelStatusEntity.WorkOrderNumber);
                sqlCommand.Parameters.AddWithValue("@PumpModel", runningModelStatusEntity.RunningPumpModel);
                sqlCommand.Parameters.AddWithValue("@ScheduleDate", runningModelStatusEntity.ScheduleDate);
                sqlCommand.Parameters.AddWithValue("@WorkOrderStatus", status);
                sqlCommand.CommandType = CommandType.Text;
                int rows_affected = sqlCommand.ExecuteNonQuery();
                if (rows_affected > 0)
                    IsUpdated = true;
                else
                    IsUpdated = false;
            }
            catch (Exception ex)
            {
                IsUpdated = false;
                Logger.WriteErrorLog("Error while updating work order status in method UpdateWorkOrderStatus - \n" + ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return IsUpdated;
        }

        internal static bool UpdateScheduleStatus(RunningModelStatusEntity runningModelStatusEntity, string status)
        {
            bool IsUpdated = false;
            SqlConnection conn = null;
            string query = @"Update ScheduleDetails set WorkOrderStatus=@WorkOrderStatus where PumpModel=@PumpModel and PackagingType=@PackagingType and [Date]=@ScheduleDate";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@PumpModel", runningModelStatusEntity.RunningPumpModel);
                sqlCommand.Parameters.AddWithValue("@PackagingType", runningModelStatusEntity.PackagingType);
                sqlCommand.Parameters.AddWithValue("@ScheduleDate", runningModelStatusEntity.ScheduleDate);
                sqlCommand.Parameters.AddWithValue("@WorkOrderStatus", status);
                sqlCommand.CommandType = CommandType.Text;
                int rows_affected = sqlCommand.ExecuteNonQuery();
                if (rows_affected > 0)
                    IsUpdated = true;
                else
                    IsUpdated = false;
            }
            catch (Exception ex)
            {
                IsUpdated = false;
                Logger.WriteErrorLog("Error while updating work order status in method UpdateScheduleStatus - \n" + ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return IsUpdated;
        }

        internal static bool UpdateBoxForceCloseStatus(RunningModelStatusEntity runningModelStatusEntity, int closeStatus)
        {
            bool IsUpdated = false;
            SqlConnection conn = null;
            string query = @"Update ScannedDetails set ScannedOrForceCloseStatus=@ScannedOrForceCloseStatus where WONumber=@WONumber and PumpModel=@PumpModel and CustomerModel=@CustomerModel and ScannedSlNo=@ScannedSlNo and ScheduledDate=@ScheduledDate";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@WONumber", runningModelStatusEntity.WorkOrderNumber);
                sqlCommand.Parameters.AddWithValue("@PumpModel", runningModelStatusEntity.RunningPumpModel);
                sqlCommand.Parameters.AddWithValue("@CustomerModel", runningModelStatusEntity.CustomerModel);
                sqlCommand.Parameters.AddWithValue("@ScannedSlNo", runningModelStatusEntity.LastScannedSerialNum);
                sqlCommand.Parameters.AddWithValue("@ScannedOrForceCloseStatus", closeStatus);
                sqlCommand.Parameters.AddWithValue("@ScheduledDate", runningModelStatusEntity.ScheduleDate);
                sqlCommand.CommandType = CommandType.Text;
                int rows_affected = sqlCommand.ExecuteNonQuery();
                if (rows_affected > 0)
                    IsUpdated = true;
                else
                    IsUpdated = false;
            }
            catch (Exception ex)
            {
                IsUpdated = false;
                Logger.WriteErrorLog("Error while updating box force close status in method UpdateBoxForceCloseStatus - \n" + ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return IsUpdated;
        }

        internal static bool UpdatePumpBoxForceCloseStatus(RunningModelStatusEntity runningModelStatusEntity, int closeStatus)
        {
            bool IsUpdated = false;
            SqlConnection conn = null;
            string query = @"Update ScannedDetails set ScannedOrForceCloseStatus=@ScannedOrForceCloseStatus where PumpModel=@PumpModel and PackagingType=@PackagingType and ScannedSlNo=@ScannedSlNo and ScheduledDate=@ScheduledDate";
            try
            {
                conn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@StationID", runningModelStatusEntity.StationID);
                sqlCommand.Parameters.AddWithValue("@PumpModel", runningModelStatusEntity.RunningPumpModel);
                sqlCommand.Parameters.AddWithValue("@PackagingType", runningModelStatusEntity.PackagingType);
                sqlCommand.Parameters.AddWithValue("@ScannedSlNo", runningModelStatusEntity.LastScannedSerialNum);
                sqlCommand.Parameters.AddWithValue("@ScannedOrForceCloseStatus", closeStatus);
                sqlCommand.Parameters.AddWithValue("@ScheduledDate", runningModelStatusEntity.ScheduleDate);
                sqlCommand.CommandType = CommandType.Text;
                int rows_affected = sqlCommand.ExecuteNonQuery();
                if (rows_affected > 0)
                    IsUpdated = true;
                else
                    IsUpdated = false;
            }
            catch (Exception ex)
            {
                IsUpdated = false;
                Logger.WriteErrorLog("Error while updating box force close status in method UpdatePumpBoxForceCloseStatus - \n" + ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return IsUpdated;
        }

        internal static ObservableCollection<ScheduleMasterEntity> GetScheduleMasterData(string WorkOrder, string PumpModel)
        {
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null; int i = 1;
            string Query = @"";
            Query = @"select * from ScheduleDetails where WONumber like '%" + WorkOrder + "%' and PumpModel like'%" + PumpModel + "%' order by [Date] desc";
            ObservableCollection<ScheduleMasterEntity> scheduleMasterDataList = new ObservableCollection<ScheduleMasterEntity>();
            try
            {
                cmd = new SqlCommand(Query, sqlConn);
                cmd.Parameters.AddWithValue("@WONumber", WorkOrder);
                cmd.Parameters.AddWithValue("@PumpModel", PumpModel);
                sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    ScheduleMasterEntity Entity = new ScheduleMasterEntity();
                    Entity.Date = Convert.ToDateTime(sdr["Date"].ToString());
                    Entity.WorkOrderNumber = sdr["WONumber"].ToString();
                    Entity.PumpPartNumber = sdr["PumpModel"].ToString();
                    Entity.CustomerModel = sdr["CustomerModel"].ToString();
                    Entity.CustomerName = sdr["CustomerName"].ToString();
                    Entity.PackagingType = sdr["PackagingType"].ToString();
                    if (!((sdr["PumpQty"]).Equals(DBNull.Value)))
                        Entity.DispatchQty = Convert.ToInt32(sdr["PumpQty"].ToString());
                    if (!((sdr["WeekwisePriority"]).Equals(DBNull.Value)))
                        Entity.Priority = Convert.ToInt32(sdr["WeekwisePriority"].ToString());
                    Entity.SerialNum = i++;
                    scheduleMasterDataList.Add(Entity);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog("Error while fetching schedule master data in method GetScheduleMasterData - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return scheduleMasterDataList;
        }

        internal static bool SavScheduleReportMaster(ScheduleMasterEntity entity)
        {
            bool updated = false;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string Query = @" if not exists(select * from ScheduleDetails where [Date]=@Date and WONumber=@WorkOrder and PumpModel=@PumpModel)
                                begin
                                insert into ScheduleDetails([Date],WONumber,PumpModel,CustomerModel,CustomerName,PackagingType,PumpQty,UpdatedTS,UpdatedBy,WorkOrderStatus)
                                values(@Date,@WorkOrder,@PumpModel,@CustomerModel,@CustomerName,@PackagingType,@PumpQty,@UpdatedTS,@UpdatedBy,@WorkOrderStatus)
                                end
                                else
                                begin
                                update ScheduleDetails set CustomerModel=@CustomerModel,CustomerName=@CustomerName,UpdatedTS=@UpdatedTS,UpdatedBy=@UpdatedBy, PackagingType=@PackagingType,PumpQty=@PumpQty where [Date]=@Date and WONumber=@WorkOrder and PumpModel=@PumpModel
                                end";
            try
            {
                cmd = new SqlCommand(Query, sqlConn);
                cmd.Parameters.AddWithValue("@Date", !entity.Date.Year.Equals(1) ? entity.Date.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@WorkOrder", entity.WorkOrderNumber);
                cmd.Parameters.AddWithValue("@PumpModel", entity.PumpPartNumber);
                if (string.IsNullOrEmpty(entity.CustomerName))
                    cmd.Parameters.AddWithValue("@CustomerName", "");
                else
                    cmd.Parameters.AddWithValue("@CustomerName", entity.CustomerName);
                if (string.IsNullOrEmpty(entity.CustomerModel))
                    cmd.Parameters.AddWithValue("@CustomerModel", "");
                else
                    cmd.Parameters.AddWithValue("@CustomerModel", entity.CustomerModel);
                if (string.IsNullOrEmpty(entity.PackagingType))
                    cmd.Parameters.AddWithValue("@PackagingType", "");
                else
                    cmd.Parameters.AddWithValue("@PackagingType", entity.PackagingType);
                cmd.Parameters.AddWithValue("@PumpQty", entity.DispatchQty);
                cmd.Parameters.AddWithValue("@UpdatedTS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@UpdatedBy", LoginPage.LoginUserName);
                cmd.Parameters.AddWithValue("@WorkOrderStatus", "New");
                cmd.ExecuteNonQuery();
                updated = true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while updating schedule details table in method SavScheduleReportMaster - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return updated;
        }

        internal static bool SavScheduleReportMaster(MonthlyScheduleMasterEntity entity)
        {
            bool updated = false;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string Query = @" if not exists(select * from ScheduleDetails where [Date] = @Date and PumpModel = @PumpModel and PackagingType = @PackagingType)
                begin
                insert into ScheduleDetails([Date], WONumber, PumpModel, CustomerModel, CustomerName, PackagingType, PumpQty, UpdatedTS, UpdatedBy, WorkOrderStatus)
                values(@Date, @WorkOrder, @PumpModel, @CustomerModel, @CustomerName, @PackagingType, @PumpQty, @UpdatedTS, @UpdatedBy, @WorkOrderStatus)
                end
                else
                begin
                update ScheduleDetails set WONumber = @WorkOrder, CustomerModel = @CustomerModel, CustomerName = @CustomerName, UpdatedTS = @UpdatedTS, UpdatedBy = @UpdatedBy, PumpQty = @PumpQty where [Date] = @Date and PumpModel = @PumpModel and PackagingType = @PackagingType
                end";
            try
            {
                cmd = new SqlCommand(Query, sqlConn);
                cmd.Parameters.AddWithValue("@Date", !entity.Date.Year.Equals(1) ? entity.Date.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@WorkOrder", string.IsNullOrEmpty(entity.WorkOrderNumber) ? "" : entity.WorkOrderNumber);
                cmd.Parameters.AddWithValue("@PumpModel", entity.PumpPartNumber);
                if (string.IsNullOrEmpty(entity.CustomerName))
                    cmd.Parameters.AddWithValue("@CustomerName", "");
                else
                    cmd.Parameters.AddWithValue("@CustomerName", entity.CustomerName);
                if (string.IsNullOrEmpty(entity.CustomerModel))
                    cmd.Parameters.AddWithValue("@CustomerModel", "");
                else
                    cmd.Parameters.AddWithValue("@CustomerModel", entity.CustomerModel);
                if (string.IsNullOrEmpty(entity.PackagingType))
                    cmd.Parameters.AddWithValue("@PackagingType", "");
                else
                    cmd.Parameters.AddWithValue("@PackagingType", entity.PackagingType);
                cmd.Parameters.AddWithValue("@PumpQty", entity.DispatchQty);
                cmd.Parameters.AddWithValue("@UpdatedTS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@UpdatedBy", LoginPage.LoginUserName);
                cmd.Parameters.AddWithValue("@WorkOrderStatus", "New");
                cmd.ExecuteNonQuery();
                updated = true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while updating schedule details table in method SavScheduleReportMaster - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return updated;
        }

        internal static bool SaveScheduleMonthReportMaster(MonthlyScheduleMasterEntity entity)
        {
            bool updated = false;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string Query = @" if not exists(select * from MonthlyandWeeklyScheduleDetails where (datepart(year,[Date])=datepart(year,@date) and datepart(month,[Date])=datepart(month,@date)) and PackagingType=@PackagingType and PumpModel=@PumpModel)
                begin
                insert into MonthlyandWeeklyScheduleDetails([Date], WONumber, PumpModel, CustomerModel, CustomerName, PackagingType, UpdatedTS, UpdatedBy, MonthRequirement, Week1Qty, Week2Qty, Week3Qty, Week4Qty, Week5Qty)
                values(@Date, @WorkOrder, @PumpModel, @CustomerModel, @CustomerName, @PackagingType, @UpdatedTS, @UpdatedBy, @MonthRequirement, @Week1Qty, @Week2Qty, @Week3Qty, @Week4Qty, @Week5Qty)
                end
                else
                begin
                update MonthlyandWeeklyScheduleDetails set CustomerName=@CustomerName, CustomerModel=@CustomerModel, UpdatedTS=@UpdatedTS, UpdatedBy=@UpdatedBy, WONumber=@WorkOrder, MonthRequirement=@MonthRequirement, Week1Qty=@Week1Qty, Week2Qty=@Week2Qty, Week3Qty=@Week3Qty, Week4Qty=@Week4Qty, Week5Qty=@Week5Qty where (datepart(year,[Date])=datepart(year,@date) and datepart(month,[Date])=datepart(month,@date)) and PackagingType=@PackagingType and PumpModel=@PumpModel
                end";
            try
            {
                cmd = new SqlCommand(Query, sqlConn);
                cmd.Parameters.AddWithValue("@Date", !entity.Date.Year.Equals(1) ? entity.Date.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@WorkOrder", String.IsNullOrEmpty(entity.WorkOrderNumber) ? "" : entity.WorkOrderNumber);
                cmd.Parameters.AddWithValue("@PumpModel", entity.PumpPartNumber);
                if (string.IsNullOrEmpty(entity.CustomerName))
                    cmd.Parameters.AddWithValue("@CustomerName", "");
                else
                    cmd.Parameters.AddWithValue("@CustomerName", entity.CustomerName);
                if (string.IsNullOrEmpty(entity.CustomerModel))
                    cmd.Parameters.AddWithValue("@CustomerModel", "");
                else
                    cmd.Parameters.AddWithValue("@CustomerModel", entity.CustomerModel);
                if (string.IsNullOrEmpty(entity.PackagingType))
                    cmd.Parameters.AddWithValue("@PackagingType", "");
                else
                    cmd.Parameters.AddWithValue("@PackagingType", entity.PackagingType);
                cmd.Parameters.AddWithValue("@MonthRequirement", entity.MonthSchedule);
                cmd.Parameters.AddWithValue("@Week1Qty", entity.ScheduleWeek1);
                cmd.Parameters.AddWithValue("@Week2Qty", entity.ScheduleWeek2);
                cmd.Parameters.AddWithValue("@Week3Qty", entity.ScheduleWeek3);
                cmd.Parameters.AddWithValue("@Week4Qty", entity.ScheduleWeek4);
                cmd.Parameters.AddWithValue("@Week5Qty", entity.ScheduleWeek5);
                cmd.Parameters.AddWithValue("@UpdatedTS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@UpdatedBy", LoginPage.LoginUserName);
                cmd.ExecuteNonQuery();
                updated = true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while updating schedule details table in method SaveScheduleMonthReportMaster - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return updated;
        }

        internal static List<string> GetPackagingTypeData()
        {
            List<string> PackagingType = new List<string>();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            SqlDataReader sdr = null;
            string query = "select distinct PackagingType from [Packing_Type_Info]";
            try
            {
                cmd = new SqlCommand(query, sqlConn);
                sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    PackagingType.Add(sdr["PackagingType"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting pump list in method GetPumplist - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (sdr != null) sdr.Close();
            }
            return PackagingType;
        }

        internal static List<string> GetWorkOrder(string PumpModel, string Type)
        {
            List<string> WorkOrder = new List<string>();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            SqlDataReader sdr = null;
            string Query = "";
            Query = Type.Equals("Schedule", StringComparison.OrdinalIgnoreCase) ? ("Select distinct WONumber from ScheduleDetails where PumpModel Like'%" + PumpModel + "%'") : (PumpModel.Equals("") ? "Select distinct WONumber  from ScheduleDetails" : "Select distinct WONumber  from ScheduleDetails where PumpModel=@PumpModel");
            try
            {
                cmd = new SqlCommand(Query, sqlConn);
                cmd.Parameters.AddWithValue("@PumpModel", PumpModel);
                sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    WorkOrder.Add(sdr["WONumber"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting work orders in method GetWorkOrder - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (sdr != null) sdr.Close();
            }
            return WorkOrder;
        }

        internal static List<string> GetPumplist()
        {
            List<string> PumpModel = new List<string>();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            SqlDataReader sdr = null;
            string query = "select distinct PumpModel from ScheduleDetails";
            try
            {
                cmd = new SqlCommand(query, sqlConn);
                sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    PumpModel.Add(sdr["PumpModel"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting pump list in method GetPumplist - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (sdr != null) sdr.Close();
            }
            return PumpModel;
        }

        internal static List<string> GetAllStationIDs()
        {
            List<string> stationIDList = new List<string>();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            SqlDataReader sdr = null;
            string query = "select distinct StationID from StationInformation";
            try
            {
                cmd = new SqlCommand(query, sqlConn);
                sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    stationIDList.Add(sdr["StationID"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting station id list in method GetAllStationIDs - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (sdr != null) sdr.Close();
            }
            return stationIDList;
        }

        internal static bool DeleteScheduleMaster(DateTime date, string workOrderNumber, string pumpPartNumber)
        {
            bool delete = false;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string Query = @"delete from ScheduleDetails where [date]=@Date and WONumber=@WorkOrder and PumpModel=@PumpModel ";
            try
            {
                cmd = new SqlCommand(Query, sqlConn);
                cmd.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@WorkOrder", workOrderNumber);
                cmd.Parameters.AddWithValue("@PumpModel", pumpPartNumber);
                cmd.ExecuteNonQuery();
                delete = true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while deleting schedule master data in method DeleteScheduleMaster - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return delete;
        }

        internal static bool DeleteScheduleMaster(DateTime date, string pumpPartNumber, string PackagingType, string CustomerName)
        {
            bool delete = false;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string Query = @"delete from ScheduleDetails where [date]=@Date and PumpModel=@PumpModel and PackagingType=@PackagingType and CustomerName=@CustomerName";
            try
            {
                cmd = new SqlCommand(Query, sqlConn);
                cmd.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@CustomerName", CustomerName);
                cmd.Parameters.AddWithValue("@PackagingType", PackagingType);
                cmd.Parameters.AddWithValue("@PumpModel", pumpPartNumber);
                cmd.ExecuteNonQuery();
                delete = true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while deleting schedule master data in method DeleteScheduleMaster - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return delete;
        }

        internal static bool DeleteMonthlyScheduleMaster(DateTime date, string pumpPartNumber, string PackagingType, string CustomerName)
        {
            bool delete = false;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string Query = @"delete from [MonthlyandWeeklyScheduleDetails] where [date]=@Date and PumpModel=@PumpModel and PackagingType=@PackagingType and CustomerName=@CustomerName";
            try
            {
                cmd = new SqlCommand(Query, sqlConn);
                cmd.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@PumpModel", pumpPartNumber);
                cmd.Parameters.AddWithValue("@PackagingType", PackagingType);
                cmd.Parameters.AddWithValue("@CustomerName", CustomerName);
                cmd.ExecuteNonQuery();
                delete = true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while deleting schedule master data in method DeleteScheduleMaster - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return delete;
        }

        internal static List<PumpDispatchReportEntity> GetPumpDispatchReportData(DateTime fromdate, DateTime todate, string StationID, string PumpModel, string WorkOrder, string Shift)
        {
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string Query = @"s_GetPackingDispatchReport_Mivin";
            StationID = StationID.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : StationID;
            PumpModel = PumpModel.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : PumpModel;
            WorkOrder = WorkOrder.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : WorkOrder;
            Shift = Shift.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : Shift;
            List<PumpDispatchReportEntity> PumpDispatchReportReportList = new List<PumpDispatchReportEntity>();
            try
            {
                cmd = new SqlCommand(Query, sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StationID", StationID);
                cmd.Parameters.AddWithValue("@Fromdate", fromdate.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@Todate", todate.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@Workorder", WorkOrder);
                cmd.Parameters.AddWithValue("@Pumpmodel", PumpModel);
                cmd.Parameters.AddWithValue("@Shift", Shift);
                sdr = cmd.ExecuteReader(); int i = 1;
                while (sdr.Read())
                {
                    PumpDispatchReportEntity Entity = new PumpDispatchReportEntity();
                    Entity.Sl_No = i.ToString(); i++;
                    Entity.StationID = sdr["StationID"].ToString();
                    Entity.PumpPart_No = sdr["PumpModel"].ToString();
                    Entity.BoxType_No = sdr["BoxtypeNo"].ToString();
                    Entity.Quantity = sdr["PackedQty"].ToString();
                    Entity.PackageType = sdr["PackagingType"].ToString();
                    Entity.Remarks = sdr["Remarks"].ToString();
                    PumpDispatchReportReportList.Add(Entity);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting pump dispatch report data in method GetPumpDispatchReportData - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (sdr != null) sdr.Close();
            }
            return PumpDispatchReportReportList;
        }

        internal static List<PackingEfficiencyReportEntity> GetPackingEfficiencyReportData(DateTime FromDate, DateTime ToDate, string StationID, string PumpModel, string WorkOrder, string Shift)
        {
            SqlDataReader sdr = null;
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string Query = @"s_GetPackingEfficiencyReport_Mivin";
            StationID = StationID.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : StationID;
            PumpModel = PumpModel.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : PumpModel;
            WorkOrder = WorkOrder.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : WorkOrder;
            Shift = Shift.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : Shift;
            List<PackingEfficiencyReportEntity> PackingEfficiencyReportList = new List<PackingEfficiencyReportEntity>();
            try
            {
                cmd = new SqlCommand(Query, sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StationID", StationID);
                cmd.Parameters.AddWithValue("@Fromdate", FromDate.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@Todate", ToDate.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@Workorder", WorkOrder);
                cmd.Parameters.AddWithValue("@Pumpmodel", PumpModel);
                cmd.Parameters.AddWithValue("@Shift", Shift);
                sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    PackingEfficiencyReportEntity Entity = new PackingEfficiencyReportEntity();
                    Entity.Date = Convert.ToDateTime(sdr["date"]).ToString("dd-MM-yyyy");
                    Entity.StationID = sdr["StationID"].ToString();
                    Entity.Shift = sdr["Shift"].ToString();
                    Entity.WorkOrderNumber = sdr["WONumber"].ToString();
                    Entity.Customer = sdr["Customer"].ToString();
                    Entity.PumpModel = sdr["PumpModel"].ToString();
                    Entity.PackedQuantity = sdr["PackedQty"].ToString();
                    Entity.ShiftTarget = sdr["ShiftTarget"].ToString();
                    Entity.ShfitEfficiency = sdr["Efficiency"].ToString();
                    Entity.Remarks = sdr["Remarks"].ToString();
                    PackingEfficiencyReportList.Add(Entity);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (sdr != null) sdr.Close();
            }
            return PackingEfficiencyReportList;
        }

        internal static List<MonthlyfulfilmentReportEntity> GetMonthlyFulfilmentReportData(string month, string year)
        {
            SqlDataReader sdr = null;
            SqlConnection sqlConn = null;
            string Query = @"s_GetProductionSchedule&PackingReport_Mivin";
            List<MonthlyfulfilmentReportEntity> monthlyfulfilmentReportList = new List<MonthlyfulfilmentReportEntity>();
            try
            {
                string fulldate = $"{year}-{month}-01 00:00:00";
                sqlConn = ConnectionManager.GetConnection();
                SqlCommand cmd = new SqlCommand(Query, sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Startdate", fulldate);
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        monthlyfulfilmentReportList.Add(new MonthlyfulfilmentReportEntity()
                        {
                            Type = sdr["PackagingType"].ToString(),
                            Customer = sdr["CustomerName"].ToString(),
                            PumpPartNumber = sdr["PumpModel"].ToString(),
                            MonthRequirement = sdr["MonthRequirement"] != null ? Convert.ToInt32(sdr["MonthRequirement"].ToString()) : 0,
                            ActualPackedQty = sdr["ActualPackedQty"] != null ? Convert.ToInt32(sdr["ActualPackedQty"].ToString()) : 0,
                            PendingForPacking = sdr["PendingForPacking"] != null ? Convert.ToInt32(sdr["PendingForPacking"].ToString()) : 0,
                            ActualDispatch = (sdr["ActualDispatch"] != null && !string.IsNullOrEmpty(sdr["ActualDispatch"].ToString())) ? sdr["ActualDispatch"].ToString() : "",
                            CW1 = sdr["CW1"] != null ? Convert.ToInt32(sdr["CW1"].ToString()) : 0,
                            CW1Actual = sdr["CW1Actual"] != null ? Convert.ToInt32(sdr["CW1Actual"].ToString()) : 0,
                            CW2 = sdr["CW2"] != null ? Convert.ToInt32(sdr["CW2"].ToString()) : 0,
                            CW2Actual = sdr["Cw2Actual"] != null ? Convert.ToInt32(sdr["Cw2Actual"].ToString()) : 0,
                            CW3 = sdr["CW3"] != null ? Convert.ToInt32(sdr["CW3"].ToString()) : 0,
                            CW3Actual = sdr["CW3Actual"] != null ? Convert.ToInt32(sdr["CW3Actual"].ToString()) : 0,
                            CW4 = sdr["CW4"] != null ? Convert.ToInt32(sdr["CW4"].ToString()) : 0,
                            CW4Actual = sdr["CW4Actual"] != null ? Convert.ToInt32(sdr["CW4Actual"].ToString()) : 0,
                            CW5 = sdr["CW5"] != null ? Convert.ToInt32(sdr["CW5"].ToString()) : 0,
                            CW5Actual = sdr["CW5Actual"] != null ? Convert.ToInt32(sdr["CW5Actual"].ToString()) : 0
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (sdr != null) sdr.Close();
            }
            return monthlyfulfilmentReportList;
        }

        internal static List<string> GetAllPumpModels()
        {
            List<string> PumpModelsList = new List<string>();
            SqlConnection sqlConn = null;
            SqlDataReader reader = null;
            try
            {
                sqlConn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(@"select distinct PumpModel from PumpMasterData", sqlConn);
                sqlCommand.CommandType = CommandType.Text;
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PumpModelsList.Add(reader["PumpModel"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting pump models in method GetAllPumpModels()" + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (reader != null) reader.Close();
            }
            return PumpModelsList;
        }

        internal static List<string> GetAllCustomerNames()
        {
            List<string> CustomerNamesList = new List<string>();
            SqlConnection sqlConn = null;
            SqlDataReader reader = null;
            try
            {
                sqlConn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(@"select distinct CustomerName from PumpMasterData where CustomerName<>''", sqlConn);
                sqlCommand.CommandType = CommandType.Text;
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (!string.IsNullOrEmpty(reader["CustomerName"].ToString()))
                            CustomerNamesList.Add(reader["CustomerName"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting customer names in method GetAllCustomerNames()" + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (reader != null) reader.Close();
            }
            return CustomerNamesList;
        }

        internal static List<string> GetAllSalesUnits()
        {
            List<string> SalesUnitsList = new List<string>();
            SqlConnection sqlConn = null;
            SqlDataReader reader = null;
            try
            {
                sqlConn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(@"select distinct SalesUnit from PumpMasterData where SalesUnit<>''", sqlConn);
                sqlCommand.CommandType = CommandType.Text;
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (!string.IsNullOrEmpty(reader["SalesUnit"].ToString()))
                            SalesUnitsList.Add(reader["SalesUnit"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting sales units in method GetAllSalesUnits()" + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (reader != null) reader.Close();
            }
            return SalesUnitsList;
        }

        internal static ObservableCollection<ShiftTargetMasterEntity> GetShiftTargetMasterData(DateTime selectedDate)
        {
            ObservableCollection<ShiftTargetMasterEntity> shiftTargetData = new ObservableCollection<ShiftTargetMasterEntity>();
            ShiftTargetMasterEntity shiftTargetInfo = null;
            SqlConnection sqlConn = null;
            SqlDataReader reader = null;
            try
            {
                int val = 3;
                sqlConn = ConnectionManager.GetConnection();
                SqlCommand sqlCommand = new SqlCommand(@"select * from ShiftwiseTargetMaster where [Date] = @SchDate", sqlConn);
                sqlCommand.Parameters.AddWithValue("@SchDate", selectedDate.ToString("yyyy-MM-dd 00:00:00"));
                sqlCommand.CommandType = CommandType.Text;
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        shiftTargetInfo = new ShiftTargetMasterEntity();
                        shiftTargetInfo.ScheduleDate = Convert.ToDateTime(reader["Date"]);
                        shiftTargetInfo.Shift = reader["Shift"].ToString();
                        shiftTargetInfo.StationID = reader["StationID"].ToString();
                        shiftTargetInfo.NumOfPersons = int.TryParse(reader["NoOfManpower"].ToString(), out val) ? val : 3;
                        shiftTargetInfo.ShiftTarget = int.TryParse(reader["Target"].ToString(), out val) ? val : 900;
                        shiftTargetData.Add(shiftTargetInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error while getting shift target data in method GetShiftTargetMasterData()" + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (reader != null) reader.Close();
            }
            return shiftTargetData;
        }

        internal static void UpdateShiftTargetData(ShiftTargetMasterEntity shiftTargetDataRow, out bool isUpdated)
        {
            isUpdated = false;
            SqlConnection sqlConn = null;
            string query = @"if exists(select * from ShiftwiseTargetMaster where [Date] = @SchDate and Shift = @Shift and StationID = @StationID) begin update ShiftwiseTargetMaster set NoOfManpower = @NoOfManpower, Target = @Target where [Date] = @SchDate and Shift = @Shift and StationID = @StationID end";
            try
            {
                if (shiftTargetDataRow != null)
                {
                    sqlConn = ConnectionManager.GetConnection();
                    SqlCommand cmd = new SqlCommand(query, sqlConn);
                    cmd.Parameters.AddWithValue("@SchDate", shiftTargetDataRow.ScheduleDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@Shift", shiftTargetDataRow.Shift);
                    cmd.Parameters.AddWithValue("@StationID", shiftTargetDataRow.StationID);
                    cmd.Parameters.AddWithValue("@NoOfManpower", shiftTargetDataRow.NumOfPersons);
                    cmd.Parameters.AddWithValue("@Target", shiftTargetDataRow.ShiftTarget);
                    cmd.CommandType = CommandType.Text;
                    int rows_affected = cmd.ExecuteNonQuery();
                    if (rows_affected > 0)
                        isUpdated = true;
                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
                Logger.WriteErrorLog("Error in updating data in [dbo].[PumpMasterData] - " + ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
        }

        internal static ObservableCollection<MonthlyScheduleMasterEntity> GetMonthlyScheduleMasterData(DateTime SelectedDate)
        {
            SqlDataReader sdr = null;
            SqlConnection sqlConn = null;
            int i = 1;
            ObservableCollection<MonthlyScheduleMasterEntity> scheduleMasterDataList = new ObservableCollection<MonthlyScheduleMasterEntity>();
            try
            {
                sqlConn = ConnectionManager.GetConnection();
                SqlCommand cmd = new SqlCommand(@"s_GetScheduleDetails_Mivin", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Startdate", SelectedDate.ToString("yyyy-MM-dd"));
                sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    MonthlyScheduleMasterEntity Entity = new MonthlyScheduleMasterEntity();
                    Entity.Date = SelectedDate;
                    Entity.PumpPartNumber = sdr["PartNo"].ToString();
                    Entity.CustomerName = sdr["Customer"].ToString();
                    Entity.PackagingType = sdr["Type"].ToString();
                    Entity.WorkOrderNumber = sdr["WONumber"].ToString();
                    Entity.CustomerModel = sdr["CustomerModel"].ToString();
                    Entity.SerialNum = i++;
                    if (!((sdr["MonthRequirement"]).Equals(DBNull.Value)))
                        Entity.MonthSchedule = Convert.ToInt32(sdr["MonthRequirement"].ToString());
                    if (!((sdr["CW1"]).Equals(DBNull.Value)))
                        Entity.ScheduleWeek1 = Convert.ToInt32(sdr["CW1"].ToString());
                    if (!((sdr["CW2"]).Equals(DBNull.Value)))
                        Entity.ScheduleWeek2 = Convert.ToInt32(sdr["CW2"].ToString());
                    if (!((sdr["CW3"]).Equals(DBNull.Value)))
                        Entity.ScheduleWeek3 = Convert.ToInt32(sdr["CW3"].ToString());
                    if (!((sdr["CW4"]).Equals(DBNull.Value)))
                        Entity.ScheduleWeek4 = Convert.ToInt32(sdr["CW4"].ToString());
                    if (!((sdr["CW5"]).Equals(DBNull.Value)))
                        Entity.ScheduleWeek5 = Convert.ToInt32(sdr["CW5"].ToString());

                    Entity.One = Convert.ToInt32(string.IsNullOrEmpty(sdr["1"].ToString()) ? "0" : sdr["1"].ToString());
                    Entity.Two = Convert.ToInt32(string.IsNullOrEmpty(sdr["2"].ToString()) ? "0" : sdr["2"].ToString());
                    Entity.Three = Convert.ToInt32(string.IsNullOrEmpty(sdr["3"].ToString()) ? "0" : sdr["3"].ToString());
                    Entity.Four = Convert.ToInt32(string.IsNullOrEmpty(sdr["4"].ToString()) ? "0" : sdr["4"].ToString());
                    Entity.Five = Convert.ToInt32(string.IsNullOrEmpty(sdr["5"].ToString()) ? "0" : sdr["5"].ToString());
                    Entity.Six = Convert.ToInt32(string.IsNullOrEmpty(sdr["6"].ToString()) ? "0" : sdr["6"].ToString());
                    Entity.Seven = Convert.ToInt32(string.IsNullOrEmpty(sdr["7"].ToString()) ? "0" : sdr["7"].ToString());
                    Entity.Eight = Convert.ToInt32(string.IsNullOrEmpty(sdr["8"].ToString()) ? "0" : sdr["8"].ToString());
                    Entity.Nine = Convert.ToInt32(string.IsNullOrEmpty(sdr["9"].ToString()) ? "0" : sdr["9"].ToString());
                    Entity.Ten = Convert.ToInt32(string.IsNullOrEmpty(sdr["10"].ToString()) ? "0" : sdr["10"].ToString());
                    Entity.Eleven = Convert.ToInt32(string.IsNullOrEmpty(sdr["11"].ToString()) ? "0" : sdr["11"].ToString());
                    Entity.Twelve = Convert.ToInt32(string.IsNullOrEmpty(sdr["12"].ToString()) ? "0" : sdr["12"].ToString());
                    Entity.Thirteen = Convert.ToInt32(string.IsNullOrEmpty(sdr["13"].ToString()) ? "0" : sdr["13"].ToString());
                    Entity.Fourteen = Convert.ToInt32(string.IsNullOrEmpty(sdr["14"].ToString()) ? "0" : sdr["14"].ToString());
                    Entity.Fifteen = Convert.ToInt32(string.IsNullOrEmpty(sdr["15"].ToString()) ? "0" : sdr["15"].ToString());
                    Entity.Sixteen = Convert.ToInt32(string.IsNullOrEmpty(sdr["16"].ToString()) ? "0" : sdr["16"].ToString());
                    Entity.Seventeen = Convert.ToInt32(string.IsNullOrEmpty(sdr["17"].ToString()) ? "0" : sdr["17"].ToString());
                    Entity.Eighteen = Convert.ToInt32(string.IsNullOrEmpty(sdr["18"].ToString()) ? "0" : sdr["18"].ToString());
                    Entity.Nineteen = Convert.ToInt32(string.IsNullOrEmpty(sdr["19"].ToString()) ? "0" : sdr["19"].ToString());
                    Entity.Twenty = Convert.ToInt32(string.IsNullOrEmpty(sdr["20"].ToString()) ? "0" : sdr["20"].ToString());
                    Entity.Twentyone = Convert.ToInt32(string.IsNullOrEmpty(sdr["21"].ToString()) ? "0" : sdr["21"].ToString());
                    Entity.Twentytwo = Convert.ToInt32(string.IsNullOrEmpty(sdr["22"].ToString()) ? "0" : sdr["22"].ToString());
                    Entity.Twentythree = Convert.ToInt32(string.IsNullOrEmpty(sdr["23"].ToString()) ? "0" : sdr["23"].ToString());
                    Entity.Twentyfour = Convert.ToInt32(string.IsNullOrEmpty(sdr["24"].ToString()) ? "0" : sdr["24"].ToString());
                    Entity.Twentyfive = Convert.ToInt32(string.IsNullOrEmpty(sdr["25"].ToString()) ? "0" : sdr["25"].ToString());
                    Entity.Twentysix = Convert.ToInt32(string.IsNullOrEmpty(sdr["26"].ToString()) ? "0" : sdr["26"].ToString());
                    Entity.Twentyseven = Convert.ToInt32(string.IsNullOrEmpty(sdr["27"].ToString()) ? "0" : sdr["27"].ToString());
                    Entity.Twentyeight = Convert.ToInt32(string.IsNullOrEmpty(sdr["28"].ToString()) ? "0" : sdr["28"].ToString());
                    Entity.Twentynine = Convert.ToInt32(string.IsNullOrEmpty(sdr["29"].ToString()) ? "0" : sdr["29"].ToString());
                    Entity.Thirty = Convert.ToInt32(string.IsNullOrEmpty(sdr["30"].ToString()) ? "0" : sdr["30"].ToString());
                    Entity.ThirtyOne = Convert.ToInt32(string.IsNullOrEmpty(sdr["31"].ToString()) ? "0" : sdr["31"].ToString());
                    scheduleMasterDataList.Add(Entity);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebugLog("Error while fetching schedule master data in method GetMonthlyScheduleMasterData - " + ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
                if (sqlConn != null) sqlConn.Close();
            }
            return scheduleMasterDataList;
        }
    }
}
