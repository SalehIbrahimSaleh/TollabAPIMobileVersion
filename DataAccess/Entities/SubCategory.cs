using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.SubCategory")]
    public class SubCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public long? CategoryId { get; set; }
        [Computed]
        public IEnumerable<Department> Departments { get; internal set; }
    }
}
