using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.StudentTransaction")]
    public class StudentTransaction
    {
        public long Id { get; set; }
        public long? CourseId { get; set; }
        public long? LiveId { get; set; }
        public string Reason { get; set; }
        public string ReasonLT { get; set; }
        public long? PaymentMethodId { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreationDate { get; set; }
        public long? StudentId { get; set; }
        public long? TrackId { get; internal set; }
        public long? PromotionId { get; internal set; }
        public long? OfflinePackageId { get; set; }
        public string PromocodeNumber { get; internal set; }
        public string PaymentId { get; internal set; }
    }
}
