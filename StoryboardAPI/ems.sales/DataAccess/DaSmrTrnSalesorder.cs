using ems.sales.Models;
using ems.utilities.Functions;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Server;
using System.Diagnostics;
using System.Globalization;
using System.Configuration;
using System.IO;
using System.Runtime.Remoting;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;


namespace ems.sales.DataAccess
{
    public class DaSmrTrnSalesorder
    {
        private readonly dbconn objdbconn;
        private readonly cmnfunctions objcmnfunctions;
        private readonly IConfiguration objconfigurations;
        private readonly Fnazurestorage objFnazurestorage;

        string? msSQL, full_path, msINGetGID, msGetinGid, msSalesGid, msSQL1, msSQL2 = string.Empty;
        OdbcDataReader? objodbcDataReader, objodbcdatareader1, objodbcDataReader2;
        DataTable? dt_datatable;
        decimal lsmrp_price;
        double subtotal, lsdiscountpercentage, reCalTotalAmount, reCaldiscountAmount, reCalTaxAmount, taxAmount, totaltaxamount;
        string? lsrefno, lscompany_code, lstaxamount, customergid, lscustomername;
        string? msEmployeeGID, lsemployee_gid, lsentity_code, lspercentage,
            start_date, msgetlead2campaign_gid, end_date, lspercentage1, lsdesignation_code,
            lstaxname2, lsorder_type, taxgid1,lsproduct_type, lsproductgid1, lstaxname1,
            lsdiscountamount, lsprice, lstype1, lsproduct_price, mssalesorderGID, mssalesorderGID1,
            mscusconGetGID, lscustomer_name, msGetCustomergid, lscustomer_gid, msGetGid2, msGetGid3,
            lsCode, msPOGetGID, msGetGID, msGetGid, msGetGid1, msAccGetGID, msGetPrivilege_gid, msGetModule2employee_gid;
        int mnResult, mnResult1, mnResult2, mnResult3, mnResult4, mnResult5;
        string? employee_gid;
        string? lsCustomername, lscustomer_id, lscustomer_code;

        string? taxsegment_gid, taxsegment_name, tax_name, tax_gid, tax_gid2, tax_gid3, tax_name2, tax_name3,
            tax_name1, tax_percentage, tax_percent2, tax_percent3, tax_amount, tax_amount2, tax_amount3,
            mrp_price, cost_price, tax1, tax2, tax3, lsproductuom_gid, lsproductgid, lsproduct_name, lscustomerproduct_code, lsproductuom_name;
        double rreCalTotalAmount, lsbasic_amount = 0.00;
        string? FileExtension, FileExtensionname = "";

        public DaSmrTrnSalesorder(dbconn dbconn, cmnfunctions cmnfunctions, IConfiguration configuration,
            Fnazurestorage fnazurestorage)
        {
            objdbconn = dbconn;
            objcmnfunctions = cmnfunctions;
            objconfigurations = configuration;
            objFnazurestorage = fnazurestorage;
        }
        public void DaGetSmrTrnSalesordersummary(MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = " select currency_code from crm_trn_tcurrencyexchange where default_currency='Y'";
                string currency = objdbconn.GetExecuteScalar(msSQL);

                msSQL = " select distinct a.salesorder_gid,z.deliverybased_invoice,a.source_flag,a.file_path,a.file_name, a.so_referenceno1,a.customer_gid,a.mintsoftid," +
                        " DATE_FORMAT(a.salesorder_date, '%d-%m-%Y') as salesorder_date,c.user_firstname as created_by,a.so_type,a.currency_code," +
                        " a.customer_contact_person, a.salesorder_status,a.currency_code,s.source_name,d.customer_code,i.branch_name, " +
                        " format(a.Grandtotal,2) as Grandtotal, concat(k.user_firstname, ' ', k.user_lastname) AS salesperson_name,  " +
                        "  a.customer_name AS customerdetails," +
                        " case when a.currency_code = '" + currency + "' then concat_ws('/',d.customer_id ,d.customer_name) " +
                        " when a.currency_code is null then concat('/',d.customer_id ,d.customer_name) " +
                        " when a.currency_code is not null and a.currency_code <> '" + currency + "' then concat_ws('/',d.customer_id ,d.customer_name) end as customer_name, " +
                        " case when a.customer_email is null then concat_ws('/',e.customercontact_name,e.mobile,e.email) " +
                        " when a.customer_email is not null then concat_ws( '/',a.customer_contact_person,a.customer_mobile,a.customer_email) end as contact,a.invoice_flag,(select mintsoft_flag from adm_mst_tcompany limit 1) as mintsoft_flag  " +
                        " from smr_trn_tsalesorder a " +
                        " left join crm_mst_tcustomer d on a.customer_gid=d.customer_gid" +
                        "  left join smr_trn_tsalesorderdtl  x on a.salesorder_gid=x.salesorder_gid" +
                        " left join crm_mst_tcustomercontact e on d.customer_gid=e.customer_gid" +
                        " left join hrm_mst_temployee b on b.employee_gid=a.created_by " +
                        " left join crm_trn_tcurrencyexchange h on a.currency_code = h.currency_code " +
                        " left join adm_mst_tuser c on b.user_gid= c.user_gid " +
                        " left join hrm_mst_tbranch i on a.branch_gid= i.branch_gid " +
                        " LEFT JOIN adm_mst_tuser k ON a.salesperson_gid = k.user_gid " +
                        " left join crm_trn_tleadbank l on l.customer_gid=a.customer_gid " +
                        " left join crm_mst_tsource s on s.source_gid=l.source_gid " +
                        " left join adm_mst_Tcompany  z on 1=1 " +
                        "  where 1=1 and a.salesorder_status not in('Cancelled','SO Amended') order by a.created_date desc";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesorder_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new salesorder_list
                        {
                            customer_gid = dt["customer_gid"].ToString(),
                            salesorder_gid = dt["salesorder_gid"].ToString(),
                            salesorder_date = dt["salesorder_date"].ToString(),
                            so_referenceno1 = dt["so_referenceno1"].ToString(),
                            customer_name = dt["customer_name"].ToString(),
                            branch_name = dt["branch_name"].ToString(),
                            contact = dt["contact"].ToString(),
                            so_type = dt["so_type"].ToString(),
                            file_path = dt["file_path"].ToString(),
                            Grandtotal = dt["Grandtotal"].ToString(),
                            user_firstname = dt["created_by"].ToString(),
                            salesorder_status = dt["salesorder_status"].ToString(),
                            mintsoftid = dt["mintsoftid"].ToString(),
                            salesperson_name = dt["salesperson_name"].ToString(),
                            customer_code = dt["customer_code"].ToString(),
                            customerdetails = dt["customerdetails"].ToString(),
                            source_flag = dt["source_flag"].ToString(),
                            file_name = dt["file_name"].ToString(),
                            mintsoft_flag = dt["mintsoft_flag"].ToString(),
                            deliverybasedinvoice_flag = dt["deliverybased_invoice"].ToString(),

                        });
                        values.salesorder_list = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Order Summary !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
               values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetSoDraftsSummary(MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = " select currency_code from crm_trn_tcurrencyexchange where default_currency='Y'";
                string currency = objdbconn.GetExecuteScalar(msSQL);

                msSQL = " select distinct a.salesorder_gid,a.source_flag,a.file_path,a.file_name, a.so_referenceno1,a.customer_gid,a.mintsoftid," +
                        " DATE_FORMAT(a.salesorder_date, '%d-%m-%Y') as salesorder_date,c.user_firstname as created_by,a.so_type,a.currency_code," +
                        " a.customer_contact_person, a.salesorder_status,a.currency_code,s.source_name,d.customer_code,i.branch_name, " +
                        " format(a.Grandtotal,2) as Grandtotal, concat(k.user_firstname, ' ', k.user_lastname) AS salesperson_name,  " +
                        "  a.customer_name AS customerdetails," +
                        " case when a.currency_code = '" + currency + "' then concat_ws('/',d.customer_id ,d.customer_name) " +
                        " when a.currency_code is null then concat('/',d.customer_id ,d.customer_name) " +
                        " when a.currency_code is not null and a.currency_code <> '" + currency + "' then concat_ws('/',d.customer_id ,d.customer_name) end as customer_name, " +
                        " case when a.customer_email is null then concat_ws('/',e.customercontact_name,e.mobile,e.email) " +
                        " when a.customer_email is not null then concat_ws( '/',a.customer_contact_person,a.customer_mobile,a.customer_email) end as contact,a.invoice_flag,(select mintsoft_flag from adm_mst_tcompany limit 1) as mintsoft_flag  " +
                        " from smr_tmp_tsalesorderdrafts a " +
                        " left join crm_mst_tcustomer d on a.customer_gid=d.customer_gid" +
                        "  left join smr_tmp_tsalesorderdtldrafts  x on a.salesorder_gid=x.salesorder_gid" +
                        " left join crm_mst_tcustomercontact e on d.customer_gid=e.customer_gid" +
                        " left join hrm_mst_temployee b on b.employee_gid=a.created_by " +
                        " left join crm_trn_tcurrencyexchange h on a.currency_code = h.currency_code " +
                        " left join adm_mst_tuser c on b.user_gid= c.user_gid " +
                        " left join hrm_mst_tbranch i on a.branch_gid= i.branch_gid " +
                        " LEFT JOIN adm_mst_tuser k ON a.salesperson_gid = k.user_gid " +
                        " left join crm_trn_tleadbank l on l.customer_gid=a.customer_gid " +
                        " left join crm_mst_tsource s on s.source_gid=l.source_gid " +
                        "  where 1=1 and a.salesorder_status not in('Cancelled','SO Amended') order by a.created_date desc";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesorder_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new salesorder_list
                        {
                            customer_gid = dt["customer_gid"].ToString(),
                            salesorder_gid = dt["salesorder_gid"].ToString(),
                            salesorder_date = dt["salesorder_date"].ToString(),
                            so_referenceno1 = dt["so_referenceno1"].ToString(),
                            customer_name = dt["customer_name"].ToString(),
                            branch_name = dt["branch_name"].ToString(),
                            contact = dt["contact"].ToString(),
                            so_type = dt["so_type"].ToString(),
                            file_path = dt["file_path"].ToString(),
                            Grandtotal = dt["Grandtotal"].ToString(),
                            user_firstname = dt["created_by"].ToString(),
                            salesorder_status = dt["salesorder_status"].ToString(),
                            mintsoftid = dt["mintsoftid"].ToString(),
                            salesperson_name = dt["salesperson_name"].ToString(),
                            customer_code = dt["customer_code"].ToString(),
                            customerdetails = dt["customerdetails"].ToString(),
                            source_flag = dt["source_flag"].ToString(),
                            file_name = dt["file_name"].ToString(),
                            mintsoft_flag = dt["mintsoft_flag"].ToString(),

                        });
                        values.salesorder_list = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Order Summary !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
               values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void checkinvoice(string salesorder_gid, MdlSmrTrnSalesorder values)
        {
            msSQL = "select * from rbl_trn_tinvoice where invoice_reference = '" + salesorder_gid + "'";
            objodbcDataReader = objdbconn.GetDataReader(msSQL);
            if (objodbcDataReader.HasRows)
            {
                values.status = false;
                values.message = "Invoice has been raised already for this salesorder";
            }
            else
            {
                values.status = true;
            }
            objodbcDataReader.Close();
        }
        public void daGetsalesordersixmonthschart(MdlSmrTrnSalesorder values)
        {


            msSQL = " select DATE_FORMAT(salesorder_date, '%b-%Y')  as salesorder_date,substring(date_format(a.salesorder_date,'%M'),1,3)as month,a.salesorder_gid,year(a.salesorder_date) as year, " +
             " format(round(sum(a.grandtotal),2),2)as amount,round(sum(a.grandtotal),2) as amount1,count(a.salesorder_gid)as ordercount ,  date_format(salesorder_date,'%M/%Y') as month_wise " +
             " from smr_trn_tsalesorder a   " +
             " where a.salesorder_date > date_add(now(), interval-6 month) and a.salesorder_date<=date(now())   " +
             " and a.salesorder_status not in('SO Amended','Cancelled','Rejected') group by date_format(a.salesorder_date,'%M') order by a.salesorder_date desc  ";

            DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

            var salesorderlastsixmonths_list = new List<salesorderlastsixmonths_list>();
            if (dt_datatable.Rows.Count != 0)
            {
                foreach (DataRow dt in  dt_datatable.Rows)
                {
                    salesorderlastsixmonths_list.Add(new salesorderlastsixmonths_list
                    {
                        salesorder_date = (dt["salesorder_date"].ToString()),
                        months = (dt["month"].ToString()),
                        orderamount = (dt["amount1"].ToString()),

                    });
                    values.salesorderlastsixmonths_list = salesorderlastsixmonths_list;
                }

            }

            msSQL = "select COUNT(CASE WHEN a.salesorder_status = 'Invoice Raised' THEN 1 END) AS invoice_count, " +
                    "  COUNT(a.salesorder_gid) AS approved_count " +
                    "  FROM  " +
                      " smr_trn_tsalesorder a " +
                       " WHERE  a.salesorder_date > DATE_ADD(NOW(), INTERVAL -6 MONTH)  AND a.salesorder_date <= DATE(NOW())";
            objodbcDataReader = objdbconn.GetDataReader(msSQL);
            if (objodbcDataReader.HasRows)
            {

                values.ordertoinvoicecount = objodbcDataReader["invoice_count"].ToString();
                values.ordercount = objodbcDataReader["approved_count"].ToString();



            }
            objodbcDataReader.Close();

        }
        public void DaGetSmrTrnSalesorder2invoicesummary(MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = " select currency_code from crm_trn_tcurrencyexchange where default_currency='Y'";
                string currency = objdbconn.GetExecuteScalar(msSQL);
                
                msSQL = " select distinct a.salesorder_gid,a.source_flag, a.so_referenceno1,a.customer_gid,a.mintsoftid," +
                        " DATE_FORMAT(a.salesorder_date, '%d-%m-%Y') as salesorder_date,c.user_firstname as created_by,a.so_type,a.currency_code," +
                        " a.customer_contact_person, a.salesorder_status,a.currency_code,s.source_name,d.customer_code,i.branch_name, " +
                        " format(a.Grandtotal,2) as Grandtotal, concat(k.user_firstname, ' ', k.user_lastname) AS salesperson_name,  " +
                        " CONCAT(d.customer_id, ' / ', d.customer_name) AS customerdetails," +
                        " case when a.currency_code = '" + currency + "' then a.customer_name " +
                        " when a.currency_code is null then a.customer_name " +
                        " when a.currency_code is not null and a.currency_code <> '" + currency + "' then (a.customer_name) end as customer_name, " +
                        " case when a.customer_email is null then concat(e.customercontact_name,'/',e.mobile,'/',e.email) " +
                        " when a.customer_email is not null then concat(a.customer_contact_person,' / ',a.customer_mobile,' / ',a.customer_email) end as contact,a.invoice_flag " +
                        " from smr_trn_tsalesorder a " +
                        " left join crm_mst_tcustomer d on a.customer_gid=d.customer_gid" +
                        "  left join smr_trn_tsalesorderdtl  x on a.salesorder_gid=x.salesorder_gid" +
                        " left join crm_mst_tcustomercontact e on d.customer_gid=e.customer_gid" +
                        " left join hrm_mst_temployee b on b.employee_gid=a.created_by " +
                        " left join crm_trn_tcurrencyexchange h on a.currency_code = h.currency_code " +
                        " left join adm_mst_tuser c on b.user_gid= c.user_gid " +
                        " left join hrm_mst_tbranch i on a.branch_gid= i.branch_gid " +
                        " LEFT JOIN adm_mst_tuser k ON a.salesperson_gid = k.user_gid " +
                        " left join crm_trn_tleadbank l on l.customer_gid=a.customer_gid " +
                        " left join crm_mst_tsource s on s.source_gid=l.source_gid " +
                        "  where x.qty_executed <> x.qty_quoted and so_type <> 'Services' and a.salesorder_status not in('Cancelled','SO Amended') order by a.salesorder_gid desc";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesorder_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new salesorder_list
                        {
                            customer_gid = dt["customer_gid"].ToString(),
                            salesorder_gid = dt["salesorder_gid"].ToString(),
                            salesorder_date = dt["salesorder_date"].ToString(),
                            so_referenceno1 = dt["so_referenceno1"].ToString(),
                            customer_name = dt["customer_name"].ToString(),
                            branch_name = dt["branch_name"].ToString(),
                            contact = dt["contact"].ToString(),
                            so_type = dt["so_type"].ToString(),
                            Grandtotal = dt["Grandtotal"].ToString(),
                            user_firstname = dt["created_by"].ToString(),
                            salesorder_status = dt["salesorder_status"].ToString(),
                            mintsoftid = dt["mintsoftid"].ToString(),
                            salesperson_name = dt["salesperson_name"].ToString(),
                            customer_code = dt["customer_code"].ToString(),
                            customerdetails = dt["customerdetails"].ToString(),
                            source_flag = dt["source_flag"].ToString(),

                        });
                        values.salesorder_list = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Order Summary !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
               values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetSmrTrnSalesorder2invoiceServicessummary(MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = " select currency_code from crm_trn_tcurrencyexchange where default_currency='Y'";
                string currency = objdbconn.GetExecuteScalar(msSQL);

                msSQL = " select distinct a.salesorder_gid,a.source_flag, a.so_referenceno1,a.customer_gid,a.mintsoftid," +
                        " DATE_FORMAT(a.salesorder_date, '%d-%m-%Y') as salesorder_date,c.user_firstname as created_by,a.so_type,a.currency_code," +
                        " a.customer_contact_person, a.salesorder_status,a.currency_code,s.source_name,d.customer_code,i.branch_name, " +
                        " format(a.Grandtotal,2) as Grandtotal, concat(k.user_firstname, ' ', k.user_lastname) AS salesperson_name,  " +
                        " CONCAT(d.customer_id, ' / ', d.customer_name) AS customerdetails," +
                        " case when a.currency_code = '" + currency + "' then a.customer_name " +
                        " when a.currency_code is null then a.customer_name " +
                        " when a.currency_code is not null and a.currency_code <> '" + currency + "' then (a.customer_name) end as customer_name, " +
                        " case when a.customer_email is null then concat(e.customercontact_name,'/',e.mobile,'/',e.email) " +
                        " when a.customer_email is not null then concat(a.customer_contact_person,' / ',a.customer_mobile,' / ',a.customer_email) end as contact,a.invoice_flag " +
                        " from smr_trn_tsalesorder a " +
                        " left join crm_mst_tcustomer d on a.customer_gid=d.customer_gid" +
                        "  left join smr_trn_tsalesorderdtl  x on a.salesorder_gid=x.salesorder_gid" +
                        " left join crm_mst_tcustomercontact e on d.customer_gid=e.customer_gid" +
                        " left join hrm_mst_temployee b on b.employee_gid=a.created_by " +
                        " left join crm_trn_tcurrencyexchange h on a.currency_code = h.currency_code " +
                        " left join adm_mst_tuser c on b.user_gid= c.user_gid " +
                        " left join hrm_mst_tbranch i on a.branch_gid= i.branch_gid " +
                        " LEFT JOIN adm_mst_tuser k ON a.salesperson_gid = k.user_gid " +
                        " left join crm_trn_tleadbank l on l.customer_gid=a.customer_gid " +
                        " left join crm_mst_tsource s on s.source_gid=l.source_gid " +
                        "  where x.qty_executed <> x.qty_quoted and so_type = 'Services' and a.salesorder_status not in('Cancelled','SO Amended') order by a.salesorder_gid desc";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesorder_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new salesorder_list
                        {
                            customer_gid = dt["customer_gid"].ToString(),
                            salesorder_gid = dt["salesorder_gid"].ToString(),
                            salesorder_date = dt["salesorder_date"].ToString(),
                            so_referenceno1 = dt["so_referenceno1"].ToString(),
                            customer_name = dt["customer_name"].ToString(),
                            branch_name = dt["branch_name"].ToString(),
                            contact = dt["contact"].ToString(),
                            so_type = dt["so_type"].ToString(),
                            Grandtotal = dt["Grandtotal"].ToString(),
                            user_firstname = dt["created_by"].ToString(),
                            salesorder_status = dt["salesorder_status"].ToString(),
                            mintsoftid = dt["mintsoftid"].ToString(),
                            salesperson_name = dt["salesperson_name"].ToString(),
                            customer_code = dt["customer_code"].ToString(),
                            customerdetails = dt["customerdetails"].ToString(),
                            source_flag = dt["source_flag"].ToString(),

                        });
                        values.salesorder_list = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Order Summary !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
               values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetSmrTrnSalesordercount(MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = "SELECT " +
                        " (SELECT COUNT(*) FROM smr_trn_tsalesorder where source_flag='I') as internalcount, " +
                        " (SELECT COUNT(*) FROM smr_trn_tsalesorder WHERE source_flag='W' ) as whatappcount, " +
                        " (SELECT COUNT(*) FROM smr_trn_tsalesorder  WHERE source_flag='S') as shopifycount";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesordertype_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new salesordertype_list
                        {

                            whatappcount = dt["whatappcount"].ToString(),
                            shopifycount = dt["shopifycount"].ToString(),
                            internalcount = dt["internalcount"].ToString(),

                        });
                        values.salesordertype_list = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Order Summary !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
               values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetViewsalesorderSummary(string salesorder_gid, MdlSmrTrnSalesorder values)

        {

            try

            {

                msSQL = " SELECT distinct a.salesorder_gid,a.currency_gid, a.currency_code, a.customerbranch_gid, a.exchange_rate, " +

                    " DATE_FORMAT(a.salesorder_date, '%d-%m-%Y') AS salesorder_date, a.salesperson_gid, " +

                    " concat(g.user_code, ' / ', g.user_firstname, ' ', g.user_lastname) AS salesperson_name, " +

                    " FORMAT(a.Grandtotal, 2) AS Grandtotal, a.termsandconditions, FORMAT(a.addon_charge, 2) AS addon_charge, " +

                    " FORMAT(a.additional_discount_l, 2) AS additional_discount, a.payment_days, FORMAT(a.tax_amount, 2) AS tax_amount, " +

                    " a.delivery_days, a.so_referenceno1, a.payment_terms, a.freight_terms, " +

                    " FORMAT(a.roundoff, 2) AS roundoff, a.so_remarks, a.shipping_to, " +

                    " a.customer_address, n.customer_name,  a.customer_contact_person AS customer_contact_person, " +

                    " case when a.start_date = '0000-00-00' then '' else DATE_FORMAT(a.start_date, '%d-%m-%Y') end AS start_date,case when a.end_date = '0000-00-00' then '' else DATE_FORMAT(a.end_date, '%d-%m-%Y') end AS end_date, " +

                    " a.termsandconditions, m.mobile, m.email,n.gst_number ,e.branch_name, " +

                    " FORMAT(a.total_amount, 2) AS total_amount, FORMAT(a.freight_charges, 2) AS freight_charges, " +

                    " FORMAT(a.packing_charges, 2) AS packing_charges,format(a.total_price, 2) as total_price,a.tax_name, FORMAT(a.buyback_charges, 2) AS buyback_charges, " +

                    " FORMAT(a.insurance_charges, 2) AS insurance_charges,CASE  WHEN a.renewal_flag = 'Y' THEN 'Yes' WHEN a.renewal_flag = 'N' THEN 'No'  END AS renewal_status ," +
                    " CASE   WHEN a.renewal_flag = 'Y' THEN DATE_FORMAT(r.renewal_date, '%d-%m-%Y')  ELSE NULL END AS renewal_date,CASE WHEN a.renewal_flag = 'Y' THEN r.frequency_term ELSE NULL END AS frequency_term FROM smr_trn_tsalesorder a " +

                    " LEFT JOIN smr_trn_tsalesorderdtl d ON d.salesorder_gid = a.salesorder_gid " +

                    " LEFT JOIN hrm_mst_tbranch e ON e.branch_gid = a.branch_gid " +

                    " LEFT JOIN acp_mst_ttax f ON f.tax_gid = a.tax_gid " +

                    " LEFT JOIN adm_mst_tuser g ON g.user_gid = a.salesperson_gid " +

                    " LEFT JOIN crm_trn_tcurrencyexchange h ON a.currency_gid = h.currencyexchange_gid " +

                    " LEFT JOIN crm_mst_tcustomercontact m ON m.customer_gid = a.customer_gid " +

                    " LEFT JOIN crm_mst_tcustomer n ON n.customer_gid = a.customer_gid " +

                    " LEFT JOIN  crm_trn_trenewal r ON a.salesorder_gid = r.salesorder_gid " +

                    " WHERE a.salesorder_gid = '" + salesorder_gid + "' GROUP BY a.salesorder_gid, d.product_gid ORDER BY a.salesorder_gid ASC";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                var getModuleList = new List<postsalesorder_list>();

                if (dt_datatable.Rows.Count != 0)

                {

                    foreach (DataRow dt in  dt_datatable.Rows)

                    {

                        getModuleList.Add(new postsalesorder_list

                        {


                            salesorder_gid = dt["salesorder_gid"].ToString(),

                            tax_name = dt["tax_name"].ToString(),
                            renewal_status = dt["renewal_status"].ToString(),
                            frequency_term = dt["frequency_term"].ToString(),
                            renewal_date = dt["renewal_date"].ToString(),

                            salesorder_date = dt["salesorder_date"].ToString(),

                            customer_name = dt["customer_name"].ToString(),

                            branch_name = dt["branch_name"].ToString(),

                            customer_contact_person = dt["customer_contact_person"].ToString(),

                            customer_email = dt["email"].ToString(),

                            customer_mobile = dt["mobile"].ToString(),

                            gst_number = dt["gst_number"].ToString(),

                            customer_address = dt["customer_address"].ToString(),

                            start_date = dt["start_date"].ToString(),

                            end_date = dt["end_date"].ToString(),

                            currency_code = dt["currency_code"].ToString(),

                            exchange_rate = dt["exchange_rate"].ToString(),

                            freight_terms = dt["freight_terms"].ToString(),

                            payment_terms = dt["payment_terms"].ToString(),

                            payment_days = dt["payment_days"].ToString(),

                            so_referencenumber = dt["so_referenceno1"].ToString(),

                            shipping_to = dt["shipping_to"].ToString(),

                            delivery_days = dt["delivery_days"].ToString(),

                            so_remarks = dt["so_remarks"].ToString(),

                            salesperson_name = dt["salesperson_name"].ToString(),

                            addon_charge = dt["addon_charge"].ToString(),

                            additional_discount = dt["additional_discount"].ToString(),

                            freight_charges = dt["freight_charges"].ToString(),

                            buyback_charges = dt["buyback_charges"].ToString(),

                            packing_charges = dt["packing_charges"].ToString(),

                            insurance_charges = dt["insurance_charges"].ToString(),

                            roundoff = dt["roundoff"].ToString(),

                            Grandtotal = dt["Grandtotal"].ToString(),

                            termsandconditions = dt["termsandconditions"].ToString(),

                            total_price = dt["total_price"].ToString(),

                            price = dt["total_price"].ToString(),

                            total_amount = dt["total_amount"].ToString(),

                            tax_amount = dt["tax_amount"].ToString(),


                        });

                        values.postsalesorder_list = getModuleList;

                    }

                }

                dt_datatable.Dispose();

            }

            catch (Exception ex)

            {

                values.message = "Exception occured while loading Sales Order Summary !";

                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +

                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +

                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }

        }
        public void DaGetViewsalesorderdetails(string salesorder_gid, string customer_gid, MdlSmrTrnSalesorder values)

        {

            try

            {

                msSQL = " select d.product_gid,i.productgroup_name,d.product_remarks,d.product_name,d.salesorderdtl_gid,d.product_code,d.uom_name,d.qty_quoted,d.margin_amount,d.margin_percentage,d.discount_percentage, d.discount_amount," +
                    " FORMAT(d.product_price, 2) AS product_price ,d.tax_name,format(d.tax_amount, 2) as tax_amount,format(d.tax_amount2, 2) as tax_amount2,FORMAT(d.price, 2) AS price," +
                    " e.tax_prefix as tax_prefix1 ,l.tax_prefix as tax_prefix2 " +
                    " FROM smr_trn_tsalesorder a LEFT JOIN smr_trn_tsalesorderdtl d ON d.salesorder_gid = a.salesorder_gid " +
                    " LEFT JOIN acp_mst_ttax e ON e.tax_gid = d.tax1_gid" +
                    " LEFT JOIN acp_mst_ttax l ON l.tax_gid = d.tax2_gid " +
                    " LEFT JOIN pmr_mst_tproductgroup i ON i.productgroup_gid = d.productgroup_gid " +
                    " WHERE a.salesorder_gid = '" + salesorder_gid + "' group by d.salesorderdtl_gid ";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                var getModuleList = new List<postsalesorderdetails_list>();

                var getGetTaxSegmentList = new List<GetTaxSegmentListorder>();

                if (dt_datatable.Rows.Count != 0)

                {

                    foreach (DataRow dt in  dt_datatable.Rows)

                    {

                        getModuleList.Add(new postsalesorderdetails_list

                        {

                            product_code = dt["product_code"].ToString(),

                            product_gid = dt["product_gid"].ToString(),

                            product_name = dt["product_name"].ToString(),

                            productgroup_name = dt["productgroup_name"].ToString(),

                            product_remarks = dt["product_remarks"].ToString(),

                            uom_name = dt["uom_name"].ToString(),

                            tax_prefix = dt["tax_prefix1"].ToString(),
                            tax_prefix2 = dt["tax_prefix2"].ToString(),

                            qty_quoted = dt["qty_quoted"].ToString(),

                            price = dt["price"].ToString(),

                            product_price = dt["product_price"].ToString(),

                            margin_percentage = dt["margin_percentage"].ToString(),

                            tax_name = dt["tax_name"].ToString(),

                            tax_amount = dt["tax_amount"].ToString(),
                            tax_amount2 = dt["tax_amount2"].ToString(),

                            margin_amount = dt["margin_amount"].ToString(),

                            discount_amount = dt["discount_amount"].ToString(),

                            discount_percentage = dt["discount_percentage"].ToString(),

                        });

                        if (!string.IsNullOrEmpty(customer_gid) && customer_gid != "undefined" && customer_gid != "0")

                        {

                            // Query tax segment details based on product_gid

                            string product_gid = dt["product_gid"].ToString();

                            DataTable taxSegmentDataTable = GetTaxDetailsForProduct(product_gid, customer_gid);

                            // Add tax segment details to the list

                            foreach (DataRow taxSegmentRow in taxSegmentDataTable.Rows)

                            {

                                getGetTaxSegmentList.Add(new GetTaxSegmentListorder

                                {

                                    taxsegment_gid = taxSegmentRow["taxsegment_gid"].ToString(),

                                    product_gid = taxSegmentRow["product_gid"].ToString(),

                                    taxsegment_name = taxSegmentRow["taxsegment_name"].ToString(),

                                    tax_name = taxSegmentRow["tax_name"].ToString(),

                                    tax_percentage = taxSegmentRow["tax_percentage"].ToString(),

                                    tax_gid = taxSegmentRow["tax_gid"].ToString(),

                                    mrp_price = taxSegmentRow["mrp_price"].ToString(),

                                    cost_price = taxSegmentRow["cost_price"].ToString(),

                                    tax_amount = taxSegmentRow["tax_amount"].ToString(),

                                });

                            }

                        }

                        values.postsalesorderdetails_list = getModuleList;

                        values.GetTaxSegmentListorder = getGetTaxSegmentList;

                    }

                }

                dt_datatable.Dispose();

            }

            catch (Exception ex)

            {

                values.message = "Exception occured while loading Sales Order Summary !";

                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +

                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +

                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }

        }
        public void DaGetEditsalesorderSummary(string salesorder_gid, string employee_gid, MdlSmrTrnSalesorder values)

        {

            try

            {
                msSQL = " delete from smr_tmp_ttmpsalesorderdtl where " +
                       " employee_gid = '" + employee_gid + "' ";

                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                if (mnResult == 1)
                {
                    msSQL = "select d.product_gid,e.taxsegment_gid,i.productgroup_name,d.taxsegmenttax_gid, i.productgroup_gid,d.product_remarks,d.uom_gid,d.product_name,d.salesorderdtl_gid,d.product_code,d.uom_name,d.qty_quoted,d.margin_amount,d.margin_percentage,d.discount_percentage, " +
                           "  d.discount_amount,d.product_price ,d.tax_name,d.tax_amount,price,e.tax_prefix, " +
                           " d.tax1_gid,d.tax2_gid,d.tax3_gid,d.tax_name2,d.tax_name3,d.tax_amount2,d.tax_amount3," +
                           " d.tax_percentage,d.tax_percentage2,d.tax_percentage3,d.product_remarks" +
                           " FROM smr_trn_tsalesorder a " +
                           " LEFT JOIN smr_trn_tsalesorderdtl d ON d.salesorder_gid = a.salesorder_gid " +
                           " LEFT JOIN acp_mst_ttax e ON e.taxsegment_gid = d.taxsegment_gid " +
                           " LEFT JOIN pmr_mst_tproductgroup i ON i.productgroup_gid = d.productgroup_gid" +
                           " WHERE a.salesorder_gid = '" + salesorder_gid + "' group by  d.salesorderdtl_gid order by d.salesorderdtl_gid  desc";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                    if (dt_datatable.Rows.Count != 0)

                    {

                        foreach (DataRow dt in  dt_datatable.Rows)

                        {
                            msGetGid = objcmnfunctions.GetMasterGID("VSDT");

                            string display_field = dt["product_remarks"].ToString();

                            msSQL = " insert into smr_tmp_tsalesorderdtl( " +
                                     " tmpsalesorderdtl_gid," +
                                     " salesorder_gid," +
                                     " employee_gid," +
                                     " product_gid," +
                                     " product_code," +
                                     " product_name," +
                                     " productgroup_gid," +
                                     " product_price," +
                                     " qty_quoted," +
                                     " uom_gid," +
                                     " uom_name," +
                                     " price," +
                                     " order_type," +
                                     " tax_rate, " +
                                     " taxsegment_gid, " +
                                    " taxsegmenttax_gid, " +
                                    " tax1_gid, " +
                                    " tax2_gid, " +
                                    " tax3_gid, " +
                                    " tax_name, " +
                                    " tax_name2, " +
                                    " tax_name3, " +
                                    " tax_percentage, " +
                                    " tax_percentage2, " +
                                    " tax_percentage3, " +
                                    " tax_amount, " +
                                    " tax_amount2, " +
                                    " tax_amount3, " +
                                     " discount_amount, " +
                                     " product_remarks, " +
                                     " discount_percentage" +
                                     ")values(" +
                                     "'" + msGetGid + "'," +
                                     "'" + salesorder_gid + "'," +
                                     "'" + employee_gid + "'," +
                                     "'" + dt["product_gid"].ToString() + "'," +
                                     "'" + dt["product_code"].ToString() + "'," +
                                     "'" + dt["product_name"].ToString() + "'," +
                                     "'" + dt["productgroup_gid"].ToString() + "'," +
                                     "'" + dt["product_price"].ToString() + "'," +
                                     "'" + dt["qty_quoted"].ToString() + "'," +
                                     "'" + dt["uom_gid"].ToString() + "'," +
                                     "'" + dt["uom_name"].ToString() + "'," +
                                     "'" + dt["price"].ToString() + "'," +
                                     "'Sales'," +
                                     "'" + dt["tax_amount"].ToString() + "'," +
                                     "'" + dt["taxsegment_gid"].ToString() + "'," +
                                     "'" + dt["taxsegmenttax_gid"].ToString() + "'," +
                                     "'" + dt["tax1_gid"].ToString() + "'," +
                                     "'" + dt["tax2_gid"].ToString() + "'," +
                                     "'" + dt["tax3_gid"].ToString() + "'," +
                                     "'" + dt["tax_name"].ToString() + "'," +
                                     "'" + dt["tax_name2"].ToString() + "'," +
                                     "'" + dt["tax_name3"].ToString() + "',";
                            if (dt["tax_percentage"].ToString() == "" || dt["tax_percentage"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_percentage"].ToString() + "', ";
                            }
                            if (dt["tax_percentage2"].ToString() == "" || dt["tax_percentage2"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_percentage2"].ToString() + "', ";
                            }
                            if (dt["tax_percentage3"].ToString() == "" || dt["tax_percentage3"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_percentage3"].ToString() + "', ";
                            }
                            if (dt["tax_amount"].ToString() == "" || dt["tax_amount"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_amount"].ToString() + "', ";
                            }
                            if (dt["tax_amount2"].ToString() == "" || dt["tax_amount2"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_amount2"].ToString() + "', ";
                            }
                            if (dt["tax_amount3"].ToString() == "" || dt["tax_amount3"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_amount3"].ToString() + "', ";
                            }

                            msSQL +=
                               "'" + dt["discount_amount"].ToString() + "'," +
                               "'" + display_field.Replace("'", "\\\'") + "'," +
                               "'" + dt["discount_percentage"].ToString() + "')";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                        }
                    }

                    msSQL = " SELECT distinct a.salesorder_gid,a.so_referenceno1,a.customer_gid,g.user_gid,a.currency_gid, h.currency_code, a.customerbranch_gid, a.exchange_rate, " +

                    " DATE_FORMAT(a.salesorder_date, '%d-%m-%Y') AS salesorder_date, a.salesperson_gid, " +

                    " concat(g.user_code, ' / ', g.user_firstname, ' ', g.user_lastname) AS salesperson_name, " +

                    "format( a.Grandtotal,2) AS Grandtotal, a.termsandconditions, a.addon_charge AS addon_charge, " +

                    " a.additional_discount_l AS additional_discount, a.payment_days, a.tax_amount AS tax_amount, " +

                    " a.delivery_days, a.so_referenceno1, a.payment_terms, a.freight_terms, " +

                    " concat(m.address1,'   ',m.city,'   ',m.state) as address1,ifnull(m.address2,'') as address2," +

                    " a.roundoff AS roundoff, a.so_remarks, a.shipping_to, a.file_path , a.file_name, " +

                    " a.customer_address, a.customer_name,  a.customer_contact_person AS customer_contact_person, " +

                    " case when a.start_date = '0000-00-00' then '' else DATE_FORMAT(a.start_date, '%d-%m-%Y') end AS start_date,case when a.end_date = '0000-00-00' then '' else DATE_FORMAT(a.end_date, '%d-%m-%Y') end AS end_date, " +

                    " a.termsandconditions, m.mobile, m.email,n.gst_number ,e.branch_name, " +

                    " FORMAT(a.total_amount, 2) AS total_amount, a.freight_charges AS freight_charges, " +

                    " FORMAT(a.packing_charges, 2) AS packing_charges,format(a.total_price, 2) as total_price,a.tax_name, FORMAT(a.buyback_charges, 2) AS buyback_charges, " +

                    " FORMAT(a.insurance_charges, 2) AS insurance_charges,a.renewal_flag," +
                    " CASE   WHEN a.renewal_flag = 'Y' THEN DATE_FORMAT(r.renewal_date, '%d-%m-%Y') ELSE NULL END AS renewal_date,CASE WHEN a.renewal_flag = 'Y' THEN r.frequency_term ELSE NULL END AS frequency_term FROM smr_trn_tsalesorder a " +

                    " LEFT JOIN smr_trn_tsalesorderdtl d ON d.salesorder_gid = a.salesorder_gid " +

                    " LEFT JOIN hrm_mst_tbranch e ON e.branch_gid = a.branch_gid " +

                    " LEFT JOIN acp_mst_ttax f ON f.tax_gid = a.tax_gid " +

                    " LEFT JOIN adm_mst_tuser g ON g.user_gid = a.salesperson_gid " +

                    " LEFT JOIN crm_trn_tcurrencyexchange h ON a.currency_gid = h.currencyexchange_gid " +

                    " LEFT JOIN crm_mst_tcustomercontact m ON m.customer_gid = a.customer_gid " +

                    " LEFT JOIN crm_mst_tcustomer n ON n.customer_gid = a.customer_gid " +
                    " LEFT JOIN  crm_trn_trenewal r ON a.salesorder_gid = r.salesorder_gid " +

                    " WHERE a.salesorder_gid = '" + salesorder_gid + "' GROUP BY a.salesorder_gid, d.product_gid ORDER BY a.salesorder_gid ASC";

                    DataTable dt_datatable1 = objdbconn.GetDataTable(msSQL);

                    var getModuleList = new List<postsalesorder_list>();

                    if (dt_datatable1.Rows.Count != 0)

                    {

                        foreach (DataRow dt in dt_datatable1.Rows)

                        {

                            getModuleList.Add(new postsalesorder_list

                            {


                                salesorder_gid = dt["salesorder_gid"].ToString(),
                                so_referenceno1 = dt["so_referenceno1"].ToString(),
                                customer_gid = dt["customer_gid"].ToString(),
                                currency_gid = dt["currency_gid"].ToString(),
                                renewal_mode = dt["renewal_flag"].ToString(),
                                frequency_term = dt["frequency_term"].ToString(),
                                renewal_date = dt["renewal_date"].ToString(),
                                tax_name = dt["tax_name"].ToString(),
                                salesorder_date = dt["salesorder_date"].ToString(),
                                customer_name = dt["customer_name"].ToString(),
                                user_gid = dt["user_gid"].ToString(),
                                branch_name = dt["branch_name"].ToString(),
                                customer_contact_person = dt["customer_contact_person"].ToString(),

                                customer_email = dt["email"].ToString(),

                                customer_mobile = dt["mobile"].ToString(),

                                gst_number = dt["gst_number"].ToString(),

                                customer_address = dt["customer_address"].ToString(),

                                start_date = dt["start_date"].ToString(),

                                end_date = dt["end_date"].ToString(),

                                currency_code = dt["currency_code"].ToString(),
                                file_path = dt["file_path"].ToString(),

                                file_name = dt["file_name"].ToString(),

                                exchange_rate = dt["exchange_rate"].ToString(),

                                freight_terms = dt["freight_terms"].ToString(),

                                payment_terms = dt["payment_terms"].ToString(),

                                payment_days = dt["payment_days"].ToString(),

                                so_referencenumber = dt["so_referenceno1"].ToString(),

                                shipping_to = dt["shipping_to"].ToString(),

                                delivery_days = dt["delivery_days"].ToString(),

                                address1 = dt["address1"].ToString(),

                                address2 = dt["address2"].ToString(),

                                so_remarks = dt["so_remarks"].ToString(),

                                salesperson_name = dt["salesperson_name"].ToString(),
                                salesperson_gid = dt["salesperson_gid"].ToString(),

                                addon_charge = dt["addon_charge"].ToString(),

                                additional_discount = dt["additional_discount"].ToString(),

                                freight_charges = dt["freight_charges"].ToString(),

                                buyback_charges = dt["buyback_charges"].ToString(),

                                packing_charges = dt["packing_charges"].ToString(),

                                insurance_charges = dt["insurance_charges"].ToString(),

                                roundoff = dt["roundoff"].ToString(),

                                Grandtotal = dt["Grandtotal"].ToString(),

                                termsandconditions = dt["termsandconditions"].ToString(),

                                total_price = dt["total_price"].ToString(),

                                price = dt["total_price"].ToString(),

                                total_amount = dt["total_amount"].ToString(),

                                tax_amount = dt["tax_amount"].ToString(),


                            });

                            values.postsalesorder_list = getModuleList;

                        }

                    }

                    dt_datatable1.Dispose();

                }
            }

            catch (Exception ex)

            {

                values.message = "Exception occured while loading Sales Order Summary !";

                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +

                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +

                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }

        }
        public void DaGetEditSODrafts(string salesorder_gid, string employee_gid, MdlSmrTrnSalesorder values)

        {

            try

            {
                msSQL = " delete from smr_tmp_tsalesorderdtl where " +
                       " employee_gid = '" + employee_gid + "' ";
                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                if (mnResult == 1)
                {
                    msSQL = "select d.product_gid,e.taxsegment_gid,i.productgroup_name,d.taxsegmenttax_gid, i.productgroup_gid,d.product_remarks,d.uom_gid,d.product_name,d.salesorderdtl_gid,d.product_code,d.uom_name,d.qty_quoted,d.margin_amount,d.margin_percentage,d.discount_percentage, " +
                           "  d.discount_amount,d.product_price ,d.tax_name,d.tax_amount,price,e.tax_prefix, " +
                           " d.tax1_gid,d.tax2_gid,d.tax3_gid,d.tax_name2,d.tax_name3,d.tax_amount2,d.tax_amount3," +
                           " d.tax_percentage,d.tax_percentage2,d.tax_percentage3,d.product_remarks" +
                           " FROM smr_tmp_tsalesorderdrafts a " +
                           " LEFT JOIN smr_tmp_tsalesorderdtldrafts d ON d.salesorder_gid = a.salesorder_gid " +
                           " LEFT JOIN acp_mst_ttax e ON e.taxsegment_gid = d.taxsegment_gid " +
                           " LEFT JOIN pmr_mst_tproductgroup i ON i.productgroup_gid = d.productgroup_gid" +
                           " WHERE a.salesorder_gid = '" + salesorder_gid + "' group by d.tmpsalesorderdtl_gid order by d.salesorderdtl_gid  desc";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                    if (dt_datatable.Rows.Count != 0)

                    {

                        foreach (DataRow dt in  dt_datatable.Rows)

                        {
                            msGetGid = objcmnfunctions.GetMasterGID("VSDT");

                            msSQL = " insert into smr_tmp_tsalesorderdtl( " +
                                     " tmpsalesorderdtl_gid," +
                                     " salesorder_gid," +
                                     " employee_gid," +
                                     " product_gid," +
                                     " product_code," +
                                     " product_name," +
                                     " productgroup_gid," +
                                     " product_price," +
                                     " qty_quoted," +
                                     " uom_gid," +
                                     " uom_name," +
                                     " price," +
                                     " order_type," +
                                     " tax_rate, " +
                                     " taxsegment_gid, " +
                                    " taxsegmenttax_gid, " +
                                    " tax1_gid, " +
                                    " tax2_gid, " +
                                    " tax3_gid, " +
                                    " tax_name, " +
                                    " tax_name2, " +
                                    " tax_name3, " +
                                    " tax_percentage, " +
                                    " tax_percentage2, " +
                                    " tax_percentage3, " +
                                    " tax_amount, " +
                                    " tax_amount2, " +
                                    " tax_amount3, " +
                                     " discount_amount, " +
                                     " product_remarks, " +
                                     " discount_percentage" +
                                     ")values(" +
                                     "'" + msGetGid + "'," +
                                     "'" + salesorder_gid + "'," +
                                     "'" + employee_gid + "'," +
                                     "'" + dt["product_gid"].ToString() + "'," +
                                     "'" + dt["product_code"].ToString() + "'," +
                                     "'" + dt["product_name"].ToString() + "'," +
                                     "'" + dt["productgroup_gid"].ToString() + "'," +
                                     "'" + dt["product_price"].ToString() + "'," +
                                     "'" + dt["qty_quoted"].ToString() + "'," +
                                     "'" + dt["uom_gid"].ToString() + "'," +
                                     "'" + dt["uom_name"].ToString() + "'," +
                                     "'" + dt["price"].ToString() + "'," +
                                     "'Sales'," +
                                     "'" + dt["tax_amount"].ToString() + "'," +
                                     "'" + dt["taxsegment_gid"].ToString() + "'," +
                                     "'" + dt["taxsegmenttax_gid"].ToString() + "'," +
                                     "'" + dt["tax1_gid"].ToString() + "'," +
                                     "'" + dt["tax2_gid"].ToString() + "'," +
                                     "'" + dt["tax3_gid"].ToString() + "'," +
                                     "'" + dt["tax_name"].ToString() + "'," +
                                     "'" + dt["tax_name2"].ToString() + "'," +
                                     "'" + dt["tax_name3"].ToString() + "',";
                            if (dt["tax_percentage"].ToString() == "" || dt["tax_percentage"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_percentage"].ToString() + "', ";
                            }
                            if (dt["tax_percentage2"].ToString() == "" || dt["tax_percentage2"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_percentage2"].ToString() + "', ";
                            }
                            if (dt["tax_percentage3"].ToString() == "" || dt["tax_percentage3"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_percentage3"].ToString() + "', ";
                            }
                            if (dt["tax_amount"].ToString() == "" || dt["tax_amount"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_amount"].ToString() + "', ";
                            }
                            if (dt["tax_amount2"].ToString() == "" || dt["tax_amount2"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_amount2"].ToString() + "', ";
                            }
                            if (dt["tax_amount3"].ToString() == "" || dt["tax_amount3"].ToString() == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + dt["tax_amount3"].ToString() + "', ";
                            }

                            msSQL +=
                               "'" + dt["discount_amount"].ToString() + "'," +
                               "'" + dt["product_remarks"].ToString() + "'," +
                               "'" + dt["discount_percentage"].ToString() + "')";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                        }
                    }

                    msSQL = " SELECT distinct a.file_path , a.file_name,a.salesorder_gid,a.so_referenceno1,a.customer_gid,g.user_gid,a.currency_gid, h.currency_code, a.customerbranch_gid, a.exchange_rate, " +
                    " DATE_FORMAT(a.salesorder_date, '%d-%m-%Y') AS salesorder_date, a.salesperson_gid, " +
                    " concat(g.user_code, ' / ', g.user_firstname, ' ', g.user_lastname) AS salesperson_name, " +
                    "format( a.Grandtotal,2) AS Grandtotal, a.termsandconditions, a.addon_charge AS addon_charge, " +
                    " a.additional_discount_l AS additional_discount, a.payment_days, a.tax_amount AS tax_amount, " +
                    " a.delivery_days, a.so_referenceno1, a.payment_terms, a.freight_terms, " +
                    " concat(m.address1,'   ',m.city,'   ',m.state) as address1,ifnull(m.address2,'') as address2," +
                    " a.roundoff AS roundoff, a.so_remarks, a.shipping_to, " +
                    " a.customer_address, concat(n.customer_name,' / ',m.email) as customer_name , a.customer_contact_person AS customer_contact_person, " +
                    " case when a.start_date = '0000-00-00' then '' else DATE_FORMAT(a.start_date, '%d-%m-%Y') end AS start_date,case when a.end_date = '0000-00-00' then '' else DATE_FORMAT(a.end_date, '%d-%m-%Y') end AS end_date, " +
                    " a.termsandconditions, m.mobile, m.email,n.gst_number ,e.branch_name,e.branch_gid, " +
                    " FORMAT(a.total_amount, 2) AS total_amount, a.freight_charges AS freight_charges, " +
                    " FORMAT(a.packing_charges, 2) AS packing_charges,format(a.total_price, 2) as total_price,a.tax_name, FORMAT(a.buyback_charges, 2) AS buyback_charges, " +
                    " FORMAT(a.insurance_charges, 2) AS insurance_charges,a.renewal_flag," +
                    " CASE   WHEN a.renewal_flag = 'Y' THEN DATE_FORMAT(r.renewal_date, '%d-%m-%Y') ELSE NULL END AS renewal_date,CASE WHEN a.renewal_flag = 'Y' THEN r.frequency_term ELSE NULL END AS frequency_term FROM smr_tmp_tsalesorderdrafts a " +
                    " LEFT JOIN smr_tmp_tsalesorderdtldrafts d ON d.salesorder_gid = a.salesorder_gid " +
                    " LEFT JOIN hrm_mst_tbranch e ON e.branch_gid = a.branch_gid " +
                    " LEFT JOIN acp_mst_ttax f ON f.tax_gid = a.tax_gid " +
                    " LEFT JOIN adm_mst_tuser g ON g.user_gid = a.salesperson_gid " +
                    " LEFT JOIN crm_trn_tcurrencyexchange h ON a.currency_gid = h.currencyexchange_gid " +
                    " LEFT JOIN crm_mst_tcustomercontact m ON m.customer_gid = a.customer_gid " +
                    " LEFT JOIN crm_mst_tcustomer n ON n.customer_gid = a.customer_gid " +
                    " LEFT JOIN  crm_trn_trenewal r ON a.salesorder_gid = r.salesorder_gid " +
                    " WHERE a.salesorder_gid = '" + salesorder_gid + "' GROUP BY a.salesorder_gid, d.product_gid ORDER BY a.salesorder_gid ASC";

                    DataTable dt_datatable2 = objdbconn.GetDataTable(msSQL);
                    var getModuleList = new List<postsalesorder_list>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in dt_datatable2.Rows)
                        {
                            getModuleList.Add(new postsalesorder_list
                            {
                                salesorder_gid = dt["salesorder_gid"].ToString(),
                                so_referenceno1 = dt["so_referenceno1"].ToString(),
                                customer_gid = dt["customer_gid"].ToString(),
                                currency_gid = dt["currency_gid"].ToString(),
                                renewal_mode = dt["renewal_flag"].ToString(),
                                frequency_term = dt["frequency_term"].ToString(),
                                renewal_date = dt["renewal_date"].ToString(),
                                tax_name = dt["tax_name"].ToString(),
                                salesorder_date = dt["salesorder_date"].ToString(),
                                customer_name = dt["customer_name"].ToString(),
                                user_gid = dt["user_gid"].ToString(),
                                branch_name = dt["branch_gid"].ToString(),
                                customer_contact_person = dt["customer_contact_person"].ToString(),
                                customer_email = dt["email"].ToString(),
                                customer_mobile = dt["mobile"].ToString(),
                                gst_number = dt["gst_number"].ToString(),
                                customer_address = dt["customer_address"].ToString(),
                                start_date = dt["start_date"].ToString(),
                                end_date = dt["end_date"].ToString(),
                                currency_code = dt["currency_code"].ToString(),
                                exchange_rate = dt["exchange_rate"].ToString(),
                                freight_terms = dt["freight_terms"].ToString(),
                                payment_terms = dt["payment_terms"].ToString(),
                                payment_days = dt["payment_days"].ToString(),
                                so_referencenumber = dt["so_referenceno1"].ToString(),
                                shipping_to = dt["shipping_to"].ToString(),
                                delivery_days = dt["delivery_days"].ToString(),
                                address1 = dt["address1"].ToString(),
                                address2 = dt["address2"].ToString(),
                                so_remarks = dt["so_remarks"].ToString(),
                                salesperson_name = dt["salesperson_name"].ToString(),
                                salesperson_gid = dt["salesperson_gid"].ToString(),
                                addon_charge = dt["addon_charge"].ToString(),
                                additional_discount = dt["additional_discount"].ToString(),
                                freight_charges = dt["freight_charges"].ToString(),
                                buyback_charges = dt["buyback_charges"].ToString(),
                                packing_charges = dt["packing_charges"].ToString(),
                                insurance_charges = dt["insurance_charges"].ToString(),
                                roundoff = dt["roundoff"].ToString(),
                                Grandtotal = dt["Grandtotal"].ToString(),
                                termsandconditions = dt["termsandconditions"].ToString(),
                                total_price = dt["total_price"].ToString(),
                                price = dt["total_price"].ToString(),
                                total_amount = dt["total_amount"].ToString(),
                                tax_amount = dt["tax_amount"].ToString(),
                                file_name = dt["file_name"].ToString(),
                                file_path = dt["file_path"].ToString(),
                            });
                            values.postsalesorder_list = getModuleList;
                        }
                    }
                    dt_datatable2.Dispose();
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Order Summary !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetOnChangeCustomer(string? customer_gid, MdlSmrTrnSalesorder values)
        {
            try
            {


                if (customer_gid != null)
                {
                    msSQL = " select a.customercontact_gid,c.taxsegment_gid,ifnull(a.address1,'') as address1,ifnull(a.address2,'') as address2,ifnull(a.city,'') as city, " +
                    " ifnull(a.state,'') as state,ifnull(a.country_gid,'') as country_gid,ifnull(a.zip_code,'') as zip_code, " +
                    " ifnull(a.mobile,'') as mobile,a.email,ifnull(b.country_name,'') as country_name,a.customerbranch_name,concat(customerbranch_name,' | ',a.customercontact_name) as " +
                    " customercontact_names,c.customer_name,c.gst_number " +
                    " from crm_mst_tcustomercontact a " +
                    " left join crm_mst_tcustomer c on a.customer_gid=c.customer_gid " +
                    " left join adm_mst_tcountry b on a.country_gid=b.country_gid " +
                    " where c.customer_gid='" + customer_gid + "'";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                    var getModuleList = new List<GetCustomerDet>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in  dt_datatable.Rows)
                        {
                            getModuleList.Add(new GetCustomerDet
                            {
                                customercontact_names = dt["customercontact_names"].ToString(),
                                taxsegment_gid = dt["taxsegment_gid"].ToString(),
                                branch_name = dt["customerbranch_name"].ToString(),
                                country_name = dt["country_name"].ToString(),
                                customer_email = dt["email"].ToString(),
                                customer_mobile = dt["mobile"].ToString(),
                                zip_code = dt["zip_code"].ToString(),
                                country_gid = dt["country_gid"].ToString(),
                                state = dt["state"].ToString(),
                                city = dt["city"].ToString(),
                                address2 = dt["address2"].ToString(),
                                address1 = dt["address1"].ToString(),
                                customercontact_gid = dt["customercontact_gid"].ToString(),
                                customer_name = dt["customer_name"].ToString(),
                                gst_number = dt["gst_number"].ToString(),

                            });
                            values.GetCustomer = getModuleList;
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Changing Customer !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetCustomerOnchangeCRM(string customer_gid, MdlSmrTrnSalesorder values)
        {
            try
            {


                if (customer_gid != null)
                {
                    msSQL = " select a.customercontact_gid,concat(a.address1,'   ',a.city,'   ',a.state) as address1,ifnull(a.address2,'') as address2,ifnull(a.city,'') as city, " +
                    " ifnull(a.state,'') as state,ifnull(a.country_gid,'') as country_gid,ifnull(a.zip_code,'') as zip_code, " +
                    " ifnull(a.mobile,'') as mobile,a.email,ifnull(b.country_name,'') as country_name,a.customerbranch_name,concat(customerbranch_name,' | ',a.customercontact_name) as " +
                    " customercontact_names, c.customer_gid " +
                    " from crm_mst_tcustomercontact a " +
                    " left join crm_mst_tcustomer c on a.customer_gid=c.customer_gid " +
                    " left join adm_mst_tcountry b on a.country_gid=b.country_gid " +
                    " where c.customer_gid='" + customer_gid + "'";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                    var getModuleList = new List<GetCustomerDet>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in  dt_datatable.Rows)
                        {
                            getModuleList.Add(new GetCustomerDet
                            {
                                customercontact_names = dt["customercontact_names"].ToString(),
                                branch_name = dt["customerbranch_name"].ToString(),
                                country_name = dt["country_name"].ToString(),
                                customer_email = dt["email"].ToString(),
                                customer_mobile = dt["mobile"].ToString(),
                                zip_code = dt["zip_code"].ToString(),
                                country_gid = dt["country_gid"].ToString(),
                                state = dt["state"].ToString(),
                                city = dt["city"].ToString(),
                                address2 = dt["address2"].ToString(),
                                customer_address = dt["address1"].ToString(),
                                customer_gid = dt["customer_gid"].ToString(),

                            });
                            values.GetCustomer = getModuleList;
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Customer Onchange CRM  !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetOnChangeCustomerCRM(string leadbank_gid, MdlSmrTrnSalesorder values)
        {
            try
            {


                if (leadbank_gid != null)
                {
                    msSQL = " select a.leadbankcontact_gid,concat(a.address1,'   ',a.city,'   ',a.state) as address1,ifnull(a.address2,'') as address2,ifnull(a.city,'') as city, " +
                    " ifnull(a.state,'') as state,ifnull(a.country_gid,'') as country_gid,ifnull(a.zip_code,'') as zip_code, " +
                    " ifnull(a.mobile,'') as mobile,a.email,ifnull(b.country_name,'') as country_name,a.customerbranch_name,concat(customerbranch_name,' | ',a.customercontact_name) as " +
                    " customercontact_names " +
                    " from crm_mst_tleadbankcontact a " +
                    " left join crm_mst_tleadbank c on a.leadbank_gid=c.leadbank_gid " +
                    " left join adm_mst_tcountry b on a.country_gid=b.country_gid " +
                    " where c.customer_gid='" + leadbank_gid + "'";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                    var getModuleList = new List<GetCustomerDet>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in  dt_datatable.Rows)
                        {
                            getModuleList.Add(new GetCustomerDet
                            {
                                customercontact_names = dt["customercontact_names"].ToString(),
                                branch_name = dt["customerbranch_name"].ToString(),
                                country_name = dt["country_name"].ToString(),
                                customer_email = dt["email"].ToString(),
                                customer_mobile = dt["mobile"].ToString(),
                                zip_code = dt["zip_code"].ToString(),
                                country_gid = dt["country_gid"].ToString(),
                                state = dt["state"].ToString(),
                                city = dt["city"].ToString(),
                                address2 = dt["address2"].ToString(),
                                customer_address = dt["address1"].ToString(),
                                customercontact_gid = dt["customercontact_gid"].ToString(),

                            });
                            values.GetCustomer = getModuleList;
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading  onchange Cusotmer CRM!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetTax1Dtl(MdlSmrTrnSalesorder values)
        {
            try
            {



                msSQL = " select tax_name,tax_gid,percentage from acp_mst_ttax where active_flag='Y' ";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetTaxoneDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetTaxoneDropdown

                        {
                            tax_gid = dt["tax_gid"].ToString(),
                            tax1_gid = dt["tax_gid"].ToString(),
                            tax_name = dt["tax_name"].ToString(),
                            percentage = dt["percentage"].ToString()
                        });
                        values.GetTax1Dtl = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Tax Dropdown !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetTax2Dtl(MdlSmrTrnSalesorder values)
        {
            try
            {


                msSQL = " select tax_name,tax_gid,percentage from acp_mst_ttax where active_flag='Y' ";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetTaxTwoDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetTaxTwoDropdown

                        {
                            tax_gid2 = dt["tax_gid"].ToString(),
                            tax_name2 = dt["tax_name"].ToString(),
                            percentage = dt["percentage"].ToString()

                        });
                        values.GetTax2Dtl = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Tax dropdown !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetTax3Dtl(MdlSmrTrnSalesorder values)
        {
            try
            {



                msSQL = " select tax_name,tax_gid,percentage from acp_mst_ttax where active_flag='Y' ";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetTaxThreeDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetTaxThreeDropdown

                        {
                            tax_gid3 = dt["tax_gid"].ToString(),
                            tax_name3 = dt["tax_name"].ToString(),
                            percentage = dt["percentage"].ToString()

                        });
                        values.GetTax3Dtl = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Tax Dropdown !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetProductNamDtl(MdlSmrTrnSalesorder values)
        {
            try
            {



                msSQL = "Select product_gid, product_name from pmr_mst_tproduct where status='1'";


                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetProductNamDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetProductNamDropdown

                        {
                            product_gid = dt["product_gid"].ToString(),
                            product_name = dt["product_name"].ToString(),

                        });
                        values.GetProductNamDtl = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Product name dropdown !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetProductNamDtlCRM(MdlSmrTrnSalesorder values)
        {
            try
            {



                msSQL = "Select product_gid, product_name from pmr_mst_tproduct";


                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetProductNamDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetProductNamDropdown

                        {
                            product_gid = dt["product_gid"].ToString(),
                            product_name = dt["product_name"].ToString(),

                        });
                        values.GetProductNamDtl = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Product Name dropdown CRM !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetOnChangeProductsNameCRM(string product_gid, MdlSmrTrnSalesorder values)
        {
            try
            {


                if (product_gid != null)
                {
                    msSQL = " Select a.product_name, a.product_code, b.productuom_gid,b.productuom_name,c.productgroup_name,c.productgroup_gid,a.productuom_gid  from pmr_mst_tproduct a  " +
                         " left join pmr_mst_tproductuom b on a.productuom_gid = b.productuom_gid  " +
                        " left join pmr_mst_tproductgroup c on a.productgroup_gid = c.productgroup_gid  " +
                    " where a.product_gid='" + product_gid + "' ";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                    var getModuleList = new List<getproductsCode>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in  dt_datatable.Rows)
                        {
                            getModuleList.Add(new getproductsCode
                            {
                                product_name = dt["product_name"].ToString(),
                                product_code = dt["product_code"].ToString(),
                                productuom_name = dt["productuom_name"].ToString(),
                                productgroup_name = dt["productgroup_name"].ToString(),
                                productuom_gid = dt["productuom_gid"].ToString(),
                                productgroup_gid = dt["productgroup_gid"].ToString(),

                            });
                            values.ProductsCodes = getModuleList;
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Product Name !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetOnChangeProductsNameAmend(string product_gid, MdlSmrTrnSalesorder values)
        {
            try
            {




                msSQL = " Select a.product_name, a.product_code, a.cost_price,a.product_gid, b.productuom_gid,b.productuom_name,c.productgroup_name," +
                " c.productgroup_gid,a.productuom_gid  from pmr_mst_tproduct a  " +
                " left join pmr_mst_tproductuom b on a.productuom_gid = b.productuom_gid  " +
                " left join pmr_mst_tproductgroup c on a.productgroup_gid = c.productgroup_gid  " +
                " where a.product_gid='" + product_gid + "' ";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                var getModuleList = new List<getproductsCode>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new getproductsCode
                        {
                            product_name = dt["product_name"].ToString(),
                            product_code = dt["product_code"].ToString(),
                            productuom_name = dt["productuom_name"].ToString(),
                            productgroup_name = dt["productgroup_name"].ToString(),
                            productuom_gid = dt["productuom_gid"].ToString(),
                            productgroup_gid = dt["productgroup_gid"].ToString(),
                            product_gid = dt["product_gid"].ToString(),
                            unitprice = dt["cost_price"].ToString()

                        });
                        values.ProductsCodes = getModuleList;
                    }
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while Changing Product Name Amend!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }


        }
        public void DaGetOnChangeProductsName(string product_gid, string customercontact_gid, MdlSmrTrnSalesorder values)
        {
            try
            {


                if (customercontact_gid != null)
                {

                    msSQL = "  select a.product_price from smr_trn_tpricesegment2product a    left join smr_trn_tpricesegment2customer b on a.pricesegment_gid= b.pricesegment_gid " +
                        "  left join pmr_mst_tproduct c on a.product_gid=c.product_gid where b.customer_gid='" + customercontact_gid + "'   and a.product_gid='" + product_gid + "'";
                    lsproduct_price = objdbconn.GetExecuteScalar(msSQL);
                    if (lsproduct_price != "")
                    {

                        msSQL = " Select a.product_name, a.product_code,case when f.customer_gid is not null then(select a.product_price from smr_trn_tpricesegment2product a " +
                        " left join smr_trn_tpricesegment2customer b on a.pricesegment_gid= b.pricesegment_gid where b.customer_gid='" + lscustomer_gid + "'" +
                        " and a.product_gid='" + product_gid + "') else (a.mrp_price)end as cost_price,  b.productuom_gid,b.productuom_name,c.productgroup_name," +
                        "c.productgroup_gid,a.productuom_gid  from pmr_mst_tproduct a  left join pmr_mst_tproductuom b on a.productuom_gid = b.productuom_gid " +
                        "  left join pmr_mst_tproductgroup c on  a.productgroup_gid = c.productgroup_gid  left join smr_trn_tpricesegment2product e" +
                        " on a.product_gid=e.product_gid left join smr_trn_tpricesegment2customer f on e.pricesegment_gid=f.pricesegment_gid " +
                        " where a.product_gid='" + product_gid + "'";
                        DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                        var getModuleList = new List<GetproductsCode>();
                        if (dt_datatable.Rows.Count != 0)
                        {
                            foreach (DataRow dt in  dt_datatable.Rows)
                            {
                                getModuleList.Add(new GetproductsCode
                                {
                                    product_name = dt["product_name"].ToString(),
                                    product_code = dt["product_code"].ToString(),
                                    productuom_name = dt["productuom_name"].ToString(),
                                    productgroup_name = dt["productgroup_name"].ToString(),
                                    productuom_gid = dt["productuom_gid"].ToString(),
                                    productgroup_gid = dt["productgroup_gid"].ToString(),
                                    unitprice = lsproduct_price,

                                });
                                values.ProductsCode = getModuleList;
                            }
                            if (dt_datatable.Rows.Count > 0)
                            {
                                lsmrp_price = Convert.ToDecimal(lsproduct_price);
                            }
                            if (lsmrp_price > 0)
                            {
                                msSQL = " select f.taxsegment_gid,d.taxsegment_gid,e.taxsegment_name,d.tax_name,d.tax_gid, " +
                                    " case when d.tax_percentage = ROUND(d.tax_percentage) then ROUND(d.tax_percentage) else d.tax_percentage end AS tax_percentage,d.tax_amount, " +
                                    " a.mrp_price,a.cost_price from acp_mst_ttaxsegment2product d " +
                                    " left join acp_mst_ttaxsegment e on e.taxsegment_gid=d.taxsegment_gid " +
                                    " left join crm_mst_tcustomer f on f.taxsegment_gid = e.taxsegment_gid " +
                                    " left  join pmr_mst_tproduct a on a.product_gid=d.product_gid " +
                                    " where a.product_gid='" + product_gid + "'   and f.customer_gid='" + customercontact_gid + "' ";
                                DataTable dt_datatable3 = objdbconn.GetDataTable(msSQL);

                                var getGetTaxSegmentList = new List<GetTaxSegmentLists>();
                                if (dt_datatable3.Rows.Count != 0)
                                {
                                    foreach (DataRow dt in dt_datatable3.Rows)
                                    {
                                        getGetTaxSegmentList.Add(new GetTaxSegmentLists
                                        {
                                            taxsegment_gid = dt["taxsegment_gid"].ToString(),
                                            taxsegment_name = dt["taxsegment_name"].ToString(),
                                            tax_name = dt["tax_name"].ToString(),
                                            tax_percentage = dt["tax_percentage"].ToString(),
                                            tax_gid = dt["tax_gid"].ToString(),
                                            mrp_price = dt["mrp_price"].ToString(),
                                            cost_price = dt["cost_price"].ToString(),
                                            tax_amount = dt["tax_amount"].ToString(),

                                        });
                                        values.GetTaxSegmentList = getGetTaxSegmentList;
                                    }
                                }

                                dt_datatable3.Dispose();
                            }
                        }

                    }
                    else
                    {
                        msSQL = " Select a.product_name, a.product_code,a.cost_price," +
                            " b.productuom_gid,b.productuom_name,c.productgroup_name,c.productgroup_gid,a.productuom_gid  from pmr_mst_tproduct a  " +
                             " left join pmr_mst_tproductuom b on a.productuom_gid = b.productuom_gid  " +
                            " left join pmr_mst_tproductgroup c on a.productgroup_gid = c.productgroup_gid " +
                        " where a.product_gid='" + product_gid + "' ";
                        DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                        var getModuleList = new List<GetproductsCode>();
                        if (dt_datatable.Rows.Count != 0)
                        {
                            foreach (DataRow dt in  dt_datatable.Rows)
                            {
                                getModuleList.Add(new GetproductsCode
                                {
                                    product_name = dt["product_name"].ToString(),
                                    product_code = dt["product_code"].ToString(),
                                    productuom_name = dt["productuom_name"].ToString(),
                                    productgroup_name = dt["productgroup_name"].ToString(),
                                    productuom_gid = dt["productuom_gid"].ToString(),
                                    productgroup_gid = dt["productgroup_gid"].ToString(),
                                    unitprice = dt["cost_price"].ToString(),

                                });
                                values.ProductsCode = getModuleList;
                            }
                            if (dt_datatable.Rows.Count > 0)
                            {
                                lsmrp_price = Convert.ToDecimal( dt_datatable.Rows[0]["mrp_price"]);
                            }
                            if (lsmrp_price > 0)
                            {
                                msSQL = " select f.taxsegment_gid,d.taxsegment_gid,e.taxsegment_name,d.tax_name,d.tax_gid, " +
                                    " case when d.tax_percentage = ROUND(d.tax_percentage) then ROUND(d.tax_percentage) else d.tax_percentage end AS tax_percentage,d.tax_amount, " +
                                    " a.mrp_price,a.cost_price from acp_mst_ttaxsegment2product d " +
                                    " left join acp_mst_ttaxsegment e on e.taxsegment_gid=d.taxsegment_gid " +
                                    " left join crm_mst_tcustomer f on f.taxsegment_gid = e.taxsegment_gid " +
                                    " left  join pmr_mst_tproduct a on a.product_gid=d.product_gid " +
                                    " where a.product_gid='" + product_gid + "'   and f.customer_gid='" + customercontact_gid + "' ";
                                DataTable dt_datatable4 = objdbconn.GetDataTable(msSQL);

                                var getGetTaxSegmentList = new List<GetproductsCode>();
                                if (dt_datatable4.Rows.Count != 0)
                                {
                                    foreach (DataRow dt in dt_datatable4.Rows)
                                    {
                                        getGetTaxSegmentList.Add(new GetproductsCode
                                        {
                                            taxsegment_gid = dt["taxsegment_gid"].ToString(),
                                            taxsegment_name = dt["taxsegment_name"].ToString(),
                                            tax_name = dt["tax_name"].ToString(),
                                            tax_percentage = dt["tax_percentage"].ToString(),
                                            tax_gid = dt["tax_gid"].ToString(),
                                            mrp_price = dt["mrp_price"].ToString(),
                                            unitprice = dt["cost_price"].ToString(),
                                            tax_amount = dt["tax_amount"].ToString(),

                                        });
                                        values.ProductsCode = getGetTaxSegmentList;
                                    }
                                }
                                dt_datatable4.Dispose();

                            }
                        }
                        dt_datatable.Dispose();
                    }
                }
                else
                {
                    values.status = false;
                    values.message = "Kindly Select Customer Before Adding Product";
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Product Name !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetOnChangeProductsNames(string product_gid, MdlSmrTrnSalesorder values)

        {
            try
            {
                msSQL = " Select a.product_name, a.product_code,a.cost_price," +
                    " b.productuom_gid,b.productuom_name,c.productgroup_name,c.productgroup_gid,a.productuom_gid  from pmr_mst_tproduct a  " +
                     " left join pmr_mst_tproductuom b on a.productuom_gid = b.productuom_gid  " +
                    " left join pmr_mst_tproductgroup c on a.productgroup_gid = c.productgroup_gid " +
                " where a.product_gid='" + product_gid + "' ";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                var getModuleList = new List<GetproductsCode>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetproductsCode
                        {
                            product_name = dt["product_name"].ToString(),
                            product_code = dt["product_code"].ToString(),
                            productuom_name = dt["productuom_name"].ToString(),
                            productgroup_name = dt["productgroup_name"].ToString(),
                            productuom_gid = dt["productuom_gid"].ToString(),
                            productgroup_gid = dt["productgroup_gid"].ToString(),
                            unitprice = dt["cost_price"].ToString(),

                        });
                        values.ProductsCode = getModuleList;
                    }
                }



            }
            catch (Exception ex)
            {
                values.message = "Exception occured while  Adding Product !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }


        }
        public void DaGetSalesOrdersummary(string employee_gid, MdlSmrTrnSalesorder values)
        {
            try
            {


                double grand_total = 0.00;
                double grandtotal = 0.00;

                msSQL = "SELECT a.tmpsalesorderdtl_gid,a.quotation_gid, a.tax_name, p.customer_gid,a.tax_amount, a.salesorderdtl_gid, a.salesorder_gid, a.product_gid, a.productgroup_gid, " +
                   " a.productgroup_name, a.product_name, FORMAT(a.product_price, 2) AS product_price, b.product_code, a.qty_quoted, a.product_remarks, " +
                   " a.uom_gid, a.vendor_gid, a.slno, a.uom_name, FORMAT(a.price, 2) AS price, " +
                   " FORMAT(a.discount_percentage,2) AS discount_percentage, FORMAT(a.discount_amount,2) AS discount_amount, " +
                   " FORMAT(a.selling_price, '0.00') AS selling_price " +
                   " FROM smr_tmp_tsalesorderdtl a " +
                   " left join smr_trn_tsalesorder s on s.salesorder_gid=a.salesorder_gid " +
                   " left join crm_mst_tcustomer p on p.taxsegment_gid = a.taxsegment_gid " +
                   " LEFT JOIN pmr_mst_tproduct b ON a.product_gid = b.product_gid " +
                   " LEFT JOIN acp_mst_tvendor e ON a.vendor_gid = e.vendor_gid " +
                   " WHERE a.employee_gid = '" + employee_gid + "' group by a.product_gid, b.delete_flag = 'N' and a.quotation_gid is null" +
                   " ORDER BY a.slno ASC";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesorders_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        grand_total += double.Parse(dt["price"].ToString());
                        grandtotal += double.Parse(dt["price"].ToString());
                        getModuleList.Add(new salesorders_list
                        {
                            tmpsalesorderdtl_gid = dt["tmpsalesorderdtl_gid"].ToString(),
                            salesorder_gid = dt["salesorder_gid"].ToString(),
                            product_name = dt["product_name"].ToString(),
                            product_gid = dt["product_gid"].ToString(),
                            product_code = dt["product_code"].ToString(),
                            slno = dt["slno"].ToString(),
                            discountamount = dt["discount_amount"].ToString(),
                            discountpercentage = double.Parse(dt["discount_percentage"].ToString()),
                            //productgroup_name = dt["productgroup_name"].ToString(),
                            product_price = dt["product_price"].ToString(),
                            quantity = dt["qty_quoted"].ToString(),
                            uom_gid = dt["uom_gid"].ToString(),
                            productuom_name = dt["uom_name"].ToString(),
                            producttotalamount = dt["price"].ToString(),
                            totalamount = dt["price"].ToString(),
                            taxname1 = dt["tax_name"].ToString(),
                            tax_amount = dt["tax_amount"].ToString(),
                            grand_total = dt["price"].ToString(),
                            grandtotal = dt["price"].ToString(),
                            customer_gid = dt["customer_gid"].ToString()


                        });
                        values.salesorders_list = getModuleList;
                        values.grand_total = grand_total;
                        values.grandtotal = grandtotal;
                    }
                }
                dt_datatable.Dispose();
                foreach (var data in values.salesorders_list)
                {

                    msSQL = " select f.taxsegment_gid,d.taxsegment_gid,e.taxsegment_name,d.tax_name,d.tax_gid, " +
                   " case when d.tax_percentage = ROUND(d.tax_percentage) then ROUND(d.tax_percentage) else d.tax_percentage end AS tax_percentage,d.tax_amount, " +
                   " a.mrp_price,a.cost_price from acp_mst_ttaxsegment2product d " +
                   " left join acp_mst_ttaxsegment e on e.taxsegment_gid=d.taxsegment_gid " +
                   " left join crm_mst_tcustomer f on f.taxsegment_gid = e.taxsegment_gid " +
                   " left  join pmr_mst_tproduct a on a.product_gid=d.product_gid " +
                   " where a.product_gid='" + data.product_gid + "'   and f.customer_gid='" + data.customer_gid + "' ";
                    dt_datatable = objdbconn.GetDataTable(msSQL);

                    var getGetTaxSegmentList = new List<GetTaxSegmentLists>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in dt_datatable.Rows)
                        {
                            getGetTaxSegmentList.Add(new GetTaxSegmentLists
                            {
                                taxsegment_gid = dt["taxsegment_gid"].ToString(),
                                taxsegment_name = dt["taxsegment_name"].ToString(),
                                tax_name = dt["tax_name"].ToString(),
                                tax_percentage = dt["tax_percentage"].ToString(),
                                tax_gid = dt["tax_gid"].ToString(),
                                mrp_price = dt["mrp_price"].ToString(),
                                cost_price = dt["cost_price"].ToString(),
                                tax_amount = dt["tax_amount"].ToString(),

                            });
                            values.GetTaxSegmentList = getGetTaxSegmentList;
                        }
                    }

                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Order Summary !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }


        }
        public void DaGetOnchangeCurrency(string currencyexchange_gid, MdlSmrTrnSalesorder values)
        {
            try
            {

                msSQL = " select currencyexchange_gid,currency_code,exchange_rate from crm_trn_tcurrencyexchange " +
                " where currencyexchange_gid='" + currencyexchange_gid + "'";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetOnchangeCurrency>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetOnchangeCurrency
                        {

                            exchange_rate = dt["exchange_rate"].ToString(),
                            currency_code = dt["currency_code"].ToString(),
                        });
                        values.GetOnchangeCurrency = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Currency !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetSalesProductdetails(string salesorder_gid, MdlSmrTrnSalesorder values)
        {
            try
            {

                msSQL = "select product_code,product_name,qty_quoted from smr_trn_tsalesorderdtl where salesorder_gid ='" + salesorder_gid + "' order by salesorderdtl_gid";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesproductlist>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new salesproductlist
                        {
                            product_code = dt["product_code"].ToString(),
                            product_name = dt["product_name"].ToString(),
                            qty_quoted = dt["qty_quoted"].ToString(),

                        });
                        values.salesproduct_list = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Product Detail !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DagetCancelSalesOrder(string salesorder_gid, MdlSmrTrnSalesorder values)
        {
            try
            {

                msSQL = "SELECT * FROM smr_trn_tsalesorder WHERE salesorder_gid = '" + salesorder_gid + "' and salesorder_status in('Approved', 'Approve Pending')";
                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                if (mnResult != 0)
                {
                    msSQL = "update smr_trn_tsalesorder set salesorder_status='Cancelled' where salesorder_gid='" + salesorder_gid + "'";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                }
                if (mnResult != 0)
                {
                    values.status = true;
                    values.message = "Order Cancelled Successfully";
                }
                else
                {
                    {
                        values.status = false;
                        values.message = "Error While Cancelling Order";
                    }
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while canceling Sales Order!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }


        }
        public void DaGetDirectSalesOrderEditProductSummary(string tmpsalesorderdtl_gid, MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = " Select tmpsalesorderdtl_gid, salesorder_gid, product_gid,product_name, product_price, qty_quoted,tax1_gid, discount_percentage," +
                    " discount_amount, tax_amount, tax_name, uom_gid, uom_name, price, product_code,taxsegment_gid from smr_tmp_tsalesorderdtl" +
                    " where tmpsalesorderdtl_gid = '" + tmpsalesorderdtl_gid + "'";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<DirecteditSalesorderList>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new DirecteditSalesorderList
                        {
                            tmpsalesorderdtl_gid = dt["tmpsalesorderdtl_gid"].ToString(),
                            product_gid = dt["product_gid"].ToString(),
                            product_name = dt["product_name"].ToString(),
                            productuom_name = dt["uom_name"].ToString(),
                            quantity = dt["qty_quoted"].ToString(),
                            product_code = dt["product_code"].ToString(),
                            unitprice = dt["product_price"].ToString(),
                            discount_percentage = dt["discount_percentage"].ToString(),
                            discountamount = dt["discount_amount"].ToString(),
                            totalamount = dt["price"].ToString(),
                            tax_name = dt["tax_name"].ToString(),
                            tax_gid = dt["tax1_gid"].ToString(),
                            tax_amount = dt["tax_amount"].ToString(),
                            taxsegment_gid = dt["taxsegment_gid"].ToString(),

                        });
                        values.directeditsalesorder_list = getModuleList;
                    }
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Order Product Summary !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }

        }
        public void DaGetSalesorderdetail(string product_gid, MdlSmrTrnSalesorder values)
        {
            try
            {

                msSQL = " select a.salesorder_gid,a.salesorderdtl_gid,a.product_gid,b.currency_code,a.product_requireddate as product_requireddate, " +
                    " d.product_name,date_format(c.salesorder_date, '%d-%m-%Y') as salesorder_date, " +
                    " b.customer_gid,b.customer_name,a.qty_quoted,format(a.price, 2) as product_price" +
                    " from smr_trn_tsalesorderdtl a left join smr_trn_tsalesorder b on a.salesorder_gid = b.salesorder_gid" +
                    " left join pmr_mst_tproduct d on a.product_gid = d.product_gid" +
                    " left join acp_trn_torder c on a.salesorder_gid = c.salesorder_gid" +
                    " where a.product_gid = '" + product_gid + "' group by a.price  order by c.salesorder_date desc";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<Directeddetailslist2>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new Directeddetailslist2
                        {
                            product_gid = dt["product_gid"].ToString(),
                            product_name = dt["product_name"].ToString(),
                            salesorder_date = dt["salesorder_date"].ToString(),
                            customer_name = dt["customer_name"].ToString(),
                            qty_quoted = dt["qty_quoted"].ToString(),
                            product_price = dt["product_price"].ToString(),
                            currency_code = dt["currency_code"].ToString(),

                        });
                        values.Directeddetailslist2 = getModuleList;
                    }
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Order Detail !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }

        }
        public void Daonbackcleartmpdata(string employee_gid, MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = " delete from smr_tmp_tsalesorderdtl " +
        " where employee_gid='" + employee_gid + "'";
                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Order Detail !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
        }
        public void DaPostUpdateDirectSOProduct(string employee_gid, DirecteditSalesorderList values)
        {
            try
            {
                //msSQL = "Select tax_gid from acp_mst_ttax where tax_name='" + values.tax_name + "'";
                //string TaxGID = objdbconn.GetExecuteScalar(msSQL);

                if (values.product_gid != null)
                {
                    lsproductgid1 = values.product_gid;
                    msSQL = "Select product_name from pmr_mst_tproduct where product_gid='" + lsproductgid1 + "'";
                    values.product_name = objdbconn.GetExecuteScalar(msSQL);
                }
                else
                {
                    msSQL = " Select product_gid from pmr_mst_tproduct where product_name = '" + values.product_name + "'";
                    lsproductgid1 = objdbconn.GetExecuteScalar(msSQL);
                }

                msSQL = "Select productuom_gid from pmr_mst_tproductuom where productuom_name='" + values.productuom_name + "'";
                string lsproductuomgid = objdbconn.GetExecuteScalar(msSQL);
                if (values.tax_gid == null)
                {
                    msSQL = "Select percentage from acp_mst_ttax where tax_name='" + values.tax_name + "'";
                    lspercentage1 = objdbconn.GetExecuteScalar(msSQL);
                }
                else
                {

                    msSQL = "Select percentage from acp_mst_ttax where tax_name='" + values.tax_name + "'";
                    lspercentage1 = objdbconn.GetExecuteScalar(msSQL);
                }
                msSQL = " SELECT a.producttype_name FROM pmr_mst_tproducttype a " +
                 " INNER JOIN pmr_mst_tproduct b ON a.producttype_gid=b.producttype_gid  " +
                 " WHERE product_gid='" + lsproductgid1 + "'";
                objodbcDataReader = objdbconn.GetDataReader(msSQL);

                if (objodbcDataReader.HasRows == true)
                {
                    lsproduct_type = "Sales";

                }
                else
                {
                    lsproduct_type = "Services";
                }
                objodbcDataReader.Close();

                msSQL = " select * from smr_tmp_tsalesorderdtl where product_gid='" + lsproductgid1 + "' and uom_gid='" + lsproductuomgid + "' " +
                        " and product_price='" + values.unitprice + "' and tax1_gid='" + values.tax_name + "'  and employee_gid='" + employee_gid + "' " +
                        " and tmpsalesorderdtl_gid = '" + values.tmpsalesorderdtl_gid + "' ";
                objodbcDataReader = objdbconn.GetDataReader(msSQL);
                if (objodbcDataReader.HasRows == true)
                {

                    msSQL = " update smr_tmp_tsalesorderdtl set qty_quoted='" + Convert.ToDouble(values.quantity) + Convert.ToDouble(objodbcDataReader["qty_quoted"].ToString()) + "' " +
                            " price='" + Convert.ToDouble(values.totalamount) + Convert.ToDouble(objodbcDataReader["price"].ToString()) + "' " +
                            " where tmpsalesorderdtl_gid='" + values.tmpsalesorderdtl_gid + "' ";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                    msSQL = " delete from smr_tmp_tsalesorderdtl where tmpsalesorderdtl_gid='" + values.tmpsalesorderdtl_gid + "' ";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                }
                else
                {
                    msSQL = " update smr_tmp_tsalesorderdtl set " +
                           " product_gid = '" + lsproductgid1 + "'," +
                           " product_name= '" + values.product_name + "'," +
                           " product_price='" + values.unitprice + "'," +
                           " qty_quoted='" + values.quantity + "'," +
                           " discount_percentage='" + values.discount_percentage + "'," +
                           " discount_amount='" + values.discountamount + "'," +
                           " uom_gid = '" + lsproductuomgid + "', " +
                           " uom_name='" + values.productuom_name + "'," +
                           " price='" + values.totalamount + "'," +
                           " employee_gid='" + employee_gid + "'," +
                           " tax_name= '" + values.tax_name + "'," +
                           //" tax1_gid= '" + TaxGID + "'," +
                           " tax_amount='" + values.tax_amount + "'," +
                           " order_type='" + lsproduct_type + "', ";

                    if (lspercentage1 == "" || lspercentage1 == null)
                    {
                        msSQL += " tax_percentage='0.00'";
                    }
                    else
                    {
                        msSQL += " tax_percentage='" + lspercentage1 + "'";
                    }
                    msSQL += " where tmpsalesorderdtl_gid='" + values.tmpsalesorderdtl_gid + "'";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                }
                objodbcDataReader.Close();

                if (mnResult != 0)
                {
                    values.status = true;
                    values.message = " Product Updated Successfully! ";

                }
                else
                {
                    values.status = false;
                    values.message = " Error While Updating Product! ";

                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while updating DirectSO Product !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }

        }
        public void DaGetsalesonupdate(string employee_gid, MdlSmrTrnSalesorder values)
        {
            try
            {



                msSQL = "  select distinct a.salesorder_gid, so_referenceno1, a.shopify_orderid,a.shopifyorder_number,a.shopifycustomer_id,DATE_FORMAT(a.salesorder_date, '%Y-%m-%d') as salesorder_date," +
                    " a.so_referencenumber,d.customer_gid,d.customer_name,d.customer_address, a.customer_contact_person as customer_contactperson,e.mobile,c.user_firstname, " +
                    " a.so_type, a.currency_code, a.salesorder_status,s.source_name,d.customer_code," +
                    " i.branch_name, a.grandtotal_l ,i.branch_gid, a.currency_code,a.so_type, a.created_by,a.created_date, " +
                    " f.product_gid,f.productgroup_gid,f.product_code, f.productgroup_name,f.product_name,f.product_price,f.qty_quoted," +
                    "f.discount_percentage,f.discount_amount,f.tax_percentage,f.tax_amount,f.uom_gid,f.uom_name,f.tax_name," +
                    "f.tax1_gid,f.display_field,f.selling_price,f.product_price_l,f.customerproduct_code,c.user_gid from smr_trn_tsalesorder a " +
                    " left join crm_mst_tcustomer d on a.customer_gid=d.customer_gid  " +
                    " left join crm_mst_tcustomercontact e on d.customer_gid=e.customer_gid   " +
                    " left join hrm_mst_temployee b on b.employee_gid=a.created_by  " +
                    " left join crm_trn_tcurrencyexchange h on a.currency_code = h.currency_code  " +
                    " left join adm_mst_tuser c on b.user_gid= c.user_gid   " +
                    " left join hrm_mst_tbranch i on a.branch_gid= i.branch_gid" +
                    " left join crm_trn_tleadbank l on l.customer_gid=a.customer_gid" +
                    " left join crm_mst_tsource s on s.source_gid=l.source_gid " +
                    " left join smr_trn_tsalesorderdtl f on f.salesorder_gid=a.salesorder_gid   " +
                    " where a.salesorder_status='paid' and a.shopify_orderid group by a.shopify_orderid ";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt1 in  dt_datatable.Rows)
                    {


                        msINGetGID = objcmnfunctions.GetMasterGID("SIVT");

                        msSQL = " insert into rbl_trn_tinvoice(" +
                        " invoice_gid," +
                        " invoice_date," +
                        " invoice_type," +
                        " invoice_status," +
                        " customer_gid," +
                        " customer_name," +
                        " customer_contactperson," +
                        " customer_contactnumber," +
                        " customer_address," +
                        " total_amount," +
                        " user_gid," +
                        " currency_code," +
                        " branch_gid " +
                        " ) values (" +
                        "'" + msINGetGID + "'," +
                        "'" + dt1["salesorder_date"].ToString() + "'," +
                        "'" + dt1["so_type"].ToString() + "'," +
                        " 'Payment Done'," +
                        "'" + dt1["customer_gid"].ToString() + "'," +
                        "'" + dt1["customer_name"].ToString() + "'," +
                        "'" + dt1["customer_contactperson"].ToString() + "'," +
                        "'" + dt1["mobile"].ToString() + "'," +
                        "'" + dt1["customer_address"].ToString() + "'," +
                        "'" + dt1["grandtotal_l"].ToString() + "'," +
                        "'" + dt1["user_gid"].ToString() + "'," +
                        "'" + dt1["currency_code"].ToString() + "'," +
                        "'" + dt1["branch_gid"].ToString() + "')";
                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                        if (mnResult == 1)
                        {
                            msGetGid = objcmnfunctions.GetMasterGID("SIVC");
                            msSQL = " insert into rbl_trn_tinvoicedtl ( " +
                                    " invoicedtl_gid," +
                                    " invoice_gid," +
                                    " product_gid," +
                                    " product_price," +
                                    " discount_percentage," +
                                    " discount_amount," +
                                    " uom_gid," +
                                    " tax_amount," +
                                    " tax_name," +
                                    " display_field," +
                                    " tax1_gid," +
                                    " product_name," +
                                    " product_code," +
                                    " customerproduct_code, " +
                                    " uom_name," +
                                    " productgroup_gid, " +
                                    " productgroup_name," +
                                    " selling_price," +
                                    " product_price_L " +
                                    " ) values ( " +
                                    "'" + msGetGid + "'," +
                                    "'" + msINGetGID + "'," +
                                    "'" + dt1["product_gid"].ToString() + "'," +
                                    "'" + dt1["product_price"].ToString() + "'," +
                                    "'" + dt1["discount_percentage"].ToString() + "'," +
                                    "'" + dt1["discount_amount"].ToString() + "'," +
                                    "'" + dt1["uom_gid"].ToString() + "'," +
                                    "'" + dt1["tax_amount"].ToString() + "'," +
                                    "'" + dt1["tax_name"].ToString() + "'," +
                                    "'" + dt1["display_field"].ToString() + "'," +
                                    "'" + dt1["tax1_gid"].ToString() + "'," +
                                    "'" + dt1["product_name"].ToString() + "'," +
                                    "'" + dt1["product_code"].ToString() + "'," +
                                    "'" + dt1["customerproduct_code"].ToString() + "'," +
                                    "'" + dt1["uom_name"].ToString() + "'," +
                                    "'" + dt1["productgroup_gid"].ToString() + "'," +
                                    "'" + dt1["productgroup_name"].ToString() + "'," +
                                    "'" + dt1["selling_price"].ToString() + "'," +
                                    "'" + dt1["product_price_L"].ToString() + "')";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                        }
                        if (mnResult == 1)
                        {
                            msGetinGid = objcmnfunctions.GetMasterGID("BPTP");

                            msSQL = " insert into rbl_trn_tpayment (" +
                                " payment_gid, " +
                                " payment_date," +
                                " invoice_gid," +
                                " total_amount," +
                                " branch," +
                                " created_by," +
                                " currency_code " + ") values (" +
                                "'" + msGetinGid + "'," +
                                 "'" + dt1["salesorder_date"].ToString() + "'," +
                                 "'" + msINGetGID + "'," +
                                "'" + dt1["grandtotal_l"].ToString() + "'," +
                                "'" + dt1["branch_name"].ToString() + "'," +
                                "'" + dt1["created_by"].ToString() + "'," +
                                "'" + dt1["currency_code"].ToString() + "')";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                        }
                    }
                }
                if (mnResult == 1)
                {
                    msSQL = "update smr_trn_tsalesorder set salesorder_status='Payment Done' where salesorder_status='paid' and shopify_orderid";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                }
                if (mnResult != 0)

                {
                    values.status = true;
                    values.message = "Status Updated Successfully!";

                }
                else
                {
                    values.status = false;
                    values.message = "Error While Updating Status!";

                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while Updating Sales Order !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }





        }
        public void DaGetupdate(string employee_gid, MdlSmrTrnSalesorder values)
        {
            try
            {


                msSQL = "SELECT salesorder_status,shopify_orderid FROM smr_trn_tsalesorder WHERE shopify_orderid and salesorder_status in('Pending')";
                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                if (mnResult != 0)
                {

                    msSQL = "update smr_trn_tsalesorder set salesorder_status='Approved' where shopify_orderid ";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                }

                if (mnResult != 0)
                {
                    values.status = true;
                    values.message = "Status Updated Successfully!";
                }
                else
                {
                    {
                        values.status = false;
                        values.message = "Error While Updating Status!";
                    }
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while Updating Status!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
        }
        public void DaGetCustomerDtl360(MdlSmrTrnSalesorder values, string leadbank_gid)
        {

            try
            {
                if (leadbank_gid.Contains("BCRM"))
                {

                    msSQL = "Select a.customer_gid, a.customer_name " +
                    " from crm_mst_tcustomer a where customer_gid='" + leadbank_gid + "' ";

                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                    var getModuleList = new List<GetCustomerDropdown>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in  dt_datatable.Rows)
                        {
                            getModuleList.Add(new GetCustomerDropdown

                            {
                                customer_gid = dt["customer_gid"].ToString(),
                                customer_name = dt["customer_name"].ToString(),

                            });
                            values.GetCustomerDtl = getModuleList;
                        }
                    }
                    dt_datatable.Dispose();
                }
                else
                {


                    msSQL = "Select customer_gid from crm_trn_tleadbank where leadbank_gid='" + leadbank_gid + "' ";
                    string lscustomer_gid = objdbconn.GetExecuteScalar(msSQL);

                    msSQL = "Select a.customer_gid, a.customer_name " +
                    " from crm_mst_tcustomer a where customer_gid='" + lscustomer_gid + "' ";

                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                    var getModuleList = new List<GetCustomerDropdown>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in  dt_datatable.Rows)
                        {
                            getModuleList.Add(new GetCustomerDropdown

                            {
                                customer_gid = dt["customer_gid"].ToString(),
                                customer_name = dt["customer_name"].ToString(),

                            });
                            values.GetCustomerDtl = getModuleList;
                        }
                    }
                    dt_datatable.Dispose();
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Customer dropdown CRM !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetsalesorderhistorysummarydata(MdlSmrTrnSalesorder values, string salesorder_gid)
        {
            try
            {

                msSQL = "select currency_code from crm_trn_tcurrencyexchange where default_currency='Y'";
                string currency = objdbconn.GetExecuteScalar(msSQL);

                msSQL = " select distinct a.salesorder_gid, a.so_referenceno1, date_format(a.salesorder_date,'%d-%m-%Y') as salesorder_date,c.user_firstname,a.so_type, " +
                "  a.customer_contact_person, a.salesorder_status,a.currency_code,i.leadbank_gid, " +
                " case when a.grandtotal_l ='0.00' then format(a.Grandtotal,2) else format(a.grandtotal_l,2) end as Grandtotal," +
                " case when a.currency_code = '" + currency + "' then a.customer_name " +
                "  when a.currency_code is null then a.customer_name " +
                "  when a.currency_code is not null and a.currency_code <> '" + currency + "' then concat(a.customer_name,' / ',h.country) end as customer_name, " +
                " case when a.customer_email is null then concat(e.customercontact_name,'/',e.mobile,'/',e.email) " +
                " when a.customer_email is not null then concat(a.customer_contact_person,' / ',a.customer_mobile,' / ',a.customer_email) end as contact,invoice_flag " +
                "  from smr_trn_tsalesorder a " +
                " left join crm_mst_tcustomer d on a.customer_gid=d.customer_gid " +
                 " left join crm_trn_tleadbank i on d.customer_gid=i.customer_gid " +
                " left join crm_mst_tcustomercontact e on d.customer_gid=e.customer_gid " +
                " left join hrm_mst_temployee b on b.employee_gid=a.created_by " +
                " left join crm_trn_tcurrencyexchange h on a.currency_code = h.currency_code " +
                " left join adm_mst_tuser c on b.user_gid= c.user_gid " +
                " where 1=1 and a.salesorder_status='SO Amended' and a.salesorder_gid like '" + salesorder_gid + "%'" +
                " order by  a.salesorder_gid desc ";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesorderhistorysummary_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new salesorderhistorysummary_list
                        {
                            salesorder_date = dt["salesorder_date"].ToString(),
                            so_referenceno1 = dt["so_referenceno1"].ToString(),
                            customer_name = dt["customer_name"].ToString(),
                            customer_contact_person = dt["customer_contact_person"].ToString(),
                            user_firstname = dt["user_firstname"].ToString(),
                            Grandtotal = dt["Grandtotal"].ToString(),
                            salesorder_status = dt["salesorder_status"].ToString(),
                            salesorder_gid = dt["salesorder_gid"].ToString(),
                            leadbank_gid = dt["leadbank_gid"].ToString(),


                        });
                        values.salesorderhistorysummarylist = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while Getting Quotation history!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }

        }
        public void DaGetsalesorderhistorydata(MdlSmrTrnSalesorder values, string salesorder_gid)
        {
            try
            {


                msSQL = " select a.salesorder_gid, h.leadbank_gid,date_format(a.salesorder_date,'%d-%m-%Y') as salesorder_date,a.customer_name, a.so_referenceno1," +
                        " a.so_remarks,f.enquiry_referencenumber from smr_trn_tsalesorder a " +
                        " left join crm_mst_tcustomer b on a.customer_gid = b.customer_gid " +
                        " left join acp_trn_tenquiry f on a.customer_gid=f.customer_gid " +
                        " left join crm_trn_tleadbank h on b.customer_gid=h.customer_gid " +
                        "  where a.salesorder_gid='" + salesorder_gid + "'";


                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesorderhistorylist>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new salesorderhistorylist
                        {
                            salesorder_gid = dt["salesorder_gid"].ToString(),
                            so_referenceno1 = dt["so_referenceno1"].ToString(),
                            salesorder_date = dt["salesorder_date"].ToString(),
                            customer_name = dt["customer_name"].ToString(),
                            leadbank_gid = dt["leadbank_gid"].ToString(),
                            so_remarks = dt["so_remarks"].ToString(),

                        });
                        values.salesorderhistorylist = getModuleList;
                    }
                    dt_datatable.Dispose();
                }
            }


            catch (Exception ex)
            {
                ex.Message.ToString();

            }
        }
        public void DaGetsalesorderproductdetails(string salesorder_gid, MdlSmrTrnSalesorder values)
        {
            try
            {

                msSQL = " select a.qty_quoted,a.product_name from smr_trn_tsalesorderdtl a " +
                    " left join pmr_mst_tproduct b on a.product_gid = b.product_gid where a.salesorder_gid='" + salesorder_gid + "'";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesorderproduct_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new salesorderproduct_list
                        {

                            product_name = dt["product_name"].ToString(),
                            qty_quoted = dt["qty_quoted"].ToString(),


                        });
                        values.salesorderproduct_list = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while Getting Product!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }

        }

        /// <------------------------------------------------new sales order------------------------------------------------------------------>
        public void DaGetTax4Dtl(MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = " select tax_prefix,tax_gid,percentage from acp_mst_ttax where active_flag='Y'  and reference_type='Customer'";

               DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetTaxFourDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetTaxFourDropdown

                        {
                            tax_gid = dt["tax_gid"].ToString(),
                            tax_name4 = dt["tax_prefix"].ToString(),
                            percentage = dt["percentage"].ToString()

                        });
                        values.GetTax4Dtl = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Tax Dropdown !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetCurrencyDtl(MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = "  select currencyexchange_gid,currency_code,default_currency,exchange_rate" +
                    " from crm_trn_tcurrencyexchange order by currency_code asc";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                var getModuleList = new List<GetCurrencyDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetCurrencyDropdown

                        {
                            currencyexchange_gid = dt["currencyexchange_gid"].ToString(),
                            currency_code = dt["currency_code"].ToString(),
                            default_currency = dt["default_currency"].ToString(),
                            exchange_rate = dt["exchange_rate"].ToString(),

                        });
                        values.GetCurrencyDtl = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Currenct Dropdown !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetPersonDtl(MdlSmrTrnSalesorder values)
        {
            try
            {



                msSQL = " select a.employee_gid,c.user_gid,e.campaign_gid,concat(e.campaign_title, ' | ', c.user_code, ' | ', c.user_firstname, ' ', c.user_lastname)AS employee_name, e.campaign_title " +
                        " from adm_mst_tmodule2employee a " +
                        " left join hrm_mst_temployee b on a.employee_gid=b.employee_gid " +
                        " left join adm_mst_tuser c on b.user_gid=c.user_gid " +
                        " left join smr_trn_tcampaign2employee d on a.employee_gid=d.employee_gid " +
                        " left join smr_trn_tcampaign e on e.campaign_gid = d.campaign_gid " +
                        " where a.module_gid = 'SMR' and a.hierarchy_level<>'-1' and a.employee_gid in  " +
                        " (select employee_gid from smr_trn_tcampaign2employee where 1=1) group by employee_name asc; ";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetPersonDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetPersonDropdown

                        {
                            user_gid = dt["user_gid"].ToString() + '.' + dt["campaign_title"].ToString() + '.' + dt["campaign_gid"].ToString(),
                            user_name = dt["employee_name"].ToString(),

                        }); ;
                        values.GetPersonDtl = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Person Dropdown !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetCustomerDtl(MdlSmrTrnSalesorder values)
        {
            try
            {



                msSQL = " select concat(a.customer_name,' / ', b.email) as customer_name ,a.customer_gid from crm_mst_tcustomer a " +
                        " left join crm_mst_tcustomercontact b on a.customer_gid=b.customer_gid " +
                        " where status='Active'";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetCustomerDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetCustomerDropdown

                        {
                            customer_gid = dt["customer_gid"].ToString(),
                            customer_name = dt["customer_name"].ToString(),

                        });
                        values.GetCustomerDtl = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Customer Dropdown !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetBranchDtl(MdlSmrTrnSalesorder values)
        {
            try
            {



                msSQL = "select branch_gid,branch_name, address1 from hrm_mst_tbranch";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetBranchDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetBranchDropdown

                        {
                            branch_gid = dt["branch_gid"].ToString(),
                            branch_name = dt["branch_name"].ToString(),
                            address1 = dt["address1"].ToString(),

                        });
                        values.GetBranchDtl = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Branch Dropdown !";

                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }

        }
        public void DaGetproducttypesales(MdlSmrTrnSalesorder values)
        {
            try
            {

                msSQL = " select producttype_gid, producttype_code , producttype_name from pmr_mst_tproducttype";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<Getproducttypesales>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new Getproducttypesales
                        {
                            producttype_gid = dt["producttype_gid"].ToString(),
                            producttype_code = dt["producttype_code"].ToString(),
                            producttype_name = dt["producttype_name"].ToString(),
                        });
                        values.Getproducttypesales = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while getting product type !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
              $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" +
              ex.Message.ToString() + "***********" + values.message.ToString() + "*****Query****" +
              msSQL + "*******Apiref********", "ErrorLog/Purchase/" + "Log" +
              DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        private DataTable GetTaxDetailsForProduct(string product_gid, string customer_gid)
        {
            // Query tax segment details based on product_gid
            msSQL = "SELECT f.taxsegment_gid, d.taxsegment_gid, e.taxsegment_name, d.tax_name, d.tax_gid, " +
                "case when d.tax_percentage = ROUND(d.tax_percentage) then ROUND(d.tax_percentage) else d.tax_percentage end AS tax_percentage, " +
                "d.tax_amount, a.mrp_price, a.cost_price, a.product_gid " +
                "FROM acp_mst_ttaxsegment2product d " +
                "LEFT JOIN acp_mst_ttaxsegment e ON e.taxsegment_gid = d.taxsegment_gid " +
                "LEFT JOIN crm_mst_tcustomer f ON f.taxsegment_gid = e.taxsegment_gid " +
                "LEFT JOIN pmr_mst_tproduct a ON a.product_gid = d.product_gid " +
                "WHERE a.product_gid = '" + product_gid + "' AND f.customer_gid = '" + customer_gid + "'";

            // Execute query to get tax segment details
            return objdbconn.GetDataTable(msSQL);
        }
        public void DaProductSalesSummary(string? customer_gid, MdlSmrTrnSalesorder values, string? smryproduct_gid)
        {
            try
            {
               
                double grand_total = 0.00;
                double grandtotal = 0.00;               

                msSQL = "SELECT a.tmpsalesorderdtl_gid,a.taxsegment_gid,a.salesorder_gid, a.tax_name,format(a.tax_amount,2) as tax_amount,a.tax_percentage,a.tax_name2,format(a.tax_amount2,2) as tax_amount2,a.tax_percentage2," +
                       " a.tax_name3,format(a.tax_amount3,2) as tax_amount3,a.tax_percentage3,format(a.tax_amount,2) as tax_amount, a.salesorderdtl_gid, a.salesorder_gid, a.product_gid, a.productgroup_gid,  b.productgroup_name, " +
                       " a.product_name, FORMAT(a.product_price, 2) AS product_price, a.product_code, a.qty_quoted, a.product_remarks,  a.uom_gid, a.vendor_gid, a.slno, a.uom_name," +
                       " FORMAT(a.price, 2) AS price, FORMAT(a.discount_percentage, 2) AS discount_percentage, FORMAT(a.discount_amount, 2) AS discount_amount, FORMAT(a.selling_price, '0.00') AS selling_price," +
                       " a.product_remarks,  CONCAT(CASE WHEN a.tax_name IS NULL THEN '' ELSE a.tax_name END, ' ', CASE WHEN a.tax_percentage = '0' THEN '' ELSE concat('', a.tax_percentage, '%') END," +
                       " CASE WHEN a.tax_amount = '0' THEN '' ELSE concat(':', a.tax_amount) END) AS tax, CONCAT(CASE WHEN a.tax_name2 IS NULL THEN '' ELSE a.tax_name2 END, ' '," +
                       " CASE WHEN a.tax_percentage2 = '0' THEN '' ELSE concat('', a.tax_percentage2, '%') END, CASE WHEN a.tax_amount2 = '0' THEN '' ELSE concat(':', a.tax_amount2) END) AS tax2," +
                       " CONCAT(  CASE WHEN a.tax_name3 IS NULL THEN '' ELSE a.tax_name3 END, ' ', CASE WHEN a.tax_percentage3 = '0' THEN '' ELSE concat('', a.tax_percentage3, ' %   ') END," +
                       " CASE WHEN a.tax_amount3 = '0' THEN '  ' ELSE concat(' : ', a.tax_amount3) END) AS tax3, format(a.tax_amount + a.tax_amount2 + a.tax_amount3, 2) as taxamount,a.tax_rate " +
                       " FROM smr_tmp_tsalesorderdtl a " +
                  "  left join pmr_mst_tproductgroup b on a.productgroup_gid = b.productgroup_gid" +
                  "  WHERE a.employee_gid = '" + employee_gid + "'  order by(a.slno+0) asc";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesorders_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        grand_total += Math.Round(double.Parse(dt["price"].ToString()), 2);
                        grandtotal += Math.Round(double.Parse(dt["price"].ToString()), 2);
                        getModuleList.Add(new salesorders_list
                        {
                            tmpsalesorderdtl_gid = dt["tmpsalesorderdtl_gid"].ToString(),
                            salesorder_gid = dt["salesorder_gid"].ToString(),
                            product_name = dt["product_name"].ToString(),
                            product_gid = dt["product_gid"].ToString(),
                            product_code = dt["product_code"].ToString(),
                            slno = dt["slno"].ToString(),
                            discountamount = dt["discount_amount"].ToString(),
                            discountpercentage = double.Parse(dt["discount_percentage"].ToString()),
                            productgroup_name = dt["productgroup_name"].ToString(),
                            product_price = dt["product_price"].ToString(),
                            quantity = dt["qty_quoted"].ToString(),
                            uom_gid = dt["uom_gid"].ToString(),
                            productuom_name = dt["uom_name"].ToString(),
                            producttotalamount = dt["price"].ToString(),
                            totalamount = dt["price"].ToString(),
                            taxname1 = dt["tax_name"].ToString(),
                            taxname2 = dt["tax_name2"].ToString(),
                            taxname3 = dt["tax_name3"].ToString(),
                            tax_amount = dt["tax_amount"].ToString(),
                            tax_amount2 = dt["tax_amount2"].ToString(),
                            tax_amount3 = dt["tax_amount3"].ToString(),
                            tax_percentage = dt["tax_percentage"].ToString(),
                            tax_percentage2 = dt["tax_percentage2"].ToString(),
                            tax_percentage3 = dt["tax_percentage3"].ToString(),
                            grand_total = dt["price"].ToString(),
                            grandtotal = dt["price"].ToString(),
                            taxsegment_gid = dt["taxsegment_gid"].ToString(),
                            tax = dt["tax"].ToString(),
                            tax2 = dt["tax2"].ToString(),
                            tax3 = dt["tax3"].ToString(),
                            product_remarks = dt["product_remarks"].ToString(),
                            taxamount = dt["taxamount"].ToString(),
                            tax_rate = dt["tax_rate"].ToString(),
                            tax_prefix1 = dt["tax_name"].ToString(),
                            tax_prefix2 = dt["tax_name2"].ToString(),
                        });
                    }
                    values.salesorders_list = getModuleList;
                }

                dt_datatable.Dispose();
                values.grand_total = Math.Round(grand_total, 2);
                values.grandtotal = Math.Round(grandtotal, 2);
            }
            catch (Exception ex)
            {
                values.message = "Exception occurred while getting product summary!";
                objcmnfunctions.LogForAudit(
                    "*******Date*****" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "***********" +
                    $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" +
                    ex.Message.ToString() + "***********" + values.message.ToString() + "*****Query****" +
                    msSQL + "*******Apiref********", "ErrorLog/Purchase/Log" +
                    DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }


        }


        public void DaGetProductsearchSummarySales(MdlSmrTrnSalesorder values)
        {
            try
            {
                string lsSqlType = "product";

                msSQL = " call pmr_mst_spproductsearch('" + lsSqlType + "','','')";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                var getModuleList = new List<GetProductsearchs>();
                var allTaxSegmentsList = new List<GetTaxSegmentListorder>();

                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        var product = new GetProductsearchs
                        {
                            product_name = dt["product_name"].ToString(),
                            product_gid = dt["product_gid"].ToString(),
                            product_code = dt["product_code"].ToString(),
                            productuom_name = dt["productuom_name"].ToString(),
                            productgroup_name = dt["productgroup_name"].ToString(),
                            productuom_gid = dt["productuom_gid"].ToString(),
                            producttype_gid = dt["producttype_gid"].ToString(),
                            productgroup_gid = dt["productgroup_gid"].ToString(),
                            unitprice = dt["mrp_price"].ToString(),
                            product_desc = dt["product_desc"].ToString(),
                            quantity = 0,
                            total_amount = 0,
                            discount_percentage = 0,
                            discount_amount = 0,
                        };
                        getModuleList.Add(product);

                        //if (!string.IsNullOrEmpty(customer_gid) && customer_gid != "undefined")
                        //{
                        //    string productGid = product.product_gid;
                        //    string productName = product.product_name;

                        //    string lsSQLTYPE = "customer";

                        //    msSQL = "call pmr_mst_spproductsearch('" + lsSQLTYPE + "','" + productGid + "', '" + customer_gid + "')";
                        //    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                        //    if (dt_datatable.Rows.Count != 0)
                        //    {
                        //        foreach (DataRow dt1 in  dt_datatable.Rows)
                        //        {
                        //            allTaxSegmentsList.Add(new GetTaxSegmentListorder
                        //            {
                        //                taxsegment_gid = dt1["taxsegment_gid"].ToString(),
                        //                taxsegment_name = dt1["taxsegment_name"].ToString(),
                        //                tax_name = dt1["tax_name"].ToString(),
                        //                tax_percentage = dt1["tax_percentage"].ToString(),
                        //                tax_gid = dt1["tax_gid"].ToString(),
                        //                mrp_price = dt1["mrp_price"].ToString(),
                        //                cost_price = dt1["cost_price"].ToString(),
                        //                tax_amount = dt1["tax_amount"].ToString(),
                        //                product_name = dt["product_name"].ToString(),
                        //                product_gid = dt["product_gid"].ToString(),
                        //                product_code = dt["product_code"].ToString(),
                        //                productuom_name = dt["productuom_name"].ToString(),
                        //                productgroup_name = dt["productgroup_name"].ToString(),
                        //                productuom_gid = dt["productuom_gid"].ToString(),
                        //                producttype_gid = dt["producttype_gid"].ToString(),
                        //                productgroup_gid = dt["productgroup_gid"].ToString(),
                        //                unitprice = dt["cost_price"].ToString(),
                        //                quantity = 0,
                        //                total_amount = 0,
                        //                discount_percentage = 0,
                        //                discount_amount = 0,
                        //            });
                        //        }
                        //    }
                        //}
                    }
                    values.GetProductsearchs = getModuleList; // Assign GetProductsearch to values.GetProductsearch
                }
                //values.GetTaxSegmentListorder = allTaxSegmentsList; // Assign allTaxSegmentsList to values.GetTaxSegmentList
            }
            catch (Exception ex)
            {
                values.message = "Exception occurred while changing product!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                  $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" +
                  ex.Message.ToString() + "***********" + values.message.ToString() + "*****Query****" +
                  msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" +
                  DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
        }
        public void DaGetProductWithTaxSummary(string? product_gid, string? customer_gid, MdlSmrTrnSalesorder values)
        {
            msSQL1 = "select pricesegment_gid from crm_mst_tcustomer where customer_gid = '" + customer_gid + "'";
            string pricesegment_gid = objdbconn.GetExecuteScalar(msSQL1);
            msSQL = "select * from smr_trn_tpricesegment2product where pricesegment_gid = '" + pricesegment_gid + "' and product_gid = '" + product_gid + "'";
            objodbcDataReader = objdbconn.GetDataReader(msSQL);
            if (objodbcDataReader.HasRows)
            {

                string lsSQLTYPE = "pricesegmentcustomer";
                msSQL = "call pmr_mst_spproductsearch('" + lsSQLTYPE + "','" + product_gid + "', '" + customer_gid + "')";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var allTaxSegmentsList = new List<GetTaxSegmentListorder>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt1 in  dt_datatable.Rows)
                    {
                        allTaxSegmentsList.Add(new GetTaxSegmentListorder
                        {
                            taxsegment_gid = dt1["taxsegment_gid"].ToString(),
                            taxsegment_name = dt1["taxsegment_name"].ToString(),
                            tax_name = dt1["tax_name"].ToString(),
                            tax_percentage = dt1["tax_percentage"].ToString(),
                            tax_gid = dt1["tax_gid"].ToString(),
                            mrp_price = dt1["product_price"].ToString(),
                            cost_price = dt1["cost_price"].ToString(),
                            tax_amount = dt1["tax_amount"].ToString(),
                            product_name = dt1["product_name"].ToString(),
                            product_gid = dt1["product_gid"].ToString(),
                            tax_prefix = dt1["tax_prefix"].ToString(),
                        });
                        values.GetTaxSegmentListorder = allTaxSegmentsList;
                    }
                }

            }
            else
            {
                string lsSQLTYPE = "customer";
                msSQL = "call pmr_mst_spproductsearch('" + lsSQLTYPE + "','" + product_gid + "', '" + customer_gid + "')";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var allTaxSegmentsList = new List<GetTaxSegmentListorder>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt1 in  dt_datatable.Rows)
                    {
                        allTaxSegmentsList.Add(new GetTaxSegmentListorder
                        {
                            taxsegment_gid = dt1["taxsegment_gid"].ToString(),
                            taxsegment_name = dt1["taxsegment_name"].ToString(),
                            tax_name = dt1["tax_name"].ToString(),
                            tax_percentage = dt1["tax_percentage"].ToString(),
                            tax_gid = dt1["tax_gid"].ToString(),
                            mrp_price = dt1["mrp_price"].ToString(),
                            cost_price = dt1["cost_price"].ToString(),
                            tax_amount = dt1["tax_amount"].ToString(),
                            product_name = dt1["product_name"].ToString(),
                            product_gid = dt1["product_gid"].ToString(),
                            tax_prefix = dt1["tax_prefix"].ToString(),
                        });
                        values.GetTaxSegmentListorder = allTaxSegmentsList;
                    }
                }
            }
            objodbcDataReader.Close();

        }
        public void GetDeleteDirectSOProductSummary(string? tmpsalesorderdtl_gid, salesorders_list values)
        {
            try
            {



                msSQL = "select price from smr_tmp_tsalesorderdtl " +
                    " where tmpsalesorderdtl_gid='" + tmpsalesorderdtl_gid + "'";
                objodbcDataReader = objdbconn.GetDataReader(msSQL);
                if (objodbcDataReader.HasRows == true)

                {

                    lsprice = objodbcDataReader["price"].ToString();

                }
                objodbcDataReader.Close();

                msSQL = " delete from smr_tmp_tsalesorderdtl " +
                        " where tmpsalesorderdtl_gid='" + tmpsalesorderdtl_gid + "'";
                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);



                if (mnResult != 0)

                {
                    values.status = true;
                    values.message = "Product Deleted Successfully!";

                }
                else
                {
                    values.status = false;
                    values.message = "Error While Deleting The Product!";


                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while deleting DirectSO product!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaUpdateSalesOrder(string employee_gid, postsales_list values)
        {
            try
            {

                string totalvalue = values.user_name;



                msSQL = " select company_code from adm_mst_tcompany";
                lscompany_code = objdbconn.GetExecuteScalar(msSQL);



                string lscustomerbranch = "H.Q";
                string lscampaign_gid = "NO CAMPAIGN";

                msSQL = " select * from smr_tmp_tsalesorderdtl " +
                    " where employee_gid='" + employee_gid + "'";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                if (dt_datatable.Rows.Count != 0)
                {

                    string inputDate = values.salesorder_date;
                    DateTime uiDate = DateTime.ParseExact(inputDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string salesorder_date = uiDate.ToString("yyyy-MM-dd");



                    msSQL = " select customer_name from crm_mst_tcustomer where customer_gid='" + values.customer_gid + " '";
                    string lscustomername = objdbconn.GetExecuteScalar(msSQL);
                    msSQL = " select branch_gid from hrm_mst_Tbranch where branch_name='" + values.branch_name + " '";
                    string lsbranchgid = objdbconn.GetExecuteScalar(msSQL);

                    msSQL = " select currency_code from crm_trn_tcurrencyexchange where currencyexchange_gid='" + values.currency_code + " '";
                    string currency_code = objdbconn.GetExecuteScalar(msSQL);

                    msSQL = " select tax_prefix from acp_mst_Ttax where tax_prefix='" + values.tax_gid + " '";
                    string lstax_name = objdbconn.GetExecuteScalar(msSQL);

                    msSQL = " select user_name from adm_mst_Tuser  where user_gid='" + values.salesperson_gid + " '";
                    string lssalesperson_name = objdbconn.GetExecuteScalar(msSQL);

                    string lslocaladdon = "0.00";
                    string lslocaladditionaldiscount = "0.00";
                    string lslocalgrandtotal = " 0.00";
                    string lsgst = "0.00";
                    string lsamount4 = "0.00";
                    //string lsproducttotalamount = "0.00";

                    double totalAmount = double.TryParse(values.tax_amount4, out double totalpriceValue) ? totalpriceValue : 0;
                    double addonCharges = double.TryParse(values.addon_charge, out double addonChargesValue) ? addonChargesValue : 0;
                    double freightCharges = double.TryParse(values.freight_charges, out double freightChargesValue) ? freightChargesValue : 0;
                    double packingCharges = double.TryParse(values.packing_charges, out double packingChargesValue) ? packingChargesValue : 0;
                    double insuranceCharges = double.TryParse(values.insurance_charges, out double insuranceChargesValue) ? insuranceChargesValue : 0;
                    double roundoff = double.TryParse(values.roundoff, out double roundoffValue) ? roundoffValue : 0;
                    double additionaldiscountAmount = double.TryParse(values.additional_discount, out double discountAmountValue) ? discountAmountValue : 0;
                    double buybackCharges = double.TryParse(values.buyback_charges, out double buybackChargesValue) ? buybackChargesValue : 0;

                    double grandTotal = (totalAmount + addonCharges + freightCharges + packingCharges + insuranceCharges + roundoff) - additionaldiscountAmount - buybackCharges;





                    msSQL = " update smr_trn_tsalesorder  set " +
                             "  salesorder_gid='" + values.salesorder_gid + "'," +
                              " branch_gid='" + lsbranchgid + "'," +
                             " salesorder_date='" + salesorder_date + "'," +
                             " customer_gid='" + values.customer_gid + "'," +
                             " renewal_flag='" + values.renewal_mode + "'," +
                             " customer_name='" + lscustomername.Replace("'", "\\\'") + "'," +
                             " customer_address ='" + values.address1.Replace("'", "\\\'") + "'," +
                             " created_by='" + employee_gid + "'," +
                             " so_referenceno1='" + values.so_referencenumber + "' ,";
                    if (values.so_remarks != null)
                    {
                        msSQL += " so_remarks='" + values.so_remarks.Replace("'", "\\\'") + "',";
                    }
                    else
                    {
                        msSQL += " so_remarks='" + values.so_remarks + "',";
                    }

                    msSQL += " payment_days='" + values.payment_days + "', " +
                       " delivery_days='" + values.delivery_days + "', " +
                       " Grandtotal='" + values.grandtotal.Replace(",", "").Trim() + "', ";
                    if(values.termsandconditions != null)
                    {
                        msSQL += " termsandconditions='" + values.termsandconditions.Replace("'", "\\\'") + "', ";
                    }
                    else
                    {
                        msSQL += " termsandconditions='" + values.termsandconditions + "', ";
                    }
                     
                     msSQL +=  " addon_charge='" + values.addon_charge.Trim() + "', " +
                       " additional_discount='" + values.additional_discount + "', " +
                       " addon_charge_l='" + values.addon_charge.Trim() + "', " +
                       " additional_discount_l='" + values.additional_discount + "', " +
                       " grandtotal_l='" + values.grandtotal.Replace(",", "").Trim() + "', " +
                       " currency_code='" + currency_code + "', " +
                       " currency_gid='" + values.currency_code + "', " +
                       " exchange_rate='" + values.exchange_rate + "', " +
                       " shipping_to='" + values.shipping_to.Replace("'", "\\\'") + "', " +
                       " tax_gid='" + lstax_name + "'," +
                       " tax_name ='" + values.tax_gid + "'," +
                       " gst_amount='" + lsgst + "'," +
                       " freight_charges='" + values.freight_charges + "'," +
                       " total_price='" + values.totalamount.Replace(",", "").Trim() + "'," +
                       " total_amount='" + values.grandtotal.Replace(",", "").Trim() + "',";
                    if (values.tax_amount4 == null || values.tax_amount4 == "")
                    {
                        msSQL += " tax_amount='0',";

                    }
                    else
                    {
                        msSQL += " tax_amount='" + values.tax_amount4.Replace(",", "").Trim() + "',";
                    }


                    msSQL += " salesperson_gid='" + values.user_name + "'," +
                             " roundoff='" + values.roundoff + "', " +
                             " updated_date= '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                              " where salesorder_gid='" + values.salesorder_gid + "'";


                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                    if (mnResult == 0)
                    {
                        values.status = false;
                        values.message = " Some Error Occurred While Inserting Salesorder Details";
                        return;
                    }
                    else
                    {
                        msSQL = " update acp_trn_Torder  set " +
                             "  salesorder_gid='" + values.salesorder_gid + "'," +
                             " branch_gid='" + lsbranchgid + "'," +
                             " salesorder_date='" + salesorder_date + "'," +
                             " customer_gid='" + values.customer_gid + "'," +
                             " customer_name='" + lscustomername.Replace("'", "\\\'") + "'," +
                             " customer_address ='" + values.address1.Replace("'", "\\\'") + "'," +
                             " created_by='" + employee_gid + "'," +
                             " so_referenceno1='" + values.so_referencenumber + "' ,";
                        if (values.so_remarks != null)
                        {
                            msSQL += " so_remarks='" + values.so_remarks.Replace("'", "\\\'") + "',";
                        }
                        else
                        {
                            msSQL += " so_remarks='" + values.so_remarks + "',";
                        }
                        msSQL += " payment_days='" + values.payment_days + "', " +
                               " delivery_days='" + values.delivery_days + "', " +
                               " Grandtotal='" + values.grandtotal.Replace(",", "").Trim() + "', " +
                               " termsandconditions='" + values.termsandconditions.Replace("'", "\\\'") + "', " +
                               " addon_charge='" + values.addon_charge + "', " +
                               " additional_discount='" + values.additional_discount + "', " +
                               " addon_charge_l='" + values.addon_charge + "', " +
                               " additional_discount_l='" + values.additional_discount + "', " +
                               " grandtotal_l='" + values.grandtotal.Replace(",", "").Trim() + "', " +
                               " currency_code='" + currency_code + "', " +
                               " currency_gid='" + values.currency_code + "', " +
                               " exchange_rate='" + values.exchange_rate + "', " +
                               " shipping_to='" + values.shipping_to.Replace("'", "\\\'") + "', " +
                               " total_amount='" + values.grandtotal.Replace(",", "").Trim() + "'," +
                               " salesperson_gid='" + values.user_name + "'," +
                               " roundoff='" + values.roundoff + "', " +
                               " updated_date= '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                                " where salesorder_gid='" + values.salesorder_gid + "'";


                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                        mnResult2 = objdbconn.ExecuteNonQuerySQL(msSQL);
                        if (mnResult2 == 1)
                        {
                            values.status = true;
                        }

                    }
                    if (values.renewal_mode == "Y")
                    {
                        string inputDate1 = values.renewal_date;
                        DateTime uiDate1 = DateTime.ParseExact(inputDate1, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        string renewal_date = uiDate1.ToString("yyyy-MM-dd");

                        msSQL = " update crm_trn_trenewal  set " +
                             " renewal_flag='" + values.renewal_mode + "'," +
                             " renewal_date ='" + renewal_date + "'," +
                             " frequency_term ='" + values.frequency_term + "'," +
                             " customer_gid='" + values.customer_gid + "'," +
                             " created_by='" + employee_gid + "'," +
                             " updated_date= '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                             " where salesorder_gid='" + values.salesorder_gid + "'";
                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    }




                    msSQL = " select " +
                            " tmpsalesorderdtl_gid," +
                            " salesorder_gid," +
                            " product_gid," +
                            " product_name," +
                             " product_code," +
                              " productgroup_gid," +
                            " product_price," +
                            " qty_quoted," +
                            " discount_percentage," +
                            " discount_amount," +
                            " uom_gid," +
                            " uom_name," +
                            " price," +
                            " tax_name," +
                            " tax_name2," +
                            " tax1_gid, " +
                             " tax2_gid, " +
                            " tax_amount," +
                            " tax_amount2," +
                            " slno," +
                            " tax_percentage," +
                            " tax_percentage2," +
                            " order_type, " +
                              " product_remarks, " +
                                " display_field, " +
                            " taxsegment_gid, " +
                            " taxsegmenttax_gid " +
                            " from smr_tmp_tsalesorderdtl where employee_gid = '" + employee_gid + "' ";

                    DataTable dt_datatable6 = objdbconn.GetDataTable(msSQL);
                    var getModuleList = new List<postsales_list>();
                    if (dt_datatable6.Rows.Count != 0)
                    {
                        msSQL = "delete from smr_trn_tsalesorderdtl where salesorder_gid = '" + values.salesorder_gid + "'";
                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                        msSQL = "delete from acp_trn_torderdtl where salesorder_gid = '" + values.salesorder_gid + "'";
                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                        int i = 0;
                        foreach (DataRow dt in dt_datatable6.Rows)
                        {
                            mssalesorderGID1 = objcmnfunctions.GetMasterGID("VSDC");

                            string display_field = dt["product_remarks"].ToString();

                            msSQL = " insert into smr_trn_tsalesorderdtl (" +
                                 " salesorderdtl_gid ," +
                                 " salesorder_gid," +
                                 " product_gid ," +
                                 " product_name," +
                                 " product_code," +
                                 " productgroup_gid," +
                                 " product_price," +
                                 " qty_quoted," +
                                 " discount_percentage," +
                                 " discount_amount," +
                                 " tax_amount ," +
                                 " tax_amount2 ," +
                                 " uom_gid," +
                                 " uom_name," +
                                 " price," +
                                 " tax_name," +
                                 " tax_name2," +
                                 " tax1_gid," +
                                 " tax2_gid," +
                                 " slno," +
                                  " product_remarks," +
                                   " display_field," +
                                 " tax_percentage," +
                                 " tax_percentage2," +
                                 " taxsegment_gid," +
                                 " taxsegmenttax_gid," +
                                 " type " +
                                 ")values(" +
                                 " '" + mssalesorderGID1 + "'," +
                                 " '" + values.salesorder_gid + "'," +
                                 " '" + dt["product_gid"].ToString() + "'," +
                                 " '" + dt["product_name"].ToString() + "'," +
                                 " '" + dt["product_code"].ToString() + "'," +
                                  " '" + dt["productgroup_gid"].ToString() + "'," +
                                 " '" + dt["product_price"].ToString() + "'," +
                                 " '" + dt["qty_quoted"].ToString() + "'," +
                                 " '" + dt["discount_percentage"].ToString() + "'," +
                                 " '" + dt["discount_amount"].ToString() + "'," +
                                 " '" + dt["tax_amount"].ToString() + "'," +
                                 " '" + dt["tax_amount2"].ToString() + "'," +
                                 " '" + dt["uom_gid"].ToString() + "'," +
                                 " '" + dt["uom_name"].ToString() + "'," +
                                 " '" + dt["price"].ToString() + "'," +
                                 " '" + dt["tax_name"].ToString() + "'," +
                                 " '" + dt["tax_name2"].ToString() + "'," +
                                 " '" + dt["tax1_gid"].ToString() + "'," +
                                 " '" + dt["tax2_gid"].ToString() + "'," +
                                 " '" + i + 1 + "',";
                            if (display_field != null)
                            {
                                msSQL += " '" + display_field.Replace("'", "\\\'") + "'," +
                                       " '" + display_field.Replace("'", "\\\'") + "',";
                            }
                            else
                            {
                                msSQL += " '" + display_field + "'," +
                                      " '" + display_field + "',";
                            }

                            msSQL += " '" + dt["tax_percentage"].ToString() + "'," +
                               " '" + dt["tax_percentage2"].ToString() + "'," +
                               " '" + dt["taxsegment_gid"].ToString() + "'," +
                               " '" + dt["taxsegmenttax_gid"].ToString() + "'," +
                               " '" + dt["order_type"].ToString() + "')";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                            msSQL = " insert into acp_trn_torderdtl (" +
                       " salesorderdtl_gid ," +
                       " salesorder_gid," +
                       " product_gid ," +
                       " product_name," +
                       " product_price," +
                       " qty_quoted," +
                       " discount_percentage," +
                       " discount_amount," +
                       " tax_amount ," +
                       " uom_gid," +
                       " uom_name," +
                       " price," +
                       " tax_name," +
                       " tax1_gid," +
                       " slno," +
                       " tax_percentage," +
                       " taxsegment_gid," +
                       " type, " +
                       " salesorder_refno" +
                       ")values(" +
                       " '" + mssalesorderGID1 + "'," +
                       " '" + values.salesorder_gid + "'," +
                       " '" + dt["product_gid"].ToString() + "'," +
                       " '" + dt["product_name"].ToString() + "'," +
                       " '" + dt["product_price"].ToString() + "'," +
                       " '" + dt["qty_quoted"].ToString() + "'," +
                       " '" + dt["discount_percentage"].ToString() + "'," +
                       " '" + dt["discount_amount"].ToString() + "'," +
                       " '" + dt["tax_amount"].ToString() + "'," +
                       " '" + dt["uom_gid"].ToString() + "'," +
                       " '" + dt["uom_name"].ToString() + "'," +
                       " '" + dt["price"].ToString() + "'," +
                       " '" + dt["tax_name"].ToString() + "'," +
                        " '" + dt["tax1_gid"].ToString() + "'," +
                       " '" + values.slno + "'," +
                       " '" + dt["tax_percentage"].ToString() + "'," +
                       " '" + dt["taxsegment_gid"].ToString() + "'," +
                       " '" + dt["order_type"].ToString() + "', " +
                       " '" + values.salesorder_refno + "')";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);


                            //     msSQL = " update smr_trn_tsalesorderdtl  set " +
                            // "  salesorder_gid='" + values.salesorder_gid + "'," +
                            //  " product_gid='" + dt["product_gid"].ToString() + "'," +
                            // " product_name='" + dt["product_name"].ToString() + "'," +
                            // " product_code='" + dt["product_code"].ToString() + "'," +
                            // " product_price='" + dt["product_price"].ToString() + "'," +
                            // " qty_quoted ='" + dt["qty_quoted"].ToString() + "'," +
                            // " created_by='" + employee_gid + "'," +
                            // " discount_percentage='" + dt["discount_percentage"].ToString() + "' ," +
                            // " discount_amount='" + dt["discount_amount"].ToString() + "'," +
                            // " tax_amount='" + dt["tax_amount"].ToString() + "', " +
                            // " uom_gid='" + dt["uom_gid"].ToString() + "', " +
                            // " uom_name='" + dt["uom_name"].ToString() + "', " +
                            // " price='" + dt["price"].ToString() + "', " +
                            // " tax_name='" + dt["tax_name"].ToString() + "', " +
                            // " tax1_gid='" + dt["tax1_gid"].ToString() + "', " +
                            // " tax_percentage='" + dt["tax_percentage"].ToString() + "', " +
                            // " taxsegment_gid='" + dt["taxsegment_gid"].ToString() + "', " +
                            // " type='" + dt["order_type"].ToString() + "', " +
                            //  " where salesorder_gid='" + values.salesorder_gid + "'";
                            //     mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                            //     msSQL = " update acp_trn_torderdtl  set " +
                            //"  salesorder_gid='" + values.salesorder_gid + "'," +
                            // " product_gid='" + dt["product_gid"].ToString() + "'," +
                            //" product_name='" + dt["product_name"].ToString() + "'," +
                            //" product_code='" + dt["product_code"].ToString() + "'," +
                            //" product_price='" + dt["product_price"].ToString() + "'," +
                            //" qty_quoted ='" + dt["qty_quoted"].ToString() + "'," +
                            //" created_by='" + employee_gid + "'," +
                            //" discount_percentage='" + dt["discount_percentage"].ToString() + "' ," +
                            //" discount_amount='" + dt["discount_amount"].ToString() + "'," +
                            //" tax_amount='" + dt["tax_amount"].ToString() + "', " +
                            //" uom_gid='" + dt["uom_gid"].ToString() + "', " +
                            //" uom_name='" + dt["uom_name"].ToString() + "', " +
                            //" price='" + dt["price"].ToString() + "', " +
                            //" tax_name='" + dt["tax_name"].ToString() + "', " +
                            //" tax1_gid='" + dt["tax1_gid"].ToString() + "', " +
                            //" tax_percentage='" + dt["tax_percentage"].ToString() + "', " +
                            //" taxsegment_gid='" + dt["taxsegment_gid"].ToString() + "', " +
                            //" type='" + dt["order_type"].ToString() + "', " +
                            // " where salesorder_gid='" + values.salesorder_gid + "'";
                            //     mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);







                            if (mnResult == 0)
                            {
                                values.status = false;
                                values.message = "Error occurred while Insertion";
                                return;
                            }




                        }
                    }




                    msSQL = "select distinct type from smr_trn_tsalesorderdtl where salesorder_gid='" + mssalesorderGID + "' ";
                    objodbcDataReader = objdbconn.GetDataReader(msSQL);

                    if (objodbcDataReader.HasRows == true)
                    {
                        if (objodbcDataReader["type"].ToString() != "Services")
                        { 
                        lsorder_type = "Sales";

                        }
                        else
                        {
                            lsorder_type = "Services";
                        }
                    }

                  
                    objodbcDataReader.Close();





                    msSQL = " update smr_trn_tsalesorder set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                    msSQL = " update acp_trn_torder set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    string lsstage = "8";
                    msgetlead2campaign_gid = objcmnfunctions.GetMasterGID("BLCC");
                    string lsso = "N";
                    msSQL = " select employee_gid from hrm_mst_temployee where user_gid='" + values.user_name + "'";
                    string employee = objdbconn.GetExecuteScalar(msSQL);


                    msSQL = " Insert into crm_trn_tenquiry2campaign ( " +
                                                      " lead2campaign_gid, " +
                                                      " quotation_gid, " +
                                                      " so_status, " +
                                                      " created_by, " +
                                                      " customer_gid, " +
                                                      " leadstage_gid," +
                                                      " created_date, " +
                                                      " assign_to ) " +
                                                      " Values ( " +
                                                      "'" + msgetlead2campaign_gid + "'," +
                                                      "'" + msGetGid + "'," +
                                                      "'" + lsso + "'," +
                                                      "'" + employee_gid + "'," +
                                                      "'" + values.customer_gid + "'," +
                                                      "'" + lsstage + "'," +
                                                      "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                                                      "'" + employee + "')";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    msSQL = "select hierarchy_flag from adm_mst_tcompany where hierarchy_flag ='Y'";
                    objodbcDataReader = objdbconn.GetDataReader(msSQL);
                    if (objodbcDataReader.HasRows == true)
                    {

                        msGetGID = objcmnfunctions.GetMasterGID("PODC");
                        msSQL = " insert into smr_trn_tapproval ( " +
                        " approval_gid, " +
                        " approved_by, " +
                        " approved_date, " +
                        " submodule_gid, " +
                        " soapproval_gid " +
                        " ) values ( " +
                        "'" + msGetGID + "'," +
                        " '" + employee_gid + "'," +
                        "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                        "'SMRSROSOA'," +
                        "'" + mssalesorderGID + "') ";
                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                        msSQL = "select approval_flag from smr_trn_tapproval where submodule_gid='SMRSROSOA' and soapproval_gid='" + mssalesorderGID + "' ";
                        objodbcDataReader = objdbconn.GetDataReader(msSQL);
                        if (objodbcDataReader.HasRows == false)
                        {

                            msSQL = "update smr_trn_tsalesorder set salesorder_status='Approved',salesorder_remarks='" + values.so_remarks + "', " +
                                   " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                            msSQL = "update acp_trn_torder set salesorder_status='Approved',salesorder_remarks='" + values.so_remarks + "', " +
                                  " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                        }
                        else
                        {
                            msSQL = "select approved_by from smr_trn_tapproval where submodule_gid='SMRSROSOA' and soapproval_gid='" + mssalesorderGID + "'";
                            objodbcdatareader1 = objdbconn.GetDataReader(msSQL);
                            if (objodbcdatareader1.RecordsAffected == 1)
                            {

                                msSQL = " update smr_trn_tapproval set " +
                               " approval_flag = 'Y', " +
                               " approved_date = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                               " where approved_by = '" + employee_gid + "'" +
                               " and soapproval_gid = '" + mssalesorderGID + "' and submodule_gid='SMRSROSOA'";
                                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                                msSQL = "update smr_trn_tsalesorder set salesorder_status='Approved',salesorder_remarks='" + values.so_remarks + "', " +
                                   " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
                                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                                msSQL = "update acp_trn_torder set salesorder_status='Approved',salesorder_remarks='" + values.so_remarks + "', " +
                                      " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
                                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                            }
                            else
                            {
                                msSQL = " update smr_trn_tapproval set " +
                                           " approval_flag = 'Y', " +
                                           " approved_date = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                                           " where approved_by = '" + employee_gid + "'" +
                                           " and soapproval_gid = '" + mssalesorderGID + "' and submodule_gid='SMRSROSOA'";
                                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                            }
                        }



                        if (mnResult2 != 0 || mnResult2 == 0)
                        {

                            msSQL = " delete from smr_tmp_tsalesorderdtl " +
                                    " where employee_gid='" + employee_gid + "'";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                        }


                        if (mnResult2 != 0)
                        {
                            values.status = true;
                            values.message = "Sales Order  Updated Successfully";
                            return;
                        }
                        else
                        {
                            values.status = false;
                            values.message = "Error While Raising Sales Order";
                            return;
                        }

                    }
                    objodbcDataReader.Close();
                }
                else
                {
                    values.status = true;
                    values.message = "Select one Product to Raise Enquiry";
                    return;
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while Submitting Sales Order !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }


        }



        //public void DaUpdateSalesOrderfileupload(HttpRequest httpRequest, result objResult, string employee_gid)
        //{
        //    try
        //    {


        //        string customer_gid = Request.Form["customer_gid"];
        //        string branch_name = Request.Form["branch_name"];
        //        string branch_gid = Request.Form["branch_gid"];
        //        string salesorder_date = Request.Form["salesorder_date"];
        //        string renewal_mode = Request.Form["renewal_mode"];
        //        string renewal_date = Request.Form["renewal_date"];
        //        string frequency_term = Request.Form["frequency_term"];
        //        string customer_name = Request.Form["customer_name"];
        //        string so_remarks = Request.Form["so_remarks"];
        //        string so_referencenumber = Request.Form["so_referencenumber"];
        //        string address1 = Request.Form["address1"];
        //        string shipping_to = Request.Form["shipping_to"];
        //        string delivery_days = Request.Form["delivery_days"];
        //        string payment_days = Request.Form["payment_days"];
        //        string currency_code = Request.Form["currency_code"];
        //        string user_name = Request.Form["user_name"];
        //        string exchange_rate = Request.Form["exchange_rate"];
        //        string termsandconditions = Request.Form["termsandconditions"];
        //        string template_name = Request.Form["template_name"];
        //        string template_gid = Request.Form["template_gid"];
        //        string grandtotal = Request.Form["grandtotal"];
        //        string round_off = Request.Form["roundoff"];
        //        string insurance_charges = Request.Form["insurance_charges"];
        //        string packing_charges = Request.Form["packing_charges"];
        //        string buyback_charges = Request.Form["buyback_charges"];
        //        string freight_charges = Request.Form["freight_charges"];
        //        string additional_discount = Request.Form["additional_discount"];
        //        string addon_charge = Request.Form["addon_charge"];
        //        string tax_amount4 = Request.Form["tax_amount4"];
        //        string tax_name4 = Request.Form["tax_name4"];
        //        string tax_gid = Request.Form["tax_gid"];
        //        string totalamount = Request.Form["totalamount"];
        //        string total_price = Request.Form["total_price"];
        //        string taxsegment_gid = Request.Form["taxsegment_gid"];
        //        string salesorder_gid = Request.Form["salesorder_gid"];
        //        string slno = "1";

        //        HttpFileCollection httpFileCollection;
        //        string lsfilepath = string.Empty;
        //        string lsdocument_gid = string.Empty;
        //        MemoryStream ms_stream = new MemoryStream();
        //        string document_gid = string.Empty;
        //        string lscompany_code = string.Empty;
        //        HttpPostedFile httpPostedFile;
        //        string lspath;
        //        string final_path = "";
        //        string vessel_name = "";

        //        msSQL = " select company_code from adm_mst_tcompany";
        //        lscompany_code = objdbconn.GetExecuteScalar(msSQL);



        //        MemoryStream ms = new MemoryStream();
        //        lspath = ConfigurationManager.AppSettings["Doc_upload_file"] + "/erp_documents" + "/" + lscompany_code + "/" + "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month;



        //        {
        //            if ((!System.IO.Directory.Exists(lspath)))
        //                System.IO.Directory.CreateDirectory(lspath);
        //        }



        //        if (httpRequest.Files.Count > 0)
        //        {
        //            string lsfirstdocument_filepath = string.Empty;
        //            httpFileCollection = httpRequest.Files;
        //            for (int i = 0; i < httpFileCollection.Count; i++)
        //            {
        //                string msdocument_gid = objcmnfunctions.GetMasterGID("UPLF");
        //                httpPostedFile = httpFileCollection[i];
        //                FileExtensionname = httpPostedFile.FileName;
        //                string lsfile_gid = msdocument_gid;
        //                string lscompany_document_flag = string.Empty;
        //                FileExtension = Path.GetExtension(FileExtensionname).ToLower();
        //                lsfile_gid = lsfile_gid + FileExtension;
        //                Stream ls_readStream;
        //                ls_readStream = httpPostedFile.InputStream;
        //                ls_readStream.CopyTo(ms);
        //                lspath = ConfigurationManager.AppSettings["Doc_upload_file"] + "/erp_documents" + "/" + lscompany_code + "/" + "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/";
        //                string status;
        //                status = objcmnfunctions.uploadFile(lspath + msdocument_gid, FileExtension);
        //                ms.Close();
        //                lspath = "erp_documents" + "/" + lscompany_code + "/" + "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/";



        //                final_path = lspath + msdocument_gid + FileExtension;



        //            }
        //        }


        //        string totalvalue = user_name;



        //        msSQL = " select company_code from adm_mst_tcompany";
        //        lscompany_code = objdbconn.GetExecuteScalar(msSQL);



        //        string lscustomerbranch = "H.Q";
        //        string lscampaign_gid = "NO CAMPAIGN";

        //        msSQL = " select * from smr_tmp_tsalesorderdtl " +
        //            " where employee_gid='" + employee_gid + "'";
        //        DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

        //        if (dt_datatable.Rows.Count != 0)
        //        {

        //            string inputDate = salesorder_date;
        //            DateTime uiDate = DateTime.ParseExact(inputDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //            string salesorder_date1 = uiDate.ToString("yyyy-MM-dd");



        //            msSQL = " select customer_name from crm_mst_tcustomer where customer_gid='" + customer_gid + " '";
        //            string lscustomername = objdbconn.GetExecuteScalar(msSQL);
        //            msSQL = " select branch_gid from hrm_mst_Tbranch where branch_name='" + branch_name + " '";
        //            string lsbranchgid = objdbconn.GetExecuteScalar(msSQL);

        //            msSQL = " select currency_code from crm_trn_tcurrencyexchange where currencyexchange_gid='" + currency_code + " '";
        //            string currency_code1 = objdbconn.GetExecuteScalar(msSQL);

        //            msSQL = " select tax_prefix from acp_mst_Ttax where tax_prefix='" + tax_gid + " '";
        //            string lstax_name = objdbconn.GetExecuteScalar(msSQL);

        //            msSQL = " select user_name from adm_mst_Tuser  where user_gid='" + user_name + " '";
        //            string lssalesperson_name = objdbconn.GetExecuteScalar(msSQL);

        //            string lslocaladdon = "0.00";
        //            string lslocaladditionaldiscount = "0.00";
        //            string lslocalgrandtotal = " 0.00";
        //            string lsgst = "0.00";
        //            string lsamount4 = "0.00";
        //            //string lsproducttotalamount = "0.00";

        //            double totalAmount = double.TryParse(tax_amount4, out double totalpriceValue) ? totalpriceValue : 0;
        //            double addonCharges = double.TryParse(addon_charge, out double addonChargesValue) ? addonChargesValue : 0;
        //            double freightCharges = double.TryParse(freight_charges, out double freightChargesValue) ? freightChargesValue : 0;
        //            double packingCharges = double.TryParse(packing_charges, out double packingChargesValue) ? packingChargesValue : 0;
        //            double insuranceCharges = double.TryParse(insurance_charges, out double insuranceChargesValue) ? insuranceChargesValue : 0;
        //            double roundoff = double.TryParse(round_off, out double roundoffValue) ? roundoffValue : 0;
        //            double additionaldiscountAmount = double.TryParse(additional_discount, out double discountAmountValue) ? discountAmountValue : 0;
        //            double buybackCharges = double.TryParse(buyback_charges, out double buybackChargesValue) ? buybackChargesValue : 0;

        //            double grandTotal = (totalAmount + addonCharges + freightCharges + packingCharges + insuranceCharges + roundoff) - additionaldiscountAmount - buybackCharges;





        //            msSQL = " update smr_trn_tsalesorder  set " +
        //                     "  salesorder_gid='" + salesorder_gid + "'," +
        //                      " branch_gid='" + lsbranchgid + "'," +
        //                     " salesorder_date='" + salesorder_date1 + "'," +
        //                     " customer_gid='" + customer_gid + "'," +
        //                     " renewal_flag='" + renewal_mode + "'," +
        //                       " file_path='" + final_path + "'," +
        //                         " file_name='" + FileExtensionname + "'," +
        //                     " customer_name='" + lscustomername.Replace("'", "\\\'") + "'," +
        //                     " customer_address ='" + address1.Replace("'", "\\\'") + "'," +
        //                     " created_by='" + employee_gid + "'," +
        //                     " so_referenceno1='" + so_referencenumber + "' ,";
        //            if (so_remarks != null)
        //            {
        //                msSQL += " so_remarks='" + so_remarks.Replace("'", "\\\'") + "',";
        //            }
        //            else
        //            {
        //                msSQL += " so_remarks='" + so_remarks + "',";
        //            }

        //            msSQL += " payment_days='" + payment_days + "', " +
        //               " delivery_days='" + delivery_days + "', " +
        //               " Grandtotal='" + grandtotal.Replace(",", "").Trim() + "', ";
        //            if(termsandconditions != null)
        //            {
        //              msSQL +=  " termsandconditions='" + termsandconditions.Replace("'", "\\\'") + "', ";
        //            }
        //            else
        //            {
        //                msSQL += " termsandconditions='" + termsandconditions + "', ";
        //            }
                
        //             msSQL +=  " addon_charge='" + addon_charge.Trim() + "', " +
        //               " additional_discount='" + additional_discount + "', " +
        //               " addon_charge_l='" + addon_charge.Trim() + "', " +
        //               " additional_discount_l='" + additional_discount + "', " +
        //               " grandtotal_l='" + grandtotal.Replace(",", "").Trim() + "', " +
        //               " currency_code='" + currency_code1 + "', " +
        //               " currency_gid='" + currency_code + "', " +
        //               " exchange_rate='" + exchange_rate + "', " +
        //               " shipping_to='" + shipping_to.Replace("'", "\\\'") + "', " +
        //               " tax_gid='" + lstax_name + "'," +
        //               " tax_name ='" + tax_gid + "'," +
        //               " gst_amount='" + lsgst + "'," +
        //               " freight_charges='" + freight_charges + "'," +
        //               " total_price='" + totalamount.Replace(",", "").Trim() + "'," +
        //               " total_amount='" + grandtotal.Replace(",", "").Trim() + "',";
        //            if (tax_amount4 == null || tax_amount4 == "")
        //            {
        //                msSQL += " tax_amount='0',";

        //            }
        //            else
        //            {
        //                msSQL += " tax_amount='" + tax_amount4.Replace(",", "").Trim() + "',";
        //            }


        //            msSQL += " salesperson_gid='" + user_name + "'," +
        //                     " roundoff='" + roundoff + "', " +
        //                     " updated_date= '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
        //                      " where salesorder_gid='" + salesorder_gid + "'";


        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //            if (mnResult == 0)
        //            {
        //                objResult.status = false;
        //                objResult.message = " Some Error Occurred While Inserting Salesorder Details";
        //                return;
        //            }
        //            else
        //            {
        //                msSQL = " update acp_trn_Torder  set " +
        //                     "  salesorder_gid='" + salesorder_gid + "'," +
        //                     " branch_gid='" + lsbranchgid + "'," +
        //                     " salesorder_date='" + salesorder_date1 + "'," +
        //                     " customer_gid='" + customer_gid + "'," +
        //                     " customer_name='" + lscustomername.Replace("'", "\\\'") + "'," +
        //                     " customer_address ='" + address1.Replace("'", "\\\'") + "'," +
        //                     " created_by='" + employee_gid + "'," +
        //                     " so_referenceno1='" + so_referencenumber + "' ,";
        //                if (so_remarks != null)
        //                {
        //                    msSQL += " so_remarks='" + so_remarks.Replace("'", "\\\'") + "',";
        //                }
        //                else
        //                {
        //                    msSQL += " so_remarks='" + so_remarks + "',";
        //                }
        //                msSQL += " payment_days='" + payment_days + "', " +
        //                       " delivery_days='" + delivery_days + "', " +
        //                       " Grandtotal='" + grandtotal.Replace(",", "").Trim() + "', " +
        //                       " termsandconditions='" + termsandconditions.Replace("'", "\\\'") + "', " +
        //                       " addon_charge='" + addon_charge + "', " +
        //                       " additional_discount='" + additional_discount + "', " +
        //                       " addon_charge_l='" + addon_charge + "', " +
        //                       " additional_discount_l='" + additional_discount + "', " +
        //                       " grandtotal_l='" + grandtotal.Replace(",", "").Trim() + "', " +
        //                       " currency_code='" + currency_code1 + "', " +
        //                       " currency_gid='" + currency_code + "', " +
        //                       " exchange_rate='" + exchange_rate + "', " +
        //                       " shipping_to='" + shipping_to.Replace("'", "\\\'") + "', " +
        //                       " total_amount='" + grandtotal.Replace(",", "").Trim() + "'," +
        //                       " salesperson_gid='" + user_name + "'," +
        //                       " roundoff='" + roundoff + "', " +
        //                       " updated_date= '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
        //                        " where salesorder_gid='" + salesorder_gid + "'";


        //                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                mnResult2 = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                if (mnResult2 == 1)
        //                {
        //                    objResult.status = true;
        //                }

        //            }
        //            if (renewal_mode == "Y")
        //            {
        //                string inputDate1 = renewal_date;
        //                DateTime uiDate1 = DateTime.ParseExact(inputDate1, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //                string renewal_date1 = uiDate1.ToString("yyyy-MM-dd");

        //                msSQL = " update crm_trn_trenewal  set " +
        //                     " renewal_flag='" + renewal_mode + "'," +
        //                     " renewal_date ='" + renewal_date1 + "'," +
        //                     " frequency_term ='" + frequency_term + "'," +
        //                     " customer_gid='" + customer_gid + "'," +
        //                     " created_by='" + employee_gid + "'," +
        //                     " updated_date= '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
        //                     " where salesorder_gid='" + salesorder_gid + "'";
        //                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //            }




        //            msSQL = " select " +
        //                    " tmpsalesorderdtl_gid," +
        //                    " salesorder_gid," +
        //                    " product_gid," +
        //                    " product_name," +
        //                     " product_code," +
        //                      " productgroup_gid," +
        //                    " product_price," +
        //                    " qty_quoted," +
        //                    " discount_percentage," +
        //                    " discount_amount," +
        //                    " uom_gid," +
        //                    " uom_name," +
        //                    " price," +
        //                    " tax_name," +
        //                    " tax_name2," +
        //                    " tax1_gid, " +
        //                     " tax2_gid, " +
        //                    " tax_amount," +
        //                    " tax_amount2," +
        //                    " slno," +
        //                    " tax_percentage," +
        //                    " tax_percentage2," +
        //                    " order_type, " +
        //                      " product_remarks, " +
        //                        " display_field, " +
        //                    " taxsegment_gid, " +
        //                    " taxsegmenttax_gid " +
        //                    " from smr_tmp_tsalesorderdtl where employee_gid = '" + employee_gid + "' ";

        //            DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
        //            var getModuleList = new List<postsales_list>();
        //            if (dt_datatable.Rows.Count != 0)
        //            {
        //                msSQL = "delete from smr_trn_tsalesorderdtl where salesorder_gid = '" + salesorder_gid + "'";
        //                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                msSQL = "delete from acp_trn_torderdtl where salesorder_gid = '" + salesorder_gid + "'";
        //                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                int i = 0;
        //                foreach (DataRow dt in  dt_datatable.Rows)
        //                {
        //                    mssalesorderGID1 = objcmnfunctions.GetMasterGID("VSDC");

        //                    string display_field = dt["product_remarks"].ToString();

        //                    msSQL = " insert into smr_trn_tsalesorderdtl (" +
        //                         " salesorderdtl_gid ," +
        //                         " salesorder_gid," +
        //                         " product_gid ," +
        //                         " product_name," +
        //                         " product_code," +
        //                         " productgroup_gid," +
        //                         " product_price," +
        //                         " qty_quoted," +
        //                         " discount_percentage," +
        //                         " discount_amount," +
        //                         " tax_amount ," +
        //                         " tax_amount2 ," +
        //                         " uom_gid," +
        //                         " uom_name," +
        //                         " price," +
        //                         " tax_name," +
        //                         " tax_name2," +
        //                         " tax1_gid," +
        //                         " tax2_gid," +
        //                         " slno," +
        //                          " product_remarks," +
        //                           " display_field," +
        //                         " tax_percentage," +
        //                         " tax_percentage2," +
        //                         " taxsegment_gid," +
        //                         " taxsegmenttax_gid," +
        //                         " type " +
        //                         ")values(" +
        //                         " '" + mssalesorderGID1 + "'," +
        //                         " '" + salesorder_gid + "'," +
        //                         " '" + dt["product_gid"].ToString() + "'," +
        //                         " '" + dt["product_name"].ToString() + "'," +
        //                         " '" + dt["product_code"].ToString() + "'," +
        //                          " '" + dt["productgroup_gid"].ToString() + "'," +
        //                         " '" + dt["product_price"].ToString() + "'," +
        //                         " '" + dt["qty_quoted"].ToString() + "'," +
        //                         " '" + dt["discount_percentage"].ToString() + "'," +
        //                         " '" + dt["discount_amount"].ToString() + "'," +
        //                         " '" + dt["tax_amount"].ToString() + "'," +
        //                         " '" + dt["tax_amount2"].ToString() + "'," +
        //                         " '" + dt["uom_gid"].ToString() + "'," +
        //                         " '" + dt["uom_name"].ToString() + "'," +
        //                         " '" + dt["price"].ToString() + "'," +
        //                         " '" + dt["tax_name"].ToString() + "'," +
        //                         " '" + dt["tax_name2"].ToString() + "'," +
        //                         " '" + dt["tax1_gid"].ToString() + "'," +
        //                         " '" + dt["tax2_gid"].ToString() + "'," +
        //                         " '" + i + 1 + "',";
        //                    if (display_field != null)
        //                    {
        //                        msSQL += " '" + display_field.Replace("'", "\\\'") + "'," +
        //                               " '" + display_field.Replace("'", "\\\'") + "',";
        //                    }
        //                    else
        //                    {
        //                        msSQL += " '" + display_field + "'," +
        //                              " '" + display_field + "',";
        //                    }

        //                    msSQL += " '" + dt["tax_percentage"].ToString() + "'," +
        //                       " '" + dt["tax_percentage2"].ToString() + "'," +
        //                       " '" + dt["taxsegment_gid"].ToString() + "'," +
        //                       " '" + dt["taxsegmenttax_gid"].ToString() + "'," +
        //                       " '" + dt["order_type"].ToString() + "')";
        //                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                    msSQL = " insert into acp_trn_torderdtl (" +
        //               " salesorderdtl_gid ," +
        //               " salesorder_gid," +
        //               " product_gid ," +
        //               " product_name," +
        //               " product_price," +
        //               " qty_quoted," +
        //               " discount_percentage," +
        //               " discount_amount," +
        //               " tax_amount ," +
        //               " uom_gid," +
        //               " uom_name," +
        //               " price," +
        //               " tax_name," +
        //               " tax1_gid," +
        //               " slno," +
        //               " tax_percentage," +
        //               " taxsegment_gid," +
        //               " type, " +
        //               " salesorder_refno" +
        //               ")values(" +
        //               " '" + mssalesorderGID1 + "'," +
        //               " '" + salesorder_gid + "'," +
        //               " '" + dt["product_gid"].ToString() + "'," +
        //               " '" + dt["product_name"].ToString() + "'," +
        //               " '" + dt["product_price"].ToString() + "'," +
        //               " '" + dt["qty_quoted"].ToString() + "'," +
        //               " '" + dt["discount_percentage"].ToString() + "'," +
        //               " '" + dt["discount_amount"].ToString() + "'," +
        //               " '" + dt["tax_amount"].ToString() + "'," +
        //               " '" + dt["uom_gid"].ToString() + "'," +
        //               " '" + dt["uom_name"].ToString() + "'," +
        //               " '" + dt["price"].ToString() + "'," +
        //               " '" + dt["tax_name"].ToString() + "'," +
        //                " '" + dt["tax1_gid"].ToString() + "'," +
        //               " '" + slno + "'," +
        //               " '" + dt["tax_percentage"].ToString() + "'," +
        //               " '" + dt["taxsegment_gid"].ToString() + "'," +
        //               " '" + dt["order_type"].ToString() + "', " +
        //               " '" + so_referencenumber + "')";
        //                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);


        //                    //     msSQL = " update smr_trn_tsalesorderdtl  set " +
        //                    // "  salesorder_gid='" + values.salesorder_gid + "'," +
        //                    //  " product_gid='" + dt["product_gid"].ToString() + "'," +
        //                    // " product_name='" + dt["product_name"].ToString() + "'," +
        //                    // " product_code='" + dt["product_code"].ToString() + "'," +
        //                    // " product_price='" + dt["product_price"].ToString() + "'," +
        //                    // " qty_quoted ='" + dt["qty_quoted"].ToString() + "'," +
        //                    // " created_by='" + employee_gid + "'," +
        //                    // " discount_percentage='" + dt["discount_percentage"].ToString() + "' ," +
        //                    // " discount_amount='" + dt["discount_amount"].ToString() + "'," +
        //                    // " tax_amount='" + dt["tax_amount"].ToString() + "', " +
        //                    // " uom_gid='" + dt["uom_gid"].ToString() + "', " +
        //                    // " uom_name='" + dt["uom_name"].ToString() + "', " +
        //                    // " price='" + dt["price"].ToString() + "', " +
        //                    // " tax_name='" + dt["tax_name"].ToString() + "', " +
        //                    // " tax1_gid='" + dt["tax1_gid"].ToString() + "', " +
        //                    // " tax_percentage='" + dt["tax_percentage"].ToString() + "', " +
        //                    // " taxsegment_gid='" + dt["taxsegment_gid"].ToString() + "', " +
        //                    // " type='" + dt["order_type"].ToString() + "', " +
        //                    //  " where salesorder_gid='" + values.salesorder_gid + "'";
        //                    //     mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                    //     msSQL = " update acp_trn_torderdtl  set " +
        //                    //"  salesorder_gid='" + values.salesorder_gid + "'," +
        //                    // " product_gid='" + dt["product_gid"].ToString() + "'," +
        //                    //" product_name='" + dt["product_name"].ToString() + "'," +
        //                    //" product_code='" + dt["product_code"].ToString() + "'," +
        //                    //" product_price='" + dt["product_price"].ToString() + "'," +
        //                    //" qty_quoted ='" + dt["qty_quoted"].ToString() + "'," +
        //                    //" created_by='" + employee_gid + "'," +
        //                    //" discount_percentage='" + dt["discount_percentage"].ToString() + "' ," +
        //                    //" discount_amount='" + dt["discount_amount"].ToString() + "'," +
        //                    //" tax_amount='" + dt["tax_amount"].ToString() + "', " +
        //                    //" uom_gid='" + dt["uom_gid"].ToString() + "', " +
        //                    //" uom_name='" + dt["uom_name"].ToString() + "', " +
        //                    //" price='" + dt["price"].ToString() + "', " +
        //                    //" tax_name='" + dt["tax_name"].ToString() + "', " +
        //                    //" tax1_gid='" + dt["tax1_gid"].ToString() + "', " +
        //                    //" tax_percentage='" + dt["tax_percentage"].ToString() + "', " +
        //                    //" taxsegment_gid='" + dt["taxsegment_gid"].ToString() + "', " +
        //                    //" type='" + dt["order_type"].ToString() + "', " +
        //                    // " where salesorder_gid='" + values.salesorder_gid + "'";
        //                    //     mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);







        //                    if (mnResult == 0)
        //                    {
        //                        objResult.status = false;
        //                        objResult.message = "Error occurred while Insertion";
        //                        return;
        //                    }




        //                }
        //            }




        //            msSQL = "select distinct type from smr_trn_tsalesorderdtl where salesorder_gid='" + mssalesorderGID + "' ";
        //            objodbcDataReader = objdbconn.GetDataReader(msSQL);

        //            if (objodbcDataReader.HasRows == true)
        //            {
        //                if (objodbcDataReader["type"].ToString() != "Services")
        //                { 
        //                lsorder_type = "Sales";
        //                }
        //                else
        //                {
        //                    lsorder_type = "Services";
        //                }

        //            }

                    
        //            objodbcDataReader.Close();





        //            msSQL = " update smr_trn_tsalesorder set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //            msSQL = " update acp_trn_torder set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //            string lsstage = "8";
        //            msgetlead2campaign_gid = objcmnfunctions.GetMasterGID("BLCC");
        //            string lsso = "N";
        //            msSQL = " select employee_gid from hrm_mst_temployee where user_gid='" + user_name + "'";
        //            string employee = objdbconn.GetExecuteScalar(msSQL);


        //            msSQL = " Insert into crm_trn_tenquiry2campaign ( " +
        //                                              " lead2campaign_gid, " +
        //                                              " quotation_gid, " +
        //                                              " so_status, " +
        //                                              " created_by, " +
        //                                              " customer_gid, " +
        //                                              " leadstage_gid," +
        //                                              " created_date, " +
        //                                              " assign_to ) " +
        //                                              " Values ( " +
        //                                              "'" + msgetlead2campaign_gid + "'," +
        //                                              "'" + msGetGid + "'," +
        //                                              "'" + lsso + "'," +
        //                                              "'" + employee_gid + "'," +
        //                                              "'" + customer_gid + "'," +
        //                                              "'" + lsstage + "'," +
        //                                              "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
        //                                              "'" + employee + "')";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //            msSQL = "select hierarchy_flag from adm_mst_tcompany where hierarchy_flag ='Y'";
        //            objodbcDataReader = objdbconn.GetDataReader(msSQL);
        //            if (objodbcDataReader.HasRows == true)
        //            {

        //                msGetGID = objcmnfunctions.GetMasterGID("PODC");
        //                msSQL = " insert into smr_trn_tapproval ( " +
        //                " approval_gid, " +
        //                " approved_by, " +
        //                " approved_date, " +
        //                " submodule_gid, " +
        //                " soapproval_gid " +
        //                " ) values ( " +
        //                "'" + msGetGID + "'," +
        //                " '" + employee_gid + "'," +
        //                "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
        //                "'SMRSROSOA'," +
        //                "'" + mssalesorderGID + "') ";
        //                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                msSQL = "select approval_flag from smr_trn_tapproval where submodule_gid='SMRSROSOA' and soapproval_gid='" + mssalesorderGID + "' ";
        //                objodbcDataReader = objdbconn.GetDataReader(msSQL);
        //                if (objodbcDataReader.HasRows == false)
        //                {

        //                    msSQL = "update smr_trn_tsalesorder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
        //                           " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                    msSQL = "update acp_trn_torder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
        //                          " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                }
        //                else
        //                {
        //                    msSQL = "select approved_by from smr_trn_tapproval where submodule_gid='SMRSROSOA' and soapproval_gid='" + mssalesorderGID + "'";
        //                    objodbcdatareader1 = objdbconn.GetDataReader(msSQL);
        //                    if (objodbcdatareader1.RecordsAffected == 1)
        //                    {

        //                        msSQL = " update smr_trn_tapproval set " +
        //                       " approval_flag = 'Y', " +
        //                       " approved_date = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
        //                       " where approved_by = '" + employee_gid + "'" +
        //                       " and soapproval_gid = '" + mssalesorderGID + "' and submodule_gid='SMRSROSOA'";
        //                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                        msSQL = "update smr_trn_tsalesorder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
        //                           " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                        msSQL = "update acp_trn_torder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
        //                              " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                    }
        //                    else
        //                    {
        //                        msSQL = " update smr_trn_tapproval set " +
        //                                   " approval_flag = 'Y', " +
        //                                   " approved_date = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
        //                                   " where approved_by = '" + employee_gid + "'" +
        //                                   " and soapproval_gid = '" + mssalesorderGID + "' and submodule_gid='SMRSROSOA'";
        //                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                    }
        //                }



        //                if (mnResult2 != 0 || mnResult2 == 0)
        //                {

        //                    msSQL = " delete from smr_tmp_tsalesorderdtl " +
        //                            " where employee_gid='" + employee_gid + "'";
        //                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                }


        //                if (mnResult2 != 0)
        //                {
        //                    objResult.status = true;
        //                    objResult.message = "Sales Order  Updated Successfully";
        //                    return;
        //                }
        //                else
        //                {
        //                    objResult.status = false;
        //                    objResult.message = "Error While updating Sales Order";
        //                    return;
        //                }

        //            }
        //            objodbcDataReader.Close();
        //        }
        //        else
        //        {
        //            objResult.status = true;
        //            objResult.message = "Select one Product to Raise Enquiry";
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objResult.message = "Exception occured while Submitting Sales Order !";
        //        objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
        //        $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
        //        objResult.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
        //    }




        //}

        public void DaPostSalesOrder( postsales_list values)
        {
            try
            {

                string totalvalue = values.user_name;



                msSQL = " select company_code from adm_mst_tcompany";
                lscompany_code = objdbconn.GetExecuteScalar(msSQL);

                msSQL = "select tax_prefix from acp_mst_ttax where tax_gid='" + values.tax_name4 + "'";
                string lstaxname1 = objdbconn.GetExecuteScalar(msSQL);


                string lscustomerbranch = "H.Q";
                string lscampaign_gid = "NO CAMPAIGN";

                msSQL = " select * from smr_tmp_tsalesorderdtl " +
                    " where employee_gid='" + employee_gid + "'";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                if (dt_datatable.Rows.Count != 0)
                {

                    string inputDate = values.salesorder_date;
                    DateTime uiDate = DateTime.ParseExact(inputDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string salesorder_date = uiDate.ToString("yyyy-MM-dd");
                    if (values.customer_gid == null)
                    {
                        customergid = values.customergid;
                    }
                    else
                    {
                        customergid = values.customer_gid;
                    }
                    msSQL = " select customer_name from crm_mst_tcustomer where customer_gid='" + customergid + " '";
                    string lscustomername = objdbconn.GetExecuteScalar(msSQL);

                    msSQL = " select currency_code from crm_trn_tcurrencyexchange where currencyexchange_gid='" + values.currency_code + " '";
                    string currency_code = objdbconn.GetExecuteScalar(msSQL);

                    string lslocaladdon = "0.00";
                    string lslocaladditionaldiscount = "0.00";
                    string lslocalgrandtotal = " 0.00";
                    string lsgst = "0.00";
                    string lsamount4 = "0.00";
                    //string lsproducttotalamount = "0.00";

                    double totalAmount = double.TryParse(values.tax_amount4, out double totalpriceValue) ? totalpriceValue : 0;
                    double addonCharges = double.TryParse(values.addon_charge, out double addonChargesValue) ? addonChargesValue : 0;
                    double freightCharges = double.TryParse(values.freight_charges, out double freightChargesValue) ? freightChargesValue : 0;
                    double packingCharges = double.TryParse(values.packing_charges, out double packingChargesValue) ? packingChargesValue : 0;
                    double insuranceCharges = double.TryParse(values.insurance_charges, out double insuranceChargesValue) ? insuranceChargesValue : 0;
                    double roundoff = double.TryParse(values.roundoff, out double roundoffValue) ? roundoffValue : 0;
                    double additionaldiscountAmount = double.TryParse(values.additional_discount, out double discountAmountValue) ? discountAmountValue : 0;
                    double buybackCharges = double.TryParse(values.buyback_charges, out double buybackChargesValue) ? buybackChargesValue : 0;

                    double grandTotal = (totalAmount + addonCharges + freightCharges + packingCharges + insuranceCharges + roundoff) - additionaldiscountAmount - buybackCharges;

                    string lsinvoice_refno = "", lsorder_refno = "";
                    mssalesorderGID = objcmnfunctions.GetMasterGID("VSOP");

                    lsrefno = objcmnfunctions.GetMasterGID("SO");
                    msSQL = "select company_code from adm_mst_Tcompany";
                    lscompany_code = objdbconn.GetExecuteScalar(msSQL);
                    if (lscompany_code == "BOBA")
                    {
                        string ls_referenceno = objcmnfunctions.getSequencecustomizerGID("INV", "RBL", values.branch_name);
                        msSQL = "SELECT SEQUENCE_CURVAL FROM adm_mst_tsequencecodecustomizer  WHERE sequence_code='INV' AND branch_gid='" + values.branch_name + "'";
                        string lscode = objdbconn.GetExecuteScalar(msSQL);


                        lsinvoice_refno = "SI" + " - " + lscode;
                        lsorder_refno = "SO" + " - " + lscode;

                    }
                    else
                    {
                        lsinvoice_refno = mssalesorderGID;
                        lsorder_refno = lsrefno;

                    }




                    msSQL = " insert  into smr_trn_tsalesorder (" +
                             " salesorder_gid ," +
                             " branch_gid ," +
                             " salesorder_date," +
                             " customer_gid," +
                             " customer_name," +
                             " customer_address ," +
                             " bill_to ," +
                             " created_by," +
                             " so_referenceno1 ," +
                             " so_referencenumber ," +
                             " so_remarks," +
                             " payment_days, " +
                             " delivery_days, " +
                             " Grandtotal, " +
                             " termsandconditions, " +
                             " salesorder_status, " +
                             " addon_charge, " +
                             " additional_discount, " +
                             " addon_charge_l, " +
                             " additional_discount_l, " +
                             " grandtotal_l, " +
                             " currency_code, " +
                             " currency_gid, " +
                             " exchange_rate, " +
                             " shipping_to, " +
                             " tax_gid," +
                             " tax_name, " +
                             " gst_amount," +
                             " total_price," +
                             " total_amount," +
                             " tax_amount," +
                             " vessel_name," +
                             " salesperson_gid," +
                             " roundoff, " +
                             " updated_addon_charge, " +
                             " updated_additional_discount, " +
                             " freight_charges," +
                             " buyback_charges," +
                             " packing_charges," +
                             " insurance_charges, " +
                             " source_flag, " +
                             " renewal_flag ," +
                             "created_date" +
                             " )values(" +
                             " '" + mssalesorderGID + "'," +
                             " '" + values.branch_name + "'," +
                             " '" + salesorder_date + "',";
                    if (values.customer_gid == null)
                    {
                        msSQL += " '" + values.customergid + "',";
                    }
                    else
                    {
                        msSQL += " '" + values.customer_gid + "',";
                    }

                    msSQL += " '" + lscustomername.Replace("'", "\\\'") + "'," +
                     " '" + values.address1.Replace("'", "\\\'") + "'," +
                     " '" + values.address1.Replace("'", "\\\'") + "'," +
                     " '" + employee_gid + "'," +
                     "' " + lsorder_refno + "'," +
                     " '" + lsinvoice_refno + "',";
                    if (values.so_remarks != null)
                    {
                        msSQL += " '" + values.so_remarks.Replace("'", "\\\'") + "',";
                    }
                    else
                    {
                        msSQL += " '" + values.so_remarks + "',";
                    }

                    msSQL += " '" + values.payment_days + "'," +
                       " '" + values.delivery_days + "'," +
                       " '" + values.grandtotal.Replace(",", "").Trim() + "',";
                    if(values.termsandconditions != null)
                    {
                        msSQL += " '" + values.termsandconditions.Replace("'", "\\\'") + "',";
                    }
                    else
                    {
                        msSQL += " '" + values.termsandconditions + "',";
                    }
                      
                      msSQL += " 'Approved',";
                    if (values.addon_charge != "" || values.addon_charge != null)
                    {
                        msSQL += "'" + values.addon_charge + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladdon + "',";
                    }
                    if (values.additional_discount != "" || values.additional_discount != null)
                    {
                        msSQL += "'" + values.additional_discount + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladditionaldiscount + "',";
                    }
                    if (values.addon_charge != "" || values.addon_charge != null)
                    {
                        msSQL += "'" + values.addon_charge + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladditionaldiscount + "',";
                    }
                    if (values.additional_discount != "" || values.additional_discount != null)
                    {
                        msSQL += "'" + values.additional_discount + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladditionaldiscount + "',";
                    }
                    msSQL += " '" + lslocalgrandtotal + "'," +
                         " '" + currency_code + "'," +
                         " '" + values.currency_code + "'," +
                         " '" + values.exchange_rate + "'," +
                         " '" + values.shipping_address.Replace("'", "\\\'") + "'," +
                         " '" + values.tax_name4 + "'," +
                         " '" + lstaxname1 + "', " +
                        "'" + lsgst + "',";
                    msSQL += " '" + values.totalamount.Replace(",", "").Trim() + "',";
                    if (values.grandtotal == null && values.grandtotal == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += " '" + values.grandtotal.Replace(",", "").Trim() + "',";
                    }

                    if (values.tax_amount4 != "" && values.tax_amount4 != null)
                    {
                        msSQL += "'" + values.tax_amount4 + "',";
                    }
                    else
                    {
                        msSQL += "'" + lsamount4 + "',";
                    }
                    msSQL += " '" + values.vessel_name + "'," +
                            " '" + values.user_name + "',";
                    if (values.roundoff == "" || values.roundoff == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.roundoff + "',";
                    }
                    if (values.addon_charge == "" || values.addon_charge == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.addon_charge + "',";
                    }
                    if (values.additional_discount == "" || values.additional_discount == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.additional_discount + "',";
                    }
                    if (values.freight_charges == "" || values.freight_charges == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.freight_charges + "',";
                    }
                    if (values.buyback_charges == "" || values.buyback_charges == null)
                    {

                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.buyback_charges + "',";
                    }
                    if (values.packing_charges == "" || values.packing_charges == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.packing_charges + "',";
                    }
                    if (values.insurance_charges == "" || values.insurance_charges == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.insurance_charges + "',";
                    }
                    msSQL += "'I',";
                    msSQL += "'" + values.renewal_mode + "',";

                    msSQL += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                    if (mnResult == 0)
                    {
                        values.status = false;
                        values.message = " Some Error Occurred While Inserting Salesorder Details";
                        return;
                    }
                    else
                    {
                        msSQL = " insert  into acp_trn_torder (" +
                               " salesorder_gid ," +
                               " branch_gid ," +
                               " salesorder_date," +
                               " customer_gid," +
                               " customer_name," +
                               " customer_address," +
                               " created_by," +
                               " so_remarks," +
                               " so_referencenumber," +
                               " payment_days, " +
                               " delivery_days, " +
                               " Grandtotal, " +
                               " termsandconditions, " +
                               " salesorder_status, " +
                               " addon_charge, " +
                               " additional_discount, " +
                               " addon_charge_l, " +
                               " additional_discount_l, " +
                               " grandtotal_l, " +
                               " currency_code, " +
                               " currency_gid, " +
                               " exchange_rate, " +
                               " updated_addon_charge, " +
                               " updated_additional_discount, " +
                               " shipping_to, " +
                               " campaign_gid, " +
                               " roundoff," +
                               " salesperson_gid, " +
                               " freight_charges," +
                               " buyback_charges," +
                               " packing_charges," +
                               " insurance_charges " +
                               ") values(" +
                               " '" + mssalesorderGID + "'," +
                               " '" + values.branch_name + "'," +
                               " '" + salesorder_date + "',";
                        if (values.customer_gid == null)
                        {
                            msSQL += " '" + values.customergid + "',";
                        }
                        else
                        {
                            msSQL += " '" + values.customer_gid + "',";
                        }
                        msSQL += " '" + lscustomername.Replace("'", "\\\'") + "'," +
                               " '" + values.address1.Replace("'", "\\\'") + "'," +
                               " '" + employee_gid + "',";
                        if (values.so_remarks != null)
                        {
                            msSQL += " '" + values.so_remarks.Replace("'", "\\\'") + "',";
                        }
                        else
                        {
                            msSQL += " '" + values.so_remarks + "',";
                        }
                        msSQL += " '" + lsrefno + "'," +
                                " '" + values.payment_days + "'," +
                                " '" + values.delivery_days + "'," +
                                " '" + values.grandtotal + "'," +
                                " '" + values.termsandconditions.Replace("'", "\\\'") + "'," +
                                " 'Approved',";
                        if (values.addon_charge == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.addon_charge + "',";
                        }
                        if (values.additional_discount == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.additional_discount + "',";
                        }
                        msSQL += "'" + lslocaladdon + "'," +
                               " '" + lslocaladditionaldiscount + "'," +
                               " '" + lslocalgrandtotal + "'," +
                               " '" + currency_code + "'," +
                               " '" + values.currency_code + "'," +
                               " '" + values.exchange_rate + "',";
                        if (values.addon_charge == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.addon_charge + "',";
                        }
                        if (values.additional_discount == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.additional_discount + "',";
                        }
                        msSQL += "'" + values.shipping_address.Replace("'", "\\\'") + "'," +
                               " '" + lscampaign_gid + "',";
                        if (values.roundoff == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.roundoff + "',";
                        }
                        msSQL += " '" + values.user_name + "',";
                        if (values.freight_charges == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.freight_charges + "',";
                        }
                        if (values.buyback_charges == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.buyback_charges + "',";
                        }
                        if (values.packing_charges == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.packing_charges + "',";
                        }
                        if (values.insurance_charges == "")
                        {
                            msSQL += "'0.00')";
                        }
                        else
                        {
                            msSQL += "'" + values.insurance_charges + "')";
                        }
                        mnResult2 = objdbconn.ExecuteNonQuerySQL(msSQL);
                        if (mnResult2 == 1)
                        {
                            values.status = true;
                        }

                    }
                    if (values.renewal_mode == "Y")
                    {
                        msINGetGID = objcmnfunctions.GetMasterGID("BRLP");

                        msSQL = " Insert into crm_trn_trenewal ( " +
                                " renewal_gid, " +
                                " renewal_flag, " +
                                " frequency_term, " +
                                " customer_gid," +
                                " renewal_date, " +
                                " salesorder_gid, " +
                                " created_by, " +
                                " renewal_type, " +
                                " created_date) " +
                               " Values ( " +
                                 "'" + msINGetGID + "'," +
                                 "'" + values.renewal_mode + "'," +
                                 "'" + values.frequency_terms + "',";
                        if (values.customer_gid == null)
                        {
                            msSQL += " '" + values.customergid + "',";
                        }
                        else
                        {
                            msSQL += " '" + values.customer_gid + "',";
                        }
                        msSQL += "'" + values.renewal_date + "'," +
                                  "'" + mssalesorderGID + "'," +
                                  "'" + employee_gid + "'," +
                                  "'sales'," +
                                "'" + DateTime.Now.ToString("yyyy-MM-dd") + "')";
                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                    }
                    msSQL = " select " +
                            " tmpsalesorderdtl_gid," +
                            " salesorder_gid," +
                            " product_gid," +
                            " productgroup_gid," +
                            " product_remarks," +
                            " product_name," +
                             " product_code," +
                            " product_price," +
                            " qty_quoted," +
                            " discount_percentage," +
                            " discount_amount," +
                            " uom_gid," +
                            " uom_name," +
                            " price," +
                            " tax_name," +
                            " tax1_gid, " +
                            " tax_amount," +
                             " tax_name2," +
                            " tax2_gid, " +
                            " tax_amount2," +
                             " tax_percentage2," +
                            " slno," +
                            " tax_percentage," +
                            " order_type, " +
                            " taxsegment_gid, " +
                            " taxsegmenttax_gid " +
                            " from smr_tmp_tsalesorderdtl" +
                            " where employee_gid='" + employee_gid + "' ";
                    DataTable dt_datatable7 = objdbconn.GetDataTable(msSQL);
                    var getModuleList = new List<postsales_list>();
                    if (dt_datatable7.Rows.Count != 0)
                    {
                        foreach (DataRow dt in  dt_datatable7.Rows)
                        {
                            getModuleList.Add(new postsales_list
                            {
                                salesorder_gid = dt["salesorder_gid"].ToString(),
                                tmpsalesorderdtl_gid = dt["tmpsalesorderdtl_gid"].ToString(),
                                product_gid = dt["product_gid"].ToString(),
                                product_name = dt["product_name"].ToString(),
                                product_code = dt["product_code"].ToString(),
                                productuom_name = dt["uom_name"].ToString(),
                                productgroup_gid = dt["productgroup_gid"].ToString(),
                                product_remarks = dt["product_remarks"].ToString(),
                                unitprice = dt["product_price"].ToString(),
                                quantity = dt["qty_quoted"].ToString(),
                                discountpercentage = dt["discount_percentage"].ToString(),
                                discountamount = dt["discount_amount"].ToString(),
                                tax_name = dt["tax_name"].ToString(),
                                tax_amount = dt["tax_amount"].ToString(),
                                totalamount = dt["price"].ToString(),
                                order_type = dt["order_type"].ToString(),
                                slno = dt["slno"].ToString(),
                                taxsegment_gid = dt["taxsegment_gid"].ToString(),
                                taxsegmenttax_gid = dt["taxsegmenttax_gid"].ToString(),
                            });

                            int i = 0;

                            //mssalesorderGID1 = objcmnfunctions.GetMasterGID("VSDC");
                            //if (mssalesorderGID1 == "E")
                            //{
                            //    values.message = "Create Sequence code for VSDC ";
                            //    return;
                            //}

                            string display_field = dt["product_remarks"].ToString();

                            msSQL = " insert into smr_trn_tsalesorderdtl (" +
                                 " salesorderdtl_gid ," +
                                 " salesorder_gid," +
                                 " product_gid ," +
                                 " product_name," +
                                 " product_code," +
                                 " product_price," +
                                 " productgroup_gid," +
                                 " product_remarks," +
                                 " display_field," +
                                 " qty_quoted," +
                                 " discount_percentage," +
                                 " discount_amount," +
                                 " tax_amount ," +
                                 " uom_gid," +
                                 " uom_name," +
                                 " price," +
                                 " tax_name," +
                                 " tax1_gid," +
                                  " tax_name2," +
                                 " tax2_gid," +
                                  " tax_percentage2," +
                                   " tax_amount2," +
                                 " slno," +
                                 " tax_percentage," +
                                 " taxsegment_gid," +
                                 " taxsegmenttax_gid," +
                                 " type " +
                                 ")values(" +
                                 " '" + dt["tmpsalesorderdtl_gid"].ToString() + "'," +
                                 " '" + mssalesorderGID + "'," +
                                 " '" + dt["product_gid"].ToString() + "'," +
                                 " '" + dt["product_name"].ToString() + "'," +
                                 " '" + dt["product_code"].ToString() + "'," +
                                 " '" + dt["product_price"].ToString() + "'," +
                                 " '" + dt["productgroup_gid"].ToString() + "',";
                            if (display_field != null)
                            {
                                msSQL += " '" + display_field.Replace("'", "\\\'") + "'," +
                                  " '" + display_field.Replace("'", "\\\'") + "',";
                            }
                            else
                            {
                                msSQL += " '" + display_field + "'," +
                                  " '" + display_field + "',";
                            }

                            msSQL += " '" + dt["qty_quoted"].ToString() + "'," +
                              " '" + dt["discount_percentage"].ToString() + "'," +
                              " '" + dt["discount_amount"].ToString() + "'," +
                              " '" + dt["tax_amount"].ToString() + "'," +
                              " '" + dt["uom_gid"].ToString() + "'," +
                              " '" + dt["uom_name"].ToString() + "'," +
                              " '" + dt["price"].ToString() + "'," +
                              " '" + dt["tax_name"].ToString() + "'," +
                              " '" + dt["tax1_gid"].ToString() + "'," +
                                " '" + dt["tax_name2"].ToString() + "'," +
                                  " '" + dt["tax2_gid"].ToString() + "'," +
                                    " '" + dt["tax_percentage2"].ToString() + "'," +
                                      " '" + dt["tax_amount2"].ToString() + "'," +
                              " '" + i + 1 + "'," +
                              " '" + dt["tax_percentage"].ToString() + "'," +
                              " '" + dt["taxsegment_gid"].ToString() + "'," +
                              " '" + dt["taxsegmenttax_gid"].ToString() + "'," +
                              " '" + dt["order_type"].ToString() + "')";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                            if (mnResult == 0)
                            {
                                values.status = false;
                                values.message = "Error occurred while Insertion";
                                return;
                            }


                            msSQL = " insert into acp_trn_torderdtl (" +
                             " salesorderdtl_gid ," +
                             " salesorder_gid," +
                             " product_gid ," +
                             " product_name," +
                             " product_price," +
                             " qty_quoted," +
                             " discount_percentage," +
                             " discount_amount," +
                             " tax_amount ," +
                             " uom_gid," +
                             " uom_name," +
                             " price," +
                             " tax_name," +
                             " tax1_gid," +
                             " slno," +
                             " tax_percentage," +
                             " taxsegment_gid," +
                             " type, " +
                             " salesorder_refno" +
                             ")values(" +
                             " '" + dt["tmpsalesorderdtl_gid"].ToString() + "'," +
                             " '" + mssalesorderGID + "'," +
                             " '" + dt["product_gid"].ToString() + "'," +
                             " '" + dt["product_name"].ToString() + "'," +
                             " '" + dt["product_price"].ToString() + "'," +
                             " '" + dt["qty_quoted"].ToString() + "'," +
                             " '" + dt["discount_percentage"].ToString() + "'," +
                             " '" + dt["discount_amount"].ToString() + "'," +
                             " '" + dt["tax_amount"].ToString() + "'," +
                             " '" + dt["uom_gid"].ToString() + "'," +
                             " '" + dt["uom_name"].ToString() + "'," +
                             " '" + dt["price"].ToString() + "'," +
                             " '" + dt["tax_name"].ToString() + "'," +
                              " '" + dt["tax1_gid"].ToString() + "'," +
                             " '" + values.slno + "'," +
                             " '" + dt["tax_percentage"].ToString() + "'," +
                             " '" + dt["taxsegment_gid"].ToString() + "'," +
                             " '" + dt["order_type"].ToString() + "', " +
                             " '" + values.salesorder_refno + "')";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                        }
                    }




                    msSQL = "select distinct type from smr_trn_tsalesorderdtl where salesorder_gid='" + mssalesorderGID + "' ";
                    objodbcDataReader = objdbconn.GetDataReader(msSQL);
                    
                    if (objodbcDataReader.HasRows == true)
                    {

                        if (objodbcDataReader["type"].ToString() != "Services")
                        {
                            lsorder_type = "Sales";
                        }
                        else
                        {
                            lsorder_type = "Services";
                        }

                    }

                    objodbcDataReader.Close();





                    msSQL = " update smr_trn_tsalesorder set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                    msSQL = " update acp_trn_torder set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    string lsstage = "8";
                    msgetlead2campaign_gid = objcmnfunctions.GetMasterGID("BLCC");
                    string lsso = "N";
                    msSQL = " select employee_gid from hrm_mst_temployee where user_gid='" + values.user_name + "'";
                    string employee = objdbconn.GetExecuteScalar(msSQL);


                    msSQL = " Insert into crm_trn_tenquiry2campaign ( " +
                                                      " lead2campaign_gid, " +
                                                      " quotation_gid, " +
                                                      " so_status, " +
                                                      " created_by, " +
                                                      " customer_gid, " +
                                                      " leadstage_gid," +
                                                      " created_date, " +
                                                      " assign_to ) " +
                                                      " Values ( " +
                                                      "'" + msgetlead2campaign_gid + "'," +
                                                      "'" + msGetGid + "'," +
                                                      "'" + lsso + "'," +
                                                      "'" + employee_gid + "',";
                    if (values.customer_gid == null)
                    {
                        msSQL += " '" + values.customergid + "',";
                    }
                    else
                    {
                        msSQL += " '" + values.customer_gid + "',";
                    }
                    msSQL += "'" + lsstage + "'," +
                     "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                     "'" + employee + "')";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    msSQL = "select hierarchy_flag from adm_mst_tcompany where hierarchy_flag ='Y'";
                    objodbcDataReader = objdbconn.GetDataReader(msSQL);
                    if (objodbcDataReader.HasRows == true)
                    {

                        msGetGID = objcmnfunctions.GetMasterGID("PODC");
                        msSQL = " insert into smr_trn_tapproval ( " +
                        " approval_gid, " +
                        " approved_by, " +
                        " approved_date, " +
                        " submodule_gid, " +
                        " soapproval_gid " +
                        " ) values ( " +
                        "'" + msGetGID + "'," +
                        " '" + employee_gid + "'," +
                        "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                        "'SMRSROSOA'," +
                        "'" + mssalesorderGID + "') ";
                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                        msSQL = "select approval_flag from smr_trn_tapproval where submodule_gid='SMRSROSOA' and soapproval_gid='" + mssalesorderGID + "' ";
                        objodbcDataReader = objdbconn.GetDataReader(msSQL);
                        if (objodbcDataReader.HasRows == false)
                        {

                            msSQL = "update smr_trn_tsalesorder set salesorder_status='Approved',salesorder_remarks='" + values.so_remarks + "', " +
                                   " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                            msSQL = "update acp_trn_torder set salesorder_status='Approved',salesorder_remarks='" + values.so_remarks + "', " +
                                  " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                        }
                        else
                        {
                            msSQL = "select approved_by from smr_trn_tapproval where submodule_gid='SMRSROSOA' and soapproval_gid='" + mssalesorderGID + "'";
                            objodbcdatareader1 = objdbconn.GetDataReader(msSQL);
                            if (objodbcdatareader1.RecordsAffected == 1)
                            {

                                msSQL = " update smr_trn_tapproval set " +
                               " approval_flag = 'Y', " +
                               " approved_date = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                               " where approved_by = '" + employee_gid + "'" +
                               " and soapproval_gid = '" + mssalesorderGID + "' and submodule_gid='SMRSROSOA'";
                                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                                msSQL = "update smr_trn_tsalesorder set salesorder_status='Approved',salesorder_remarks='" + values.so_remarks + "', " +
                                   " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
                                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                                msSQL = "update acp_trn_torder set salesorder_status='Approved',salesorder_remarks='" + values.so_remarks + "', " +
                                      " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
                                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                            }
                            else
                            {
                                msSQL = " update smr_trn_tapproval set " +
                                           " approval_flag = 'Y', " +
                                           " approved_date = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                                           " where approved_by = '" + employee_gid + "'" +
                                           " and soapproval_gid = '" + mssalesorderGID + "' and submodule_gid='SMRSROSOA'";
                                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                            }
                            objodbcdatareader1.Close();
                        }



                        if (mnResult2 != 0 || mnResult2 == 0)
                        {

                            msSQL = " delete from smr_tmp_tsalesorderdtl " +
                                    " where employee_gid='" + employee_gid + "'";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                            msSQL = " delete from smr_tmp_tsalesorderdrafts " +
                                   " where salesorder_gid='" + values.salesorder_gid + "'";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                            msSQL = " delete from smr_tmp_tsalesorderdtldrafts " +
                                  " where salesorder_gid='" + values.salesorder_gid + "'";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                        }


                        if (mnResult2 != 0)
                        {
                            values.status = true;
                            values.message = "Sales Order  Raised Successfully";
                            return;
                        }
                        else
                        {
                            values.status = false;
                            values.message = "Error While Raising Sales Order";
                            return;
                        }

                    }
                    objodbcDataReader.Close();

                }
                else
                {
                    values.status = true;
                    values.message = "Select one Product to Raise Enquiry";
                    return;
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while Submitting Sales Order !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }


        }
        //public void DaPostSalesOrderfileupload(IFormFile file, result objResult, string? employee_gid)
        //{
        //    try
        //    {
        //        string customer_gid = Request.Form["customer_gid"];
        //        string customergid = Request.Form["customergid"];
        //        if (customer_gid.Contains("BCRM"))
        //        {
        //            lscustomer_gid = customer_gid;
        //        }
        //        else
        //        {
        //            lscustomer_gid = customergid;
        //        }
        //        string branch_name = Request.Form["branch_name"];
        //        string branch_gid = Request.Form["branch_gid"];
        //        string salesorder_date = Request.Form["salesorder_date"];
        //        string renewal_mode = Request.Form["renewal_mode"];
        //        string renewal_date = Request.Form["renewal_date"];
        //        string frequency_terms = Request.Form["frequency_terms"];
        //        string customer_name = Request.Form["customer_name"];
        //        string so_remarks = Request.Form["so_remarks"];
        //        string so_referencenumber = Request.Form["so_referencenumber"];
        //        string address1 = Request.Form["address1"];
        //        string shipping_address = Request.Form["shipping_address"];
        //        string delivery_days = Request.Form["delivery_days"];
        //        string payment_days = Request.Form["payment_days"];
        //        string currency_code = Request.Form["currency_code"];
        //        string user_name = Request.Form["user_name"];
        //        string exchange_rate = Request.Form["exchange_rate"];
        //        string termsandconditions = Request.Form["termsandconditions"];
        //        string template_name = Request.Form["template_name"];
        //        string template_gid = Request.Form["template_gid"];
        //        string grandtotal = Request.Form["grandtotal"];
        //        string roundoff = Request.Form["roundoff"];
        //        string insurance_charges = Request.Form["insurance_charges"];
        //        string packing_charges = Request.Form["packing_charges"];
        //        string buyback_charges = Request.Form["buyback_charges"];
        //        string freight_charges = Request.Form["freight_charges"];
        //        string additional_discount = Request.Form["additional_discount"];
        //        string addon_charge = Request.Form["addon_charge"];
        //        string tax_amount4 = Request.Form["tax_amount4"];
        //        string tax_name4 = Request.Form["tax_name4"];
        //        string totalamount = Request.Form["totalamount"];
        //        string total_price = Request.Form["total_price"];
        //        string taxsegment_gid = Request.Form["taxsegment_gid"];
        //        string salesorder_gid = Request.Form["salesorder_gid"];

        //        //HttpFileCollection httpFileCollection;
        //        string lsfilepath = string.Empty;
        //        string lsdocument_gid = string.Empty;
        //        MemoryStream ms_stream = new MemoryStream();
        //        string document_gid = string.Empty;
        //        string lscompany_code = string.Empty;
        //        //HttpPostedFile httpPostedFile;
        //        string lspath;
        //        string final_path = "";
        //        string vessel_name = "";

        //        msSQL = " select company_code from adm_mst_tcompany";
        //        lscompany_code = objdbconn.GetExecuteScalar(msSQL);



        //        MemoryStream ms = new MemoryStream();
        //        lspath = ConfigurationManager["Doc_upload_file"] + "/erp_documents" + "/" + lscompany_code + "/" + "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month;



        //        {
        //            if ((!System.IO.Directory.Exists(lspath)))
        //                System.IO.Directory.CreateDirectory(lspath);
        //        }



        //        if (file.Length > 0)
        //        {
        //            string lsfirstdocument_filepath = string.Empty;
        //            for (int i = 0; i < file.Length; i++)
        //            {
        //                string msdocument_gid = objcmnfunctions.GetMasterGID("UPLF");
        //                FileExtensionname = file.FileName;
        //                string lsfile_gid = msdocument_gid;
        //                string lscompany_document_flag = string.Empty;
        //                FileExtension = Path.GetExtension(FileExtensionname).ToLower();
        //                lsfile_gid = lsfile_gid + FileExtension;                    
        //                lspath = ConfigurationManager.AppSettings["Doc_upload_file"] + "/erp_documents" + "/" +
        //                lscompany_code + "/" + "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month 
        //                + "/";                       
        //                lspath = "erp_documents" + "/" + lscompany_code + "/" + "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/";
        //                final_path = lspath + msdocument_gid + FileExtension;
        //            }
        //        }





        //        string totalvalue = user_name;





        //        msSQL = "select tax_prefix from acp_mst_ttax where tax_gid='" + tax_name4 + "'";
        //        string lstaxname1 = objdbconn.GetExecuteScalar(msSQL);


        //        string lscustomerbranch = "H.Q";
        //        string lscampaign_gid = "NO CAMPAIGN";

        //        msSQL = " select * from smr_tmp_tsalesorderdtl " +
        //            " where employee_gid='" + employee_gid + "'";
        //        DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

        //        if (dt_datatable.Rows.Count != 0)
        //        {

        //            string inputDate = salesorder_date;
        //            DateTime uiDate = DateTime.ParseExact(inputDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //            string salesorder_date1 = uiDate.ToString("yyyy-MM-dd");

        //            msSQL = " select customer_name from crm_mst_tcustomer where customer_gid='" + lscustomer_gid + " '";
        //            lscustomername = objdbconn.GetExecuteScalar(msSQL);


        //            msSQL = " select currency_code from crm_trn_tcurrencyexchange where currencyexchange_gid='" + currency_code + " '";
        //            string currency_code1 = objdbconn.GetExecuteScalar(msSQL);

        //            string lslocaladdon = "0.00";
        //            string lslocaladditionaldiscount = "0.00";
        //            string lslocalgrandtotal = " 0.00";
        //            string lsgst = "0.00";
        //            string lsamount4 = "0.00";
        //            //string lsproducttotalamount = "0.00";

        //            double totalAmount = double.TryParse(tax_amount4, out double totalpriceValue) ? totalpriceValue : 0;
        //            double addonCharges = double.TryParse(addon_charge, out double addonChargesValue) ? addonChargesValue : 0;
        //            double freightCharges = double.TryParse(freight_charges, out double freightChargesValue) ? freightChargesValue : 0;
        //            double packingCharges = double.TryParse(packing_charges, out double packingChargesValue) ? packingChargesValue : 0;
        //            double insuranceCharges = double.TryParse(insurance_charges, out double insuranceChargesValue) ? insuranceChargesValue : 0;
        //            double roundoff1 = double.TryParse(roundoff, out double roundoffValue) ? roundoffValue : 0;
        //            double additionaldiscountAmount = double.TryParse(additional_discount, out double discountAmountValue) ? discountAmountValue : 0;
        //            double buybackCharges = double.TryParse(buyback_charges, out double buybackChargesValue) ? buybackChargesValue : 0;

        //            double grandTotal = (totalAmount + addonCharges + freightCharges + packingCharges + insuranceCharges + roundoff1) - additionaldiscountAmount - buybackCharges;

        //            string lsinvoice_refno = "", lsorder_refno = "";
        //            lsrefno = objcmnfunctions.GetMasterGID("SO");
        //            msSQL = "select company_code from adm_mst_Tcompany";
        //            lscompany_code = objdbconn.GetExecuteScalar(msSQL);
        //            if (lscompany_code == "BOBA")
        //            {
        //                string ls_referenceno = objcmnfunctions.getSequencecustomizerGID("INV", "RBL", branch_name);
        //                msSQL = "SELECT SEQUENCE_CURVAL FROM adm_mst_tsequencecodecustomizer  WHERE sequence_code='INV' AND branch_gid='" + branch_name + "'";
        //                string lscode = objdbconn.GetExecuteScalar(msSQL);


        //                lsinvoice_refno = "SI" + " - " + lscode;
        //                lsorder_refno = "SO" + " - " + lscode;

        //            }
        //            else
        //            {
        //                lsinvoice_refno = mssalesorderGID;
        //                lsorder_refno = lsrefno;

        //            }




        //            mssalesorderGID = objcmnfunctions.GetMasterGID("VSOP");

        //            msSQL = " insert  into smr_trn_tsalesorder (" +
        //                     " salesorder_gid ," +
        //                     " branch_gid ," +
        //                     " salesorder_date," +
        //                     " customer_gid," +
        //                     " customer_name," +
        //                     " customer_address ," +
        //                     " bill_to ," +
        //                     " created_by," +
        //                     " so_referenceno1 ," +
        //                      " so_referencenumber ," +
        //                     " so_remarks," +
        //                     " payment_days, " +
        //                     " delivery_days, " +
        //                     " Grandtotal, " +
        //                     " termsandconditions, " +
        //                     " salesorder_status, " +
        //                     " addon_charge, " +
        //                     " additional_discount, " +
        //                     " addon_charge_l, " +
        //                     " additional_discount_l, " +
        //                     " grandtotal_l, " +
        //                     " currency_code, " +
        //                     " currency_gid, " +
        //                     " exchange_rate, " +
        //                      " file_path, " +
        //                     " shipping_to, " +
        //                     " tax_gid," +
        //                     " tax_name, " +
        //                     " gst_amount," +
        //                     " total_price," +
        //                     " total_amount," +
        //                     " tax_amount," +
        //                     " vessel_name," +
        //                     " salesperson_gid," +
        //                     " roundoff, " +
        //                     " updated_addon_charge, " +
        //                     " updated_additional_discount, " +
        //                     " freight_charges," +
        //                     " buyback_charges," +
        //                     " packing_charges," +
        //                     " insurance_charges, " +
        //                      " source_flag, " +
        //                      " renewal_flag ," +
        //                      " file_name ," +
        //                     "created_date" +
        //                     " )values(" +
        //                     " '" + mssalesorderGID + "'," +
        //                     " '" + branch_name + "'," +
        //                     " '" + salesorder_date1 + "'," +
        //                     " '" + lscustomer_gid + "'," +
        //                     " '" + lscustomername.Replace("'", "\\\'") + "'," +
        //                     " '" + address1.Replace("'", "\\\'") + "'," +
        //                     " '" + address1.Replace("'", "\\\'") + "'," +
        //                     " '" + employee_gid + "'," +
        //                     // if(values.so_referencenumber != "" || values.so_referencenumber != null)
        //                     // {
        //                     //msSQL+= "'" + values.so_referencenumber + "',";
        //                     //  }
        //                     // else
        //                     // {
        //                     //        msSQL+=" '" + lsrefno + "',";
        //                     // }
        //                     " '" + lsorder_refno + "'," +
        //                       " '" + lsinvoice_refno + "',";
        //            if (so_remarks != null)
        //            {
        //                msSQL += " '" + so_remarks.Replace("'", "\\\'") + "',";
        //            }
        //            else
        //            {
        //                msSQL += " '" + so_remarks + "',";
        //            }

        //            msSQL += " '" + payment_days + "'," +
        //                " '" + delivery_days + "'," +
        //                " '" + grandtotal.Replace(",", "").Trim() + "',";
        //            if (termsandconditions != null)
        //            {
        //                msSQL += " '" + termsandconditions.Replace("'", "\\\'") + "',";
        //            }
        //            else
        //            {
        //                msSQL += " '" + termsandconditions + "',";
        //            }
        //            msSQL += " 'Approved',";
        //            if (addon_charge != "")
        //            {
        //                msSQL += "'" + addon_charge + "',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + lslocaladdon + "',";
        //            }
        //            if (additional_discount != "")
        //            {
        //                msSQL += "'" + additional_discount + "',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + lslocaladditionaldiscount + "',";
        //            }
        //            if (addon_charge != "")
        //            {
        //                msSQL += "'" + addon_charge + "',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + lslocaladditionaldiscount + "',";
        //            }
        //            if (additional_discount != "")
        //            {
        //                msSQL += "'" + additional_discount + "',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + lslocaladditionaldiscount + "',";
        //            }
        //            msSQL += " '" + lslocalgrandtotal + "'," +
        //                 " '" + currency_code1 + "'," +
        //                 " '" + currency_code + "'," +
        //                 " '" + exchange_rate + "'," +
        //                   " '" + final_path + "'," +
        //                 " '" + shipping_address.Replace("'", "\\\'") + "'," +
        //                 " '" + tax_name4 + "'," +
        //                 " '" + lstaxname1 + "', " +
        //                "'" + lsgst + "',";
        //            msSQL += " '" + totalamount.Replace(",", "").Trim() + "',";
        //            if (grandtotal == null && grandtotal == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += " '" + grandtotal.Replace(",", "").Trim() + "',";
        //            }

        //            if (tax_amount4 != "" && tax_amount4 != null)
        //            {
        //                msSQL += "'" + tax_amount4 + "',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + lsamount4 + "',";
        //            }
        //            msSQL += " '" + vessel_name + "'," +
        //                    " '" + user_name + "',";
        //            if (roundoff == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + roundoff + "',";
        //            }
        //            if (addon_charge == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + addon_charge + "',";
        //            }
        //            if (additional_discount == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + additional_discount + "',";
        //            }
        //            if (freight_charges == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + freight_charges + "',";
        //            }
        //            if (buyback_charges == "")
        //            {

        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + buyback_charges + "',";
        //            }
        //            if (packing_charges == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + packing_charges + "',";
        //            }
        //            if (insurance_charges == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + insurance_charges + "',";
        //            }
        //            msSQL += "'I',";
        //            msSQL += "'" + renewal_mode + "',";
        //            msSQL += "'" + FileExtensionname + "',";

        //            msSQL += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //            if (mnResult == 0)
        //            {
        //                objResult.status = false;
        //                objResult.message = " Some Error Occurred While Inserting Salesorder Details";
        //                return;
        //            }
        //            else
        //            {
        //                msSQL = " insert  into acp_trn_torder (" +
        //                       " salesorder_gid ," +
        //                       " branch_gid ," +
        //                       " salesorder_date," +
        //                       " customer_gid," +
        //                       " customer_name," +
        //                       " customer_address," +
        //                       " created_by," +
        //                       " so_remarks," +
        //                       " so_referencenumber," +
        //                       " payment_days, " +
        //                       " delivery_days, " +
        //                       " Grandtotal, " +
        //                       " termsandconditions, " +
        //                       " salesorder_status, " +
        //                       " addon_charge, " +
        //                       " additional_discount, " +
        //                       " addon_charge_l, " +
        //                       " additional_discount_l, " +
        //                       " grandtotal_l, " +
        //                       " currency_code, " +
        //                       " currency_gid, " +
        //                       " exchange_rate, " +
        //                        " file_path, " +
        //                       " updated_addon_charge, " +
        //                       " updated_additional_discount, " +
        //                       " shipping_to, " +
        //                       " campaign_gid, " +
        //                       " roundoff," +
        //                       " salesperson_gid, " +
        //                       " freight_charges," +
        //                       " buyback_charges," +
        //                       " packing_charges," +
        //                       " insurance_charges " +
        //                       ") values(" +
        //                       " '" + mssalesorderGID + "'," +
        //                       " '" + branch_name + "'," +
        //                       " '" + salesorder_date1 + "'," +
        //                       " '" + lscustomer_gid + "'," +
        //                       " '" + lscustomername.Replace("'", "\\\'") + "'," +
        //                       " '" + address1.Replace("'", "\\\'") + "'," +
        //                       " '" + employee_gid + "',";

        //                if (so_remarks != null)
        //                {
        //                    msSQL += " '" + so_remarks.Replace("'", "\\\'") + "',";
        //                }
        //                else
        //                {
        //                    msSQL += " '" + so_remarks + "',";
        //                }
        //                msSQL += " '" + lsinvoice_refno + "'," +
        //                  " '" + payment_days + "'," +
        //                         " '" + delivery_days + "'," +
        //                         " '" + grandtotal + "'," +
        //                         " '" + termsandconditions.Replace("'", "\\\'") + "'," +
        //                         " 'Approved',";
        //                if (addon_charge == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + addon_charge + "',";
        //                }
        //                if (additional_discount == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + additional_discount + "',";
        //                }
        //                msSQL += "'" + lslocaladdon + "'," +
        //                       " '" + lslocaladditionaldiscount + "'," +
        //                       " '" + lslocalgrandtotal + "'," +
        //                       " '" + currency_code1 + "'," +
        //                       " '" + currency_code + "'," +
        //                       " '" + exchange_rate + "'," +
        //                " '" + final_path + "',";
        //                if (addon_charge == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + addon_charge + "',";
        //                }
        //                if (additional_discount == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + additional_discount + "',";
        //                }
        //                msSQL += "'" + shipping_address.Replace("'", "\\\'") + "'," +
        //                       " '" + lscampaign_gid + "',";
        //                if (roundoff == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + roundoff + "',";
        //                }
        //                msSQL += " '" + user_name + "',";
        //                if (freight_charges == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + freight_charges + "',";
        //                }
        //                if (buyback_charges == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + buyback_charges + "',";
        //                }
        //                if (packing_charges == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + packing_charges + "',";
        //                }
        //                if (insurance_charges == "")
        //                {
        //                    msSQL += "'0.00')";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + insurance_charges + "')";
        //                }
        //                mnResult2 = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                if (mnResult2 == 1)
        //                {
        //                    objResult.status = true;
        //                }

        //            }
        //            if (renewal_mode == "Y")
        //            {
        //                msINGetGID = objcmnfunctions.GetMasterGID("BRLP");

        //                msSQL = " Insert into crm_trn_trenewal ( " +
        //                        " renewal_gid, " +
        //                        " renewal_flag, " +
        //                        " frequency_term, " +
        //                        " customer_gid," +
        //                        " renewal_date, " +
        //                        " salesorder_gid, " +
        //                        " created_by, " +
        //                        " renewal_type, " +
        //                        " created_date) " +
        //                       " Values ( " +
        //                         "'" + msINGetGID + "'," +
        //                         "'" + renewal_mode + "'," +
        //                         "'" + frequency_terms + "'," +
        //                         "'" + lscustomer_gid + "'," +
        //                         "'" + renewal_date + "'," +
        //                         "'" + mssalesorderGID + "'," +
        //                         "'" + employee_gid + "'," +
        //                         "'sales'," +
        //                       "'" + DateTime.Now.ToString("yyyy-MM-dd") + "')";
        //                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //            }
        //            msSQL = " select " +
        //                    " tmpsalesorderdtl_gid," +
        //                    " salesorder_gid," +
        //                    " product_gid," +
        //                    " productgroup_gid," +
        //                    " product_remarks," +
        //                    " product_name," +
        //                     " product_code," +
        //                    " product_price," +
        //                    " qty_quoted," +
        //                    " discount_percentage," +
        //                    " discount_amount," +
        //                    " uom_gid," +
        //                    " uom_name," +
        //                    " price," +
        //                    " tax_name," +
        //                    " tax1_gid, " +
        //                    " tax_amount," +
        //                     " tax_name2," +
        //                    " tax2_gid, " +
        //                    " tax_amount2," +
        //                     " tax_percentage2," +
        //                    " slno," +
        //                    " tax_percentage," +
        //                    " order_type, " +
        //                    " taxsegment_gid, " +
        //                    " taxsegmenttax_gid " +
        //                    " from smr_tmp_tsalesorderdtl" +
        //                    " where employee_gid='" + employee_gid + "' ";
        //            DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
        //            var getModuleList = new List<postsales_list>();
        //            if (dt_datatable.Rows.Count != 0)
        //            {
        //                foreach (DataRow dt in  dt_datatable.Rows)
        //                {
        //                    getModuleList.Add(new postsales_list
        //                    {
        //                        salesorder_gid = dt["salesorder_gid"].ToString(),
        //                        tmpsalesorderdtl_gid = dt["tmpsalesorderdtl_gid"].ToString(),
        //                        product_gid = dt["product_gid"].ToString(),
        //                        product_name = dt["product_name"].ToString(),
        //                        product_code = dt["product_code"].ToString(),
        //                        productuom_name = dt["uom_name"].ToString(),
        //                        productgroup_gid = dt["productgroup_gid"].ToString(),
        //                        product_remarks = dt["product_remarks"].ToString(),
        //                        unitprice = dt["product_price"].ToString(),
        //                        quantity = dt["qty_quoted"].ToString(),
        //                        discountpercentage = dt["discount_percentage"].ToString(),
        //                        discountamount = dt["discount_amount"].ToString(),
        //                        tax_name = dt["tax_name"].ToString(),
        //                        tax_amount = dt["tax_amount"].ToString(),
        //                        totalamount = dt["price"].ToString(),
        //                        order_type = dt["order_type"].ToString(),
        //                        slno = dt["slno"].ToString(),
        //                        taxsegment_gid = dt["taxsegment_gid"].ToString(),
        //                        taxsegmenttax_gid = dt["taxsegmenttax_gid"].ToString(),
        //                    });

        //                    int i = 0;

        //                    //mssalesorderGID1 = objcmnfunctions.GetMasterGID("VSDC");
        //                    //if (mssalesorderGID1 == "E")
        //                    //{
        //                    //    values.message = "Create Sequence code for VSDC ";
        //                    //    return;
        //                    //}

        //                    string display_field = dt["product_remarks"].ToString();

        //                    msSQL = " insert into smr_trn_tsalesorderdtl (" +
        //                         " salesorderdtl_gid ," +
        //                         " salesorder_gid," +
        //                         " product_gid ," +
        //                         " product_name," +
        //                         " product_code," +
        //                         " product_price," +
        //                         " productgroup_gid," +
        //                         " product_remarks," +
        //                         " display_field," +
        //                         " qty_quoted," +
        //                         " discount_percentage," +
        //                         " discount_amount," +
        //                         " tax_amount ," +
        //                         " uom_gid," +
        //                         " uom_name," +
        //                         " price," +
        //                         " tax_name," +
        //                         " tax1_gid," +
        //                          " tax_name2," +
        //                         " tax2_gid," +
        //                          " tax_percentage2," +
        //                           " tax_amount2," +
        //                         " slno," +
        //                         " tax_percentage," +
        //                         " taxsegment_gid," +
        //                         " taxsegmenttax_gid," +
        //                         " type " +
        //                         ")values(" +
        //                         " '" + dt["tmpsalesorderdtl_gid"].ToString() + "'," +
        //                         " '" + mssalesorderGID + "'," +
        //                         " '" + dt["product_gid"].ToString() + "'," +
        //                         " '" + dt["product_name"].ToString() + "'," +
        //                         " '" + dt["product_code"].ToString() + "'," +
        //                         " '" + dt["product_price"].ToString() + "'," +
        //                         " '" + dt["productgroup_gid"].ToString() + "',";
        //                    if (display_field != null)
        //                    {
        //                        msSQL += " '" + display_field.Replace("'", "\\\'") + "'," +
        //                          " '" + display_field.Replace("'", "\\\'") + "',";
        //                    }
        //                    else
        //                    {
        //                        msSQL += " '" + display_field + "'," +
        //                          " '" + display_field + "',";
        //                    }
        //                    msSQL += " '" + dt["qty_quoted"].ToString() + "'," +
        //                          " '" + dt["discount_percentage"].ToString() + "'," +
        //                          " '" + dt["discount_amount"].ToString() + "'," +
        //                          " '" + dt["tax_amount"].ToString() + "'," +
        //                          " '" + dt["uom_gid"].ToString() + "'," +
        //                          " '" + dt["uom_name"].ToString() + "'," +
        //                          " '" + dt["price"].ToString() + "'," +
        //                          " '" + dt["tax_name"].ToString() + "'," +
        //                          " '" + dt["tax1_gid"].ToString() + "'," +
        //                            " '" + dt["tax_name2"].ToString() + "'," +
        //                              " '" + dt["tax2_gid"].ToString() + "'," +
        //                                " '" + dt["tax_percentage2"].ToString() + "'," +
        //                                  " '" + dt["tax_amount2"].ToString() + "'," +
        //                          " '" + i + 1 + "'," +
        //                          " '" + dt["tax_percentage"].ToString() + "'," +
        //                          " '" + dt["taxsegment_gid"].ToString() + "'," +
        //                          " '" + dt["taxsegmenttax_gid"].ToString() + "'," +
        //                          " '" + dt["order_type"].ToString() + "')";
        //                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                    if (mnResult == 0)
        //                    {
        //                        objResult.status = false;
        //                        objResult.message = "Error occurred while Insertion";
        //                        return;
        //                    }


        //                    msSQL = " insert into acp_trn_torderdtl (" +
        //                     " salesorderdtl_gid ," +
        //                     " salesorder_gid," +
        //                     " product_gid ," +
        //                     " product_name," +
        //                     " product_price," +
        //                     " qty_quoted," +
        //                     " discount_percentage," +
        //                     " discount_amount," +
        //                     " tax_amount ," +
        //                     " uom_gid," +
        //                     " uom_name," +
        //                     " price," +
        //                     " tax_name," +
        //                     " tax1_gid," +
        //                     " slno," +
        //                     " tax_percentage," +
        //                     " taxsegment_gid," +
        //                     " type, " +
        //                     " salesorder_refno" +
        //                     ")values(" +
        //                     " '" + dt["tmpsalesorderdtl_gid"].ToString() + "'," +
        //                     " '" + mssalesorderGID + "'," +
        //                     " '" + dt["product_gid"].ToString() + "'," +
        //                     " '" + dt["product_name"].ToString() + "'," +
        //                     " '" + dt["product_price"].ToString() + "'," +
        //                     " '" + dt["qty_quoted"].ToString() + "'," +
        //                     " '" + dt["discount_percentage"].ToString() + "'," +
        //                     " '" + dt["discount_amount"].ToString() + "'," +
        //                     " '" + dt["tax_amount"].ToString() + "'," +
        //                     " '" + dt["uom_gid"].ToString() + "'," +
        //                     " '" + dt["uom_name"].ToString() + "'," +
        //                     " '" + dt["price"].ToString() + "'," +
        //                     " '" + dt["tax_name"].ToString() + "'," +
        //                      " '" + dt["tax1_gid"].ToString() + "'," +
        //                     " '" + i + 1 + "'," +
        //                     " '" + dt["tax_percentage"].ToString() + "'," +
        //                     " '" + dt["taxsegment_gid"].ToString() + "'," +
        //                     " '" + dt["order_type"].ToString() + "', " +
        //                     " '" + so_referencenumber + "')";
        //                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                }
        //            }




        //            msSQL = "select distinct type from smr_trn_tsalesorderdtl where salesorder_gid='" + mssalesorderGID + "' ";
        //            objodbcDataReader = objdbconn.GetDataReader(msSQL);

        //            if (objodbcDataReader.HasRows == true)
        //            {
        //                if (objodbcDataReader["type"].ToString() != "Services")
        //                {
        //                    lsorder_type = "Sales";
        //                }
        //                else
        //                {
        //                    lsorder_type = "Services";
        //                }

        //            }


        //            objodbcDataReader.Close();





        //            msSQL = " update smr_trn_tsalesorder set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //            msSQL = " update acp_trn_torder set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //            string lsstage = "8";
        //            msgetlead2campaign_gid = objcmnfunctions.GetMasterGID("BLCC");
        //            string lsso = "N";
        //            msSQL = " select employee_gid from hrm_mst_temployee where user_gid='" + user_name + "'";
        //            string employee = objdbconn.GetExecuteScalar(msSQL);


        //            msSQL = " Insert into crm_trn_tenquiry2campaign ( " +
        //                                              " lead2campaign_gid, " +
        //                                              " quotation_gid, " +
        //                                              " so_status, " +
        //                                              " created_by, " +
        //                                              " customer_gid, " +
        //                                              " leadstage_gid," +
        //                                              " created_date, " +
        //                                              " assign_to ) " +
        //                                              " Values ( " +
        //                                              "'" + msgetlead2campaign_gid + "'," +
        //                                              "'" + msGetGid + "'," +
        //                                              "'" + lsso + "'," +
        //                                              "'" + employee_gid + "'," +
        //                                              "'" + lscustomer_gid + "'," +
        //                                              "'" + lsstage + "'," +
        //                                              "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
        //                                              "'" + employee + "')";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //            msSQL = "select hierarchy_flag from adm_mst_tcompany where hierarchy_flag ='Y'";
        //            objodbcDataReader = objdbconn.GetDataReader(msSQL);

        //            if (objodbcDataReader.HasRows == true)
        //            {

        //                msGetGID = objcmnfunctions.GetMasterGID("PODC");
        //                msSQL = " insert into smr_trn_tapproval ( " +
        //                " approval_gid, " +
        //                " approved_by, " +
        //                " approved_date, " +
        //                " submodule_gid, " +
        //                " soapproval_gid " +
        //                " ) values ( " +
        //                "'" + msGetGID + "'," +
        //                " '" + employee_gid + "'," +
        //                "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
        //                "'SMRSROSOA'," +
        //                "'" + mssalesorderGID + "') ";
        //                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                msSQL = "select approval_flag from smr_trn_tapproval where submodule_gid='SMRSROSOA' and soapproval_gid='" + mssalesorderGID + "' ";
        //                objodbcDataReader = objdbconn.GetDataReader(msSQL);
        //                if (objodbcDataReader.HasRows == false)
        //                {

        //                    msSQL = "update smr_trn_tsalesorder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
        //                           " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                    msSQL = "update acp_trn_torder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
        //                          " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                }
        //                else
        //                {
        //                    msSQL = "select approved_by from smr_trn_tapproval where submodule_gid='SMRSROSOA' and soapproval_gid='" + mssalesorderGID + "'";
        //                    objodbcdatareader1 = objdbconn.GetDataReader(msSQL);
        //                    if (objodbcdatareader1.RecordsAffected == 1)
        //                    {

        //                        msSQL = " update smr_trn_tapproval set " +
        //                       " approval_flag = 'Y', " +
        //                       " approved_date = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
        //                       " where approved_by = '" + employee_gid + "'" +
        //                       " and soapproval_gid = '" + mssalesorderGID + "' and submodule_gid='SMRSROSOA'";
        //                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                        msSQL = "update smr_trn_tsalesorder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
        //                           " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                        msSQL = "update acp_trn_torder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
        //                              " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                    }
        //                    else
        //                    {
        //                        msSQL = " update smr_trn_tapproval set " +
        //                                   " approval_flag = 'Y', " +
        //                                   " approved_date = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
        //                                   " where approved_by = '" + employee_gid + "'" +
        //                                   " and soapproval_gid = '" + mssalesorderGID + "' and submodule_gid='SMRSROSOA'";
        //                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                    }
        //                }



        //                if (mnResult2 != 0 || mnResult2 == 0)
        //                {

        //                    msSQL = " delete from smr_tmp_tsalesorderdtl " +
        //                            " where employee_gid='" + employee_gid + "'";
        //                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                    msSQL = " delete from smr_tmp_tsalesorderdrafts " +
        //                          " where salesorder_gid='" + salesorder_gid + "'";
        //                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                }


        //                if (mnResult2 != 0)
        //                {
        //                    objResult.status = true;
        //                    objResult.message = "Sales Order  Raised Successfully";
        //                    return;
        //                }
        //                else
        //                {
        //                    objResult.status = false;
        //                    objResult.message = "Error While Raising Sales Order";
        //                    return;
        //                }

        //            }
        //            objodbcDataReader.Close();
        //        }
        //        else
        //        {
        //            objResult.status = true;
        //            objResult.message = "Select one Product to Raise Enquiry";
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objResult.message = "Exception occured while Submitting Sales Order !";
        //        objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
        //        $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
        //        objResult.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
        //    }


        //}
        public void DaPostOnAdds(string employee_gid, salesorders_list values)
        {
            try
            {

                foreach (var data in values.SOProductList)
                {
                    if (data.quantity == null || data.quantity == "0")
                    {

                    }
                    else
                    {
                        msGetGid = objcmnfunctions.GetMasterGID("VSDT");

                        msSQL = "select product_gid from pmr_mst_tproduct where product_name='" + data.product_name + "'";
                        string lsproductgid = objdbconn.GetExecuteScalar(msSQL);

                        msSQL = "select productuom_gid from pmr_mst_tproductuom where productuom_name='" + data.productuom_name + "'";
                        string lsproductuomgid = objdbconn.GetExecuteScalar(msSQL);

                        msSQL = " SELECT e.taxsegment_gid, case when d.tax_name like '%CGST' then d.tax_name else null  end as tax1 ," +
                               " case when d.tax_name like '%SGST' then d.tax_name else null  end as tax2, " +
                               " case when d.tax_name like '%IGST' then d.tax_name else null  end as tax3, " +
                               " d.tax_name  as tax_name," +
                               " e.taxsegment_name, d.tax_name, d.tax_gid, d.tax_percentage," +
                               " d.tax_amount, a.mrp_price," +
                               " a.cost_price" +
                               " FROM acp_mst_ttaxsegment2product d " +
                               " LEFT JOIN acp_mst_ttaxsegment e ON e.taxsegment_gid = d.taxsegment_gid " +
                               " LEFT JOIN crm_mst_tcustomer f ON f.taxsegment_gid = e.taxsegment_gid " +
                               " LEFT JOIN pmr_mst_tproduct a ON a.product_gid = d.product_gid " +
                               " where a.product_gid='" + lsproductgid + "' and f.customer_gid='" + values.customer_gid + "'";
                        objodbcDataReader = objdbconn.GetDataReader(msSQL);
                        if (objodbcDataReader.HasRows == true)
                        {

                            taxsegment_gid = objodbcDataReader["taxsegment_gid"].ToString();
                            taxsegment_name = objodbcDataReader["taxsegment_name"].ToString();
                            tax_name = objodbcDataReader["tax_name"].ToString();
                            tax_gid = objodbcDataReader["tax_gid"].ToString();
                            tax_percentage = objodbcDataReader["tax_percentage"].ToString();
                            tax1 = objodbcDataReader["tax1"].ToString();
                            tax2 = objodbcDataReader["tax2"].ToString();
                            tax3 = objodbcDataReader["tax3"].ToString();
                            tax_amount = objodbcDataReader["tax_amount"].ToString();
                            mrp_price = objodbcDataReader["mrp_price"].ToString();
                            cost_price = objodbcDataReader["cost_price"].ToString();

                        }
                        objodbcDataReader.Close();

                        if (data.discount_percentage == null || data.discount_percentage == 0)
                        {
                            lsdiscountpercentage = 0.00;
                        }
                        else
                        {
                            lsdiscountpercentage = (double)data.discount_percentage;
                        }

                        if (data.discountamount == null || data.discountamount == "")
                        {
                            lsdiscountamount = "0.00";
                        }
                        else
                        {
                            lsdiscountamount = data.discountamount;
                        }


                        //msSQL = "select tax_gid from acp_mst_ttax where tax_name='" + data.tax_name + "'";
                        //string lstaxgid = objdbconn.GetExecuteScalar(msSQL);
                        //msSQL = "select percentage from acp_mst_ttax where tax_gid='" + lstaxgid + "'";
                        //string lspercentage1 = objdbconn.GetExecuteScalar(msSQL);



                   
                        msSQL = "select producttype_name from pmr_mst_tproduct WHERE product_gid='" + lsproductgid + "'";
                        objodbcDataReader = objdbconn.GetDataReader(msSQL);
                        if (objodbcDataReader.HasRows == true)
                        {

                            if (objodbcDataReader["producttype_name"].ToString() != "Services")
                            {
                                lsorder_type = "Sales";
                            }
                            else
                            {
                                lsorder_type = "Services";
                            }

                        }
                        objodbcDataReader.Close();
                        if ((values.taxprecentage1 == "" || values.taxprecentage1 == null) && (values.taxamount1 == null || values.taxamount1 == ""))
                        {
                            lstaxamount = "0.00";
                            lspercentage1 = "0";
                        }
                        try
                        {

                            double exchange, costPrice, quantity;
                            decimal discountPercentage;

                            if (!double.TryParse(values.exchange_rate, out exchange))
                            {
                                exchange = 0.0;
                            }

                            if (!double.TryParse(data.unitprice, out costPrice))
                            {
                                costPrice = 0.0;
                            }


                            if (!double.TryParse(data.quantity, out quantity))
                            {
                                quantity = 0.0;
                            }


                            if (!decimal.TryParse(data.discount_percentage.ToString(), out discountPercentage))
                            {
                                discountPercentage = 0;
                            }
                            if (!double.TryParse(values.totaltaxamount, out taxAmount))
                            {
                                taxAmount = 0.0;
                            }

                            //subtotal = exchange * costPrice * quantity;
                            //reCaldiscountAmount = (subtotal * discountPercentage) / 100;
                            //reCalTaxAmount = taxAmount;
                            //reCalTotalAmount = subtotal - reCaldiscountAmount + reCalTaxAmount;
                            //rreCalTotalAmount = reCalTotalAmount;
                            msSQL = "select currency_code from crm_trn_tcurrencyexchange where curencyexchange_gid='" + data.currency_code + "'";
                            string lscurrency = objdbconn.GetExecuteScalar(msSQL);

                            if (lscurrency != "INR")
                            {
                                if (reCalTotalAmount % 1 != 0)
                                {
                                    // Round to the nearest 0.5 if total amount has cents
                                    reCalTotalAmount = Math.Round(reCalTotalAmount * 2, MidpointRounding.AwayFromZero) / 2;

                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            values.message = "Exception occured while calculating!";
                            objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                            $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                            values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
                        }
                        if (data.quantity == null || data.quantity == "" || data.quantity == "undefined" || Convert.ToInt32(data.quantity) < 1)
                        {

                            values.status = false;
                            values.message = "Product quantity cannot be zero or empty.";
                            return;
                        }


                        else if (data.unitprice == null || data.unitprice == "" || data.unitprice == "undefined")
                        {
                            values.status = false;
                            values.message = "Price cannot be left empty or set to zero";
                            return;
                        }
                        else
                        {

                            msSQL = " insert into smr_tmp_tsalesorderdtl( " +
                       " tmpsalesorderdtl_gid," +
                       " salesorder_gid," +
                       " employee_gid," +
                       " product_gid," +
                       " product_code," +
                       " product_name," +
                       " product_price," +
                       " qty_quoted," +
                       " uom_gid," +
                       " uom_name," +
                       " price," +
                       " order_type," +
                       " taxsegment_gid, " +
                      " taxsegmenttax_gid, " +
                      " tax1_gid, " +
                      " tax2_gid, " +
                      " tax3_gid, " +
                      " tax_name, " +
                      " tax_name2, " +
                      " tax_name3, " +
                      " tax_percentage, " +
                      " tax_percentage2, " +
                      " tax_percentage3, " +
                      " tax_amount, " +
                      " tax_amount2, " +
                      " tax_amount3, " +
                       " discount_amount, " +
                       " discount_percentage" +
                       ")values(" +
                       "'" + msGetGid + "'," +
                       "'" + data.salesorder_gid + "'," +
                       "'" + employee_gid + "'," +
                       "'" + lsproductgid + "'," +
                       "'" + data.product_code + "'," +
                       "'" + data.product_name + "'," +
                       "'" + data.unitprice + "'," +
                       "'" + data.quantity + "'," +
                       "'" + lsproductuomgid + "'," +
                       "'" + data.productuom_name + "'," +
                       "'" + values.total_amount + "'," +
                       " '" + lsorder_type + "', " +
                       " '" + values.taxsegment_gid + "', " +
                       " '" + values.taxsegment_gid + "', " +
                       " '" + values.taxgid1 + "', " +
                       " '" + values.taxgid2 + "', " +
                       " '" + values.taxgid3 + "', " +
                       " '" + values.taxname1 + "', " +
                       " '" + values.taxname2 + "', " +
                       " '" + values.taxname3 + "', ";
                            if (values.taxprecentage1 == "" || values.taxprecentage1 == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + values.taxprecentage1 + "', ";
                            }
                            if (values.taxprecentage2 == "" || values.taxprecentage2 == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + values.taxprecentage2 + "', ";
                            }
                            if (values.taxprecentage3 == "" || values.taxprecentage3 == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + values.taxprecentage3 + "', ";
                            }
                            if (values.taxamount1 == "" || values.taxamount1 == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + values.taxamount1 + "', ";
                            }
                            if (values.taxamount2 == "" || values.taxamount2 == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + values.taxamount2 + "', ";
                            }
                            if (values.taxamount3 == "" || values.taxamount3 == null)
                            {
                                msSQL += "'0.00',";
                            }
                            else
                            {
                                msSQL += " '" + values.taxamount3 + "', ";
                            }

                            msSQL +=
                               "'" + values.discount_amount + "'," +
                               "'" + lsdiscountpercentage + "')";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                            if (mnResult != 0)
                            {
                                values.status = true;
                                values.message = "Product Added Successfully";
                            }
                            else
                            {
                                values.status = false;
                                values.message = "Error While Adding Product";
                            }

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while Adding Product !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetOnChangeProductGroup(string productgroup_gid, MdlSmrTrnSalesorder values)
        {
            try
            {


                if (productgroup_gid != null)
                {
                    msSQL = "select product_gid,product_name from pmr_mst_tproduct where productgroup_gid = '" + productgroup_gid + "'";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                    var getModuleList = new List<GetCustomerDet>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in  dt_datatable.Rows)
                        {
                            getModuleList.Add(new GetCustomerDet
                            {
                                product_name = dt["product_name"].ToString(),
                                product_gid = dt["product_gid"].ToString()

                            });
                            values.GetCustomer = getModuleList;
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Changing Customer !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaGetOnChangeProductGroupName(string product_gid, MdlSmrTrnSalesorder values)
        {
            try
            {


                if (product_gid != null)
                {
                    msSQL = "select product_gid,product_code from pmr_mst_tproduct where product_gid='" + product_gid + "'";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                    var getModuleList = new List<GetProdGrpName>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in  dt_datatable.Rows)
                        {
                            getModuleList.Add(new GetProdGrpName
                            {
                                product_code = dt["product_code"].ToString(),
                                product_gid = dt["product_gid"].ToString()

                            });
                            values.GetProdGrpName = getModuleList;
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Changing Customer !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }

        }
        public void DaPostProductAdd(PostProduct_list values)
        {
            try
            {
                string lsproducttype_name = "";
                msGetGid = objcmnfunctions.GetMasterGID("VSDT");
                msSQL = " SELECT a.productuom_gid, a.product_gid, a.product_name,a.producttype_name, b.productuom_name FROM pmr_mst_tproduct a " +
                     " LEFT JOIN pmr_mst_tproductuom b ON a.productuom_gid = b.productuom_gid " +
                     " WHERE product_gid = '" + values.product_name + "'";
                objodbcDataReader = objdbconn.GetDataReader(msSQL);
                if (objodbcDataReader.HasRows == true)
                {

                    lsproductgid = objodbcDataReader["product_gid"].ToString();
                    lsproductuom_gid = objodbcDataReader["productuom_gid"].ToString();
                    lsproduct_name = objodbcDataReader["product_name"].ToString();
                    lsproductuom_name = objodbcDataReader["productuom_name"].ToString();
                    lsproducttype_name = objodbcDataReader["producttype_name"].ToString();

                }
                objodbcDataReader.Close();

                if (values.productdiscount == null || values.productdiscount == 0)
                {
                    lsdiscountpercentage = 0.00;
                }
                else
                {
                    lsdiscountpercentage = (double)values.productdiscount;
                }

                if (values.discount_amount == null || values.discount_amount == 0)
                {
                    lsdiscountamount = "0.00";
                }
                else
                {
                    lsdiscountamount = values.discount_amount.ToString();
                }
                if (lsproducttype_name != "Services")
                {
                    lsorder_type = "Sales";   
                }
                else
                {
                    lsorder_type = "Services";

                }
                
                if (values.productquantity == "undefined" || values.productquantity == null || values.productquantity == "" || Convert.ToInt32(values.productquantity) < 1)
                {
                    values.status = false;
                    values.message = "Product quantity cannot be zero or empty";
                    return;

                }
                else if (values.unitprice == null || values.unitprice == "" || values.unitprice == "undefined")
                {
                    values.status = false;
                    values.message = "Price cannot be left empty or set to zero";
                    return;
                }
                else
                {
                    msSQL = " insert into smr_tmp_tsalesorderdtl( " +
                              " tmpsalesorderdtl_gid," +
                              " employee_gid," +
                              " product_gid," +
                              " product_code," +
                               " customerproduct_code," +
                              " product_name," +
                              " productgroup_gid," +
                              " product_price," +
                              " qty_quoted," +
                              " uom_gid," +
                              " uom_name," +
                              " price," +
                              " order_type," +
                              " tax_rate, " +
                              " taxsegment_gid, " +
                             " taxsegmenttax_gid, " +
                             " tax1_gid, " +
                             " tax2_gid, " +
                             " tax3_gid, " +
                             " tax_name, " +
                             " tax_name2, " +
                             " tax_name3, " +
                             " tax_percentage, " +
                             " tax_percentage2, " +
                             " tax_percentage3, " +
                             " tax_amount, " +
                             " tax_amount2, " +
                             " tax_amount3, " +
                              " discount_amount, " +
                              " product_remarks, " +
                              " discount_percentage" +
                              ")values(" +
                              "'" + msGetGid + "'," +
                              "'" + employee_gid + "'," +
                              "'" + lsproductgid + "'," +
                              "'" + values.product_code + "'," +
                               "'" + values.product_code + "'," +
                              "'" + lsproduct_name + "'," +
                              "'" + values.productgroup_name + "'," +
                              "'" + values.unitprice + "'," +
                              "'" + values.productquantity + "'," +
                              "'" + lsproductuom_gid + "'," +
                              "'" + lsproductuom_name + "'," +
                              "'" + values.producttotal_amount + "'," +
                              " '" + lsorder_type + "', " +
                              " '" + values.tax_prefix + "', " +
                              " '" + values.taxsegment_gid + "', " +
                              " '" + values.taxsegment_gid + "', " +
                              " '" + taxgid1 + "', " +
                              " '" + taxgid1 + "', " +
                              " '" + taxgid1 + "', " +
                              " '" + values.tax_prefix + "', " +
                              " '" + values.tax_prefix2 + "', " +
                              " '" + values.taxname3 + "', ";
                    if (values.taxprecentage1 == 0 || values.taxprecentage1 == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += " '" + values.taxprecentage1 + "', ";
                    }
                    if (values.taxprecentage2 == 0 || values.taxprecentage2 == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += " '" + values.taxprecentage2 + "', ";
                    }
                    if (values.taxprecentage3 == 0 || values.taxprecentage3 == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += " '" + values.taxprecentage3 + "', ";
                    }
                    if (values.taxamount1 == "" || values.taxamount1 == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += " '" + values.taxamount1 + "', ";
                    }
                    if (values.taxamount2 == 0 || values.taxamount2 == 0)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += " '" + values.taxamount2 + "', ";
                    }
                    if (values.taxamount3 == 0 || values.taxamount3 == 0)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += " '" + values.taxamount3 + "', ";
                    }

                    msSQL +=
                       "'" + values.discount_amount + "',";
                    if (values.product_remarks != null)
                    {
                        msSQL += "'" + values.product_remarks.Replace("'", "\\\'") + "',";
                    }
                    else
                    {
                        msSQL += "'" + values.product_remarks + "',";
                    }

                    msSQL += "'" + lsdiscountpercentage + "')";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);


                    if (mnResult != 0)
                    {
                        values.status = true;
                        values.message = "Product Added Successfully";
                    }
                    else
                    {
                        values.status = false;
                        values.message = "Error While Adding Product";
                    }
                }


            }
            catch (Exception ex)
            {
                values.message = "Exception occured while Adding Product!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }
        }
        public void DaGetProductGroup(MdlSmrTrnSalesorder values)
        {
            msSQL = " select productgroup_gid, productgroup_name from pmr_mst_tproductgroup";
            DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
            var getModuleList = new List<Getproductgroup>();
            if (dt_datatable.Rows.Count != 0)
            {
                foreach (DataRow dt in  dt_datatable.Rows)
                {
                    getModuleList.Add(new Getproductgroup
                    {
                        productgroup_gid = dt["productgroup_gid"].ToString(),
                        productgroup_name = dt["productgroup_name"].ToString(),
                    });
                    values.Getproductgroup = getModuleList;
                }
            }
            dt_datatable.Dispose();
        }
        public void DaGetOnChangeBranch(string branch_gid, MdlSmrTrnSalesorder values)
        {
            msSQL = "select address1,branch_gid,branch_name from hrm_mst_tbranch where branch_gid = '" + branch_gid + "'";
            DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
            var GetOnChangeBranch = new List<GetOnChangeBranch_list>();
            if (dt_datatable.Rows.Count > 0)
            {
                foreach (DataRow dt in  dt_datatable.Rows)
                {
                    GetOnChangeBranch.Add(new GetOnChangeBranch_list
                    {
                        address1 = dt["address1"].ToString(),
                        branch_name = dt["branch_name"].ToString(),
                        branch_gid = dt["branch_gid"].ToString(),
                    });
                    values.GetOnChangeBranch_list = GetOnChangeBranch;
                }
            }
            dt_datatable.Dispose();
        }

        ///////------------------------------approval------------------------------------------------------------------/////
        public void DaGetApprovalSalesOrderSummary(string salesorder_gid, MdlSmrTrnSalesorder values)
        {
            try
            {

                msSQL = " SELECT distinct a.salesorder_gid, h.currency_code, a.customerbranch_gid, a.exchange_rate, " +
                    " DATE_FORMAT(a.salesorder_date, '%d-%m-%Y') AS salesorder_date, a.salesperson_gid, a.customer_instruction," +
                    " concat(g.user_code, ' / ', g.user_firstname, ' ', g.user_lastname) AS salesperson_name, " +
                    " FORMAT(a.Grandtotal, 2) AS Grandtotal, a.termsandconditions, FORMAT(a.addon_charge, 2) AS addon_charge, " +
                    " FORMAT(a.additional_discount_l, 2) AS additional_discount, a.payment_days, FORMAT(a.tax_amount, 2) AS tax_amount, " +
                    " a.delivery_days, a.so_referenceno1, a.payment_terms, a.freight_terms, " +
                    " FORMAT(a.roundoff, 2) AS roundoff, a.so_remarks, a.shipping_to, " +
                    " a.customer_address, a.customer_name,  a.customer_contact_person AS customer_contact_person, " +
                    " case when a.start_date = '0000-00-00' then '' else DATE_FORMAT(a.start_date, '%d-%m-%Y') end AS start_date,case when a.end_date = '0000-00-00' then '' else DATE_FORMAT(a.end_date, '%d-%m-%Y') end AS end_date, " +
                    " a.termsandconditions, a.customer_mobile, a.customer_email, e.branch_name, a.order_instruction," +
                    " FORMAT(a.total_amount, 2) AS total_amount, FORMAT(a.freight_charges, 2) AS freight_charges, " +
                    " FORMAT(a.packing_charges, 2) AS packing_charges,format(a.total_price, 2) as total_price,a.tax_name, FORMAT(a.buyback_charges, 2) AS buyback_charges, " +
                    " FORMAT(a.insurance_charges, 2) AS insurance_charges FROM smr_trn_tsalesorder a " +
                    " LEFT JOIN smr_trn_tsalesorderdtl d ON d.salesorder_gid = a.salesorder_gid " +
                    " LEFT JOIN hrm_mst_tbranch e ON e.branch_gid = a.branch_gid " +
                    " LEFT JOIN acp_mst_ttax f ON f.tax_gid = a.tax_gid " +
                    " LEFT JOIN adm_mst_tuser g ON g.user_gid = a.salesperson_gid " +
                    " LEFT JOIN crm_trn_tcurrencyexchange h ON a.currency_gid = h.currencyexchange_gid " +
                    " WHERE a.salesorder_gid = '" + salesorder_gid + "' GROUP BY a.salesorder_gid, d.product_gid ORDER BY a.salesorder_gid ASC";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<postsalesorder_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        getModuleList.Add(new postsalesorder_list
                        {

                            customer_instruction = dt["customer_instruction"].ToString(),
                            salesorder_gid = dt["salesorder_gid"].ToString(),
                            tax_name = dt["tax_name"].ToString(),
                            salesorder_date = dt["salesorder_date"].ToString(),
                            customer_name = dt["customer_name"].ToString(),
                            branch_name = dt["branch_name"].ToString(),
                            customer_contact_person = dt["customer_contact_person"].ToString(),
                            customer_email = dt["customer_email"].ToString(),
                            customer_mobile = dt["customer_mobile"].ToString(),
                            customer_address = dt["customer_address"].ToString(),
                            start_date = dt["start_date"].ToString(),
                            end_date = dt["end_date"].ToString(),
                            currency_code = dt["currency_code"].ToString(),
                            exchange_rate = dt["exchange_rate"].ToString(),
                            freight_terms = dt["freight_terms"].ToString(),
                            payment_terms = dt["payment_terms"].ToString(),
                            payment_days = dt["payment_days"].ToString(),
                            so_referencenumber = dt["so_referenceno1"].ToString(),
                            shipping_to = dt["shipping_to"].ToString(),
                            delivery_days = dt["delivery_days"].ToString(),
                            so_remarks = dt["so_remarks"].ToString(),
                            salesperson_name = dt["salesperson_name"].ToString(),
                            addon_charge = dt["addon_charge"].ToString(),
                            additional_discount = dt["additional_discount"].ToString(),
                            freight_charges = dt["freight_charges"].ToString(),
                            buyback_charges = dt["buyback_charges"].ToString(),
                            packing_charges = dt["packing_charges"].ToString(),
                            insurance_charges = dt["insurance_charges"].ToString(),
                            roundoff = dt["roundoff"].ToString(),
                            Grandtotal = dt["Grandtotal"].ToString(),
                            termsandconditions = dt["termsandconditions"].ToString(),
                            total_price = dt["total_price"].ToString(),
                            price = dt["total_price"].ToString(),
                            total_amount = dt["total_amount"].ToString(),
                            tax_amount = dt["tax_amount"].ToString(),
                            order_instruction = dt["order_instruction"].ToString(),


                        });
                        values.postsalesorder_list = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading Sales Order Summary !";

                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }
        }

        public void DaGetapprovalsalesorderdetails(string salesorder_gid, MdlSmrTrnSalesorder values)
        {
            msSQL = " select a.salesorderdtl_gid,a.vendor_price,a.salesorder_gid,a.customerproduct_code,e.vendor_companyname,a.product_gid,a.productgroup_gid," +
                "a.productgroup_name,a.product_name,b.product_image,format(a.product_price,2)as product_price,a.qty_quoted," +
              " a.product_remarks,  a.uom_gid, a.uom_name, a.payment_days, a.delivery_period, format(a.price,2)as price , " +
              " a.display_field,format(a.selling_price,2)as selling_price," +
              " format(discount_percentage,2)as discount_percentage,format(discount_amount,2)as discount_amount ,format(tax_percentage,2)as tax_percentage," +
              " format(tax_amount,2)as tax_amount ,  format(a.price,2)as price , " +
              " a.tax_name,a.tax_name2,a.tax_name3,format(a.tax_percentage2,2)as tax_percentage2 ," +
              "format(a.tax_percentage3,2)as tax_percentage3  ," +
              " format(a.tax_amount2,2)as tax_amount2 , format(a.tax_amount3,2)as tax_amount3 ," +
              " salesorder_refno, product_delivered, " +
              " a.salesorder_refno, a.product_delivered,a.slno,a.product_requireddateremarks, " +
              " format(a.margin_percentage,2)as margin_percentage,format(a.margin_amount,2)as margin_amount," +
              "b.product_code,date_format(product_requireddate,'%d-%m-%Y') as product_requireddate   " +
              " from smr_trn_tsalesorderdtl a" +
              " left join pmr_mst_tproduct b on b.product_gid=a.product_gid" +
               " left join acp_mst_tvendor e on a.vendor_gid=e.vendor_gid " +
              " where a.salesorder_gid = '" + salesorder_gid + "' order by salesorderdtl_gid asc ";
            DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
            var Getapprovalsalesorder = new List<Getapprovalsalesorder_list>();
            if (dt_datatable.Rows.Count > 0)
            {
                foreach (DataRow dt in  dt_datatable.Rows)
                {
                    Getapprovalsalesorder.Add(new Getapprovalsalesorder_list
                    {
                        salesorderdtl_gid = dt["salesorderdtl_gid"].ToString(),
                        vendor_price = dt["vendor_price"].ToString(),
                        salesorder_gid = dt["salesorder_gid"].ToString(),
                        customerproduct_code = dt["customerproduct_code"].ToString(),
                        vendor_companyname = dt["vendor_companyname"].ToString(),
                        product_gid = dt["product_gid"].ToString(),
                        productgroup_gid = dt["productgroup_gid"].ToString(),
                        productgroup_name = dt["productgroup_name"].ToString(),
                        product_name = dt["product_name"].ToString(),
                        product_price = dt["product_price"].ToString(),
                        qty_quoted = dt["qty_quoted"].ToString(),
                        product_remarks = dt["product_remarks"].ToString(),
                        uom_gid = dt["uom_gid"].ToString(),
                        uom_name = dt["uom_name"].ToString(),
                        payment_days = dt["payment_days"].ToString(),
                        price = dt["price"].ToString(),
                        delivery_period = dt["delivery_period"].ToString(),
                        display_field = dt["display_field"].ToString(),
                        selling_price = dt["selling_price"].ToString(),
                        discount_percentage = dt["discount_percentage"].ToString(),
                        discount_amount = dt["discount_amount"].ToString(),
                        tax_percentage = dt["tax_percentage"].ToString(),
                        tax_amount = dt["tax_amount"].ToString(),
                        tax_name2 = dt["tax_name2"].ToString(),
                        tax_name3 = dt["tax_name3"].ToString(),
                        tax_name = dt["tax_name"].ToString(),
                        tax_percentage2 = dt["tax_percentage2"].ToString(),
                        tax_percentage3 = dt["tax_percentage3"].ToString(),
                        tax_amount2 = dt["tax_amount2"].ToString(),
                        tax_amount3 = dt["tax_amount3"].ToString(),
                        salesorder_refno = dt["salesorder_refno"].ToString(),
                        product_delivered = dt["product_delivered"].ToString(),
                        slno = dt["slno"].ToString(),
                        product_requireddateremarks = dt["product_requireddateremarks"].ToString(),
                        margin_percentage = dt["margin_percentage"].ToString(),
                        margin_amount = dt["margin_amount"].ToString(),
                        product_requireddate = dt["product_requireddate"].ToString(),
                        product_code = dt["product_code"].ToString(),
                    });
                    values.Getapprovalsalesorder_list = Getapprovalsalesorder;
                }
            }
        }
        public void DaGetsalesorderdtlrecords(string salesorder_gid, MdlSmrTrnSalesorder values)
        {

            try

            {

                msSQL = " select d.product_gid,i.productgroup_name,d.product_remarks,d.product_name,d.salesorderdtl_gid,d.product_code,d.uom_name,d.qty_quoted,d.margin_amount,d.margin_percentage,d.discount_percentage, d.discount_amount," +

                    " FORMAT(d.product_price, 2) AS product_price ,d.tax_name,format(d.tax_amount, 2) as tax_amount,FORMAT(d.price, 2) AS price,e.tax_prefix " +

                    " FROM smr_trn_tsalesorder a LEFT JOIN smr_trn_tsalesorderdtl d ON d.salesorder_gid = a.salesorder_gid " +

                     " LEFT JOIN acp_mst_ttax e ON e.taxsegment_gid = d.taxsegment_gid " +

                     " LEFT JOIN pmr_mst_tproductgroup i ON i.productgroup_gid = d.productgroup_gid " +

                    " WHERE a.salesorder_gid = '" + salesorder_gid + "' order by d.salesorderdtl_gid ";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                var getModuleList = new List<postsalesorderdetails_list>();

                var getGetTaxSegmentList = new List<GetTaxSegmentListorder>();

                if (dt_datatable.Rows.Count != 0)

                {
                    int i = 0;

                    foreach (DataRow dt in  dt_datatable.Rows)

                    {

                        getModuleList.Add(new postsalesorderdetails_list

                        {

                            product_code = dt["product_code"].ToString(),

                            product_gid = dt["product_gid"].ToString(),

                            product_name = dt["product_name"].ToString(),

                            productgroup_name = dt["productgroup_name"].ToString(),

                            product_remarks = dt["product_remarks"].ToString(),

                            uom_name = dt["uom_name"].ToString(),

                            tax_prefix = dt["tax_prefix"].ToString(),

                            qty_quoted = dt["qty_quoted"].ToString(),

                            price = dt["price"].ToString(),

                            product_price = dt["product_price"].ToString(),

                            margin_percentage = dt["margin_percentage"].ToString(),

                            tax_name = dt["tax_name"].ToString(),

                            tax_amount = dt["tax_amount"].ToString(),

                            margin_amount = dt["margin_amount"].ToString(),

                            discount_amount = dt["discount_amount"].ToString(),

                            discount_percentage = dt["discount_percentage"].ToString(),

                        });



                        // Query tax segment details based on product_gid


                        values.postsalesorderdetails_list = getModuleList;
                        msSQL = " insert into smr_tmp_tsalesorderdtl (" +
                               " salesorderdtl_gid ," +
                               " salesorder_gid," +
                               " product_gid ," +
                               " product_name," +
                               " product_code," +
                               " product_price," +
                               " qty_quoted," +
                               " discount_percentage," +
                               " discount_amount," +
                               " tax_amount ," +
                               " uom_gid," +
                               " uom_name," +
                               " price," +
                               " tax_name," +
                               " tax1_gid," +
                               " slno," +
                               " tax_percentage," +
                               " taxsegment_gid," +
                               " taxsegmenttax_gid," +
                               " type " +
                               ")values(" +
                               " '" + mssalesorderGID1 + "'," +
                               " '" + mssalesorderGID + "'," +
                               " '" + dt["product_gid"].ToString() + "'," +
                               " '" + dt["product_name"].ToString() + "'," +
                               " '" + dt["product_code"].ToString() + "'," +
                               " '" + dt["product_price"].ToString() + "'," +
                               " '" + dt["qty_quoted"].ToString() + "'," +
                               " '" + dt["discount_percentage"].ToString() + "'," +
                               " '" + dt["discount_amount"].ToString() + "'," +
                               " '" + dt["tax_amount"].ToString() + "'," +
                               " '" + dt["uom_gid"].ToString() + "'," +
                               " '" + dt["uom_name"].ToString() + "'," +
                               " '" + dt["price"].ToString() + "'," +
                               " '" + dt["tax_name"].ToString() + "'," +
                               " '" + dt["tax1_gid"].ToString() + "'," +
                               " '" + i + 1 + "'," +
                               " '" + dt["tax_percentage"].ToString() + "'," +
                               " '" + dt["taxsegment_gid"].ToString() + "'," +
                               " '" + dt["taxsegmenttax_gid"].ToString() + "'," +
                               " '" + dt["order_type"].ToString() + "')";
                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                        if (mnResult == 0)
                        {
                            values.status = false;
                            values.message = "Error occurred while Insertion";
                            return;
                        }
                    }


                }
            }











            catch (Exception ex)

            {

                values.message = "Exception occured while loading Sales Order Summary !";

                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +

                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +

                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }

        }
        ////----------------------------------ordertoinvoice new design asset-sm---------------------------------------------///
        public void DaGetOrderToinvoiceDetailsSummary(string salesorder_gid, MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = " select a.salesorder_gid as serviceorder_gid,d.currencyexchange_gid,b.customer_id," +
                "a.branch_gid, f.branch_name,a.payment_days,a.delivery_days, concat(a.so_referenceno1,case when so_referencenumber='' then '' else concat(' ','-',' '," +
                " case when so_referencenumber is not null then so_referenceno1 else '' end) end )as so_reference," +
                " DATE_format(a.salesorder_date, '%d-%m-%Y') as serviceorder_date, " +
                " concat(a.customer_name,'/',c.email) as customer_name,b.customer_gid,format(a.grandtotal, 2) as grand_total,a.shipping_to," +
                " a.customer_contact_person as customercontact_name,c.email as email,b.customer_code," +
                " a.termsandconditions,c.mobile,b.gst_number," +
                " a.addon_charge as addon_amount ,a.additional_discount as discount_amount," +
                " a.customer_address,format(total_amount,2) as order_total ," +
                " a.freight_charges as freight_charges," +
                " format(a.buyback_charges,2)as buyback_charges," +
                " format(a.packing_charges,2)as packing_charges," +
                " format(a.insurance_charges,2)as insurance_charges," +
                "a.tax_name,a.tax_gid,a.roundoff,a.tax_amount,a.tax_name4," +
                " a.currency_code, a.currency_gid, a.exchange_rate, c.address1,c.address2,c.city," +
                " c.state,c.country_gid,c.zip_code,e.country_name " +
                " from smr_trn_tsalesorder a" +
                " left join crm_mst_tcustomer b on a.customer_gid=b.customer_gid " +
                " left join crm_mst_tcustomercontact c on b.customer_gid = c.customer_gid " +
                " left join crm_trn_tcurrencyexchange d on d.currency_code=a.currency_code" +
                " left join adm_mst_tcountry e on c.country_gid=e.country_gid " +
                " left join hrm_mst_tbranch f on f.branch_gid=a.branch_gid" +
                " where a.salesorder_gid='" + salesorder_gid + "'";
                var Getordertoinvoice = new List<Getordertoinvoice_list>();
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                if (dt_datatable.Rows.Count > 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        Getordertoinvoice.Add(new Getordertoinvoice_list
                        {
                            salesorder_gid = dt["serviceorder_gid"].ToString(),
                            so_reference = dt["so_reference"].ToString(),
                            serviceorder_date = dt["serviceorder_date"].ToString(),
                            customer_name = dt["customer_name"].ToString(),
                            customer_gid = dt["customer_gid"].ToString(),
                            grand_total = dt["grand_total"].ToString(),
                            customercontact_name = dt["customercontact_name"].ToString(),
                            email = dt["email"].ToString(),
                            customer_code = dt["customer_code"].ToString(),
                            termsandconditions = dt["termsandconditions"].ToString(),
                            customer_mobile = dt["mobile"].ToString(),
                            addon_amount = dt["addon_amount"].ToString(),
                            discount_amount = dt["discount_amount"].ToString(),
                            shipping_to = dt["shipping_to"].ToString(),
                            customer_address = dt["customer_address"].ToString(),
                            order_total = dt["order_total"].ToString(),
                            freight_charges = dt["freight_charges"].ToString(),
                            buyback_charges = dt["buyback_charges"].ToString(),
                            packing_charges = dt["packing_charges"].ToString(),
                            insurance_charges = dt["insurance_charges"].ToString(),
                            currencyexchange_gid = dt["currencyexchange_gid"].ToString(),
                            customer_id = dt["customer_id"].ToString(),
                            currency_code = dt["currency_code"].ToString(),
                            currency_gid = dt["currency_gid"].ToString(),
                            exchange_rate = dt["exchange_rate"].ToString(),
                            country_name = dt["country_name"].ToString(),
                            zip_code = dt["zip_code"].ToString(),
                            country_gid = dt["country_gid"].ToString(),
                            state = dt["state"].ToString(),
                            city = dt["city"].ToString(),
                            address1 = dt["address1"].ToString(),
                            address2 = dt["address2"].ToString(),
                            branch_gid = dt["branch_gid"].ToString(),
                            branch_name = dt["branch_name"].ToString(),
                            delivery_days = dt["delivery_days"].ToString(),
                            payment_days = dt["payment_days"].ToString(),
                            gst_number = dt["gst_number"].ToString(),
                            tax_name = dt["tax_name"].ToString(),
                            tax_gid = dt["tax_gid"].ToString(),
                            roundoff = dt["roundoff"].ToString(),
                            tax_amount = dt["tax_amount"].ToString(),
                            tax_name4 = dt["tax_name4"].ToString(),
                        });
                        values.Getordertoinvoice_list = Getordertoinvoice;
                    }
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occurred while Order to invoice summary!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                  $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" +
                  ex.Message.ToString() + "***********" + values.message.ToString() + "*****Query****" +
                  msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" +
                  DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
        }

        public void DaGetOrderToInvoiceProductSummary(string employee_gid, string salesorder_gid, MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = " delete from rbl_tmp_tinvoicedtl where employee_gid='" + employee_gid + "'";
                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                msSQL = " select a.salesorderdtl_gid as serviceorderdtl_gid,a.salesorder_gid," +
                " if(a.customerproduct_code='&nbsp;',' ',a.customerproduct_code) as customerproduct_code," +
                " a.product_gid,a.tax1_gid,a.tax2_gid,a.tax3_gid,format(a.qty_quoted,2) as qty_quoted,a.uom_gid,a.uom_name," +
                " format(a.vendor_price,2) as amount ,format(a.product_price,2) as productprice,a.price," +
                " format(a.tax_amount,2) as tax_amount1, " +
                " format(a.tax_amount2,2) as tax_amount2,format(a.tax_amount3,2) as tax_amount3," +
                " format(a.price,2) as total_amount,c.productgroup_gid, c.productgroup_name,a.product_code, " +
                " a.display_field as description,a.product_remarks as description1, a.tax_name as tax_name1,a.tax_name2,a.tax_name3, " +
                " format(a.discount_amount,2) as discount_amount,a.margin_percentage, " +
                " format(a.discount_percentage,2) as discount_percentage,a.product_name, a.taxsegment_gid, " +
                "a.tax_percentage,a.tax_percentage2,a.tax_percentage3 " +
                " from smr_trn_tsalesorderdtl a " +
                " left join pmr_mst_tproductgroup c on c.productgroup_gid=a.productgroup_gid" +
                " where a.salesorder_gid='" + salesorder_gid + "' " +
                " group by a.product_gid,a.salesorderdtl_gid order by a.salesorderdtl_gid asc ";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                if (dt_datatable.Rows.Count > 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        double lsvendor_price = double.Parse(dt["productprice"].ToString().Replace(",", ""));
                        string invoicedtl_gid = objcmnfunctions.GetMasterGID("SIVC");

                        string display_field = dt["description"].ToString();

                        msSQL = " insert into rbl_tmp_tinvoicedtl ( " +
                        " invoicedtl_gid, invoice_gid, product_gid," +
                        " qty_invoice,product_price, discount_percentage," +
                        " discount_amount, tax_amount, product_total," +
                        " uom_gid, uom_name, tax_amount2, tax_amount3," +
                        " tax_name, tax_name2,tax_name3,display_field," +
                        " tax1_gid, tax2_gid, tax3_gid, product_name," +
                        " productgroup_gid," +
                        " productgroup_name, " +
                        " employee_gid, " +
                        " selling_price, " +
                        " product_code," +
                        " customerproduct_code, " +
                        " tax_percentage," +
                        " tax_percentage2," +
                        " tax_percentage3 ," +
                        " vendor_price, " +
                        " product_remarks " +
                        ") values (" +
                        "'" + invoicedtl_gid + "'," +
                        "'" + dt["salesorder_gid"] + "'," +
                        "'" + dt["product_gid"] + "'," +
                        "'" + dt["qty_quoted"] + "'," +
                        "'" + dt["productprice"].ToString().Replace(",", "") + "'," +
                        "'" + dt["discount_percentage"] + "'," +
                        "'" + dt["discount_amount"].ToString().Replace(",", "") + "'," +
                        "'" + dt["tax_amount1"].ToString().Replace(",", "") + "'," +
                        "'" + dt["total_amount"].ToString().Replace(",", "") + "'," +
                        "'" + dt["uom_gid"] + "'," +
                        "'" + dt["uom_name"] + "'," +
                        "'" + dt["tax_amount2"].ToString().Replace(",", "") + "'," +
                        "'" + dt["tax_amount3"].ToString().Replace(",", "") + "'," +
                        "'" + dt["tax_name1"].ToString() + "'," +
                        "'" + dt["tax_name2"].ToString() + "'," +
                        "'" + dt["tax_name3"].ToString() + "',";
                        if (display_field != null)
                        {
                            msSQL += "'" + display_field.Replace("'", "\\\'") + "',";
                        }
                        else
                        {
                            msSQL += "'" + display_field.Replace("'", "\\\'") + "',";
                        }

                        msSQL += "'" + dt["tax1_gid"].ToString() + "'," +
                           "'" + dt["tax2_gid"].ToString() + "'," +
                           "'" + dt["tax3_gid"].ToString() + "'," +
                           "'" + dt["product_name"].ToString() + "'," +
                           "'" + dt["productgroup_gid"].ToString() + "'," +
                           "'" + dt["productgroup_name"].ToString() + "'," +
                           "'" + employee_gid + "'," +
                           "'" + dt["productprice"].ToString().Replace(",", "") + "'," +
                           "'" + dt["product_code"] + "'," +
                           "'" + dt["customerproduct_code"] + "'," +
                           "'" + dt["tax_percentage"] + "'," +
                           "'" + dt["tax_percentage2"] + "'," +
                           "'" + dt["tax_percentage3"] + "'," +
                           "'" + lsvendor_price + "',";
                        if (display_field != null)
                        {
                            msSQL += "'" + display_field.Replace("'", "\\\'") + "')";
                        }
                        else
                        {
                            msSQL += "'" + display_field.Replace("'", "\\\'") + "')";
                        }
                        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    }
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occurred while Order to invoice product summary!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                  $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" +
                  ex.Message.ToString() + "***********" + values.message.ToString() + "*****Query****" +
                  msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" +
                  DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
        }

        public void DaGetOrderToInvoiceProductDetails(string employee_gid, string salesorder_gid, MdlSmrTrnSalesorder values)
        {
            try
            {


                double grand_total = 0.00;
                double grandtotal = 0.00;

                msSQL = "SELECT  a.invoicedtl_gid,a.invoice_gid, a.product_gid, a.product_name," +
                    "a.customerproduct_code, a.product_remarks,a.product_code, b.productgroup_gid," +
                    " FORMAT(a.qty_invoice, 2) AS qty_ordered, c.productgroup_name, a.uom_name, b.producttype_gid," +
                    " a.created_by, a.product_price, FORMAT(a.product_price, 2) AS producttotal_price," +
                    " a.discount_percentage,format(a.product_total,2) as product_total, FORMAT(a.discount_amount, 2) AS discount_amount," +
                    " a.tax_percentage, FORMAT(a.tax_amount, 2) AS tax_amount, a.uom_gid, a.display_field," +
                    " a.tax_name, a.tax_name2, a.tax_name3, a.tax_percentage2,a.tax_percentage3," +
                    " FORMAT(a.tax_amount2, 2) AS tax_amount2, FORMAT(a.tax_amount3, 2) AS tax_amount3, a.tax1_gid," +
                    " a.tax2_gid, a.tax3_gid ," +
                    " concat( case when a.tax_name is null then '' else a.tax_name end, ' '," +
                    " case when a.tax_percentage = '0' then '' else concat('',a.tax_percentage,'%') end ," +
                    " case when a.tax_amount = '0' then '' else concat(':',a.tax_amount) end) as tax," +
                    " concat(case when a.tax_name2 is null then '' else a.tax_name2 end, ' ', " +
                    " case when a.tax_percentage2 = '0' then '' else concat('', a.tax_percentage2, '%') end, " +
                    " case when a.tax_amount2 = '0' then '' else concat(':', a.tax_amount2) end) as tax2," +
                    " concat(case when a.tax_name3 is null then '' else a.tax_name3 end, ' '," +
                    " case when a.tax_percentage3 = '0' then '' else concat('', a.tax_percentage3, ' %   ')" +
                    " end, case when a.tax_amount3 = '0' then '  ' else concat(' : ', a.tax_amount3) end) as tax3," +
                    "format(a.tax_amount + a.tax_amount2 + a.tax_amount3, 2) as taxamount,a.taxsegment_gid " +
                    " FROM rbl_tmp_tinvoicedtl a  LEFT JOIN pmr_mst_tproduct b ON a.product_gid = b.product_gid " +
                    " LEFT JOIN pmr_mst_tproductgroup c ON b.productgroup_gid = c.productgroup_gid" +
                    " LEFT JOIN pmr_mst_tproductuom d ON a.uom_gid = d.productuom_gid  " +
                    " WHERE a.employee_gid = '" + employee_gid + "' and a.invoice_gid='" + salesorder_gid + "'";
                var GetOrderToInvoiceProduct = new List<GetOrderToInvoiceProduct_list>();
                dt_datatable = objdbconn.GetDataTable(msSQL);
                if (dt_datatable.Rows.Count > 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
                    {
                        grand_total += double.Parse(dt["product_total"].ToString());
                        grandtotal += double.Parse(dt["product_total"].ToString());
                        GetOrderToInvoiceProduct.Add(new GetOrderToInvoiceProduct_list
                        {
                            invoice_gid = dt["invoice_gid"].ToString(),
                            invoicedtl_gid = dt["invoicedtl_gid"].ToString(),
                            customerproduct_code = dt["customerproduct_code"].ToString(),
                            product_gid = dt["product_gid"].ToString(),
                            tax1_gid = dt["tax1_gid"].ToString(),
                            tax2_gid = dt["tax2_gid"].ToString(),
                            tax3_gid = dt["tax3_gid"].ToString(),
                            qty_quoted = dt["qty_ordered"].ToString(),
                            uom_gid = dt["uom_gid"].ToString(),
                            product_total = dt["product_total"].ToString(),
                            productprice = dt["product_price"].ToString(),
                            tax_amount1 = dt["tax_amount"].ToString(),
                            tax_amount2 = dt["tax_amount2"].ToString(),
                            tax_amount3 = dt["tax_amount3"].ToString(),
                            productgroup_gid = dt["productgroup_gid"].ToString(),
                            productgroup_name = dt["productgroup_name"].ToString(),
                            product_code = dt["product_code"].ToString(),
                            product_remarks = dt["product_remarks"].ToString(),
                            tax_name1 = dt["tax_name"].ToString(),
                            tax_name2 = dt["tax_name2"].ToString(),
                            tax_name3 = dt["tax_name3"].ToString(),
                            discount_amount = dt["discount_amount"].ToString(),
                            discount_percentage = dt["discount_percentage"].ToString(),
                            product_name = dt["product_name"].ToString(),
                            taxsegment_gid = dt["taxsegment_gid"].ToString(),
                            tax_percentage = dt["tax_percentage"].ToString(),
                            tax_percentage2 = dt["tax_percentage2"].ToString(),
                            tax_percentage3 = dt["tax_percentage3"].ToString(),
                            uom_name = dt["uom_name"].ToString(),
                            taxamount = dt["taxamount"].ToString(),
                        });
                        values.GetOrderToInvoiceProduct_list = GetOrderToInvoiceProduct;
                        values.grand_total = Math.Round(grand_total, 2);
                        values.grandtotal = Math.Round(grandtotal, 2);
                    }
                    dt_datatable.Dispose();
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occurred while Order to invoice summary!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                  $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" +
                  ex.Message.ToString() + "***********" + values.message.ToString() + "*****Query****" +
                  msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" +
                  DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
        }

        public void PostOrderToInvoiceProductAdd(string employee_gid, ordertoinvoiceproductsubmit_list values)
        {
            try
            {

                double discount_precentage = double.TryParse(values.discountprecentage.ToString(), out double discountprecentage) ? discountprecentage : 0;
                double discount_amount = double.TryParse(values.discount_amount, out double discountamount) ? discountamount : 0;
                msSQL = " SELECT a.productuom_gid, a.product_gid,a.customerproduct_code, a.product_name, b.productuom_name" +
                    " ,a.productgroup_gid FROM pmr_mst_tproduct a " +
                 " LEFT JOIN pmr_mst_tproductuom b ON a.productuom_gid = b.productuom_gid " +
                 " WHERE a.product_gid = '" + values.product_name + "'";
                objodbcDataReader = objdbconn.GetDataReader(msSQL);
                if (objodbcDataReader.HasRows == true)
                {

                    lsproductgid = objodbcDataReader["product_gid"].ToString();
                    lsproductuom_gid = objodbcDataReader["productuom_gid"].ToString();
                    lsproduct_name = objodbcDataReader["product_name"].ToString();
                    lsproductuom_name = objodbcDataReader["productuom_name"].ToString();
                    lscustomerproduct_code = objodbcDataReader["customerproduct_code"].ToString();

                }
                string invoicedtl_gid = objcmnfunctions.GetMasterGID("SIVC");
                string invoice_gid = objcmnfunctions.GetMasterGID("INV");
                if (values.productquantity == "undefined" || values.productquantity == null || Convert.ToInt32(values.productquantity) < 1 || values.productquantity == "")
                {

                    values.status = false;
                    values.message = "Product quantity cannot be zero or empty.";
                    return;
                }
                else if (values.unitprice == null || values.unitprice == "" || values.unitprice == "undefined")
                {
                    values.status = false;
                    values.message = "Price cannot be left empty";
                    return;
                }
                else
                {
                    msSQL = " insert into rbl_tmp_tinvoicedtl ( " +
                        " invoicedtl_gid, invoice_gid, product_gid," +
                        " qty_invoice,product_price, discount_percentage," +
                        " discount_amount, tax_amount, product_total," +
                        " uom_gid, uom_name, tax_amount2, tax_amount3," +
                        " tax_name, tax_name2,tax_name3,display_field," +
                        " tax1_gid, tax2_gid, tax3_gid, product_name," +
                        " productgroup_gid," +
                        " productgroup_name, " +
                        " employee_gid, " +
                        " selling_price, " +
                        " product_code," +
                        " customerproduct_code, " +
                        " tax_percentage," +
                        " tax_percentage2," +
                        " tax_percentage3 ," +
                        " vendor_price, " +
                        " product_remarks " +
                        " ) values ( " +
                        "'" + invoicedtl_gid + "'," +
                        "'" + values.salesorder_gid + "'," +
                        "'" + lsproductgid + "'," +
                        "'" + values.productquantity + "'," +
                        "'" + values.unitprice + "'," +
                        "'" + discount_precentage + "'," +
                        "'" + discount_amount + "'," +
                        "'" + values.taxamount1 + "'," +
                        "'" + values.producttotal_amount + "'," +
                        "'" + lsproductuom_gid + "'," +
                        "'" + lsproductuom_name + "'," +
                        "'" + values.taxamount2 + "'," +
                        "'" + values.taxamount3 + "'," +
                        "'" + values.tax_prefix + "'," +
                        "'" + values.tax_prefix2 + "'," +
                        "'" + values.tax_prefix3 + "'," +
                        "'" + lsproduct_name + "'," +
                        "'" + values.taxgid1 + "'," +
                        "'" + values.taxgid2 + "'," +
                        "'" + values.taxgid3 + "'," +
                        "'" + lsproduct_name + "'," +
                        "'" + values.productgroup_gid + "'," +
                        "'" + values.productgroup_name + "'," +
                        "'" + employee_gid + "'," +
                        "'" + values.unitprice + "'," +
                        "'" + values.product_code + "'," +
                        "'" + lscustomerproduct_code + "'," +
                        "'" + values.taxprecentage1 + "'," +
                        "'" + values.taxprecentage2 + "'," +
                        "'" + values.taxprecentage3 + "'," +
                        "'" + values.unitprice + "',";
                    if (values.product_desc != null)
                    {
                        msSQL += "'" + values.product_desc.Replace("'", "\\\'") + "')";
                    }
                    else
                    {
                        msSQL += "'" + values.product_desc + "')";
                    }


                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                    if (mnResult != 0)
                    {
                        values.status = true;
                        values.message = "Product Added Successfully";
                    }
                    else
                    {
                        values.status = false;
                        values.message = "Error While Adding Product";
                    }
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occurred while changing product!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "***********" +
                    $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" +
                    ex.Message.ToString() + "***********" + values.message.ToString() + "*****Query****" +
                    msSQL + "*******Apiref********", "ErrorLog/Purchase/" + "Log" +
                    DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
        }
        public void PostOnsubmitOrdertoInvoice(string employee_gid, string user_gid, ordertoinvoicesubmit values)
        {
            try
            {
                double addonCharges = double.TryParse(values.addon_charge, out double addonChargesValue) ? addonChargesValue : 0;
                double freightCharges = double.TryParse(values.freight_charges, out double freightChargesValue) ? freightChargesValue : 0;
                double forwardingCharges = double.TryParse(values.forwardingCharges, out double packingChargesValue) ? packingChargesValue : 0;
                double insuranceCharges = double.TryParse(values.insurance_charges, out double insuranceChargesValue) ? insuranceChargesValue : 0;
                double roundoff = double.TryParse(values.roundoff, out double roundoffValue) ? roundoffValue : 0;
                double additionaldiscountAmount = double.TryParse(values.additional_discount, out double discountAmountValue) ? discountAmountValue : 0;
                double buybackCharges = double.TryParse(values.buybackcharges, out double buybackChargesValue) ? buybackChargesValue : 0;

                string uiDateStr = values.invoice_date;
                DateTime uiDate = DateTime.ParseExact(uiDateStr, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string mysqlinvoiceDate = uiDate.ToString("yyyy-MM-dd");

                string uiDateStr2 = values.due_date;
                DateTime uiDate2 = DateTime.ParseExact(uiDateStr2, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string mysqlpaymentDate = uiDate2.ToString("yyyy-MM-dd");
                double lsexchange = double.Parse(values.exchange_rate);
                double lstotalamount_l = Math.Round((double.Parse(values.producttotalamount) * lsexchange), 2);
                double lsgrandtotal_l = Math.Round((double.Parse(values.grandtotal) * lsexchange), 2);
                double lsaddoncharges_l = Math.Round((double.Parse(values.addon_charge) * lsexchange), 2);
                double lsadditionaldiscountAmount_l = Math.Round((additionaldiscountAmount * lsexchange), 2);


                //string uiDateStr3 = values.delivery_days;
                //DateTime uiDate3 = DateTime.ParseExact(uiDateStr3, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                //string mysqldeliveryDate = uiDate3.ToString("yyyy-MM-dd");
                string ls_referenceno = "";

                msINGetGID = objcmnfunctions.GetMasterGID("SIVT");
                msSQL = "select company_code from adm_mst_tcompany";
                string lscompanycode = objdbconn.GetExecuteScalar(msSQL);
                if (lscompanycode == "BOBA")
                {
                    msSQL = "select  so_referencenumber from smr_trn_tsalesorder where salesorder_gid='" + values.salesorder_gid + "'";
                    ls_referenceno = objdbconn.GetExecuteScalar(msSQL);

                    if (ls_referenceno == "" || ls_referenceno == null)
                    {
                        ls_referenceno = objcmnfunctions.getSequencecustomizerGID("INV", "RBL", values.branch_gid);

                    }

                }
                else
                {


                    ls_referenceno = objcmnfunctions.getSequencecustomizerGID("INV", "RBL", values.branch_gid);
                }


                msSQL = "select customer_name, customer_code, customer_id from crm_mst_tcustomer where customer_gid='" + values.customer_gid + "'";
                objodbcDataReader2 = objdbconn.GetDataReader(msSQL);
                if (objodbcDataReader2.HasRows == true)
                {
                    objodbcDataReader2.Read();
                    lsCustomername = objodbcDataReader2["customer_name"].ToString();
                    lscustomer_code = objodbcDataReader2["customer_code"].ToString();
                    lscustomer_id = objodbcDataReader2["customer_id"].ToString();
                }
                msSQL = "Select finance_flag from adm_mst_Tcompany";
                string lsfinanceflag = objdbconn.GetExecuteScalar(msSQL);
                if (lsfinanceflag == "Y")
                {

                    msSQL = "SELECT account_gid FROM crm_mst_tcustomer WHERE customer_gid='" + values.customer_gid + "'";
                    objodbcDataReader2 = objdbconn.GetDataReader(msSQL);

                    if (objodbcDataReader2.HasRows)
                    {
                        while (objodbcDataReader2.Read())
                        {
                            string lsaccount_gid = objodbcDataReader2["account_gid"]?.ToString(); // Safely get the value

                            // Check if lsaccount_gid is null or empty
                            if (string.IsNullOrEmpty(lsaccount_gid))
                            {
                                //objfincmn.finance_vendor_debitor("Sales", lscustomer_id, lsCustomername, values.customer_gid, employee_gid);
                                //string trace_comment = "Added a customer on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                //objcmnfunctions.Tracelog(msGetGid, employee_gid, trace_comment, "added_customer");
                            }
                        }
                    }

                    objodbcDataReader2.Close();
                }


                msSQL = " select customercontact_name, email, mobile, concat(address1,' ', address2) as address from crm_mst_tcustomercontact" +
                    " where customer_gid='" + values.customer_gid + "'";
                objodbcDataReader = objdbconn.GetDataReader(msSQL);
                if (objodbcDataReader.HasRows == true)
                {

                    values.customercontactperson = objodbcDataReader["customercontact_name"].ToString();
                    values.customercontactnumber = objodbcDataReader["mobile"].ToString();
                    values.customeraddress = objodbcDataReader["address"].ToString();
                    values.customeremailaddress = objodbcDataReader["email"].ToString();

                }

                msSQL = "select currencyexchange_gid from crm_trn_tcurrencyexchange  where currency_code='" + values.currency_code + "'";
                string lscurrencycodegid = objdbconn.GetExecuteScalar(msSQL);
                msSQL = "select tax_gid from acp_mst_ttax  where tax_name='" + values.tax_name4 + "'";
                string lstaxgid = objdbconn.GetExecuteScalar(msSQL);
                msSQL = "select tax_percentage from acp_mst_ttax  where tax_gid='" + lstaxgid + "'";
                string lstaxpercentage = objdbconn.GetExecuteScalar(msSQL);



                msSQL = "SELECT  a.invoicedtl_gid, a.product_gid, a.customerproduct_code,a.product_name, a.product_remarks,a.product_code, b.productgroup_gid," +
                " format(a.qty_invoice, 2) AS qty_ordered_1, a.qty_invoice as qty_ordered, c.productgroup_name, a.uom_name, b.producttype_gid," +
                " a.created_by, a.product_price, format(a.product_price, 2) AS producttotal_price," +
                " a.discount_percentage,a.product_total,a.discount_amount, format(a.discount_amount, 2) AS discount_amount_1," +
                " a.tax_percentage, format(a.tax_amount, 2) AS tax_amount, a.uom_gid, a.display_field," +
                " a.tax_name, a.tax_name2, a.tax_name3, a.tax_percentage2,a.tax_percentage3," +
                " format(a.tax_amount2, 2) AS tax_amount2, format(a.tax_amount3, 2) AS tax_amount3, a.tax1_gid," +
                " a.tax2_gid, a.tax3_gid ," +
                " concat( case when a.tax_name is null then '' else a.tax_name end, ' '," +
                " case when a.tax_percentage = '0' then '' else concat('',a.tax_percentage,'%') end ," +
                " case when a.tax_amount = '0' then '' else concat(':',a.tax_amount) end) as tax," +
                " concat(case when a.tax_name2 is null then '' else a.tax_name2 end, ' ', " +
                " case when a.tax_percentage2 = '0' then '' else concat('', a.tax_percentage2, '%') end, " +
                " case when a.tax_amount2 = '0' then '' else concat(':', a.tax_amount2) end) as tax2," +
                " concat(case when a.tax_name3 is null then '' else a.tax_name3 end, ' '," +
                " case when a.tax_percentage3 = '0' then '' else concat('', a.tax_percentage3, ' %   ')" +
                " end, case when a.tax_amount3 = '0' then '  ' else concat(' : ', a.tax_amount3) end) as tax3," +
                " format(a.tax_amount + a.tax_amount2 + a.tax_amount3, 2) as taxamount,a.taxsegment_gid,vendor_price " +
                " FROM rbl_tmp_tinvoicedtl a  " +
                "LEFT JOIN pmr_mst_tproduct b ON a.product_gid = b.product_gid " +
                " LEFT JOIN pmr_mst_tproductgroup c ON b.productgroup_gid = c.productgroup_gid" +
                " LEFT JOIN pmr_mst_tproductuom d ON a.uom_gid = d.productuom_gid  " +
                " WHERE a.employee_gid = '" + employee_gid + "'";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                foreach (DataRow dt in  dt_datatable.Rows)
                {
                    double lsvendorprice = Math.Round(double.Parse(dt["vendor_price"].ToString().Replace(",", "")), 2);
                    double lsproduct_discount = Math.Round(double.Parse(dt["discount_amount"].ToString().Replace(",", "")), 2);

                    double lsproduct_price = Math.Round((double.Parse(dt["qty_ordered"].ToString()) * lsvendorprice) - lsproduct_discount, 2);
                    double lsexchangerate = double.Parse(values.exchange_rate);
                    double lsproductprice_l = Math.Round((lsproduct_price * lsexchangerate), 2);
                    double lstaxamount_l = Math.Round((double.Parse(dt["tax_amount"].ToString().Replace(",", "")) * lsexchangerate), 2);
                    double lstaxamount2_l = Math.Round((double.Parse(dt["tax_amount2"].ToString().Replace(",", "")) * lsexchangerate), 2);
                    double lstaxamount3_l = Math.Round((double.Parse(dt["tax_amount3"].ToString().Replace(",", "")) * lsexchangerate), 2);
                    double lsdiscountamount_l = Math.Round((double.Parse(dt["discount_amount"].ToString().Replace(",", "")) * lsexchangerate), 2);
                    double lsproducttotal_l = Math.Round((double.Parse(dt["product_total"].ToString().Replace(",", "")) * lsexchangerate), 2);
                    msGetGid = objcmnfunctions.GetMasterGID("SIVC");

                    string display_field = dt["product_remarks"].ToString();

                    msSQL = " insert into rbl_trn_tinvoicedtl (" +
                        " invoicedtl_gid, " +
                        " invoice_gid, " +
                        " invoice_reference, " +
                        " product_gid, " +
                        " product_code, " +
                        " productgroup_gid, " +
                        " product_name, " +
                        " uom_gid, " +
                        " productuom_name, " +
                        " product_price, " +
                        " discount_percentage, " +
                        " discount_amount, " +
                        " tax_name, " +
                        " tax_name2, " +
                        " tax_name3, " +
                        " tax1_gid," +
                        " tax2_gid," +
                        " tax3_gid," +
                        " tax_percentage, " +
                        " tax_percentage2, " +
                        " tax_percentage3, " +
                        " tax_amount, " +
                        " tax_amount2, " +
                        " tax_amount3, " +
                        " qty_invoice, " +
                        " product_remarks, " +

                        " product_total, " +
                        " product_price_L, " +
                        " discount_amount_L, " +
                        " tax_amount1_L, " +
                        " tax_amount2_L, " +
                        " tax_amount3_L, " +
                        " product_total_L, " +
                        " display_field, " +
                        " customerproduct_code," +
                        " taxsegment_gid," +
                        " vendor_price," +
                        " created_by," +
                        " created_date " +
                            " ) values ( " +
                            "'" + msGetGid + "'," +
                            "'" + msINGetGID + "'," +
                            "'" + values.salesorder_gid + "'," +
                            "'" + dt["product_gid"].ToString() + "'," +
                            "'" + dt["product_code"].ToString() + "'," +
                             "'" + dt["productgroup_gid"].ToString() + "'," +
                            "'" + dt["product_name"].ToString() + "'," +
                            "'" + dt["uom_gid"].ToString() + "'," +
                            "'" + dt["uom_name"].ToString() + "'," +
                            "'" + dt["product_price"].ToString() + "'," +
                            "'" + dt["discount_percentage"].ToString() + "'," +
                            "'" + dt["discount_amount"].ToString().Replace(",", "") + "'," +
                            "'" + dt["tax_name"].ToString() + "'," +
                            "'" + dt["tax_name2"].ToString() + "'," +
                            "'" + dt["tax_name3"].ToString() + "'," +
                            "'" + dt["tax1_gid"].ToString() + "'," +
                            "'" + dt["tax2_gid"].ToString() + "'," +
                            "'" + dt["tax3_gid"].ToString() + "'," +
                            "'" + dt["tax_percentage"].ToString() + "'," +
                            "'" + dt["tax_percentage2"].ToString() + "'," +
                            "'" + dt["tax_percentage3"].ToString() + "'," +
                            "'" + dt["tax_amount"].ToString().Replace(",", "") + "'," +
                            "'" + dt["tax_amount2"].ToString().Replace(",", "") + "'," +
                            "'" + dt["tax_amount3"].ToString().Replace(",", "") + "'," +
                            "'" + dt["qty_ordered"].ToString() + "',";
                    if (display_field != null)
                    {
                        msSQL += "'" + display_field.Replace("'", "\\\'") + "',";
                    }
                    else
                    {
                        msSQL += "'" + display_field + "',";
                    }


                    msSQL += "'" + dt["product_total"].ToString().Replace(",", "") + "'," +
                       "'" + lsproductprice_l + "'," +
                       "'" + lsdiscountamount_l + "'," +
                       "'" + lstaxamount_l + "'," +
                       "'" + lstaxamount2_l + "'," +
                       "'" + lstaxamount3_l + "'," +
                       "'" + lsproducttotal_l + "',";
                    if (display_field != null)
                    {
                        msSQL += "'" + display_field.Replace("'", "\\\'") + "',";
                    }
                    else
                    {
                        msSQL += "'" + display_field + "',";
                    }

                    msSQL += "'" + dt["product_code"].ToString() + "'," +
                              "'" + dt["taxsegment_gid"].ToString() + "'," +
                              "'" + dt["vendor_price"].ToString() + "'," +
                              "'" + employee_gid + "'," +
                              "'" + DateTime.Now.ToString("yyyy-MM-dd") + "')"
                              ;
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    msSQL = " update smr_Trn_Tsalesorderdtl set qty_executed='" + dt["qty_ordered"].ToString() + "' where salesorder_gid='" + values.salesorder_gid + "' and product_gid='" + dt["product_gid"].ToString() + "'";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                }
                string lstype = string.Empty;
                msSQL = "select distinct type from smr_trn_tsalesorderdtl where salesorder_gid='" + values.salesorder_gid + "' ";
                string lstype_1 = objdbconn.GetExecuteScalar(msSQL);
                if (lstype == "")
                {
                    lstype = lstype_1;
                }
                else
                {
                    lstype = "Both";
                }



                msSQL = "select so_type from smr_trn_tsalesorder a " +
               "where a.salesorder_gid='" + values.salesorder_gid + "'";
                string order_type = objdbconn.GetExecuteScalar(msSQL);
                msSQL = " insert into rbl_trn_tinvoice(" +
                       " invoice_gid," +
                       " invoice_date," +
                       " payment_term, " +
                       " payment_date," +
                       " invoice_from," +
                       " invoice_type," +
                       " customer_gid," +
                       " customer_name," +
                       " customer_contactperson," +
                       " customer_contactnumber," +
                       " customer_address," +
                       " customer_email," +
                       " mode_of_despatch," +
                       " total_amount," +
                       " invoice_amount," +
                       " invoice_refno," +
                       " invoice_status," +
                       " invoice_flag," +
                       " user_gid," +
                       " discount_amount," +
                       " additionalcharges_amount," +
                       " total_amount_L," +
                       " invoice_amount_L," +
                       " discount_amount_L," +
                       " additionalcharges_amount_L," +
                       " invoice_remarks," +
                       " termsandconditions," +
                       " currency_code," +
                       " exchange_rate," +
                       " branch_gid, " +
                       " roundoff," +
                       " tax_gid," +
                       " tax_name," +
                       " tax_percentage, " +
                       " tax_amount," +
                       " taxsegment_gid," +
                       " created_date," +
                       " freight_charges," +
                       " packing_charges," +
                       " delivery_date," +
                       " payment_days," +
                       " delivery_days," +
                       " insurance_charges, " +
                       " invoice_reference, " +
                       " shipping_to, " +
                       " sales_type, " +
                       " bill_email " +
                       " ) values (" +
                       " '" + msINGetGID + "'," +
                       "'" + mysqlinvoiceDate + "'," +
                       "'" + values.payment_days + "'," +
                       "'" + mysqlpaymentDate + "'," +
                       "'" + order_type + "'," +
                       "'" + order_type + "'," +
                       " '" + values.customer_gid + "'," +
                       " '" + lsCustomername.Replace("'", "\\\'") + "'," +
                       " '" + values.customercontactperson + "'," +
                       " '" + values.customercontactnumber + "'," +
                       " '" + values.customer_address.Replace("'", "\\\'") + "'," +
                       " '" + values.customeremailaddress + "'," +
                       " '" + values.dispatch_mode + "'," +
                       " '" + values.producttotalamount + "'," +
                       " '" + values.grandtotal + "'," +
                       " '" + ls_referenceno + "'," +
                       " 'Payment Pending'," +
                       " 'Invoice Approved'," +
                       " '" + employee_gid + "'," +
                       " '" + additionaldiscountAmount + "'," +
                       " '" + addonCharges + "'," +
                       "'" + lstotalamount_l + "'," +
                       "'" + lsgrandtotal_l + "'," +
                       "'" + lsadditionaldiscountAmount_l + "'," +
                       "'" + lsaddoncharges_l + "',";
                if (values.remarks != null)
                {
                    msSQL += "'" + values.remarks.Replace("'", "\\\'") + "',";
                }
                else
                {
                    msSQL += "'" + values.remarks + "',";
                }

                msSQL += "'" + values.termsandconditions.Trim().Replace("<br />", "<br>").Replace("'", "") + "', " +
                    "'" + values.currency_code + "'," +
                    "'" + values.exchange_rate + "'," +
                    "'" + values.branch_gid + "', " +
                    "'" + roundoff + "'," +
                    "'" + lstaxgid + "'," +
                    "'" + values.tax_name4 + "', " +
                    "'" + lstaxpercentage + "'," +
                    "'" + values.tax_amount4 + "'," +
                    "'" + values.taxsegment_gid + "'," +
                    "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                    "'" + freightCharges + "'," +
                    "'" + forwardingCharges + "'," +
                    "'" + values.delivery_days + "'," +
                    "'" + values.payment + "'," +
                    "'" + values.delivery_days + "'," +
                    "'" + insuranceCharges + "'," +
                    "'" + values.salesorder_gid + "'," +
                    "'" + values.shipping_to.Replace("'", "\\\'") + "'," +
                     "'" + values.sales_type + "'," +
                    "'" + values.bill_email + "')";
                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                if (mnResult == 1)
                {
                    msSQL = "update cst_trn_tsalesorder set invoice_gid='" + msINGetGID + "' where salesorder_gid='" + values.salesorder_gid + "'";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    msSQL = "update smr_trn_tsalesorder set salesorder_status='Invoice Raised' where salesorder_gid='" + values.salesorder_gid + "'";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);


                    msSQL = " update smr_trn_tsalesorder set so_type='" + order_type + "' where salesorder_gid='" + values.salesorder_gid + "'";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    msSQL = " update acp_trn_torder set so_type='" + order_type + "' where salesorder_gid='" + values.salesorder_gid + "'";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    msSQL = "select finance_flag from adm_mst_tcompany ";
                    string finance_flag = objdbconn.GetExecuteScalar(msSQL);
                    if (finance_flag == "Y")
                    {

                        double roundoff1 = roundoff * Convert.ToDouble(values.exchange_rate);
                        string lsproduct_price_l = "", lstax1_gid = "", lstax2_gid = "", lstax1="", lstax2="";
                        msSQL = " select sum(product_price_L) as product_price_L,sum(tax_amount1_L) as tax1,sum(tax_amount2_L) as tax2,tax1_gid,tax2_gid from rbl_trn_tinvoicedtl " +
                             " where invoice_gid='" + msINGetGID + "' ";
                        objodbcDataReader = objdbconn.GetDataReader(msSQL);
                        if (objodbcDataReader.HasRows == true)
                        {
                            lsproduct_price_l = objodbcDataReader["product_price_L"].ToString();
                            lstax1 = objodbcDataReader["tax1"].ToString();
                            lstax2 = objodbcDataReader["tax2"].ToString();
                            lstax1_gid = objodbcDataReader["tax1_gid"].ToString();
                            lstax2_gid = objodbcDataReader["tax2_gid"].ToString();
                        }
                        objodbcDataReader.Close();
                        lsbasic_amount = Math.Round(double.Parse(lsproduct_price_l), 2);
                        //objfincmn.jn_invoice(mysqlinvoiceDate, values.remarks, values.branch_gid, ls_referenceno, msINGetGID
                        // , lsbasic_amount, addonCharges, additionaldiscountAmount, lsgrandtotal_l, values.customer_gid, "Invoice", "RBL",
                        // values.sales_type, roundoff1, freightCharges, buybackCharges, packingChargesValue, insuranceCharges, values.tax_amount4, values.tax_name4);






                        if (lstax1 != "0.00" && lstax1 != "" &&lstax1!=null)
                        {
                            decimal lstaxsum = decimal.Parse(lstax1);
                            string lstaxamount = lstaxsum.ToString("F2");
                            double tax_amount = Math.Round(double.Parse(lstaxamount), 2);

                           // objfincmn.jn_sales_tax(msINGetGID, ls_referenceno, values.remarks, tax_amount, lstax1_gid);
                        }
                        if (lstax2 != "0.00" && lstax2 != "" && lstax2 != null && lstax2 != "0")
                        {
                            decimal lstaxsum = decimal.Parse(lstax2);
                            string lstaxamount = lstaxsum.ToString("F2");
                            double tax_amount = Math.Round(double.Parse(lstaxamount), 2);

                            //objfincmn.jn_sales_tax(msINGetGID, ls_referenceno, values.remarks, tax_amount, lstax2_gid);
                        }




                    }

                    msSQL = " delete from rbl_tmp_tinvoicedtl where employee_gid='" + employee_gid + "'";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    values.status = true;
                    values.message = "Invoice Submitted Successfully";
                }
                else
                {
                    values.status = false;
                    values.message = "Error while submitting the Invoice";
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while submitting Invoice!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" + $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() +
"***********" + ex.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/rbl/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
        }
        public string GetRandomString(int length)
        {
            string guidResult = Guid.NewGuid().ToString();
            guidResult = guidResult.Replace("-", string.Empty);
            guidResult = guidResult.ToUpper();

            if (length <= 0 || length > guidResult.Length)
            {
                throw new ArgumentException("Length must be between 1 and " + guidResult.Length);
            }

            return guidResult.Substring(0, length);
        }
        public void DaGetCourierSerive(MdlSmrTrnSalesorder values)
        {
            try
            {
                msSQL = " select courierservice_id, courierservicetype_id, name from crm_smm_tmintsoftcourierservice where name in ('Customer Collection','Driver Delivery','Fedex Next Day Domestic')";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var GetCourierService = new List<GetCourierService_list>();
                if (dt_datatable.Rows.Count > 0)
                {
                    foreach (DataRow dt in  dt_datatable.Rows)
                    {
                        GetCourierService.Add(new GetCourierService_list
                        {
                            courierservice_id = dt["courierservice_id"].ToString(),
                            name = dt["name"].ToString(),
                        });
                    }
                    values.GetCourierService_list = GetCourierService;
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occurred while Order to invoice summary!";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" +
                ex.Message.ToString() + "***********" + values.message.ToString() + "*****Query****" +
                msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" +
                DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
        }



        //Drafts

        public void DaPostSalesOrderDrafts(string employee_gid, postsales_list values)
        {
            try
            {

                string totalvalue = values.user_name;

                msSQL = " select company_code from adm_mst_tcompany";
                lscompany_code = objdbconn.GetExecuteScalar(msSQL);

                msSQL = "select tax_prefix from acp_mst_ttax where tax_gid='" + values.tax_name4 + "'";
                string lstaxname1 = objdbconn.GetExecuteScalar(msSQL);


                string lscustomerbranch = "H.Q";
                string lscampaign_gid = "NO CAMPAIGN";

                string inputDate = values.salesorder_date;
                DateTime uiDate = DateTime.ParseExact(inputDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string salesorder_date = uiDate.ToString("yyyy-MM-dd");

               
                    msSQL = " select customer_name from crm_mst_tcustomer where customer_gid='" + values.customer_gid + " '";
                    string lscustomername = objdbconn.GetExecuteScalar(msSQL);
                
                
               

                msSQL = " select currency_code from crm_trn_tcurrencyexchange where currencyexchange_gid='" + values.currency_code + " '";
                string currency_code = objdbconn.GetExecuteScalar(msSQL);

                string lslocaladdon = "0.00";
                string lslocaladditionaldiscount = "0.00";
                string lslocalgrandtotal = " 0.00";
                string lsgst = "0.00";
                string lsamount4 = "0.00";
                //string lsproducttotalamount = "0.00";

                double totalAmount = double.TryParse(values.tax_amount4, out double totalpriceValue) ? totalpriceValue : 0;
                double addonCharges = double.TryParse(values.addon_charge, out double addonChargesValue) ? addonChargesValue : 0;
                double freightCharges = double.TryParse(values.freight_charges, out double freightChargesValue) ? freightChargesValue : 0;
                double packingCharges = double.TryParse(values.packing_charges, out double packingChargesValue) ? packingChargesValue : 0;
                double insuranceCharges = double.TryParse(values.insurance_charges, out double insuranceChargesValue) ? insuranceChargesValue : 0;
                double roundoff = double.TryParse(values.roundoff, out double roundoffValue) ? roundoffValue : 0;
                double additionaldiscountAmount = double.TryParse(values.additional_discount, out double discountAmountValue) ? discountAmountValue : 0;
                double buybackCharges = double.TryParse(values.buyback_charges, out double buybackChargesValue) ? buybackChargesValue : 0;

                double grandTotal = (totalAmount + addonCharges + freightCharges + packingCharges + insuranceCharges + roundoff) - additionaldiscountAmount - buybackCharges;

                string lsinvoice_refno = "", lsorder_refno = "";

                msSQL = "Select * from smr_tmp_tsalesorderdrafts where salesorder_gid = '" + values.salesorder_gid + "'";
                string lssalesorder_gid = objdbconn.GetExecuteScalar(msSQL);

                if (lssalesorder_gid == null || lssalesorder_gid == "")
                {
                    mssalesorderGID = objcmnfunctions.GetMasterGID("VSOP");

                    //lsrefno = objcmnfunctions.GetMasterGID("SOR");
                    //msSQL = "select company_code from adm_mst_Tcompany";
                    //lscompany_code = objdbconn.GetExecuteScalar(msSQL);
                    //if (lscompany_code == "BOBA")
                    //{
                    //    string ls_referenceno = objcmnfunctions.getSequencecustomizerGID("INV", "RBL", values.branch_name);
                    //    msSQL = "SELECT SEQUENCE_CURVAL FROM adm_mst_tsequencecodecustomizer  WHERE sequence_code='INV' AND branch_gid='" + values.branch_name + "'";
                    //    string lscode = objdbconn.GetExecuteScalar(msSQL);


                    //    lsinvoice_refno = "SI" + " - " + lscode;
                    //    lsorder_refno = "SO" + " - " + lscode;

                    //}
                    //else
                    //{
                    //    lsinvoice_refno = mssalesorderGID;
                    //    lsorder_refno = lsrefno;

                    //}




                    msSQL = " insert  into smr_tmp_tsalesorderdrafts (" +
                             " salesorder_gid ," +
                             " branch_gid ," +
                             " salesorder_date," +
                             " customer_gid," +
                             " customer_name," +
                             " customer_address ," +
                             " bill_to ," +
                             " created_by," +
                             //" so_referenceno1 ," +
                             //" so_referencenumber ," +
                             " so_remarks," +
                             " payment_days, " +
                             " delivery_days, " +
                             " Grandtotal, " +
                             " termsandconditions, " +
                             " salesorder_status, " +
                             " addon_charge, " +
                             " additional_discount, " +
                             " addon_charge_l, " +
                             " additional_discount_l, " +
                             " grandtotal_l, " +
                             " currency_code, " +
                             " currency_gid, " +
                             " exchange_rate, " +
                             " shipping_to, " +
                             " tax_gid," +
                             " tax_name, " +
                             " gst_amount," +
                             " total_price," +
                             " total_amount," +
                             " tax_amount," +
                             " vessel_name," +
                             " salesperson_gid," +
                             " roundoff, " +
                             " updated_addon_charge, " +
                             " updated_additional_discount, " +
                             " freight_charges," +
                             " buyback_charges," +
                             " packing_charges," +
                             " insurance_charges, " +
                             " source_flag, " +
                             " renewal_flag ," +
                             "created_date" +
                             " )values(" +
                             " '" + mssalesorderGID + "'," +
                             " '" + values.branch_name + "'," +
                             " '" + salesorder_date + "'," +
                             " '" + values.customer_gid + "'," +
                             " '" + lscustomername.Replace("'", "\\\'") + "'," +
                             " '" + values.address1.Replace("'", "\\\'") + "'," +
                             " '" + values.address1.Replace("'", "\\\'") + "'," +
                             " '" + employee_gid + "'," +
                              //"' " + lsorder_refno + "'," +
                              //" '" + lsinvoice_refno + "'," +
                              " '" + values.so_remarks + "'," +
                             " '" + values.payment_days + "'," +
                             " '" + values.delivery_days + "'," +
                             " '" + values.grandtotal.Replace(",", "").Trim() + "'," +
                             " '" + values.termsandconditions.Replace("'", "\\\'") + "'," +
                             " 'Approved',";
                    if (values.addon_charge != "")
                    {
                        msSQL += "'" + values.addon_charge + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladdon + "',";
                    }
                    if (values.additional_discount != "")
                    {
                        msSQL += "'" + values.additional_discount + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladditionaldiscount + "',";
                    }
                    if (values.addon_charge != "")
                    {
                        msSQL += "'" + values.addon_charge + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladditionaldiscount + "',";
                    }
                    if (values.additional_discount != "")
                    {
                        msSQL += "'" + values.additional_discount + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladditionaldiscount + "',";
                    }
                    msSQL += " '" + lslocalgrandtotal + "'," +
                         " '" + currency_code + "'," +
                         " '" + values.currency_code + "'," +
                         " '" + values.exchange_rate + "'," +
                         " '" + values.shipping_address.Replace("'", "\\\'") + "'," +
                         " '" + values.tax_name4 + "'," +
                         " '" + lstaxname1 + "', " +
                        "'" + lsgst + "',";
                    msSQL += " '" + values.totalamount.Replace(",", "").Trim() + "',";
                    if (values.grandtotal == null && values.grandtotal == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += " '" + values.grandtotal.Replace(",", "").Trim() + "',";
                    }

                    if (values.tax_amount4 != "" && values.tax_amount4 != null)
                    {
                        msSQL += "'" + values.tax_amount4 + "',";
                    }
                    else
                    {
                        msSQL += "'" + lsamount4 + "',";
                    }
                    msSQL += " '" + values.vessel_name + "'," +
                            " '" + values.user_name + "',";
                    if (values.roundoff == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.roundoff + "',";
                    }
                    if (values.addon_charge == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.addon_charge + "',";
                    }
                    if (values.additional_discount == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.additional_discount + "',";
                    }
                    if (values.freight_charges == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.freight_charges + "',";
                    }
                    if (values.buyback_charges == "" || values.buyback_charges == null)
                    {

                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.buyback_charges + "',";
                    }
                    if (values.packing_charges == "" || values.packing_charges == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.packing_charges + "',";
                    }
                    if (values.insurance_charges == "" || values.insurance_charges == null)
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + values.insurance_charges + "',";
                    }
                    msSQL += "'I',";
                    msSQL += "'" + values.renewal_mode + "',";

                    msSQL += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                    if (mnResult == 0)
                    {
                        values.status = false;
                        values.message = " Some Error Occurred While Inserting Salesorder Details";
                        return;
                    }
                    else
                    {
                        msSQL = " insert  into acp_trn_torder (" +
                               " salesorder_gid ," +
                               " branch_gid ," +
                               " salesorder_date," +
                               " customer_gid," +
                               " customer_name," +
                               " customer_address," +
                               " created_by," +
                               " so_remarks," +
                               " so_referencenumber," +
                               " payment_days, " +
                               " delivery_days, " +
                               " Grandtotal, " +
                               " termsandconditions, " +
                               " salesorder_status, " +
                               " addon_charge, " +
                               " additional_discount, " +
                               " addon_charge_l, " +
                               " additional_discount_l, " +
                               " grandtotal_l, " +
                               " currency_code, " +
                               " currency_gid, " +
                               " exchange_rate, " +
                               " updated_addon_charge, " +
                               " updated_additional_discount, " +
                               " shipping_to, " +
                               " campaign_gid, " +
                               " roundoff," +
                               " salesperson_gid, " +
                               " freight_charges," +
                               " buyback_charges," +
                               " packing_charges," +
                               " insurance_charges " +
                               ") values(" +
                               " '" + mssalesorderGID + "'," +
                               " '" + values.branch_name + "'," +
                               " '" + salesorder_date + "'," +
                               " '" + values.customer_gid + "'," +
                               " '" + lscustomername.Replace("'", "\\\'") + "'," +
                               " '" + values.address1.Replace("'", "\\\'") + "'," +
                               " '" + employee_gid + "'," +
                               " '" + values.so_remarks + "'," +
                               " '" + lsrefno + "'," +
                               " '" + values.payment_days + "'," +
                               " '" + values.delivery_days + "'," +
                               " '" + values.grandtotal + "'," +
                               " '" + values.termsandconditions.Replace("'", "\\\'") + "'," +
                               " 'Approved',";
                        if (values.addon_charge == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.addon_charge + "',";
                        }
                        if (values.additional_discount == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.additional_discount + "',";
                        }
                        msSQL += "'" + lslocaladdon + "'," +
                               " '" + lslocaladditionaldiscount + "'," +
                               " '" + lslocalgrandtotal + "'," +
                               " '" + currency_code + "'," +
                               " '" + values.currency_code + "'," +
                               " '" + values.exchange_rate + "',";
                        if (values.addon_charge == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.addon_charge + "',";
                        }
                        if (values.additional_discount == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.additional_discount + "',";
                        }
                        msSQL += "'" + values.shipping_address.Replace("'", "\\\'") + "'," +
                               " '" + lscampaign_gid + "',";
                        if (values.roundoff == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.roundoff + "',";
                        }
                        msSQL += " '" + values.user_name + "',";
                        if (values.freight_charges == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.freight_charges + "',";
                        }
                        if (values.buyback_charges == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.buyback_charges + "',";
                        }
                        if (values.packing_charges == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + values.packing_charges + "',";
                        }
                        if (values.insurance_charges == "")
                        {
                            msSQL += "'0.00')";
                        }
                        else
                        {
                            msSQL += "'" + values.insurance_charges + "')";
                        }
                        mnResult2 = objdbconn.ExecuteNonQuerySQL(msSQL);
                        if (mnResult2 == 1)
                        {
                            values.status = true;
                        }

                    }



                    msSQL = " select * from smr_tmp_tsalesorderdtl " +
                      " where employee_gid='" + employee_gid + "'";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                    msSQL = "delete from smr_tmp_tsalesorderdtldrafts where salesorder_gid = '" + values.salesorder_gid + "'";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                    if (mnResult == 1)
                    {

                        if (dt_datatable.Rows.Count != 0)
                        {

                            msSQL = " select " +
                                        " tmpsalesorderdtl_gid," +
                                        " salesorder_gid," +
                                        " product_gid," +
                                        " productgroup_gid," +
                                        " product_remarks," +
                                        " product_name," +
                                         " product_code," +
                                        " product_price," +
                                        " qty_quoted," +
                                        " discount_percentage," +
                                        " discount_amount," +
                                        " uom_gid," +
                                        " uom_name," +
                                        " price," +
                                        " tax_name," +
                                        " tax1_gid, " +
                                        " tax_amount," +
                                         " tax_name2," +
                                        " tax2_gid, " +
                                        " tax_amount2," +
                                         " tax_percentage2," +
                                        " slno," +
                                        " tax_percentage," +
                                        " order_type, " +
                                        " taxsegment_gid, " +
                                        " taxsegmenttax_gid " +
                                        " from smr_tmp_tsalesorderdtl" +
                                        " where employee_gid='" + employee_gid + "' ";
                            dt_datatable = objdbconn.GetDataTable(msSQL);
                            var getModuleList = new List<postsales_list>();
                            if (dt_datatable.Rows.Count != 0)
                            {
                                foreach (DataRow dt in  dt_datatable.Rows)
                                {
                                    getModuleList.Add(new postsales_list
                                    {
                                        salesorder_gid = dt["salesorder_gid"].ToString(),
                                        tmpsalesorderdtl_gid = dt["tmpsalesorderdtl_gid"].ToString(),
                                        product_gid = dt["product_gid"].ToString(),
                                        product_name = dt["product_name"].ToString(),
                                        product_code = dt["product_code"].ToString(),
                                        productuom_name = dt["uom_name"].ToString(),
                                        productgroup_gid = dt["productgroup_gid"].ToString(),
                                        product_remarks = dt["product_remarks"].ToString(),
                                        unitprice = dt["product_price"].ToString(),
                                        quantity = dt["qty_quoted"].ToString(),
                                        discountpercentage = dt["discount_percentage"].ToString(),
                                        discountamount = dt["discount_amount"].ToString(),
                                        tax_name = dt["tax_name"].ToString(),
                                        tax_amount = dt["tax_amount"].ToString(),
                                        totalamount = dt["price"].ToString(),
                                        order_type = dt["order_type"].ToString(),
                                        slno = dt["slno"].ToString(),
                                        taxsegment_gid = dt["taxsegment_gid"].ToString(),
                                        taxsegmenttax_gid = dt["taxsegmenttax_gid"].ToString(),
                                    });

                                    int i = 0;

                                    //mssalesorderGID1 = objcmnfunctions.GetMasterGID("VSDC");
                                    //if (mssalesorderGID1 == "E")
                                    //{
                                    //    values.message = "Create Sequence code for VSDC ";
                                    //    return;
                                    //}



                                    msSQL = " insert into smr_tmp_tsalesorderdtldrafts (" +
                                         " tmpsalesorderdtl_gid ," +
                                         " salesorder_gid," +
                                         " product_gid ," +
                                         " product_name," +
                                         " product_code," +
                                         " product_price," +
                                         " productgroup_gid," +
                                         " product_remarks," +
                                         " display_field," +
                                         " qty_quoted," +
                                         " discount_percentage," +
                                         " discount_amount," +
                                         " tax_amount ," +
                                         " uom_gid," +
                                         " uom_name," +
                                         " price," +
                                         " tax_name," +
                                         " tax1_gid," +
                                          " tax_name2," +
                                         " tax2_gid," +
                                          " tax_percentage2," +
                                           " tax_amount2," +
                                         " slno," +
                                         " tax_percentage," +
                                         " taxsegment_gid," +
                                         " employee_gid," +
                                         " taxsegmenttax_gid," +
                                         " type " +
                                         ")values(" +
                                         " '" + dt["tmpsalesorderdtl_gid"].ToString() + "'," +
                                         " '" + mssalesorderGID + "'," +
                                         " '" + dt["product_gid"].ToString() + "'," +
                                         " '" + dt["product_name"].ToString() + "'," +
                                         " '" + dt["product_code"].ToString() + "'," +
                                         " '" + dt["product_price"].ToString() + "'," +
                                         " '" + dt["productgroup_gid"].ToString() + "'," +
                                         " '" + dt["product_remarks"].ToString() + "'," +
                                          " '" + dt["product_remarks"].ToString() + "'," +
                                         " '" + dt["qty_quoted"].ToString() + "'," +
                                         " '" + dt["discount_percentage"].ToString() + "'," +
                                         " '" + dt["discount_amount"].ToString() + "'," +
                                         " '" + dt["tax_amount"].ToString() + "'," +
                                         " '" + dt["uom_gid"].ToString() + "'," +
                                         " '" + dt["uom_name"].ToString() + "'," +
                                         " '" + dt["price"].ToString() + "'," +
                                         " '" + dt["tax_name"].ToString() + "'," +
                                         " '" + dt["tax1_gid"].ToString() + "'," +
                                           " '" + dt["tax_name2"].ToString() + "'," +
                                             " '" + dt["tax2_gid"].ToString() + "'," +
                                               " '" + dt["tax_percentage2"].ToString() + "'," +
                                                 " '" + dt["tax_amount2"].ToString() + "'," +
                                         " '" + i + 1 + "'," +
                                         " '" + dt["tax_percentage"].ToString() + "'," +
                                         " '" + dt["taxsegment_gid"].ToString() + "'," +
                                         " '" + dt["taxsegment_gid"].ToString() + "'," +
                                         " '" + employee_gid + "'," +
                                         " '" + dt["order_type"].ToString() + "')";
                                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                                    if (mnResult == 0)
                                    {
                                        values.status = false;
                                        values.message = "Error occurred while Insertion";
                                        return;
                                    }


                                    msSQL = " insert into acp_trn_torderdtl (" +
                                     " salesorderdtl_gid ," +
                                     " salesorder_gid," +
                                     " product_gid ," +
                                     " product_name," +
                                     " product_price," +
                                     " qty_quoted," +
                                     " discount_percentage," +
                                     " discount_amount," +
                                     " tax_amount ," +
                                     " uom_gid," +
                                     " uom_name," +
                                     " price," +
                                     " tax_name," +
                                     " tax1_gid," +
                                     " slno," +
                                     " tax_percentage," +
                                     " taxsegment_gid," +
                                     " type, " +
                                     " salesorder_refno" +
                                     ")values(" +
                                     " '" + dt["tmpsalesorderdtl_gid"].ToString() + "'," +
                                     " '" + mssalesorderGID + "'," +
                                     " '" + dt["product_gid"].ToString() + "'," +
                                     " '" + dt["product_name"].ToString() + "'," +
                                     " '" + dt["product_price"].ToString() + "'," +
                                     " '" + dt["qty_quoted"].ToString() + "'," +
                                     " '" + dt["discount_percentage"].ToString() + "'," +
                                     " '" + dt["discount_amount"].ToString() + "'," +
                                     " '" + dt["tax_amount"].ToString() + "'," +
                                     " '" + dt["uom_gid"].ToString() + "'," +
                                     " '" + dt["uom_name"].ToString() + "'," +
                                     " '" + dt["price"].ToString() + "'," +
                                     " '" + dt["tax_name"].ToString() + "'," +
                                      " '" + dt["tax1_gid"].ToString() + "'," +
                                     " '" + values.slno + "'," +
                                     " '" + dt["tax_percentage"].ToString() + "'," +
                                     " '" + dt["taxsegment_gid"].ToString() + "'," +
                                     " '" + dt["order_type"].ToString() + "', " +
                                     " '" + values.salesorder_refno + "')";
                                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                                }
                            }
                        }
                    }
                   
                }
                else
                {
                    msSQL = " UPDATE smr_tmp_tsalesorderdrafts SET " +
                            "branch_gid = '" + values.branch_name + "'," +
                            "salesorder_date = '" + salesorder_date + "'," +
                            "customer_gid = '" + values.customer_gid + "'," +
                            "customer_name = '" + lscustomername.Replace("'", "\\\'") + "'," +
                            "customer_address = '" + values.address1.Replace("'", "\\\'") + "'," +
                            "bill_to = '" + values.address1.Replace("'", "\\\'") + "'," +
                            "created_by = '" + employee_gid + "'," +
                            "so_remarks = '" + values.so_remarks + "'," +
                            "payment_days = '" + values.payment_days + "'," +
                            "delivery_days = '" + values.delivery_days + "'," +
                            "Grandtotal = '" + values.grandtotal.Replace(",", "").Trim() + "'," +
                            "termsandconditions = '" + values.termsandconditions.Replace("'", "\\\'") + "'," +
                            "salesorder_status = 'Approved'," +
                            "addon_charge = '" + (values.addon_charge != "" ? values.addon_charge : lslocaladdon) + "'," +
                            "additional_discount = '" + (values.additional_discount != "" ? values.additional_discount : lslocaladditionaldiscount) + "'," +
                            "addon_charge_l = '" + (values.addon_charge != "" ? values.addon_charge : lslocaladditionaldiscount) + "'," +
                            "additional_discount_l = '" + (values.additional_discount != "" ? values.additional_discount : lslocaladditionaldiscount) + "'," +
                            "grandtotal_l = '" + lslocalgrandtotal + "'," +
                            "currency_code = '" + currency_code + "'," +
                            "currency_gid = '" + values.currency_code + "'," +
                            "exchange_rate = '" + values.exchange_rate + "'," +
                            "shipping_to = '" + values.shipping_address.Replace("'", "\\\'") + "'," +
                            "tax_gid = '" + values.tax_name4 + "'," +
                            "tax_name = '" + lstaxname1 + "'," +
                            "gst_amount = '" + lsgst + "'," +
                            "total_price = '" + values.totalamount.Replace(",", "").Trim() + "'," +
                            "total_amount = '" + (string.IsNullOrEmpty(values.grandtotal) ? "0.00" : values.grandtotal.Replace(",", "").Trim()) + "'," +
                            "tax_amount = '" + (string.IsNullOrEmpty(values.tax_amount4) ? lsamount4 : values.tax_amount4) + "'," +
                            "vessel_name = '" + values.vessel_name + "'," +
                            "salesperson_gid = '" + values.user_name + "'," +
                            "roundoff = '" + (string.IsNullOrEmpty(values.roundoff) ? "0.00" : values.roundoff) + "'," +
                            "updated_addon_charge = '" + (string.IsNullOrEmpty(values.addon_charge) ? "0.00" : values.addon_charge) + "'," +
                            "updated_additional_discount = '" + (string.IsNullOrEmpty(values.additional_discount) ? "0.00" : values.additional_discount) + "'," +
                            "freight_charges = '" + (string.IsNullOrEmpty(values.freight_charges) ? "0.00" : values.freight_charges) + "'," +
                            "buyback_charges = '" + (string.IsNullOrEmpty(values.buyback_charges) ? "0.00" : values.buyback_charges) + "'," +
                            "packing_charges = '" + (string.IsNullOrEmpty(values.packing_charges) ? "0.00" : values.packing_charges) + "'," +
                            "insurance_charges = '" + (string.IsNullOrEmpty(values.insurance_charges) ? "0.00" : values.insurance_charges) + "'," +
                            "source_flag = 'I'," +
                            "renewal_flag = '" + values.renewal_mode + "'," +
                            "created_date = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                            "  WHERE salesorder_gid = '" + values.salesorder_gid + "'";

                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                    msSQL = " select * from smr_tmp_tsalesorderdtl " +
                             " where employee_gid='" + employee_gid + "'";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                    msSQL = "delete from smr_tmp_tsalesorderdtldrafts where salesorder_gid = '" + values.salesorder_gid + "'";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                    if (mnResult == 1)
                    {

                        if (dt_datatable.Rows.Count != 0)
                        {

                            msSQL = " select " +
                                        " tmpsalesorderdtl_gid," +
                                        " salesorder_gid," +
                                        " product_gid," +
                                        " productgroup_gid," +
                                        " product_remarks," +
                                        " product_name," +
                                         " product_code," +
                                        " product_price," +
                                        " qty_quoted," +
                                        " discount_percentage," +
                                        " discount_amount," +
                                        " uom_gid," +
                                        " uom_name," +
                                        " price," +
                                        " tax_name," +
                                        " tax1_gid, " +
                                        " tax_amount," +
                                         " tax_name2," +
                                        " tax2_gid, " +
                                        " tax_amount2," +
                                         " tax_percentage2," +
                                        " slno," +
                                        " tax_percentage," +
                                        " order_type, " +
                                        " taxsegment_gid, " +
                                        " taxsegmenttax_gid " +
                                        " from smr_tmp_tsalesorderdtl" +
                                        " where employee_gid='" + employee_gid + "' ";
                            dt_datatable = objdbconn.GetDataTable(msSQL);
                            var getModuleList = new List<postsales_list>();
                            if (dt_datatable.Rows.Count != 0)
                            {
                                foreach (DataRow dt in  dt_datatable.Rows)
                                {
                                    getModuleList.Add(new postsales_list
                                    {
                                        salesorder_gid = dt["salesorder_gid"].ToString(),
                                        tmpsalesorderdtl_gid = dt["tmpsalesorderdtl_gid"].ToString(),
                                        product_gid = dt["product_gid"].ToString(),
                                        product_name = dt["product_name"].ToString(),
                                        product_code = dt["product_code"].ToString(),
                                        productuom_name = dt["uom_name"].ToString(),
                                        productgroup_gid = dt["productgroup_gid"].ToString(),
                                        product_remarks = dt["product_remarks"].ToString(),
                                        unitprice = dt["product_price"].ToString(),
                                        quantity = dt["qty_quoted"].ToString(),
                                        discountpercentage = dt["discount_percentage"].ToString(),
                                        discountamount = dt["discount_amount"].ToString(),
                                        tax_name = dt["tax_name"].ToString(),
                                        tax_amount = dt["tax_amount"].ToString(),
                                        totalamount = dt["price"].ToString(),
                                        order_type = dt["order_type"].ToString(),
                                        slno = dt["slno"].ToString(),
                                        taxsegment_gid = dt["taxsegment_gid"].ToString(),
                                        taxsegmenttax_gid = dt["taxsegmenttax_gid"].ToString(),
                                    });

                                    int i = 0;

                                    //mssalesorderGID1 = objcmnfunctions.GetMasterGID("VSDC");
                                    //if (mssalesorderGID1 == "E")
                                    //{
                                    //    values.message = "Create Sequence code for VSDC ";
                                    //    return;
                                    //}



                                    msSQL = " insert into smr_tmp_tsalesorderdtldrafts (" +
                                         " tmpsalesorderdtl_gid ," +
                                         " salesorder_gid," +
                                         " product_gid ," +
                                         " product_name," +
                                         " product_code," +
                                         " product_price," +
                                         " productgroup_gid," +
                                         " product_remarks," +
                                         " display_field," +
                                         " qty_quoted," +
                                         " discount_percentage," +
                                         " discount_amount," +
                                         " tax_amount ," +
                                         " uom_gid," +
                                         " uom_name," +
                                         " price," +
                                         " tax_name," +
                                         " tax1_gid," +
                                          " tax_name2," +
                                         " tax2_gid," +
                                          " tax_percentage2," +
                                           " tax_amount2," +
                                         " slno," +
                                         " tax_percentage," +
                                         " taxsegment_gid," +
                                         " employee_gid," +
                                         " taxsegmenttax_gid," +
                                         " type " +
                                         ")values(" +
                                         " '" + dt["tmpsalesorderdtl_gid"].ToString() + "'," +
                                         " '" + values.salesorder_gid + "'," +
                                         " '" + dt["product_gid"].ToString() + "'," +
                                         " '" + dt["product_name"].ToString() + "'," +
                                         " '" + dt["product_code"].ToString() + "'," +
                                         " '" + dt["product_price"].ToString() + "'," +
                                         " '" + dt["productgroup_gid"].ToString() + "'," +
                                         " '" + dt["product_remarks"].ToString() + "'," +
                                          " '" + dt["product_remarks"].ToString() + "'," +
                                         " '" + dt["qty_quoted"].ToString() + "'," +
                                         " '" + dt["discount_percentage"].ToString() + "'," +
                                         " '" + dt["discount_amount"].ToString() + "'," +
                                         " '" + dt["tax_amount"].ToString() + "'," +
                                         " '" + dt["uom_gid"].ToString() + "'," +
                                         " '" + dt["uom_name"].ToString() + "'," +
                                         " '" + dt["price"].ToString() + "'," +
                                         " '" + dt["tax_name"].ToString() + "'," +
                                         " '" + dt["tax1_gid"].ToString() + "'," +
                                           " '" + dt["tax_name2"].ToString() + "'," +
                                             " '" + dt["tax2_gid"].ToString() + "'," +
                                               " '" + dt["tax_percentage2"].ToString() + "'," +
                                                 " '" + dt["tax_amount2"].ToString() + "'," +
                                         " '" + i + 1 + "'," +
                                         " '" + dt["tax_percentage"].ToString() + "'," +
                                         " '" + dt["taxsegment_gid"].ToString() + "'," +
                                         " '" + dt["taxsegment_gid"].ToString() + "'," +
                                         " '" + employee_gid + "'," +
                                         " '" + dt["order_type"].ToString() + "')";
                                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                                }

                            }


                            msSQL = "select distinct type from smr_trn_tsalesorderdtl where salesorder_gid='" + mssalesorderGID + "' ";
                            objodbcDataReader = objdbconn.GetDataReader(msSQL);

                            if (objodbcDataReader.HasRows == true)
                            {
                                if (objodbcDataReader["type"].ToString()!="Services")
                                { 

                                lsorder_type = "Sales";
                                }
                                else
                                {
                                    lsorder_type = "Services";
                                }


                            }

                           
                            objodbcDataReader.Close();

                            msSQL = " update smr_tmp_tsalesorderdrafts set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                            msSQL = " update acp_trn_torder set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                        }
                    }
                   

                }
                if (mnResult != 0)
                {
                    values.status = true;
                    values.message = "Sales order draft has been saved successfully";
                    return;
                }
                else
                {
                    values.status = false;
                    values.message = "Error While Saving Sales Order Draft";
                    return;
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while Submitting Sales Order !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
            finally
            {
                if (objodbcDataReader != null)
                    objodbcDataReader.Close();
            }


        }
        //public void DaPostSalesOrderfileuploadDrafts(HttpRequest httpRequest, result objResult, string employee_gid)
        //{
        //    try
        //    {

        //        string customer_gid = Request.Form["customer_gid"];
        //        string branch_name = Request.Form["branch_name"];
        //        string branch_gid = Request.Form["branch_gid"];
        //        string salesorder_date = Request.Form["salesorder_date"];
        //        string renewal_mode = Request.Form["renewal_mode"];
        //        string renewal_date = Request.Form["renewal_date"];
        //        string frequency_terms = Request.Form["frequency_terms"];
        //        string customer_name = Request.Form["customer_name"];
        //        string so_remarks = Request.Form["so_remarks"];
        //        string so_referencenumber = Request.Form["so_referencenumber"];
        //        string address1 = Request.Form["address1"];
        //        string shipping_address = Request.Form["shipping_address"];
        //        string delivery_days = Request.Form["delivery_days"];
        //        string payment_days = Request.Form["payment_days"];
        //        string currency_code = Request.Form["currency_code"];
        //        string user_name = Request.Form["user_name"];
        //        string exchange_rate = Request.Form["exchange_rate"];
        //        string termsandconditions = Request.Form["termsandconditions"];
        //        string template_name = Request.Form["template_name"];
        //        string template_gid = Request.Form["template_gid"];
        //        string grandtotal = Request.Form["grandtotal"];
        //        string roundoff = Request.Form["roundoff"];
        //        string insurance_charges = Request.Form["insurance_charges"];
        //        string packing_charges = Request.Form["packing_charges"];
        //        string buyback_charges = Request.Form["buyback_charges"];
        //        string freight_charges = Request.Form["freight_charges"];
        //        string additional_discount = Request.Form["additional_discount"];
        //        string addon_charge = Request.Form["addon_charge"];
        //        string tax_amount4 = Request.Form["tax_amount4"];
        //        string tax_name4 = Request.Form["tax_name4"];
        //        string totalamount = Request.Form["totalamount"];
        //        string total_price = Request.Form["total_price"];
        //        string taxsegment_gid = Request.Form["taxsegment_gid"];
        //        string salesorder_gid = Request.Form["salesorder_gid"];

        //        HttpFileCollection httpFileCollection;
        //        string lsfilepath = string.Empty;
        //        string lsdocument_gid = string.Empty;
        //        MemoryStream ms_stream = new MemoryStream();
        //        string document_gid = string.Empty;
        //        string lscompany_code = string.Empty;
        //        HttpPostedFile httpPostedFile;
        //        string lspath;
        //        string final_path = "";
        //        string vessel_name = "";

        //        msSQL = " select company_code from adm_mst_tcompany";
        //        lscompany_code = objdbconn.GetExecuteScalar(msSQL);



        //        MemoryStream ms = new MemoryStream();
        //        lspath = ConfigurationManager.AppSettings["Doc_upload_file"] + "/erp_documents" + "/" + lscompany_code + "/" + "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month;



        //        {
        //            if ((!System.IO.Directory.Exists(lspath)))
        //                System.IO.Directory.CreateDirectory(lspath);
        //        }



        //        if (httpRequest.Files.Count > 0)
        //        {
        //            string lsfirstdocument_filepath = string.Empty;
        //            httpFileCollection = httpRequest.Files;
        //            for (int i = 0; i < httpFileCollection.Count; i++)
        //            {
        //                string msdocument_gid = objcmnfunctions.GetMasterGID("UPLF");
        //                httpPostedFile = httpFileCollection[i];
        //                FileExtensionname = httpPostedFile.FileName;
        //                string lsfile_gid = msdocument_gid;
        //                string lscompany_document_flag = string.Empty;
        //                FileExtension = Path.GetExtension(FileExtensionname).ToLower();
        //                lsfile_gid = lsfile_gid + FileExtension;
        //                Stream ls_readStream;
        //                ls_readStream = httpPostedFile.InputStream;
        //                ls_readStream.CopyTo(ms);
        //                lspath = ConfigurationManager.AppSettings["Doc_upload_file"] + "/erp_documents" + "/" + lscompany_code + "/" + "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/";
        //                string status;
        //                status = objcmnfunctions.uploadFile(lspath + msdocument_gid, FileExtension);
        //                ms.Close();
        //                lspath = "erp_documents" + "/" + lscompany_code + "/" + "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/";



        //                final_path = lspath + msdocument_gid + FileExtension;



        //            }
        //        }





        //        string totalvalue = user_name;





        //        msSQL = "select tax_prefix from acp_mst_ttax where tax_gid='" + tax_name4 + "'";
        //        string lstaxname1 = objdbconn.GetExecuteScalar(msSQL);


        //        string lscustomerbranch = "H.Q";
        //        string lscampaign_gid = "NO CAMPAIGN";



        //            string inputDate = salesorder_date;
        //            DateTime uiDate = DateTime.ParseExact(inputDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //            string salesorder_date1 = uiDate.ToString("yyyy-MM-dd");

        //            msSQL = " select customer_name from crm_mst_tcustomer where customer_gid='" + customer_gid + " '";
        //            string lscustomername = objdbconn.GetExecuteScalar(msSQL);

        //            msSQL = " select currency_code from crm_trn_tcurrencyexchange where currencyexchange_gid='" + currency_code + " '";
        //            string currency_code1 = objdbconn.GetExecuteScalar(msSQL);

        //            string lslocaladdon = "0.00";
        //            string lslocaladditionaldiscount = "0.00";
        //            string lslocalgrandtotal = " 0.00";
        //            string lsgst = "0.00";
        //            string lsamount4 = "0.00";
        //            //string lsproducttotalamount = "0.00";

        //            double totalAmount = double.TryParse(tax_amount4, out double totalpriceValue) ? totalpriceValue : 0;
        //            double addonCharges = double.TryParse(addon_charge, out double addonChargesValue) ? addonChargesValue : 0;
        //            double freightCharges = double.TryParse(freight_charges, out double freightChargesValue) ? freightChargesValue : 0;
        //            double packingCharges = double.TryParse(packing_charges, out double packingChargesValue) ? packingChargesValue : 0;
        //            double insuranceCharges = double.TryParse(insurance_charges, out double insuranceChargesValue) ? insuranceChargesValue : 0;
        //            double roundoff1 = double.TryParse(roundoff, out double roundoffValue) ? roundoffValue : 0;
        //            double additionaldiscountAmount = double.TryParse(additional_discount, out double discountAmountValue) ? discountAmountValue : 0;
        //            double buybackCharges = double.TryParse(buyback_charges, out double buybackChargesValue) ? buybackChargesValue : 0;

        //            double grandTotal = (totalAmount + addonCharges + freightCharges + packingCharges + insuranceCharges + roundoff1) - additionaldiscountAmount - buybackCharges;


        //        //lsrefno = objcmnfunctions.GetMasterGID("SOR");

        //        //if (so_referencenumber.Length > 0)
        //        //{
        //        //}
        //        //else
        //        //{
        //        //    so_referencenumber = lsrefno;
        //        //}
        //        msSQL = "Select * from smr_tmp_tsalesorderdrafts where salesorder_gid = '" + salesorder_gid + "'";
        //        string lssalesorder_gid = objdbconn.GetExecuteScalar(msSQL);
        //        if(lssalesorder_gid == null || lssalesorder_gid == "")
        //        {


        //            mssalesorderGID = objcmnfunctions.GetMasterGID("VSOP");

        //            msSQL = " insert  into smr_tmp_tsalesorderdrafts (" +
        //                     " salesorder_gid ," +
        //                     " branch_gid ," +
        //                     " salesorder_date," +
        //                     " customer_gid," +
        //                     " customer_name," +
        //                     " customer_address ," +
        //                     " bill_to ," +
        //                     " created_by," +
        //                     //" so_referenceno1 ," +
        //                     " so_remarks," +
        //                     " payment_days, " +
        //                     " delivery_days, " +
        //                     " Grandtotal, " +
        //                     " termsandconditions, " +
        //                     " salesorder_status, " +
        //                     " addon_charge, " +
        //                     " additional_discount, " +
        //                     " addon_charge_l, " +
        //                     " additional_discount_l, " +
        //                     " grandtotal_l, " +
        //                     " currency_code, " +
        //                     " currency_gid, " +
        //                     " exchange_rate, " +
        //                      " file_path, " +
        //                     " shipping_to, " +
        //                     " tax_gid," +
        //                     " tax_name, " +
        //                     " gst_amount," +
        //                     " total_price," +
        //                     " total_amount," +
        //                     " tax_amount," +
        //                     " vessel_name," +
        //                     " salesperson_gid," +
        //                     " roundoff, " +
        //                     " updated_addon_charge, " +
        //                     " updated_additional_discount, " +
        //                     " freight_charges," +
        //                     " buyback_charges," +
        //                     " packing_charges," +
        //                     " insurance_charges, " +
        //                      " source_flag, " +
        //                      " renewal_flag ," +
        //                      " file_name ," +
        //                     "created_date" +
        //                     " )values(" +
        //                     " '" + mssalesorderGID + "'," +
        //                     " '" + branch_name + "'," +
        //                     " '" + salesorder_date1 + "'," +
        //                     " '" + customer_gid + "'," +
        //                     " '" + lscustomername.Replace("'", "\\\'") + "'," +
        //                     " '" + address1.Replace("'", "\\\'") + "'," +
        //                     " '" + address1.Replace("'", "\\\'") + "'," +
        //                     " '" + employee_gid + "'," +
        //                      // if(values.so_referencenumber != "" || values.so_referencenumber != null)
        //                      // {
        //                      //msSQL+= "'" + values.so_referencenumber + "',";
        //                      //  }
        //                      // else
        //                      // {
        //                      //        msSQL+=" '" + lsrefno + "',";
        //                      // }
        //                      //" '" + so_referencenumber + "'," +
        //                      " '" + so_remarks + "'," +
        //                     " '" + payment_days + "'," +
        //                     " '" + delivery_days + "'," +
        //                     " '" + grandtotal.Replace(",", "").Trim() + "'," +
        //                     " '" + termsandconditions.Replace("'", "\\\'") + "'," +
        //                     " 'Approved',";
        //            if (addon_charge != "")
        //            {
        //                msSQL += "'" + addon_charge + "',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + lslocaladdon + "',";
        //            }
        //            if (additional_discount != "")
        //            {
        //                msSQL += "'" + additional_discount + "',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + lslocaladditionaldiscount + "',";
        //            }
        //            if (addon_charge != "")
        //            {
        //                msSQL += "'" + addon_charge + "',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + lslocaladditionaldiscount + "',";
        //            }
        //            if (additional_discount != "")
        //            {
        //                msSQL += "'" + additional_discount + "',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + lslocaladditionaldiscount + "',";
        //            }
        //            msSQL += " '" + lslocalgrandtotal + "'," +
        //                 " '" + currency_code1 + "'," +
        //                 " '" + currency_code + "'," +
        //                 " '" + exchange_rate + "'," +
        //                   " '" + final_path + "'," +
        //                 " '" + shipping_address.Replace("'", "\\\'") + "'," +
        //                 " '" + tax_name4 + "'," +
        //                 " '" + lstaxname1 + "', " +
        //                "'" + lsgst + "',";
        //            msSQL += " '" + totalamount.Replace(",", "").Trim() + "',";
        //            if (grandtotal == null && grandtotal == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += " '" + grandtotal.Replace(",", "").Trim() + "',";
        //            }

        //            if (tax_amount4 != "" && tax_amount4 != null)
        //            {
        //                msSQL += "'" + tax_amount4 + "',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + lsamount4 + "',";
        //            }
        //            msSQL += " '" + vessel_name + "'," +
        //                    " '" + user_name + "',";
        //            if (roundoff == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + roundoff + "',";
        //            }
        //            if (addon_charge == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + addon_charge + "',";
        //            }
        //            if (additional_discount == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + additional_discount + "',";
        //            }
        //            if (freight_charges == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + freight_charges + "',";
        //            }
        //            if (buyback_charges == "")
        //            {

        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + buyback_charges + "',";
        //            }
        //            if (packing_charges == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + packing_charges + "',";
        //            }
        //            if (insurance_charges == "")
        //            {
        //                msSQL += "'0.00',";
        //            }
        //            else
        //            {
        //                msSQL += "'" + insurance_charges + "',";
        //            }
        //            msSQL += "'I',";
        //            msSQL += "'" + renewal_mode + "',";
        //            msSQL += "'" + FileExtensionname + "',";

        //            msSQL += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //            if (mnResult == 0)
        //            {
        //                objResult.status = false;
        //                objResult.message = " Some Error Occurred While Inserting Salesorder Details";
        //                return;
        //            }
        //            else
        //            {
        //                msSQL = " insert  into acp_trn_torder (" +
        //                       " salesorder_gid ," +
        //                       " branch_gid ," +
        //                       " salesorder_date," +
        //                       " customer_gid," +
        //                       " customer_name," +
        //                       " customer_address," +
        //                       " created_by," +
        //                       " so_remarks," +
        //                       " so_referencenumber," +
        //                       " payment_days, " +
        //                       " delivery_days, " +
        //                       " Grandtotal, " +
        //                       " termsandconditions, " +
        //                       " salesorder_status, " +
        //                       " addon_charge, " +
        //                       " additional_discount, " +
        //                       " addon_charge_l, " +
        //                       " additional_discount_l, " +
        //                       " grandtotal_l, " +
        //                       " currency_code, " +
        //                       " currency_gid, " +
        //                       " exchange_rate, " +
        //                        " file_path, " +
        //                       " updated_addon_charge, " +
        //                       " updated_additional_discount, " +
        //                       " shipping_to, " +
        //                       " campaign_gid, " +
        //                       " roundoff," +
        //                       " salesperson_gid, " +
        //                       " freight_charges," +
        //                       " buyback_charges," +
        //                       " packing_charges," +
        //                       " insurance_charges " +
        //                       ") values(" +
        //                       " '" + mssalesorderGID + "'," +
        //                       " '" + branch_name + "'," +
        //                       " '" + salesorder_date1 + "'," +
        //                       " '" + customer_gid + "'," +
        //                       " '" + lscustomername.Replace("'", "\\\'") + "'," +
        //                       " '" + address1.Replace("'", "\\\'") + "'," +
        //                       " '" + employee_gid + "'," +
        //                       " '" + so_remarks + "'," +
        //                       " '" + lsrefno + "'," +
        //                       " '" + payment_days + "'," +
        //                       " '" + delivery_days + "'," +
        //                       " '" + grandtotal + "'," +
        //                       " '" + termsandconditions.Replace("'", "\\\'") + "'," +
        //                       " 'Approved',";
        //                if (addon_charge == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + addon_charge + "',";
        //                }
        //                if (additional_discount == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + additional_discount + "',";
        //                }
        //                msSQL += "'" + lslocaladdon + "'," +
        //                       " '" + lslocaladditionaldiscount + "'," +
        //                       " '" + lslocalgrandtotal + "'," +
        //                       " '" + currency_code1 + "'," +
        //                       " '" + currency_code + "'," +
        //                       " '" + exchange_rate + "'," +
        //                " '" + final_path + "',";
        //                if (addon_charge == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + addon_charge + "',";
        //                }
        //                if (additional_discount == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + additional_discount + "',";
        //                }
        //                msSQL += "'" + shipping_address.Replace("'", "\\\'") + "'," +
        //                       " '" + lscampaign_gid + "',";
        //                if (roundoff == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + roundoff + "',";
        //                }
        //                msSQL += " '" + user_name + "',";
        //                if (freight_charges == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + freight_charges + "',";
        //                }
        //                if (buyback_charges == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + buyback_charges + "',";
        //                }
        //                if (packing_charges == "")
        //                {
        //                    msSQL += "'0.00',";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + packing_charges + "',";
        //                }
        //                if (insurance_charges == "")
        //                {
        //                    msSQL += "'0.00')";
        //                }
        //                else
        //                {
        //                    msSQL += "'" + insurance_charges + "')";
        //                }
        //                mnResult2 = objdbconn.ExecuteNonQuerySQL(msSQL);


        //            }

        //                msSQL = " select * from smr_tmp_tsalesorderdtl " +
        //                        " where employee_gid='" + employee_gid + "'";
        //                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

        //            msSQL = "delete from smr_tmp_tsalesorderdtldrafts where salesorder_gid = '" + salesorder_gid + "'";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //            if (mnResult == 1)
        //            {
        //                if (dt_datatable.Rows.Count != 0)
        //                {


        //                    msSQL = " select " +
        //                            " tmpsalesorderdtl_gid," +
        //                            " salesorder_gid," +
        //                            " product_gid," +
        //                            " productgroup_gid," +
        //                            " product_remarks," +
        //                            " product_name," +
        //                             " product_code," +
        //                            " product_price," +
        //                            " qty_quoted," +
        //                            " discount_percentage," +
        //                            " discount_amount," +
        //                            " uom_gid," +
        //                            " uom_name," +
        //                            " price," +
        //                            " tax_name," +
        //                            " tax1_gid, " +
        //                            " tax_amount," +
        //                             " tax_name2," +
        //                            " tax2_gid, " +
        //                            " tax_amount2," +
        //                             " tax_percentage2," +
        //                            " slno," +
        //                            " tax_percentage," +
        //                            " order_type, " +
        //                            " taxsegment_gid, " +
        //                            " taxsegmenttax_gid " +
        //                            " from smr_tmp_tsalesorderdtl" +
        //                            " where employee_gid='" + employee_gid + "' ";
        //                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
        //                    var getModuleList = new List<postsales_list>();
        //                    if (dt_datatable.Rows.Count != 0)
        //                    {
        //                        foreach (DataRow dt in  dt_datatable.Rows)
        //                        {
        //                            getModuleList.Add(new postsales_list
        //                            {
        //                                salesorder_gid = dt["salesorder_gid"].ToString(),
        //                                tmpsalesorderdtl_gid = dt["tmpsalesorderdtl_gid"].ToString(),
        //                                product_gid = dt["product_gid"].ToString(),
        //                                product_name = dt["product_name"].ToString(),
        //                                product_code = dt["product_code"].ToString(),
        //                                productuom_name = dt["uom_name"].ToString(),
        //                                productgroup_gid = dt["productgroup_gid"].ToString(),
        //                                product_remarks = dt["product_remarks"].ToString(),
        //                                unitprice = dt["product_price"].ToString(),
        //                                quantity = dt["qty_quoted"].ToString(),
        //                                discountpercentage = dt["discount_percentage"].ToString(),
        //                                discountamount = dt["discount_amount"].ToString(),
        //                                tax_name = dt["tax_name"].ToString(),
        //                                tax_amount = dt["tax_amount"].ToString(),
        //                                totalamount = dt["price"].ToString(),
        //                                order_type = dt["order_type"].ToString(),
        //                                slno = dt["slno"].ToString(),
        //                                taxsegment_gid = dt["taxsegment_gid"].ToString(),
        //                                taxsegmenttax_gid = dt["taxsegmenttax_gid"].ToString(),
        //                            });

        //                            int i = 0;

        //                            //mssalesorderGID1 = objcmnfunctions.GetMasterGID("VSDC");
        //                            //if (mssalesorderGID1 == "E")
        //                            //{
        //                            //    values.message = "Create Sequence code for VSDC ";
        //                            //    return;
        //                            //}



        //                            msSQL = " insert into smr_tmp_tsalesorderdtldrafts (" +
        //                                 " tmpsalesorderdtl_gid ," +
        //                                 " salesorder_gid," +
        //                                 " product_gid ," +
        //                                 " product_name," +
        //                                 " product_code," +
        //                                 " product_price," +
        //                                 " productgroup_gid," +
        //                                 " product_remarks," +
        //                                 " display_field," +
        //                                 " qty_quoted," +
        //                                 " discount_percentage," +
        //                                 " discount_amount," +
        //                                 " tax_amount ," +
        //                                 " uom_gid," +
        //                                 " uom_name," +
        //                                 " price," +
        //                                 " tax_name," +
        //                                 " tax1_gid," +
        //                                  " tax_name2," +
        //                                 " tax2_gid," +
        //                                  " tax_percentage2," +
        //                                   " tax_amount2," +
        //                                 " slno," +
        //                                 " tax_percentage," +
        //                                 " taxsegment_gid," +
        //                                 " taxsegmenttax_gid," +
        //                                 " type " +
        //                                 ")values(" +
        //                                 " '" + dt["tmpsalesorderdtl_gid"].ToString() + "'," +
        //                                 " '" + mssalesorderGID + "'," +
        //                                 " '" + dt["product_gid"].ToString() + "'," +
        //                                 " '" + dt["product_name"].ToString() + "'," +
        //                                 " '" + dt["product_code"].ToString() + "'," +
        //                                 " '" + dt["product_price"].ToString() + "'," +
        //                                 " '" + dt["productgroup_gid"].ToString() + "'," +
        //                                 " '" + dt["product_remarks"].ToString() + "'," +
        //                                  " '" + dt["product_remarks"].ToString() + "'," +
        //                                 " '" + dt["qty_quoted"].ToString() + "'," +
        //                                 " '" + dt["discount_percentage"].ToString() + "'," +
        //                                 " '" + dt["discount_amount"].ToString() + "'," +
        //                                 " '" + dt["tax_amount"].ToString() + "'," +
        //                                 " '" + dt["uom_gid"].ToString() + "'," +
        //                                 " '" + dt["uom_name"].ToString() + "'," +
        //                                 " '" + dt["price"].ToString() + "'," +
        //                                 " '" + dt["tax_name"].ToString() + "'," +
        //                                 " '" + dt["tax1_gid"].ToString() + "'," +
        //                                   " '" + dt["tax_name2"].ToString() + "'," +
        //                                     " '" + dt["tax2_gid"].ToString() + "'," +
        //                                       " '" + dt["tax_percentage2"].ToString() + "'," +
        //                                         " '" + dt["tax_amount2"].ToString() + "'," +
        //                                 " '" + i + 1 + "'," +
        //                                 " '" + dt["tax_percentage"].ToString() + "'," +
        //                                 " '" + dt["taxsegment_gid"].ToString() + "'," +
        //                                 " '" + dt["taxsegmenttax_gid"].ToString() + "'," +
        //                                 " '" + dt["order_type"].ToString() + "')";
        //                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                            if (mnResult == 0)
        //                            {
        //                                objResult.status = false;
        //                                objResult.message = "Error occurred while Insertion";
        //                                return;
        //                            }


        //                            msSQL = " insert into acp_trn_torderdtl (" +
        //                             " salesorderdtl_gid ," +
        //                             " salesorder_gid," +
        //                             " product_gid ," +
        //                             " product_name," +
        //                             " product_price," +
        //                             " qty_quoted," +
        //                             " discount_percentage," +
        //                             " discount_amount," +
        //                             " tax_amount ," +
        //                             " uom_gid," +
        //                             " uom_name," +
        //                             " price," +
        //                             " tax_name," +
        //                             " tax1_gid," +
        //                             " slno," +
        //                             " tax_percentage," +
        //                             " taxsegment_gid," +
        //                             " type, " +
        //                             " salesorder_refno" +
        //                             ")values(" +
        //                             " '" + dt["tmpsalesorderdtl_gid"].ToString() + "'," +
        //                             " '" + mssalesorderGID + "'," +
        //                             " '" + dt["product_gid"].ToString() + "'," +
        //                             " '" + dt["product_name"].ToString() + "'," +
        //                             " '" + dt["product_price"].ToString() + "'," +
        //                             " '" + dt["qty_quoted"].ToString() + "'," +
        //                             " '" + dt["discount_percentage"].ToString() + "'," +
        //                             " '" + dt["discount_amount"].ToString() + "'," +
        //                             " '" + dt["tax_amount"].ToString() + "'," +
        //                             " '" + dt["uom_gid"].ToString() + "'," +
        //                             " '" + dt["uom_name"].ToString() + "'," +
        //                             " '" + dt["price"].ToString() + "'," +
        //                             " '" + dt["tax_name"].ToString() + "'," +
        //                              " '" + dt["tax1_gid"].ToString() + "'," +
        //                             " '" + i + 1 + "'," +
        //                             " '" + dt["tax_percentage"].ToString() + "'," +
        //                             " '" + dt["taxsegment_gid"].ToString() + "'," +
        //                             " '" + dt["order_type"].ToString() + "', " +
        //                             " '" + so_referencenumber + "')";
        //                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                        }
        //                    }
        //                }
        //            }





        //            msSQL = "select distinct type from smr_trn_tsalesorderdtl where salesorder_gid='" + mssalesorderGID + "' ";
        //            objodbcDataReader = objdbconn.GetDataReader(msSQL);

        //            if (objodbcDataReader.HasRows == true)
        //            {
        //                if (objodbcDataReader["type"].ToString()!="Services")
        //                { 
        //                lsorder_type = "Sales";
        //                }
        //                else
        //                {
        //                    lsorder_type = "Services";
        //                }

        //            }







        //            msSQL = " update smr_tmp_tsalesorderdrafts set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //            msSQL = " update acp_trn_torder set so_type='" + lsorder_type + "' where salesorder_gid='" + mssalesorderGID + "' ";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);


        //        }
        //        else
        //        {


        //            if (!customer_gid.Contains("BCRM"))
        //            {
        //                string customerName = customer_gid;
        //                string[] cusname = customerName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        //                string name = cusname[0].Trim();

        //                msSQL = " select customer_gid from crm_mst_tcustomer where customer_name='" + name + " '";
        //                 lscustomer_gid = objdbconn.GetExecuteScalar(msSQL);
        //            }



        //            msSQL = "UPDATE smr_tmp_tsalesorderdrafts SET " +
        //                     " branch_gid = '" + branch_name + "', " +
        //                     " salesorder_date = '" + salesorder_date1 + "', " +
        //                     " customer_gid = '" + lscustomer_gid + "', " +
        //                     " customer_name = '" + customer_gid.Replace("'","\\\'") + "', " +
        //                     " customer_address = '" + address1.Replace("'", "\\\'") + "', " +
        //                     " bill_to = '" + address1.Replace("'", "\\\'") + "', " +
        //                     " created_by = '" + employee_gid + "', " +
        //                     // " so_referenceno1 = '" + (values.so_referencenumber ?? lsrefno) + "', " +
        //                     " so_remarks = '" + so_remarks + "', " +
        //                     " payment_days = '" + payment_days + "', " +
        //                     " delivery_days = '" + delivery_days + "', " +
        //                     " Grandtotal = '" + grandtotal.Replace(",", "").Trim() + "', " +
        //                     " termsandconditions = '" + termsandconditions.Replace("'", "\\\'") + "', " +
        //                     " salesorder_status = 'Approved', ";

        //            if (addon_charge != "")
        //            {
        //                msSQL += " addon_charge = '" + addon_charge + "', ";
        //            }
        //            else
        //            {
        //                msSQL += " addon_charge = '" + lslocaladdon + "', ";
        //            }

        //            if (additional_discount != "")
        //            {
        //                msSQL += " additional_discount = '" + additional_discount + "', ";
        //            }
        //            else
        //            {
        //                msSQL += " additional_discount = '" + lslocaladditionaldiscount + "', ";
        //            }

        //            msSQL += " grandtotal_l = '" + lslocalgrandtotal + "', " +
        //                     " currency_code = '" + currency_code1 + "', " +
        //                     " currency_gid = '" + currency_code + "', " +
        //                     " exchange_rate = '" + exchange_rate + "', " +
        //                     " file_path = '" + final_path + "', " +
        //                     " shipping_to = '" + shipping_address.Replace("'", "\\\'") + "', " +
        //                     " tax_gid = '" + tax_name4 + "', " +
        //                     " tax_name = '" + lstaxname1 + "', " +
        //                     " gst_amount = '" + lsgst + "', " +
        //                     " total_price = '" + totalamount.Replace(",", "").Trim() + "', ";

        //            if (grandtotal == null || grandtotal == "")
        //            {
        //                msSQL += " total_amount = '0.00', ";
        //            }
        //            else
        //            {
        //                msSQL += " total_amount = '" + grandtotal.Replace(",", "").Trim() + "', ";
        //            }

        //            if (tax_amount4 != "" && tax_amount4 != null)
        //            {
        //                msSQL += " tax_amount = '" + tax_amount4 + "', ";
        //            }
        //            else
        //            {
        //                msSQL += " tax_amount = '" + lsamount4 + "', ";
        //            }

        //            msSQL += " vessel_name = '" + vessel_name + "', " +
        //                     " salesperson_gid = '" + user_name + "', ";

        //            if (roundoff == "")
        //            {
        //                msSQL += " roundoff = '0.00', ";
        //            }
        //            else
        //            {
        //                msSQL += " roundoff = '" + roundoff + "', ";
        //            }


        //            msSQL += " updated_addon_charge = '" + (addon_charge == "" ? "0.00" : addon_charge) + "', " +
        //                     " updated_additional_discount = '" + (additional_discount == "" ? "0.00" : additional_discount) + "', " +
        //                     " freight_charges = '" + (freight_charges == "" ? "0.00" : freight_charges) + "', " +
        //                     " source_flag = 'I', " +
        //                     " renewal_flag = '" + renewal_mode + "', " +
        //                     " file_name = '" + FileExtensionname + "', " +
        //                     " created_date = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
        //            msSQL += "WHERE salesorder_gid = '" + salesorder_gid + "'";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //            msSQL = " select * from smr_tmp_tsalesorderdtl " +
        //                       " where employee_gid='" + employee_gid + "'";
        //            DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

        //            msSQL = "delete from smr_tmp_tsalesorderdtldrafts where salesorder_gid = '" + salesorder_gid + "'";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //            if (mnResult == 1)
        //            {
        //                if (dt_datatable.Rows.Count != 0)
        //                {


        //                    msSQL = " select " +
        //                            " tmpsalesorderdtl_gid," +
        //                            " salesorder_gid," +
        //                            " product_gid," +
        //                            " productgroup_gid," +
        //                            " product_remarks," +
        //                            " product_name," +
        //                             " product_code," +
        //                            " product_price," +
        //                            " qty_quoted," +
        //                            " discount_percentage," +
        //                            " discount_amount," +
        //                            " uom_gid," +
        //                            " uom_name," +
        //                            " price," +
        //                            " tax_name," +
        //                            " tax1_gid, " +
        //                            " tax_amount," +
        //                             " tax_name2," +
        //                            " tax2_gid, " +
        //                            " tax_amount2," +
        //                             " tax_percentage2," +
        //                            " slno," +
        //                            " tax_percentage," +
        //                            " order_type, " +
        //                            " taxsegment_gid, " +
        //                            " taxsegmenttax_gid " +
        //                            " from smr_tmp_tsalesorderdtl" +
        //                            " where employee_gid='" + employee_gid + "' ";
        //                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
        //                    var getModuleList = new List<postsales_list>();
        //                    if (dt_datatable.Rows.Count != 0)
        //                    {
        //                        foreach (DataRow dt in  dt_datatable.Rows)
        //                        {
        //                            getModuleList.Add(new postsales_list
        //                            {
        //                                salesorder_gid = dt["salesorder_gid"].ToString(),
        //                                tmpsalesorderdtl_gid = dt["tmpsalesorderdtl_gid"].ToString(),
        //                                product_gid = dt["product_gid"].ToString(),
        //                                product_name = dt["product_name"].ToString(),
        //                                product_code = dt["product_code"].ToString(),
        //                                productuom_name = dt["uom_name"].ToString(),
        //                                productgroup_gid = dt["productgroup_gid"].ToString(),
        //                                product_remarks = dt["product_remarks"].ToString(),
        //                                unitprice = dt["product_price"].ToString(),
        //                                quantity = dt["qty_quoted"].ToString(),
        //                                discountpercentage = dt["discount_percentage"].ToString(),
        //                                discountamount = dt["discount_amount"].ToString(),
        //                                tax_name = dt["tax_name"].ToString(),
        //                                tax_amount = dt["tax_amount"].ToString(),
        //                                totalamount = dt["price"].ToString(),
        //                                order_type = dt["order_type"].ToString(),
        //                                slno = dt["slno"].ToString(),
        //                                taxsegment_gid = dt["taxsegment_gid"].ToString(),
        //                                taxsegmenttax_gid = dt["taxsegmenttax_gid"].ToString(),
        //                            });

        //                            int i = 0;

        //                            //mssalesorderGID1 = objcmnfunctions.GetMasterGID("VSDC");
        //                            //if (mssalesorderGID1 == "E")
        //                            //{
        //                            //    values.message = "Create Sequence code for VSDC ";
        //                            //    return;
        //                            //}



        //                            msSQL = " insert into smr_tmp_tsalesorderdtldrafts (" +
        //                                 " tmpsalesorderdtl_gid ," +
        //                                 " salesorder_gid," +
        //                                 " product_gid ," +
        //                                 " product_name," +
        //                                 " product_code," +
        //                                 " product_price," +
        //                                 " productgroup_gid," +
        //                                 " product_remarks," +
        //                                 " display_field," +
        //                                 " qty_quoted," +
        //                                 " discount_percentage," +
        //                                 " discount_amount," +
        //                                 " tax_amount ," +
        //                                 " uom_gid," +
        //                                 " uom_name," +
        //                                 " price," +
        //                                 " tax_name," +
        //                                 " tax1_gid," +
        //                                  " tax_name2," +
        //                                 " tax2_gid," +
        //                                  " tax_percentage2," +
        //                                   " tax_amount2," +
        //                                 " slno," +
        //                                 " tax_percentage," +
        //                                 " taxsegment_gid," +
        //                                 " taxsegmenttax_gid," +
        //                                 " type " +
        //                                 ")values(" +
        //                                 " '" + dt["tmpsalesorderdtl_gid"].ToString() + "'," +
        //                                 " '" + salesorder_gid + "'," +
        //                                 " '" + dt["product_gid"].ToString() + "'," +
        //                                 " '" + dt["product_name"].ToString() + "'," +
        //                                 " '" + dt["product_code"].ToString() + "'," +
        //                                 " '" + dt["product_price"].ToString() + "'," +
        //                                 " '" + dt["productgroup_gid"].ToString() + "'," +
        //                                 " '" + dt["product_remarks"].ToString() + "'," +
        //                                  " '" + dt["product_remarks"].ToString() + "'," +
        //                                 " '" + dt["qty_quoted"].ToString() + "'," +
        //                                 " '" + dt["discount_percentage"].ToString() + "'," +
        //                                 " '" + dt["discount_amount"].ToString() + "'," +
        //                                 " '" + dt["tax_amount"].ToString() + "'," +
        //                                 " '" + dt["uom_gid"].ToString() + "'," +
        //                                 " '" + dt["uom_name"].ToString() + "'," +
        //                                 " '" + dt["price"].ToString() + "'," +
        //                                 " '" + dt["tax_name"].ToString() + "'," +
        //                                 " '" + dt["tax1_gid"].ToString() + "'," +
        //                                   " '" + dt["tax_name2"].ToString() + "'," +
        //                                     " '" + dt["tax2_gid"].ToString() + "'," +
        //                                       " '" + dt["tax_percentage2"].ToString() + "'," +
        //                                         " '" + dt["tax_amount2"].ToString() + "'," +
        //                                 " '" + i + 1 + "'," +
        //                                 " '" + dt["tax_percentage"].ToString() + "'," +
        //                                 " '" + dt["taxsegment_gid"].ToString() + "'," +
        //                                 " '" + dt["taxsegmenttax_gid"].ToString() + "'," +
        //                                 " '" + dt["order_type"].ToString() + "')";
        //                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //                            if (mnResult == 0)
        //                            {
        //                                objResult.status = false;
        //                                objResult.message = "Error occurred while Insertion";
        //                                return;
        //                            }


        //                            msSQL = " insert into acp_trn_torderdtl (" +
        //                             " salesorderdtl_gid ," +
        //                             " salesorder_gid," +
        //                             " product_gid ," +
        //                             " product_name," +
        //                             " product_price," +
        //                             " qty_quoted," +
        //                             " discount_percentage," +
        //                             " discount_amount," +
        //                             " tax_amount ," +
        //                             " uom_gid," +
        //                             " uom_name," +
        //                             " price," +
        //                             " tax_name," +
        //                             " tax1_gid," +
        //                             " slno," +
        //                             " tax_percentage," +
        //                             " taxsegment_gid," +
        //                             " type, " +
        //                             " salesorder_refno" +
        //                             ")values(" +
        //                             " '" + dt["tmpsalesorderdtl_gid"].ToString() + "'," +
        //                             " '" + salesorder_gid + "'," +
        //                             " '" + dt["product_gid"].ToString() + "'," +
        //                             " '" + dt["product_name"].ToString() + "'," +
        //                             " '" + dt["product_price"].ToString() + "'," +
        //                             " '" + dt["qty_quoted"].ToString() + "'," +
        //                             " '" + dt["discount_percentage"].ToString() + "'," +
        //                             " '" + dt["discount_amount"].ToString() + "'," +
        //                             " '" + dt["tax_amount"].ToString() + "'," +
        //                             " '" + dt["uom_gid"].ToString() + "'," +
        //                             " '" + dt["uom_name"].ToString() + "'," +
        //                             " '" + dt["price"].ToString() + "'," +
        //                             " '" + dt["tax_name"].ToString() + "'," +
        //                              " '" + dt["tax1_gid"].ToString() + "'," +
        //                             " '" + i + 1 + "'," +
        //                             " '" + dt["tax_percentage"].ToString() + "'," +
        //                             " '" + dt["taxsegment_gid"].ToString() + "'," +
        //                             " '" + dt["order_type"].ToString() + "', " +
        //                             " '" + so_referencenumber + "')";
        //                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //                        }
        //                    }
        //                }
        //            }





        //            msSQL = "select distinct type from smr_trn_tsalesorderdtl where salesorder_gid='" + salesorder_gid + "' ";
        //            objodbcDataReader = objdbconn.GetDataReader(msSQL);

        //            if (objodbcDataReader.HasRows == true)
        //            {
        //                if (objodbcDataReader["type"].ToString() != "Services")
        //                {
        //                    lsorder_type = "Sales";
        //                }
        //                else
        //                {
        //                    lsorder_type = "Services";
        //                }

        //            }







        //            msSQL = " update smr_tmp_tsalesorderdrafts set so_type='" + lsorder_type + "' where salesorder_gid='" + salesorder_gid + "' ";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
        //            msSQL = " update acp_trn_torder set so_type='" + lsorder_type + "' where salesorder_gid='" + salesorder_gid + "' ";
        //            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

        //        }
        //        if (mnResult != 0)
        //        {
        //            objResult.status = true;
        //            objResult.message = "Sales order draft has been saved successfully";
        //            return;
        //        }
        //        else
        //        {
        //            objResult.status = false;
        //            objResult.message = "Error While Saving Sales Order Draft";
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objResult.message = "Exception occured while Submitting Sales Order !";
        //        objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
        //        $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
        //        objResult.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
        //    }


        //}
        public void DaGetSalesDtl(MdlSmrTrnSalesorder values)
        {
            try
            {

                msSQL = "select user_gid, user_firstname from adm_mst_tuser";


                dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetSalesDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
                    {
                        getModuleList.Add(new GetSalesDropdown

                        {
                            user_gid = dt["user_gid"].ToString(),
                            user_name = dt["user_firstname"].ToString(),

                        });
                        values.GetSalesDropdown = getModuleList;
                    }
                }
                dt_datatable.Dispose();
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while loading User Name !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");

            }

        }
        public Dictionary<string, object> DaGetCustomerStatementPDF(string? customer_gid, MdlSmrTrnSalesorder values)
        {
            var ls_response = new Dictionary<string, object>();
            try
            {
                OdbcConnection myConnection = new OdbcConnection();
                myConnection.ConnectionString = objdbconn.GetConnectionString("");
                OdbcCommand MyCommand = new OdbcCommand();
                MyCommand.Connection = myConnection;
                DataSet myDS = new DataSet();
                OdbcDataAdapter MyDA = new OdbcDataAdapter();

                msSQL = " select customer_name from crm_mst_tcustomer where customer_gid='" + customer_gid + "'";
                string customet_name = objdbconn.GetExecuteScalar(msSQL);

                msSQL = "Select distinct a.invoice_gid,concat('Sales Invoice',' ',a.invoice_refno) as invoice_refno,a.invoice_status,a.invoice_flag,a.invoice_date,a.invoice_type,c.branch_name, " +
                    " FORMAT(a.advance_amount,2) as advance_amount,invoice_from, e.address1, e.city, e.zip_code, e.customercontact_name," +
                    " ifnull(cast(payment_amount * exchange_rate as decimal),0.00) as payment_amount,a.payment_date, " +
                    " CASE WHEN a.payment_flag <>'PY Pending' then a.payment_flag else invoice_flag END as 'overall_status',  " +
                    " concat(case when a.payment_date < now() then DATEDIFF(NOW(), a.payment_date) end,' days') AS expiry_days," +
                    " DATE_FORMAT(a.payment_date,'%d-%m-%Y') as due_date, d.customer_name," +
                    " ifnull(cast(a.invoice_amount*a.exchange_rate as decimal),0.00) as invoice_amount,invoice_date,  " +
                    " CONCAT(a.customer_name,'/',a.customer_contactperson,'/',a.customer_contactnumber) as companydetails, " +
                    " a.invoice_from,ifnull(cast((a.invoice_amount*a.exchange_rate) - (a.payment_amount*a.exchange_rate) as decimal),0.00) as outstanding_amount  " +
                    " from rbl_trn_tinvoice a  " +
                    " left join rbl_trn_tinvoicedtl b on a.invoice_gid = b.invoice_gid  " +
                    " left join hrm_mst_tbranch c on a.branch_gid=c.branch_gid " +
                    " left join crm_mst_tcustomer d on d.customer_gid = a.customer_gid" +
                    " left join crm_mst_tcustomercontact e on e.customer_gid= d.customer_gid" +
                    " where 1=1 and a.invoice_amount  <> a.payment_amount and a.invoice_status <> 'Invoice Cancelled' and a.invoice_flag <> 'Invoice Cancelled' " +
                    " and a.customer_gid='" + customer_gid + "' " +
                    " group by a.invoice_gid  order by date(invoice_date) desc,invoice_date asc, invoice_gid desc ";
                MyCommand.CommandText = msSQL;
                MyCommand.CommandType = System.Data.CommandType.Text;
                MyDA.SelectCommand = MyCommand;
                myDS.EnforceConstraints = false;
                MyDA.Fill(myDS, "DataTable1");

                msSQL = "select sum(case when datediff(now(), a.payment_date) <= 30 then a.invoice_amount  ELSE 0 END) AS 'up_to_30_days', " +
                    " sum(case when datediff(now(), a.payment_date) BETWEEN 61 AND 90  then a.invoice_amount else 0 end) as '61_to_90_days', " +
                    " sum(case when datediff(now(), a.payment_date) BETWEEN 31 AND 60  then a.invoice_amount else 0 end) as '31_to_60_days', " +
                    " sum(case when datediff(now(), a.payment_date) > 90 then b.amount else 0 end) as 'more_than_90_days', " +
                    " CONCAT('£ ', CAST(FORMAT(SUM(a.invoice_amount * a.exchange_rate), 2) AS CHAR), ' ', a.currency_code) AS DataColumn5, " +
                    " CONCAT('£ ', CAST(FORMAT(SUM((a.invoice_amount * a.exchange_rate) - COALESCE(b.amount, 0)), 2) AS CHAR), ' ', a.currency_code) AS DataColumn6  from " +
                    " rbl_trn_tinvoice a \r\n left join rbl_trn_tpayment b on a.invoice_gid=b.invoice_gid " +
                    " where a.customer_gid='" + customer_gid + "'";
                MyCommand.CommandText = msSQL;
                MyCommand.CommandType = System.Data.CommandType.Text;
                MyDA.SelectCommand = MyCommand;
                myDS.EnforceConstraints = false;
                MyDA.Fill(myDS, "DataTable2");

                msSQL = " select template_content as notes from adm_mst_ttemplate where templatetype_gid='3'";
                MyCommand.CommandText = msSQL;
                MyCommand.CommandType = System.Data.CommandType.Text;
                MyDA.SelectCommand = MyCommand;
                myDS.EnforceConstraints = false;
                MyDA.Fill(myDS, "DataTable3");

                msSQL = " select 'Receipt' as invoice_ref , concat('- ',a.amount) as amount, a.payment_date as DataColumn3  from rbl_trn_tpayment a " +
                    " left join rbl_trn_tinvoice b on a.invoice_gid=b.invoice_gid" +
                    " where b.customer_gid ='" + customer_gid + "'";
                MyCommand.CommandText = msSQL;
                MyCommand.CommandType = System.Data.CommandType.Text;
                MyDA.SelectCommand = MyCommand;
                myDS.EnforceConstraints = false;
                MyDA.Fill(myDS, "DataTable4");

                try
                {
                    ReportDocument oRpt = new ReportDocument();
                    string base_pathOF_currentFILE = AppDomain.CurrentDomain.BaseDirectory;
                    string report_path = Path.Combine(base_pathOF_currentFILE, "ems.sales", "Reports", "smr_crp_customerstatement.rpt");

                    if (!File.Exists(report_path))
                    {
                        values.status = false;
                        values.message = "Your Rpt path not found !!";
                        ls_response = new Dictionary<string, object>
                        {
                            {"status",false },
                            {"message",values.message}
                        };
                    }
                    oRpt.Load(report_path);
                    oRpt.SetDataSource(myDS);

                    string? path = Path.Combine(objconfigurations["report_path"].ToString());

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string PDFfile_name = customet_name + " Statement.pdf";
                    full_path = Path.Combine(path, PDFfile_name);

                    oRpt.ExportToDisk(ExportFormatType.PortableDocFormat, full_path);
                    myConnection.Close();
                    ls_response = objFnazurestorage.reportStreamDownload(full_path);
                    values.status = true;
                }
                catch (Exception Ex)
                {
                    values.status = false;
                    values.message = Ex.Message;
                    ls_response = new Dictionary<string, object>
                    {
                        { "status", false },
                        { "message", Ex.Message }
                    };
                }
                finally
                {
                    if (full_path != null)
                    {
                        try
                        {
                            File.Delete(full_path);
                        }
                        catch (Exception Ex)
                        {
                            values.message = Ex.Message;
                            ls_response = new Dictionary<string, object>
                            {
                                { "status", false },
                                { "message", Ex.Message }
                            };
                        }
                    }
                }
                return ls_response;
            }
            catch (Exception ex)
            {
                values.message = ex.Message;
                ls_response = new Dictionary<string, object>
                {
                   { "status", false },
                   { "message", ex.Message }
                };
                return ls_response;
            }
        }
    }
}

