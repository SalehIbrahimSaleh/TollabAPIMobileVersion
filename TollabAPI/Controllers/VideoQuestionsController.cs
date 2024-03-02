using DataAccess.Entities;
using DataAccess.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
using TollabAPI.Utils;

namespace TollabAPI.Controllers
{
    [Authorize]
    public class VideoQuestionsController : ApiController
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
        StudentService _studentService;
        TeacherService _teacherService;
        VideoQuestionService _videoQuestionService;
        public VideoQuestionsController()
        {
            _studentService = new StudentService();
            _teacherService = new TeacherService();
            _videoQuestionService = new VideoQuestionService();
        }


        #region AddContact
        [HttpPost]
        [Route("api/AddContact")]
        public async Task<HttpResponseMessage> AddContact(ContactUs contactUs)
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
                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);

                if (StudentData == null&& teacherData==null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (StudentData!=null)
                {
                    contactUs.StudentId = StudentData.Id;

                }
                if (teacherData!=null)
                {
                    contactUs.TeacherId = teacherData.Id;
                }
                contactUs.CreationDate = DateTime.UtcNow;
                contactUs.Status = false;
                var resultData = await _studentService.AddContact(contactUs);
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

        #endregion

        [HttpPost]
        [Route("api/AddQuestion")]
        public async Task<HttpResponseMessage> AddQuestion()
        {
            string fileSaveLocation = "";
            string returnedUrl = "";
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

                VideoQuestion videoQuestion = new VideoQuestion();

                videoQuestion.Question = HttpContext.Current.Request.Params["Question"];
                videoQuestion.Minute =float.Parse( HttpContext.Current.Request.Params["Minute"]);
                videoQuestion.Image = HttpContext.Current.Request.Params["Image"];
                if (!string.IsNullOrEmpty(videoQuestion.Image))
                {
                    var image = SetPhoto(videoQuestion.Image);
                    videoQuestion.Image = image;
                }
                if (HttpContext.Current.Request.Params["ContentId"] != null)
                {
                    videoQuestion.ContentId = Convert.ToInt64(HttpContext.Current.Request.Params["ContentId"]);
                }

                if (HttpContext.Current.Request.Params["LiveId"] != null)
                {
                    videoQuestion.LiveId = Convert.ToInt64(HttpContext.Current.Request.Params["LiveId"]);
                }
                videoQuestion.ViewMyAccount =Convert.ToBoolean(HttpContext.Current.Request.Params["ViewMyAccount"]);
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                // Prepare CustomMultipartFormDataStreamProvider in which our multipart form
                // data will be loaded.
                 fileSaveLocation = HttpContext.Current.Server.MapPath("~");
                 returnedUrl = fileSaveLocation;
                if (fileSaveLocation.EndsWith("sws"))
                {
                    fileSaveLocation = fileSaveLocation.Remove(fileSaveLocation.Length -3);

                }
                fileSaveLocation = fileSaveLocation + "ws/Images/QuestionImagesOrVoices";
                CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
                List<string> files = new List<string>();
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);
                string fileName = null;
                foreach (MultipartFileData file in provider.FileData)
                {
                    fileName = Path.GetFileName(file.LocalFileName);
                   
                    files.Add(Path.GetFileName(file.LocalFileName));
                }
                //
                videoQuestion.Voice = fileName;
                videoQuestion.StudentId = StudentData.Id;
                videoQuestion.CreationDate = DateTime.UtcNow;
               
                var resultData = await _videoQuestionService.AddQuestion(videoQuestion);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.User, resultData);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddError(AppConstants.Error, fileSaveLocation + "||" + returnedUrl);

                response.AddMeta(AppConstants.Message, "Added Successfuly");
                return response.getResponseMessage(HttpStatusCode.OK);

            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");
                response.AddError(AppConstants.Code, AppConstants.Result_error_try_later);
                response.AddError(AppConstants.Error, fileSaveLocation+"||"+ returnedUrl);
                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }


        }

        [HttpPost]
        [Route("api/AddStudentReply")]
        public async Task<HttpResponseMessage> AddStudentReply()
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

                //start
                Reply reply   = new  Reply();

                reply.Comment = HttpContext.Current.Request.Params["Comment"];
                reply.VideoQuestionId = Convert.ToInt64(HttpContext.Current.Request.Params["VideoQuestionId"]);
                reply.ViewMyAccount =Convert.ToBoolean( HttpContext.Current.Request.Params["ViewMyAccount"]);
                reply.Image =HttpContext.Current.Request.Params["Image"];
                if (!string.IsNullOrEmpty(reply.Image))
                {
                    var image = SetPhoto(reply.Image);
                    reply.Image = image;
                }
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                // Prepare CustomMultipartFormDataStreamProvider in which our multipart form
                // data will be loaded.
                string fileSaveLocation = HttpContext.Current.Server.MapPath("~");
                if (fileSaveLocation.EndsWith("sws"))
                {
                    fileSaveLocation = fileSaveLocation.Remove(fileSaveLocation.Length - 3);

                }
                fileSaveLocation = fileSaveLocation + "ws/Images/QuestionImagesOrVoices";
                CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
                List<string> files = new List<string>();
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);
                string fileName = null;
                foreach (MultipartFileData file in provider.FileData)
                {
                    fileName = Path.GetFileName(file.LocalFileName);

                    files.Add(Path.GetFileName(file.LocalFileName));
                }
                //
                reply.Voice = fileName;
                reply.StudentId = StudentData.Id;
                reply.CreationDate = DateTime.UtcNow;
                //end
                var resultData = await _videoQuestionService.AddReplay(reply);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.User, resultData);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Added Successfuly");
                return response.getResponseMessage(HttpStatusCode.OK);

            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "An Error Occurred");
                response.AddError(AppConstants.Code, AppConstants.Result_error_try_later);
                response.AddError(AppConstants.Error, e);
                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }


        }


        [HttpPost]
        [Route("api/AddTeacherReply")]
        public async Task<HttpResponseMessage> AddTeacherReply()
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                TeacherAssistant assistantData = null;

                var IsTeacherAssistant = Request.GetOwinContext().Authentication.User.IsInRole("TeacherAssistant");
                if (IsTeacherAssistant)
                {
                    assistantData = await _teacherService.GetTeacherAssistantByIdentityId(IdentityUserId);
                    if (assistantData == null)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "User not found");
                        response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }
                    IdentityUserId = (await _teacherService.GetTeacherById(assistantData.TeacherId.Value)).IdentityId;
                }

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

                //start
                Reply reply = new Reply();

                reply.Comment = HttpContext.Current.Request.Params["Comment"];
                reply.VideoQuestionId = Convert.ToInt64(HttpContext.Current.Request.Params["VideoQuestionId"]);
                reply.ViewMyAccount = true;
                reply.Image = HttpContext.Current.Request.Params["Image"];
                if (!string.IsNullOrEmpty(reply.Image))
                {
                    var image = SetPhoto(reply.Image);
                    reply.Image = image;
                }
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                // Prepare CustomMultipartFormDataStreamProvider in which our multipart form
                // data will be loaded.
                string fileSaveLocation = HttpContext.Current.Server.MapPath("~");
                if (fileSaveLocation.EndsWith("sws"))
                {
                    fileSaveLocation = fileSaveLocation.Remove(fileSaveLocation.Length - 3);

                }
                fileSaveLocation = fileSaveLocation + "ws/Images/QuestionImagesOrVoices";
                CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
                List<string> files = new List<string>();

                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                //if (provider.Contents.Count>0)
                //{
                //    await Request.Content.ReadAsMultipartAsync(provider);

                //}
                await Request.Content.ReadAsMultipartAsync(provider);

                string fileName = null;
                foreach (MultipartFileData file in provider.FileData)
                {
                    fileName = Path.GetFileName(file.LocalFileName);

                    files.Add(Path.GetFileName(file.LocalFileName));
                }
                //
                reply.Voice = fileName;
                reply.TeacherId = TeacherData.Id;
                reply.CreationDate = DateTime.UtcNow;
                if (assistantData!=null)
                {
                    reply.TeacherAssistantId = assistantData.Id;
                }
                //end
                var resultData = await _videoQuestionService.AddReplay(reply);
                if (resultData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.User, resultData);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Added Successfuly");
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
        [Route("api/GetQuestions")]
        public async Task<HttpResponseMessage> GetQuestions(long CourseId,int Page=0, long VideoQuestionId=0)
        {
            try
            {
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code ,AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var resultData = await _videoQuestionService.GetQuestions(CourseId,Page, VideoQuestionId);
                if (resultData.Count() == 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_returned);
                    return response.getResponseMessage(HttpStatusCode.OK);
                }
                response.AddModel(AppConstants.User, resultData);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Added Successfuly");
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
        [Route("api/GetLiveQuestions")]
        public async Task<HttpResponseMessage> GetLiveQuestions(long LiveId, int Page = 0, long VideoQuestionId = 0)
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

                var resultData = await _videoQuestionService.GetLiveQuestions(LiveId, Page, VideoQuestionId);
                if (resultData.Count() == 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Data");
                    response.AddError(AppConstants.Code, AppConstants.Result_No_Data_returned);
                    return response.getResponseMessage(HttpStatusCode.OK);
                }
                response.AddModel(AppConstants.User, resultData);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Added Successfuly");
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


        public string SetPhoto(string image)
        {
            var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");

            if (basePath.EndsWith("sws"))
            {
                basePath = basePath.Remove(basePath.Length - 3);

            }
           // basePath = basePath + "ws/Images/QuestionImagesOrVoices";
            string imagePath = "/ws/Images/QuestionImagesOrVoices/";
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

    }
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string fileName = "Voice-" + DateTime.Now.ToString("" + "dd_MM_yyyy_HH_mm_ss") + "";
            var s= headers.ContentDisposition.FileName.Replace("\"", string.Empty);

            var extension = Path.GetExtension(headers.ContentDisposition.FileName.Replace("\"", string.Empty));
            if (!string.IsNullOrWhiteSpace(extension))
                fileName += extension;
            return fileName;//headers.ContentDisposition.FileName.Replace("\"", string.Empty);
        }
    }
}
