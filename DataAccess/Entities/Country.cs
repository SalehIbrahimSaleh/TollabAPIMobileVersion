using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.Country")]
    public class Country
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public string Flag { get; set; }
        public string Code { get; set; }
        public string Currency { get; set; }
        public string CurrencyLT { get; set; }
        public string CountryCode { get; set; }
        public int LengthPhoneNumebr { get; set; }
        public string WhatsappNumber { get; set; }
        
    }
}
