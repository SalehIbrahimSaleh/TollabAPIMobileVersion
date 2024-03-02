using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("ExamQuestionType")]
    public class ExamQuestionType
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
    }
}
