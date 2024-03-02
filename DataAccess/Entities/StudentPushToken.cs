using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("StudentPushToken")]
   public class StudentPushToken
    {
        public long  Id { get; set; }
        public string Token { get; set; }
        public string OS { get; set; }
        public long StudentId { get; set; }
        public string ApplicationVersion { get; set; }
        public string DeviceVersion { get; set; }
        public DateTime? ModifiedDate  { get; set; }

    }
}
