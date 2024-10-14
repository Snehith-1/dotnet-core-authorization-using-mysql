namespace ems.system.Models
{
    public class UserData : result
    {
        public string user_code { get; set; }
        public string user_name { get; set; }
        public string user_photo { get; set; }
        public string user_designation { get; set; }
        public string user_department { get; set; }

    }

    public class result
    {
        public string message { get; set; }
        public bool status { get; set; }
    }

    public class menu_response : result
    {
        public List<sys_menu> menu_list { get; set; }


    }

    public class sys_menu
    {
        public string menu_access { get; set; }
        public string module_gid { get; set; }
        public string text { get; set; }
        public string sref { get; set; }
        public string angular_sref { get; set; }
        public string icon { get; set; }
        public string label { get; set; }
        public string privilege { get; set; }
        public string menu_indication { get; set; }
        public string menu_indication1 { get; set; }
        public List<sys_submenu> submenu { get; set; }
    }

    public class sys_submenu
    {
        // public string submenu_code { get; set; }
        public string text { get; set; }
        public string module_gid { get; set; }
        public string sref { get; set; }
        public string angular_sref { get; set; }
        public string menu_indication { get; set; }
        public string menu_indication1 { get; set; }
        public string menu_access { get; set; }
        public bool ennableState { get; set; }
        public bool activeState { get; set; }
        public List<sys_sub1menu> sub1menu { get; set; }
    }
    public class sys_sub1menu
    {
        // public string submenu_code { get; set; }
        public string text { get; set; }
        public string module_gid { get; set; }
        public string sref { get; set; }
        public string angular_sref { get; set; }
        //public string icon { get; set; }
        public string menu_access { get; set; }
        public List<sys_sub2menu> sub2menu { get; set; }

    }
    public class sys_sub2menu
    {
        // public string submenu_code { get; set; }
        public string text { get; set; }
        public string module_gid { get; set; }
        public bool module_checked { get; set; }
        public string sref { get; set; }
        public string angular_sref { get; set; }
        public string icon { get; set; }
        //  public string sub_icon { get; set; }
        public string menu_access { get; set; }
    }
    public class moduleprivilege_userlist
    {
        //public List<moduleprivilege_user> moduleprivilege_user { get; set; }
        public string user_gid { get; set; }
        public string module_gid { get; set; }
        public string module_name { get; set; }
        public string module_gid_parent { get; set; }
        public string sref { get; set; }
        public string icon { get; set; }
        public string menu_level { get; set; }

    }
    public class moduleprivilege_user
    {
        public string user_gid { get; set; }
        public string module_gid { get; set; }
        public string module_name { get; set; }
        public string module_gid_parent { get; set; }
        public string sref { get; set; }
        public string icon { get; set; }
        public string menu_level { get; set; }

    }


    public class project_list
    {
        public List<privilege> privileges { get; set; }
    }
    public class privilege
    {
        public string project { get; set; }

    }
    public class projectlist
    {
        public List<privilegelevel3> privilegelevel3 { get; set; }
    }
    public class privilegelevel3
    {
        public string project { get; set; }

    }

    public class companydetails : result
    {
        public string company_name { get; set; }
        public string company_logo { get; set; }
        public string companylogo_responsive { get; set; }
    }

    public class mdlMenuData
    {
        public string module_gid { get; set; }
        public string module_name { get; set; }
        public string sref { get; set; }
        public string icon { get; set; }
        public string menu_level { get; set; }
        public string module_gid_parent { get; set; }
        public string display_order { get; set; }
        public string menu_access { get; set; }
    }

}