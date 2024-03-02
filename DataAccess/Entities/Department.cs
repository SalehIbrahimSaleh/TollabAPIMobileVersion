using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.Department")]

    public class Department
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public long? SubCategoryId { get; set; }
    }
}
