using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.PaymentMethod")]
    public class PaymentMethod
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
    }
}
