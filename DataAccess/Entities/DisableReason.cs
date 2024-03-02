using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("DisableReason")]
  public  class DisableReason
    {

        public long Id { get; set; }
        public long StudentId { get; set; }
        public string Reason { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
