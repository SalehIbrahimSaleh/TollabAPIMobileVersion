using Dapper.Contrib.Extensions;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    [Table("dbo.[Group]")]
    public class Group
    {
        public long Id { get; set; }
        public string GroupCourse { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public long? CourseId { get; set; }
        [Computed]
        public bool LockCourseContent { get; set; }
        [Computed]
        public bool AnswerExam { get; set; }
        [Computed]
        public List<Content> Contents { get; set; }
        public int OrderNumber { get;  set; }
    }
}
