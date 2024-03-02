using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("SystemSetting")]
   public class SystemSetting
    {
        public long Id { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }

    }
}
