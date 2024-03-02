using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Views
{
   public class GetprofileModel
    {
        public string CheckToken { get; set; }
        public string State { get; set; }
        public string AppVersion { get; set; }
        public string DeviceName { get; set; }
        public string OS { get; set; }

    }
}
