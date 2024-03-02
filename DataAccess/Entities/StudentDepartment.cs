using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.StudentDepartment")]
    public class StudentDepartment
    {
        public long Id { get; set; }
        public long? StudentId { get; set; }
        public long? DepartmentId { get; set; }
    }
}
