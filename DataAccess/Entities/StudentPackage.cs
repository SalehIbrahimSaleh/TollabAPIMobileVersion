using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.StudentPackage")]
    public class StudentPackage
    {
        public long Id { get; set; }
        public long? StudentId { get; set; }
        public long? PackageId { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public int? CompletionPercentage { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }
    }
}
