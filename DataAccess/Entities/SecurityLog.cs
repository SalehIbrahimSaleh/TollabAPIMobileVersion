using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("SecurityLog")]
   public class SecurityLog
    {
        public long Id { get; set; }
        public string BlockReason { get; set; }
        public string BlockReasonCode { get; set; }
        public string DeviceName { get; set; }
        public long? StudentId { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
