using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class TrackSubscriptionRepository : GenericRepository<TrackSubscription>
    {
        StudentCourseRepository _studentCourseRepository = new StudentCourseRepository();


        public async Task<TrackSubscription> CheckIfStudentSubscripeThistrackBefore(long trackId, long studentId)
        {
            var result = await GetWhere(" where StudentId=" + studentId + " And TrackId=" + trackId + " ");
            if (result == null)
            {
                return null;
            }
            return result;
        }

        public async Task<long> AddTrackToStudent(TrackSubscription trackSubscription)
        {
            var id = await Add(trackSubscription);
            if (id>0)
            {
                var courses =  _connectionFactory.GetConnection.Query<Course>("Select * from Course where TrackId=" + trackSubscription.TrackId + "").ToList();
                foreach (var course in courses)
                {
                    var IsCourseFound = await _studentCourseRepository.GetWhere(" Where StudentId=" + trackSubscription.StudentId + " And CourseId=" + course.Id + "");
                    if (IsCourseFound != null)
                    {
                        continue;
                    }

                    StudentCourse studentCourse = new StudentCourse
                    {
                        CourseId = course.Id,
                        StudentId = trackSubscription.StudentId,
                        EnrollmentDate = DateTime.UtcNow,
                        ReferenceNumber= trackSubscription.ReferenceNumber
                    };
                  await  _studentCourseRepository.Add(studentCourse);
                }

            }
            return id;
        }

        public async Task<long> UpdateTrackToStudent(TrackSubscription isEnrolledBefore)
        {
            var updated = await Update(isEnrolledBefore);
            if (updated)
            {
                var courses = _connectionFactory.GetConnection.Query<Course>("Select * from Course where TrackId=" + isEnrolledBefore.TrackId + "").ToList();
                foreach (var course in courses)
                {
                    var IsCourseFound = await _studentCourseRepository.GetWhere(" Where StudentId="+isEnrolledBefore.StudentId+" And CourseId="+course.Id+"");
                    if (IsCourseFound!=null)
                    {
                        continue;
                    }

                    StudentCourse studentCourse = new StudentCourse
                    {
                        CourseId = course.Id,
                        StudentId = isEnrolledBefore.StudentId,
                        EnrollmentDate = DateTime.UtcNow,
                        ReferenceNumber = isEnrolledBefore.ReferenceNumber

                    };
                    await _studentCourseRepository.Add(studentCourse);
                }

            }
            if (updated)
            {
                return isEnrolledBefore.Id;

            }
            return 0;

        }
    }
}
