using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Views
{
   public class LoginAndoridModel
    {
      public string PhoneNumberWithKey { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CheckToken { get; set; }
      public string State { get; set; }
    }
    public class ForgotPasswordModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
