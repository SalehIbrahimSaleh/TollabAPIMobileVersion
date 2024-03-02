using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.Section")]

    public class Section
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public string Image { get; set; }
        public long? CountryId { get; set; }
        [Computed]
        public IEnumerable<Category> Categories { get;  set; }
    }
}
