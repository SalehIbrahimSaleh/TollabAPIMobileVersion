using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.StudentCourseContent")]
    public class StudentCourseContent
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public long CourseId { get; set; }
        public long ContentId { get; set; }
    }
}
