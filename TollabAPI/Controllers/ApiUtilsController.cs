using Dapper;
using DataAccess.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class ApiUtilsController : ApiController
    {
        private CustomeResponseMessage response = new CustomeResponseMessage();
        ConnectionFactory conn = new ConnectionFactory();
        [AllowAnonymous]
        [HttpPost]
        [Route("api/SetPhoto")]
        public async Task<HttpResponseMessage> SetPhoto(ImageModel imageModel)
        {
            string imagePath = null;
            try
            {
                var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                if (basePath.EndsWith("sws"))
                {
                    basePath = basePath.Remove(basePath.Length - 3);
                }


                if (imageModel.ImageType ==(int)ImageFolders.Flags)
                {
                    imagePath = "/ws/Images/Flags/";
                }
              else  if (imageModel.ImageType == (int)ImageFolders.OffersImages)
                {
                    imagePath = "/ws/Images/OffersImages/";
                }
                else if (imageModel.ImageType == (int)ImageFolders.SectionImages)
                {
                    imagePath = "/ws/Images/SectionImages/";
                }
                else if (imageModel.ImageType == (int)ImageFolders.SubjectImages)
                {
                    imagePath = "/ws/Images/SubjectImages/";
                }
                else if (imageModel.ImageType == (int)ImageFolders.TeacherSubjectImages)
                {
                    imagePath = "/ws/Images/TeacherSubjectImages/";
                }
                else if (imageModel.ImageType == (int)ImageFolders.CourseImages)
                {
                    imagePath = "/ws/Images/CourseImages/";
                }
                else if (imageModel.ImageType == (int)ImageFolders.StudentImages)
                {
                    imagePath = "/ws/Images/StudentImages/";
                }
                else if (imageModel.ImageType == (int)ImageFolders.TeacherImages)
                {
                    imagePath = "/ws/Images/TeacherImages/";
                }
                else if (imageModel.ImageType == (int)ImageFolders.CourseVideos)
                {
                    imagePath = "/ws/CourseVideos/";
                  //  string fileNameV = "UI" + DateTime.Now.ToString("IMG" + "dd_MM_yyyy_HH_mm_ss");
                     byte[] fileBytesV = Convert.FromBase64String(imageModel.Image);
                    MemoryStream msV = new MemoryStream(fileBytesV);
                    string fullPathV = basePath + imagePath; /*+ fileNameV;*/
                    System.IO.Directory.CreateDirectory(basePath + imagePath);
                    FileStream fsV = new FileStream(fullPathV, FileMode.Create);
                    msV.WriteTo(fsV);
                    msV.Close();
                    fsV.Close();
                    fsV.Dispose();
                    string queryV = "Update  [" + imageModel.Table + "] set " + imageModel.CoulmnName + " ='" + fsV.Name + "'  OUTPUT INSERTED.Id where Id=" + imageModel.RecordId + "";
                    long idV = await conn.GetConnection.ExecuteScalarAsync<long>(queryV);
                    if (idV <= 0)
                    {
                        response.clearBody();
                        response.AddError(AppConstants.Message, "Not Updated");
                        return response.getResponseMessage(HttpStatusCode.BadRequest);
                    }
                    response.AddModel(AppConstants.Result, fsV.Name);
                    response.AddMeta(AppConstants.Result, AppConstants.Success);
                    response.AddMeta(AppConstants.Message, "Successfuly Updated");
                    return response.getResponseMessage(HttpStatusCode.OK);

                }
                else if (imageModel.ImageType == (int)ImageFolders.TeacherAssistantImages)
                {
                    imagePath = "/ws/Images/TeacherAssistantImages/";
                }
                else if(imageModel.ImageType == (int)ImageFolders.LiveImages)
                {
                    imagePath = "/ws/Images/LiveImages/";
                }
                string fileName = "UI" + DateTime.Now.ToString("IMG" + "dd_MM_yyyy_HH_mm_ss") + ".png";
                byte[] fileBytes = Convert.FromBase64String(imageModel.Image);
                MemoryStream ms = new MemoryStream(fileBytes);
                string fullPath = basePath + imagePath + fileName;
                System.IO.Directory.CreateDirectory(basePath + imagePath);
                FileStream fs = new FileStream(fullPath, FileMode.Create);
                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
                fs.Dispose();
                string query = "Update  [" + imageModel.Table + "] set " + imageModel.CoulmnName + " ='" + fileName + "'  OUTPUT INSERTED.Id where Id=" + imageModel.RecordId + "";
                long id = await conn.GetConnection.ExecuteScalarAsync<long>(query);
                if (id <= 0)
                {
                    response.clearBody();
                    response.AddError(AppConstants.Message, "Not Updated");
                    return response.getResponseMessage(HttpStatusCode.BadRequest);
                }
                response.AddModel(AppConstants.Result, fileName);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Successfuly Updated");
                return response.getResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "try_later");
                return response.getResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }


        }



        [HttpPost]
        [Route("api/SetVideo")]
        public HttpResponseMessage SetVideo()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            if (System.Web.HttpContext.Current.Request.Files.Count > 0)
            {
                var IntroVideo = System.Web.HttpContext.Current.Request.Files[0];
                var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");

                string   VideoPath = "/CourseVideos/";

                string fileName= "UI" + DateTime.Now.ToString("IMG" + "dd_MM_yyyy_HH_mm_ss");
                var extension = Path.GetExtension(IntroVideo.FileName);

                if (!string.IsNullOrWhiteSpace(extension))
                    fileName += extension;

                byte[] theVideoAsBytes = new byte[IntroVideo.ContentLength];
                using (BinaryReader theReader = new BinaryReader(IntroVideo.InputStream))
                {
                    theVideoAsBytes = theReader.ReadBytes(IntroVideo.ContentLength);
                }
                byte[] fileBytes = theVideoAsBytes;
                MemoryStream msV = new MemoryStream(fileBytes);
                string fullPathV = basePath + VideoPath + fileName;
                System.IO.Directory.CreateDirectory(basePath + VideoPath);
                FileStream fsV = new FileStream(fullPathV, FileMode.Create);
                msV.WriteTo(fsV);
                msV.Close();
                fsV.Close();
                fsV.Dispose();
                response.AddModel(AppConstants.Result, fileName);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Successfuly saved");
                return response.getResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "try_later");
                return response.getResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }


        [HttpPost]
        [Route("api/Upload")]
        public async Task<HttpResponseMessage> Upload()
        {
            try
            {
                var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                if (basePath.EndsWith("sws"))
                {
                    basePath = basePath.Remove(basePath.Length - 3);
                }
                string VideoPath = "/ws/CourseVideos/";
                string fullPathV = basePath + VideoPath;

                var provider = new MultipartFormDataStreamProvider(fullPathV);
                var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
                foreach (var header in Request.Content.Headers)
                {
                    content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                //Code for renaming the random file to Original file name  
                //Code renaming ends... 
                await content.ReadAsMultipartAsync(provider);

                string uploadingFileName = provider.FileData.Select(x => x.LocalFileName).FirstOrDefault();
                string originalFileName = String.Concat("\\" + (provider.Contents[0].Headers.ContentDisposition.FileName).Trim(new Char[] { '"' }));
                string fileName = "UI" + DateTime.Now.ToString("IMG" + "dd_MM_yyyy_HH_mm_ss");
                var extension = Path.GetExtension(originalFileName);
                if (!string.IsNullOrWhiteSpace(extension))
                    fileName += extension;
                originalFileName = fullPathV + fileName;
                if (File.Exists(originalFileName))
                {
                    File.Delete(originalFileName);
                }
                File.Move(uploadingFileName, originalFileName);

                response.AddModel(AppConstants.Result, fileName);
                response.AddMeta(AppConstants.Result, AppConstants.Success);
                response.AddMeta(AppConstants.Message, "Successfuly saved");
                return response.getResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                response.clearBody();
                response.AddError(AppConstants.Message, "try_later");
                return response.getResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }

        }

    }



    public enum ImageFolders 
    {
        Flags =1,
        SectionImages= 2,
        OffersImages=3,
        QuestionImagesOrVoices=4,
        CourseImages = 5,
        StudentImages=6,
        SubjectImages=7,
        TeacherImages=8,
        TeacherSubjectImages=9,
        CourseVideos=10,
        TeacherAssistantImages = 11,
        LiveImages = 12
    }
}
