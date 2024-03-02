using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace TollabAPI.Utils
{
    public class CustomeResponseMessage
    {
        private HttpResponseMessage responseMessage = new HttpResponseMessage();
        private Dictionary<string, dynamic> responsebody = new Dictionary<string, dynamic>();

        public CustomeResponseMessage()
        {
            responsebody.Add(AppConstants.Model, new List<string>());
            responsebody.Add(AppConstants.Metas, new Dictionary<string, dynamic>());
            responsebody.Add(AppConstants.Error, new Dictionary<string, dynamic>());

        }

        public HttpResponseMessage getResponseMessage(HttpStatusCode code)
        {
            try
            {
                responseMessage.StatusCode = code;
                responseMessage.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(responsebody).ToString());
                responseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return responseMessage;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }



        public void AddModel(string key, dynamic value)
        {
            try
            {
                responsebody.Remove(AppConstants.Model);
                responsebody.Add(AppConstants.Model, value);
            }
            catch
            {

            }

        }

       

        public void AddMeta(string key, dynamic value)
        {
            Dictionary<string, dynamic> MetasParent = new Dictionary<string, dynamic>();
            if (!responsebody.ContainsKey(AppConstants.Metas))
            {
                responsebody.Add(AppConstants.Metas, MetasParent);
            }

            foreach (KeyValuePair<string, dynamic> Metas in responsebody)
            {
                if (Metas.Key.ToString().Equals(AppConstants.Metas))
                {
                    MetasParent = Metas.Value;

                }
            }
            try
            {
                MetasParent.Add(key, value);
            }
            catch { }
            responsebody.Remove(AppConstants.Metas);
            responsebody.Add(AppConstants.Metas, MetasParent);
        }

        internal void AddModel(object user, object p)
        {
            throw new NotImplementedException();
        }

        public void AddError(string key, dynamic value)
        {
            Dictionary<string, dynamic> ErrorParent = new Dictionary<string, dynamic>();
            if (!responsebody.ContainsKey(AppConstants.Error))
            {
                responsebody.Add(AppConstants.Error, ErrorParent);
            }

            foreach (KeyValuePair<string, dynamic> Error in responsebody)
            {
                if (Error.Key.ToString().Equals(AppConstants.Error))
                {
                    ErrorParent = Error.Value;

                }
            }
            try
            {
                ErrorParent.Add(key, value);
            }
            catch { }
            responsebody.Remove(AppConstants.Error);
            responsebody.Add(AppConstants.Error, ErrorParent);
        }

        public HttpResponseMessage getResponseMessage(HttpStatusCode code, JObject response_string)
        {
            try
            {
                responseMessage.StatusCode = code;
                responseMessage.Content = new StringContent(response_string.ToString());
                responseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return responseMessage;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public void clearBody()
        {
            responsebody.Clear();
            responsebody.Add(AppConstants.Model, new List<string>());
            responsebody.Add(AppConstants.Metas, new Dictionary<string, dynamic>());
            responsebody.Add(AppConstants.Error, new Dictionary<string, dynamic>());
        }

        
    }

}