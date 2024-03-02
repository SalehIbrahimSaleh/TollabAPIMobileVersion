using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("dbo.PaymentMethodType")]
    public class PaymentMethodType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public long CountryId { get; set; }
        public bool? InAppPurchase { get; set; }
        public double? TheFactor { get; set; }

    }
}
