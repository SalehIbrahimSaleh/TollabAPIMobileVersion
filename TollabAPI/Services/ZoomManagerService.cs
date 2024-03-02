using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Text;

namespace TollabAPI.Services
{
    public class ZoomManagerService : IZoomManagerService
    {
        public (HttpStatusCode status, string hostURL, string joinURL, long meetingId, string meetingPassword) CreateSchedualMeeting(string name, int duration, DateTime date, long countryId)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            var apiSecret = "JTpmaAaDaTtlJcjLw7pTDId899RydwLrVQ8R";
            byte[] symmetricKey = Encoding.ASCII.GetBytes(apiSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "ZU2qefs0Rp6rLTwOsPq9lQ",
                Expires = now.AddSeconds(300),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var client = new RestClient("https://api.zoom.us/v2/users/me/meetings");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            var passwordForMeeting = GetRandomPassword();
            request.AddJsonBody(new
            {
                topic = name,
                duration = duration.ToString(),
                start_time = date.ToString("yyyy-MM-ddTHH:mm:ss"),
                timezone = GetTimeZone(countryId),
                type = "2",
                password = passwordForMeeting
            });

            request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
            IRestResponse restResponse = client.Execute(request);
            HttpStatusCode statusCode = restResponse.StatusCode;
            var jObject = JObject.Parse(restResponse.Content);

            var host = (string)jObject["start_url"];
            var join = (string)jObject["join_url"];
            var id = (long)jObject["id"];
            var password = string.IsNullOrEmpty(((string)jObject["password"])) ? passwordForMeeting : ((string)jObject["password"]);
            return (statusCode, host, join, id, password);
        }
        public HttpStatusCode Delete(long meetingId)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            var apiSecret = "C8IKO7IHqUTfFaaWO7g72z7ycB9KTLjr3mrE";
            byte[] symmetricKey = Encoding.ASCII.GetBytes(apiSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "SRPZGtMSS-Wfw86avfJjqA",
                Expires = now.AddSeconds(300),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var client = new RestClient("https://api.zoom.us/v2/meetings/" + meetingId);
            var request = new RestRequest(Method.DELETE);

            request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
            IRestResponse restResponse = client.Execute(request);
            HttpStatusCode statusCode = restResponse.StatusCode;
            return statusCode;
        }

        public HttpStatusCode UpdateSchedualMeeting(string name, int duration, DateTime date, long countryId, long meetingId)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            var apiSecret = "JTpmaAaDaTtlJcjLw7pTDId899RydwLrVQ8R";
            byte[] symmetricKey = Encoding.ASCII.GetBytes(apiSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "ZU2qefs0Rp6rLTwOsPq9lQ",
                Expires = now.AddSeconds(300),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var client = new RestClient("https://api.zoom.us/v2/meetings/" + meetingId);
            var request = new RestRequest(Method.PATCH);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new
            {
                topic = name,
                duration = duration.ToString(),
                start_time = date.ToString("yyyy-MM-ddTHH:mm:ss"),
                timezone = GetTimeZone(countryId),
                type = "2"
            });

            request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
            IRestResponse restResponse = client.Execute(request);
            HttpStatusCode statusCode = restResponse.StatusCode;
            return statusCode;
        }


        private string GetTimeZone(long countryId)
        {
            switch (countryId)
            {
                case 3:
                    return "Asia/Kuwait";
                case 20011:
                    return "Africa/Cairo";
                case 20012:
                    return "Asia/Amman";
                case 20013:
                    return "Asia/Qatar";
                default:
                    return null;
            }
        }

        private string GetRandomPassword()
        {
            string password = string.Empty;
            Random randomGenerator = new Random();
            for (int i = 0; i < 6; i++)
            {
                var num = randomGenerator.Next(0, 10);
                password += num.ToString();
            }
            return password;
        }
    }
}