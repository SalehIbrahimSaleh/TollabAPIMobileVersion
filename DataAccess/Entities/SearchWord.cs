using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.SearchWord")]

    public class SearchWord
    {
        public long Id { get; set; }
        public long? StudentId { get; set; }
        public string Word { get; set; }
    }
}
