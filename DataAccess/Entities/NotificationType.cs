using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.NotificationType")]
    public class NotificationType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
    }
}
