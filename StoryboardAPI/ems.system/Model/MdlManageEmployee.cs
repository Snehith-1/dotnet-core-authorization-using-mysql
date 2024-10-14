namespace ems.system.Models
{
    public class employee_list : employee
    {
        public List<employee>? employee { get; set; }
        public List<employeename_list>? employeename_list { get; set; }
    }
    public class employee
    {
        public string? user_gid { get; set; }
        public string? user_code { get; set; }
        public string? user_firstname { get; set; }
        public string? user_lastname { get; set; }
        public string? user_password { get; set; }
        public string? user_status { get; set; }
        public string? employee_gid { get; set; }
        public string? designation_gid { get; set; }
        public string? employee_mobileno { get; set; }
        public string? employee_dob { get; set; }
        public string? employee_emailid { get; set; }
        public string? employee_gender { get; set; }
        public string? department_gid { get; set; }
        public string? employee_photo { get; set; }
        public string? useraccess { get; set; }
        public string? engagement_type { get; set; }
        public string? biometric_id { get; set; }
        public string? attendance_flag { get; set; }
        public string? branch_gid { get; set; }
        public string? branch_name { get; set; }
        public string? department_name { get; set; }
        public string? designation_name { get; set; }
        public string? employee_name { get; set; }
        public string? company_name { get; set; }
        public string? gender { get; set; }
        public string? entity_gid { get; set; }
        public string? entity_name { get; set; }
        public string? entity_flag { get; set; }
        public string? per_address1 { get; set; }
        public string? per_address2 { get; set; }
        public string? per_country_gid { get; set; }
        public string? per_country_name { get; set; }
        public string? per_state { get; set; }
        public string? per_city { get; set; }
        public string? per_postal_code { get; set; }
        public string? temp_address1 { get; set; }
        public string? temp_address2 { get; set; }
        public string? temp_country_gid { get; set; }
        public string? temp_country_name { get; set; }
        public string? temp_state { get; set; }
        public string? temp_city { get; set; }
        public string? temp_postal_code { get; set; }
        public string? role_name { get; set; }
        public string? role_gid { get; set; }
        public string? employee_reportingto_name { get; set; }
        public string? employee_reportingto { get; set; }
        public string? created_by { get; set; }
        public string? created_date { get; set; }
        public string? updated_by { get; set; }
        public string? updated_date { get; set; }
        public string? baselocation_gid { get; set; }
        public string? baselocation_name { get; set; }
        public string? user_access { get; set; }
        public string? employeereporting_to { get; set; }
        public string? marital_status { get; set; }
        public string? marital_status_gid { get; set; }
        public string? bloodgroup_name { get; set; }
        public string? bloodgroup_gid { get; set; }
        public string? joining_date { get; set; }
        public DateTime joiningdate { get; set; }
        public string? personal_phone_no { get; set; }
        public string? personal_emailid { get; set; }
        public string? remarks { get; set; }
        public string? relive_date { get; set; }
        public string? employee_status { get; set; }
        public string? hierarchy_level { get; set; }
        public string? reportingto { get; set; }
        public string? erp_id { get; set; }
        public string? employee_externalid { get; set; }
        public string? employee_entitychange_flag { get; set; }
        public string? subfunction_gid { get; set; }
        public string? subfunction_name { get; set; }
        public string? api_code { get; set; }

        public string? default_template { get; set; }

    }

    public class role_list : rolemaster
    {
        public List<rolemaster>? rolemaster { get; set; }

    }
    public class rolemaster
    {

        public string? role_gid { get; set; }
        public string? role_name { get; set; }

    }
    public class reportingto_list : reportingto
    {
        public List<reportingto>? reportingto { get; set; }
    }
    public class reportingto
    {
        public string? employee_gid { get; set; }
        public string? user_gid { get; set; }
        public string? user_code { get; set; }
        public string? employee_name { get; set; }
    }
    public class country_list : country
    {
        public List<country>? country { get; set; }
    }
    public class country
    {
        public string? country_gid { get; set; }
        public string? country_name { get; set; }
    }
    public class entity_list : entity
    {
        public List<entity>? entity { get; set; }
    }
    public class entity : result
    {
        public string? entity_gid { get; set; }
        public string? entity_name { get; set; }
    }

    public class export : result
    {
        public string? lspath { get; set; }
        public string? lsname { get; set; }
    }
    public class tasklist : result
    {

        public string? user_gid { get; set; }
        public string? employee_gid { get; set; }
        public string? approval_remarks { get; set; }
        public string? approval_type { get; set; }
        public string? task_remarks { get; set; }
        public string? task_name { get; set; }
        public string? task_gid { get; set; }
        public string? taskinitiate_gid { get; set; }
        public string? task_completeremarks { get; set; }
        public List<tasklists> tasklists { get; set; }
        public List<tasksummarylists> tasksummarylists { get; set; }
        public List<tasksummarylist> tasksummarylist { get; set; }
    }

    public class tasklists
    {
        public string? task_gid { get; set; }
        public string? task_name { get; set; }
        public string? task_remark { get; set; }
        public string? employee_gid { get; set; }
        public string? created_by { get; set; }
        public string? created_date { get; set; }
        public string? task_remarks { get; set; }
        public string? temptaskinitiate_gid { get; set; }

    }

    public class tasksummarylists
    {
        public string? task_gid { get; set; }
        public string? task_name { get; set; }
        public string? task_remark { get; set; }
        public string? employee_gid { get; set; }
        public string? created_by { get; set; }
        public string? created_date { get; set; }
        public string? task_remarks { get; set; }
        public string? temptaskinitiate_gid { get; set; }
        public string? employee { get; set; }
        public string? task_status { get; set; }
        public string? taskinitiate_flag { get; set; }
        public string? overallinitiate_flag { get; set; }

    }


    public class tasksummarylist
    {
        public string? task_gid { get; set; }
        public string? task_name { get; set; }
        public string? task_remark { get; set; }
        public string? employee_gid { get; set; }
        public string? created_by { get; set; }
        public string? created_date { get; set; }
        public string? task_remarks { get; set; }
        public string? temptaskinitiate_gid { get; set; }
        public string? taskinitiate_gid { get; set; }
        public string? taskinitiate_flag { get; set; }
        public string? employee_name { get; set; }
        public string? complete_flag { get; set; }
        public string? completed_date { get; set; }
        public string? completed_by { get; set; }
        public string? lsemployee_name { get; set; }
        public string? lsemployee_gid { get; set; }
        public string? task_completeremarks { get; set; }
        public string? api_code { get; set; }
    }
    public class MdlTaskList : result
    {
        public string? taskinitiate_gid { get; set; }
        public string? initiate_flag { get; set; }
        public string? employee_gid { get; set; }
        public string? complete_flag { get; set; }
        public List<tasklists> tasklists { get; set; }
        public List<tasksummarylists> tasksummarylists { get; set; }
        public List<tasksummarylist> tasksummarylist { get; set; }
    }
    public class countlist : result
    {
        public string? pending_count { get; set; }
        public string? completed_count { get; set; }
    }
    public class MdlDeactivation : result
    {

        public string? employeereporting_to { get; set; }
        public string? tempasset_status { get; set; }
        public string? asset_status { get; set; }
        public string? module_name { get; set; }
        public string? applicationapproval_gid { get; set; }
        public string? appcreditapproval_gid { get; set; }
        public string? ccmeeting2members_gid { get; set; }
        public string? cadgroupmanager_gid { get; set; }
        public string? cadgroupmembers_gid { get; set; }
        public string? creditops2maker_gid { get; set; }
        public string? creditops2checker_gid { get; set; }
        public string? creditmapping_gid { get; set; }
        public string? processtype_assign { get; set; }
        public string? submitbutton { get; set; }

        public string? agrapplicationapproval_gid { get; set; }
        public string? agrappcreditapproval_gid { get; set; }
        public string? agrccmeeting2members_gid { get; set; }
        public string? agrprocesstype_assign { get; set; }

        public string? auditmapping2employee_gid { get; set; }
        public string? multipleauditee_gid { get; set; }
        public string? auditcreation_gid { get; set; }

        public string? customerapproving_gid { get; set; }
        public string? campaign_gid { get; set; }
        public string? finalcampaign_gid { get; set; }

        public string? marketingcall_gid { get; set; }

        public string? inboundcall_gid { get; set; }

        public string? sacontactinstitution_gid { get; set; }
        public string? sacontact_gid { get; set; }

        public string? activedepartment2manager_gid { get; set; }
        public string? activedepartment2member_gid { get; set; }
        public string? supportteam2member_gid { get; set; }
        public string? requestapproval_gid { get; set; }
        public string? servicerequest_gid { get; set; }
        public string? ticketassign_gid { get; set; }
        public string? maildetails_gid { get; set; }
        public string? ccmembermaster_gid { get; set; }
        public string? agrcreditmapping_gid { get; set; }

        public string? productdeskmanager_gid { get; set; }
        public string? productdeskmember_gid { get; set; }


        public string? mstpmgapproval_gid { get; set; }
        public string? mstproductapproval_gid { get; set; }
        public string? appproductapproval_gid { get; set; }
        public string? warehouse2approval_gid { get; set; }


        public string? agrapplication_gid { get; set; }
        public string? application_gid { get; set; }

        public string? campaignapproving2employee_gid { get; set; }
        public string? appcustomerapproving_gid { get; set; }



        public string? makersacontactinstitution_gid { get; set; }
        public string? checkersacontactinstitution_gid { get; set; }
        public string? finalsacontactinstitution_gid { get; set; }
        public string? makersacontact_gid { get; set; }
        public string? checkersacontact_gid { get; set; }
        public string? finalsacontact_gid { get; set; }
        public string? email_gid { get; set; }



        public string? asset { get; set; }
        public string? samfin { get; set; }
        public string? samagro { get; set; }
        public string? audit { get; set; }
        public string? foundation { get; set; }
        public string? saonboarding { get; set; }
        public string? servicerequest { get; set; }


    }
    public class hrdoc_list : hrdoc
    {
        public List<hrdoc> hrdoc { get; set; }
        public string? pendingesign_count { get; set; }
        public string? completedesign_count { get; set; }
        public string? expiredesign_count { get; set; }
        public string? migration_flag { get; set; }
    }
    public class hrdoc : result
    {
        public string? employee_gid { get; set; }
        public string? hrdoc_id { get; set; }
        public string? hrdoc_name { get; set; }
        public string? hrdoc_path { get; set; }
        public string? created_by { get; set; }
        public string? created_date { get; set; }
        public string? hrdocument_gid { get; set; }
        public string? hrdocument_name { get; set; }
        public string? documentsentforsign_flag { get; set; }
        public string? esignexpiry_flag { get; set; }
        public string? documentsigned_flag { get; set; }
        public string? documentemployee_gid { get; set; }
        public string? documentemployee_name { get; set; }
        public string? document_proceededforesign { get; set; }
        public string? expire_on { get; set; }
        public string? documentsigned_date { get; set; }
        public string? lspath { get; set; }
        public string? lscloudpath { get; set; }
        public string? lsname { get; set; }

    }

    public class HRuploaddocument : result
    {
        public string? institution2form60documentupload_gid { get; set; }
        public string? institution2documentupload_gid { get; set; }
        public string? institution_gid { get; set; }
        public string?[] filename { get; set; }
        public string? filepath { get; set; }
        public string?[] compfilename { get; set; }
        public string? compfilepath { get; set; }
        public string?[] forwardfilename { get; set; }
        public string? forwardfilepath { get; set; }

        public string?[] doufilename { get; set; }
        public string? doufilepath { get; set; }
    }
    public class uploaddocument : result
    {
        public List<upload_list> upload_list { get; set; }
    }
    public class upload_list
    {
        public string? tmp_documentGid { get; set; }
        public string? document_name { get; set; }
        public string? document_path { get; set; }
        public string? created_date { get; set; }
        public string? created_by { get; set; }
    }

    public class employeename_list
    {
        public string? Name { get; set; }
        public string? service_flag { get; set; }
    }
}
