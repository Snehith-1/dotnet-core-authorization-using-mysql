using ems.utilities.Functions;
using Microsoft.AspNetCore.Mvc;
using StoryboardAPI.Authorization;
using StoryboardAPI.Model;
using System.Data.Odbc;

namespace StoryboardAPI.Controllers
{
    [Route("api/CustomerLogin")]
    [ApiController]
    public class CustomerLogin : ControllerBase
    {
        string? domain;
        private readonly dbconn conn;
        private readonly cmnfunctions objcmnfunctions;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private readonly validateUser objvalidateuser;
        public CustomerLogin(dbconn dbconn, cmnfunctions cmnfunctions, IHttpContextAccessor contextAccessor,
            IConfiguration configuration, validateUser validateUser)
        {
            conn = dbconn;
            objcmnfunctions = cmnfunctions;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            objvalidateuser = validateUser;
        }

        string ?msSQL, tokenvalue = string.Empty;
        OdbcDataReader? odbcDataReader;

        [HttpPost("customerlogin")]
        public IActionResult customerlogin(Postcustomer values)
        {
            customerloginResponse objresponse = new customerloginResponse();
            try
            {
                msSQL = " select eportal_status from " + values.company_code + ".crm_mst_tcustomer where eportal_emailid='" + values.eportal_emailid + "'";
                string portal_status = conn.GetExecuteScalar(msSQL);
                if (portal_status == "Y")
                {
                    var objtoken = objvalidateuser.Token(values.eportal_emailid, objcmnfunctions.ConvertToAscii(values.eportal_password), values.company_code, "api/customerlogin");
                    if (objtoken != null)
                    {
                        tokenvalue = "Jwt Bearer " + objtoken;
                        msSQL = " call adm_mst_spcvstoretoken('" + tokenvalue + "','" + values.eportal_emailid + "','" + objcmnfunctions.ConvertToAscii(values.eportal_password) + "','" + values.company_code + "')";
                        odbcDataReader = conn.GetDataReader(msSQL);
                        if (odbcDataReader.HasRows == true)
                        {
                            odbcDataReader.Read();
                            objresponse.token = tokenvalue;
                            objresponse.customer_gid = odbcDataReader["customer_gid"]?.ToString();
                            objresponse.c_code = values.company_code;
                            objresponse.message = "Login Successfull!";
                            objresponse.dashboard_flag = "SMR";
                            objresponse.status = true;
                        }
                    }
                    else
                    {
                        objresponse.message = "Invalid user.";
                    }
                }
                else
                {
                    objresponse.message = "Activate the ePortal status for the customer.";
                }
                return Ok(objresponse);
            }
            catch (Exception ex)
            {
                return BadRequest(objresponse);
            }
        }

        [HttpPost("UploadFile")]
        public IActionResult UploadFile(IFormFile file)
        {
            string? customer_gid = Request.Form["customer_gid"];

            msSQL = " select customer_name from crm_mst_tcustomer where customer_gid='" + customer_gid + "'";
            string customer_name = conn.GetExecuteScalar(msSQL);
            string? file_path = _configuration["uploadfile"];
            var filepath = Path.Combine(file_path, file.FileName);
            using (var Stream = new FileStream(filepath, FileMode.Create))
            {
                file.CopyToAsync(Stream);
            }
            return Ok();
        }
    }
}
