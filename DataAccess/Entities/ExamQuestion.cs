using Dapper.Contrib.Extensions;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    [Table("ExamQuestion")]
    public class ExamQuestion
    {
        [Key]
        public long Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int? OrderNumber { get; set; }
        public long? ExamQuestionTypeId { get; set; }
        public long? ExamId { get; set; }
        public float? Degree { get; set; }
        public string FilePath { get; set; }
        public string IdealAnswerFilePath { get; set; }
        [Computed]
        public List<ExamAnswer> ExamAnswers { get; set; }
    }
}
