using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoryboardAPI.Model
{
    public class MdlCustomerLogin
    {
        public List<customerloginResponse>? customerloginResponse { get; set; }
        public List<Postcustomer>? Postcustomer { get; set; }
    }
    public class Postcustomer
    {
        [Required]
        [DisplayName("MailID")]
        public string? eportal_emailid { get; set; }
        public string? eportal_password { get; set; }
        public string? company_code { get; set; }
    }
    public class customerloginResponse
    {
        public string? token { get; set; }
        public string? customer_gid { get; set; }
        public string? c_code { get; set; }
        public string? dashboard_flag { get; set; }
        public string? message { get; set; }
        public bool status { get; set; }
    }
}
