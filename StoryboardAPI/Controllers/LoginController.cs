using ems.utilities.Functions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Newtonsoft.Json.Linq;
using StoryboardAPI.Authorization;
using StoryboardAPI.Model;
using System.Data.Odbc;

namespace StoryboardAPI.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        string? tokenvalue, msSQL, lsdefault_screen;
        private readonly cmnfunctions objcmnfunctions;
        private readonly validateUser objvalidateuser;
        private readonly dbconn objdbconn;
        OdbcDataReader? objMySqlDataReader;

        public LoginController(cmnfunctions cmnfunctions, dbconn dbconn, validateUser validateUser)
        {
            objcmnfunctions = cmnfunctions;
            objdbconn = dbconn;
            objvalidateuser = validateUser;
        }

        [HttpPost("UserLogin")]
        public IActionResult PostUserLogin(PostUserLogin values)
        {
            loginresponse objloginresponse = new loginresponse();
            var ObjToken = objvalidateuser.Token(values.user_code, objcmnfunctions.ConvertToAscii(values.user_password), values.company_code.ToLower());
            if (ObjToken != null)
            {
                tokenvalue = "Bearer " + ObjToken;
                msSQL = "call adm_mst_spstoretoken('" + tokenvalue + "','" + values.user_code + "','" + objcmnfunctions.ConvertToAscii(values.user_password) + "','" + values.company_code + "')";
                objMySqlDataReader = objdbconn.GetDataReader(msSQL);
                if (objMySqlDataReader.HasRows)
                {
                    objloginresponse.token = tokenvalue;
                    objloginresponse.user_gid = objMySqlDataReader["user_gid"].ToString();
                    objloginresponse.dashboard_flag = objMySqlDataReader["dashboard_flag"].ToString();
                    objloginresponse.c_code = values.company_code;
                    objloginresponse.message = "Login Successful!";
                    objloginresponse.status = true;
                    msSQL = "select a.default_screen from " + objloginresponse.c_code + ".hrm_mst_temployee a left join " + objloginresponse.c_code + ".adm_mst_tuser b " +
                       "  on a.user_gid= b.user_gid where b.user_gid ='" + objloginresponse.user_gid + "'";
                    lsdefault_screen = objdbconn.GetExecuteScalar(msSQL);
                    if (lsdefault_screen != "")
                    {
                        msSQL = " select sref,k_sref from " + objloginresponse.c_code + ".adm_mst_tmoduleangular where module_gid " +
                            "in (select default_screen from " + objloginresponse.c_code + ".hrm_mst_temployee a left join " + objloginresponse.c_code + ".adm_mst_tuser b" +
                            " on b.user_gid = a.user_gid where b.user_gid ='" + objloginresponse.user_gid + "')";
                        objMySqlDataReader = objdbconn.GetDataReader(msSQL);
                        if (objMySqlDataReader.HasRows)
                        {
                            objloginresponse.sref = objMySqlDataReader["sref"].ToString();
                            objloginresponse.k_sref = objMySqlDataReader["k_sref"].ToString();
                        }
                    }
                }
                else
                {
                    objloginresponse.message = "Invalid Credentials!";
                }
            }
            return Ok(objloginresponse);
        }
    }
}
