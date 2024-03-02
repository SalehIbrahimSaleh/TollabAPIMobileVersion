using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TollabAPI.Utils
{
    public class TokenGenerator
    {
        public static string getToken()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

    }
}