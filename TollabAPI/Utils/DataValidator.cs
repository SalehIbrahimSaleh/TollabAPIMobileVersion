using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace TollabAPI.Utils
{
    public class DataValidator
    {
        ///////////////////// password validations ///////////////////////////////////////////////////////////
        public bool validateEmail(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }

        }
        ///////////////////// password validations ///////////////////////////////////////////////////////////

        public bool validatePasswordLength(string password)
        {
            try
            {
                if (password.Length >= 6 && password.Length <= 20)
                    return true;

                else
                    return false;

            }
            catch
            {
                return false;
            }
        }
        public bool validatePasswordConfirmation(string password, string password_confirmation)
        {
            try
            {
                if (password.Equals(password_confirmation))
                    return true;

                else
                    return false;

            }
            catch
            {
                return false;
            }
        }

        public bool validatePasswordStructure(string password)
        {
            try
            {
                return password.Any(char.IsDigit) &&
                       password.Any(char.IsUpper) &&
                       password.Any(char.IsLower) &&
                       password.Any(char.IsLetter);
            }
            catch //(Exception e)
            {
                return false;
            }
        }
        ///////////////////////////////// user name validation//////////////////////////////////////////////////

        //public bool validateUserName(string user_name)
        //{
        //    try
        //    {
        //        if (user_name.Length >= AppConstants.USER_MIN_LEGTH && user_name.Length <= AppConstants.USER_MAX_LEGTH)
        //            return true;
        //        else
        //            return false;

        //    }
        //    catch //(Exception e)
        //    {
        //        return false;
        //    }
        //}
        ///////////////////// mobile validations ///////////////////////////////////////////////////////////
        public bool validateMobile(string mobile)
        {
            try
            {
                //////////clean phone
                Regex digitsOnly = new Regex(@"[^\d]");
                mobile = digitsOnly.Replace(mobile, "");

                if (mobile.Length >= AppConstants.MOBILE_MIN_LENGTH && mobile.Length <= AppConstants.MOBILE_MAX_LENGTH)
                    return true;

                else
                    return false;

            }
            catch //(Exception e)
            {
                return false;
            }
        }

        internal static bool IsNullOrEmpty(string deviceType)
        {
            try
            {
                if (deviceType == null)
                    return true;
                else if (deviceType.SequenceEqual(""))
                    return true;
                else return false;
            }
            catch (Exception e)
            {
                return true;
            }
        }
    }
}