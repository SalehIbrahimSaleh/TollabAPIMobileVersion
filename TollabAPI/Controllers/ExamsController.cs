using DataAccess.Entities;
using DataAccess.Entities.Views;
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
using TollabAPI.Models;
using TollabAPI.Utils;

namespace TollabAPI.Controllers
{
    [Authorize]
    public class ExamsController : ApiController
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
        StudentService _studentService;
        ApplicationDbContext dbContext = new ApplicationDbContext();
        ExamService _examService;


        public ExamsController()
        {
            _teacherService = new TeacherService();
            _examService = new ExamService();
            _studentService = new StudentService();
        }

        #region TeacherExam Module
        [HttpPost]
        [Route("api/AddExam")]
        public async Task<HttpResponseMessage> AddExam(Exam exam)
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
                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (exam.CourseId == 0 || exam.CourseId == 0 || exam.ExamTypeId == 0 || string.IsNullOrEmpty(exam.Name))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Parameter");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Parameter);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (exam.DeadlineDate < exam.StartDate)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Deadline Date less than start date");
                    response.AddError(AppConstants.Code, AppConstants.Deadline_Date_Less_Than_Start_Date);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var CourseData = await _teacherService.GetCoursesById(exam.CourseId, teacherData.Id);
                if (CourseData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Course Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Course_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                exam.Publish = false;
                var ExamData = await _examService.AddExam(exam);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.Exam, ExamData);
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

        [HttpPost]
        [Route("api/UpdateExam")]
        public async Task<HttpResponseMessage> UpdateExam(Exam exam)
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
                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var ExamData = await _examService.GetExamById(exam.Id);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.Publish)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "This Exam Published and can not Update It");
                    response.AddError(AppConstants.Code, AppConstants.This_Exam_Published_and_can_not_Update_It);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (exam.CourseId == 0 || exam.CourseId == 0 || exam.ExamTypeId == 0 || string.IsNullOrEmpty(exam.Name))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Parameter");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Parameter);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (exam.DeadlineDate < exam.StartDate)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Deadline Date less than start date");
                    response.AddError(AppConstants.Code, AppConstants.Deadline_Date_Less_Than_Start_Date);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var studentExams = await _examService.CheckIfThisExamSolvedByStudents(ExamData.Id);
                if (studentExams.Count() > 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "You Can Not Update Exam Because Students Solved It");
                    response.AddError(AppConstants.Code, AppConstants.You_Can_Not_Update_This_Exam_Because_Students_Solved_It);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var questions = await _examService.GetAllExamQuestionWithAnswers(ExamData.Id, 0);
                if (questions.Count > 0 && ExamData.ExamTypeId != exam.ExamTypeId)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "You Can Not change Exam type Because Question added to It");
                    response.AddError(AppConstants.Code, AppConstants.You_Can_Not_change_Exam_type_Because_Question_added_It);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                exam.Publish = false;
                var ExamDataAfterUpdate = await _examService.UpdateExam(exam);
                if (ExamDataAfterUpdate == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.Exam, ExamDataAfterUpdate);
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
        [Route("api/DeleteExam")]
        public async Task<HttpResponseMessage> DeleteExam(long ExamId)
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
                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var ExamData = await _examService.GetExamById(ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.Publish)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "This Exam Published and can not Update It");
                    response.AddError(AppConstants.Code, AppConstants.This_Exam_Published_and_can_not_Update_It);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var studentExams = await _examService.CheckIfThisExamSolvedByStudents(ExamData.Id);
                if (studentExams.Count() > 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "You Can Not Delete Exam Because Students Solved It");
                    response.AddError(AppConstants.Code, AppConstants.You_Can_Not_Delete_This_Exam_Because_Students_Solved_It);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }

                var delete = _examService.DeleteExam(ExamData);
                if (delete == false)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.Exam, delete);
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

        [HttpPost]
        [Route("api/UploadExamFile")]
        public async Task<HttpResponseMessage> UploadExamFile()
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
                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }


                ExamQuestion examQuestion = new ExamQuestion();
                examQuestion.Id = Convert.ToInt64(HttpContext.Current.Request.Params["Id"]);
                examQuestion.Title = HttpContext.Current.Request.Params["Title"];
                examQuestion.ExamId = Convert.ToInt64(HttpContext.Current.Request.Params["ExamId"]);
                examQuestion.Degree = float.Parse(HttpContext.Current.Request.Params["Degree"]);
                examQuestion.ExamQuestionTypeId = AppConstants.ExamQuestionType_Pdf;
                examQuestion.OrderNumber = 1;
                if (examQuestion.ExamId < 1 || examQuestion.Degree < 1/*|| HttpContext.Current.Request.Files.Count <2*/)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Parameter");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Parameter);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var ExamData = await _examService.GetExamById(examQuestion.ExamId.Value);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.Publish)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "This Exam Published and can not Update It");
                    response.AddError(AppConstants.Code, AppConstants.This_Exam_Published_and_can_not_Update_It);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.ExamTypeId != 2)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide_Exam_Type");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Exam_Type);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                string fileSaveLocation = HttpContext.Current.Server.MapPath("~");
                if (fileSaveLocation.EndsWith("sws"))
                {
                    fileSaveLocation = fileSaveLocation.Remove(fileSaveLocation.Length - 3);

                }
                fileSaveLocation = fileSaveLocation + "ws/ExamFiles";

                CustomMultipartFormDataStreamProviderForUplaodEexamFile provider = new CustomMultipartFormDataStreamProviderForUplaodEexamFile(fileSaveLocation);
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);

                bool InsertedFileBefore = false;
                if (examQuestion.Id > 0)
                {
                    var oldExamQuestion = await _examService.GetExamQuestionById(examQuestion.Id);
                    InsertedFileBefore = !string.IsNullOrEmpty(oldExamQuestion.FilePath) || !string.IsNullOrEmpty(oldExamQuestion.IdealAnswerFilePath) ? true : false;
                }
                if (InsertedFileBefore == false && (provider.FileData == null || provider.FileData.Count == 0))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No Files");
                    response.AddError(AppConstants.Code, AppConstants.No_Files);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                if (provider.FileData != null && provider.FileData.Count > 0)
                {
                    for (int fileIndex = 0; fileIndex < provider.FileData.Count; fileIndex++)
                    {
                        MultipartFileData file = provider.FileData[fileIndex];
                        var fileName = Path.GetFileName(file.LocalFileName);
                        var newFileName = ExamData.Name + "_" + DateTime.Now.ToString("" + "dd_MM_yyyy_HH_mm_ss") + "_" + fileIndex + ".pdf";

                        if (fileName.EndsWith(".pdf"))
                        {
                            if (!string.IsNullOrEmpty(newFileName))
                            {
                                string path = fileSaveLocation + "/" + newFileName;
                                FileInfo myfileinf = new FileInfo(path);
                                myfileinf.Delete();
                            }
                            File.Move(fileSaveLocation + "/" + fileName, fileSaveLocation + "/" + newFileName);

                            if (file.Headers.ContentDisposition.Name.Replace("\"", string.Empty) == "FilePath")
                                examQuestion.FilePath = newFileName;
                            else

                                examQuestion.IdealAnswerFilePath = newFileName;
                        }
                    }

                }

                examQuestion.Title = ExamData.Name + "_" + DateTime.Now.ToString("" + "dd_MM_yyyy_HH_mm_ss") + "";
                var ExamQuestionData = await _examService.AddOrUpdateExamQuestion(examQuestion);
                response.AddModel(AppConstants.Exam, ExamQuestionData);
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

        [HttpGet]
        [Route("api/DelteExamFile")]
        public async Task<HttpResponseMessage> DelteExamFile(long ExamQuestionId)
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
                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                if (ExamQuestionId < 1)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Parameter");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Parameter);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var ExamQuestionData = await _examService.GetExamQuestionById(ExamQuestionId);
                if (ExamQuestionData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }

                var studentExams = await _examService.CheckIfThisExamSolvedByStudents((long)ExamQuestionData.ExamId);
                if (studentExams.Count() > 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "You Can Not Update Question To This Exam Because Students Solved It");
                    response.AddError(AppConstants.Code, AppConstants.You_Can_Not_Update_Question_To_This_Exam_Because_Students_Solved_It);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (!string.IsNullOrEmpty(ExamQuestionData.FilePath))
                {
                    string path = HttpContext.Current.Server.MapPath("~");
                    if (path.EndsWith("sws"))
                    {
                        path = path.Remove(path.Length - 3);

                    }
                    path = path + "ws/ExamFiles" + "/" + ExamQuestionData.FilePath;
                    //string path = HttpContext.Current.Server.MapPath("~/ExamFiles")+"/"+ExamQuestionData.FilePath;
                    FileInfo myfileinf = new FileInfo(path);
                    myfileinf.Delete();
                }
                if (!string.IsNullOrEmpty(ExamQuestionData.IdealAnswerFilePath))
                {
                    //string path = HttpContext.Current.Server.MapPath("~/ExamFiles") + "/" + ExamQuestionData.IdealAnswerFilePath;
                    string path = HttpContext.Current.Server.MapPath("~");
                    if (path.EndsWith("sws"))
                    {
                        path = path.Remove(path.Length - 3);

                    }
                    path = path + "ws/ExamFiles" + "/" + ExamQuestionData.IdealAnswerFilePath;
                    FileInfo myfileinf = new FileInfo(path);
                    myfileinf.Delete();
                }
                var delete = await _examService.DeleteExamQuestion(ExamQuestionData);
                if (!delete)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.Exam, delete);
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
        [Route("api/GetExamFile")]
        public async Task<HttpResponseMessage> GetExamFile(long ExamId)
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

                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                if (ExamId < 1)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Parameter");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Parameter);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var ExamData = await _examService.GetExamById(ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Parameter");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Parameter);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (ExamData.ExamTypeId != AppConstants.ExamTypeFile)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Exam type");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Exam_Type);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var ExamFile = await _examService.GetExamFileByExamId(ExamId);
                response.AddModel(AppConstants.Exam, ExamFile);
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

        [HttpPost]
        [Route("api/AddExamQuestionWithAnswers")]
        public async Task<HttpResponseMessage> AddExamQuestionWithAnswers(ExamQuestion examQuestion)
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
                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (string.IsNullOrEmpty(examQuestion.Title) || examQuestion.Degree < 1 || examQuestion.ExamQuestionTypeId < 1)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Parameter");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Parameter);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var ExamData = await _examService.GetExamById(examQuestion.ExamId.Value);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.Publish)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "This Exam Published and can not Update It");
                    response.AddError(AppConstants.Code, AppConstants.This_Exam_Published_and_can_not_Update_It);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }

                var studentExams = await _examService.CheckIfThisExamSolvedByStudents(ExamData.Id);
                if (studentExams.Count() > 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "You Can Not Add Question To This Exam Because Students Solved It");
                    response.AddError(AppConstants.Code, AppConstants.You_Can_Not_Add_Question_To_This_Exam_Because_Students_Solved_It);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.ExamTypeId != 1)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide_Exam_Type");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Exam_Type);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (examQuestion.ExamAnswers.Count == 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Please Add Answers");
                    response.AddError(AppConstants.Code, AppConstants.Please_Add_Answers);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (!string.IsNullOrEmpty(examQuestion.Image))
                {
                    var image = SetPhoto(examQuestion.Image);
                    examQuestion.Image = image;
                }
                var ExamQuestionData = await _examService.AddExamQuestionWithAnswers(examQuestion);
                response.AddModel(AppConstants.Exam, ExamQuestionData);
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


        [HttpPost]
        [Route("api/UpdateExamQuestionWithAnswers")]
        public async Task<HttpResponseMessage> UpdateExamQuestionWithAnswers(ExamQuestion examQuestion)
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
                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (string.IsNullOrEmpty(examQuestion.Title) || examQuestion.Id < 1 || examQuestion.Degree < 1 || examQuestion.ExamQuestionTypeId < 1)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Parameter");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Parameter);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var ExamData = await _examService.GetExamById(examQuestion.ExamId.Value);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                //if (ExamData.Publish)
                //{
                //    response.clearBody();
                //    response.AddError(AppConstants.Message, "This Exam Published and can not Update It");
                //    response.AddError(AppConstants.Code, AppConstants.This_Exam_Published_and_can_not_Update_It);
                //    return response.getResponseMessage(HttpStatusCode.BadRequest);

                //}
                if (ExamData.ExamTypeId != 1)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide_Exam_Type");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Exam_Type);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var studentExams = await _examService.CheckIfThisExamSolvedByStudents(ExamData.Id);
                if (studentExams.Count() > 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "You Can Not Update Question To This Exam Because Students Solved It");
                    response.AddError(AppConstants.Code, AppConstants.You_Can_Not_Update_Question_To_This_Exam_Because_Students_Solved_It);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }

                var ExamQuestionData = await _examService.GetExamQuestionById(examQuestion.Id);
                if (ExamQuestionData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (examQuestion.ExamAnswers.Count == 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Please Add Answers");
                    response.AddError(AppConstants.Code, AppConstants.Please_Add_Answers);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                if (!string.IsNullOrEmpty(examQuestion.Image))
                {
                    var image = "";
                    if (!examQuestion.Image.EndsWith(".png"))
                    {
                        if (!string.IsNullOrEmpty(ExamQuestionData.Image))
                        {
                            string path = HttpContext.Current.Server.MapPath("~/ws/Images/ExamImages") + "/" + ExamQuestionData.Image;
                            FileInfo myfileinf = new FileInfo(path);
                            myfileinf.Delete();
                        }
                        image = SetPhoto(examQuestion.Image);
                    }
                    else
                        image = examQuestion.Image;
                    examQuestion.Image = image;

                }

                var updatedExamQuestionWithAnswers = await _examService.UpdateExamQuestionWithAnswers(examQuestion);
                response.AddModel(AppConstants.Exam, updatedExamQuestionWithAnswers);
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
        [Route("api/DeleteExamQuestionWithAnswers")]
        public async Task<HttpResponseMessage> DeleteExamQuestionWithAnswers(long Id)
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
                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                if (Id < 1)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Parameter");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Parameter);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var ExamQuestionData = await _examService.GetExamQuestionById(Id);
                if (ExamQuestionData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid ExamQuestion Id");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_ExamQuestion_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var studentExams = await _examService.CheckIfThisExamSolvedByStudents(ExamQuestionData.ExamId.Value);
                if (studentExams.Count() > 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "You Can Not Update Question To This Exam Because Students Solved It");
                    response.AddError(AppConstants.Code, AppConstants.You_Can_Not_Update_Question_To_This_Exam_Because_Students_Solved_It);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }

                if (!string.IsNullOrEmpty(ExamQuestionData.Image))
                {
                    try
                    {
                        string path = HttpContext.Current.Server.MapPath("~/Images/ExamImages") + "/" + ExamQuestionData.Image;
                        FileInfo myfileinf = new FileInfo(path);
                        myfileinf.Delete();

                    }
                    catch { }
                }
                var delete = await _examService.DeleteExamQuestionWithAnswers(ExamQuestionData);
                if (!delete)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.Exam, delete);
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
        [Route("api/GetAllExamQuestionWithAnswers")]
        public async Task<HttpResponseMessage> GetAllExamQuestionWithAnswers(long ExamId, int Page)
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

                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                if (ExamId < 1)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Parameter");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Parameter);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var ExamData = await _examService.GetExamById(ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var ExamQuestionWithAnswers = await _examService.GetAllExamQuestionWithAnswers(ExamId, Page);
                response.AddModel(AppConstants.Exam, ExamQuestionWithAnswers);
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
        [Route("api/GetTeacherExams")]
        public async Task<HttpResponseMessage> GetTeacherExams(long? CourseId = null, bool? Publish = null, int Page = 0)
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

                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }


                var exams = await _examService.GetTeacherExams(teacherData.Id, CourseId, Publish, Page);
                response.AddModel(AppConstants.Exam, exams);
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
        [Route("api/GetExamDeatailsForTeacher")]
        public async Task<HttpResponseMessage> GetExamDeatailsForTeacher(long ExamId)
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

                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var ExamData = await _examService.GetExamDeatailsForTeacher(ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                response.AddModel(AppConstants.Exam, ExamData);
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
        [Route("api/PublishOrUnPublishExam")]
        public async Task<HttpResponseMessage> PublishExam(long ExamId, bool Publish = true)
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
                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }

                var ExamData = await _examService.GetExamById(ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var ExamQuestions = await _examService.GetAllExamQuestionWithAnswers(ExamData.Id, 0);
                if (ExamQuestions.Count == 0 && Publish == true)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "No_Questions_Added");
                    response.AddError(AppConstants.Code, AppConstants.No_Questions_Added);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                ExamData.Publish = Publish;
                var ExamDataAfterUpdate = await _examService.UpdateExam(ExamData);
                if (ExamDataAfterUpdate == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.Exam, ExamDataAfterUpdate);
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
        [Route("api/GetStudentExamsToCorrect")]
        public async Task<HttpResponseMessage> GetStudentExamsToCorrect(long ExamId, int Page, long? solveStatusId = null)
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
                //var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                //if (teacherData == null)
                //{
                //    response.clearBody();
                //    response.AddError(AppConstants.Message, "User not found");
                //    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                //    return response.getResponseMessage(HttpStatusCode.BadRequest);
                //}

                var ExamData = await _examService.GetExamById(ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                //if (!ExamData.Publish)
                //{
                //    response.clearBody();
                //    response.AddError(AppConstants.Message, "This Exam Not Published yet");
                //    response.AddError(AppConstants.Code, AppConstants.This_Exam_Not_Published_yet);
                //    return response.getResponseMessage(HttpStatusCode.BadRequest);

                //}
                var studentExams = await _examService.GetStudentExamsToCorrect(ExamId, Page, solveStatusId);

                response.AddModel(AppConstants.Exam, studentExams);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Added Successfuly");
                return response.getResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, e);
                response.AddError(AppConstants.Code, AppConstants.Result_error_try_later);
                return response.getResponseMessage(HttpStatusCode.InternalServerError);
            }


        }

        [HttpGet]
        [Route("api/GetExaminationSummary")]
        public async Task<HttpResponseMessage> GetExaminationSummary(long ExamId, long StudentId)
        {
            try
            {
                //787888
                //string IdentityUserId = User.Identity.GetUserId();
                //if (string.IsNullOrEmpty(IdentityUserId))
                //{
                //    response.clearBody();
                //    response.AddError(AppConstants.Message, "User not found");
                //    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                //    return response.getResponseMessage(HttpStatusCode.BadRequest);
                //}
                //var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                //if (teacherData == null)
                //{
                //    response.clearBody();
                //    response.AddError(AppConstants.Message, "User not found");
                //    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                //    return response.getResponseMessage(HttpStatusCode.BadRequest);
                //}

                var ExamData = await _examService.GetExamById(ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var StudentExamData = await _examService.GetStuedntExamForTeacher(ExamId, StudentId);
                if (StudentExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Not Solved Yet");
                    response.AddError(AppConstants.Code, AppConstants.Not_Solved_Yet);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var examinationSummary = await _examService.GetExaminationSummary(StudentExamData.Id, ExamId);
                response.AddModel(AppConstants.Exam, examinationSummary);
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
        [Route("api/GetStudentQuestionsToCorrect")]
        public async Task<HttpResponseMessage> GetStudentQuestionsToCorrect(long ExamId, long StudentId)
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
                //var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                //if (teacherData == null)
                //{
                //    response.clearBody();
                //    response.AddError(AppConstants.Message, "User not found");
                //    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                //    return response.getResponseMessage(HttpStatusCode.BadRequest);
                //}

                var ExamData = await _examService.GetExamById(ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var StudentExamData = await _examService.GetStuedntExamForTeacher(ExamId, StudentId);
                if (StudentExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Not Solved Yet");
                    response.AddError(AppConstants.Code, AppConstants.Not_Solved_Yet);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var studentExams = await _examService.GetStudentExamAnswers(StudentExamData.Id);

                response.AddModel(AppConstants.Exam, studentExams);
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

        [HttpPost]
        [Route("api/CorrectEssayQuestion")]
        public async Task<HttpResponseMessage> CorrectEssayQuestion(StudentAnswer studentAnswer)
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
                //var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                //if (teacherData == null)
                //{
                //    response.clearBody();
                //    response.AddError(AppConstants.Message, "User not found");
                //    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                //    return response.getResponseMessage(HttpStatusCode.BadRequest);
                //}

                var studentAnswerData = await _examService.GetStudentAnswer(studentAnswer.Id);
                if (studentAnswerData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Answer Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Answer_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                //if (studentAnswerData.ExamQuestionTypeId!=AppConstants.ExamQuestionType_Article)
                //{
                //    response.clearBody();
                //    response.AddError(AppConstants.Message, "Invalide Exam Question Type");
                //    response.AddError(AppConstants.Code, AppConstants.Invalide_ExamQuestion_Type);
                //    return response.getResponseMessage(HttpStatusCode.BadRequest);

                //}
                studentAnswerData.TeacherCorrectance = studentAnswer.TeacherCorrectance;
                if (studentAnswerData.ExamQuestionTypeId == AppConstants.ExamQuestionType_Article)
                {
                    studentAnswerData.Degree = studentAnswer.Degree;
                    if (studentAnswer.Degree > 0)
                    {
                        studentAnswerData.IsTrue = true;
                    }
                }

                studentAnswerData.Corrected = true;
                var correctedAnswer = await _examService.CorrectEssayQuestion(studentAnswerData);
                if (correctedAnswer == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.Exam, correctedAnswer);
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

        [HttpPost]
        [Route("api/CorrectPdfExam")]
        public async Task<HttpResponseMessage> CorrectPdfExam()
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
                //var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                //if (teacherData == null)
                //{
                //    response.clearBody();
                //    response.AddError(AppConstants.Message, "User not found");
                //    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                //    return response.getResponseMessage(HttpStatusCode.BadRequest);
                //}


                StudentAnswer studentAnswer = new StudentAnswer();
                studentAnswer.Id = Convert.ToInt64(HttpContext.Current.Request.Params["Id"]);
                studentAnswer.Degree = float.Parse(HttpContext.Current.Request.Params["Degree"]);
                studentAnswer.TeacherCorrectance = HttpContext.Current.Request.Params["TeacherCorrectance"];
                studentAnswer.VoicePath = HttpContext.Current.Request.Params["VoicePath"];
                studentAnswer.Duration = Convert.ToInt64(HttpContext.Current.Request.Params["Duration"]);



                //string fileSaveLocation = HttpContext.Current.Server.MapPath("~/AnswerFiles");
                string fileSaveLocation = HttpContext.Current.Server.MapPath("~");
                if (fileSaveLocation.EndsWith("sws"))
                {
                    fileSaveLocation = fileSaveLocation.Remove(fileSaveLocation.Length - 3);

                }
                fileSaveLocation = fileSaveLocation + "ws/AnswerFiles";
                CustomMultipartFormDataStreamProviderForUplaodEexamFile provider = new CustomMultipartFormDataStreamProviderForUplaodEexamFile(fileSaveLocation);
                List<string> files = new List<string>();
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);
                string fileName = null;
                foreach (MultipartFileData file in provider.FileData)
                {
                    fileName = Path.GetFileName(file.LocalFileName);
                    if (fileName.EndsWith("mp3"))
                    {
                        studentAnswer.VoicePath = fileName;
                    }
                }
                //
                var studentAnswerData = await _examService.GetStudentAnswer(studentAnswer.Id);
                if (studentAnswerData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Answer Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Answer_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (studentAnswerData.ExamQuestionTypeId != AppConstants.ExamQuestionType_Pdf)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Exam Question Type");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_ExamQuestion_Type);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var Question = await _examService.GetExamQuestionById(studentAnswerData.ExamQuestionId.Value);


                studentAnswerData.TeacherCorrectance = studentAnswer.TeacherCorrectance;
                studentAnswerData.Duration = studentAnswer.Duration;
                studentAnswerData.Degree = studentAnswerData.Degree > Question.Degree ? Question.Degree : studentAnswer.Degree;
                studentAnswerData.VoicePath = !string.IsNullOrEmpty(studentAnswer.VoicePath) ? studentAnswer.VoicePath : null;
                if (studentAnswer.Degree > 0)
                {
                    studentAnswerData.IsTrue = true;
                }


                var correctedAnswer = await _examService.CorrectEssayQuestion(studentAnswerData);
                if (correctedAnswer == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.Exam, correctedAnswer);
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
            string imagePath = "/ws/Images/ExamImages/";
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

        [HttpGet]
        [Route("api/SearchStudentsResults")]
        public async Task<HttpResponseMessage> SearchStudentsResults(string searchTerm = null, long? courseId = null)
        {
            try
            {
                //"2ec6f42f-f2f4-4f3f-93e8-ffaa2526c03e";
                string IdentityUserId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(IdentityUserId))
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
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
                var teacherData = await _teacherService.GetTeacherByIdentityId(IdentityUserId);
                if (teacherData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "User not found");
                    response.AddError(AppConstants.Code, AppConstants.User_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var studentsResults = await _examService.GetStudentsResults(searchTerm, courseId, teacherData.Id);
                if (studentsResults == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid student number or invalid course ");
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }

                response.AddModel("Results", studentsResults);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
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

        #region StudentExam Module

        [HttpGet]
        [Route("api/GetStudentExams")]
        public async Task<HttpResponseMessage> GetStudentExams(long? CourseId = null, long? SolveStatusId = null, int Page = 0)
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


                var StudentExams = await _examService.GetStuedntExam(StudentData.Id, CourseId, SolveStatusId, Page);
                response.AddModel(AppConstants.Exam, StudentExams);
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
        [Route("api/GetExamDeatailsForStudent")]
        public async Task<HttpResponseMessage> GetExamDeatailsForStudent(long ExamId)
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

                var ExamData = await _examService.GetExamDeatailsForStudent(StudentData.Id, ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                response.AddModel(AppConstants.Exam, ExamData);
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
        [Route("api/ViewResult")]
        public async Task<HttpResponseMessage> ViewResult(long ExamId)
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
                var ExamData = await _examService.GetExamById(ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var ExamResult = await _examService.GetStudentExamAnswersForStudent(StudentData.Id, ExamId);
                if (ExamResult.Count() == 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Not Solved Yet");
                    response.AddError(AppConstants.Code, AppConstants.Not_Solved_Yet);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.Exam, ExamResult);
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
        [Route("api/StartPdfExam")]
        public async Task<HttpResponseMessage> StartPdfExam(long ExamId)
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
                var ExamData = await _examService.GetExamById(ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.Publish == false)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "This Exam Not Published yet");
                    response.AddError(AppConstants.Code, AppConstants.This_Exam_Not_Published_yet);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.ExamTypeId == AppConstants.ExamTypeInteractive)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Exam Type");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Exam_Type);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.EndDate < DateTime.UtcNow)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "This Exam Closed");
                    response.AddError(AppConstants.Code, AppConstants.This_Exam_Closed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var studentExamData = await _examService.GetStudentExam(StudentData.Id, ExamId);

                if (studentExamData == null)
                {
                    StudentExam studentExam = new StudentExam()
                    {
                        CreationDate = DateTime.UtcNow,
                        ExamId = ExamId,
                        StudentId = StudentData.Id,
                        SolveStatusId = AppConstants.SolveStatus_NotComplete,
                        TotalScore = 0
                    };
                    studentExamData = await _examService.StartExam(studentExam);
                    if (studentExamData == null)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "Operation_Not_Completed");
                        response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }

                }
                var ExamQuestionWithAnswers = await _examService.GetAllExamQuestionWithAnswersForStudent(ExamId);
                StudentExamQuestionsWithAnswers studentExamQuestionsWithAnswers = new StudentExamQuestionsWithAnswers()
                {
                    StudentExam = studentExamData,
                    ExamQuestions = ExamQuestionWithAnswers

                };
                response.AddModel(AppConstants.Exam, studentExamQuestionsWithAnswers);
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

        [HttpPost]
        [Route("api/AnswerPdfExam")]
        public async Task<HttpResponseMessage> AnswerPdfExam()
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

                var StudentData = await _studentService.GetStudentByIdentityIdAsync(IdentityUserId);
                if (StudentData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Student not found");
                    response.AddError(AppConstants.Code, AppConstants.Student_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }


                StudentAnswer studentAnswer = new StudentAnswer();
                studentAnswer.CreationDate = DateTime.UtcNow;
                studentAnswer.ExamQuestionId = Convert.ToInt64(HttpContext.Current.Request.Params["ExamQuestionId"]);
                //studentAnswer.ExamQuestionTypeId = Convert.ToInt64(HttpContext.Current.Request.Params["ExamQuestionTypeId"]);
                studentAnswer.StudentExamId = Convert.ToInt64(HttpContext.Current.Request.Params["StudentExamId"]);



                var StudentExamData = await _examService.GetStudentExamById(studentAnswer.StudentExamId.Value);
                if (StudentExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "You did not  start this  Exam");
                    response.AddError(AppConstants.Code, AppConstants.You_did_not_start_this_Exam);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (StudentExamData.SolveStatusId != AppConstants.SolveStatus_NotComplete)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid_SolveStatus");
                    response.AddError(AppConstants.Code, AppConstants.Invalid_SolveStatus);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var examQuestion = await _examService.GetExamQuestionById(studentAnswer.ExamQuestionId.Value);
                if (examQuestion == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Result_Invalid_Parametrs");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Parametrs);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (examQuestion.ExamQuestionTypeId != AppConstants.ExamQuestionType_Pdf)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Exam Question Type");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_ExamQuestion_Type);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }

                //  string fileSaveLocation = HttpContext.Current.Server.MapPath("~/AnswerFiles");
                string fileSaveLocation = HttpContext.Current.Server.MapPath("~");
                if (fileSaveLocation.EndsWith("sws"))
                {
                    fileSaveLocation = fileSaveLocation.Remove(fileSaveLocation.Length - 3);

                }
                fileSaveLocation = fileSaveLocation + "ws/AnswerFiles";
                CustomMultipartFormDataStreamProviderForUplaodEexamFile provider = new CustomMultipartFormDataStreamProviderForUplaodEexamFile(fileSaveLocation);
                List<string> files = new List<string>();
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);
                string fileName = null;
                foreach (MultipartFileData file in provider.FileData)
                {
                    fileName = Path.GetFileName(file.LocalFileName);
                    if (fileName.EndsWith("pdf"))
                    {
                        studentAnswer.PdfAnswerPath = fileName;
                    }
                }
                //
                studentAnswer.ExamQuestionTypeId = examQuestion.ExamQuestionTypeId;

                var addAnswer = await _examService.AddStudentPdfAnswer(studentAnswer);
                if (addAnswer == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var UpdateStudentExam = await _examService.GetStudentExam(StudentData.Id, examQuestion.ExamId.Value);
                if (UpdateStudentExam != null)
                {
                    UpdateStudentExam.SolveStatusId = 3;
                    await _examService.UpdateStudentExam(UpdateStudentExam);

                }

                response.AddModel(AppConstants.Exam, addAnswer);
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
        [Route("api/StartExam")]
        public async Task<HttpResponseMessage> StartExam(long ExamId)
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
                var ExamData = await _examService.GetExamById(ExamId);
                if (ExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid Exam Id");
                    response.AddError(AppConstants.Code, AppConstants.Result_Invalid_Exam_Id);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.Publish == false)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "This Exam Not Published yet");
                    response.AddError(AppConstants.Code, AppConstants.This_Exam_Not_Published_yet);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.ExamTypeId == AppConstants.ExamTypeFile)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalide Exam Type");
                    response.AddError(AppConstants.Code, AppConstants.Invalide_Exam_Type);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (ExamData.EndDate < DateTime.UtcNow)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "This Exam Closed");
                    response.AddError(AppConstants.Code, AppConstants.This_Exam_Closed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                var IsStartedBefor = await _examService.GetStudentExam(StudentData.Id, ExamId);
                if (IsStartedBefor != null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "You started this  Exam");
                    response.AddError(AppConstants.Code, AppConstants.You_started_this_Exam);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                StudentExam studentExam = new StudentExam()
                {
                    CreationDate = DateTime.UtcNow,
                    ExamId = ExamId,
                    StudentId = StudentData.Id,
                    SolveStatusId = AppConstants.SolveStatus_NotComplete,
                    TotalScore = 0
                };
                var studentExamAdd = await _examService.StartExam(studentExam);
                if (studentExamAdd == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation_Not_Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                studentExamAdd.Duration = ExamData.Duration;
                var ExamQuestionWithAnswers = await _examService.GetAllExamQuestionWithAnswersForStudent(ExamId);
                StudentExamQuestionsWithAnswers studentExamQuestionsWithAnswers = new StudentExamQuestionsWithAnswers()
                {
                    StudentExam = studentExamAdd,
                    ExamQuestions = ExamQuestionWithAnswers

                };
                response.AddModel(AppConstants.Exam, studentExamQuestionsWithAnswers);
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

        [HttpPost]
        [Route("api/AnswerExam")]
        public async Task<HttpResponseMessage> AnswerExam(StudentAnswersWithStudentExamId answersWithStudentExamId)
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
                var StudentData = await _studentService.GetStudentByIdentityIdAsync(IdentityUserId);
                if (StudentData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Student not found");
                    response.AddError(AppConstants.Code, AppConstants.Student_Not_Found);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var StudentExamData = await _examService.GetStudentExamById(answersWithStudentExamId.StudentExamId);
                if (StudentExamData == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "You did not  start this  Exam");
                    response.AddError(AppConstants.Code, AppConstants.You_did_not_start_this_Exam);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);

                }
                if (StudentExamData.SolveStatusId != AppConstants.SolveStatus_NotComplete)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Invalid_SolveStatus");
                    response.AddError(AppConstants.Code, AppConstants.Invalid_SolveStatus);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                var SaveStudentAnswers = await _examService.SaveStudentAnswers(answersWithStudentExamId.StudentAnswers, answersWithStudentExamId.StudentExamId);
                if (SaveStudentAnswers == null)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Operation Not Completed");
                    response.AddError(AppConstants.Code, AppConstants.Operation_Not_Completed);
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.Exam, SaveStudentAnswers);
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



        #endregion

    }
    public class CustomMultipartFormDataStreamProviderForUplaodEexamFile : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProviderForUplaodEexamFile(string path) : base(path) { }
        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string fileName = "Exam_" + headers.ContentDisposition.Name.Replace("\"", string.Empty) + "_" + DateTime.Now.ToString("" + "dd_MM_yyyy_HH_mm_ss") + "";
            var s = headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            var extension = Path.GetExtension(headers.ContentDisposition.FileName.Replace("\"", string.Empty));
            //if (string.IsNullOrEmpty( extension))
            //{
            //    return "";
            //}
            if (!string.IsNullOrWhiteSpace(extension))
                fileName += extension;
            return fileName;
        }
    }

}
