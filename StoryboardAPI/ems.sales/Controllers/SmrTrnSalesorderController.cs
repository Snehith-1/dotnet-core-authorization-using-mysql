using ems.sales.Models;
using ems.utilities.Functions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Odbc;
using System.Globalization;

namespace ems.sales.Controllers
{
  [Route("api/SmrTrnSalesorder")]
    [ApiController]
    public class SmrTrnSalesorderController : ControllerBase
    {
        string? msSQL, lscustomer_gid, FileExtensionname, FileExtension, lscustomername, lsrefno,
        mssalesorderGID, msINGetGID, lsorder_type, msgetlead2campaign_gid, msGetGid, msGetGID,
        employee_gid, lscompany_code, customergid, lsproductgid, lsproduct_name, lsproductuom_name, lsproductuom_gid
        , lsdiscountamount, taxgid1, full_path;
        double lsdiscountpercentage;
        int mnResult, mnResult2;
        DataTable? dt_datatable;
        OdbcDataReader? objodbcDataReader, objodbcdatareader1;
        private readonly dbconn objdbconn;
        private readonly cmnfunctions objcmnfunctions;
        private readonly DataAccess.DaSmrTrnSalesorder objsales;
        private readonly IHttpContextAccessor objHttpContext;
        private readonly IConfiguration objconfiguration;
        private readonly Fnazurestorage objFnazurestorage;
        public SmrTrnSalesorderController(dbconn dbconn, cmnfunctions cmnfunctions,
            DataAccess.DaSmrTrnSalesorder objgetgid,
            IHttpContextAccessor HttpContext,
            IConfiguration configuration, Fnazurestorage Fnazurestorage)
        {
            objdbconn = dbconn;
            objcmnfunctions = cmnfunctions;
            objsales = objgetgid;
            objHttpContext = HttpContext;
            objconfiguration = configuration;
            objFnazurestorage = Fnazurestorage;
        }
        [HttpGet("GetCustomerStatement")]
        public IActionResult GetCustomerStatement([FromRoute] string? customer_gid)
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
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
                    //ReportDocument oRpt = new ReportDocument();
                    //string base_pathOF_currentFILE = AppDomain.CurrentDomain.BaseDirectory;
                    //string report_path = Path.Combine(base_pathOF_currentFILE, "ems.sales", "Reports", "smr_crp_customerstatement.rpt");

                    //oRpt.Load(report_path);
                    //oRpt.SetDataSource(myDS);

                    //string? path = Path.Combine(objconfiguration["report_path"].ToString());

                    //if (!Directory.Exists(path))
                    //{
                    //    Directory.CreateDirectory(path);
                    //}

                    //string PDFfile_name = customet_name + " Statement.pdf";
                    //full_path = Path.Combine(path, PDFfile_name);

                    //oRpt.ExportToDisk(ExportFormatType.PortableDocFormat, full_path);
                    //myConnection.Close();
                    //ls_response = objFnazurestorage.reportStreamDownload(full_path);
                    //values.status = true;
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

                    }
                }
                return Ok(ls_response);
            }
            catch (Exception ex)
            {
                values.message = ex.Message;
                ls_response = new Dictionary<string, object>
                {
                   { "status", false },
                   { "message", ex.Message }
                };
                return Ok(ls_response);
            }
        }

        [HttpGet("GetSmrTrnSalesordersummary")]
        public IActionResult GetSmrTrnSalesordersummary()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            try
            {
                msSQL = " select currency_code from crm_trn_tcurrencyexchange where default_currency='Y'";
                string? currency = objdbconn.GetExecuteScalar(msSQL);

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
                dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<salesorder_list>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
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
                // values.message = "Exception occured while loading Sales Order Summary !";
                //// objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString()?("yyyy - MM - dd HH: mm:ss") + "***********" +
                // $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                //values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString()?("yyyy-MM-dd HH") + ".txt");
            }
            return Ok(values);
        }
        [HttpGet("Getsalesordersixmonthschart")]
        public IActionResult Getsalesordersixmonthschart()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();

            msSQL = " select DATE_FORMAT(salesorder_date, '%b-%Y')  as salesorder_date,substring(date_format(a.salesorder_date,'%M'),1,3)as month,a.salesorder_gid,year(a.salesorder_date) as year, " +
            " format(round(sum(a.grandtotal),2),2)as amount,round(sum(a.grandtotal),2) as amount1,count(a.salesorder_gid)as ordercount ,  date_format(salesorder_date,'%M/%Y') as month_wise " +
            " from smr_trn_tsalesorder a   " +
            " where a.salesorder_date > date_add(now(), interval-6 month) and a.salesorder_date<=date(now())   " +
            " and a.salesorder_status not in('SO Amended','Cancelled','Rejected') group by date_format(a.salesorder_date,'%M') order by a.salesorder_date desc  ";
            dt_datatable = objdbconn.GetDataTable(msSQL);
            var salesorderlastsixmonths_list = new List<salesorderlastsixmonths_list>();
            if (dt_datatable.Rows.Count != 0)
            {
                foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetProductsearchSummarySales")]
        public IActionResult GetProductsearchSummarySales()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            try
            {
                string? lsSqlType = "product";

                msSQL = " call pmr_mst_spproductsearch('" + lsSqlType + "','','')";
                dt_datatable = objdbconn.GetDataTable(msSQL);

                var getModuleList = new List<GetProductsearchs>();
                var allTaxSegmentsList = new List<GetTaxSegmentListorder>();

                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
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
                    }
                    values.GetProductsearchs = getModuleList; // Assign GetProductsearch to values.GetProductsearch
                }
                //values.GetTaxSegmentListorder = allTaxSegmentsList; // Assign allTaxSegmentsList to values.GetTaxSegmentList
            }
            catch (Exception ex)
            {
                //values.message = "Exception occurred while changing product!";
                //objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString()?("yyyy - MM - dd HH: mm:ss") + "***********" +
                //  $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" +
                //  ex.Message.ToString() + "***********" + values.message.ToString() + "*****Query****" +
                //  msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" +
                //  DateTime.Now.ToString()?("yyyy-MM-dd HH") + ".txt");
            }
            return Ok(values);
        }
        [HttpGet("GetBranchDtl")]
        public IActionResult GetBranchDtl()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            try
            {
                msSQL = "select branch_gid,branch_name, address1 from hrm_mst_tbranch";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetBranchDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetCurrencyDtl")]
        public IActionResult GetCurrencyDtl()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            try
            {
                msSQL = "  select currencyexchange_gid,currency_code,default_currency,exchange_rate" +
                    " from crm_trn_tcurrencyexchange order by currency_code asc";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                var getModuleList = new List<GetCurrencyDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("ProductSalesSummary")]
        public IActionResult ProductSalesSummary(string? customer_gid, string? product_gid)
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            string? token = Request.Headers["Authorization"].FirstOrDefault();
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
                    foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetCustomerDtl")]
        public IActionResult GetCustomerDtl()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            try
            {
                msSQL = " select concat(a.customer_name,' / ', b.email) as customer_name ,a.customer_gid from crm_mst_tcustomer a " +
                        " left join crm_mst_tcustomercontact b on a.customer_gid=b.customer_gid " +
                        " where status='Active'";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetCustomerDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetPersonDtl")]
        public IActionResult GetPersonDtl()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
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
                    foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("Getproducttypesales")]
        public IActionResult Getproducttypesales()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            try
            {

                msSQL = " select producttype_gid, producttype_code , producttype_name from pmr_mst_tproducttype";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<Getproducttypesales>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetProductGroup")]
        public IActionResult GetProductGroup()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            msSQL = " select productgroup_gid, productgroup_name from pmr_mst_tproductgroup";
            DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
            var getModuleList = new List<Getproductgroup>();
            if (dt_datatable.Rows.Count != 0)
            {
                foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetProductNamDtl")]
        public IActionResult GetProductNamDtl()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            try
            {
                msSQL = "Select product_gid, product_name from pmr_mst_tproduct where status='1'";
                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetProductNamDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetTax4Dtl")]
        public IActionResult GetTax4Dtl()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            try
            {
                msSQL = " select tax_prefix,tax_gid,percentage from acp_mst_ttax where active_flag='Y'  and reference_type='Customer'";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetTaxFourDropdown>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetOnChangeCustomer")]
        public IActionResult GetOnChangeCustomer(string? customer_gid)
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
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
                        foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetOnChangeBranch")]
        public IActionResult GetOnChangeBranch(string? branch_gid)
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            msSQL = "select address1,branch_gid,branch_name from hrm_mst_tbranch where branch_gid = '" + branch_gid + "'";
            DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
            var GetOnChangeBranch = new List<GetOnChangeBranch_list>();
            if (dt_datatable.Rows.Count > 0)
            {
                foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetOnChangeCurrency")]
        public IActionResult GetOnChangeCurrency(string? currencyexchange_gid)
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            try
            {
                msSQL = " select currencyexchange_gid,currency_code,exchange_rate from crm_trn_tcurrencyexchange " +
                " where currencyexchange_gid='" + currencyexchange_gid + "'";

                DataTable dt_datatable = objdbconn.GetDataTable(msSQL);
                var getModuleList = new List<GetOnchangeCurrency>();
                if (dt_datatable.Rows.Count != 0)
                {
                    foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpPost("PostSalesOrderfileupload")]
        public IActionResult PostSalesOrderfileupload(IFormFile file)
        {
            string? token = Request.Headers["Authorization"].FirstOrDefault();

            result objResult = new result();
            try
            {
                string? customer_gid = Request.Form["customer_gid"];
                string? customergid = Request.Form["customergid"];
                if (customer_gid.Contains("BCRM"))
                {
                    lscustomer_gid = customer_gid;
                }
                else
                {
                    lscustomer_gid = customergid;
                }
                string? branch_name = Request.Form["branch_name"];
                string? branch_gid = Request.Form["branch_gid"];
                string? salesorder_date = Request.Form["salesorder_date"];
                string? renewal_mode = Request.Form["renewal_mode"];
                string? renewal_date = Request.Form["renewal_date"];
                string? frequency_terms = Request.Form["frequency_terms"];
                string? customer_name = Request.Form["customer_name"];
                string? so_remarks = Request.Form["so_remarks"];
                string? so_referencenumber = Request.Form["so_referencenumber"];
                string? address1 = Request.Form["address1"];
                string? shipping_address = Request.Form["shipping_address"];
                string? delivery_days = Request.Form["delivery_days"];
                string? payment_days = Request.Form["payment_days"];
                string? currency_code = Request.Form["currency_code"];
                string? user_name = Request.Form["user_name"];
                string? exchange_rate = Request.Form["exchange_rate"];
                string? termsandconditions = Request.Form["termsandconditions"];
                string? template_name = Request.Form["template_name"];
                string? template_gid = Request.Form["template_gid"];
                string? grandtotal = Request.Form["grandtotal"];
                string? roundoff = Request.Form["roundoff"];
                string? insurance_charges = Request.Form["insurance_charges"];
                string? packing_charges = Request.Form["packing_charges"];
                string? buyback_charges = Request.Form["buyback_charges"];
                string? freight_charges = Request.Form["freight_charges"];
                string? additional_discount = Request.Form["additional_discount"];
                string? addon_charge = Request.Form["addon_charge"];
                string? tax_amount4 = Request.Form["tax_amount4"];
                string? tax_name4 = Request.Form["tax_name4"];
                string? totalamount = Request.Form["totalamount"];
                string? total_price = Request.Form["total_price"];
                string? taxsegment_gid = Request.Form["taxsegment_gid"];
                string? salesorder_gid = Request.Form["salesorder_gid"];
                string? lscompany_code;
                string? lspath;
                string? lspath1;
                string? final_path = "";
                string? vessel_name = "";

                msSQL = " select company_code from adm_mst_tcompany";
                lscompany_code = objdbconn.GetExecuteScalar(msSQL);

                MemoryStream ms = new MemoryStream();
                lspath = objconfiguration["Doc_upload_file"] + "/erp_documents" + "/" + lscompany_code + "/" +
                "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month;
                {
                    if ((!System.IO.Directory.Exists(lspath)))
                        System.IO.Directory.CreateDirectory(lspath);
                }
                IFormFileCollection formFiles = Request.Form.Files;
                if (formFiles.Count > 0)
                {
                    for (int i = 0; i < formFiles.Count; i++)
                    {
                        string? msdocument_gid = objcmnfunctions.GetMasterGID("UPLF");
                        FileExtensionname = file.FileName;
                        string? lsfile_gid = msdocument_gid;
                        FileExtension = Path.GetExtension(FileExtensionname).ToLower();
                        lsfile_gid = lsfile_gid + FileExtension;
                        lspath = objconfiguration["Doc_upload_file"] + "/erp_documents" + "/" +
                        lscompany_code + "/" + "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month
                        + "/";
                        lspath1 = "erp_documents" + "/" + lscompany_code + "/" + "Sales/Salesorderfiles/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/";
                        final_path = lspath + msdocument_gid + FileExtension;
                        using (var stream = new FileStream(final_path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }
                }

                string? totalvalue = user_name;

                msSQL = "select tax_prefix from acp_mst_ttax where tax_gid='" + tax_name4 + "'";
                string? lstaxname1 = objdbconn.GetExecuteScalar(msSQL);

                string? lscustomerbranch = "H.Q";
                string? lscampaign_gid = "NO CAMPAIGN";

                msSQL = " select * from smr_tmp_tsalesorderdtl " +
                    " where employee_gid='" + employee_gid + "'";
                dt_datatable = objdbconn.GetDataTable(msSQL);

                if (dt_datatable.Rows.Count != 0)
                {
                    string? inputDate = salesorder_date;
                    DateTime uiDate = DateTime.ParseExact(inputDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string? salesorder_date1 = uiDate.ToString("yyyy-MM-dd");

                    msSQL = " select customer_name from crm_mst_tcustomer where customer_gid='" + lscustomer_gid + " '";
                    lscustomername = objdbconn.GetExecuteScalar(msSQL);

                    msSQL = " select currency_code from crm_trn_tcurrencyexchange where currencyexchange_gid='" + currency_code + " '";
                    string? currency_code1 = objdbconn.GetExecuteScalar(msSQL);

                    string? lslocaladdon = "0.00";
                    string? lslocaladditionaldiscount = "0.00";
                    string? lslocalgrandtotal = " 0.00";
                    string? lsgst = "0.00";
                    string? lsamount4 = "0.00";

                    double totalAmount = double.TryParse(tax_amount4, out double totalpriceValue) ? totalpriceValue : 0;
                    double addonCharges = double.TryParse(addon_charge, out double addonChargesValue) ? addonChargesValue : 0;
                    double freightCharges = double.TryParse(freight_charges, out double freightChargesValue) ? freightChargesValue : 0;
                    double packingCharges = double.TryParse(packing_charges, out double packingChargesValue) ? packingChargesValue : 0;
                    double insuranceCharges = double.TryParse(insurance_charges, out double insuranceChargesValue) ? insuranceChargesValue : 0;
                    double roundoff1 = double.TryParse(roundoff, out double roundoffValue) ? roundoffValue : 0;
                    double additionaldiscountAmount = double.TryParse(additional_discount, out double discountAmountValue) ? discountAmountValue : 0;
                    double buybackCharges = double.TryParse(buyback_charges, out double buybackChargesValue) ? buybackChargesValue : 0;

                    double grandTotal = (totalAmount + addonCharges + freightCharges + packingCharges + insuranceCharges + roundoff1) - additionaldiscountAmount - buybackCharges;

                    string? lsinvoice_refno = "", lsorder_refno = "";
                    lsrefno = objcmnfunctions.GetMasterGID("SO");
                    msSQL = "select company_code from adm_mst_Tcompany";
                    lscompany_code = objdbconn.GetExecuteScalar(msSQL);
                    if (lscompany_code == "BOBA")
                    {
                        string? ls_referenceno = objcmnfunctions.getSequencecustomizerGID("INV", "RBL", branch_name);
                        msSQL = "SELECT SEQUENCE_CURVAL FROM adm_mst_tsequencecodecustomizer  WHERE sequence_code='INV' AND branch_gid='" + branch_name + "'";
                        string? lscode = objdbconn.GetExecuteScalar(msSQL);

                        lsinvoice_refno = "SI" + " - " + lscode;
                        lsorder_refno = "SO" + " - " + lscode;
                    }
                    else
                    {
                        lsinvoice_refno = mssalesorderGID;
                        lsorder_refno = lsrefno;
                    }
                    mssalesorderGID = objcmnfunctions.GetMasterGID("VSOP");

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
                              " file_path, " +
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
                              " file_name ," +
                             "created_date" +
                             " )values(" +
                             " '" + mssalesorderGID + "'," +
                             " '" + branch_name + "'," +
                             " '" + salesorder_date1 + "'," +
                             " '" + lscustomer_gid + "'," +
                             " '" + lscustomername.Replace("'", "\\\'") + "'," +
                             " '" + address1.Replace("'", "\\\'") + "'," +
                             " '" + address1.Replace("'", "\\\'") + "'," +
                             " '" + employee_gid + "'," +
                             // if(values.so_referencenumber != "" || values.so_referencenumber != null)
                             // {
                             //msSQL+= "'" + values.so_referencenumber + "',";
                             //  }
                             // else
                             // {
                             //        msSQL+=" '" + lsrefno + "',";
                             // }
                             " '" + lsorder_refno + "'," +
                               " '" + lsinvoice_refno + "',";
                    if (so_remarks != null)
                    {
                        msSQL += " '" + so_remarks.Replace("'", "\\\'") + "',";
                    }
                    else
                    {
                        msSQL += " '" + so_remarks + "',";
                    }

                    msSQL += " '" + payment_days + "'," +
                        " '" + delivery_days + "'," +
                        " '" + grandtotal.Replace(",", "").Trim() + "',";
                    if (termsandconditions != null)
                    {
                        msSQL += " '" + termsandconditions.Replace("'", "\\\'") + "',";
                    }
                    else
                    {
                        msSQL += " '" + termsandconditions + "',";
                    }
                    msSQL += " 'Approved',";
                    if (addon_charge != "")
                    {
                        msSQL += "'" + addon_charge + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladdon + "',";
                    }
                    if (additional_discount != "")
                    {
                        msSQL += "'" + additional_discount + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladditionaldiscount + "',";
                    }
                    if (addon_charge != "")
                    {
                        msSQL += "'" + addon_charge + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladditionaldiscount + "',";
                    }
                    if (additional_discount != "")
                    {
                        msSQL += "'" + additional_discount + "',";
                    }
                    else
                    {
                        msSQL += "'" + lslocaladditionaldiscount + "',";
                    }
                    msSQL += " '" + lslocalgrandtotal + "'," +
                         " '" + currency_code1 + "'," +
                         " '" + currency_code + "'," +
                         " '" + exchange_rate + "'," +
                           " '" + final_path + "'," +
                         " '" + shipping_address.Replace("'", "\\\'") + "'," +
                         " '" + tax_name4 + "'," +
                         " '" + lstaxname1 + "', " +
                        "'" + lsgst + "',";
                    msSQL += " '" + totalamount.Replace(",", "").Trim() + "',";
                    if (grandtotal == null && grandtotal == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += " '" + grandtotal.Replace(",", "").Trim() + "',";
                    }

                    if (tax_amount4 != "" && tax_amount4 != null)
                    {
                        msSQL += "'" + tax_amount4 + "',";
                    }
                    else
                    {
                        msSQL += "'" + lsamount4 + "',";
                    }
                    msSQL += " '" + vessel_name + "'," +
                            " '" + user_name + "',";
                    if (roundoff == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + roundoff + "',";
                    }
                    if (addon_charge == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + addon_charge + "',";
                    }
                    if (additional_discount == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + additional_discount + "',";
                    }
                    if (freight_charges == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + freight_charges + "',";
                    }
                    if (buyback_charges == "")
                    {

                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + buyback_charges + "',";
                    }
                    if (packing_charges == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + packing_charges + "',";
                    }
                    if (insurance_charges == "")
                    {
                        msSQL += "'0.00',";
                    }
                    else
                    {
                        msSQL += "'" + insurance_charges + "',";
                    }
                    msSQL += "'I',";
                    msSQL += "'" + renewal_mode + "',";
                    msSQL += "'" + FileExtensionname + "',";

                    msSQL += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                    if (mnResult == 0)
                    {
                        objResult.status = false;
                        objResult.message = " Some Error Occurred While Inserting Salesorder Details";
                        return Ok(objResult);
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
                                " file_path, " +
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
                               " '" + branch_name + "'," +
                               " '" + salesorder_date1 + "'," +
                               " '" + lscustomer_gid + "'," +
                               " '" + lscustomername.Replace("'", "\\\'") + "'," +
                               " '" + address1.Replace("'", "\\\'") + "'," +
                               " '" + employee_gid + "',";

                        if (so_remarks != null)
                        {
                            msSQL += " '" + so_remarks.Replace("'", "\\\'") + "',";
                        }
                        else
                        {
                            msSQL += " '" + so_remarks + "',";
                        }
                        msSQL += " '" + lsinvoice_refno + "'," +
                          " '" + payment_days + "'," +
                                 " '" + delivery_days + "'," +
                                 " '" + grandtotal + "'," +
                                 " '" + termsandconditions.Replace("'", "\\\'") + "'," +
                                 " 'Approved',";
                        if (addon_charge == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + addon_charge + "',";
                        }
                        if (additional_discount == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + additional_discount + "',";
                        }
                        msSQL += "'" + lslocaladdon + "'," +
                               " '" + lslocaladditionaldiscount + "'," +
                               " '" + lslocalgrandtotal + "'," +
                               " '" + currency_code1 + "'," +
                               " '" + currency_code + "'," +
                               " '" + exchange_rate + "'," +
                        " '" + final_path + "',";
                        if (addon_charge == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + addon_charge + "',";
                        }
                        if (additional_discount == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + additional_discount + "',";
                        }
                        msSQL += "'" + shipping_address.Replace("'", "\\\'") + "'," +
                               " '" + lscampaign_gid + "',";
                        if (roundoff == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + roundoff + "',";
                        }
                        msSQL += " '" + user_name + "',";
                        if (freight_charges == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + freight_charges + "',";
                        }
                        if (buyback_charges == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + buyback_charges + "',";
                        }
                        if (packing_charges == "")
                        {
                            msSQL += "'0.00',";
                        }
                        else
                        {
                            msSQL += "'" + packing_charges + "',";
                        }
                        if (insurance_charges == "")
                        {
                            msSQL += "'0.00')";
                        }
                        else
                        {
                            msSQL += "'" + insurance_charges + "')";
                        }
                        mnResult2 = objdbconn.ExecuteNonQuerySQL(msSQL);
                        if (mnResult2 == 1)
                        {
                            objResult.status = true;
                        }

                    }
                    if (renewal_mode == "Y")
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
                                 "'" + renewal_mode + "'," +
                                 "'" + frequency_terms + "'," +
                                 "'" + lscustomer_gid + "'," +
                                 "'" + renewal_date + "'," +
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
                    dt_datatable = objdbconn.GetDataTable(msSQL);
                    var getModuleList = new List<postsales_list>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in dt_datatable.Rows)
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

                            string? display_field = dt["product_remarks"].ToString();

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
                                objResult.status = false;
                                objResult.message = "Error occurred while Insertion";
                                return Ok(objResult);
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
                             " '" + i + 1 + "'," +
                             " '" + dt["tax_percentage"].ToString() + "'," +
                             " '" + dt["taxsegment_gid"].ToString() + "'," +
                             " '" + dt["order_type"].ToString() + "', " +
                             " '" + so_referencenumber + "')";
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

                    string? lsstage = "8";
                    msgetlead2campaign_gid = objcmnfunctions.GetMasterGID("BLCC");
                    string? lsso = "N";
                    msSQL = " select employee_gid from hrm_mst_temployee where user_gid='" + user_name + "'";
                    string? employee = objdbconn.GetExecuteScalar(msSQL);


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
                                                      "'" + lscustomer_gid + "'," +
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

                            msSQL = "update smr_trn_tsalesorder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
                                   " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where salesorder_gid='" + mssalesorderGID + "' ";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                            msSQL = "update acp_trn_torder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
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
                                msSQL = "update smr_trn_tsalesorder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
                                   " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                                   " where salesorder_gid='" + mssalesorderGID + "' ";
                                mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);

                                msSQL = "update acp_trn_torder set salesorder_status='Approved',salesorder_remarks='" + so_remarks + "', " +
                                      " approved_by='" + employee_gid + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                                      " where salesorder_gid='" + mssalesorderGID + "' ";
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

                            msSQL = " delete from smr_tmp_tsalesorderdrafts " +
                                  " where salesorder_gid='" + salesorder_gid + "'";
                            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
                        }


                        if (mnResult2 != 0)
                        {
                            objResult.status = true;
                            objResult.message = "Sales Order  Raised Successfully";
                            return Ok(objResult);
                        }
                        else
                        {
                            objResult.status = false;
                            objResult.message = "Error While Raising Sales Order";
                            return Ok(objResult);
                        }

                    }
                    objodbcDataReader.Close();
                }
                else
                {
                    objResult.status = true;
                    objResult.message = "Select one Product to Raise Enquiry";
                    return Ok(objResult);
                }
            }
            catch (Exception ex)
            {
                objResult.message = "Exception occured while Submitting Sales Order !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                objResult.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
            return Ok(objResult);
        }
        [HttpPost("PostSalesOrder")]
        public IActionResult PostSalesOrder(postsales_list values)
        {
            string? token = Request.Headers["Authorization"].FirstOrDefault();
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
                    if (values.termsandconditions != null)
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
                        return Ok(values);
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
                        foreach (DataRow dt in dt_datatable7.Rows)
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
                                return Ok(values);
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
                            return Ok(values);
                        }
                        else
                        {
                            values.status = false;
                            values.message = "Error While Raising Sales Order";
                            return Ok(values);
                        }
                    }
                    objodbcDataReader.Close();
                }
                else
                {
                    values.status = true;
                    values.message = "Select one Product to Raise Enquiry";
                    return Ok(values);
                }
            }
            catch (Exception ex)
            {
                values.message = "Exception occured while Submitting Sales Order !";
                objcmnfunctions.LogForAudit("*******Date*****" + DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + "***********" +
                $"DataAccess: {System.Reflection.MethodBase.GetCurrentMethod().Name}" + "***********" + ex.Message.ToString() + "***********" +
                values.message.ToString() + "*****Query****" + msSQL + "*******Apiref********", "ErrorLog/Sales/" + "Log" + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt");
            }
            return Ok(values);
        }
        [HttpGet("GetProductWithTaxSummary")]
        public IActionResult GetProductWithTaxSummary(string product_gid, string customer_gid)
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            msSQL = "select pricesegment_gid from crm_mst_tcustomer where customer_gid = '" + customer_gid + "'";
            string pricesegment_gid = objdbconn.GetExecuteScalar(msSQL);
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
                    foreach (DataRow dt1 in dt_datatable.Rows)
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
                    foreach (DataRow dt1 in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpPut("PostProductAdd")]
        public IActionResult PostProductAdd([FromBody] PostProduct_list values)
        {
            string? token = Request.Headers["Authorization"].FirstOrDefault();
            try
            {
                string? lsproducttype_name = "";
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
                    return Ok(values);

                }
                else if (values.unitprice == null || values.unitprice == "" || values.unitprice == "undefined")
                {
                    values.status = false;
                    values.message = "Price cannot be left empty or set to zero";
                    return Ok(values);
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
                              " '" + values.taxgid1 + "', " +
                              " '" + values.taxgid2 + "', " +
                              " '" + values.taxgid1 + "', " +
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
            return Ok(values);
        }
        [HttpGet("GetOnChangeProductGroup")]
        public IActionResult GetOnChangeProductGroup(string productgroup_gid)
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            try
            {


                if (productgroup_gid != null)
                {
                    msSQL = "select product_gid,product_name from pmr_mst_tproduct where productgroup_gid = '" + productgroup_gid + "'";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                    var getModuleList = new List<GetCustomerDet>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetOnChangeProductGroupName")]
        public IActionResult GetOnChangeProductGroupName(string product_gid)
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            try
            {


                if (product_gid != null)
                {
                    msSQL = "select product_gid,product_code from pmr_mst_tproduct where product_gid='" + product_gid + "'";
                    DataTable dt_datatable = objdbconn.GetDataTable(msSQL);

                    var getModuleList = new List<GetProdGrpName>();
                    if (dt_datatable.Rows.Count != 0)
                    {
                        foreach (DataRow dt in dt_datatable.Rows)
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
            return Ok(values);
        }
        [HttpGet("GetSalesDtl")]
        public IActionResult GetSalesDtl()
        {
            MdlSmrTrnSalesorder values = new MdlSmrTrnSalesorder();
            objsales.DaGetSalesDtl(values);
            return Ok(values);
        }
        [HttpDelete("GetDeleteDirectSOProductSummary")]
        public IActionResult GetDeleteDirectSOProductSummary(string? tmpsalesorderdtl_gid)
        {
            salesorders_list objresult = new salesorders_list();
            msSQL = " delete from smr_tmp_tsalesorderdtl " +
                       " where tmpsalesorderdtl_gid='" + tmpsalesorderdtl_gid + "'";
            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL);
            if (mnResult == 1)
            {
                objresult.message = "Product Deleted successfully.";
                objresult.status = true;
            }
            else
            {
                objresult.message = "Error while deleting product";
                objresult.status = false;
            }
            return Ok(objresult);
        }       
    }
}
