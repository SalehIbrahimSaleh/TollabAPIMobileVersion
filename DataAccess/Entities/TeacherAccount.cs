using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.TeacherAccount")]
    public class TeacherAccount
    {
        public long Id { get; set; }
        public long? TeacherId { get; set; }
        public string AccountNumber { get; set; }
        public string Note { get; set; }
    }
}
