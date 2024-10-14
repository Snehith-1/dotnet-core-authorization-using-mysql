using System.Data;
using System.Data.Odbc;

namespace ems.utilities.Functions
{
    public class dbconn
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public dbconn(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetConnectionString(string companycode)
        {
            string? lsConnectionString;

            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authorizationHeader) || authorizationHeader == "null")
            {               
                lsConnectionString = _configuration.GetSection("user").GetConnectionString("Auth");
            }
            else
            {
                using (OdbcConnection conn = new OdbcConnection(_configuration.GetSection("user").GetConnectionString("Auth")))
                {
                    using (OdbcCommand cmd = new OdbcCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "CALL adm_mst_spgetconnectionstring('" + authorizationHeader + "')";
                        cmd.Connection = conn;
                        conn.Open();
                        lsConnectionString = cmd.ExecuteScalar()?.ToString();
                        conn.Close();
                    }
                }

                if (string.IsNullOrEmpty(lsConnectionString))
                {
                    throw new Exception("Conflict: Connection string not found.");
                }
            }

            return lsConnectionString;
        }
        public OdbcConnection OpenConn(string companyCode = "")
        {
            try
            {
                OdbcConnection gs_ConnDB;
                gs_ConnDB = new OdbcConnection(GetConnectionString(companyCode));
                if (gs_ConnDB.State != ConnectionState.Open)
                {
                    gs_ConnDB.Open();
                }
                return gs_ConnDB;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void CloseConn()
        {
            try
            {
                if (OpenConn().State != ConnectionState.Closed)
                {
                    OpenConn().Dispose();
                    OpenConn().Close();
                }
            }

            catch (Exception ex)
            {
                LogForAudit($"HTTP Response Exception: {ex.ToString()}");
                throw;
            }
        }
        public int ExecuteNonQuerySQL(string query, string user_gid = null, string module_reference = null, string module_name = "Log")
        {
            int mnResult = 0;
            try
            {
                OdbcConnection ObjOdbcConnection = OpenConn();
                try
                {
                    OdbcCommand cmd = new OdbcCommand(query, ObjOdbcConnection);
                    mnResult = cmd.ExecuteNonQuery();
                    mnResult = 1;
                }
                catch (Exception e)
                {
                    LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" + e.Message.ToString() + "*****Query****" + query + "*******Apiref********" + module_reference);
                }
                ObjOdbcConnection.Close();
                return mnResult;
            }
            catch (Exception ex)
            {
                return mnResult;
            }
        }
        public int ExecuteNonQuerySQLForgot(string query, string companyCode = "", string user_gid = null, string module_reference = null, string module_name = "Log")
        {
            int mnResult = 0;
            try
            {
                OdbcConnection ObjOdbcConnection = OpenConn(companyCode);
                try
                {
                    OdbcCommand cmd = new OdbcCommand(query, ObjOdbcConnection);
                    mnResult = cmd.ExecuteNonQuery();
                    mnResult = 1;
                }
                catch (Exception e)
                {
                    LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" + e.Message.ToString() + "*****Query****" + query + "*******Apiref********" + module_reference);
                }
                ObjOdbcConnection.Close();
                return mnResult;
            }
            catch (Exception ex) { return mnResult; }
        }
        public string GetExecuteScalar(string query, string companyCode = "", string user_gid = null, string module_reference = null, string module_name = "Log")
        {
            string? val = "";
            try
            {
                OdbcConnection ObjOdbcConnection = OpenConn(companyCode);

                OdbcCommand cmd = new OdbcCommand(query, ObjOdbcConnection);
                var val1 = cmd.ExecuteScalar();
                val = val1?.ToString();

                ObjOdbcConnection.Close();
            }
            catch (Exception e)
            {
                LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" + e.Message.ToString() + "*****Query****" + query + "*******Apiref********" + module_reference);
            }

            return val;
        }
        public OdbcDataReader GetDataReader(string query, string companyCode = "", string user_gid = null, string module_reference = null, string module_name = "Log")
        {
            try
            {
                OdbcCommand? cmd = new OdbcCommand(query, OpenConn(companyCode));
                OdbcDataReader? rdr;
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //rdr.Read();
                return rdr;
            }
            catch (Exception e)
            {
                LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" + e.Message.ToString() + "*****Query****" + query + "*******Apiref********" + module_reference);
                return null;
            }

        }
        public DataTable GetDataTable(string query, string user_gid = null, string module_reference = null, string module_name = "Log")
        {
            try
            {
                OdbcConnection ObjOdbcConnection = OpenConn();
                DataTable dt = new DataTable();
                OdbcDataAdapter da = new OdbcDataAdapter(query, ObjOdbcConnection);
                da.Fill(dt);
                ObjOdbcConnection.Close();
                return dt;
            }
            catch (Exception e)
            {
                LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" + e.Message.ToString() + "*****Query****" + query + "*******Apiref********" + module_reference);
                return null;
            }

        }
        public DataSet GetDataSet(string query, string table, string user_gid = null, string module_reference = null, string module_name = "Log")
        {
            try
            {
                OdbcConnection ObjOdbcConnection = OpenConn();
                DataSet ds = new DataSet();
                OdbcDataAdapter da = new OdbcDataAdapter(query, ObjOdbcConnection);
                da.Fill(ds, table);
                ObjOdbcConnection.Close();
                return ds;
            }
            catch (Exception e)
            {
                LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" + e.Message.ToString() + "*****Query****" + query + "*******Apiref********" + module_reference);
                return null;
            }

        }
        public DataTable GetDataTableSP(string storedProcedureName, params OdbcParameter[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OdbcConnection objOdbcConnection = OpenConn())
                {
                    using (OdbcCommand cmd = new OdbcCommand(storedProcedureName, objOdbcConnection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add the parameters to the command
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (OdbcDataAdapter adapter = new OdbcDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                LogForAudit($"HTTP Response Exception: {ex.ToString()}");
                throw;
            }

        }
        public void LogForAudit(string content)
        {
            try
            {
                var company_code = _httpContextAccessor.HttpContext?.Request.Headers["c_code"].ToString();
                company_code = company_code == null ? "COMMON" : company_code.ToUpper();
                try
                {
                    string? lspath = _configuration["Log_path"] + company_code + "/ExceptionLog";
                    if ((!System.IO.Directory.Exists(lspath)))
                    {
                        System.IO.Directory.CreateDirectory(lspath);
                    }
                    lspath = lspath + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(lspath, true);
                    sw.WriteLine(content);
                    sw.Close();
                }
                catch (Exception ex)
                {

                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
