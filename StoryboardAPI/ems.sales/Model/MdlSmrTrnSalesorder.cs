
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ems.sales.Models

{
    public class MdlSmrTrnSalesorder : result
    {
        public List<Directeddetailslist2> ?Directeddetailslist2 { get; set; }
        public List<Getapprovalsalesorder_list> ?Getapprovalsalesorder_list { get; set; }
        public List<GetOnChangeBranch_list>? GetOnChangeBranch_list { get; set; }
        public List<Getproductgroup>? Getproductgroup { get; set; }
        public List<Getproducttypesales>? Getproducttypesales { get; set; }
        public List<salesorderlastsixmonths_list>? salesorderlastsixmonths_list { get; set; }
        public List<updatesales_list>? updatesales_list { get; set; }
        public List<Getamendsalesorderdtl>? Getamendsalesorderdtl { get; set; }
        public List<Getamendsalesorderdtl1>? Getamendsalesorderdtl1 { get; set; }
        public List<summarydtl_list>? summarydtl_list { get; set; }
        public List<GetTaxSegmentLists>? GetTaxSegmentList { get; set; }
        public List<salesorder_list>? salesorder_list { get; set; }
        public List<salesproductlist>? salesproduct_list { get; set; }
        public List<GetOnchangeCurrency>? GetOnchangeCurrency { get; set; }
        public List<postsalesorder_list>? postsalesorder_list { get; set; }
        public List<GetBranchDropdown>? GetBranchDtl { get; set; }
        public List<GetCustomerDropdown>? GetCustomerDtl { get; set; }
        public List<productsummary_list>? productsummary_list { get; set; }
        public List<DirecteditSalesorderList>? directeditsalesorder_list { get; set; }
        public List<editsalesorderdtllist>? editsalesorderdtllist { get; set; }
        public List<GetPersonDropdown>? GetPersonDtl { get; set; }
        public List<GetCurrencyDropdown>? GetCurrencyDtl { get; set; }
        public List<GetCustomerDet>? GetCustomer { get; set; }
        public List<GetTaxoneDropdown>? GetTax1Dtl { get; set; }
        public List<GetTaxTwoDropdown>? GetTax2Dtl { get; set; }
        public List<GetTaxThreeDropdown>? GetTax3Dtl { get; set; }
        public List<GetTaxFourDropdown>? GetTax4Dtl { get; set; }
        public List<GetProductNamDropdown>? GetProductNamDtl { get; set; }
        public List<Getupdate>? Getupdate { get; set; }
        public List<GetproductsCode>? ProductsCode { get; set; }
        public List<getproductsCode>? ProductsCodes { get; set; }
        public List<salesorders_list>? salesorders_list { get; set; }
        public List<GetViewSalesOrder>? GetViewSalesOrder { get; set; }
        public List<salesordertype_list>? salesordertype_list { get; set; }
        public List<postsales_list>? postsales_list { get; set; }
        public List<GetProdGrpName>? GetProdGrpName { get; set; }
        public double grand_total { get; set; }
        public double grandtotal { get; set; }
        public List<postsalesorderdetails_list>? postsalesorderdetails_list { get; set; }
        public List<salesorderhistorylist>? salesorderhistorylist { get; set; }
        public List<Productsummarys_list>? Productsummarys_list { get; set; }
        public List<salesorderhistorysummary_list>? salesorderhistorysummarylist { get; set; }
        public List<salesorderproduct_list>? salesorderproduct_list { get; set; }
        public List<GetTaxSegmentListorder>? GetTaxSegmentListorder { get; set; }
        public string? margin_percentage { get; internal set; }
        public string? product_requireddate { get; internal set; }
        public string? product_name { get; internal set; }
        public string? customer_name { get; internal set; }
        public string? tax1_gid { get; internal set; }
        public string? tax2_gid { get; internal set; }
        public string? tax3_gid { get; internal set; }
        public string? productuom_name { get; internal set; }
        public string? display_field { get; internal set; }
        public string? selling_price { get; internal set; }
        public string? productgroup_name { get; internal set; }
        public string? product_gid { get; set; }
        public string? customer_gid { get; set; }
        public string? ordertoinvoicecount { get; set; }
        public string? ordercount { get; set; }
        public List<GetProductsearchs> ?GetProductsearchs { get; set; } 
        public List<PostProduct_list> ?PostProduct_list { get; set; }
        public List<GetCourierService_list> ?GetCourierService_list { get; set; }
        public List<Getordertoinvoice_list> ?Getordertoinvoice_list { get; set; }
        public List<GetOrderToInvoiceProduct_list> ?GetOrderToInvoiceProduct_list { get; set; }
        public List<ordertoinvoiceproductsubmit_list> ?ordertoinvoiceproductsubmit_list { get; set; }
        public List<ordertoinvoicesubmit> ?ordertoinvoicesubmit { get; set; }
        public List<GetSalesDropdown> ? GetSalesDropdown { get; set; }

    }
    public class GetSalesDropdown : result
    {
        public string user_gid { get; set; }
        public string user_name { get; set; }
    }
    public class result
    {
        public string? message { get; set; }
        public bool? status { get; set; }
    }
    public class GetCourierService_list: result
    {
        public string?  courierservice_id { get; set; }
        public string? courierservicetype_id { get; set; }
        public string? name { get; set; }
    }
    public class Getproductgroup : result
    {
        public string? productgroup_gid { get; set; }
        public string? productgroup_name { get; set; }
    }
    public class salesorderlastsixmonths_list : result
    {
        public string? salesorder_date { get; set; }
        public string? months { get; set; }
        public string? orderamount { get; set; }
    }
    public class salesordertype_list : result
    {
        public string? whatappcount { get; set; }
        public string? shopifycount { get; set; }
        public string? internalcount { get; set; }
    }
    public class GetProductsearchs
    {
        public string? product_gid { get; set; }
        public string? product_code { get; set; }
        public string? productgroup_name { get; set; }
        public string? productuom_name { get; set; }
        public string? productuom_gid { get; set; }
        public string? productgroup_gid { get; set; }
        public string? product_name { get; set; }
        public string? unitprice { get; set; }
        public string? producttype_gid { get; set; }
        public double taxamount1 { get; set; }
        public double taxamount2 { get; set; }
        public double quantity { get; set; }
        public double total_amount { get; set; }
        public double discount_amount { get; set; }
        public double discount_percentage { get; set; }
        public double discount_persentage { get; set; }
        public string? product_desc { get; set; }

    }

    public class PostProduct_list : result
    {
        public string? taxgid1 { get; set; }
        public string? taxgid2 { get; set; }
        public string? producttype_name { get; set; }
        public string? productdiscount_amountvalue { get; set; }
        public string? product_remarks { get; set; }
        public string? tax_prefix { get; set; }
        public string? tax_prefix2 { get; set; }
        public string? product_gid { get; set; }
        public string? productgroup_name { get; set; }
        public string? productuom_name { get; set; }
        public string? product_code { get; set; }
        public string? product_name { get; set; }
        public string? productquantity { get; set; }
        public string? unitprice { get; set; }
        public decimal? productdiscount { get; set; }
        public decimal? discountprecentage { get; set; }
        public string? producttotal_amount { get; set; }
        public string? taxamount1 { get; set; }
        public decimal? taxamount2 { get; set; }
        public decimal? taxamount3 { get; set; } 
        public decimal? taxprecentage1 { get; set; }
        public decimal? taxprecentage2 { get; set; }
        public decimal? taxprecentage3 { get; set; }
        public string ?exchange_rate { get; set; }
        public string? customer_gid { get; set; }
        public string? taxname3 { get; set; }
        public string? taxname2 { get; set; }
        public string? taxname1 { get; set; }
        public decimal? discount_amount { get; set; }        
        public string? salesorder_gid { get; set; }
        public string? taxsegment_gid { get; set; }
    }
    
    public class taxSegments_list
    {
        public string? tax_name { get; set; }
        public string? tax_gid { get; set; }
        public string? tax_percentage { get; set; }
        public string? taxsegment_name { get; set; }
        public string? taxsegment_gid { get; set; }
        public string? taxAmount { get; set; }
    } 
    public class SOProductList1
    {
        public List<taxSegments_list> ?taxSegments { get; set; }
        public string? qty_requested { get; set; }
        public string? quantity { get; set; }
        public string? discount_persentage { get; set; }
        public string? discount_amount { get; set; }
        public string? tax1 { get; set; }
        public string? tax2 { get; set; }
        public string? totalTaxAmount { get; set; }
        public string? tax_amount { get; set; }
        public string? taxamount2 { get; set; }
        public string? taxamount1 { get; set; }
        public string? taxsegment_gid { get; set; }
        public string? taxsegmenttax_gid { get; set; }
        public string? tax_name { get; set; }
        public string? salesorder_gid { get; set; }
        public string? exchange_rate { get; set; }
        public decimal? discount_percentage { get; set; }
        public string? discountamount { get; set; }
        public string? currency_code { get; set; }
        public string? tax_gids_string { get; set; }
        public string? total_amount { get; set; }
        public string? product_name { get; set; }
        public string? product_gid { get; set; }
        public string? productuom_gid { get; set; }
        public string? productuom_name { get; set; }
        public string? productgroup_gid { get; set; }
        public string? productgroup_name { get; set; }
        public string? product_code { get; set; }
        public string? tax3 { get; set; }
        public string? display_field { get; set; }
        public string? unitprice { get; set; }
        public bool status { get; set; }
        public string? message { get; set; }
        public string? product_remarks { get; set; }
        public string? taxamount { get; set; }
        public string? tax_rate { get; set; }
    }

    public class GetTaxSegmentListorder : result
    {
        public string? product_gid { get; set; }
        public string? tax_prefix { get; set; }
        public string? product_name { get; set; }
        public string? taxsegment_gid { get; set; }
        public string? taxsegment_name { get; set; }
        public string? tax_name { get; set; }
        public string? tax_gid { get; set; }
        public string? tax_percentage { get; set; }
        public string? mrp_price { get; set; }
        public string? cost_price { get; set; }
        public string? tax_amount { get; set; }
        public string? product_code { get; set; }
        public string? productuom_name { get; set; }
        public string? productgroup_name { get; set; }
        public string? productuom_gid { get; set; }
        public string? producttype_gid { get; set; }
        public string? productgroup_gid { get; set; }
        public string? unitprice { get; set; }
        public double total_amount { get; set; }
        public double quantity { get; set; }
        public double discount_percentage { get; set; }
        public double discount_amount { get; set; }

    }


    public class productsummary_list : result
    {
        public string? productuom_name { get; set; }
        public string? taxamount1 { get; set; }
        public string? unitprice { get; set; }

        public string? product_name { get; set; }
        public string? tmppurchaseorderdtl_gid { get; set; }
        public string? discount_percentage { get; set; }

        public string? tax { get; set; }
        public string? totalamount { get; set; }
        public string? hsn { get; set; }
        public string? product_code { get; set; }
        public string? productgroup_name { get; set; }
        public string? customerproduct_code { get; set; }
        public string? uom_name { get; set; }
        public string? qty { get; set; }
        public string? product_price { get; set; }
        public string? discount_amount { get; set; }
        public string? product_total { get; set; }
        public string? taxsegment_gid { get; set; }
        public string? display_field { get; set; }
        public string? tax_name1 { get; set; }
        public string? tax_name2 { get; set; }

        public string? tax_name3 { get; set; }
        public string? tax_percentage3 { get; set; }
        public string? tax_percentage2 { get; set; }
        public string? tax_percentage { get; set; }
        public string? purchaseorder_gid { get; set; }
        public string? product_gid { get; set; }
    }
    public class Getproducttypesales
    {
        public string? producttype_name { get; set; }
        public string? producttype_code { get; set; }
        public string? producttype_gid { get; set; }

    }
    public class salesorderproduct_list : result
    {
        public string? product_name { get; set; }
        public string? qty_quoted { get; set; }
        public string? salesorder_gid { get; set; }



    }

    public class salesorderhistorysummary_list : result
    {
        public string? salesorder_date { get; set; }
        public string? so_referenceno1 { get; set; }
        public string? customer_name { get; set; }
        public string? customer_contact_person { get; set; }
        public string? user_firstname { get; set; }
        public string? Grandtotal { get; set; }
        public string? salesorder_status { get; set; }
        public string? salesorder_gid { get; set; }
        public string? leadbank_gid { get; set; }

    }
    public class salesorderhistorylist : result
    {
        public string? salesorder_gid { get; set; }
        public string? leadbank_gid { get; set; }
        public string? so_referenceno1 { get; set; }
        public string? salesorder_date { get; set; }
        public string? customer_name { get; set; }
        public string? so_remarks { get; set; }
    }
    public class Getamendsalesorderdtl1 : result
    {
        public string? customer_name { get; set; }
        public string? roundoff { get; set; }
        public string? salesorder_gid { get; set; }
        public string? customer_gid { get; set; }
        public string? salesorder_date { get; set; }
        public string? customer_mobile { get; set; }
        public string? freight_terms { get; set; }
        public string? payment_terms { get; set; }
        public string? tax_gid { get; set; }
        public string? customer_contact_person { get; set; }
        public string? total_price { get; set; }
        public string? shipping_to { get; set; }
        public string? gst_amount { get; set; }
        public string? termsandconditions { get; set; }
        public string? so_remarks { get; set; }
        public string? so_referencenumber { get; set; }
        public string? Grandtotal { get; set; }
        public string? delivery_days { get; set; }
        public string? payment_days { get; set; }
        public string? additional_discount { get; set; }
        public string? addon_charge { get; set; }
        public string? branch_name { get; set; }
        public string? campaign_gid { get; set; }
        public string? so_referenceno1 { get; set; }
        public string? total_amount { get; set; }
        public string? vessel_name { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public string? salesperson_gid { get; set; }
        public string? freight_charges { get; set; }
        public string? packing_charges { get; set; }
        public string? buyback_charges { get; set; }
        public string? insurance_charges { get; set; }
        public string? exchange_rate { get; set; }
        public string? currency_code { get; set; }
        public string? currency_gid { get; set; }
        public string? customer_address { get; set; }
        public string? customer_email { get; set; }
        public string? tax_name { get; set; }
        public string? customer_branch { get; set; }
        public string? quotation_refno { get; set; }
    }


    public class updatesales_list : result
    {

        public string? tmpsalesorderdtl_gid { get; internal set; }
        public string? tax_name { get; internal set; }
        public string? tax_amount { get; internal set; }
        public string? tax_gid { get; internal set; }
        public string? total_amount { get; internal set; }
        public string? productgroup_name { get; internal set; }
        public string? discountamount { get; internal set; }
        public string? discountpercentage { get; internal set; }
        public string? quantity { get; internal set; }
        public string? unitprice { get; internal set; }
        public string? unit { get; internal set; }
        public string? product_name { get; internal set; }
        public string? product_code { get; internal set; }
    }

        public class summarydtl_list : result
    {
        public string? totalamount { get; internal set; }
        public double grand_total { get; internal set; }
        public string? tax1_gid { get; internal set; }
        public string? tmpsalesorderdtl_gid { get; internal set; }
        public string? margin_percentage { get; internal set; }
        public string? margin_amount { get; internal set; }
        public string? salesorder_gid { get; internal set; }
        public string? selling_price { get; internal set; }
        public string? productgroup_gid { get; internal set; }
        public string? productgroup_name { get; internal set; }
        public string? customerproduct_code { get; internal set; }
        public string? discount_percentage { get; internal set; }
        public string? discount_amount { get; internal set; }
        public string? tax_percentage { get; internal set; }
        public string? tax_amount { get; internal set; }
        public string?  payment_days { get; internal set; }
        public string? delivery_period { get; internal set; }
        public string? product_remarks { get; internal set; }
        public string? price { get; internal set; }
        public string? display_field { get; internal set; }
        public string? tax_name { get; internal set; }
        public string? product_gid { get; internal set; }
        public string? product_name { get; internal set; }
        public string?  uom_gid { get; internal set; }
        public string? uom_name { get; internal set; }
   
        public string? employee_gid { get; internal set; }
        public string? vendor_gid { get; internal set; }

        public string? vendor_companyname { get; internal set; }
        public string? product_code { get; internal set; }
        public string? slno { get; internal set; }
        public string? tax_gid { get; internal set; }
        public string? product_requireddateremarks { get; internal set; }
        public string? product_price { get; internal set; }
        public string? product_requireddate { get; internal set; }
        public string? qty_quoted { get; internal set; }
        public string? product_delivered { get; internal set; }


    }
    public class salesproductlist : result
    {
        public string? customerproduct_code { get; set; }
        public string? product_code { get; set; }

        public string? product_name { get; set; }
        public string? qty_quoted { get; set; }

    }
    public class Getamendsalesorderdtl : result
    {
        public string? salesorder_refno { get; set; }
        public string? user_name { get; set; }
      //  public string? customer_email { get; set; }
        public string? customer_name { get; set; }
        public string? leadbank_name { get; set; }
        public string? leadbank_address1 { get; set; }
      //  public string? customer_address { get; set; }
        public string? roundoff { get; set; }
        public string? salesorder_gid { get; set; }
        public string? customer_gid { get; set; }
        public string? salesorder_date { get; set; }
        public string? customer_mobile { get; set; }
        public string? freight_terms { get; set; }
        public string? payment_terms { get; set; }
        public string? tax_gid { get; set; }
        public string? customer_contact_person { get; set; }
        public string? total_price { get; set; }
        public string? shipping_to { get; set; }
        public string? gst_amount { get; set; }
        public string? termsandconditions { get; set; }
        public string? so_remarks { get; set; }
        public string? so_referencenumber { get; set; }
        public string? Grandtotal { get; set; }
        public string? delivery_days { get; set; }
        public string? payment_days { get; set; }
        public string? additional_discount { get; set; }
        public string? addon_charge { get; set; }
        public string? branch_name { get; set; }
        public string? campaign_gid { get; set; }
        public string? so_referenceno1 { get; set; }
        public string? total_amount { get; set; }
        public string? vessel_name { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public string? salesperson_gid { get; set; }
        public string? freight_charges { get; set; }
        public string? packing_charges { get; set; }
        public string? buyback_charges { get; set; }
        public string? insurance_charges { get; set; }
        public string? exchange_rate { get; set; }
        public string? currency_code { get; set; }
        public string? currency_gid { get; set; }
        
        public string? tax_name { get;set; }
        public string? customer_branch { get;set; }
        public string? quotation_refno { get; set; }
        public string? customer_address { get; set; }

        public string? customer_email { get; set; }
    }

        public class Productsummarys_list : result
    {
        public string? product_name { get; set; }
        public string? tmpquotationdtl_gid { get; set; }
        public string? product_requireddate { get; set; }
        public string? product_gid { get; set; }
        public string? margin_percentage { get; set; }
        public string? employee_gid { get; set; }
        public string? tax3_gid { get; set; }
        public string? tax2_gid { get; set; }
        public string? tax1_gid { get; set; }
        public string? selling_price { get; set; }
        public string? uom_gid { get; set; }
        public string? productuom_gid { get; set; }
        public string? productuom_name { get; set; }
        public string? display_field { get; set; }

    }
    public class postsalesorder_list : result
    {
        public string? so_referenceno1 { get; set; }
        public string? salesperson_gid { get; set; }
        public string? renewal_status { get; set; }
        public string? currency_gid { get; set; }
        public string? customer_gid { get; set; }
        public string? address1 { get; set; }
        public string? address2 { get; set; }
        public string? total_amount { get; set; }
        public string? renewal_date { get; set; }
        public string? frequency_term { get; set; }
        public string? renewal_mode { get; set; }

        public string? gst_amount { get; set; }
        public string? gst_number { get; set; }
        public string? salesorder_date { get; set; }
        public string? branch_name { get; set; }
        public string? customer_name { get; set; }
        public string? customer_address { get; set; }
        public string? customer_contact_person { get; set; }
        public string? user_gid { get; set; }
        public string? customer_email { get; set; }
        public string? customer_mobile { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public string? currency_code { get; set; }
        public string? file_path { get; set; }
        public string? file_name { get; set; }
        public string? salesorder_gid { get; set; }
        public string? customer_instruction { get; set; }
        public string? exchange_rate { get; set; }
        public string? freight_terms { get; set; }
        public string? payment_terms { get; set; }
        public string? payment_days { get; set; }
        public string? so_referencenumber { get; set; }
        public string? shipping_to { get; set; }
        public string? delivery_days { get; set; }
        public string? so_remarks { get; set; }
        public string? salesperson_name { get; set; }
        public string? productgroup_name { get; set; }
        public string? product_code { get; set; }
        public string? product_name { get; set; }
        public string? uom_name { get; set; }
        public string? qty_quoted { get; set; }
        public string? selling_price { get; set; }
        public string? price { get; set; }
        public string? product_requireddate { get; set; }
        public string? product_price { get; set; }
        public string? addon_charge { get; set; }
        public string? additional_discount { get; set; }
        public string? freight_charges { get; set; }
        public string? buyback_charges { get; set; }
        public string? packing_charges { get; set; }
        public string? insurance_charges { get; set; }
        public string? roundoff { get; set; }
        public string? Grandtotal { get; set; }
        public string? termsandconditions { get; set; }
        public string? margin_percentage { get; set; }
        public string? tax_name { get; set; }
        public string? tax_name2 { get; set; }
        public string? tax_name3 { get; set; }
        public string? order_instruction { get; set; }
        public string? tax_amount { get; set; }
        public string? tax_amount2 { get; set; }
        public string? tax_amount3 { get; set; }
        public string? totax { get; set; }
        public string? tax_name4 { get; set; }
        public string? total_price { get; set; }
    

    }

    public class GetSaleslist : result
    {
        public string? tax_gid3 { get; set; }
        public string? tax_gid { get; set; }

        public string? quantity { get; set; }
        public string? productname { get; set; }
        public string? productuom { get; set; }
        public string? product_gid { get; set; }
        public string? tax_name1 { get; set; }
        public string? totalamount { get; set; }
        public string? tax_name2 { get; set; }
        public string? unitprice { get; set; }
        public string? discountpercentage { get; set; }
        public string? discountamount { get; set; }
        public string? taxamount1 { get; set; }
        public string? taxamount2 { get; set; }
        public string? taxamount3 { get; set; }

        public string? tax_name3 { get; set; }
        public string? totalamount1 { get; set; }
        public string? productuom_gid { get; set; }
        public string? productuom_name { get; set; }
        public string? created_by { get; set; }
        public string? taxpercentage3 { get; set; }

        public string? needby_date { get; set; }

        public string? taxname1 { get; set; }
        public string? taxname2 { get; set; }
        public string? productdescription { get; set; }
        public string? tax_gid1 { get; set; }
        public string? tax_gid2 { get; set; }
        public string? product_name { get; set; }
        public string? productgroup_gid { get; set; }
        public string? productgroup { get; set; }
        public string? costprice { get; set; }
        public string? productcode { get; set; }
        public string? customerproductcode { get; set; }
        public string? taxpercentage1 { get; set; }
        public string? taxpercentage2 { get; set; }
        public string? productgid { get; set; }

        public string? vendorprice { get; set; }
    }

    public class GetViewSalesOrder : result
    {
        public string? vendor_fax { get; set; }
        public string? tax_name { get; set; }
        public string? gst_amount { get; set; }
        public string? email_address { get; set; }
        public string? productuom_gid { get; set; }
        public string? overalltax { get; set; }
        public string? additional_discount { get; set; }

        public string? dispatch_name { get; set; }

        public string? contact_person { get; set; }

        public string? pocovernote_address { get; set; }

        public string? grandtotal { get; set; }

        public string? template_content { get; set; }
        public string? contact_number { get; set; }

        public string? purchaseorder_gid { get; set; }
        public string? purchaseorder_date { get; set; }
        public string? manualporef_no { get; set; }
        public string? deliverytobranch { get; set; }
        public string? branch_name { get; set; }
        public string? vendor_contactnumber { get; set; }
        public string? vendor_contact_person { get; set; }
        public string? vendor_faxnumber { get; set; }
        public string? vendor_companyname { get; set; }
        public string? vendor_emailid { get; set; }
        public string? vendor_address { get; set; }
        public string? exchange_rate { get; set; }

        public string? ship_via { get; set; }
        public string? addoncharge { get; set; }

        public string? payment_terms { get; set; }
        public string? freight_terms { get; set; }
        public string? delivery_location { get; set; }
        public string? currency_code { get; set; }
        public string? shipping_address { get; set; }
        public string? purchaseorder_reference { get; set; }
        public string? purchaseorder_remarks { get; set; }
        public string? discount_percentage { get; set; }
        public string? tax_gid { get; set; }
        public string? packing_charges { get; set; }
        public string? insurance_charges { get; set; }
        public string? payment_days { get; set; }
        public string? termsandconditions { get; set; }
        public string? buybackorscrap { get; set; }
        public string? roundoff { get; set; }
        public string? discount_amount { get; set; }
        public string? total_amount { get; set; }
        public string? freighttax_amount { get; set; }
        public string? tax_amount { get; set; }
        public string? approver_remarks { get; set; }

        public string? delivery_days { get; set; }

        public string? productgroup_name { get; set; }

        public string? product_code { get; set; }

        public string? product_name { get; set; }

        public string? productuom_name { get; set; }

        public string? product_price { get; set; }

        public string? priority_n { get; set; }

        public string? qty_ordered { get; set; }
        public string? qty_Received { get; set; }

        public string? qty_grnadjusted { get; set; }
        public string? priority_remarks { get; set; }

        public string? tax_amount2 { get; set; }

        public string? tax_amount3 { get; set; }

        public string? addon_amount { get; set; }
        public string? product_gid { get; set; }
        public string? customer_gid { get; set; }
        public string? customer_name { get; set; }
        public string? address { get; set; }
        public string? employee_gid { get; set; }
        public string? quotation { get; set; }
        public string? remarks { get; set; }
        public string? reference_no { get; set; }
        public string? addon { get; set; }
        public string? addon_charge_l { get; set; }
        public string? additional_discount_l { get; set; }
        public string? grandtotal_l { get; set; }
        public string? currency_gid { get; set; }
        public string? updated_addon_charge { get; set; }
        public string? updated_additional_discount { get; set; }
        public string? campaign_gid { get; set; }
        public string? vessel_name { get; set; }
        public string? salesperson_gid { get; set; }

    }
    public class postsales_list : result
    {
       
        public string? total_amount { get; set; }
        public string? renewal_date { get; set; }
        public string? renewal_mode { get; set; }
        public string? frequency_terms { get; set; }
        public string? frequency_term { get; set; }
        public string? salesorder_gid { get; set; }        
        public string? address1 { get; set; }        
        public string? product_remarks { get; set; }        
        public string? shipping_address { get; set; }        
        public string? customerinstruction { get; set; }        
        public string? branch_address { get; set; }        
        public string? taxsegment_gid { get; set; }        
        public string? taxsegmenttax_gid { get; set; }        
        public string? file { get; set; }        
        public string? tax_amount4 { get; set; }        
        public string? currency_gid { get; set; }
        public string? leadbank_gid { get; set; }
        public string? contactemailaddress { get; set; }
        public string? campaign_gid { get; set; }
        public string? salesperson_gid { get; set; }
        public string? gst_amount { get; set; }
        public string? customer_contact_person { get; set; }
        public string? tax_amount { get; set; }
        public string? product_code { get; set; }
        public string? tax_name { get; set; }
        public string? customer_address2 { get; set; }
        public string? customer_address { get; set; }
        public string? productuom_gid { get; set; }
        public string? customer_city { get; set; }
        public string? additional_discount { get; set; }

        public string? currencyexchange_gid { get; set; }

        public string? countryname { get; set; }

        public string? region_name { get; set; }

        public string? customer_state { get; set; }

        public string? grandtotal { get; set; }

        public string? gst_number { get; set; }
        public string? postal_code { get; set; }

        public string? customercontact_name { get; set; }
        public string? customer_email { get; set; }
        public string? customer_mobile { get; set; }
        public string? designation { get; set; }
        public string? fax { get; set; }
        public string? fax_area_code { get; set; }
        public string? fax_country_code { get; set; }
        public string? branch_gid { get; set; }
        public string? salesorder_date { get; set; }
        public string? customerbranch_gid { get; set; }
        public string? customercontact_gid { get; set; }
        public string? exchange_rate { get; set; }

        public string? customercontact_names { get; set; }
        public string? addoncharge { get; set; }

        public string? payment_terms { get; set; }
        public string? freight_terms { get; set; }
        public string? currency_code { get; set; }
        public string? so_referencenumber { get; set; }
        public string? so_referenceno1 { get; set; }
        public string? saleorder_gid { get; set; }
        public string? so_remarks { get; set; }
        public string? discount_percentage { get; set; }
        public string? tax_gid { get; set; }
        public string? packing_charges { get; set; }
        public string? insurance_charges { get; set; }
        public string? payment_days { get; set; }
        public string? termsandconditions { get; set; }
        public string? buybackorscrap { get; set; }
        public string? roundoff { get; set; }
        public string? shipping_to { get; set; }
        public string? tax_name4 { get; set; }
        public string? txttaxamount_1 { get; set; }
        public string? totalamount { get; set; }
        public string? total_price { get; set; }

        public string? delivery_days { get; set; }

        public string? productgroup_name { get; set; }

        public string? user_name { get; set; }

        public string? product_name { get; set; }

        public string? productuom_name { get; set; }

        public string? start_date { get; set; }

        public string? end_date { get; set; }

        public string? freight_charges { get; set; }
        public string? main_branch { get; set; }
        public string? created_flag { get; set; }
        public string? did_number { get; set; }
        public string? country_gid { get; set; }
        public string? leadbankcontact_gid { get; set; }
        public string? main_contact { get; set; }
        public string? branch_name { get; set; }
        public string? unitprice { get; set; }
        public string? quantity { get; set; }
        public string? discountpercentage { get; set; }
        public string? discountamount { get; set; }
        public string? taxamount_2 { get; set; }
        public string? taxamount_3 { get; set; }
        public string? taxamount_l { get; set; }
        public string? selling_price_l { get; set; }
        public string? totalamount_l { get; set; }
        public string? discount_amount_l { get; set; }
        public string? marginpercentage { get; set; }
        public string? tmpsalesorderdtl_gid { get; set; }
        public string? buyback_charges { get; set; }
        public string? addon_charge { get; set; }

        public string? tax_amount2 { get; set; }

        public string? tax_amount3 { get; set; }

        public string? productgroup_gid { get; set; }
        public string? product_gid { get; set; }
        public string? customer_gid { get; set; }
        public string? customergid { get; set; }
        public string? customer_name { get; set; }
        public string? customerproduct_code { get; set; }
        public string? display_field { get; set; }
        public string? selling_price { get; set; }
        public string? qty_quoted { get; set; }
        public string? margin_percentage { get; set; }
        public string? margin_amount { get; set; }
        public string? addon_charge_l { get; set; }
        public string? additional_discount_l { get; set; }
        public string? grandtotal_l { get; set; }
        public string? tax_name2 { get; set; }
        public string? tax_name3 { get; set; }
        public string? tax2_gid { get; set; }
        public string? tax3_gid { get; set; }
        public string? vessel_name { get; set; }
        public string? tax_amount_l { get; set; }

        public string? tax_amount2_l { get; set; }
        public string? tax_amount3_l { get; set; }
        public string? product_discount { get; set; }
        public string? price { get; set; }
        public string? salesorder_refno { get; set; }
        public string? vendor_gid { get; set; }

        public string? slno { get; set; }
        public string? product_requireddate { get; set; }

        public string? product_requireddateremarks { get; set; }
        public string? vendor_price { get; set; }
        public string? tax_percentage { get; set; }
        public string? tax_percentage2 { get; set; }
        public string? tax_percentage3 { get; set; }
        public string? order_type { get; set; }
        public string? mrp { get; set; }
        public string? taxamount_1 { get; set; }
        public string? producttotalamount { get; set; }


    }
    public class salesorders_list : result
    {
        public List<SOProductList1> ?SOProductList { get; set; }
        public string? taxamount1 { get; set; }
        public string? taxamount2 { get; set; }
        public string? taxamount3 { get; set; }
        public string? taxprecentage1 { get; set; }
        public string? taxprecentage2 { get; set; }
        public string? taxprecentage3 { get; set; }
        public string? total_amount { get; set; }
        public string? unit { get; set; }
        public string? grand_total { get; set; }
        public string? grandtotal { get; set; }
        public string? totaltaxamount { get; set; }
        public string? tmpsalesenquiry_gid { get; set; }
        public string? tmpsalesorderdtl_gid { get; set; }
        public string? created_by { get; set; }
        public string? salesorder_gid { get; set; }
        public string? customerproduct_code { get; set; }
        public string? selling_price { get; set; }
        public string? productuom_gid { get; set; }
        public string? productgroup_gid { get; set; }
        public string? product_name { get; set; }
        public string? currency_code { get; set; }
        public string? taxsegment_gid { get; set; }
        public string? taxsegmenttax_gid { get; set; }
        public string? tax { get; set; }
        public string? tax2 { get; set; }
        public string? tax3 { get; set; }
        public string? employee_gid { get; set; }
        public string? product_gid { get; set; }
        public string? customer_gid { get; set; }
        public string? product_code { get; set; }
        public string? slno { get; set; }
        public string? productgroup_name { get; set; }
        public string? product_price { get; set; }
        public string? display_field { get; set; }
        public string? taxgid2 { get; set; }
        public string? taxgid1 { get; set; }
        public string? taxgid3 { get; set; }

        public string? qty_quoted { get; set; }
        public string? discount_percentage { get; set; }
        public string? margin_percentage { get; set; }
        public string? margin_amount { get; set; }
        public string? discount_amount { get; set; }
        public string? uom_gid { get; set; }
        public string? uom_name { get; set; }

        public string? price { get; set; }
        public string? taxname1 { get; set; }
        public string? taxname2 { get; set; }
        public string? taxname3 { get; set; }
        public string? tax_gid { get; set; }
        public string? tax2_gid { get; set; }
        public string? tax3_gid { get; set; }

        public string? vendor_gid { get; set; }

        public string? tax_amount { get; set; }
        public string? tax_amount2 { get; set; }
        public string? tax_amount3 { get; set; }
        public string? tax_percentage { get; set; }
        public string? tax_percentage2 { get; set; }

        public string? tax_percentage3 { get; set; }
        public string? order_type { get; set; }
        public string? product_requireddateremarks { get; set; }
        public string? product_requireddate { get; set; }
        public string? unitprice { get; set; }
        public string? exchange_rate { get; set; }
        public string? quantity { get; set; }
        public string? productuom_name { get; set; }
        public string? totalamount { get; set; }
        public string? producttotalamount { get; set; }
        public double discountpercentage { get; set; }
        public string? discountamount { get; set; }
        public string? product_type { get; set; }
        public string? taxsegment_name { get; set; }
      
        public string? mrp_price { get; set; }
        public string? cost_price { get; set; }
        public string? product_remarks { get; set; }
        public string? taxamount { get; set; }
        public string? tax_rate { get; set; }
        public string? tax_prefix1 { get; set; }
        public string? tax_prefix2 { get; set; }



    }

    public class GetTaxSegmentLists : result
    {
        public string? product_gid { get; set; }
        public string? taxsegment_gid { get; set; }
        public string? taxsegment_name { get; set; }
        public string? tax_name { get; set; }
        public string? tax_gid { get; set; }
        public string? tax_percentage { get; set; }
        public string? mrp_price { get; set; }
        public string? cost_price { get; set; }
        public string? tax_amount { get; set; }

    }
    public class GetproductsCode : result
    {
        public string? product_gid { get; set; }
        public string? product_code { get; set; }
        public string? productgroup_name { get; set; }
        public string? productuom_name { get; set; }
        public string? productuom_gid { get; set; }
        public string? productgroup_gid { get; set; }
        public string? product_name { get; set; }
        public string? unitprice { get; set; }
        public string? taxsegment_gid { get; set; }
        public string? taxsegment_name { get; set; }
        public string? tax_name { get; set; }
        public string? tax_percentage { get; set; }
        public string? tax_gid { get; set; }
        public string? mrp_price { get; set; }
        public string? cost_price { get; set; }
        public string? tax_amount { get; set; }

    }

    public class getproductsCode : result
    {
        public string? product_gid { get; set; }
        public string? product_code { get; set; }
        public string? productgroup_name { get; set; }
        public string? productuom_name { get; set; }
        public string? productuom_gid { get; set; }
        public string? productgroup_gid { get; set; }
        public string? product_name { get; set; }
        public string? unitprice { get; set; }

    }

    public class salesorder_list : result
    {
        public string? contact { get; set; }
        public string? mintsoft_flag { get; set; }
        public string? deliverybasedinvoice_flag { get; set; }
        public string? file_name { get; set; }
        public string? customerdetails { get; set; }
        public string? customer_gid { get; set; }
        public string? salesorder_gid { get; set; }
        public string? salesorder_date { get; set; }
        public string? so_referenceno1 { get; set; }
        public string? source_flag { get; set; }
        public string? customer_name { get; set; }
        public string? branch_name { get; set; }
        public string? so_type { get; set; }
        public string? Grandtotal { get; set; }
        public string? user_firstname { get; set; }
        public string? file_path { get; set; }
        public string? salesorder_status { get; set; }
        public string? salesperson_name { get; set; }
        public string? customer_code { get; set; }
        public string? mintsoftid { get; set; }        


    }
    public class GetBranchDropdown : result
    {
        public string? branch_gid { get; set; }
        public string? branch_name { get; set; }
        public string? address1 { get; set; }

    }

    public class GetCustomerDropdown : result
    {
        public string? customer_gid { get; set; }
        public string? customer_name { get; set; }
        public string? leadbank_gid { get; set; }
        public string? leadbank_name { get; set; }

    }



    public class GetPersonDropdown : result
    {
        public string? user_gid { get; set; }
        public string? user_name { get; set; }

    }

    public class GetCurrencyDropdown : result
    {
        public string? currencyexchange_gid { get; set; }
        public string? currency_code { get; set; }
        public string? default_currency { get; set; }
        public string? exchange_rate { get; set; }

    }

    public class GetCustomerDet : result
    {
        public string? customercontact_names { get; set; }
        public string? gst_number { get; set; }
        public string? address1 { get; set; }
        public string? taxsegment_gid { get; set; }
        public string? branch_name { get; set; }
        public string? country_name { get; set;}
        public string? customer_email { get; set;}
        public string? customer_mobile { get; set;}
        public string? zip_code { get; set;}
        public string? country_gid { get; set;}
        public string? state { get; set;}
        public string? city { get; set;}
        public string? address2 { get; set;}
        public string? customer_address { get; set;}
        public string? customercontact_gid { get; set;}
        public string? customer_gid { get;set;}
        public string? customer_name { get;set;}
        public string? product_name { get;set;}
        public string? product_gid { get;set;}
    }

    public class GetTaxoneDropdown : result
    {
        public string? tax_gid { get; set; }
        public string? tax1_gid { get; set; }
        public string? tax_name { get; set; }
        public string? percentage { get; set; }

    }

    public class GetTaxTwoDropdown : result
    {
        public string? tax_gid2 { get; set; }
        public string? tax_name2 { get; set; }
        public string? percentage { get; set; }


    }

    public class GetTaxThreeDropdown : result
    {
        public string? tax_gid3 { get; set; }
        public string? tax_name3 { get; set; }
        public string? percentage { get; set; }


    }

    public class GetProductNamDropdown : result
    {
        public string? product_gid { get; set; }
        public string? product_name { get; set; }

    }

    public class GetTaxFourDropdown : result
    {
        public string? tax_gid { get; set; }
        public string? tax_name4 { get; set; } 
        public string? tax { get; set; }
        public string? percentage { get; set; }

    }
    public class GetOnchangeCurrency
    {

        public string? exchange_rate { get; set; }
        public string? currency_code { get; set; }

    }

    public class DirecteditSalesorderList : result
    {
        public string? product_code { get; set;}
        public string? product_gid { get; set;}
        public string? product_name { get; set;}
        public string? productuom_name { get; set;}
        public string? productgroup_name { get; set;}
        public string? productgroup_gid { get; set;}
        public string? quantity { get; set;}
        public string? unitprice { get; set;}
        public string? discount_percentage { get; set;}
        public string? discountamount { get; set;}
        public string? totalamount { get; set;}
        public string? tax_name { get; set;}
        public string? tax_gid { get; set;}
        public string? tax_amount { get; set;}
        public string? taxsegment_gid { get; set;}
        public string? tmpsalesorderdtl_gid { get; set;}
    }
    public class Directeddetailslist2 : result
    {
        public string? product_gid { get; set; }
        public string? product_name { get; set; }
        public string? currency_code { get; set; }
        public string? product_price { get; set; }
        public string? qty_quoted { get; set; }
        public string? customer_name { get; set; }
        public string? salesorder_date { get; set; }



    }

    public class Getupdate : result
    {

        public string? shopify_orderid { get; set; }
        public string? salesorder_status { get; set; }


    }

    public class postsalesorderdetails_list : result
    {
        public string? product_name { get; set; }
        public string? product_gid { get; set; }
        public string? product_code { get; set; }
        public string? productgroup_name { get; set; }
        public string? product_remarks { get; set; }
        public string? uom_name { get; set; }
        public string? tax_prefix { get; set; }
        public string? tax_prefix2 { get; set; }
        public string? salesorder_gid { get; set; }
        public string? qty_quoted { get; set; }
        public string? margin_amount { get; set; }
        public string? margin_percentage { get; set; }
        public string? product_price { get; set; }
        public string? tax_name { get; set; }
        public string? tax_amount { get; set; }
        public string? tax_amount2 { get; set; }
        public string? discount_percentage { get; set; }
        public string? discount_amount { get; set; }

        public string? price { get; set; }
    }
    public class GetProdGrpName : result
    {
        public string? customercontact_names { get; set; }
        public string? taxsegment_gid { get; set; }
        public string? branch_name { get; set; }
        public string? country_name { get; set; }
        public string? customer_email { get; set; }
        public string? customer_mobile { get; set; }
        public string? zip_code { get; set; }
        public string? country_gid { get; set; }
        public string? state { get; set; }
        public string? city { get; set; }
        public string? address2 { get; set; }
        public string? customer_address { get; set; }
        public string? customercontact_gid { get; set; }
        public string? customer_gid { get; set; }
        public string? customer_name { get; set; }
        public string? product_name { get; set; }
        public string? product_gid { get; set; }
        public string? product_code { get; set; }
    }
    public class GetOnChangeBranch_list : result
    {
        public string? branch_name { get; set; }
        public string? branch_gid { get; set; }
        public string? address1 { get; set; }
    }

    public class Getapprovalsalesorder_list : result 
    {
        public string? salesorderdtl_gid { get; set; }
        public string? vendor_price { get; set; }
        public string? salesorder_gid { get; set; }
        public string? customerproduct_code { get; set; }
        public string? vendor_companyname { get; set; }
        public string? product_gid { get; set; }
        public string? productgroup_gid { get; set; }
        public string? productgroup_name { get; set; }
        public string? product_name { get; set; }
        public string? product_price { get; set; }
        public string? qty_quoted { get; set; }
        public string? product_remarks { get; set; }
        public string? uom_gid { get; set; }
        public string? uom_name { get; set; }
        public string? payment_days { get; set; }
        public string? delivery_period { get; set; }
        public string? price { get; set; }
        public string? display_field { get; set; }
        public string? selling_price { get; set; }
        public string? discount_percentage { get; set; }
        public string? discount_amount { get; set; }
        public string? tax_percentage { get; set; }
        public string? tax_name { get; set; }
        public string? tax_name2 { get; set; }
        public string? tax_name3 { get; set; }
        public string? tax_percentage2 { get; set; }
        public string? tax_percentage3 { get; set; }
        public string? tax_amount { get; set; }
        public string? tax_amount2 { get; set; }
        public string? tax_amount3 { get; set; }
        public string? salesorder_refno { get; set; }
        public string? product_delivered { get; set; }
        public string? slno { get; set; }
        public string? product_requireddateremarks { get; set; }
        public string? margin_percentage { get; set; }
        public string? margin_amount { get; set; }
        public string? product_code { get; set; }
        public string? product_requireddate { get; set; }
    }
    public class editsalesorderdtllist : result
    {
        public string? salesorderdtl_gid { get; set; }
        public string? vendor_price { get; set; }
        public string? salesorder_gid { get; set; }
        public string? customerproduct_code { get; set; }
        public string? vendor_companyname { get; set; }
        public string? product_gid { get; set; }
        public string? productgroup_gid { get; set; }
        public string? productgroup_name { get; set; }
        public string? product_name { get; set; }
        public string? product_price { get; set; }
        public string? qty_quoted { get; set; }
        public string? product_remarks { get; set; }
        public string? uom_gid { get; set; }
        public string? uom_name { get; set; }
        public string? payment_days { get; set; }
        public string? delivery_period { get; set; }
        public string? price { get; set; }
        public string? display_field { get; set; }
        public string? selling_price { get; set; }
        public string? discount_percentage { get; set; }
        public string? discount_amount { get; set; }
        public string? tax_percentage { get; set; }
        public string? tax_name { get; set; }
        public string? tax_name2 { get; set; }
        public string? tax_name3 { get; set; }
        public string? tax_percentage2 { get; set; }
        public string? tax_percentage3 { get; set; }
        public string? tax_amount { get; set; }
        public string? tax_amount2 { get; set; }
        public string? tax_amount3 { get; set; }
        public string? salesorder_refno { get; set; }
        public string? product_delivered { get; set; }
        public string? slno { get; set; }
        public string? product_requireddateremarks { get; set; }
        public string? margin_percentage { get; set; }
        public string? margin_amount { get; set; }
        public string? product_code { get; set; }
        public string? product_requireddate { get; set; }
    }
    public class Getordertoinvoice_list : result
    {
        public string? salesorder_gid { get; set; }
        public string? shipping_to { get; set; }
        public string? so_reference { get; set; }
        public string? serviceorder_date { get; set; }
        public string? customer_name { get; set; }
        public string? grand_total { get; set; }
        public string? customer_gid { get; set; }
        public string? customercontact_name { get; set; }
        public string? email { get; set; }
        public string? customer_code { get; set; }
        public string? termsandconditions { get; set; }
        public string? customer_mobile { get; set; }
        public string? addon_amount { get; set; }
        public string? discount_amount { get; set; }
        public string? customer_address { get; set; }
        public string? order_total { get; set; }
        public string? insurance_charges { get; set; }
        public string? packing_charges { get; set; }
        public string? buyback_charges { get; set; }
        public string? freight_charges { get; set; }
        public string? currencyexchange_gid { get; set; }
        public string? customer_id { get; set; }
        public string? currency_code { get; set; }
        public string? currency_gid { get; set; }
        public string? exchange_rate { get; set; }
        public string? address1 { get; set; }
        public string? address2 { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? country_gid { get; set; }
        public string? zip_code { get; set; }
        public string? country_name { get; set; }
        public string? branch_gid { get; set; }
        public string? branch_name { get; set; }
        public string? delivery_days { get; set; }
        public string? payment_days { get; set; }
        public string? gst_number { get; set; }
        public string? tax_name { get; set; }
        public string? tax_gid { get; set; }
        public string? roundoff { get; set; }
        public string? tax_amount { get; set; }
        public string? tax_name4 { get; set; }
    }

    public class GetOrderToInvoiceProduct_list : result
    {
        public string? invoice_gid { get; set; }
        public string? invoicedtl_gid { get; set; }
        public string? customerproduct_code { get; set; }
        public string? product_gid { get; set; }
        public string? tax1_gid { get; set; }
        public string? tax2_gid { get; set; }
        public string? tax3_gid { get; set; }
        public string? qty_quoted { get; set; }
        public string? uom_gid { get; set; }
        public string? product_total { get; set; }
        public string? productprice { get; set; }
        public string? tax_amount1 { get; set; }
        public string? tax_amount2 { get; set; }
        public string? tax_amount3 { get; set; }
        public string? total_amount { get; set; }
        public string? productgroup_gid { get; set; }
        public string? productgroup_name { get; set; }
        public string? product_code { get; set; }
        public string? product_remarks { get; set; }
        public string? tax_name1 { get; set; }
        public string? tax_name2 { get; set; }
        public string? tax_name3 { get; set; }
        public string? discount_amount { get; set; }
        public string? margin_percentage { get; set; }
        public string? discount_percentage { get; set; }
        public string? product_name { get; set; }
        public string? taxsegment_gid { get; set; }
        public string? tax_percentage { get; set; }
        public string? tax_percentage2 { get; set; }
        public string? tax_percentage3 { get; set; }
        public string? uom_name { get; set; }
        public string? taxamount { get; set; }
    }
    public class ordertoinvoiceproductsubmit_list
    {
        public string? taxsegment_gid { get; set; }
        public string? taxseg_taxtotal { get; set; }
        public string? taxsegmenttax_gid { get; set; }
        public string? qty_requested { get; set; }
        public string? productquantity { get; set; }
        public string? salesorder_gid { get; set; }
        public decimal? discountprecentage { get; set; }
        public string? discount_amount { get; set; }
        public string? tax1 { get; set; }
        public string? tax2 { get; set; }
        public string? taxamount2 { get; set; }
        public decimal? taxamount1 { get; set; }
        public string? producttotal_amount { get; set; }
        public string? product_name { get; set; }
        public string? product_gid { get; set; }
        public string? productuom_gid { get; set; }
        public string? productuom_name { get; set; }
        public string? productgroup_gid { get; set; }
        public string? productgroup_name { get; set; }
        public string? product_code { get; set; }
        public string? unitprice { get; set; }
        public string? display_field { get; set; }
        public string? tax3 { get; set; }
        public bool status { get; set; }
        public string? message { get; set; }
        public string? cost_price { get; set; }
        public string? discountpercentage { get; set; }
        public string? discountamount { get; set; }
        public string? taxamount3 { get; set; }
        public string? taxgid1 { get; set; }
        public string? taxgid2 { get; set; }
        public string? taxgid3 { get; set; }
        public string? taxprecentage1 { get; set; }
        public string? taxprecentage2 { get; set; }
        public string? taxprecentage3 { get; set; }
        public string? taxname1 { get; set; }
        public string? taxname2 { get; set; }
        public string? taxname3 { get; set; }
        public string? product_desc { get; set; }
        public string? tax_prefix { get; set; }
        public string? tax_prefix2 { get; set; }
        public string? tax_prefix3 { get; set; }
    }
    public class ordertoinvoicesubmit : result
    {
        public string? customer_gid { get; set; }
        public string? invoice_gid { get; set; }
        public string? grandtotal { get; set; }
        public string? currencygid { get; set; }
        public string? invoiceref_no { get; set; }
        public string? invoice_date { get; set; }
        public string? payment_days { get; set; }
        public string? salesorder_gid { get; set; }
        public string? due_date { get; set; }
        public string? customercontactperson { get; set; }
        public string? customercontactnumber { get; set; }
        public string? customeraddress { get; set; }
        public string? customeremailaddress { get; set; }
        public string? invoicetotalamount { get; set; }
        public string? GrandTotalAmount { get; set; }
        public string? invoicediscountamount { get; set; }
        public string? addon_charge { get; set; }
        public string? additional_discount { get; set; }
        public string? remarks { get; set; }
        public string? termsandconditions { get; set; }
        public string? exchange_rate { get; set; }
        public string? branch_gid { get; set; }
        public string? roundoff { get; set; }
        public string? tax_gid { get; set; }
        public string? tax_name4 { get; set; }
        public string? tax_amount4 { get; set; }
        public string? taxsegment_gid { get; set; }
        public string? freight_charges { get; set; }
        public string? packing_charges { get; set; }
        public string? buybackcharges { get; set; }
        public string? forwardingCharges { get; set; }
        public string? insurance_charges { get; set; }
        public string? producttotalamount { get; set; }
        public string? salestype { get; set; }
        public string? dispatch_mode { get; set; }
        public string? delivery_days { get; set; }
        public string? payment { get; set; }
        public string? deliveryperiod { get; set; }
        public string? currency_code { get; set; }
        public string? shipping_to { get; set; }
        public string? sales_type { get; set; }
        public string? bill_email { get; set; }
        public string? customer_address { get; set; }
        

    }
}
