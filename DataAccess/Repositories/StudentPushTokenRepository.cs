using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class StudentPushTokenRepository : GenericRepository<StudentPushToken>
    {
        public async Task<bool> DeleteAllTokensAsync(long studentId)
        {
            string sql = @"Delete from StudentPushToken where StudentId="+studentId+"";
            var result=await _connectionFactory.GetConnection.ExecuteAsync(sql);
            if (result>=0)
            {
                return true;
            }
            return false;
        }

        public async Task<StudentTokens> GetAllByCourseId(long CourseId)
        {
            StudentTokens studentTokens = new StudentTokens();
            studentTokens.IosStudentPushTokens = await _connectionFactory.GetConnection.QueryAsync<StudentPushToken>(@"select * from StudentPushToken join StudentCourse on StudentPushToken.StudentId=StudentCourse.StudentId
where StudentCourse.CourseId = "+CourseId+" And StudentPushToken.OS = 'ios'");
            studentTokens.AndroidStudentPushTokens = await _connectionFactory.GetConnection.QueryAsync<StudentPushToken>(@"select * from StudentPushToken join StudentCourse on StudentPushToken.StudentId=StudentCourse.StudentId
where StudentCourse.CourseId = " + CourseId + " And StudentPushToken.OS = 'android'");

            return studentTokens;
        }
    }
}
