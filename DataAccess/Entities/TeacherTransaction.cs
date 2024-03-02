using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.TeacherTransaction")]
    public class TeacherTransaction
    {

        public long Id { get; set; }
        public string Reason { get; set; }
        public string ReasonLT { get; set; }

        public long? PaymentMethodId { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreationDate { get; set; }
        public long? CourseId { get; set; }
        public long? LiveId { get; set; }
        public long? TeacherId { get; set; }
        public long? TrackId { get; set; }
        public string PromocodeNumber { get; internal set; }
        public long? CountryId { get;  set; }
    }
}
