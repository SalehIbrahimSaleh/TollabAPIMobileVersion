using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace DataAccess.Services
{
    public class MyFatoorahClient
    {

        private string _apiKey;
        private string _apiUrl;
        public MyFatoorahClient()
        {

            _apiKey = WebConfigurationManager.AppSettings["MyFatoorahApiKey"];
            _apiUrl = WebConfigurationManager.AppSettings["MyFatooraApiUrl"];
        }
        public async Task<string> PerformRequest(string requestJSON, string url = "", string endPoint = "")
        {
            if (string.IsNullOrEmpty(url))
                url = _apiUrl + $"/v2/{endPoint}";
            var httpContent = new StringContent(requestJSON, System.Text.Encoding.UTF8, "application/json");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            var responseMessage = await client.PostAsync(url, httpContent).ConfigureAwait(false);
            string response = string.Empty;
            if (!responseMessage.IsSuccessStatusCode)
            {
                var rawResponse = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.SerializeObject(new
                {
                    IsSuccess = false,
                    Message = rawResponse
                });
            }
            else
            {
                response = await responseMessage.Content.ReadAsStringAsync();
            }

            return response;
        }
        public async Task<string> PerformRequest(string requestUrl)
        {
            string url = _apiUrl + requestUrl;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            var responseMessage = await client.PostAsync(url, null).ConfigureAwait(false);
            string response = string.Empty;
            if (!responseMessage.IsSuccessStatusCode)
            {
                response = JsonConvert.SerializeObject(new
                {
                    IsSuccess = false,
                    Message = responseMessage.StatusCode.ToString()
                });
            }
            else
            {
                response = await responseMessage.Content.ReadAsStringAsync();
            }

            return response;
        }

    }
}
