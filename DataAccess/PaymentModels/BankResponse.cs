using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.PaymentModels
{
    [Table("dbo.BankResponse")]
    public class BankResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ValidationErrors { get; set; }
        [Computed]
        public Data Data { get; set; }

    }
}
