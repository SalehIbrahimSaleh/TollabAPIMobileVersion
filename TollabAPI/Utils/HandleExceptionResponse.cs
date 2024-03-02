using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TollabAPI.Utils
{
    public class HandleExceptionResponse
    {

        public string getMessage(Exception exception)
        {
            string message = exception.Message.ToString().ToLower();
            if (message.Contains("unique"))
            {
                return message.Substring(message.IndexOf("(") + 1, (message.IndexOf(")") - message.IndexOf("(") - 1)) + "  allready exists";
            }
            else
            {
                return "internal server error";
            }
        }
    }
}