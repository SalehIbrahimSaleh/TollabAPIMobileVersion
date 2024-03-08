using DataAccess.DTOs;
using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TollabAPI.Models;
using TollabAPI.Services;
using TollabAPI.Utils;

namespace TollabAPI.Controllers
{
    public class TeachersController : ApiController
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
        TeacherService _teacherService;
        ApplicationDbContext dbContext = new ApplicationDbContext();
        private readonly IZoomManagerService _zoomManagerService = new ZoomManagerService();
        private const string VimeoToken = "3251e9876001f9b94a96b446bacdb6c2";
        public TeachersController()
        {
            _teacherService = new TeacherService();
        }

        #region Login Module
        [AllowAnonymous]
        [HttpGet]
        [Route("api/TeacherLogin")]
        public async Task<HttpResponseMessage> TeacherLogin(string Email, string Password)
        {

            if (string.IsNullOrEmpty(Email))
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "invalide PhoneNumber");
                response.AddError(AppConstants.Code, AppConstants.Invalide_PhoneNumber);
                return response.getResponseMessage(HttpStatusCode.BadRequest);
            }
            if (string.IsNullOrEmpty(Password) || Password.Length < 6)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "invalide Password");
                response.AddError(AppConstants.Code, AppConstants.Invalide_Password);
                return response.getResponseMessage(HttpStatusCode.BadRequest);
            }

            var AppUser = dbContext.Database.SqlQuery<ApplicationUser>("select * from AspNetUsers where UserName='" + Email + "'").FirstOrDefault();
            if (AppUser == null)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "Teacher Not Register Yet");
                response.AddError(AppConstants.Code, AppConstants.Result_Teacher_not_Register_Yet);
                return response.getResponseMessage(HttpStatusCode.BadRequest);
            }
            try
            {
                Tuple<bool, JArray> result = GetToken(Email, Password);
                if (result.Item1 == true)
                {
                    var roles = await UserManager.GetRolesAsync(AppUser.Id);
                    TeacherAssistant teacherAssistantData = null;
                    if (roles.Contains("TeacherAssistant"))
                    {
                        teacherAssistantData = await _teacherService.GetTeacherAssistantByIdentityId(AppUser.Id);
                        if (teacherAssistantData == null)
                        {
                            response.clearBody();
                            response.AddError(AppConstants.Message, "Teacher Assistant not found");
                            response.AddError(AppConstants.Code, AppConstants.Result_Teacher_Assistant_not_found);
                            return response.getResponseMessage(HttpStatusCode.NotFound);
                        }
                        if (teacherAssistantData.Enabled == false)
                        {
                            response.clearBody();
                            response.AddError(AppConstants.Message, "Your Account Is Disabled");
                            response.AddError(AppConstants.Code, AppConstants.Your_Account_Is_Disabled);
                            return response.getResponseMessage(HttpStatusCode.NotFound);
                        }
                      
                        teacherAssistantData.IsTeacher = false;
                        var TeacherData = await _teacherService.GetTeacherById(teacherAssistantData.TeacherId.Value);
                        TeacherData.IsTeacher = false;
                        TeacherData.Token = result.Item2;
                        TeacherData.TeacherAssistant = teacherAssistantData;
                        response.AddModel(AppConstants.User, TeacherData);
                        response.AddMeta(AppConstants.Result, AppConstants.Success);
                        response.AddMeta(AppConstants.Message, "Successfuly Login");
                        return response.getResponseMessage(HttpStatusCode.OK);

                    }
                    else
                    {
                        var resultData = await _teacherService.GetTeacherByIdentityId(AppUser.Id);
                        if (resultData == null)
                        {
                            response.clearBody();
                            response.AddError(AppConstants.Message, "Teacher not found");
                            response.AddError(AppConstants.Code, AppConstants.Result_Teacher_not_found);
                            return response.getResponseMessage(HttpStatusCode.NotFound);
                        }
                        if (resultData.Enabled == false)
                        {
                            response.clearBody();
                            response.AddError(AppConstants.Message, "Your Account Is Disabled");
                            response.AddError(AppConstants.Code, AppConstants.Your_Account_Is_Disabled);
                            return response.getResponseMessage(HttpStatusCode.NotFound);
                        }
                        resultData.Token = result.Item2;
                        resultData.IsTeacher = true;

                        response.AddModel(AppConstants.User, resultData);
                        response.AddMeta(AppConstants.Result, AppConstants.Success);
                        response.AddMeta(AppConstants.Message, "Successfuly Login");
                        return response.getResponseMessage(HttpStatusCode.OK);

                    }
                }
                else
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Incorrect Password");
                    response.AddError(AppConstants.Code, AppConstants.Incorrect_Password);

                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }


            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");
                response.AddError(AppConstants.Code, AppConstants.Result_error_try_later);
                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }


        }

        [HttpGet]
        [Route("api/GetTeacherProfile")]
        public async Task<HttpResponseMessage> GetTeacherProfile(long CountryId=0)
        {
            try
            {
                bool IsTeacher = true;
                //"2ec6f42f-f2f4-4f3f-93e8-ffaa2526c03e";
                string IdentityUserId =  User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherAssistantData = Request.GetOwinContext().Authentication.User.IsInRole("TeacherAssistant");
                TeacherAssistant assistantData = null;
                if (TeacherAssistantData)
                {
                     assistantData = await _teacherService.GetTeacherAssistantByIdentityId(IdentityUserId);
                    if (assistantData==null)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "User not found");
                        response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }
                    IdentityUserId =(await _teacherService.GetTeacherById(assistantData.TeacherId.Value)).IdentityId;
                }
                var resultData = await _teacherService.GetTeacherByIdentityId(IdentityUserId,CountryId);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher Not Found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (resultData.Enabled == false)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Your Account Is Disabled");
                    response.AddError(AppConstants.Code, AppConstants.Your_Account_Is_Disabled);
                    return response.getResponseMessage(HttpStatusCode.NotFound);
                }
                /*if (CountryId == 0)
                {
                    CountryId = resultData.CountryId.Value;
                }
               */
                var profile = await _teacherService.GetTeacherByIdInCountry(resultData.Id, CountryId);
                if (TeacherAssistantData)
                {
                    profile.IsTeacher = false;
                    profile.TeacherAssistant = assistantData;
                }
                else
                {
                    profile.IsTeacher = true;

                }
                response.AddModel(AppConstants.User, profile);
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

        [HttpPost]
        [Route("api/EditBio")]
        public async Task<HttpResponseMessage> EditBio(Teacher teacher)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (string.IsNullOrEmpty(teacher.Bio))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Parametrs");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Parametrs);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var resultData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher Not Found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                resultData.Bio = teacher.Bio;
                var profile = await _teacherService.EditProfile(resultData);
                if (profile == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "OPeration Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.User, profile);
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


        [HttpPost]
        [Route("api/ChangeTeacherPhoto")]
        public async Task<HttpResponseMessage> ChangeTeacherPhoto(Teacher teacher)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (string.IsNullOrEmpty(teacher.Photo))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Parametrs");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Parametrs);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var resultData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher Not Found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var image = SetTeacherPhoto(teacher.Photo);
                resultData.Photo = image;
                var profile = await _teacherService.EditProfile(resultData);
                if (profile == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "OPeration Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.User, profile);
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


        [HttpGet]
        [Route("api/GetTeacherById")]
        public async Task<HttpResponseMessage> GetTeacherById(long TeacherId)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var resultData = await _teacherService.GetTeacherById(TeacherId);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_returned);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
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

        [HttpGet]
        [Route("api/GetCoursesBySubjectId")]
        public async Task<HttpResponseMessage> GetCoursesBySubjectId(long TeacherId, long SubjectId, int Page = 0)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var resultData = await _teacherService.GetCoursesBySubjectId(TeacherId, SubjectId, Page);
                if (resultData.Count() == 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
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

        [HttpPost]
        [Route("api/AddTeacherPushToken")]
        public async Task<HttpResponseMessage> AddTeacherPushToken(TeacherPushToken teacherPushToken)
        {

            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                teacherPushToken.TeacherId = TeacherData.Id;
                var resultData = await _teacherService.AddTeacherPushToken(teacherPushToken);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "operation not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.User, resultData);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Updated Successfuly");
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

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        [HttpGet]
        [Route("api/TeacherLogout")]
        public async Task<HttpResponseMessage> TeacherLogout()
        {

            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher_Not_Found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherAssistantData = Request.GetOwinContext().Authentication.User.IsInRole("TeacherAssistant");
                if (TeacherAssistantData)
                {
                    Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                    Request.GetOwinContext().Authentication.SignOut();

                    Request.GetOwinContext().Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.Current.GetOwinContext().Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);


                    response.AddModel(AppConstants.User, true);
                    response.AddMeta(AppConstants.Result, AppConstants.Success);
                    response.AddMeta(AppConstants.Message, "log out Successfuly");
                    return response.getResponseMessage(HttpStatusCode.OK);
                }

                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                Request.GetOwinContext().Authentication.SignOut();

                Request.GetOwinContext().Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);

                HttpContext.Current.GetOwinContext().Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);

                var resultData = await _teacherService.DeleteAllTokens(teacherData.Id);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "operation not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.User, resultData);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "log out Successfuly");
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


        static Tuple<bool, JArray> GetToken(string userName, string password)
        {
            var request = HttpContext.Current.Request;
            var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username",userName),
                    new KeyValuePair<string, string>("password", password)
                };
                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                var tokenServiceResponse = client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded).Result;
                var loginResponse = tokenServiceResponse.Content.ReadAsStringAsync().Result;
                var loginResponseMessageContent = new JArray();
                var responseObject = JObject.Parse(loginResponse);
                loginResponseMessageContent.Add(responseObject);

                var responseCode = tokenServiceResponse.StatusCode;
                if (responseCode == HttpStatusCode.OK)
                    return Tuple.Create(true, loginResponseMessageContent);
                else
                    return Tuple.Create(false, loginResponseMessageContent);
            }
        }

        #endregion

        #region   Course Module
        [HttpGet]
        [Route("api/GetCoursesByTrackIdForTeacher")]
        public async Task<HttpResponseMessage> GetCoursesByTrackId(long TrackId)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }


                var resultData = await _teacherService.GetCoursesByTrackId(TrackId);
                if (resultData == null)
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
        [HttpGet]
        [Route("api/GetAllCoursesByTrackIdForTeacher")]
        public async Task<HttpResponseMessage> GetAllCoursesByTrackIdForTeacher(long TrackId)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }


                var resultData = await _teacherService.GetAllCoursesByTrackId(TrackId);
                if (resultData == null)
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


        [HttpGet]
        [Route("api/GetTeacherHomeCourses")]
        public async Task<HttpResponseMessage> GetTeacherHomeCourses(int Page = 0, long CountryId =0)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherAssistantData = Request.GetOwinContext().Authentication.User.IsInRole("TeacherAssistant");
                if (TeacherAssistantData)
                {
                    var assistantData = await _teacherService.GetTeacherAssistantByIdentityId(IdentityUserId);
                    if (assistantData == null)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "User not found");
                        response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }
                    IdentityUserId = (await _teacherService.GetTeacherById(assistantData.TeacherId.Value)).IdentityId;
                     
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (CountryId==0)
                {
                    CountryId = TeacherData.CountryId.Value;
                }
                var resultData = await _teacherService.GetTeacherHomeCourses(TeacherData.Id, CountryId, Page);
                if (resultData.Count() == 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
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

        [HttpGet]
        [Route("api/GetTracks")]
        public async Task<HttpResponseMessage> GetTracks(long? SubjectId, int Page = 0)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherAssistantData = Request.GetOwinContext().Authentication.User.IsInRole("TeacherAssistant");
                if (TeacherAssistantData)
                {
                    var assistantData = await _teacherService.GetTeacherAssistantByIdentityId(IdentityUserId);
                    if (assistantData == null)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "User not found");
                        response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }
                    IdentityUserId = (await _teacherService.GetTeacherById(assistantData.TeacherId.Value)).IdentityId;

                }

                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var resultData = await _teacherService.GetTracks(TeacherData.Id, SubjectId, Page);
                if (resultData.Count() == 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
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

        [HttpGet]
        [Route("api/GetTeacherSubjects")]
        public async Task<HttpResponseMessage> GetTeacherSubjects(long CountryId,int Page = 0)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherAssistantData = Request.GetOwinContext().Authentication.User.IsInRole("TeacherAssistant");
                if (TeacherAssistantData)
                {
                    var assistantData = await _teacherService.GetTeacherAssistantByIdentityId(IdentityUserId);
                    if (assistantData == null)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "User not found");
                        response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }
                    IdentityUserId = (await _teacherService.GetTeacherById(assistantData.TeacherId.Value)).IdentityId;

                }

                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (CountryId == 0)
                {
                    CountryId = TeacherData.CountryId.Value;
                }
                var resultData = await _teacherService.GetTeacherSubject(TeacherData.Id, CountryId, Page);
                if (resultData.Count() == 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
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

        [HttpPost]
        [Route("api/AddTrack")]
        public async Task<HttpResponseMessage> AddTrack(Track track)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (!string.IsNullOrEmpty(track.Image))
                {
                    var img = SetTrackPhoto(track.Image);
                    track.Image = img;
                }
                if (!string.IsNullOrEmpty(track.ImageHomeCover))
                {
                    var imgHome = SetTrackPhoto(track.ImageHomeCover);
                    track.ImageHomeCover = imgHome;
                }
                
                track.TeacherId = TeacherData.Id;
                var resultData = await _teacherService.AddTrack(track);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Try with Other Name");
                    response.AddError(AppConstants.Code, AppConstants.Try_with_Other_Name);
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


        [HttpPost]
        [Route("api/AddCourseImage")]
        public async Task<HttpResponseMessage> AddCourseImage (Course  course)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (string.IsNullOrEmpty(course.Image))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Parametrs");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Parametrs);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (!string.IsNullOrEmpty(course.Image))
                {
                    var img = SetCoursePhoto(course.Image);
                    course.Image = img;
                }
                var resultData = await _teacherService.setCourseImage(course.Image,course.Id);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Try with Other Name");
                    response.AddError(AppConstants.Code, AppConstants.Try_with_Other_Name);
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

        [HttpPost]
        [Route("api/AddOrUpdateCourseTitle")]
        public async Task<HttpResponseMessage> AddOrUpdateCourseTitle(Course course )
        {
            try
            {
                int id = -1;
                if (course.Id==0)
                {
                    id = 0;
                }
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (course.Name.Length >150 || course.NameLT.Length > 150)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Maximum Letters Are 150");
                    response.AddError(AppConstants.Code, AppConstants.Maximum_Letters_Are_150);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var resultData = await _teacherService.AddOrUpdateCourseTitle(course);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
                    return response.getResponseMessage(HttpStatusCode.OK);
                }
                if (id == 0)
                {
                    var uri = await CreateAlbumAsync(resultData.CourseTrack);
                    resultData.AlbumUri = uri;
                    await _teacherService.UpdateCourseWithAlbumUri(resultData);

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




        public async Task<string> CreateAlbumAsync(string CourseTrack)
        {
            string AlbumUri = "";
            string uri = "https://api.vimeo.com/users/101981438/albums";
            var clientTocall = new HttpClient();
            clientTocall.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", VimeoToken/*"3e67029d6d7a2b9bf3583a5b7869a3cb"*/);
            clientTocall.DefaultRequestHeaders.Add("Accept", "application/vnd.vimeo.*+json;version=3.4");
            var VideoObject = new { name = CourseTrack };
            var response = await clientTocall.PostAsJsonAsync(uri, VideoObject);
            var responseString = await response.Content.ReadAsStringAsync();
            var tempResponse = JObject.Parse(responseString);
            responseString = tempResponse.ToString();
            var responseCode = response.StatusCode;
            if (responseCode == HttpStatusCode.Created)
            {
                AlbumUri = (string)tempResponse["uri"];
                return AlbumUri;
            }
            return AlbumUri;
        }


        [HttpPost]
        [Route("api/AddOrUpdateCoursePrice")]
        public async Task<HttpResponseMessage> AddOrUpdateCoursePrice(Course course)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var resultData = await _teacherService.AddOrUpdateCoursePrice(course);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
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

        [HttpPost]
        [Route("api/AddCourseFullDescription")]
        public async Task<HttpResponseMessage> AddCourseFullDescription(Course course)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var resultData = await _teacherService.AddCourseFullDescription(course);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
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

        [HttpPost]
        [Route("api/AddIntroVideoToCourse")]
        public async Task<HttpResponseMessage> AddIntroVideoToCourse()
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                //// video saving

                //string fileSaveLocation = HttpContext.Current.Server.MapPath("~/CourseVideos");
                //CustomMultipartFormDataStreamProviderForUplaodContents provider = new CustomMultipartFormDataStreamProviderForUplaodContents(fileSaveLocation);
                //List<string> files = new List<string>();
                //// Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                //await Request.Content.ReadAsMultipartAsync(provider);
                //string fileName = null;
                //foreach (MultipartFileData file in provider.FileData)
                //{
                //    fileName = Path.GetFileName(file.LocalFileName);

                //    files.Add(Path.GetFileName(file.LocalFileName));
                //}

                //// end video saving

                var courseId = Convert.ToInt64(HttpContext.Current.Request.Params["CourseId"]);
                var CourseData = await _teacherService.GetCoursesById(courseId, TeacherData.Id);
                if (CourseData.IntroVideoUri!=null)
                {
                   await DeleteIntroVideoFromViemoAsync(CourseData.IntroVideoUri);
                }
                CourseData.IntroVideoUri = HttpContext.Current.Request.Params["VideoUri"];
                CourseData.IntroVideo = HttpContext.Current.Request.Params["VideoLink"];
                var resultData = await _teacherService.UpdateCourseIntroVideo(CourseData);

                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
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

       

        [HttpPost]
        [Route("api/AddOrUpdateGroup")]
        public async Task<HttpResponseMessage> AddOrUpdateGroup(Group  group)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var resultData = await _teacherService.AddOrUpdateGroup(group);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
                    return response.getResponseMessage(HttpStatusCode.OK);
                }
                response.AddModel(AppConstants.User, resultData);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Returned Successfully");
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



        [HttpPost]
        [Route("api/AddContent")]
        public async Task<HttpResponseMessage> AddContent()
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }


                Content content = new Content();
                content.Name = HttpContext.Current.Request.Params["Name"];
                content.NameLT = HttpContext.Current.Request.Params["NameLT"];
                content.IsFree = Convert.ToBoolean(HttpContext.Current.Request.Params["IsFree"]);
                content.ContentTypeId = Convert.ToInt64(HttpContext.Current.Request.Params["ContentTypeId"]);
                if (content.ContentTypeId==1)
                {
                    content.Duration = Convert.ToDouble(HttpContext.Current.Request.Params["Duration"]);

                }
                content.GroupId = Convert.ToInt64(HttpContext.Current.Request.Params["GroupId"]);
                content.VideoUri = HttpContext.Current.Request.Params["VideoUri"];
                content.Path = HttpContext.Current.Request.Params["VideoLink"];
                if (content.ContentTypeId==2)
                {
                   // string fileSaveLocation = HttpContext.Current.Server.MapPath("~/CourseVideos");
                    string fileSaveLocation = HttpContext.Current.Server.MapPath("~");
                    if (fileSaveLocation.EndsWith("sws"))
                    {
                        fileSaveLocation = fileSaveLocation.Remove(fileSaveLocation.Length - 3);

                    }
                    fileSaveLocation = fileSaveLocation + "ws/CourseVideos";

                    CustomMultipartFormDataStreamProviderForUplaodContents provider = new CustomMultipartFormDataStreamProviderForUplaodContents(fileSaveLocation);
                    List<string> files = new List<string>();
                    // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                    await Request.Content.ReadAsMultipartAsync(provider);
                    string fileName = null;
                    foreach (MultipartFileData file in provider.FileData)
                    {
                        fileName = Path.GetFileName(file.LocalFileName);
                        files.Add(Path.GetFileName(file.LocalFileName));
                    }
                    content.Path = fileName;

                }
                if (content.OrderNumber==null)
                {
                    content.OrderNumber = 0;
                }
                var resultData = await _teacherService.AddContent(content);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
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


        [HttpGet]
        [Route("api/DeleteContent")]
        public async Task<HttpResponseMessage> DeleteContent(long ContentId)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
               await DeleteFromViemoAsync(ContentId);
                var resultData = await _teacherService.DeleteContent(ContentId);

                if (resultData == false)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
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

        private async Task DeleteFromViemoAsync(long contentId)
        {
            var content = await _teacherService.GetContent(contentId);
            if (content.ContentTypeId==1)
            {
                var uri = "https://api.vimeo.com" + content.VideoUri;

                var clientTocall = new HttpClient();
                clientTocall.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", VimeoToken);
                clientTocall.DefaultRequestHeaders.Add("Accept", "application/vnd.vimeo.*+json;version=3.4");
                var response = await clientTocall.DeleteAsync(uri);

                var responseString = await response.Content.ReadAsStringAsync();
               
            }

        }

        private async Task DeleteIntroVideoFromViemoAsync(string introVideoUri)
        {
            var uri = "https://api.vimeo.com" + introVideoUri;

            var clientTocall = new HttpClient();
            clientTocall.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", VimeoToken);
            clientTocall.DefaultRequestHeaders.Add("Accept", "application/vnd.vimeo.*+json;version=3.4");
            var response = await clientTocall.DeleteAsync(uri);
            var responseString = await response.Content.ReadAsStringAsync();
           
        }

        [HttpGet]
        [Route("api/GetCourseById")]
        public async Task<HttpResponseMessage> GetCourseById(long CourseId)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherAssistantData = Request.GetOwinContext().Authentication.User.IsInRole("TeacherAssistant");
                if (TeacherAssistantData)
                {
                    var assistantData = await _teacherService.GetTeacherAssistantByIdentityId(IdentityUserId);
                    if (assistantData == null)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "User not found");
                        response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }
                    IdentityUserId = (await _teacherService.GetTeacherById(assistantData.TeacherId.Value)).IdentityId;

                }

                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var resultData = await _teacherService.GetCoursesById(CourseId, TeacherData.Id);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
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

        [HttpGet]
        [Route("api/GetCourseWithOneContentForTeacher")]
        public async Task<HttpResponseMessage> GetCourseWithOneContentForTeacher(long CourseId, long ContentId,long VideoQuestionId)
        {
            try
            {

                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherAssistantData = Request.GetOwinContext().Authentication.User.IsInRole("TeacherAssistant");
                if (TeacherAssistantData)
                {
                    var assistantData = await _teacherService.GetTeacherAssistantByIdentityId(IdentityUserId);
                    if (assistantData == null)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "User not found");
                        response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }
                    IdentityUserId = (await _teacherService.GetTeacherById(assistantData.TeacherId.Value)).IdentityId;

                }

                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var resultData = await _teacherService.GetCourseWithOneContentForTeacher(CourseId, ContentId, TeacherData.Id, VideoQuestionId);
                if (resultData == null)
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


        [HttpGet]
        [Route("api/GetGroupsWithContentsByCourseId")]
        public async Task<HttpResponseMessage> GetGroupsWithContentsByCourseId(long CourseId, int Page = 0, long ContentId=0)
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
                

                var resultData = await _teacherService.GetGroupsWithContentsByCourseId(CourseId, Page, ContentId);
                if (resultData == null)
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

        [HttpGet]
        [Route("api/SubmitToReview")]
        public async Task<HttpResponseMessage> SubmitToReview(long CourseId)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var resultData = await _teacherService.SubmitToReview(CourseId);
                if (resultData == false)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
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





        public string SetTrackPhoto(string image)
        {
            var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            if (basePath.EndsWith("sws"))
            {
                basePath = basePath.Remove(basePath.Length - 3);
            }
            string imagePath = "/ws/Images/TeacherSubjectImages/";
            string fileName = "UI" + DateTime.Now.ToString("IMG" + "dd_MM_yyyy_HH_mm_ss") + ".png";
            byte[] fileBytes = Convert.FromBase64String(image);
            MemoryStream ms = new MemoryStream(fileBytes);
            string fullPath = basePath + imagePath + fileName;
            System.IO.Directory.CreateDirectory(basePath + imagePath);
            FileStream fs = new FileStream(fullPath, FileMode.Create);
            ms.WriteTo(fs);
            ms.Close();
            fs.Close();
            fs.Dispose();
            return fileName;

        }

        public string SetCoursePhoto(string image)
        {
            var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            if (basePath.EndsWith("sws"))
            {
                basePath = basePath.Remove(basePath.Length - 3);
            }
            string imagePath = "/ws/Images/CourseImages/";
            string fileName = "UI" + DateTime.Now.ToString("IMG" + "dd_MM_yyyy_HH_mm_ss") + ".png";
            byte[] fileBytes = Convert.FromBase64String(image);
            MemoryStream ms = new MemoryStream(fileBytes);
            string fullPath = basePath + imagePath + fileName;
            System.IO.Directory.CreateDirectory(basePath + imagePath);
            FileStream fs = new FileStream(fullPath, FileMode.Create);
            ms.WriteTo(fs);
            ms.Close();
            fs.Close();
            fs.Dispose();
            return fileName;

        }
        public string SetLivePhoto(string image)
        {
            var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            if (basePath.EndsWith("sws"))
            {
                basePath = basePath.Remove(basePath.Length - 3);
            }
            string imagePath = "/ws/Images/LiveImages/";
            string fileName = "UI" + DateTime.Now.ToString("IMG" + "dd_MM_yyyy_HH_mm_ss") + ".png";
            byte[] fileBytes = Convert.FromBase64String(image);
            MemoryStream ms = new MemoryStream(fileBytes);
            string fullPath = basePath + imagePath + fileName;
            System.IO.Directory.CreateDirectory(basePath + imagePath);
            FileStream fs = new FileStream(fullPath, FileMode.Create);
            ms.WriteTo(fs);
            ms.Close();
            fs.Close();
            fs.Dispose();
            return fileName;

        }
        public string SetTeacherPhoto(string image)
        {
            var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            if (basePath.EndsWith("sws"))
            {
                basePath = basePath.Remove(basePath.Length - 3);
            }
            string imagePath = "/ws/Images/TeacherImages/";
            string fileName = "UI" + DateTime.Now.ToString("IMG" + "dd_MM_yyyy_HH_mm_ss") + ".png";
            byte[] fileBytes = Convert.FromBase64String(image);
            MemoryStream ms = new MemoryStream(fileBytes);
            string fullPath = basePath + imagePath + fileName;
            System.IO.Directory.CreateDirectory(basePath + imagePath);
            FileStream fs = new FileStream(fullPath, FileMode.Create);
            ms.WriteTo(fs);
            ms.Close();
            fs.Close();
            fs.Dispose();
            return fileName;

        }



        //[HttpGet]
        //[Route("api/SearchInSubjects")]
        //public async Task<HttpResponseMessage> AddTeacherSubject(TeacherSubject teacherSubject)
        //{
        //    try
        //    {
        //        string IdentityUserId = User.Identity.GetUserId();
        //        if (string.IsNullOrEmpty(IdentityUserId))
        //        {
        //            response.clearBody();
        //            response.AddError(AppConstants.Message, "Teacher not found");
        //            response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
        //            return response.getResponseMessage(HttpStatusCode.BadRequest);
        //        }
        //        var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
        //        if (TeacherData == null)
        //        {
        //            response.clearBody();
        //            response.AddError(AppConstants.Message, "Teacher not found");
        //            response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
        //            return response.getResponseMessage(HttpStatusCode.BadRequest);
        //        }
        //        var resultData = await _teacherService.SearchInSubjects(Word, Page);
        //        if (resultData.Count() == 0)
        //        {
        //            response.clearBody();
        //            response.AddError(AppConstants.Message, "No Data");
        //            response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
        //            return response.getResponseMessage(HttpStatusCode.OK);
        //        }
        //        response.AddModel(AppConstants.User, resultData);
        //        response.AddMeta(AppConstants.Result, AppConstants.Success);
        //        response.AddMeta(AppConstants.Message, "Returned Successfuly");
        //        return response.getResponseMessage(HttpStatusCode.OK);

        //    }
        //    catch (Exception e)
        //    {
        //        response.clearBody();
        //        response.AddError(AppConstants.Message, "An Error Occurred");
        //        response.AddError(AppConstants.Code, AppConstants.Result_error_try_later);
        //        return response.getResponseMessage(HttpStatusCode.InternalServerError);
        //    }


        //}

        #endregion


          #region Live Module

        [HttpPost]
        [Route("api/AddLive")]
        public async Task<HttpResponseMessage> AddLive(CreatingLiveDTO creatingLiveDTO)
        {
            try
            { 
                //"2ec6f42f-f2f4-4f3f-93e8-ffaa2526c03e";
                string IdentityUserId =  User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                switch (creatingLiveDTO.LiveLinkType)
                {
                    case LiveLinkType.TRACK:
                        creatingLiveDTO.CourseId = null;
                        break;
                    case LiveLinkType.COURSE:
                        creatingLiveDTO.TrackId = null;
                        break;
                    case LiveLinkType.OTHER:
                        creatingLiveDTO.CourseId = null;
                        creatingLiveDTO.TrackId = null;
                        break;
                }
                var (statusCode, hostURL, joinURL, meetingId, password) = _zoomManagerService.CreateSchedualMeeting(creatingLiveDTO.LiveName, creatingLiveDTO.Duration, creatingLiveDTO.LiveDate, creatingLiveDTO.CountryId);
                if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created)
                {
                    if (!string.IsNullOrEmpty(creatingLiveDTO.Image))
                    {
                        var img = SetLivePhoto(creatingLiveDTO.Image);
                        creatingLiveDTO.Image = img;
                    }
                    var DataReturned = await _teacherService.AddLive(creatingLiveDTO, TeacherData.Id, creatingLiveDTO.CountryId, hostURL, joinURL, meetingId, password);
                    response.AddModel(AppConstants.User, DataReturned);
                    response.AddMeta(AppConstants.Result, AppConstants.Success);
                    response.AddMeta(AppConstants.Message, "Returned Successfuly");
                    return response.getResponseMessage(HttpStatusCode.OK);
                    //if (DataReturned.HasData)
                    //{
                    //    await _imageUploadService.UploadImage(item.Id, nameof(Live), nameof(Live.Image), (int)ImageFolders.LiveImages, ImageFile);
                    //}
                }
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");
                response.AddError(AppConstants.Code, AppConstants.Result_error_try_later);
                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");
                response.AddError(AppConstants.Code, AppConstants.Result_error_try_later);
                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }


        }


        [HttpPost]
        [Route("api/UpdateLive")]
        public async Task<HttpResponseMessage> UpdateLive(UpdatingLiveDTO updatingLiveDTO)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                switch (updatingLiveDTO.LiveLinkType)
                {
                    case LiveLinkType.TRACK:
                        updatingLiveDTO.CourseId = null;
                        break;
                    case LiveLinkType.COURSE:
                        updatingLiveDTO.TrackId = null;
                        break;
                    case LiveLinkType.OTHER:
                        updatingLiveDTO.CourseId = null;
                        updatingLiveDTO.TrackId = null;
                        break;
                }
                var dbLive = await _teacherService.GetLive(updatingLiveDTO.Id);
                var statusCode = _zoomManagerService.UpdateSchedualMeeting(updatingLiveDTO.LiveName, updatingLiveDTO.Duration, updatingLiveDTO.LiveDate, updatingLiveDTO.CountryId, dbLive.ZoomMeetingId.Value);

                if (statusCode == HttpStatusCode.NoContent)
                {
                    if (!string.IsNullOrEmpty(updatingLiveDTO.Image))
                    {
                        var img = SetLivePhoto(updatingLiveDTO.Image);
                        updatingLiveDTO.Image = img;
                    }
                    var DataReturned = await _teacherService.UpdateLive(dbLive, updatingLiveDTO);
                    response.AddModel(AppConstants.User, DataReturned);
                    response.AddMeta(AppConstants.Result, AppConstants.Success);
                    response.AddMeta(AppConstants.Message, "Returned Successfuly");
                    return response.getResponseMessage(HttpStatusCode.OK);
                    //if (DataReturned.HasData)
                    //{
                    //    await _imageUploadService.UploadImage(item.Id, nameof(Live), nameof(Live.Image), (int)ImageFolders.LiveImages, ImageFile);

                    //}
                }
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");
                response.AddError(AppConstants.Code, AppConstants.Result_error_try_later);
                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");
                response.AddError(AppConstants.Code, AppConstants.Result_error_try_later);
                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/GetLiveDetailsForTeacher")]
        public async Task<HttpResponseMessage> GetLiveDetails(long liveId)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var liveDetails = await _teacherService.GetLiveDetails(liveId, TeacherData.Id);
                response.AddModel(AppConstants.User, liveDetails);
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


        [HttpGet]
        [Route("api/GetTeacherLives")]
        public async Task<HttpResponseMessage> GetTeacherLives(int page=0)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var liveDetails = await _teacherService.GetLives(TeacherData.CountryId.Value, TeacherData.Id, page);
                response.AddModel(AppConstants.User, liveDetails);
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


        [HttpPost]
        [Route("api/DeleteLive")]
        public async Task<HttpResponseMessage> DeleteLive(long liveId)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                await _teacherService.DeleteLive(liveId);
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




        [HttpPost]
        [Route("api/AddLiveAttachment")]
        public async Task<HttpResponseMessage> AddLiveAttachment()
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }


                LiveAttachment liveAttachment = new LiveAttachment();
                liveAttachment.Name = HttpContext.Current.Request.Params["Name"];
                liveAttachment.LiveId = Convert.ToInt64(HttpContext.Current.Request.Params["LiveId"]);
                liveAttachment.OrderNumber = HttpContext.Current.Request.Params["OrderNumber"] == null ? 0 : Convert.ToInt32(HttpContext.Current.Request.Params["OrderNumber"]);
                string fileSaveLocation = HttpContext.Current.Server.MapPath("~");
                if (fileSaveLocation.EndsWith("sws"))
                {
                    fileSaveLocation = fileSaveLocation.Remove(fileSaveLocation.Length - 3);

                }
                fileSaveLocation = fileSaveLocation + "ws/CourseVideos";

                CustomMultipartFormDataStreamProviderForUplaodContents provider = new CustomMultipartFormDataStreamProviderForUplaodContents(fileSaveLocation);
                List<string> files = new List<string>();
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);
                string fileName = null;
                foreach (MultipartFileData file in provider.FileData)
                {
                    fileName = Path.GetFileName(file.LocalFileName);
                    files.Add(Path.GetFileName(file.LocalFileName));
                }
                liveAttachment.Path = fileName;

                var resultData = await _teacherService.AddLiveAttachment(liveAttachment);
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

        [HttpPost]
        [Route("api/UpdateLiveAttachmentName")]
        public async Task<HttpResponseMessage> UpdateLiveAttachmentName(UpdatingLiveAttachmentDTO updatingLiveAttachment)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var resultData = await _teacherService.UpdateLiveAttachment(updatingLiveAttachment);
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


        [HttpPost]
        [Route("api/DeleteLiveAttachment")]
        public async Task<HttpResponseMessage> DeleteLiveAttachment(long liveAttachmentId)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                await _teacherService.DeleteLiveAttachment(liveAttachmentId);
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
        #endregion


        [HttpGet]
        [Route("api/GetTeacherTransactions")]
        public async Task<HttpResponseMessage> GetTeacherTransactions(int Page = 0,long CountryId=0)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (CountryId == 0)
                {
                    CountryId = TeacherData.CountryId.Value;
                }
                var resultData = await _teacherService.GetTeacherTransactions(TeacherData.Id, Page, CountryId);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
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
        [HttpGet]
        [Route("api/GetAllTeacherNotification")]
        public async Task<HttpResponseMessage> GetAllTeacherNotification(int Page = 0)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherAssistantData = Request.GetOwinContext().Authentication.User.IsInRole("TeacherAssistant");
                if (TeacherAssistantData)
                {
                    var assistantData = await _teacherService.GetTeacherAssistantByIdentityId(IdentityUserId);
                    if (assistantData == null)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "User not found");
                        response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }
                    IdentityUserId = (await _teacherService.GetTeacherById(assistantData.TeacherId.Value)).IdentityId;

                }

                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var resultData = await _teacherService.GetAllTeacherNotification(TeacherData.Id, Page);
                if (resultData.Count() == 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_Found);
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

        [HttpGet]
        [Route("api/SeenTeacherNotification")]
        public async Task<HttpResponseMessage> SeenTeacherNotification(long NotificationId)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherAssistantData = Request.GetOwinContext().Authentication.User.IsInRole("TeacherAssistant");
                if (TeacherAssistantData)
                {
                    var assistantData = await _teacherService.GetTeacherAssistantByIdentityId(IdentityUserId);
                    if (assistantData == null)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "User not found");
                        response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }
                    IdentityUserId = (await _teacherService.GetTeacherById(assistantData.TeacherId.Value)).IdentityId;

                }

                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var resultData = await _teacherService.SeenTeacherNotification(TeacherData.Id, NotificationId);
                if (resultData == false)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation_Not_Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
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


        [HttpGet]
        [Route("api/GetTeacherNotificationNotSeenCount")]
        public async Task<HttpResponseMessage> GetTeacherNotificationNotSeenCount()
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var TeacherAssistantData = Request.GetOwinContext().Authentication.User.IsInRole("TeacherAssistant");
                if (TeacherAssistantData)
                {
                    var assistantData = await _teacherService.GetTeacherAssistantByIdentityId(IdentityUserId);
                    if (assistantData == null)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "User not found");
                        response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }
                    IdentityUserId = (await _teacherService.GetTeacherById(assistantData.TeacherId.Value)).IdentityId;

                }

                var TeacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (TeacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Teacher not found");
                    response.AddError(AppConstants.Code, AppConstants.Teacher_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var resultData = await _teacherService.GetTeacherNotificationNotSeenCount(TeacherData.Id);

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
    public   class CustomMultipartFormDataStreamProviderForUplaodContents : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProviderForUplaodContents(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string fileName = "pdf-" + DateTime.Now.ToString("" + "dd_MM_yyyy_HH_mm_ss") + "";
            var s = headers.ContentDisposition.FileName.Replace("\"", string.Empty);

            var extension = Path.GetExtension(headers.ContentDisposition.FileName.Replace("\"", string.Empty));
            if (!string.IsNullOrWhiteSpace(extension))
                fileName += extension;
            return fileName;
        }
    }

}
