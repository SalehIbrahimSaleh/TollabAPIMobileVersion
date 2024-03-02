using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
  public  class VideoQuestionRepository:GenericRepository<VideoQuestion>
    {
        public async Task<IEnumerable<VideoQuestion>> GetQuestions(long CourseId, int Page,long VideoQuestionId)
        {
            try
            {
                var conn = _connectionFactory.GetConnection;
                Page = Page * 3;
                string sqlQuestions = @"select VideoQuestion.*,Student.Name as StudentName,Student.Photo as StudentPhoto,Content.Name as ContentName,
                           Content.NameLT as ContentNameLT from VideoQuestion join Content on VideoQuestion.ContentId=Content.Id
                            left join Student on Student.Id=VideoQuestion.StudentId and VideoQuestion.ViewMyAccount=1  join [Group] on [Group].Id=Content.GroupId 
                            join Course on [Group].CourseId=Course.Id where Course.Id =" + CourseId + " ";
                if (VideoQuestionId>0)
                {
                    sqlQuestions = sqlQuestions + " And VideoQuestion.Id="+ VideoQuestionId + " ";
                }
                sqlQuestions = sqlQuestions + " order by VideoQuestion.Id DESC  OFFSET " + Page + " Rows FETCH Next 3 Rows ONLY ";
                var QuestionList = await conn.QueryAsync<VideoQuestion>(sqlQuestions);
                foreach (var item in QuestionList)
                {
                    string sqlReplies = @"select Reply.*,Student.Name as StudentName,Student.Photo as StudentPhoto,Teacher.Name As TeacherName,Teacher.Photo as TeacherPhoto
                                        from Reply left join Student on Reply.StudentId=Student.Id and (Reply.StudentId Is Not Null and Reply.ViewMyAccount=1 )
                                        left  join Teacher on Teacher.Id=Reply.TeacherId and ( Reply.TeacherId Is Not Null and Reply.ViewMyAccount=1)
                                        where Reply.VideoQuestionId=" + item.Id + "  order by Reply.Id DESC";
                    var RepliesList = await conn.QueryAsync<Reply>(sqlReplies);
                    if (RepliesList.Count() > 0)
                    {
                        item.Replies = RepliesList;
                    }
                }
                return QuestionList;

            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public async Task<IEnumerable<VideoQuestion>> GetLiveQuestions(long liveId, int Page, long VideoQuestionId)
        {
            try
            {
                var conn = _connectionFactory.GetConnection;
                Page = Page * 3;
                string sqlQuestions = @"select VideoQuestion.*,Student.Name as StudentName,Student.Photo as StudentPhoto
                            from VideoQuestion join Live on VideoQuestion.LiveId= Live.Id
                            left join Student on Student.Id=VideoQuestion.StudentId and VideoQuestion.ViewMyAccount=1 where Live.Id =" + liveId + " ";
                if (VideoQuestionId > 0)
                {
                    sqlQuestions = sqlQuestions + " And VideoQuestion.Id=" + VideoQuestionId + " ";
                }
                sqlQuestions = sqlQuestions + " order by VideoQuestion.Id DESC  OFFSET " + Page + " Rows FETCH Next 3 Rows ONLY ";
                var QuestionList = await conn.QueryAsync<VideoQuestion>(sqlQuestions);
                foreach (var item in QuestionList)
                {
                    string sqlReplies = @"select Reply.*,Student.Name as StudentName,Student.Photo as StudentPhoto,Teacher.Name As TeacherName,Teacher.Photo as TeacherPhoto
                                        from Reply left join Student on Reply.StudentId=Student.Id and (Reply.StudentId Is Not Null and Reply.ViewMyAccount=1 )
                                        left  join Teacher on Teacher.Id=Reply.TeacherId and ( Reply.TeacherId Is Not Null and Reply.ViewMyAccount=1)
                                        where Reply.VideoQuestionId=" + item.Id + "  order by Reply.Id DESC";
                    var RepliesList = await conn.QueryAsync<Reply>(sqlReplies);
                    if (RepliesList.Count() > 0)
                    {
                        item.Replies = RepliesList;
                    }
                }
                return QuestionList;

            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public async  Task<Group> GetGroupByReplyId(long ReplyId)
        {
            string sql = @"select [Group].* from Reply join VideoQuestion on VideoQuestion.Id=Reply.VideoQuestionId join Content on Content.Id=VideoQuestion.ContentId
             join [Group] on [Group].Id=Content.GroupId where Reply.Id=" + ReplyId + "";
            var result = _connectionFactory.GetConnection.Query<Group>(sql);

            return result.FirstOrDefault();

        }

        public async Task<IEnumerable<VideoQuestion>> GetQuestionsForStudent(long ContentId,long StudentId,long VideoQuestionId)
        {
            try
            {
                var conn = _connectionFactory.GetConnection;
                string sqlQuestions = @"select VideoQuestion.*,Student.Name as StudentName,Student.Photo as StudentPhoto,Content.Name as ContentName,
 Content.NameLT as ContentNameLT from VideoQuestion join Content on VideoQuestion.ContentId=Content.Id
 left join Student on Student.Id=VideoQuestion.StudentId and VideoQuestion.ViewMyAccount=1 
 where Content.Id="+ ContentId + " and  VideoQuestion.Id=" + VideoQuestionId + " and  VideoQuestion.StudentId=" + StudentId+"";
                var QuestionList = await conn.QueryAsync<VideoQuestion>(sqlQuestions);
                foreach (var item in QuestionList)
                {
                    string sqlReplies = @"select Reply.*,Student.Name as StudentName,Student.Photo as StudentPhoto,Teacher.Name As TeacherName,Teacher.Photo as TeacherPhoto
                                        from Reply left join Student on Reply.StudentId=Student.Id and (Reply.StudentId Is Not Null and Reply.ViewMyAccount=1 )
                                        left  join Teacher on Teacher.Id=Reply.TeacherId and ( Reply.TeacherId Is Not Null and Reply.ViewMyAccount=1)
                                        where Reply.VideoQuestionId=" + item.Id + "  order by Reply.Id DESC";
                    var RepliesList = await conn.QueryAsync<Reply>(sqlReplies);
                    if (RepliesList.Count() > 0)
                    {
                        item.Replies = RepliesList;
                    }
                }
                return QuestionList;

            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public async Task<IEnumerable<VideoQuestion>> GetQuestionsForTeacher(long ContentId, long TeacherId,long VideoQuestionId)
        {
            try
            {
                var conn = _connectionFactory.GetConnection;
                string sqlQuestions = @"select VideoQuestion.*,Student.Name as StudentName,Student.Photo as StudentPhoto,Content.Name as ContentName,
 Content.NameLT as ContentNameLT from VideoQuestion join Content on VideoQuestion.ContentId=Content.Id
 left join Student on Student.Id=VideoQuestion.StudentId and VideoQuestion.ViewMyAccount=1 
 where Content.Id=" + ContentId + " and VideoQuestion.Id="+ VideoQuestionId + "";
                var QuestionList = await conn.QueryAsync<VideoQuestion>(sqlQuestions);
                foreach (var item in QuestionList)
                {
                    string sqlReplies = @"select Reply.*,Student.Name as StudentName,Student.Photo as StudentPhoto,Teacher.Name As TeacherName,Teacher.Photo as TeacherPhoto
                                        from Reply left join Student on Reply.StudentId=Student.Id and (Reply.StudentId Is Not Null and Reply.ViewMyAccount=1 )
                                        left  join Teacher on Teacher.Id=Reply.TeacherId and ( Reply.TeacherId Is Not Null and Reply.ViewMyAccount=1)
                                        where Reply.VideoQuestionId=" + item.Id + "  order by Reply.Id DESC";
                    var RepliesList = await conn.QueryAsync<Reply>(sqlReplies);
                    if (RepliesList.Count() > 0)
                    {
                        item.Replies = RepliesList;
                    }
                }
                return QuestionList;

            }
            catch (Exception e)
            {

                throw e;
            }

        }

    }
}
