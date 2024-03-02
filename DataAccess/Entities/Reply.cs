using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.Reply")]

    public class Reply
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public long? VideoQuestionId { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? ViewMyAccount { get; set; }
        public string Image { get; set; }
        public string Voice { get; set; }

        public long? StudentId { get; set; }
        [Computed]
        public string StudentName { get; set; }
        [Computed]
        public string StudentPhoto { get; set; }
        public long? TeacherId { get; set; }
        public long? TeacherAssistantId { get; set; }
        [Computed]
        public string TeacherName { get; set; }
        [Computed]
        public string TeacherPhoto { get; set; }
    }
}
