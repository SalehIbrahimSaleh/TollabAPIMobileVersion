using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.MyFatoorahPaymentLog")]
    public class MyFatoorahPaymentLog
    {
        [Key]
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public bool IsResponded { get; set; }
        public bool IsChecked { get; set; }
        public DateTime? LastChecked { get; set; }
        public string LastStatus { get; set; }

    }
}
