using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
  public class TrackRepository:GenericRepository<Track>
    {
        public async Task<IEnumerable<Track>> GetTracks(long? TeacherId,long? SubjectId, int Page)
        {
            try
            {
                Page = Page * 30;

                var result = await GetAllByQuery("select Track.*,(select Count(Course.Id) from Course where TrackId=Track.Id And Course.CourseStatusId!=3 ) as CountCourse from Track Where Track.Isactive=1 and TeacherId=" + TeacherId + " And SubjectId="+ SubjectId + "  order by Id   OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY");
                return result;
            }
            catch (Exception e)
            {

                throw e;
            } 

        }

        public async Task<Track> GetTrackByCourseId(long courseId)
        {
            var result = await GetOneByQuery("select Track.* from Track join Course on Track.Id = Course.TrackId where Track.IsActive=1  and Course.Id =" + courseId+"");
            return result;
        }

        public async Task<long?> GetTecherIdByCourseId(long? courseId)
        {
            var result = await GetOneByQuery("select Track.TeacherId from Track join Course on Track.Id = Course.TrackId where Course.Id="+ courseId + " ");
            if (result!=null)
            {
                return result.TeacherId;
            }
            return null;
        }

        public async  Task<bool> IsFound(Track track)
        {
            var result = await GetAll(" where ( Name Like '" + track.Name + "' or NameLT Like '" + track.NameLT + "' ) and Track.Isactive=1 And TeacherId=" + track.TeacherId + " And SubjectId=" + track.SubjectId + "");
            if (result.Count()>0)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Subject>> GetSubjectsWithTracksByDepartmentId(long StudentId,long departmentId, int Page)
        {
            try
            {
                Page = Page * 3;
                string sql = @"select * ,(select count(Course.Id) from Course where TrackId=Track.Id And Course.CourseStatusId=3) CountCourse,
(select SUM(Course.CurrentCost) from Course where TrackId=Track.Id And Course.CourseStatusId=3) TotalCurrentCost,
(select SUM(Course.PreviousCost) from Course where TrackId=Track.Id And Course.CourseStatusId=3 ) TotalOldCost,
(select 'true' from TrackSubscription where  TrackId= Track.Id And StudentId="+StudentId+") IsSubscriped,"+
                             @"(select Teacher.name from Teacher where Id =Track.TeacherId) as TeacherName
                            from Subject join Track on Subject.Id=Track.SubjectId where Track.IsActive=1 and Subject.DepartmentId=" + departmentId+ " order by Track.OrderNumber ";
                var SubjectDictionary = new Dictionary<long,Subject>();
                var list = _connectionFactory.GetConnection.Query<Subject, Track, Subject>(
                    sql,
                    (subject, track) =>
                    {
                        Subject SubjectEntry;

                        if (!SubjectDictionary.TryGetValue(subject.Id, out SubjectEntry))
                        {
                            SubjectEntry = subject;
                            SubjectEntry.Tracks = new List<Track>();
                            SubjectDictionary.Add(SubjectEntry.Id, SubjectEntry);
                        }
                        SubjectEntry.Tracks.Add(track);

                        return SubjectEntry;
                    },
                    splitOn: "Id")
                .Distinct().Skip(Page)
                .ToList();
                return list;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}
