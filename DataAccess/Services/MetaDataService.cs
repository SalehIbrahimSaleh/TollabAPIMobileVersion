using DataAccess.Entities;
using DataAccess.Repositories;
using DataAccess.Services;
using DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class MetaDataService
    {
        StudentUnit _studentUnit;
        public MetaDataService()
        {
            _studentUnit = new StudentUnit();
        }

        public async Task<MetaData> GetMetaData()
        {
            UnitOfWork.MetaDataUnit unitOfWork = new UnitOfWork.MetaDataUnit();

            MetaData metaData = new MetaData();
            var countries = await unitOfWork.CountryRepository.GetAll("");
            var paymentMethods = await unitOfWork.PaymentMethodRepository.GetAll("");
            var termAndConditions = await unitOfWork.TermAndConditionRepository.GetAll("");
            var references = await unitOfWork.ReferenceRepository.GetAll("");
            var courseStatuses = await unitOfWork.CourseStatusRepository.GetAll("");
           var aboutUs = await unitOfWork.AboutUsRepository.GetAll("");
            var settings = await unitOfWork.SettingRepository.GetAll("");
            var systemSettings = await unitOfWork.SystemSettingRepository.GetAll("");
            var examQuestionTypes = await unitOfWork.ExamQuestionTypeRepository.GetAll("");
            var examTypes = await unitOfWork.ExamTypeRepository.GetAll("");
            var solveStatuses = await unitOfWork.SolveStatusRepository.GetAll("");


            metaData.Countries = countries.ToList<Country>();
            metaData.PaymentMethods = paymentMethods.ToList<PaymentMethod>();
            metaData.TermAndConditions = termAndConditions.ToList<TermAndCondition>();
            metaData.References = references.ToList<Reference>();
            metaData.CourseStatuses = courseStatuses.ToList<CourseStatus>();
            metaData.AboutUs = aboutUs.ToList<AboutUs>();
            metaData.Settings = settings.ToList<Setting>();
            metaData.SystemSettings = systemSettings.ToList<SystemSetting>();
            metaData.ExamQuestionTypes = examQuestionTypes.ToList();
            metaData.ExamTypes = examTypes.ToList();
            metaData.SolveStatuses = solveStatuses.ToList();
            return metaData;

        }
        public async Task<long> SavePercentagePerVideoContent(long studentId,long videoId, long courseId, long trackId, double percentage, int secondsCount)
        {
            TeacherUnit _studentUnit = new TeacherUnit();
            var contentObj= await _studentUnit.ContentRepository.Get(videoId);
            
            var existObj = await _studentUnit.ContentCourseTrackUserRepository.GetWhere(" Where ContentId=" + videoId  + " And UserId=" + studentId + " ");
            long calculatedPercentage = 0;
            if(existObj != null)
            {
                var obj = existObj;
                obj.SecoundsCount = existObj.SecoundsCount+ secondsCount;
                if (percentage > existObj.Percentage)
                {
                    
                    obj.Id = existObj.Id;
                    obj.ContentId = videoId;
                    obj.CourseId = courseId;
                    obj.UserId = studentId;
                    obj.TrackId = trackId;
                    obj.Percentage = percentage;
                    obj.MinuteCount= Convert.ToInt32((contentObj.Duration * obj.Percentage)/100);
                 
                    calculatedPercentage = Convert.ToInt64( obj.Percentage);
                   await _studentUnit.ContentCourseTrackUserRepository.Update(obj);
                }
            }
            else
            {
                    var obj = new ContentCourseTrackUser();
                    obj.ContentId = videoId;
                    obj.CourseId = courseId;
                    obj.UserId = studentId;
                    obj.TrackId = trackId;
                    obj.Percentage = percentage;
                obj.SecoundsCount = secondsCount;
                obj.MinuteCount = Convert.ToInt32((contentObj.Duration * obj.Percentage) / 100);
                calculatedPercentage = Convert.ToInt64(obj.Percentage);
                await _studentUnit.ContentCourseTrackUserRepository.Add(obj);
            }
            
            return calculatedPercentage;
        }
        public async Task<MetaData> GetMetaDataIOS()
        {
            UnitOfWork.MetaDataUnit unitOfWork = new UnitOfWork.MetaDataUnit();

            MetaData metaData = new MetaData();
            var countries = await unitOfWork.CountryRepository.GetAll("");
            var paymentMethods = await unitOfWork.PaymentMethodRepository.GetAll("");
            var termAndConditions = await unitOfWork.TermAndConditionRepository.GetAll("");
            var references = await unitOfWork.ReferenceRepository.GetAll("");
            var courseStatuses = await unitOfWork.CourseStatusRepository.GetAll("");
            var aboutUs = await unitOfWork.AboutUsRepository.GetAll("");
            var settings = await unitOfWork.SettingRepository.GetAll("where Id != 4 and Id !=5 and Id !=6 and Id !=3");
            var systemSettings = await unitOfWork.SystemSettingRepository.GetAll("where Id !=1 and Id !=2 and Id !=3");
            var examQuestionTypes = await unitOfWork.ExamQuestionTypeRepository.GetAll("");
            var examTypes = await unitOfWork.ExamTypeRepository.GetAll("");
            var solveStatuses = await unitOfWork.SolveStatusRepository.GetAll("");


            metaData.Countries = countries.ToList<Country>();
            metaData.PaymentMethods = paymentMethods.ToList<PaymentMethod>();
            metaData.TermAndConditions = termAndConditions.ToList<TermAndCondition>();
            metaData.References = references.ToList<Reference>();
            metaData.CourseStatuses = courseStatuses.ToList<CourseStatus>();
            metaData.AboutUs = aboutUs.ToList<AboutUs>();
            metaData.Settings = settings.ToList<Setting>();
            metaData.SystemSettings = systemSettings.ToList<SystemSetting>();
            metaData.ExamQuestionTypes = examQuestionTypes.ToList();
            metaData.ExamTypes = examTypes.ToList();
            metaData.SolveStatuses = solveStatuses.ToList();
            return metaData;

        }

        public async Task<List<ContentIdPath>> GetContentsByCourseId(long courseId)
        {
           var lst= await _studentUnit.FavouriteRepository.GetContentsByCourseId(courseId);
            return lst.ToList();
        }
    }
}
