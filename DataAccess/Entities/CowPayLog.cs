using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("CowPayLog")]
  public  class CowPayLog
    {
        public long Id { get; set; }
        public string cowpay_reference_id { get; set; }
        public string payment_gateway_reference_id { get; set; }
        public string customer_merchant_profile_id { get; set; }
        public string merchant_reference_id { get; set; }
        public string merchant_code { get; set; }
        public string order_status   { get; set; }
        public string amount { get; set; }
        public string signature { get; set; }
        public string callback_type { get; set; }
        public string channel { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
