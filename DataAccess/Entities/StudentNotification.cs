using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.StudentNotification")]

    public class StudentNotification
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string TitleLT { get; set; }
        public long? NotificationTypeId { get; set; }
        public DateTime? CreationDate { get; set; }
        public long? CourseId { get; set; }
        public long? LiveId { get; set; }
        public long? NotificationToId { get; set; }
        public long? ReferenceId { get; set; }
        public long? StudentId { get; set; }
        public long? ContentId { get; set; }
        public long? VideoQuestionId { get; set; }
        public float? Minute { get; set; }

        [Computed]
        public Reference Reference { get;  set; }
        public bool? Seen { get;  set; }
    }
}
