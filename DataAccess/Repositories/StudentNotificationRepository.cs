using Dapper;
using DataAccess.Entities;
using DataAccess.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class StudentNotificationRepository:GenericRepository<StudentNotification>
    {
        public async Task<IEnumerable<StudentNotification>> GetAllStudentNotification(long StudentId, int Page)
        {
            
            Page = Page * 30;
           // var result = await GetAll(" where StudentNotification.StudentId="+ StudentId + "  order by Id OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY");

            string sql = "SELECT  * FROM StudentNotification  INNER JOIN Reference  ON Reference.Id = StudentNotification.ReferenceId where StudentNotification.StudentId=" + StudentId + " Order By StudentNotification.Id DESC";
            var CategoryDictionary = new Dictionary<long, StudentNotification>();
            var list = _connectionFactory.GetConnection.Query<StudentNotification, Reference, StudentNotification>(
                sql,
                (studentNotification, reference) =>
                {
                    StudentNotification studentNotificationEntry;

                    if (!CategoryDictionary.TryGetValue(studentNotification.Id, out studentNotificationEntry))
                    {
                        studentNotificationEntry = studentNotification;
                        studentNotificationEntry.Reference = new Reference();
                        CategoryDictionary.Add(studentNotificationEntry.Id, studentNotificationEntry);
                    }

                    studentNotificationEntry.Reference=reference;
                    return studentNotificationEntry;
                },
                splitOn: "Id")
            .Distinct().Skip(Page).Take(30)
            .ToList();



            return list;
        }


        public async Task AddNotification(StudentNotification studentNotification)
        {
            var id =await Add(studentNotification);
            string sql = @"select * from StudentPushToken where StudentId=" + studentNotification.StudentId + "";

            var Tokens = _connectionFactory.GetConnection.Query<StudentPushToken>(sql);
            foreach (var item in Tokens)
            {
                if (item.OS=="ios")
                {
                    PushManager.PushToStudentToIphoneDevice(item.Token, studentNotification.Title, studentNotification.NotificationToId, studentNotification.ReferenceId, studentNotification.ContentId, studentNotification.VideoQuestionId, studentNotification.Minute);
                }
                else if (item.OS == "android")
                {
                    PushManager.pushToAndroidDevice(item.Token, studentNotification.Title, studentNotification.NotificationToId, studentNotification.ReferenceId, studentNotification.ContentId, studentNotification.VideoQuestionId, studentNotification.Minute);
                }
            }

        }


        public async Task AddBulkNotification(StudentNotification studentNotification,StudentTokens Tokens)
        {
            //List<StudentNotification> studentNotifications = new List<StudentNotification>();
            foreach (var item in Tokens.IosStudentPushTokens)
            {
                StudentNotification notification = new StudentNotification();
                notification.NotificationToId = studentNotification.NotificationToId;
                notification.NotificationTypeId = studentNotification.NotificationTypeId;
                notification.ReferenceId = studentNotification.ReferenceId;
                notification.StudentId = item.StudentId;
                notification.Title = studentNotification.Title;
                notification.TitleLT = studentNotification.TitleLT;
                notification.CourseId = studentNotification.CourseId;
                notification.CreationDate = studentNotification.CreationDate;
                //studentNotifications.Add(notification);
               await Add(notification);
                PushManager.PushToStudentToIphoneDevice(item.Token, studentNotification.Title, studentNotification.NotificationToId, studentNotification.ReferenceId, studentNotification.ContentId, studentNotification.VideoQuestionId);
            }
            List<string> AndroidTokens = new List<string>();
            foreach (var tok in Tokens.AndroidStudentPushTokens)
            {
                StudentNotification notification = new StudentNotification();
                notification.NotificationToId = studentNotification.NotificationToId;
                notification.NotificationTypeId = studentNotification.NotificationTypeId;
                notification.ReferenceId = studentNotification.ReferenceId;
                notification.StudentId = tok.StudentId;
                notification.Title = studentNotification.Title;
                notification.TitleLT = studentNotification.TitleLT;
                notification.CourseId = studentNotification.CourseId;
                notification.CreationDate = studentNotification.CreationDate;
                await Add(notification);
                AndroidTokens.Add(tok.Token);
            }
            PushManager.pushBulkToAndroidDevice(AndroidTokens, studentNotification.Title, studentNotification.NotificationToId, studentNotification.ReferenceId, studentNotification.ContentId, studentNotification.VideoQuestionId);
        }

        public async Task AddNotificationWithoutPush(StudentNotification studentNotification)
        {
            await Add(studentNotification);
        }

        internal async Task<int> GetStudentNotificationNotSeenCount(long studentId)
        {

            string sql = @"select Count(*) from StudentNotification where StudentId=" + studentId + " and Seen is null";
            var CountNotification =await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<int>(sql);
            return CountNotification;
        }
    }
}
