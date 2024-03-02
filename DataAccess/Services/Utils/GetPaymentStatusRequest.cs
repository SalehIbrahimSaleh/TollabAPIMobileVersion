using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Utils
{
    public class GetPaymentStatusRequest
    {

        public string Key { get; set; }

        public string KeyType { get; set; } // InvoiceId  PaymentId
    }
    public enum TransactionType
    {
        Track = 1,
        Course,
        Live
    }
    public class GetPaymentStatusResponse
    {

        public long InvoiceId { get; set; }
        public string InvoiceStatus { get; set; }
        public string InvoiceReference { get; set; }
        public string CustomerReference { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ExpiryDate { get; set; }
        public decimal InvoiceValue { get; set; }
        public string Comments { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
        public string UserDefinedField { get; set; }
        public string InvoiceDisplayValue { get; set; }

    }
}
