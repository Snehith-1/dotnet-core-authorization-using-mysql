using Microsoft.IdentityModel.Tokens;
using System.Data.Odbc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoryboardAPI.Authorization
{
    public class validateUser
    {
        private readonly ems.utilities.Functions.dbconn objdbconn;
        private readonly IConfiguration objconfiguration;
        OdbcDataReader? objODBCDataReader;
        string? mssql;

        public validateUser(ems.utilities.Functions.dbconn dbconn, IConfiguration configuration)
        {
            objdbconn = dbconn;
            objconfiguration = configuration;
        }
        public bool isvalid(string? username, string? password, string? RouterPrefix = "", string companycode = "")
        {
            if (RouterPrefix == "api/InstituteLogin")
            {
                mssql = " SELECT institute_gid FROM " + companycode + ".law_mst_tinstitute " +
                    " WHERE institute_code='" + username + "' AND password='" + password + "'";
            }
            else if (RouterPrefix == "api/customerlogin")
            {
                mssql = " SELECT customer_gid FROM " + companycode + ".crm_mst_tcustomer " +
                   " WHERE eportal_emailid='" + username + "' AND eportal_password='" + password + "'";
            }
            else
            {
                mssql = " SELECT user_gid FROM " + companycode + ".adm_mst_tuser " +
                " WHERE user_code='" + username + "' AND user_password='" + password + "'";
            }
            objODBCDataReader = objdbconn.GetDataReader(mssql, companycode);
            if (objODBCDataReader.HasRows)
            {
                objODBCDataReader.Close();
                return true;
            }
            else
            {
                objODBCDataReader.Close();
                return false;
            }
        }
        public class MdlCmnConn
        {
            public string? connection_string { get; set; }
            public string? company_code { get; set; }
            public string? company_dbname { get; set; }
        }
        public string Token(string user_name, string? password, string companycode, string routerprefix = "")
        {
            bool status = isvalid(user_name, password, routerprefix, companycode);
            if (status == false)
            {
                return null;
            }
            else
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(objconfiguration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier,user_name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var token = new JwtSecurityToken(objconfiguration["Jwt:Issuer"],
                objconfiguration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
    }
}