using Dapper.Contrib.Extensions;
using System.Collections;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    [Table("dbo.Category")]
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public long? SectionId { get; set; }
        [Computed]
        public List<SubCategory> SubCategories { get; set; }
    }
}
