using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.SystemTransaction")]
    public class SystemTransaction
    {
        public long Id { get; set; }
        public string Reason { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreationDate { get; set; }
        public string ReferenceNumber { get; set; }

    }
}
