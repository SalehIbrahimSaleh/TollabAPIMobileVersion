using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.ContentType")]
    public class ContentType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
    }
}
