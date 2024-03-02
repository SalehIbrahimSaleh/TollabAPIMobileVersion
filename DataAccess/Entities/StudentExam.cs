using System;
using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("StudentExam")]
    public class StudentExam
    {
        [Key]
        public long Id { get; set; }
        public long? StudentId { get; set; }
        public long? ExamId { get; set; }
        public long? SolveStatusId { get; set; }
        public int? TotalScore { get; set; }
        public DateTime? CreationDate { get; set; }
        public long? TeacherAssistantId { get; set; }
        [Computed]
        public int? Duration { get; set; }
    }
}
