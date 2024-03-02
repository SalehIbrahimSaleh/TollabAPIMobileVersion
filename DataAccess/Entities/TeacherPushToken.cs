using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("TeacherPushToken")]
   public class TeacherPushToken
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public string OS { get; set; }
        public long TeacherId { get; set; }
        public string ApplicationVersion { get; set; }
        public string DeviceVersion { get; set; }


    }
}
