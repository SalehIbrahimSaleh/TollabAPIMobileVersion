using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.Favourite")]
    public class Favourite
    {
        public long Id { get; set; }
        public long? StudentId { get; set; }
        public long? CourseId { get; set; }
    }
}
