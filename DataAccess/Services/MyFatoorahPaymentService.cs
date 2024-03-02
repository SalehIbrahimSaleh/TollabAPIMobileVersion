using DataAccess.Services.Utils;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class MyFatoorahPaymentService
    {
        private readonly MyFatoorahClient _client;
        public MyFatoorahPaymentService()
        {
            _client = new MyFatoorahClient();
        }



        public async Task<string> GetPaymentStatus(GetPaymentStatusRequest getPaymentStatusRequest)
        {

            var GetPaymentStatusRequestJSON = JsonConvert.SerializeObject(getPaymentStatusRequest);
            return await _client.PerformRequest(GetPaymentStatusRequestJSON, endPoint: "GetPaymentStatus").ConfigureAwait(false);

        }
    }
}
