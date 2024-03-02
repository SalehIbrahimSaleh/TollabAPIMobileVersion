using DataAccess.DTOs;
using DataAccess.Entities;
using DataAccess.QueryResults;
using DataAccess.UnitOfWork;
using DataAccess.Utils;
using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using static Nexmo.Api.Search;
using static System.Net.WebRequestMethods;

namespace DataAccess.Services
{
    public class StudentService
    {
        StudentUnit _studentUnit;
        VideoQuestionUnit _videoQuestionUnit;
        
        public StudentService()
        {
            _studentUnit = new StudentUnit();
            _videoQuestionUnit = new VideoQuestionUnit();
        }
        #region Register Methods

        public async Task<Student> Register(Student student)
        {
            int MaxCountLogin = 0;
            var setting = await _studentUnit.SettingRepository.Get(21);
            if (setting != null)
            {
                MaxCountLogin = Convert.ToInt32(setting.SettingValue);
            }
            Student StudentData = null;
            student.NumberMaxLoginCount = MaxCountLogin;
            student.NumberCurrentLoginCount = 0;
            var id = await _studentUnit.StudentRepository.Add(student);
            if (id > 0)
            {
                await SendVerifyCode(student.PhoneKey + student.Phone, student.IdentityId);
                StudentData = await _studentUnit.StudentRepository.Get(id);
                if (StudentData.PhoneKey.StartsWith("+965"))
                {
                    var link = await SendPaymentLink(StudentData);

                }
                return StudentData;
            }

            StudentData.Vcode = 0;
            return StudentData;
        }
        public async Task<long> AddDownloadedCourses(DownloadObj obj)
        {
            var id = await _studentUnit.DownloadObjRepository.Add(obj);         
            return id;
        }

        

        public async Task<Student> GetStudentByIdentityIdAsync(string IdentityId)
        {
            Student student = null;
            var data = await _studentUnit.StudentRepository.GetAll(" Where IdentityId=N'" + IdentityId + "'");
            if (data.Count() > 0)
            {
                student = data.FirstOrDefault();
                student.Interests= await _studentUnit.DepartmentRepository.GetInterests(student.Id);
            }

            return student;
        }
        public async Task<Student> GetStudentProfileByPhoneAndSendCode(string PhoneNumber)
        {
            var student = await _studentUnit.StudentRepository.GetStudentDataByPhone(PhoneNumber);
            if (student != null)
            {
                await SendVerifyCode(PhoneNumber, student.IdentityId);
                var updatedStudent = await _studentUnit.StudentRepository.GetStudentByIdentityIdAsync(student.IdentityId);
                //updatedStudent.Vcode = 0;
                return updatedStudent;
            }
            return student;
        }
        public async Task<Student> GetStudentById(long id)
        {
            Student student = null;
            var data = await _studentUnit.StudentRepository.GetAll($" Where Id={id}");
            if (data.Count() > 0)
            {
                student = data.FirstOrDefault();
            }

            return student;
        }
        public async Task<Student> GetStudentProfileAndSendCode(string PhoneNumber, string IdentityId)
        {
            var student = await _studentUnit.StudentRepository.GetStudentByIdentityIdAsync(IdentityId);
            if (student != null)
            {
                await SendVerifyCode(PhoneNumber, IdentityId);
                var    updatedStudent = await _studentUnit.StudentRepository.GetStudentByIdentityIdAsync(IdentityId);
                //updatedStudent.Vcode = 0;
                return updatedStudent;
            }
            return student;
        }

        public async Task<int> SendVerifyCode(string PhoneNumber, string IdentityId)
        {
            try
            {
                if (PhoneNumber.StartsWith("+"))
                {
                    PhoneNumber = PhoneNumber.Remove(0, 1);
                }
                 int _min = 1000;
                int _max = 9999;
                Random _rdm = new Random();
                int vcode = _rdm.Next(_min, _max);
                string TollabApp = "Tollab App";
                string Message = "شكراً لإختياركم طلاب رمز التحقق الخاص بك هو " + vcode + "";
                if (PhoneNumber.StartsWith("965"))
                {
                    string SenderName = "Tollab App";
                    //var postData = "http://api.smart2group.net/api/send.aspx?username=tollab&password=Kw7q7cU9ES56&language=2&sender=" + TollabApp + "&mobile=" + PhoneNumber + "" + "&message=" + Message + "";
                    var postData = "https://api-server14.com/api/send.aspx?apikey=LRVPLiibcfjOLalFjOa7qZYBI&language=1&sender="+SenderName+ "&mobile="+ PhoneNumber+"&message=" + Message + "";
                   // var content = new StringContent(postData, Encoding.UTF8, "application/json");
                    string URL = postData;
                    var httpClient = new HttpClient();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "GET";
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    ServicePointManager.DefaultConnectionLimit = 9999;
                    var response = await httpClient.GetAsync(URL);
                    var responseString = await response.Content.ReadAsStringAsync();

                }
                else if (PhoneNumber.StartsWith("20"))
                {

                    await SendcodeSmsMisr(PhoneNumber, vcode.ToString());
                }
                else
                {
                    await CallNexmo(PhoneNumber, Message);

                }

                var student = await _studentUnit.StudentRepository.GetStudentByIdentityIdAsync(IdentityId);

                 
                    student.Vcode = vcode;
               
                //student.Vcode = vcode;
                student.ExpirationVCodeDate = DateTime.UtcNow.AddMinutes(15);
                student.Verified = false;
                await _studentUnit.StudentRepository.Update(student);//(PhoneNumber, vcode);
                return vcode;
            }
            catch (Exception e)
            {
                return 0;
            }

        }

        public async Task<Student> GetStudentProfileByPhone(string phone)
        {
            var StudentData = await _studentUnit.StudentRepository.GetStudentProfileByPhone(phone);
            return StudentData;
        }
        public async Task<Student> GetStudentDataByPhone(string phone)
        {
            var StudentData = await _studentUnit.StudentRepository.GetStudentDataByPhone(phone);
            return StudentData;
        }
        public async Task<Student> GetStudentDataByPhoneOrEmail(string phone, string email)
        {
            var StudentData = await _studentUnit.StudentRepository.GetStudentDataByPhoneOrEmail(phone, email);
            return StudentData;
        }
        
        public async Task<string> SendPaymentLink(Student student)
        {
            try
            {
                // var student = await _studentUnit.StudentRepository.GetStudentByIdentityIdAsync(IdentityId);
                string PhoneNumber = student.PhoneKey + student.Phone;
                if (PhoneNumber.StartsWith("+"))
                {
                    PhoneNumber = PhoneNumber.Remove(0, 1);
                }

                var paymentKey = PaymentKeyGenerator.getPaymentKey();
                string arabicMessage = "لتتمكن من الإشتراك في دوراتنا أو شحن محفظتك  قم بزيارة الرابط التالي ";
                string EndArabicmessage = "طلاب. افضل. اوفر.   ";
                var message = arabicMessage + "  " + "https://tollab.azurewebsites.net/Pay/" + paymentKey + student.Id.ToString() + "  " + EndArabicmessage;

                string TollabApp = "Tollab App";

                if (PhoneNumber.StartsWith("965"))
                {
                    var postData = "http://api.smart2group.net/api/send.aspx?username=tollab&password=Kw7q7cU9ES56&language=2&sender=" + TollabApp + "&mobile=" + PhoneNumber + "" + "&message=" + message + "";
                    var content = new StringContent(postData, Encoding.UTF8, "application/json");
                    string URL = postData;
                    var httpClient = new HttpClient();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "POST";
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    ServicePointManager.DefaultConnectionLimit = 9999;
                    var response = await httpClient.PostAsync(URL, content);
                }
                else
                {
                    await CallNexmo(PhoneNumber, message);
                }
                student.PaymentLink = "https://tollab.azurewebsites.net/Pay/"+ paymentKey+student.Id.ToString();
                student.PaymentKey= paymentKey + student.Id.ToString();
                student.LastSendDate = DateTime.UtcNow;
                await _studentUnit.StudentRepository.Update(student);//(PhoneNumber, vcode);
                return message;
            }
            catch (Exception e)
            {
                return "0";
            }

        }

        public async Task<string> SendPaymentLinkAgain(Student student)
        {
            try
            {
                // var student = await _studentUnit.StudentRepository.GetStudentByIdentityIdAsync(IdentityId);
                string PhoneNumber = student.PhoneKey + student.Phone;
                if (PhoneNumber.StartsWith("+"))
                {
                    PhoneNumber = PhoneNumber.Remove(0, 1);
                }
                var paymentKey = PaymentKeyGenerator.getPaymentKey();
                string arabicMessage = "لتتمكن من الإشتراك في دوراتنا أو شحن محفظتك  قم بزيارة الرابط التالي ";
                string EndArabicmessage = "طلاب. افضل. اوفر.     ";

                var message =arabicMessage + " " + student.PaymentLink+ "  " + EndArabicmessage;
                string TollabApp = "Tollab App";
                if (PhoneNumber.StartsWith("965"))
                {

                    var postData = "http://api.smart2group.net/api/send.aspx?username=tollab&password=Kw7q7cU9ES56&language=2&sender=" + TollabApp + "&mobile=" + PhoneNumber + "" + "&message=" + message + "";
                    var content = new StringContent(postData, Encoding.UTF8, "application/json");
                    string URL = postData;
                    var httpClient = new HttpClient();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "POST";
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    ServicePointManager.DefaultConnectionLimit = 9999;
                    var response = await httpClient.PostAsync(URL, content);
                }
                else
                {
                    await CallNexmo(PhoneNumber, message);
                }
               student.LastSendDate = DateTime.UtcNow;
               await _studentUnit.StudentRepository.Update(student);//(PhoneNumber, vcode);
               return message;
               
            }
            catch (Exception e)
            {
                return "0";
            }

        }

        public async Task<string> SendRenewableMessage(Student student)
        {
            try
            {
                // var student = await _studentUnit.StudentRepository.GetStudentByIdentityIdAsync(IdentityId);
                string PhoneNumber = student.PhoneKey + student.Phone;
                if (PhoneNumber.StartsWith("+"))
                {
                    PhoneNumber = PhoneNumber.Remove(0, 1);
                }
                string arabicMessage = "برجاء العلم أنه سيتم انتهاء الإشتراك الخاص بك خلال ثلاثة أيام برجاء تجديد الاشتراك بعدأنتهاءه  ";
                string EndArabicmessage = "طلاب. افضل. اوفر.   ";
                var message = arabicMessage +"  " + EndArabicmessage;
                string TollabApp = "Tollab App";
                if (PhoneNumber.StartsWith("965"))
                {
                    var postData = "http://api.smart2group.net/api/send.aspx?username=tollab&password=Kw7q7cU9ES56&language=2&sender=" + TollabApp + "&mobile=" + PhoneNumber + "" + "&message=" + message + "";
                    var content = new StringContent(postData, Encoding.UTF8, "application/json");
                    string URL = postData;
                    var httpClient = new HttpClient();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "POST";
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    ServicePointManager.DefaultConnectionLimit = 9999;
                    var response = await httpClient.PostAsync(URL, content);
                }
                else
                {
                    await CallNexmo(PhoneNumber, message);
                }
              return message;

            }
            catch (Exception e)
            {
                return "0";
            }

        }

        public async Task SendcodeSmsMisr(String to, string Vcode)
        {

            if (to.StartsWith("+"))
            {
                to = to.Substring(1);
            }
        //string SenderName = "Express";
        //if (to.StartsWith("2011"))
        //{
        // string   SenderName = "TollabApp";
        //  }


       // https://smsmisr.com/api/OTP/?environment=1&username=lAvYEiSM&sender=f0c640e1f0dffd6d6e2869fdf66d6f1cc5a5cd8821ccbeedbab6b0ce7246b5ef&password=5c28b4d3554e5671009aa2292334468e3edf146db946c6f4d2253066357d8138&mobile=00201203369538&otp=12344&template=eb60c2a456825a40a56dd36813e8ba8740b6dbe1c5d6921034bd9508e78d5fac

            string uri = "https://smsmisr.com/api/OTP/?environment=1&username=lAvYEiSM&password=5c28b4d3554e5671009aa2292334468e3edf146db946c6f4d2253066357d8138&sender=f0c640e1f0dffd6d6e2869fdf66d6f1cc5a5cd8821ccbeedbab6b0ce7246b5ef" + "&mobile=" + to + "&otp="+Vcode+ "&template=eb60c2a456825a40a56dd36813e8ba8740b6dbe1c5d6921034bd9508e78d5fac";
            var client = new HttpClient(new HttpClientHandler
            {
                UseProxy = false
            });
            string PostParams = @"";
            var prm = new StringContent(PostParams);
            var response = await client.PostAsync(uri, prm);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseCode = response.StatusCode;
        }

        public async Task CallNexmo(string PhoneNumber, string text)
        {
            const string API_KEY = "b3722f03";
            const string API_SECRET = "cJBDgju04NrtyaG9";

            var client = new Client(creds: new Nexmo.Api.Request.Credentials(
                nexmoApiKey: API_KEY, nexmoApiSecret: API_SECRET));
            var results = client.SMS.Send(new SMS.SMSRequest
            {
                from = "Tollab App",
                to = PhoneNumber,
                text = text,
                type = "unicode"
            });


        }

        public async Task<Student> Verify(string PhoneKey, string Phone, int? vcode)
        {
            Phone = Phone.Trim();
            Phone = Phone.Replace(" ", "");
            var student = await _studentUnit.StudentRepository.Verify(PhoneKey, Phone, vcode);
            if (student != null)
            {
                student.Interests = await _studentUnit.DepartmentRepository.GetInterests(student.Id);
                student.Sections = await _studentUnit.SectionRepository.GetSectionsByCountryId(student.CountryId);
                var country = await _studentUnit.CountryRepository.Get(student.CountryId);
                student.Currency = country.Currency;
                student.CurrencyLT = country.CurrencyLT;
                student.CountryCode = country.Code;
                student.Enabled = true;
                student.Verified = true;
            }
            //if (student.PhoneKey.StartsWith("+965"))
            //{
            //    if (student.PaymentKey == null)
            //    {
            //        var link = await SendPaymentLink(student);
            //    }

            //    if (student.PaymentLink != null)
            //    {
            //        var link = await SendPaymentLinkAgain(student);
            //    }
            //}
            return student;
        }

        public async Task<Student> GetStudentProfile(long StudentId)
        {
            Student student = await _studentUnit.StudentRepository.Get(StudentId);
            if (student != null)
            {
                student.Interests = await _studentUnit.DepartmentRepository.GetInterests(StudentId);
                student.Sections = await _studentUnit.SectionRepository.GetSectionsByCountryId(student.CountryId);
                student.NumberOfCourses = await _studentUnit.StudentCourseRepository.GetCourseCountByStudentId(StudentId);
                var country = await _studentUnit.CountryRepository.Get(student.CountryId);
                student.Currency = country.Currency;
                student.CurrencyLT = country.CurrencyLT;
                student.CountryCode = country.CountryCode;
                     var resultData = await GetStudentPackages(student.Id, 0);
                var package = new Package();

                if (resultData != null)
                {
                    package.id = resultData?.FirstOrDefault()?.Id;
                    package.color = resultData?.FirstOrDefault()?.Color;
                    package.details = resultData?.FirstOrDefault()?.Description;
                    package.name = resultData?.FirstOrDefault()?.Name;
                    package.period = resultData?.FirstOrDefault()?.Period;
                    package.price = resultData?.FirstOrDefault()?.NewPrice;
                    package.SKUNumber = resultData?.FirstOrDefault()?.SkuNumber;
                    package.PackageExpireDate = resultData?.FirstOrDefault()?.PackageEndDate.ToString("dd/MM/yyyy");
                    student.package = package;
                }
            }
            return student;
        }

        public async Task<StudentPushToken> AddStudentPushToken(StudentPushToken studentPushToken)
        {
            StudentPushToken NewStudentPushToken = null;
            var tokens = await _studentUnit.StudentPushTokenRepository.GetAll(" where StudentId =" + studentPushToken.StudentId + " ");
            if (tokens.Count() > 0)
            {
                foreach (var token in tokens)
                {
                    if (token.Token == studentPushToken.Token && token.OS == studentPushToken.OS)
                    {
                        token.ModifiedDate = DateTime.UtcNow;
                        token.DeviceVersion = studentPushToken.DeviceVersion;
                        token.ApplicationVersion = studentPushToken.ApplicationVersion;
                         
                        await _studentUnit.StudentPushTokenRepository.Update(token);
                        return token;
                    }
                    else if (token.Token != studentPushToken.Token && token.OS == studentPushToken.OS)
                    {
                        token.Token = studentPushToken.Token;
                        token.ModifiedDate = DateTime.UtcNow;
                        token.DeviceVersion = studentPushToken.DeviceVersion;
                        token.ApplicationVersion = studentPushToken.ApplicationVersion;
                        var d = await _studentUnit.StudentPushTokenRepository.Update(token);
                        return token;
                    }

                }
            }
            studentPushToken.ModifiedDate = DateTime.UtcNow;
            var result = await _studentUnit.StudentPushTokenRepository.Add(studentPushToken);
            if (result > 0)
            {
                NewStudentPushToken = await _studentUnit.StudentPushTokenRepository.Get(result);
            }
            return NewStudentPushToken;
        }


        public async Task<Student> GetGuestProfile(string Phone)
        {
            try
            {
                Student student = await _studentUnit.StudentRepository.GetOneByQuery("select * from Student where Phone='" + Phone + "'");
                if (student != null)
                {
                    student.Sections = await _studentUnit.SectionRepository.GetSectionsByCountryId(student.CountryId);
                    var country = await _studentUnit.CountryRepository.Get(student.CountryId);
                    student.Currency = country.Currency;
                    student.CurrencyLT = country.CurrencyLT;

                }
                return student;

            }
            catch (Exception e)
            {

                throw;
            }
        }

        #endregion


        #region Logs Module
        public async Task<long> AddUserDeviceLogAsync(UserDeviceLog userDeviceLog)
        {
            var d = await _studentUnit.UserDeviceLogRepository.Add(userDeviceLog);
            return d;
        }


        public async Task<long> AddDisableReason(DisableReason disableReason)
        {
            var d = await _studentUnit.DisableReasonRepository.Add(disableReason);
            return d;
        }

        public async Task<long> AddSecurityLog(SecurityLog securityLog)
        {
            var d = await _studentUnit.SecurityLogRepository.Add(securityLog);
            return d;
        }

        #endregion
        #region  Section Module

        public async Task<IEnumerable<Section>> GetSectionsByCountryId(long CountryId)
        {
            var result = await _studentUnit.SectionRepository.GetSectionsByCountryId(CountryId);
            return result;
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithSubCategoriesBySectionId(long SectionId, int Page)
        {
            var result = await _studentUnit.CategoryRepository.GetCategoriesWithSubCategoriesBySectionId(SectionId, Page);
            return result;
        }

        public async Task<IEnumerable<SubCategory>> GetSubCategoriesByCategoryId(long CategoryId, int Page)
        {
            var result = await _studentUnit.SubCategoryRepository.GetSubCategoriesByCategoryId(CategoryId, Page);
            return result;

        }

        public async Task<IEnumerable<Department>> GetDepartmentsBySubCategoryId(long SubCategoryId, int Page)
        {
            var result = await _studentUnit.DepartmentRepository.GetDepartmentsBySubCategoryId(SubCategoryId, Page);
            return result;

        }

        public async Task<IEnumerable<Department>> AddDepartmentToStudent(List<long> DepartmentIds, long StudentId)
        {
            IEnumerable<Department> departments = null;
            var result = await _studentUnit.StudentDepartmentRepository.AddDepartmentToStudent(DepartmentIds, StudentId);
            if (result)
            {
                departments = await _studentUnit.DepartmentRepository.GetDepartmentsByStudentId(StudentId);
            }

            return departments;

        }


        public async Task<IEnumerable<Course>> GetCoursesByDepartmentId(long DepartmentId, int Page)
        {
            var result = await _studentUnit.CourseRepository.GetCoursesByDepartmentId(DepartmentId, Page);
            return result;
        }

        public async Task<IEnumerable<Subject>> GetSubjectsWithTracksByDepartmentId(long StudentId,long DepartmentId, int Page)
        {
            var result = await _studentUnit.TrackRepository.GetSubjectsWithTracksByDepartmentId(StudentId,DepartmentId, Page);
            return result;
        }
        #endregion


        #region Home

        public async Task<IEnumerable<StudentHomeCourse>> GetHomeCoursesByStudentId(long StudentId,long CountryId,long Page)
        {
            IEnumerable<StudentHomeCourse> result = null;
            //var InterestsData = await _studentUnit.DepartmentRepository.GetInterestsList(StudentId);

            result = await _studentUnit.DepartmentRepository.GetHomeCoursesByStudentInterests(StudentId, CountryId, Page);
            return result;
        }

        public async Task<IEnumerable<Category>> GetInterestsBeforeEdit(long StudentId)
        {
            IEnumerable<Category> result = null;
            //var InterestsData = await _studentUnit.DepartmentRepository.GetInterestsList(StudentId);

            result = await _studentUnit.DepartmentRepository.GetInterestsBeforeEdit(StudentId);
            return result;

        }
        #endregion

        #region Courses Module 
        public async Task<CoursesByTrackIdModel> GetCoursesByTrackId(long TrackId,string studeIden,long? studentid)
        {
            var result = await _studentUnit.CourseRepository.GetCoursesByTrackId(TrackId, studeIden, studentid);
            return result;
        }

        public async Task<Course> GetCourseById(long CourseId, long StudentId)
        {
            var result = await _studentUnit.CourseRepository.GetCoursesById(CourseId, StudentId);
            if (result != null)
            {
                var tags = await _studentUnit.CourseDepartmentRepository.GetCoursesTags(CourseId);
                if (tags.Count()>0)
                {
                    result.CourseTags = tags;
                }
            }
            return result;
        }
     

        public async Task<long> AddLog(LogError logError)
        {
            var add = await _studentUnit.LogErrorRepository.Add(logError);
            return add;
        }

        public async Task<IEnumerable<MyCourse>> GetMyCourses(long StudentId, int Page)
        {
            var result = await _studentUnit.StudentCourseRepository.GetMyCourses(StudentId, Page);
            return result;
        }
        public async Task<IEnumerable<MyCourse>> GetAvailableCoursesToDownload(long StudentId, int Page)
        {
            var result = await _studentUnit.StudentCourseRepository.GetAvailableCoursesToDownload(StudentId, Page);
            return result;
        }
        
        public async Task<IEnumerable<MyCourse>> GetDownloadedCourses(long StudentId, int Page)
        {
            var result = await _studentUnit.StudentCourseRepository.GetDownloadedCourses(StudentId, Page);
            return result;
        }
        public async Task<int> RemoveCourseFromDownloadById(long courseId)
        {
            var result = await _studentUnit.StudentCourseRepository.RemoveCourseFromDownloadById(courseId);
            return result;
        }
        
        public async Task SendSmsToStudentForRenewSubscription()
        {
            var Students = await _studentUnit.StudentCourseRepository.GetAllStudentForRenewSubscription();
            foreach (var student in Students)
            {
                await SendRenewableMessage(student);
            }
        }
        public async Task<List<Group>> GetGroupsWithContentsByCourseId(long CourseId, long StudentId, int Page,long ContentId)
        {
            var result = await _studentUnit.CourseRepository.GetGroupsWithContentsByCourseIdForStudent(CourseId, StudentId, Page, ContentId);
            return result;
        }


        public async Task<bool> ViewThisContent(long ContentId, long StudentId)
        {
            var result = await _studentUnit.StudentContentRepository.ViewThisContent(ContentId, StudentId);
            return result;
        }
        public async Task<IEnumerable<CourseContentLinks>> GetCourseContentLinks(long courseId)
        {
            var result = await _studentUnit.StudentCourseRepository.GetCourseContentLinks(courseId);
            return result;
        }

        
        public async Task<bool> AddFavourite(Favourite favourite)
        {
            var IsFound = await _studentUnit.FavouriteRepository.IsAdded(favourite);
            if (IsFound)
            {
                return true;
            }
            var id = await _studentUnit.FavouriteRepository.Add(favourite);
            if (id > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteFavourite(Favourite favourite)
        {
            var getFavourit = await _studentUnit.FavouriteRepository.GetFavourite(favourite);
            if (getFavourit == null)
            {
                return true;
            }
            _studentUnit.FavouriteRepository.Delete(getFavourit);
            var IsFound = await _studentUnit.FavouriteRepository.IsAdded(favourite);
            if (IsFound)
            {
                return false;
            }
            return true;
        }

        public async Task<IEnumerable<Course>> GetAllFavourite(long StudentId, int Page)
        {
            var result = await _studentUnit.FavouriteRepository.GetAllFavourite(StudentId, Page);
            return result;
        }
        public async Task<IEnumerable<OfflinePackage>> GetStudentPackages(long StudentId, int Page)
        {
            var packages = new List<OfflinePackage>();
            var result = await _studentUnit.StudentPackageRepository.GetAll(" Where StudentId="+StudentId+"");
            foreach (var item in result)
            {
                var resultData = await _studentUnit.OfflinePackageRepository.GetOneByQuery("Select * from OfflinePackage Where Id=" + item.PackageId+ "");
                if (resultData.PackageEndDate > DateTime.Now)
                {
                    packages.Add(resultData);
                }
            }
            return packages;
        }
        public async Task<IEnumerable<OfflinePackage>> GetAllPackages()
        {
            var result = await _studentUnit.OfflinePackageRepository.GetAll();
            return result;
        }
        public async Task NotifyServerByDownloadedCourse(long StudentId, long courseId,bool isAdd)
        {
            var obj = new StudentCourseDownloaded() { StudentId = StudentId, CourseId = courseId, EnrollmentDate = DateTime.Now };
            if (isAdd == true)
            {
                await _studentUnit.StudentCourseDownloadedRepository.Add(obj);
            }
            else
            {
                _studentUnit.StudentCourseDownloadedRepository.DeleteWhere(" Where StudentId=" + StudentId + " and CourseId = " + courseId + "");
            }
        }
        public async Task NotifyServerByStudentCourseContent(long StudentId, long courseId, long contentId,bool isAdd)
        {
            var obj = new StudentCourseContent() { StudentId = StudentId, CourseId = courseId, ContentId = contentId };
            if (isAdd == true)
            {
                await _studentUnit.StudentCourseContentRepository.Add(obj);
            }
            else
            {
                 _studentUnit.StudentCourseContentRepository.DeleteWhere(" Where StudentId=" + StudentId + " and CourseId = "+courseId+" and ContentId="+contentId+"");
            }
        }
        public async Task<IEnumerable<long>> GetDownloadedContentByStudentCourse(long StudentId, long courseId)
        {
            var resultGroup = await _studentUnit.StudentCourseContentRepository.GetAll(" Where CourseId=" + courseId + " and StudentId="+StudentId+"");
            var xx = resultGroup.ToList().Select(x => x.ContentId);
            return xx;
        }
        
        public async Task NotifyServerBySubscribedPackage(long StudentId, long packageId, int Page)
        {
            var obj = new StudentPackage() { StudentId = StudentId, PackageId = packageId, EnrollmentDate = DateTime.Now };
            await _studentUnit.StudentPackageRepository.Add(obj);
        }
        public async Task<long> GetGroupByCourse(long courseId)
        {
            var resultGroup = await _studentUnit.GroupRepository.GetWhere(" Where CourseId=" + courseId + "");
            return resultGroup.Id;
        }
        public async Task<List<Content>> GetContentByGroup(long groupId)
        {
            var resultContent = await _studentUnit.ContentRepository.GetAll(" Where GroupId=" + groupId + "");
            return resultContent.ToList();
        }


        #endregion

        #region Search
        public async Task<IEnumerable<Course>> Serach(string word, long StudentId,long studentCountry, int Page)
        {
            string newWord = word.Trim();
            SearchWord searchWord = new SearchWord
            {
                StudentId = StudentId,
                Word = newWord
            };
            var IsFound = await _studentUnit.SearchWordRepository.GetSearchWord(StudentId, word);
            if (IsFound == null)
            {
                await _studentUnit.SearchWordRepository.Add(searchWord);

            }
            var result = await _studentUnit.CourseRepository.SearchAsync(newWord, studentCountry, Page);
            return result;

        }

        public async Task<IEnumerable<SearchWord>> GetSavedWords(long StudentId)
        {
            var result = await _studentUnit.SearchWordRepository.GetSearchWords(StudentId);
            return result;
        }

        public async Task<bool> DeleteWord(long StudentId, string word)
        {
            var data = await _studentUnit.SearchWordRepository.GetSearchWord(StudentId, word);
            if (data!=null)
            {
                _studentUnit.SearchWordRepository.Delete(data);
            }
            return true;
            // return false;

        }
        #endregion

        #region Notification Module
        public async Task<IEnumerable<StudentNotification>> GetAllStudentNotification(long StudentId, int Page)
        {
            var result = await _studentUnit.StudentNotificationRepository.GetAllStudentNotification(StudentId, Page);
            return result;
        }

        public async Task<int> GetStudentNotificationNotSeenCount(long StudentId)
        {
            var result = await _studentUnit.StudentNotificationRepository.GetStudentNotificationNotSeenCount(StudentId);
            return result;
        }
        public async Task<bool> SeenStudentNotification(long StudentId,long NotificationId)
        {
            var OldNotification =( await _studentUnit.StudentNotificationRepository.GetAll(" where StudentId=" + StudentId + " and Id=" + NotificationId + "")).FirstOrDefault();
            if (OldNotification==null)
            {
                return false;
            }
            OldNotification.Seen = true;
            var d =await _studentUnit.StudentNotificationRepository.Update(OldNotification);
            return d;
        }
        #endregion

        #region Contact Us Module 
        public async Task<ContactUs> AddContact(ContactUs contactUs)
        {
            ContactUs NewContactUs = null;
            var id = await _studentUnit.ContactUsRepository.Add(contactUs);
            if (id > 0)
            {
                NewContactUs = await _studentUnit.ContactUsRepository.Get(id);
            }
            return NewContactUs;
        }

        public async Task<Student> UpdateProfile(Student studentData)
        {
            var result = await _studentUnit.StudentRepository.Update(studentData);
            if (result)
            {
                var data = await GetStudentProfile(studentData.Id);
                return data;
            }
            return null;
        }

        public async Task<bool?> DeletePushTokens(long StudentId)
        {
            var result=  await  _studentUnit.StudentPushTokenRepository.DeleteAllTokensAsync(StudentId);
            return result;
        }

        public async Task<Track> GetTrackById(long trackId,long StudentId)
        {
            var track = await _studentUnit.TrackRepository.Get(trackId);
            if (track!=null)
            {
               var IsSubscriped=await _studentUnit.TrackSubscriptionRepository.CheckIfStudentSubscripeThistrackBefore(trackId, StudentId);
                if (IsSubscriped != null)
                {
                    track.IsSubscriped = true;
                }
                else
                {
                    track.IsSubscriped = false;
                }
            }
            return track;
        }
        public async Task<IEnumerable<TrackPromotionDetail>> GetPromotionsByTrackId(long trackId,long studentId)
        {
            var result = await _studentUnit.TrackPromotionRepository.GetPromotionDetails(trackId, studentId);
            return result;
        }
        
        public async Task<Student> GetStudentByPaymentKey(string PaymentKey)
        {
            var student = await _studentUnit.StudentRepository.GetWhere(" where PaymentKey='" + PaymentKey + "' ");
            return student;
        }

        public async Task<Course> GetCourseWithOneContentForStudent(long CourseId, long contentId, long StudentId,long VideoQuestionId)
        {
            var CourseData = await _studentUnit.CourseRepository.GetCoursesWithContentById(CourseId, StudentId, contentId);
            if (CourseData != null)
            {
                var tags = await _studentUnit.CourseDepartmentRepository.GetCoursesTags(CourseId);
                if (tags.Count() > 0)
                {
                    CourseData.CourseTags = tags;
                }
                CourseData.VideoQuestions = await _videoQuestionUnit.VideoQuestionRepository.GetQuestionsForStudent(contentId, StudentId,VideoQuestionId);
                
            }

            return CourseData;

        }

        public async Task<List<string>> GetCountriesCodes()
        {
            List<string> countryCodes = new List<string>();
            var Countries = await _studentUnit.CountryRepository.GetAll();
            foreach (var item in Countries)
            {
                countryCodes.Add(item.CountryCode);

            }
            return countryCodes;
        }

        #endregion

        #region Live Module
        public async Task<IEnumerable<StudentLiveHome>> GetTopLives(long countryId, long studentId)
        {
            var result = await _studentUnit.LiveRepository.GetTopLives(countryId, studentId);
            var groupedLives = result.GroupBy(a => new 
            { 
                a.CategoryId, 
                a.CategoryName, 
                a.CategoryNameLT , 
                a.SubCategoryId, 
                a.SubCategoryName,
                a.SubCategoryNameLT
            }).Select(a => new StudentLiveHome()
            {
                SubCategoryId = a.Key.SubCategoryId,
                SubCategoryName = a.Key.SubCategoryName,
                SubCategoryNameLT = a.Key.SubCategoryNameLT,
                CategoryId = a.Key.CategoryId,
                CategoryName = a.Key.CategoryName,
                CategoryNameLT = a.Key.CategoryNameLT,
                Lives = a.Select(live => new LiveDTO()
                {
                    Id = live.LiveId,
                    LiveName = live.LiveName,
                    TeacherName = live.TeacherName,
                    Duration = live.Duration,
                    CoverImage = "http://tollab.azurewebsites.net/ws/Images/LiveImages/" + live.Image,
                    CurrentCost = live.CurrentPrice,
                    PreviousCost = live.OldPrice,
                    VideoURI = live.VideoURI,
                    VideoURL = live.VideoURL,
                    MeetingPassword = (live.Enrollment != null && live.Enrollment == 1) ? live.MeetingPassword : null,
                    HostURL = (live.Enrollment != null && live.Enrollment == 1) ? live.HostURL : null,
                    JoinURL = (live.Enrollment != null && live.Enrollment == 1) ? live.JoinURL : null,
                    MeetingId = (live.Enrollment != null && live.Enrollment == 1) ? live.ZoomMeetingId : null,
                    SubscriptionCount = live.SubscriptionCount,
                    MeetingDate = live.LiveDate,
                    CountryId = live.CountryId,
                    Enrollment = live.Enrollment,
                    SKUNumber = live.SKUNumber,
                    SKUPrice = live.CurrentSKUPrice,
                    OldSKUPrice = live.OldSKUPrice
                })
            });
            return groupedLives;
        }

        public async Task<IEnumerable<StudentLiveHome>> GetLives(long countryId, long studentId, int page=0)
        {
            var result = await _studentUnit.LiveRepository.GetLives(countryId , studentId, page);
            var groupedLives = result.GroupBy(a => new
            {
                a.CategoryId,
                a.CategoryName,
                a.CategoryNameLT,
                a.SubCategoryId,
                a.SubCategoryName,
                a.SubCategoryNameLT
            }).Select(a => new StudentLiveHome()
            {
                SubCategoryId = a.Key.SubCategoryId,
                SubCategoryName = a.Key.SubCategoryName,
                SubCategoryNameLT = a.Key.SubCategoryNameLT,
                CategoryId = a.Key.CategoryId,
                CategoryName = a.Key.CategoryName,
                CategoryNameLT = a.Key.CategoryNameLT,
                Lives = a.Select(live => new LiveDTO()
                {
                    Id = live.LiveId,
                    LiveName = live.LiveName,
                    TeacherName = live.TeacherName,
                    Duration = live.Duration,
                    CoverImage = "http://tollab.azurewebsites.net/ws/Images/LiveImages/" + live.Image,
                    CurrentCost = live.CurrentPrice,
                    PreviousCost = live.OldPrice,
                    VideoURI = live.VideoURI,
                    VideoURL = live.VideoURL,
                    MeetingPassword = (live.Enrollment != null && live.Enrollment == 1) ? live.MeetingPassword : null,
                    HostURL = (live.Enrollment != null && live.Enrollment == 1) ? live.HostURL : null,
                    JoinURL = (live.Enrollment != null && live.Enrollment == 1) ? live.JoinURL : null,
                    MeetingId = (live.Enrollment != null && live.Enrollment == 1) ? live.ZoomMeetingId : null,
                    SubscriptionCount = live.SubscriptionCount,
                    MeetingDate = live.LiveDate,
                    CountryId = live.CountryId,
                    Enrollment = live.Enrollment,
                    SKUNumber = live.SKUNumber,
                    SKUPrice = live.CurrentSKUPrice,
                    OldSKUPrice = live.OldSKUPrice
                })
            });
            return groupedLives;
        }

        

        public async Task<LiveDetails> GetLive(long id, long studentId)
        {
            var result = await _studentUnit.LiveRepository.GetLive(id, studentId);
            if (result == null)
                return null;

            var groupedLives = new LiveDetails()
            {
                Id = result.LiveId,
                LiveName = result.LiveName,
                TeacherName = result.TeacherName,
                Duration = result.Duration,
                CoverImage = "http://tollab.azurewebsites.net/ws/Images/LiveImages/" + result.Image,
                CurrentCost = result.CurrentPrice,
                PreviousCost = result.OldPrice,
                VideoURI = result.VideoURI,
                VideoURL = result.VideoURL,
                MeetingPassword = (result.Enrollment != null && result.Enrollment == 1) ? result.MeetingPassword : null,
                HostURL = (result.Enrollment != null && result.Enrollment == 1) ? result.HostURL : null,
                JoinURL = (result.Enrollment != null && result.Enrollment == 1) ? result.JoinURL : null,
                MeetingId = (result.Enrollment != null && result.Enrollment == 1) ? result.ZoomMeetingId : null,
                SubscriptionCount = result.SubscriptionCount,
                MeetingDate = result.LiveDate,
                CountryId = result.CountryId,
                Enrollment = result.Enrollment,
                Attachments = result.LiveAttachments.Select(att => new LiveAttachment()
                {
                    Id = att.Id,
                    Name = att.Name,
                    LiveId = att.LiveId,
                    OrderNumber = att.OrderNumber,
                    Path = "http://tollab.azurewebsites.net/ws/CourseVideos/" + att.Path
                }),
                SKUNumber = result.SKUNumber,
                SKUPrice = result.CurrentSKUPrice,
                OldSKUPrice = result.OldSKUPrice
            };
            return groupedLives;
        }
        #endregion

        public async Task AddTokenStoreAsync(TokenStore tokenStore)
        {
            var update = await _studentUnit.TokenStoreRepository.InvalidateOldTokens(tokenStore.StudentId);
            var add = await _studentUnit.TokenStoreRepository.Add(tokenStore);

        }
        public async Task<TokenStore> GetTokenStoreAsync(string Token)
        {
            var tokenStore = (await _studentUnit.TokenStoreRepository.GetAll(" where Token='" + Token + "'")).FirstOrDefault() ;
            return tokenStore;
        }
    }
}
