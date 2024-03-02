using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.Subject")]
    public class Subject
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public string Image { get; set; }
        public long? DepartmentId { get; set; }
        [Computed]
        public List<Track> Tracks { get;  set; }
    }
}
