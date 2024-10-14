namespace StoryboardAPI.Model
{
    public class result
    {
        public bool status { get; set; }
        public string? message { get; set; }
    }
    public class token
    {
        public string? token_type { get; set; }
        public string? scope { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public string? access_token { get; set; }
    }

    public class Rootobject
    {
        public string? odatacontext { get; set; }
        public object? businessPhones { get; set; }
        public string? displayName { get; set; }
        public string? givenName { get; set; }
        public object? jobTitle { get; set; }
        public string? mail { get; set; }
        public object? mobilePhone { get; set; }
        public object? officeLocation { get; set; }
        public object? preferredLanguage { get; set; }
        public string? surname { get; set; }
        public string? userPrincipalName { get; set; }
        public string? id { get; set; }
    }

    public class userlog : result
    {
        public List<userloglist>? userloglist { get; set; }
    }

    public class userloglist
    {
        public string? businessPhones { get; set; }
        public string? displayName { get; set; }
        public string? givenName { get; set; }
        public string? jobTitle { get; set; }
        public string? mail { get; set; }
        public string? mobilePhone { get; set; }
        public string? officeLocation { get; set; }
        public string? preferredLanguage { get; set; }
        public string? surname { get; set; }
        public string? userPrincipalName { get; set; }
        public string? id { get; set; }
    }


    public class loginresponse
    {
        public string? dashboard_flag { get; set; }
        public string? token { get; set; }
        public bool status { get; set; }
        public string? message { get; set; }
        public string? user_gid { get; set; }
        public string? username { get; set; }
        public string? usercode { get; set; }
        public string? c_code { get; set; }
        public string? sref { get; set; }
        public string? k_sref { get; set; }
    }
    public class logininput
    {
        public string? code { get; set; }
    }
    public class userlogininput
    {
        public string? hostname { get; set; }
        public string? company_code { get; set; }
        public string? user_code { get; set; }
        public string? user_password { get; set; }
        public string? lawyer_email { get; set; }
    }

    public class loginERPinput
    {
        public string? user_code { get; set; }
        public string? company_code { get; set; }
    }

    public class loginVendorInput
    {
        public string? user_code { get; set; }
        public string? pass_word { get; set; }
    }

    public class appVendorInput
    {
        public string? app_code { get; set; }
        public string? password { get; set; }
    }
    //public class Mdladminlogin : result
    //{
    //    public string? user_code { get; set; }
    //    public string? user_password { get; set; }
    //    public string? company_code { get; set; }
    //}
    public class PostUserLogin
    {
        public string? user_code { get; set; }
        public string? user_password { get; set; }
        public string? company_code { get; set; }

    }
    public class PostUserForgot : result
    {
        public string? confirmpassword_reset { get; set; }
        public string? companyid { get; set; }
        public string? customer_code { get; set; }
        public string? forgot_pwd { get; set; }
        public string? usercode { get; set; }
        public string? forgotportal_emailid { get; set; }

    }
    public class PostUserReset : result
    {

        public string? confirmpassword_reset { get; set; }
        public string? companyid_reset { get; set; }
        public string? old_password { get; set; }
        public string? password { get; set; }
        public string? usercode_reset { get; set; }

    }

    public class otplogin
    {
        //internal string mobile_number;
        public string? employee_emailid { get; set; }
        public string? employee_mobileno { get; set; }
        public string? message { get; set; }
        public string? otpvalue { get; set; }
        public string? created_time { get; set; }
        public bool status { get; set; }


    }
    public class otpverify : PostUserLogin
    {
        //internal string mobile_number;
        public string? employee_emailid { get; set; }
        public string? employee_mobileno { get; set; }
        public string? message { get; set; }
        public string? otpvalue { get; set; }
        public bool status { get; set; }

    }
    public class otpverifyresponse
    {
        //internal string mobile_number;
        public string? token { get; set; }
        public string? employee_emailid { get; set; }
        public string? employee_mobileno { get; set; }
        public string? message { get; set; }
        public string? otpvalue { get; set; }
        public bool status { get; set; }
        public string? user_gid { get; set; }
        public string? dashboard_flag { get; set; }

        public string? c_code { get; set; }
        public string? sref { get; set; }
        public string? k_sref { get; set; }


    }

    public class otpresponse
    {
        public string? otp_flag { get; set; }
    }

    public class mdlIncomingMessage
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public Body body { get; set; }
        public string? channelId { get; set; }
        public string? conversationMessageId { get; set; }
        public DateTime createdAt { get; set; }
        public string? direction { get; set; }
        public DateTime lastStatusAt { get; set; }
        public string? messageId { get; set; }
        public Meta meta { get; set; }
        public string? platformId { get; set; }
        public string? platformReferenceId { get; set; }
        public Receiver receiver { get; set; }
        public object reference { get; set; }
        public Sender sender { get; set; }
        public string? status { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class Body
    {
        public string? type { get; set; }
        public Image image { get; set; }
        public Text text { get; set; }
        public list list { get; set; }
        public Files file { get; set; }
        public Location2 location { get; set; }

    }

    public class Files
    {
        public File1[] files { get; set; }
    }

    public class File1
    {
        public string? contentType { get; set; }
        public string? mediaUrl { get; set; }
        public Metadata metadata { get; set; }
    }

    public class Metadata
    {
        public bool isAnimated { get; set; }
    }

    public class list
    {
        public string? text { get; set; }
        public string? altText { get; set; }
    }

    public class Image
    {
        public string? text { get; set; }
        public Image1[] images { get; set; }
    }

    public class Image1
    {
        public string? mediaUrl { get; set; }
        public string? altText { get; set; }
    }

    public class Text
    {
        public string? text { get; set; }
    }

    public class Meta
    {
        public Extrainformation extraInformation { get; set; }
        public Order order { get; set; }
    }

    public class Order
    {
        public Product[] products { get; set; }
    }

    public class Product
    {
        public string? externalCatalogId { get; set; }
        public string? externalProductId { get; set; }
        public Price price { get; set; }
        public int quantity { get; set; }
    }

    public class Price
    {
        public double amount { get; set; }
        public string? currencyCode { get; set; }
        public double exponent { get; set; }
    }


    public class Extrainformation
    {
        public string? conversation_id { get; set; }
        public string? cost_request_id { get; set; }
        public string? useCase { get; set; }
        public string? use_wa_platform_account_id_approach { get; set; }
        public string? timestamp { get; set; }
    }

    public class Receiver
    {
        public Contact1[] contacts { get; set; }
        public Connector1 connector { get; set; }
    }

    public class Connector1
    {
        public string? id { get; set; }
        public string? identifierValue { get; set; }
    }

    public class Contact1
    {
        public string? id { get; set; }
        public string? identifierKey { get; set; }
        public string? identifierValue { get; set; }
        public string? type { get; set; }
    }

    public class Sender
    {
        public Contact contact { get; set; }
        public Participant participant { get; set; }
    }

    public class Contact
    {
        public string? contactId { get; set; }
        public string? identifierKey { get; set; }
        public string? identifierValue { get; set; }
    }


    public class incomingMail
    {
        public Msys msys { get; set; }
    }

    public class Msys
    {
        public Relay_Message relay_message { get; set; }
    }

    public class Relay_Message
    {
        public Content content { get; set; }
        public string? customer_id { get; set; }
        public string? friendly_from { get; set; }
        public string? msg_from { get; set; }
        public string? rcpt_to { get; set; }
        public string? webhook_id { get; set; }
    }

    public class Content
    {
        public string? email_rfc822 { get; set; }
        public bool email_rfc822_is_base64 { get; set; }
        public Header[] headers { get; set; }
        public string? html { get; set; }
        public string? subject { get; set; }
        public string? text { get; set; }
        public string?[] to { get; set; }
    }

    public class Header
    {
        public string? ReturnPath { get; set; }
        public string? MIMEVersion { get; set; }
        public string? From { get; set; }
        public string? Received { get; set; }
        public string? Date { get; set; }
        public string? MessageID { get; set; }
        public string? Subject { get; set; }
        public string? To { get; set; }
    }

    public class sbscontactus
    {
        public string? email { get; set; }
        public string? fname { get; set; }
        public string? phone { get; set; }
        public string? company { get; set; }
        public string? message { get; set; }
        public List<productsmail_list> products { get; set; }
    }

    public class productsmail_list
    {
        public int id { get; set; }
        public string? name { get; set; }
    }

    public class graphLoginSuccessResponse
    {
        public string? token_type { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public string? access_token { get; set; }
    }

    public class graphtoken
    {
        public string? access_token { get; set; }
        public bool status { get; set; }
    }


    public class MdlGraphMailContent
    {
        public Message1 message { get; set; }
        public string? saveToSentItems { get; set; }
    }

    public class Message1
    {
        public string? subject { get; set; }
        public Body1 body { get; set; }
        public Torecipient[] toRecipients { get; set; }
    }

    public class Body1
    {
        public string? contentType { get; set; }
        public string? content { get; set; }
    }

    public class Torecipient
    {
        public Emailaddress emailAddress { get; set; }
    }

    public class Emailaddress
    {
        public string? address { get; set; }
    }

    public class MdlStoryboardSystems
    {
        public string? customer_name { get; set; }
        public string? customer_number { get; set; }
    }

    public class clicktocall
    {
        public string? station { get; set; }
        public string? phone_number { get; set; }
        public string? didnumber { get; set; }
        public string? status { get; set; }
        public string? direction { get; set; }
        public string? uniqueid { get; set; }
        public string? duration { get; set; }
        public string? start_time { get; set; }
        public string? answer_time { get; set; }
        public string? end_time { get; set; }
        public string? recording_path { get; set; }
        public string? hangup_by { get; set; }
        public agent agent { get; set; }
    }

    public class agent
    {
        public string? user { get; set; }
        public string? value { get; set; }
    }

    public class click2callresponse
    {
        public int statusCode { get; set; }
        public bool error { get; set; }
        public string? message { get; set; }
        public string? uid { get; set; }
    }

    public class click2callerrorresponse
    {
        public int statusCode { get; set; }
        public string? error { get; set; }
        public string? message { get; set; }
    }

    public class Result
    {
        public string? id { get; set; }
        public string? channelId { get; set; }
        public Sender sender { get; set; }
        public Receiver receiver { get; set; }
        public Body body { get; set; }
        public Meta meta { get; set; }
        public Template template { get; set; }
        public string? reference { get; set; }
        public string? status { get; set; }
        public string? reason { get; set; }
        public string? direction { get; set; }
        public DateTime lastStatusAt { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class Template
    {
        public string? projectId { get; set; }
        public string? version { get; set; }
        public string? locale { get; set; }
        public object variables { get; set; }
    }


    public class MdlCalendy
    {
        public DateTime created_at { get; set; }
        public string? created_by { get; set; }
        public string? _event { get; set; }
        public Payload payload { get; set; }
    }

    public class Payload
    {
        public string? cancel_url { get; set; }
        public DateTime created_at { get; set; }
        public string? email { get; set; }
        public string? _event { get; set; }
        public object first_name { get; set; }
        public object invitee_scheduled_by { get; set; }
        public object last_name { get; set; }
        public string? name { get; set; }
        public object new_invitee { get; set; }
        public object no_show { get; set; }
        public object old_invitee { get; set; }
        public object payment { get; set; }
        public object[] questions_and_answers { get; set; }
        public object reconfirmation { get; set; }
        public string? reschedule_url { get; set; }
        public bool rescheduled { get; set; }
        public object routing_form_submission { get; set; }
        public Scheduled_Event scheduled_event { get; set; }
        public object scheduling_method { get; set; }
        public string? status { get; set; }
        public object text_reminder_number { get; set; }
        public string? timezone { get; set; }
        public Tracking tracking { get; set; }
        public DateTime updated_at { get; set; }
        public string? uri { get; set; }
    }

    public class Scheduled_Event
    {
        public DateTime created_at { get; set; }
        public DateTime end_time { get; set; }
        public Event_Guests[] event_guests { get; set; }
        public Event_Memberships[] event_memberships { get; set; }
        public string? event_type { get; set; }
        public Invitees_Counter invitees_counter { get; set; }
        public Location location { get; set; }
        public object meeting_notes_html { get; set; }
        public object meeting_notes_plain { get; set; }
        public string? name { get; set; }
        public DateTime start_time { get; set; }
        public string? status { get; set; }
        public DateTime updated_at { get; set; }
        public string? uri { get; set; }
    }

    public class Invitees_Counter
    {
        public int total { get; set; }
        public int active { get; set; }
        public int limit { get; set; }
    }

    public class Location
    {
        public string? join_url { get; set; }
        public string? status { get; set; }
        public string? type { get; set; }
        public string? location { get; set; }

    }

    public class Event_Guests
    {
        public DateTime created_at { get; set; }
        public string? email { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Event_Memberships
    {
        public string? user { get; set; }
        public string? user_email { get; set; }
        public string? user_name { get; set; }
    }

    public class Tracking
    {
        public object utm_campaign { get; set; }
        public object utm_source { get; set; }
        public object utm_medium { get; set; }
        public object utm_content { get; set; }
        public object utm_term { get; set; }
        public object salesforce_uuid { get; set; }
    }

    public class MdlMetaProducts
    {
        public string? company_code { get; set; }
        public string? access_token { get; set; }
        public string? catalog_id { get; set; }
    }


    public class MdlMetaProductsRespose
    {
        public Datum[] data { get; set; }
        public Paging paging { get; set; }
    }

    public class Paging
    {
        public Cursors cursors { get; set; }
        public string? next { get; set; }
        public string? previous { get; set; }
    }

    public class Cursors
    {
        public string? before { get; set; }
        public string? after { get; set; }
    }

    public class Datum
    {
        public string? name { get; set; }
        public string? availability { get; set; }
        public string? currency { get; set; }
        public string? price { get; set; }
        public string? condition { get; set; }
        public string? id { get; set; }
        public string? retailer_id { get; set; }
    }

    public class Location2
    {
        public Coordinates coordinations { get; set; }
        public Location1 location { get; set; }
    }

    public class Coordinates
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class Location1
    {
        public string? address { get; set; }
        public string? label { get; set; }
    }

    public class MdlIncomingOrder : result
    {

        public string? email { get; set; }
        public string? order_id { get; set; }
        public string? order_gid { get; set; }
        public string? address { get; set; }

    }
    public class mdlDynamicDBCreation : result
    {
        public string? otp { get; set; }
    }

    public class MdlOrderDispatchMode : result
    {
        public string? order_gid { get; set; }
        public string? dispatch_mode { get; set; }
        public string? pin_code { get; set; }
        public string? address { get; set; }
        public string? delivery_cost { get; set; }
        public string? c_code { get; set; }
        public string? order_instructions { get; set; }
        public string? payment_link { get; set; }
        public string? payment_mode { get; set; }
    }

    public class whatsappconfiguration1
    {
        public string? workspace_id { get; set; }
        public string? channel_id { get; set; }
        public string? access_token { get; set; }
        public string? channelgroup_id { get; set; }
    }

    public class MdlContactResponse
    {
        public string? id { get; set; }
        public string? computedDisplayName { get; set; }
        public Featuredidentifier[] featuredIdentifiers { get; set; }
        public int identifierCount { get; set; }
        public Attributes attributes { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class Attributes
    {
        public string? displayName { get; set; }
        public string? initialSource { get; set; }
        public string?[] phonenumber { get; set; }
        public string?[] reachableIdentifiersForSmsMarketing { get; set; }
        public string?[] reachableIdentifiersForSmsTransactional { get; set; }
        public string?[] reachableIdentifiersForWhatsAppMarketing { get; set; }
        public string?[] reachableIdentifiersForWhatsAppTransactional { get; set; }
        public bool subscribedWhatsApp { get; set; }
    }

    public class Featuredidentifier
    {
        public string? key { get; set; }
        public string? value { get; set; }
    }

    public class Participant
    {
        public string? avatarUrl { get; set; }
        public DateTime createdAt { get; set; }
        public string? displayName { get; set; }
        public string? participantId { get; set; }
        public string? participantType { get; set; }
        public string? status { get; set; }
        public DateTime updatedAt { get; set; }
    }


    public class stripeproductResponse
    {
        public string? id { get; set; }
        public string? _object { get; set; }
        public bool active { get; set; }
        public int created { get; set; }
        public object default_price { get; set; }
        public object description { get; set; }
        public object[] images { get; set; }
        public object[] features { get; set; }
        public bool livemode { get; set; }
        public Metadata1 metadata { get; set; }
        public string? name { get; set; }
        public object package_dimensions { get; set; }
        public object shippable { get; set; }
        public object statement_descriptor { get; set; }
        public object tax_code { get; set; }
        public object unit_label { get; set; }
        public int updated { get; set; }
        public object url { get; set; }
    }

    public class Metadata1
    {
    }

    public class stripePriceResponse
    {
        public string? id { get; set; }
        public string? _object { get; set; }
        public bool active { get; set; }
        public string? billing_scheme { get; set; }
        public int created { get; set; }
        public string? currency { get; set; }
        public object custom_unit_amount { get; set; }
        public bool livemode { get; set; }
        public object lookup_key { get; set; }
        public Metadata2 metadata { get; set; }
        public object nickname { get; set; }
        public string? product { get; set; }
        public Recurring recurring { get; set; }
        public string? tax_behavior { get; set; }
        public object tiers_mode { get; set; }
        public object transform_quantity { get; set; }
        public string? type { get; set; }
        public int unit_amount { get; set; }
        public string? unit_amount_decimal { get; set; }
    }

    public class Metadata2
    {
    }

    public class Recurring
    {
        public object aggregate_usage { get; set; }
        public string? interval { get; set; }
        public int interval_count { get; set; }
        public object trial_period_days { get; set; }
        public string? usage_type { get; set; }
    }

    public class MdlStripeProducts
    {
        public string? c_code { get; set; }
        public string? table { get; set; }
    }

    public class MdlStripePaymentLink
    {
        public string? id { get; set; }
        public string? _object { get; set; }
        public bool active { get; set; }
        public After_Completion after_completion { get; set; }
        public bool allow_promotion_codes { get; set; }
        public object application { get; set; }
        public object application_fee_amount { get; set; }
        public object application_fee_percent { get; set; }
        public Automatic_Tax automatic_tax { get; set; }
        public string? billing_address_collection { get; set; }
        public object consent_collection { get; set; }
        public string? currency { get; set; }
        public object[] custom_fields { get; set; }
        public Custom_Text custom_text { get; set; }
        public string? customer_creation { get; set; }
        public object inactive_message { get; set; }
        public Invoice_Creation invoice_creation { get; set; }
        public bool livemode { get; set; }
        public object on_behalf_of { get; set; }
        public object payment_intent_data { get; set; }
        public string? payment_method_collection { get; set; }
        public object payment_method_types { get; set; }
        public Phone_Number_Collection phone_number_collection { get; set; }
        public object restrictions { get; set; }
        public object shipping_address_collection { get; set; }
        public object[] shipping_options { get; set; }
        public string? submit_type { get; set; }
        public object subscription_data { get; set; }
        public Tax_Id_Collection tax_id_collection { get; set; }
        public object transfer_data { get; set; }
        public string? url { get; set; }
    }

    public class After_Completion
    {
        public Hosted_Confirmation hosted_confirmation { get; set; }
        public string? type { get; set; }
    }

    public class Hosted_Confirmation
    {
        public object custom_message { get; set; }
    }

    public class Automatic_Tax
    {
        public bool enabled { get; set; }
        public object liability { get; set; }
    }

    public class Custom_Text
    {
        public object after_submit { get; set; }
        public object shipping_address { get; set; }
        public object submit { get; set; }
        public object terms_of_service_acceptance { get; set; }
    }

    public class Invoice_Creation
    {
        public bool enabled { get; set; }
        public Invoice_Data invoice_data { get; set; }
    }

    public class Invoice_Data
    {
        public object account_tax_ids { get; set; }
        public object custom_fields { get; set; }
        public object description { get; set; }
        public object footer { get; set; }
        public object issuer { get; set; }
        public object rendering_options { get; set; }
    }

    public class Phone_Number_Collection
    {
        public bool enabled { get; set; }
    }

    public class Tax_Id_Collection
    {
        public bool enabled { get; set; }
    }
    public class paymentgatewayconfiguration1
    {
        public string? razorpay_accname { get; set; }
        public string? razorpay_accpwd { get; set; }
        public string? stripe_key { get; set; }
        public string? payment_gateway { get; set; }
    }


    public class MdlRazorpayResponse
    {
        public bool accept_partial { get; set; }
        public int amount { get; set; }
        public int amount_paid { get; set; }
        public string? callback_method { get; set; }
        public string? callback_url { get; set; }
        public int cancelled_at { get; set; }
        public int created_at { get; set; }
        public string? currency { get; set; }
        public object[] customer { get; set; }
        public string? description { get; set; }
        public int expire_by { get; set; }
        public int expired_at { get; set; }
        public int first_min_partial_amount { get; set; }
        public string? id { get; set; }
        public object notes { get; set; }
        public Notify notify { get; set; }
        public object payments { get; set; }
        public string? reference_id { get; set; }
        public bool reminder_enable { get; set; }
        public object[] reminders { get; set; }
        public string? short_url { get; set; }
        public string? status { get; set; }
        public int updated_at { get; set; }
        public bool upi_link { get; set; }
        public string? user_id { get; set; }
        public bool whatsapp_link { get; set; }
    }

    public class Notify
    {
        public bool email { get; set; }
        public bool sms { get; set; }
        public bool whatsapp { get; set; }
    }

}