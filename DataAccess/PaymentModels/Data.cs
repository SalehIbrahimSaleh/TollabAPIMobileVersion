using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.PaymentModels
{
    [Table("[Data]")]
    public class Data
    {
        public string InvoiceDisplayValue { get; set; }
        public string UserDefinedField { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerName { get; set; }
        public string Comments { get; set; }
        public decimal? InvoiceValue { get; set; }
        public string ExpiryDate { get; set; }
        public string CreatedDate { get; set; }
        public string InvoiceReference { get; set; }
        public string InvoiceStatus { get; set; }
        public long? InvoiceId { get; set; }
        [Computed]
        public List<InvoiceTransaction> InvoiceTransactions { get; set; }
        public long? BankResponseId { get; internal set; }
    }
}
