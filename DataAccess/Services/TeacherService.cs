using DataAccess.DTOs;
using DataAccess.Entities;
using DataAccess.UnitOfWork;
using DataAccess.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class TeacherService
    {
        TeacherUnit _teacherUnit;
        StudentUnit  _studentUnit;
        VideoQuestionUnit _videoQuestionUnit;
        public TeacherService()
        {
            _studentUnit = new StudentUnit();
            _teacherUnit = new TeacherUnit();
            _videoQuestionUnit = new VideoQuestionUnit();
        }

        public async Task<Teacher> GetTeacherById(long TeacherId,long CountryId=0)
        {
            try
            {
                var result = await _teacherUnit.TeacherRepository.Get(TeacherId);
                if (result != null)
                {
                    var CourseCount = await _teacherUnit.TeacherRepository.GetCourseCount(TeacherId,CountryId);
                    result.CourseCount = CourseCount;
                    var StudentCount = await _teacherUnit.TeacherRepository.GetStudentSubscriptionsCount(TeacherId, CountryId);
                    result.StudentCount = StudentCount;

                     var teacherCourses = await _teacherUnit.TeacherRepository.GetCoursesByTeacherId(TeacherId);
                    result.TrackWithCourses = teacherCourses;
                    var country = await _teacherUnit.CountryRepository.Get(result.CountryId);
                    result.Currency = country.Currency;
                    result.CurrencyLT = country.CurrencyLT;
                    var Countries = await _teacherUnit.CountryRepository.GetAll();
                    result.Countries = Countries;


                }
                return result;

            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public async Task<Teacher> GetTeacherByIdInCountry(long TeacherId,long CountryId=0)
        {
            try
            {
                var result = await _teacherUnit.TeacherRepository.Get(TeacherId);
                if (result != null)
                {
                    var CourseCount = await _teacherUnit.TeacherRepository.GetCourseCountInCountry(TeacherId, CountryId);
                    result.CourseCount = CourseCount;
                    var StudentCount = await _teacherUnit.TeacherRepository.GetStudentSubscriptionsCountInCountry(TeacherId, CountryId);
                    result.StudentCount = StudentCount;
                    var StudentSubscriptionsCount = await _teacherUnit.TeacherRepository.GetStudentSubscriptionsCount(TeacherId,CountryId);
                    result.StudentSubscriptionsCount = StudentSubscriptionsCount;

                    var teacherCourses = await _teacherUnit.TeacherRepository.GetCoursesByTeacherIdForCountry(TeacherId,CountryId);
                    result.TrackWithCourses = teacherCourses;
                    var country = await _teacherUnit.CountryRepository.Get(result.CountryId);
                    result.Currency = country.Currency;
                    result.CurrencyLT = country.CurrencyLT;
                    var Countries = await _teacherUnit.CountryRepository.GetAll();
                    result.Countries = Countries;


                }
                return result;

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        

        public async Task<Teacher>EditProfile(Teacher teacher)
        {
            try
            {
                var Updated = await _teacherUnit.TeacherRepository.Update(teacher);
                if (Updated)
                {
                    var result = await GetTeacherById(teacher.Id);
                    return result;

                }
                return null;
            }
            catch (Exception e)
            {

                throw e;
            }
         
        }

        public async Task<IEnumerable<Course>> GetCoursesBySubjectId(long TeacherId, long SubjectId, int Page)
        {
            var result = await _teacherUnit.TeacherRepository.GetCoursesBySubjectId(TeacherId, SubjectId, Page);
            return result;

        }

        public async Task<Teacher> GetTeacherByIdentityId(string IdentityId, long CountryId=0)
        {
            var result = await _teacherUnit.TeacherRepository.GetTeacherByIdentityId(IdentityId);
            if (result != null)
            {
                var CourseCount = await _teacherUnit.TeacherRepository.GetCourseCount(result.Id,CountryId);
                result.CourseCount = CourseCount;
                var teacherCourses = await _teacherUnit.TeacherRepository.GetCoursesByTeacherId(result.Id);
                result.TrackWithCourses = teacherCourses;
                var StudentSubscriptionsCount = await _teacherUnit.TeacherRepository.GetStudentSubscriptionsCount(result.Id,CountryId);
                result.StudentSubscriptionsCount = StudentSubscriptionsCount;
                var StudentCount = await _teacherUnit.TeacherRepository.GetStudentEnrolledCount(result.Id,CountryId);
                result.StudentCount = StudentCount;
                var country = await _teacherUnit.CountryRepository.Get(result.CountryId);
                if (country!=null)
                {
                    result.Currency = country.Currency;
                    result.CurrencyLT = country.CurrencyLT;

                }
                var Countries = await _teacherUnit.CountryRepository.GetAll();
                result.Countries = Countries;

            }
            return result;
        }

        public async  Task<TeacherAssistant> GetTeacherAssistantByIdentityId(string IdentityId)
        {
            var result = await _teacherUnit.TeacherAssistantRepository.GetTeacherAssistantByIdentityId(IdentityId);
            return result;

        }
        public async Task<IEnumerable<Track>> GetTracks(long TeacherId,long? SubjectId, int Page)
        {

            var result = await _teacherUnit.TrackRepository.GetTracks(TeacherId, SubjectId, Page);
            return result;

        }

        public async Task<IEnumerable<Subject>> GetTeacherSubject(long TeacherId,long CountryId, int page)
        {
            var result = await _teacherUnit.TeacherSubjectRepository.GetTeacherSubject(TeacherId, CountryId, page);
            return result;
        }

        public async Task<Track> AddTrack(Track  track)
        {
            Track NewTrack= null;
            var IsFound = await _teacherUnit.TrackRepository.IsFound(track);
            var subject = await _teacherUnit.SubjectRepository.Get(track.SubjectId);
            if (IsFound==false)
            {
                track.TrackSubject = track.Name + "-"+ subject.Name;
                var id = await _teacherUnit.TrackRepository.Add(track);
                if (id > 0)
                {
                    NewTrack = await _teacherUnit.TrackRepository.Get(id);

                    //add code for track
                    string code = CodeGenerator.getCourseOrTrackCode();
                    var TrackCode = "T" + NewTrack.Id +code ;
                    NewTrack.TrackCode = TrackCode;

                    await _teacherUnit.TrackRepository.Update(NewTrack);
                }
            }
            return NewTrack;

        }
        public async Task<Live> AddLive(CreatingLiveDTO creatingLiveDTO, long teacherId, long countryId, string hostURL, string joinURL, long zoomMeetingId, string meetingPassword)
        {
            Live newLive = new Live()
            {
                CountryId = countryId,
                CourseId = creatingLiveDTO.CourseId,
                CurrentPrice = 0,
                Duration = creatingLiveDTO.Duration,
                HostURL = hostURL,
                JoinURL = joinURL,
                LiveAppearanceDate = creatingLiveDTO.LiveAppearanceDate,
                LiveDate = creatingLiveDTO.LiveDate,
                LiveLinkType = creatingLiveDTO.LiveLinkType,
                LiveName = creatingLiveDTO.LiveName,
                MeetingPassword = meetingPassword,
                TeacherId = teacherId,
                TrackId = creatingLiveDTO.TrackId,
                ZoomMeetingId = zoomMeetingId,
                Image = creatingLiveDTO.Image
            };
            var result = await _teacherUnit.LiveRepository.Add(newLive);
            return newLive;
        }

        public async Task<Live> GetLive(long liveId)
        {
            return await _teacherUnit.LiveRepository.Get(liveId);
        }
        public async Task<Live> UpdateLive(Live dbLive, UpdatingLiveDTO updatingLiveDTO)
        {
            dbLive.LiveName = updatingLiveDTO.LiveName;
            dbLive.Duration = updatingLiveDTO.Duration;
            dbLive.LiveAppearanceDate = updatingLiveDTO.LiveAppearanceDate;
            dbLive.LiveDate = updatingLiveDTO.LiveDate;
            dbLive.LiveLinkType = updatingLiveDTO.LiveLinkType;
            dbLive.CourseId = updatingLiveDTO.CourseId;
            dbLive.TrackId = updatingLiveDTO.TrackId;
            dbLive.CountryId = updatingLiveDTO.CountryId;
            dbLive.Image = updatingLiveDTO.Image;
            var result = await _teacherUnit.LiveRepository.Update(dbLive);
            return dbLive;
        }

        public async Task<LiveAttachment> AddLiveAttachment(LiveAttachment liveAttachment)
        {
            await _teacherUnit.LiveAttachmentRepository.Add(liveAttachment);
            return liveAttachment;
        }

        public async Task<bool> UpdateLiveAttachment(UpdatingLiveAttachmentDTO updatingLiveAttachment)
        {
            var dbLiveAttchemnt = await _teacherUnit.LiveAttachmentRepository.Get(updatingLiveAttachment.Id);
            dbLiveAttchemnt.Name = updatingLiveAttachment.Name;
            return await _teacherUnit.LiveAttachmentRepository.Update(dbLiveAttchemnt);
        }

        public async Task DeleteLiveAttachment(long liveAttachmentId)
        {
            var liveAttachment = await _teacherUnit.LiveAttachmentRepository.Get(liveAttachmentId);
            if (liveAttachment != null)
            {
                _teacherUnit.LiveAttachmentRepository.Delete(liveAttachment);
            }
        }
        public async Task<TeacherLiveDetails> GetLiveDetails(long liveId, long teacherId)
        {
            var result = await _studentUnit.LiveRepository.GetLiveForTeacher(liveId, teacherId);
            if (result == null)
                return null;

            var teacherLive = new TeacherLiveDetails()
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
                Attachments = result.LiveAttachments.Select(att => new LiveAttachment()
                {
                    Id = att.Id,
                    Name = att.Name,
                    LiveId = att.LiveId,
                    OrderNumber = att.OrderNumber,
                    Path = att.Path
                }),
                SKUNumber = result.SKUNumber,
                SKUPrice = result.CurrentSKUPrice,
                OldSKUPrice = result.OldSKUPrice
            };
            return teacherLive;
        }

        public async Task<IEnumerable<StudentLiveHome>> GetLives(long countryId, long teacherID, int page = 0)
        {
            var result = await _studentUnit.LiveRepository.GetLivesForTeacher(countryId, teacherID, page);
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
                    CoverImage = "http://tollab.azurewebsites.net//ws/Images/LiveImages/" + live.Image,
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
                    SKUNumber = live.SKUNumber,
                    SKUPrice = live.CurrentSKUPrice,
                    OldSKUPrice = live.OldSKUPrice
                })
            });
            return groupedLives;
        }

        public async Task DeleteLive(long liveId)
        {
            var live = await _teacherUnit.LiveRepository.Get(liveId);
            _teacherUnit.LiveRepository.Delete(live);
        }

        public async Task<IEnumerable<TeacherHomeCourse>> GetTeacherHomeCourses(long TeacherId,long CountryId, int Page)
        {
            var result = await _teacherUnit.TeacherRepository.GetTeacherHomeCourses(TeacherId, CountryId, Page);
            return result;
        }

        public async Task<TeacherTransactionVM> GetTeacherTransactions(long TeacherId, int Page,long CountryId)
        {
            var result = await _teacherUnit.TeacherTransactionRepository.GetTransactions(TeacherId, Page, CountryId);
            return result;
        }

        public async Task<IEnumerable<TeacherNotification>> GetAllTeacherNotification(long TeacherId, int Page)
        {
            var result = await _teacherUnit.TeacherNotificationRepository.GetAllTeacherNotification(TeacherId, Page);
            return result;
        }

        public async Task<bool> SeenTeacherNotification(long TeacherId,long NotificationId)
        {
            var OldNotification = (await _teacherUnit.TeacherNotificationRepository.GetAll(" where TeacherId=" + TeacherId + " and Id=" + NotificationId + "")).FirstOrDefault();
            if (OldNotification == null)
            {
                return false;
            }
            OldNotification.Seen = true;
            var d = await _teacherUnit.TeacherNotificationRepository.Update(OldNotification);
            return d;
        }
        public async Task<int> GetTeacherNotificationNotSeenCount(long TeacherId)
        {
            var result = await _teacherUnit.TeacherNotificationRepository.GetTeacherNotificationNotSeenCount(TeacherId);
            return result;
        }
       

        public async Task<TeacherPushToken> AddTeacherPushToken(TeacherPushToken teacherPushToken )
        {
            TeacherPushToken NewTeacherPushToken = null;
            var tokens = await _teacherUnit.TeacherPushTokenRepository.GetAll(" where TeacherId =" + teacherPushToken.TeacherId + " ");
            if (tokens.Count() > 0)
            {
                foreach (var token in tokens)
                {
                    if (token.Token == teacherPushToken.Token && token.OS == teacherPushToken.OS)
                    {
                        await _teacherUnit.TeacherPushTokenRepository.Update(token);
                        return token;
                    }
                    else if (token.Token != teacherPushToken.Token && token.OS == teacherPushToken.OS)
                    {
                        token.Token = teacherPushToken.Token;
                        var d = await _teacherUnit.TeacherPushTokenRepository.Update(token);
                        return token;
                    }

                }
            }
            var result = await _teacherUnit.TeacherPushTokenRepository.Add(teacherPushToken);
            if (result > 0)
            {
                NewTeacherPushToken = await _teacherUnit.TeacherPushTokenRepository.Get(result);
            }
            return NewTeacherPushToken;
        }

        public async Task<bool?> DeleteAllTokens(long TeacherId)
        {
            var result =await _teacherUnit.TeacherPushTokenRepository.DeleteAllTokensAsync(TeacherId);
            return result;
        }
        #region Course Module
        public async Task<bool> UpdateCourseWithAlbumUri(Course course)
        {
                var updated = await _teacherUnit.CourseRepository.Update(course);
                if (updated)
                {
                    return true;
                }
                return false;          
        }


        public async Task<Course> AddOrUpdateCourseTitle(Course course)
        {
            Course NewCourse = null;
            var track = await _teacherUnit.TrackRepository.Get(course.TrackId);
            //Update
            if (course.Id>0)
            {
                var oldCourse = await _teacherUnit.CourseRepository.Get(course.Id);
                oldCourse.Name = course.Name;
                oldCourse.NameLT = course.NameLT;
                oldCourse.CourseTrack = course.Name +"-"+ track.Name;
                var updated = await _teacherUnit.CourseRepository.Update(oldCourse);
                if (updated)
                {
                    return oldCourse;
                }
                return NewCourse;

            }
            //add
            course.CreationDate = DateTime.UtcNow;
            course.CourseStatusId = 1;
            course.CourseTrack = course.Name + track.Name;
            var id = await _teacherUnit.CourseRepository.Add(course);
            if (id > 0)
            {
                NewCourse = await _teacherUnit.CourseRepository.Get(id);

                //add code to course
                var code = CodeGenerator.getCourseOrTrackCode();
                var CourseCode = "C" + NewCourse.Id + code;
                NewCourse.CourseCode = CourseCode;
                await _teacherUnit.CourseRepository.Update(NewCourse);

                var departmentId = await _teacherUnit.CourseDepartmentRepository.GetDepartmentIdByTrackId(course.TrackId.Value);
                CourseDepartment courseDepartment = new CourseDepartment { CourseId = id, DepartmentId = departmentId };
                var  addCoursTag = await _teacherUnit.CourseDepartmentRepository.Add(courseDepartment);
            }
            return NewCourse;
        }


        public async Task<Course> UpdateCourseIntroVideo(Course course)
        {

                var updated = await _teacherUnit.CourseRepository.Update(course);
                if (updated)
                {
                    return course;
                }
                return null;

        }
        

        public async Task<Course> AddOrUpdateCoursePrice(Course course)
        {
            Course NewCourse = null;
            var oldCourse = await _teacherUnit.CourseRepository.Get(course.Id);
            oldCourse.PreviousCost = oldCourse.CurrentCost;
            oldCourse.CurrentCost = course.CurrentCost;

            var updated = await _teacherUnit.CourseRepository.Update(oldCourse);
            if (updated)
            {
                return oldCourse;
            }
            return NewCourse;

            
        }


        public async Task<Course> AddCourseFullDescription(Course course)
        {
            var result = await _teacherUnit.CourseRepository.AddCourseFullDescription(course);
            return result;
        }

        public async Task<Group> AddOrUpdateGroup(Group group)
        {
            try
            {
                Group NewGroup = null;
                var course = await _teacherUnit.CourseRepository.Get(group.CourseId);
                if (group.Id > 0)
                {
                    group.GroupCourse = group.Name + "-" + course.Name;
                    var updated = await _teacherUnit.GroupRepository.Update(group);
                    if (updated)
                    {
                        return group;
                    }
                    return NewGroup;
                }
                group.GroupCourse = group.Name + course.Name;
                var id = await _teacherUnit.GroupRepository.Add(group);
                if (id > 0)
                {
                    NewGroup = await _teacherUnit.GroupRepository.Get(id);
                }
                return NewGroup;
            }
            catch (Exception e)
            {

                throw e;
            }
       
        }

        public async Task<Course> GetCoursesById(long courseId,long TeacherId)
        {
            var result = await _teacherUnit.CourseRepository.GetCourseByIdForTeacher(courseId, TeacherId);
            return result;
        }

        public async Task<Content> AddContent(Content content)
        {
            Content NewContent = null;
            var id = await _teacherUnit.ContentRepository.Add(content);
            if (id > 0)
            {
                NewContent = await _teacherUnit.ContentRepository.Get(id);
                var courseId = (await _teacherUnit.GroupRepository.Get(content.GroupId)).CourseId;
                var StudentTokens = await _studentUnit.StudentPushTokenRepository.GetAllByCourseId(courseId.Value);
                StudentNotification studentNotification = new StudentNotification()
                {
                    NotificationToId = courseId,
                    CourseId = courseId,
                    NotificationTypeId = 2,
                    CreationDate = DateTime.UtcNow,
                    ReferenceId = 1,
                    Title = "New Content Added",
                    TitleLT = "تم اضافة درس جديد"
                };
                try
                {
                    await _studentUnit.StudentNotificationRepository.AddBulkNotification(studentNotification, StudentTokens);
                }
                catch
                {}  
            }
            return NewContent;
        }

        public async Task<Content> GetContent(long contentId)
        {
            var NewContent = await _teacherUnit.ContentRepository.Get(contentId);
         
            return NewContent;
        }

        public async Task<bool> DeleteContent(long? ContentId)
        {
            
            var content = await _teacherUnit.ContentRepository.Get(ContentId);
            if (content!=null)
            {
                try
                {
                    _teacherUnit.ContentRepository.Delete(content);
                    return true;


                }
                catch (Exception e)
                {

                    throw;
                }
            }
            return false;
        }



        public async Task<bool> SubmitToReview(long CourseId)
        {
            var result = await _teacherUnit.CourseRepository.SubmitToReview(CourseId);
            return result;
        }

        public async  Task<Course> setCourseImage(string image, long CourseId)
        {
            try
            {
                var course = await _teacherUnit.CourseRepository.Get(CourseId);
                if (course != null)
                {
                    course.Image = image;
                    var updated = await _teacherUnit.CourseRepository.Update(course);
                    if (updated)
                    {
                        return course;
                    }

                }
                return null;
            }
            catch (Exception e)
            {

                throw e;
            }
         

        }

        public async Task<CoursesByTrackIdModel> GetCoursesByTrackId(long TrackId)
        {
            var result = await _teacherUnit.CourseRepository.GetCoursesByTrackIdForTeacher(TrackId);
            return result;
        }
        public async Task<CoursesByTrackIdModel> GetAllCoursesByTrackId(long TrackId)
        {
            var result = await _teacherUnit.CourseRepository.GetAllCoursesByTrackIdForTeacher(TrackId);
            return result;
        }


        public async Task<string> GetAlbumByGroupId(long? groupId)
        {

            var album = await _teacherUnit.GroupRepository.GetAlbumByGroupId(groupId);
            return album;
        }




        public async Task<List<Group>> GetGroupsWithContentsByCourseId(long CourseId, int Page,long ContentId)
        {
            var result = await _teacherUnit.CourseRepository.GetGroupsWithContentsByCourseId(CourseId, Page, ContentId);
            return result;
        }
        public async Task<Course> GetCourseWithOneContentForTeacher(long CourseId, long ContentId, long TeacherId,long VideoQuestionId)
        {
            var CourseData = await _teacherUnit.CourseRepository.GetCourseByIdForTeacher(CourseId, TeacherId, ContentId);
            
            if (CourseData != null)
            {
                 
                CourseData.VideoQuestions = await _videoQuestionUnit.VideoQuestionRepository.GetQuestionsForTeacher(ContentId, TeacherId, VideoQuestionId);

            }

            return CourseData;

        }

        #endregion
    }
}
