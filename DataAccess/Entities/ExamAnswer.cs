using Dapper.Contrib.Extensions;
namespace DataAccess.Entities
{
    [Table("ExamAnswer")]
    public class ExamAnswer
    {
        [Key]
        public long Id { get; set; }
        public long? ExamQuestionId { get; set; }
        public string Answer { get; set; }
        public bool? IsTrue { get; set; }
    }
}
