using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Utils
{
    public class MobileNumberChecker
    {
        public static string handelMobileNumber(string PhoneNumber)
        {

            PhoneNumber = PhoneNumber.Trim();
            PhoneNumber = PhoneNumber.Replace(" ", "");
            if (PhoneNumber.StartsWith("00"))
            {
                PhoneNumber = PhoneNumber.Remove(0,2);
                PhoneNumber = "+" + PhoneNumber;
            }
            if (!PhoneNumber.StartsWith("+"))
            {

                PhoneNumber = "+" + PhoneNumber;
            }
            if (PhoneNumber.StartsWith("+2001"))
            {
                PhoneNumber=  PhoneNumber.Remove(0, 3);
                PhoneNumber = "+2" + PhoneNumber;
            }
            return PhoneNumber;

        }
    }
}
