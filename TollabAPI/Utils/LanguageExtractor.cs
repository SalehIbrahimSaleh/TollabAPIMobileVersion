using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace TollabAPI.Utils
{
    public class LanguageExtractor
    {
        public string getLanguage(HttpRequestMessage request)
        {
            string language = "";
            try
            {
                language = request.Headers.GetValues(AppConstants.Accept_Language).First();
            }
            catch { }

            return language;
        }
        public string getLanguage(NameValueCollection headers)
        {
            string language = "";
            try
            {
                language = headers.GetValues(AppConstants.Language).First();
            }
            catch { }

            return language;
        }
    }
}