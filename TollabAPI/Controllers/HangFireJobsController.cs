using DataAccess.PaymentModels;
using DataAccess.Services;
using DataAccess.Services.Utils;
using Hangfire;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace TollabAPI.Controllers
{

    [AllowAnonymous]
    public class HangFireJobsController : ApiController
    {
        private readonly StudentService _studentService;
        private readonly MyFatoorahPaymentLogService _myfatoorahService;
        private readonly MyFatoorahPaymentService _paymentService;
        private readonly TransactionService _transactionService;
        public HangFireJobsController()
        {
            _studentService = new StudentService();
            _myfatoorahService = new MyFatoorahPaymentLogService();
            _paymentService = new MyFatoorahPaymentService();
            _transactionService = new TransactionService();
        }


        public void SendSmsToStudentForRenewSubscription()
        {

           // RecurringJob.AddOrUpdate("RenewSubscription-job", () => _studentService.SendSmsToStudentForRenewSubscription(), Cron.Daily);


        }
        public void FixInvoices()
        {
          //  RecurringJob.AddOrUpdate("FixMyFatoorahInvoices", () => FixMyFatoorahInvoicesAsync(), Cron.Minutely);

        }


        public void FixMyFatoorahInvoicesAsync()
        {
            //try
            //{
            //    var failedInvoices = _myfatoorahService.GetAllUnresponded().GetAwaiter().GetResult();
            //    if (failedInvoices.Any())
            //    {
            //        foreach (var inv in failedInvoices.ToList())
            //        {
            //            var message = _paymentService.GetPaymentStatus(new GetPaymentStatusRequest
            //            {
            //                Key = inv.InvoiceId.ToString(),
            //                KeyType = "invoiceId"
            //            }).GetAwaiter().GetResult();
            //            var response = JsonConvert.DeserializeObject<GenericResponse<GetPaymentStatusResponse>>(message);

            //            if (response.IsSuccess)
            //            {
            //                if (response.Data.InvoiceStatus.ToLower() =="paid")
            //                {
            //                    var computedFiled = response.Data.UserDefinedField.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //                    var userId = response.Data.CustomerName ?? "";
            //                    switch (computedFiled[0])
            //                    {
            //                        case string transaction when int.Parse(transaction) == (int)TransactionType.Course:

            //                            var courseId = !string.IsNullOrEmpty(response.Data.CustomerReference) ? long.Parse(response.Data.CustomerReference) : 0;

            //                            if (!string.IsNullOrEmpty(message))
            //                            {
            //                                if (!string.IsNullOrEmpty(userId))
            //                                {
            //                                    var StudentData = _studentService.GetStudentById(long.Parse(userId)).GetAwaiter().GetResult();
            //                                    var resultData = _transactionService.BuyCourseFromWeb(courseId, StudentData, "", StudentData.CountryId).GetAwaiter().GetResult();
            //                                    if (resultData == 7 || resultData == 1)
            //                                    {
            //                                        if (resultData == 7)
            //                                        {
            //                                            var bankResponse = JsonConvert.DeserializeObject<BankResponse>((JObject.Parse(message)).ToString());

            //                                            if (bankResponse != null)
            //                                            {

            //                                                //var transactionResult =  _transactionService.AddStudentTransaction(2, courseId, bankResponse).GetAwaiter().GetResult();
            //                                                //if (transactionResult == 0)
            //                                                //{
            //                                                //     _myfatoorahService.UpdateInvoice(inv.InvoiceId, true, false, "Invoice Transaction Failed").GetAwaiter().GetResult();
            //                                                //    break;
            //                                                //}


            //                                                _transactionService.LogResponseToDataBase(bankResponse).GetAwaiter().GetResult();
            //                                            }
            //                                        }
            //                                        _myfatoorahService.UpdateInvoice(inv.InvoiceId, true, true, response.Data.InvoiceStatus.ToLower()).GetAwaiter().GetResult();
            //                                    }
            //                                }
            //                            }

            //                            break;

            //                        case string transaction when int.Parse(transaction) == (int)TransactionType.Live:
            //                            var liveId = !string.IsNullOrEmpty(response.Data.CustomerReference) ? long.Parse(response.Data.CustomerReference) : 0;
            //                            if (!string.IsNullOrEmpty(message))
            //                            {
            //                                var bankResponse = JsonConvert.DeserializeObject<BankResponse>((JObject.Parse(message)).ToString());
            //                                if (bankResponse != null)
            //                                {
            //                                    var transactionResult = _transactionService.AddStudentTransaction(3, liveId, bankResponse).GetAwaiter().GetResult();
            //                                    if (transactionResult == 0)
            //                                    {
            //                                        _myfatoorahService.UpdateInvoice(inv.InvoiceId, true, false, "Invoice Transaction Failed").GetAwaiter().GetResult();
            //                                        break;
            //                                    }
            //                                    _transactionService.LogResponseToDataBase(bankResponse).GetAwaiter().GetResult();
            //                                }

            //                                if (!string.IsNullOrEmpty(userId))
            //                                {
            //                                    var StudentData = _studentService.GetStudentByIdentityIdAsync(userId).GetAwaiter().GetResult();
            //                                    var resultData = _transactionService.BuyLiveFromWeb(liveId, StudentData, "", StudentData.CountryId).GetAwaiter().GetResult();
            //                                    if (resultData == 7)
            //                                    {
            //                                        _myfatoorahService.UpdateInvoice(inv.InvoiceId, true, true, response.Data.InvoiceStatus.ToLower()).GetAwaiter().GetResult();
            //                                    }
            //                                }
            //                            }
            //                            break;

            //                    }

            //                }


            //            }
            //            else
            //            {
            //                if (response.Data != null)
            //                {
            //                    _myfatoorahService.UpdateInvoice(inv.InvoiceId, true, false, response.Data.InvoiceStatus.ToLower()).GetAwaiter().GetResult();
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //    Console.WriteLine(ex.Message);
            //}
        }
    }
}
