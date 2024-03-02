using Dapper.Contrib.Extensions;


namespace DataAccess.Entities
{
    [Table("SolveStatus")]
    public class SolveStatus
    {
         [Key]
         public long Id { get; set; }
         public string Name { get; set; }
         public string NameLT { get; set; }
         public string Color { get; set; }
    }
}
