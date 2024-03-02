using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.StudentPromotion")]
    public class StudentPromotion
    {
        public long Id { get; set; }
        public long? StudentId { get; set; }
        public long? PromotionId { get; set; }
        public DateTime? EnrollmentDate { get; set; }
         public string ReferenceNumber { get; set; }
    }
}
