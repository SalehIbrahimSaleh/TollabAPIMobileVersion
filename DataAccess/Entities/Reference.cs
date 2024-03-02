using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("[Reference]")]
   public class Reference
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
    }
}
