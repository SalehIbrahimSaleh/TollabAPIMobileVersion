using Dapper.Contrib.Extensions;
namespace DataAccess.Entities
{
    [Table("ExamType")]
    public class ExamType
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
    }
}
