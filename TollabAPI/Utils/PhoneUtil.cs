using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TollabAPI.Utils
{
    public class PhoneUtil
    {
        //internal static Parent separateKeyAndPhone(Parent parent)
        //{
        //    parent.Phone = handelMobileNumber(parent.Phone);
        //    foreach (var item in AppConstants.getCountryCodes())
        //    {
        //        if (parent.Phone.StartsWith(item))
        //        {
        //            parent.PhoneKey = item;
        //            parent.Phone = parent.Phone.Substring(item.Length);
        //        }
        //    }
        //    return parent;
        //}
        internal static string separateKeyAndPhone(string Phone)
        {
            Phone = handelMobileNumber(Phone);
            foreach (var item in AppConstants.getCountryCodes())
            {
                if (Phone.StartsWith(item))
                {
                    Phone = Phone.Substring(item.Length);
                }
            }
            return Phone;
        }

        public static string handelMobileNumber(string PhoneNumber)
        {

            PhoneNumber = PhoneNumber.Trim();

            if (PhoneNumber.StartsWith("00"))
            {
                PhoneNumber = PhoneNumber.Substring(2);
                PhoneNumber = "+" + PhoneNumber;
            }
            //if (!PhoneNumber.StartsWith("+"))
            //{

            //    PhoneNumber = "+" + PhoneNumber;
            //}
            return PhoneNumber;

        }

    }
}