using ems.system.Models;
using ems.utilities.Functions;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ems.system.Controllers
{
    [Route("api/ManageEmployee")]
    [ApiController]
    public class ManageEmployeeController : ControllerBase
    {
        string? msSQL, lsemployee_gid;
        DataTable? dt_datatable;
        private readonly dbconn objdbconn;
        private readonly IWebHostEnvironment objenvironment;

        public ManageEmployeeController(dbconn dbconn, IWebHostEnvironment environment)
        {
            objdbconn = dbconn;
            objenvironment = environment;
        }
        [HttpGet("GetEmployeename")]
        public IActionResult GetEmployeename(string user_gid)
        {
            employee_list objvalues = new employee_list();
            result objresult = new result();
            try
            {

                msSQL = " SELECT employee_gid FROM adm_mst_tuser a left join hrm_mst_temployee b on b.user_gid=a.user_gid WHERE a.user_gid='" + user_gid + "' ";
                lsemployee_gid = objdbconn.GetExecuteScalar(msSQL);

                msSQL = " select concat(user_firstname,' ',user_lastname,' / ',user_code) As Name , " +
                        " CASE WHEN '" + lsemployee_gid + "' = 'E1' THEN 'Y' ELSE 'N' END AS service_flag " +
                        " FROM adm_mst_tuser " +
                        " left join hrm_mst_temployee using(user_gid) where employee_gid='" + lsemployee_gid + "'";
                dt_datatable = objdbconn.GetDataTable(msSQL);

                var get_holiday = new List<employeename_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
                    {
                        get_holiday.Add(new employeename_list
                        {
                            Name = dt["Name"].ToString(),
                            service_flag = dt["service_flag"].ToString(),
                        });
                    }
                    objvalues.employeename_list = get_holiday;
                }
                dt_datatable.Dispose();
                objresult.status = true;
                return Ok(objvalues);
            }
            catch
            {
                objresult.status = false;
                return BadRequest(objvalues);
            }
        }
    }
}
