using DataAccess.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TollabAPI.Utils;
namespace TollabAPI.Controllers
{
    [Authorize]
    public class OffersController : ApiController
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private CustomeResponseMessage response = new CustomeResponseMessage();
        OfferService _offerService ;
        StudentService _studentService;

        public OffersController()
        {
            _offerService = new  OfferService();
            _studentService = new StudentService();
        }


        [HttpGet]
        [Route("api/GetOffers")]
        public async Task<HttpResponseMessage> GetOffers()
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                //string IdentityUserId = "556b1ba5-5acb-49d8-b780-93f37ce85dc9";
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

                var resultData = await _offerService.GetOffers(StudentData.CountryId);
                if (resultData.Count() == 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_returned);
                    return response.getResponseMessage(HttpStatusCode.OK);
                }
                response.AddModel(AppConstants.User, resultData);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Returned Successfuly");
                return response.getResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");
                response.AddError(AppConstants.Code, AppConstants.Result_error_try_later);
                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }


        }


    }
}
