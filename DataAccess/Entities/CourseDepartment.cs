using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("dbo.CourseDepartment")]
    public class CourseDepartment
    {
        public long  Id { get; set; }
        public long CourseId { get; set; }
        public long DepartmentId { get; set; }   
    }
}
