using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
namespace DataAccess.Entities
{
    [Table("StudentAnswer")]
    public class StudentAnswer
    {
        [Key]
        public long Id { get; set; }
        public long? StudentExamId { get; set; }
        public long? ExamQuestionId { get; set; }
        public long? ExamQuestionTypeId { get; set; }
        [Computed]
        public ExamQuestion ExamQuestion { get; set; }

        public long? ExamAnswerId { get; set; }
        [Computed]
        public ExamAnswer ExamAnswer { get; set; }

        public float? Degree { get; set; }
        public string AnswerText { get; set; }
        public string TeacherCorrectance { get; set; }
        public string VoicePath { get; set; }
        public long? Duration { get; set; }
        public bool? IsTrue { get; set; }
        public DateTime? CreationDate { get; set; }
        public string PdfAnswerPath { get; set; }
        public bool? Corrected { get; set; }
    }
}
