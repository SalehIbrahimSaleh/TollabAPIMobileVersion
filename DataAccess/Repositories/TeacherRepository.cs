using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class TeacherRepository : GenericRepository<Teacher>
    {


        public async Task<Teacher> GetTeacherByIdentityId(string IdentityId)
        {
            try
            {
                Teacher teacher = null;
                var data = await GetAll(" Where IdentityId=N'" + IdentityId + "'");
                if (data.Count() > 0)
                {
                    teacher = data.FirstOrDefault();
                }

                return teacher;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public async Task<int> GetCourseCount(long TeacherId, long CountryId)
        {
            try
            {
                string sql = "";
                if (CountryId != 0)
                {
                    sql = @"select Count(Course.Id) from Course join Track on Course.TrackId=Track.Id join Teacher on Teacher.Id=Track.TeacherId 
                          join Subject on Subject.Id=Track.SubjectId
						  join Department on Department.Id=Subject.DepartmentId
						  join SubCategory on SubCategory.Id=Department.SubCategoryId
						  join Category on Category.Id=SubCategory.CategoryId
						  join Section on Category.SectionId=Section.Id
                             where Teacher.Id=" + TeacherId + " and Section.CountryId=" + CountryId+" and Course.CourseStatusId=3";
                }
                else
                {
                    sql= @"select Count(Course.Id) from Course join Track on Course.TrackId=Track.Id join Teacher on Teacher.Id=Track.TeacherId
                             where Teacher.Id=" + TeacherId + " and Course.CourseStatusId=3";

                }

                var count = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<int>(sql);

                return count;

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<long> GetStudentSubscriptionsCount(long teacherId, long CountryId)
        {
            string sql = "";
            if (CountryId != 0)
            {
                sql = @"select Count(Distinct StudentCourse.StudentId) as StudentCount 
                          from StudentCourse join Course on  StudentCourse.CourseId=Course.Id join Track 
                          on Track.Id=Course.TrackId
                          join Subject on Subject.Id=Track.SubjectId
						  join Department on Department.Id=Subject.DepartmentId
						  join SubCategory on SubCategory.Id=Department.SubCategoryId
						  join Category on Category.Id=SubCategory.CategoryId
						  join Section on Category.SectionId=Section.Id
                          where Track.TeacherId=" + teacherId + " and Section.CountryId=" + CountryId + "";
            }
            else
            {
                sql = @"select Count(Distinct StudentCourse.StudentId) as StudentCount 
                          from StudentCourse join Course on  StudentCourse.CourseId=Course.Id join Track 
                          on Track.Id=Course.TrackId
                          where Track.TeacherId=" + teacherId + "";
            }
            var Count =await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<long>(sql);
            return Count;
        }

        public async Task<long> GetStudentEnrolledCount(long teacherId, long CountryId)
        {
            string sql = "";
            if (CountryId != 0)
            {
                sql = @"select Count( Distinct StudentCourse.StudentId) as StudentCount 
                          from StudentCourse join Course on  StudentCourse.CourseId=Course.Id join Track 
                          on Track.Id=Course.TrackId
                          join Subject on Subject.Id=Track.SubjectId
						  join Department on Department.Id=Subject.DepartmentId
						  join SubCategory on SubCategory.Id=Department.SubCategoryId
						  join Category on Category.Id=SubCategory.CategoryId
						  join Section on Category.SectionId=Section.Id
                          where Track.TeacherId=" + teacherId + " and Section.CountryId=" + CountryId + "";
            }
            else {
                sql = @"select Count( Distinct StudentCourse.StudentId) as StudentCount 
                          from StudentCourse join Course on  StudentCourse.CourseId=Course.Id join Track 
                          on Track.Id=Course.TrackId
                          where Track.TeacherId=" + teacherId + "";
            }
            var Count = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<long>(sql);
            return Count;
        }

        public async Task<int> GetCourseCountInCountry(long TeacherId,long CountryId)
        {
            try
            {
                string sql = @"select Count(Course.Id) from Course
join Track on Course.TrackId=Track.Id
join Teacher on Teacher.Id=Track.TeacherId
join Subject on Subject.Id=Track.SubjectId
join Department on Department.Id=Subject.DepartmentId
join SubCategory on SubCategory.Id=Department.SubCategoryId
join Category on Category.Id=SubCategory.CategoryId
join Section on Category.SectionId=Section.Id
where Teacher.Id=" + TeacherId + " and Section.CountryId="+CountryId+" and Course.CourseStatusId=3";

                var count = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<int>(sql);

                return count;

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<long> GetStudentSubscriptionsCountInCountry(long TeacherId, long CountryId)
        {
            string sql = "";
            if (CountryId != 0)
            {
                sql = @"select Count(Distinct StudentCourse.StudentId) as StudentCount from StudentCourse
join Course on  StudentCourse.CourseId=Course.Id 
join Track on Track.Id=Course.TrackId
join Subject on Subject.Id=Track.SubjectId
join Department on Department.Id=Subject.DepartmentId
join SubCategory on SubCategory.Id=Department.SubCategoryId
join Category on Category.Id=SubCategory.CategoryId
join Section on Category.SectionId=Section.Id 
 where Track.TeacherId=" + TeacherId + "  and Section.CountryId=" + CountryId + "";
            }
            else
            {
                sql = @"select Count(Distinct StudentCourse.StudentId) as StudentCount from StudentCourse
join Course on  StudentCourse.CourseId=Course.Id 
join Track on Track.Id=Course.TrackId 
 where Track.TeacherId=" + TeacherId + "  ";
            }
            var Count = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<long>(sql);
            return Count;
        }

        public async Task<long> GetStudentEnrolledCountInCountry(long TeacherId, long CountryId)
        {
            string sql = "";
            if (CountryId != 0)
            {
                sql=@"select Count( Distinct StudentCourse.StudentId) as StudentCount from StudentCourse
join Course on  StudentCourse.CourseId=Course.Id 
join Track on Track.Id=Course.TrackId
join Subject on Subject.Id=Track.SubjectId
join Department on Department.Id=Subject.DepartmentId
join SubCategory on SubCategory.Id=Department.SubCategoryId
join Category on Category.Id=SubCategory.CategoryId
join Section on Category.SectionId=Section.Id 
 where Track.TeacherId=" + TeacherId + "  and Section.CountryId=" + CountryId + "";
            }
            else
            {
                sql = @"select Count( Distinct StudentCourse.StudentId) as StudentCount from StudentCourse
join Course on  StudentCourse.CourseId=Course.Id 
join Track on Track.Id=Course.TrackId
  
 where Track.TeacherId=" + TeacherId + " ";
            }
            
            var Count = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<long>(sql);
            return Count;
        }

        public async Task<IEnumerable<Interest>> GetTeacherInterests(long TeacherId)
        {
            try
            {
                List<Interest> result = new List<Interest>();
                string query = @"select Distinct * from
 (
select Section.Id as SectionId,Section.Image as SectionImage,Section.Name as SectionName,Section.NameLT as SectionNameLT,
Category.Name as CategoryName,Category.NameLT as CategoryNameLT,SubCategory.Name SubCategoryName ,
SubCategory.NameLT SubCategoryNameLT,SubCategory.Id as SubCategoryId from Category join SubCategory on Category.Id=SubCategory.CategoryId
join Department on SubCategory.Id=Department.SubCategoryId join [Subject] On
Department.Id=[Subject].DepartmentId join Section on Category.SectionId=Section.Id join Track on [Subject].Id=Track.SubjectId
Where Track.TeacherId=" + TeacherId + ") NewTable ";
                var list = await _connectionFactory.GetConnection.QueryAsync<Interest>(query);

                return list;

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<IEnumerable<TrackWithCourses>> GetCoursesByTeacherId(long TeacherId)
        {
            try
            {
                 string sql= @"select Distinct [Track].ImageHomeCover as Image,

[Track].[Id]
      ,[Track].[TrackSubject]
      ,[Track].[Name]
      ,[Track].[NameLT]
      ,[Track].[TeacherId]
      ,[Track].[SubjectId]
      ,[Track].[SubscriptionDuration]
      ,[Track].[SubscriptionCurrentPrice]
      ,[Track].[SubscriptionOldPrice]
      ,[Track].[BySubscription]
      ,[Track].[OrderNumber]
      ,[Track].[TrackCode]
      ,[Track].[SKUNumber]
      ,[Track].[SKUPrice]
      ,[Track].[OldSKUPrice]
      ,[Track].[ShowWaterMark]
      ,[Track].[WhatsupGroupLink]
      ,[Track].[IsLogicallyDeleted]
      ,(select Count(Id) from Course where TrackId=Track.Id and Course.CourseStatusId IN (1,3) ) as CountCourse ,
                (select ISNULL( Sum(CurrentCost),0) from Course where TrackId=Track.Id and Course.CourseStatusId IN (1,3)  ) as TotalCurrentCost,
                (select ISNULL( Sum(PreviousCost),0) from Course where TrackId=Track.Id and Course.CourseStatusId IN (1,3)) as TotalOldCost  
                from Department join  [Subject] on Department.Id=[Subject].DepartmentId 
                join Track on [Subject].Id=Track.SubjectId
                join Course on Track.Id=Course.TrackId
                where Track.TeacherId=" + TeacherId+ " And Course.CourseStatusId IN (1,3)";
                var Header = await _connectionFactory.GetConnection.QueryAsync<TrackWithCourses>(sql);
              
                return Header;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<IEnumerable<TrackWithCourses>> GetCoursesByTeacherIdForCountry(long TeacherId,long CountryId)
        {
            try
            {
                string sql = @"select Distinct  [Track].ImageHomeCover as Image,

[Track].[Id]
      ,[Track].[TrackSubject]
      ,[Track].[Name]
      ,[Track].[NameLT]
      ,[Track].[TeacherId]
      ,[Track].[SubjectId]
      ,[Track].[SubscriptionDuration]
      ,[Track].[SubscriptionCurrentPrice]
      ,[Track].[SubscriptionOldPrice]
      ,[Track].[BySubscription]
      ,[Track].[OrderNumber]
      ,[Track].[TrackCode]
      ,[Track].[SKUNumber]
      ,[Track].[SKUPrice]
      ,[Track].[OldSKUPrice]
      ,[Track].[ShowWaterMark]
      ,[Track].[WhatsupGroupLink]
      ,[Track].[IsLogicallyDeleted]
      
,(select Count(Id) from Course where TrackId=Track.Id and Course.CourseStatusId IN (1,3) ) as CountCourse ,
                (select ISNULL( Sum(CurrentCost),0) from Course where TrackId=Track.Id and Course.CourseStatusId IN (1,3)  ) as TotalCurrentCost,
                (select ISNULL( Sum(PreviousCost),0) from Course where TrackId=Track.Id and Course.CourseStatusId IN (1,3)) as TotalOldCost  
                from Department 
				join  [Subject] on Department.Id=[Subject].DepartmentId 
                join Track on [Subject].Id=Track.SubjectId
                join Course on Track.Id=Course.TrackId
join SubCategory on SubCategory.Id=Department.SubCategoryId
join Category on Category.Id=SubCategory.CategoryId
join Section on Category.SectionId=Section.Id

                where Track.TeacherId=" + TeacherId + " and Section.CountryId="+CountryId+" And Course.CourseStatusId IN (1,3)";
                var Header = await _connectionFactory.GetConnection.QueryAsync<TrackWithCourses>(sql);

                return Header;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<IEnumerable<Course>> GetCoursesBySubjectId(long TeacherId,long SubjectId,int Page)
        {
            Page = Page * 30;
            string sql2 = @"select Course.* from Course where   Course.CourseStatusId=3 And
                 TrackId In(select Track.Id from Track where Track.SubjectId=" + SubjectId + " And Track.TeacherId=" + TeacherId + "  ) order by Course.Id  OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY";
            var Courses = await _connectionFactory.GetConnection.QueryAsync<Course>(sql2);
            return Courses;
        }

        public async Task<IEnumerable<TeacherHomeCourse>> GetTeacherHomeCourses(long TeacherId,long CountryId, int Page)
        {
            try
            {
               
                Page = Page * 3;
                string sqlHeader = @"select distinct * from (select Subject.Id as SubjectId, Subject.Name as SubjectName,Subject.NameLT as SubjectNameLT,
           SubCategory.Name as SubCategoryName,SubCategory.NameLT as SubCategoryNameLT,Category.Name as CategoryName,Category.NameLT as CategoryNameLT
           from Subject join Track  on Subject.Id=Track.SubjectId join Department on Department.Id=Subject.DepartmentId
            join SubCategory on SubCategory.Id=Department.SubCategoryId 
			join Category on Category.Id=SubCategory.CategoryId
			join Section on Section.Id=Category.SectionId 
            where Track.TeacherId=" + TeacherId + " and  Section.CountryId="+ CountryId + " ) newTable";// order by Subject.Id OFFSET " + Page + " Rows FETCH Next 3 Rows ONLY
                var Header = await _connectionFactory.GetConnection.QueryAsync<TeacherHomeCourse>(sqlHeader);
                foreach (var item in Header)
                {//(select Count(StudentId) from StudentCourse where CourseId=Course.Id) CountStudentEnrolled 
 string sqlBody = @" select OuterCourse.*,( select Count(StudentCourse.StudentId) as StudentCount from StudentCourse join Course on  StudentCourse.CourseId=Course.Id join Track on Track.Id=Course.TrackId where StudentCourse.CourseId=OuterCourse.Id And Track.TeacherId=" + TeacherId + ") as CountStudentEnrolled " +
",(select isnull(sum(Amount), 0) from TeacherTransaction where CourseId = OuterCourse.Id And TeacherId = " + TeacherId + ") GainedMoney "+
@" from Course as OuterCourse join Track on OuterCourse.TrackId=Track.Id
where OuterCourse.CourseStatusId IN (1,3) And Track.SubjectId=" + item.SubjectId + " And Track.teacherId="+TeacherId+"";
                    var Body = await _connectionFactory.GetConnection.QueryAsync<Course>(sqlBody);
                    item.Courses = Body;
                }
                return Header;

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<Teacher> GetTeacherByTrackId(long trackId)
        {
            try
            {
                var teacher = _connectionFactory.GetConnection.QueryFirstOrDefault<Teacher>("select Teacher.* from Teacher Join Track on Teacher.Id=Track.TeacherId where Track.Id=" + trackId + "");
                return teacher;

            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<Teacher> GetTeacherByCourseId(long CourseId)
        {
            try
            {
                var data = await GetOneByQuery("select Distinct Teacher.* from Teacher join Track on Teacher.Id=Track.TeacherId join Course on Course.TrackId=Track.Id where Course.Id="+CourseId+" ");
                return data;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public async Task<Teacher> GetTeacherByLiveId(long liveId)
        {
            try
            {
                var data = await GetOneByQuery("select Distinct Teacher.* from Teacher join Live on Teacher.Id=Live.TeacherId where Live.Id=" + liveId+ " ");
                return data;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

    }
}
