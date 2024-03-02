using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
   public class StudentTokens
    {
        public IEnumerable<StudentPushToken>  IosStudentPushTokens { get; set; }
        public IEnumerable<StudentPushToken>  AndroidStudentPushTokens { get; set; }
    }
}
