using DataAccess.Entities;
using DataAccess.Services;
using DataAccess.UnitOfWork;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TollabAPI.Utils;

namespace TollabAPI.Controllers
{
    public class MetaDataController : ApiController
    {
        MetaDataService _metaDataService;
        StudentService _studentService;
      
        private CustomeResponseMessage response = new CustomeResponseMessage();

        public MetaDataController()
        {
            _metaDataService = new MetaDataService();
            _studentService = new StudentService();
        }
    
        public async Task<HttpResponseMessage> GetAllIOS()
        {
            MetaData metaData = new MetaData();
            // doPayment();
            try
            {
                var resultData = await _metaDataService.GetMetaDataIOS();
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    return response.getResponseMessage(HttpStatusCode.OK);
                }
                response.AddModel(AppConstants.Metas, resultData);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Returned Successfuly");

                return response.getResponseMessage(HttpStatusCode.OK);

            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");

                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
        public async Task<HttpResponseMessage> GetOTPByVideoId(string videoId)
        {
            var url = string.Format("https://dev.vdocipher.com/api/videos/{0}/otp", videoId);
            var clients = new RestClient(url);
            var requests = new RestRequest(Method.POST);
            requests.AddHeader("Content-Type", "application/json");
            requests.AddHeader("Accept", "application/json");
            requests.AddHeader("Authorization", "Apisecret gZKDngGYS8bcAb2Xpl8i97qTs75K9pjVaRq6na8LdlLRASOF7OAVqddrB2cy9Ep0");
            requests.AddParameter("undefined", "{\n\t\"ttl\":300\n}", ParameterType.RequestBody);
           
             
            try
            {
                var resultData = clients.Execute(requests);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    return response.getResponseMessage(HttpStatusCode.OK);
                }
                JsonSerializerSettings serSettings = new JsonSerializerSettings();
                serSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                var obj = JsonConvert.DeserializeObject<OTPVideoDto>(resultData.Content, serSettings);

                response.AddModel(AppConstants.OTP, obj );
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Returned Successfuly");

                return response.getResponseMessage(HttpStatusCode.OK);

            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");

                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
        
        [HttpGet]
        public async Task<HttpResponseMessage> GetOTPForOfflineByVideoId(long courseId)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Student not found");
                    response.AddError(AppConstants.Code, AppConstants.Student_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var StudentData = await _studentService.GetStudentByIdentityIdAsync(IdentityUserId);
                if (StudentData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Student not found");
                    response.AddError(AppConstants.Code, AppConstants.Student_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var client = new RestClient("https://dev.vdocipher.com")
                {
                    // using user secrets to keep api key
                    Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator("gZKDngGYS8bcAb2Xpl8i97qTs75K9pjVaRq6na8LdlLRASOF7OAVqddrB2cy9Ep0", "Apisecret")
                };
                var otpParameters = new
                {
                    // 5 minutes until OTP is used
                    // only used when setting up offline, not again
                    ttl = 300,
                    licenseRules = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        rentalDuration = 15 * 24 * 3600, // 15 days
                        canPersist = true,
                    }),
                };
                IRestResponse resultData = null;
                var lst = new List<OTPVideoDto>();
                // var groups=_studentService.
                var groups = await _studentService.GetGroupByCourse(courseId);
                var contents = await _studentService.GetContentByGroup(groups);
                foreach (var item in contents)
                {
                    var url = $"/api/videos/{item.Path}/otp";
                    var request = new RestRequest(url).AddJsonBody(otpParameters);
                    var cancellationToken = CancellationToken.None;
                    resultData = await client.ExecutePostAsync(request, cancellationToken);
                    JsonSerializerSettings serSettings = new JsonSerializerSettings();
                    serSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    var obj = JsonConvert.DeserializeObject<OTPVideoDto>(resultData.Content, serSettings);
                    obj.contentId = item.Id;
                    obj.videoId = item.Path;
                    lst.Add(obj);
                }
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    return response.getResponseMessage(HttpStatusCode.OK);
                }
                response.AddModel(AppConstants.OTP, lst);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Returned Successfuly");

                return response.getResponseMessage(HttpStatusCode.OK);

            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");

                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }
         
 
        }
        
        public class OTPVideoDto
        {
            public string otp { get; set; }
            public string playbackInfo { get; set; }
            public long? contentId { get; set; }
            public string videoId { get; set; }
        }



        public async Task<HttpResponseMessage> GetAll()
        {
            MetaData metaData = new MetaData();
           // doPayment();
            try
            {
                var resultData = await _metaDataService.GetMetaData();
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    return response.getResponseMessage(HttpStatusCode.OK);
                }
                response.AddModel(AppConstants.Metas, resultData);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Returned Successfuly");

                return response.getResponseMessage(HttpStatusCode.OK);

            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");

                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }
        }


        public void doPayment()
        {
            //####### Direct Payment ######

            string token = @"i6PbcDUyfgtlAa0dWwW4N5FEsDpKaPfFeLox4He0Y-ylMQ3BT629H1EkuoOi55cITROETH_BmS8_u2-qX4hIKPc4l4ImL4EJV2FQIg8psAf-u-JsA4MJFk1VJc5u5K-hmzDTnUi9QEgcZ3sa8XVc9dYx3M_ZxoYYZUTgeZPte_5WcvmeNyqWUyU69d5v5d7EMBDnSZErlPQUhO-k0fvRKuaxTYh7IfFr3gAvvi5k3p3bdUl9XY6_kTrkAGyUuZuF390kGGcExhCs1yKhNpkNy_aspWRatTdN6FSU7ewqyR_1CwJRVanVHYn-hyzEhtH887YKQW75cFpxFnkBDTtm_z95s9AsuQJ1yIb1ZfetnKk8K-GIRwygftlFSNVMAmPQxhiVajQi4sgVQFjVMIb6edxgiP0UQjU9_L-Ra3Glv2WhRl9xUG0v8twXNovaD-d6LHKs8Nt0GhjjVRHzg9tgZawIguw5h5WqAlVayshI5GZibg0qfUB0fhhZyoqC757ugyevkMRTNIt30hkuJ79MmQPqDf7ZKa_cUA9zsos_HNia7125cIb19QhRqyGejOcL6ZYmf3dMoJPQUlxVGD3h70xtz4BDoEdOxy0-CU1KGCX5lzCyUpGK4tkJEP_ovLuL6wy71JnNfZ_2seXT3Acwg6UV42L8h985gxd98AtdEZILVHv-"; //token value to be placed here
            string baseURL = "https://apitest.myfatoorah.com";
            var url = baseURL + "/v2/ExecutePayment";
            var client = new HttpClient(); client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token); client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            string json = "{\"PaymentMethodId\":\"2\",\"CustomerName\": \"Ahmed\",\"DisplayCurrencyIso\": \"KWD\", \"MobileCountryCode\":\"+965\",\"CustomerMobile\": \"92249038\",\"CustomerEmail\": \"aramadan@myfatoorah.com\",\"InvoiceValue\": 100,\"CallBackUrl\": \"https://google.com\",\"ErrorUrl\": \"https://google.com\",\"Language\": \"en\",\"CustomerReference\" :\"ref1\",\"CustomerCivilId\":12345678,\"UserDefinedField\": \"Custom field\",\"ExpireDate\": \"\",\"CustomerAddress\" :{\"Block\":\"\",\"Street\":\"\",\"HouseBuildingNo\":\"\",\"Address\":\"\",\"AddressInstructions\":\"\"},\"InvoiceItems\": [{\"ItemName\": \"Product 01\",\"Quantity\": 1,\"UnitPrice\": 100}]}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var messge = client.PostAsync(url, content).Result;
            string result = messge.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //context.Response.WriteAsync("\n" + result + "\n");
            JObject obj = JObject.Parse(result);
            string paymentURL = (string)obj["Data"]["PaymentURL"];
            url = paymentURL;
            client = new HttpClient(); client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            json = "{\"paymentType\": \"card\",\"card\": {\"Number\":\"5123450000000008\",\"expiryMonth\":\"05\",\"expiryYear\":\"21\",\"securityCode\":\"100\"},\"saveToken\": false}";
            content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json"); messge = client.PostAsync(url, content).Result;
            result = messge.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            // context.Response.WriteAsync("\n" + result + "\n");
        }

    }

}
