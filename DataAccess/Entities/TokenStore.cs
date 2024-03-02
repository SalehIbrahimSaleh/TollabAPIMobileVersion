using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("TokenStore")]
   public class TokenStore
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string Token { get; set; }
        public bool Valid { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
