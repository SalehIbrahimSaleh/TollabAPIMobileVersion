using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class FawryLog
    {
        public string requestId { get; set; }
        public string fawryRefNumber { get; set; }
        public string merchantRefNumber { get; set; }
        public string paymentRefrenceNumber { get; set; }
        public string customerMobile { get; set; }
        public string customerName { get; set; }
        public string customerMerchantId { get; set; }
        public string customerMail { get; set; }
        public double paymentAmount { get; set; }
        public double orderAmount { get; set; }
        public double fawryFees { get; set; }
        public object shippingFees { get; set; }
        public string orderStatus { get; set; }
        public string paymentMethod { get; set; }
        public string messageSignature { get; set; }
        public long orderExpiryDate { get; set; }
        public List<OrderItem> orderItems { get; set; }
        public ThreeDSInfo threeDSInfo { get; set; }
        public InvoiceInfo invoiceInfo { get; set; }
        public double installmentInterestAmount { get; set; }
        public int installmentMonths { get; set; }
    }
    public class InvoiceInfo
    {
        public string number { get; set; }
        public string businessRefNumber { get; set; }
        public string dueDate { get; set; }
        public long expiryDate { get; set; }
    }

    public class OrderItem
    {
        public string itemCode { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
    }

    public class ThreeDSInfo
    {
        public string eci { get; set; }
        public string xid { get; set; }
        public string enrolled { get; set; }
        public string status { get; set; }
        public string batchNumber { get; set; }
        public string command { get; set; }
        public string message { get; set; }
        public string verSecurityLevel { get; set; }
        public string verStatus { get; set; }
        public string verType { get; set; }
        public string verToken { get; set; }
        public string version { get; set; }
        public string receiptNumber { get; set; }
        public string sessionId { get; set; }
    }

}
