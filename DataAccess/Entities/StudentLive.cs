using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.StudentLive")]
    public class StudentLive
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public long LiveId { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
