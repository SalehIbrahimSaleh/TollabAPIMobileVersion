using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class CourseDepartmentRepository:GenericRepository<CourseDepartment>
    {
        public async Task<IEnumerable<CourseTags>> GetCoursesTags(long CourseId)
        {
            try
            {
                string sql = @"  SELECT dbo.Department.Name, dbo.Department.NameLT
                          FROM dbo.Track INNER JOIN
                         dbo.Course ON dbo.Track.Id = dbo.Course.TrackId INNER JOIN
                         dbo.Subject ON dbo.Track.SubjectId = dbo.Subject.Id INNER JOIN
                         dbo.Department ON dbo.Subject.DepartmentId = dbo.Department.Id where Track.Isactive=1 and  Course.Id=" + CourseId + "";

                var result = await _connectionFactory.GetConnection.QueryAsync<CourseTags>(sql);
                return result;
            }
            catch (Exception e)
            {

                throw e;
            }      
        }
        
        public async Task<long> GetDepartmentIdByTrackId(long TrackId)
        {
            string sql = @"select [Subject].DepartmentId from [Subject] join Track on [Subject].Id=Track.SubjectId where Track.Isactive=1 and Track.Id=" + TrackId + "";

            var id=await  _connectionFactory.GetConnection.QueryAsync<long>(sql);
            return id.FirstOrDefault();
        }
    }
}
