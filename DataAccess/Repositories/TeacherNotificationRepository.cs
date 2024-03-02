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
  public  class TeacherNotificationRepository:GenericRepository<TeacherNotification>
    {
        public async Task<IEnumerable<TeacherNotification>> GetAllTeacherNotification(long TeacherId, int Page)
        {
            Page = Page * 30;
            string sql = "SELECT  * FROM TeacherNotification  INNER JOIN Reference  ON Reference.Id = TeacherNotification.ReferenceId where TeacherNotification.TeacherId=" + TeacherId + " Order By TeacherNotification.Id DESC";
            var CategoryDictionary = new Dictionary<long, TeacherNotification>();
            var list = _connectionFactory.GetConnection.Query<TeacherNotification, Reference, TeacherNotification>(
                sql,
                (teacherNotification, reference) =>
                {
                    TeacherNotification teacherNotificationEntry;

                    if (!CategoryDictionary.TryGetValue(teacherNotification.Id, out teacherNotificationEntry))
                    {
                        teacherNotificationEntry = teacherNotification;
                        teacherNotificationEntry.Reference = new Reference();
                        CategoryDictionary.Add(teacherNotificationEntry.Id, teacherNotificationEntry);
                    }

                    teacherNotificationEntry.Reference = reference;
                    return teacherNotificationEntry;
                },
                splitOn: "Id")
            .Distinct().Skip(Page).Take(30)
            .ToList();



            return list;
        }

        public async Task AddNotification(TeacherNotification teacherNotification)
        {

            var id = await Add(teacherNotification);
            string sql = @"select * from TeacherPushToken where TeacherId=" + teacherNotification.TeacherId + "";

            var Tokens = _connectionFactory.GetConnection.Query<TeacherPushToken>(sql);
            foreach (var item in Tokens)
            {
                if (item.OS.ToLower() == "ios")
                {
                    PushManager.PushToTeacherToIphoneDevice(item.Token, teacherNotification.Title, teacherNotification.NotificationToId, teacherNotification.ReferenceId, teacherNotification.ContentId, teacherNotification.VideoQuestionId, teacherNotification.Minute);
                }
                else if (item.OS.ToLower() == "android")
                {
                    PushManager.pushToAndroidDevice(item.Token, teacherNotification.Title, teacherNotification.NotificationToId, teacherNotification.ReferenceId, teacherNotification.ContentId, teacherNotification.VideoQuestionId, teacherNotification.Minute);
                }
            }

        }

        internal async Task<int> GetTeacherNotificationNotSeenCount(long TeacherId)
        {

            string sql = @"select Count(*) from TeacherNotification where TeacherId=" + TeacherId + " and Seen is null";
            var CountNotification = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<int>(sql);
            return CountNotification;
        }
    }
}
