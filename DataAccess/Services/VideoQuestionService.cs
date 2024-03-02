using DataAccess.Entities;
using DataAccess.Repositories;
using DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class VideoQuestionService
    {
        VideoQuestionUnit _videoQuestionUnit ;
        public VideoQuestionService()
        {
            _videoQuestionUnit = new  VideoQuestionUnit();
        }
        public async Task<VideoQuestion> AddQuestion(VideoQuestion videoQuestion)
        {
            try
            {
                var CourseId = await _videoQuestionUnit.GroupRepository.GetCourseIdByContentId(videoQuestion.ContentId);
                var teacherId = await _videoQuestionUnit.TrackRepository.GetTecherIdByCourseId(CourseId);
                var courseData = await _videoQuestionUnit.CourseRepository.Get(CourseId);
                VideoQuestion newVideoQuestion = null;
                var result = await _videoQuestionUnit.VideoQuestionRepository.Add(videoQuestion);
                if (result > 0)
                {
                    TeacherNotification teacherNotification = new TeacherNotification
                    {
                        CreationDate = DateTime.UtcNow,
                        NotificationToId = CourseId,
                        ReferenceId = 2,
                        ContentId = videoQuestion.ContentId,
                        VideoQuestionId= videoQuestion.Id,
                        TeacherId = teacherId,
                        Title = "New qusetion added to course ("+ courseData.Name + ")",
                        TitleLT="تم اضافة سؤال جديد لدورة ("+courseData.NameLT+")"
                    };
                    await _videoQuestionUnit.TeacherNotificationRepository.AddNotification(teacherNotification);

                    newVideoQuestion = await _videoQuestionUnit.VideoQuestionRepository.Get(result);

                }
                return newVideoQuestion;
            }
            catch (Exception e)
            {

                throw e;
            }
            
        }

        public async Task<Reply> AddReplay(Reply  reply)
        {
            try
            {
                Reply newReply = null;
                var result = await _videoQuestionUnit.ReplyRepository.Add(reply);
                var CourseId = (await _videoQuestionUnit.VideoQuestionRepository.GetGroupByReplyId(reply.Id)).CourseId;
                var teacherId = await _videoQuestionUnit.TrackRepository.GetTecherIdByCourseId(CourseId);
                var courseData = await _videoQuestionUnit.CourseRepository.Get(CourseId);
                var ContentId =( await _videoQuestionUnit.VideoQuestionRepository.Get(reply.VideoQuestionId)).ContentId;

                if (result > 0)
                {
                    newReply = await _videoQuestionUnit.ReplyRepository.Get(result);

                    //push to Owner of question
                    var question =await  _videoQuestionUnit.VideoQuestionRepository.Get(newReply.VideoQuestionId);
                    StudentNotification studentNotification = new StudentNotification
                    {
                        CreationDate = DateTime.UtcNow,
                        NotificationToId = CourseId,
                        ReferenceId = 3,
                        ContentId = ContentId,
                        VideoQuestionId= reply.VideoQuestionId,
                        Title = "New reply added to your question ",
                        TitleLT="تم إضافة رد جديد علي سؤالك ",
                        StudentId = question.StudentId,
                        CourseId= CourseId,
                        Minute=question.Minute
                    };
                    var d = Task.Run(async () =>
                    {
                        await _videoQuestionUnit.StudentNotificationRepository.AddNotification(studentNotification);
                    });
                    //push to teacher
                    TeacherNotification teacherNotification = new TeacherNotification
                    {
                        CreationDate = DateTime.UtcNow,
                        NotificationToId = CourseId,
                        ReferenceId = 3,
                        ContentId = ContentId,
                        VideoQuestionId = reply.VideoQuestionId,

                        TeacherId = teacherId,
                        Title = "New reply added to course (" + courseData.Name + ")",
                        TitleLT= "اضافة رد جديد لدورة (" + courseData.NameLT + ") ",
                        Minute = question.Minute

                    };
                    var d2 = Task.Run(async () =>
                    {
                        await _videoQuestionUnit.TeacherNotificationRepository.AddNotification(teacherNotification);
                    });
                }
                return newReply;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public async Task<IEnumerable<VideoQuestion>> GetQuestions(long CourseId,int Page,long VideoQuestionId)
        {
            var result = await _videoQuestionUnit.VideoQuestionRepository.GetQuestions(CourseId, Page, VideoQuestionId);

            return result;
        }

        public async Task<IEnumerable<VideoQuestion>> GetLiveQuestions(long liveID, int Page, long VideoQuestionId)
        {
            var result = await _videoQuestionUnit.VideoQuestionRepository.GetLiveQuestions(liveID, Page, VideoQuestionId);
            return result;
        }


    }
}
