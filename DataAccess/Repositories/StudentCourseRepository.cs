using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace DataAccess.Repositories
{
    public class StudentCourseRepository : GenericRepository<StudentCourse>
    {
        public async Task<int> GetCourseCountByStudentId(long StudentId)
        {
            var result = await GetAll(" where StudentId=" + StudentId + "");

            return result.Count();
        }

        public async Task<bool> CheckIfStudentEnrolledInThisCourseBefore(long CourseId, long StudentId)
        {
            var result = await GetWhere(" where StudentId=" + StudentId + " And CourseId=" + CourseId + " ");
            if (result == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CheckIfStudentEnrolledInThisLiveBefore(long liveId, long StudentId)
        {
            var result = await GetWhere(" where StudentId=" + StudentId + " And CourseId=" + liveId + " ");
            if (result == null)
            {
                return false;
            }
            return true;
        }
       
        public async Task<int> RemoveCourseFromDownloadById(long courseId)
        {
                var delete = await _connectionFactory.GetConnection.ExecuteAsync("delete from DownloadObj where CourseId=" + courseId + "");
                return delete;
           
        }
        
        public async Task<IEnumerable<MyCourse>> GetMyCourses(long StudentId, int Page)
        {
            try
            {
                Page = Page * 30;
                var conn = _connectionFactory.GetConnection;
                string queryGetHeader = @"select distinct * from 
            (
            select SubCategory.Name as SubCategoryName ,SubCategory.NameLT as SubCategoryNameLT
                        ,[Subject].Id as SubjectId,[Subject].Name as SubjectName,[Subject].NameLT as SubjectNameLT
                        from Subject join Track on Subject.Id=Track.SubjectId
                        join Department on Department.Id=Subject.DepartmentId 
                        join SubCategory on SubCategory.Id=Department.SubCategoryId 
                        join Course on Course.TrackId=Track.Id
            where Course.Id In (select Distinct CourseId from StudentCourse where StudentId=" + StudentId + ")) NewTable order by NewTable.SubjectId OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY";
                var Header = await conn.QueryAsync<MyCourse>(queryGetHeader);

                foreach (var item in Header)
                {
string sqlBody = @"select myCourse.* , (select Name from Teacher where Id=Track.TeacherId) as TeacherName,
(select  Count(StudentContent.Id)  from Content left Join StudentContent  on Content.Id=StudentContent.ContentId 
left join  [Group] on [Group].Id=Content.GroupId left join Course on [Group].CourseId=Course.Id where Course.Id=myCourse.Id  " +
" And StudentContent.StudentId="+StudentId+") ViewedContent," +
"(select  Count(Content.Id)  from Content  left join  [Group] on [Group].Id=Content.GroupId "+
" left join Course on [Group].CourseId=Course.Id join StudentCourse on StudentCourse.CourseId=Course.Id  where Course.Id=myCourse.Id   And StudentCourse.StudentId="+StudentId+")"+
@" as ContentCount from Subject join Track on Subject.Id=Track.SubjectId 
join Department on Department.Id=Subject.DepartmentId  
join SubCategory on SubCategory.Id=Department.SubCategoryId 
join Course as myCourse on myCourse.TrackId=Track.Id            
where myCourse.CourseStatusId=3 And myCourse.Id In (select Distinct CourseId from StudentCourse where StudentId=" + StudentId+"" +
" And CourseId Not IN (select Course.Id from TrackSubscription join Course on TrackSubscription.TrackId=Course.TrackId where TrackSubscription.TrackId=Track.Id And StudentId="+StudentId+" and DurationExpiration< getdate()) ) And [Subject].Id=" + item.SubjectId+"";
                    var Body = await conn.QueryAsync<Course>(sqlBody);

                    

                    if (Body.Count() > 0)
                    {
                        foreach (var items in Body)
                        {
                            var syntaxx = "";
                            if (StudentId != null)
                            {
                                syntaxx = "and StudentCourseDownloaded.studentid=" + StudentId;
                            }
                            var xx = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<DownloadObj>("select course.Id as Id, OfflinePackage.IsShowInMobile as CanDownload,\r\n case when count(StudentCourseDownloaded.StudentId) >= 0 then 1 else 0 end  as IsDownloaded,\r\nOfflinePackage.PackageEndDate  as ValidToDownloadDate,\r\nDATEDIFF(day, getdate(),OfflinePackage.PackageEndDate) AS RemainingDays,\r\n\r\nCourse.CurrentCost as Price,\r\ncourse.Id as CourseId\r\nfrom Course inner join StudentCourseDownloaded \r\non Course.id = StudentCourseDownloaded.CourseId \r\ninner join StudentPackage on StudentCourseDownloaded.StudentId= StudentPackage.StudentId\r\ninner join OfflinePackage on OfflinePackage.Id=StudentPackage.PackageId\r\n\r\nwhere course.id=" + items.Id + syntaxx + "group by course.Id, OfflinePackage.IsShowInMobile ,\r\n OfflinePackage.PackageEndDate  ,\r\nDATEDIFF(day, OfflinePackage.PackageEndDate, getdate()),\r\n\r\nCourse.CurrentCost ,\r\ncourse.Id ");
                            if (xx != null)
                            {
                                var obj = new DownloadObj();
                                obj.Id = xx.Id;
                                obj.CanDownload = xx.CanDownload;
                                obj.IsDownloaded = xx.IsDownloaded;
                                obj.CourseId = xx.CourseId;
                                obj.Price = xx.Price;
                                obj.RemainingDays = xx.RemainingDays;
                                obj.ValidToDownloadDate = xx.ValidToDownloadDate;
                                items.DownloadObj = obj;
                            }
                            else
                            {
                                var obj = new DownloadObj();
                                obj.Id = 1;
                                obj.CanDownload = items.Content != null ? true : false;
                                obj.IsDownloaded = false;
                                obj.CourseId = items.Id;
                                obj.Price = 55;
                                obj.RemainingDays = 25;
                                obj.ValidToDownloadDate = DateTime.Now.AddDays(25);
                                items.DownloadObj = obj;
                            }
                        }
                        item.Courses = Body;
                    }


                }

                return Header;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //and CourseId  in (select CourseId from StudentCourseDownloaded where Studentid="+StudentId+")
        public async Task<IEnumerable<MyCourse>> GetAvailableCoursesToDownload(long StudentId, int Page)
        {
            try
            {
                Page = Page * 30;
                var conn = _connectionFactory.GetConnection;
                string queryGetHeader = @"select distinct * from 
            (
            select SubCategory.Name as SubCategoryName ,SubCategory.NameLT as SubCategoryNameLT
                        ,[Subject].Id as SubjectId,[Subject].Name as SubjectName,[Subject].NameLT as SubjectNameLT
                        from Subject join Track on Subject.Id=Track.SubjectId
                        join Department on Department.Id=Subject.DepartmentId 
                        join SubCategory on SubCategory.Id=Department.SubCategoryId 
                        join Course on Course.TrackId=Track.Id
            where  Course.Id In (select Distinct CourseId from StudentCourse where StudentId=" + StudentId + ")) NewTable order by NewTable.SubjectId OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY";
                var Header = await conn.QueryAsync<MyCourse>(queryGetHeader);

                foreach (var item in Header)
                {
                    string sqlBody = @"select myCourse.* , (select Name from Teacher where Id=Track.TeacherId) as TeacherName,
(select  Count(StudentContent.Id)  from Content left Join StudentContent  on Content.Id=StudentContent.ContentId 
left join  [Group] on [Group].Id=Content.GroupId left join Course on [Group].CourseId=Course.Id where Course.Id=myCourse.Id  " +
                    " And StudentContent.StudentId=" + StudentId + ") ViewedContent," +
                    "(select  Count(Content.Id)  from Content  left join  [Group] on [Group].Id=Content.GroupId " +
                    " left join Course on [Group].CourseId=Course.Id join StudentCourse on StudentCourse.CourseId=Course.Id  where Course.Id=myCourse.Id   And StudentCourse.StudentId=" + StudentId + ")" +
                    @" as ContentCount from Subject join Track on Subject.Id=Track.SubjectId 
join Department on Department.Id=Subject.DepartmentId  
join SubCategory on SubCategory.Id=Department.SubCategoryId 
join Course as myCourse on myCourse.TrackId=Track.Id            
where myCourse.IsAllowToDownload=1 and myCourse.CourseStatusId=3 And myCourse.Id In (select Distinct CourseId from StudentCourse where StudentId=" + StudentId + "" +
                    "  And CourseId Not IN (select Course.Id from TrackSubscription join Course on TrackSubscription.TrackId=Course.TrackId where TrackSubscription.TrackId=Track.Id And StudentId=" + StudentId + " and DurationExpiration< getdate()) ) and myCourse.Id  not in (select CourseId from StudentCourseDownloaded where Studentid=" + StudentId+") And [Subject].Id=" + item.SubjectId + "";
                    var Body = await conn.QueryAsync<Course>(sqlBody);
                    if (Body.Count() > 0)
                    {
                        foreach (var items in Body)
                        {
                            var syntaxx = "";
                            if (StudentId != null)
                            {
                                syntaxx = "and StudentCourseDownloaded.studentid=" + StudentId;
                            }
                            var xx = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<DownloadObj>("select course.Id as Id, OfflinePackage.IsShowInMobile as CanDownload,\r\n case when count(StudentCourseDownloaded.StudentId) >= 0 then 1 else 0 end  as IsDownloaded,\r\nOfflinePackage.PackageEndDate  as ValidToDownloadDate,\r\nDATEDIFF(day, getdate(),OfflinePackage.PackageEndDate) AS RemainingDays,\r\n\r\nCourse.CurrentCost as Price,\r\ncourse.Id as CourseId\r\nfrom Course inner join StudentCourseDownloaded \r\non Course.id = StudentCourseDownloaded.CourseId \r\ninner join StudentPackage on StudentCourseDownloaded.StudentId= StudentPackage.StudentId\r\ninner join OfflinePackage on OfflinePackage.Id=StudentPackage.PackageId\r\n\r\nwhere course.id=" + items.Id + syntaxx + "group by course.Id, OfflinePackage.IsShowInMobile ,\r\n OfflinePackage.PackageEndDate  ,\r\nDATEDIFF(day, OfflinePackage.PackageEndDate, getdate()),\r\n\r\nCourse.CurrentCost ,\r\ncourse.Id ");
                            if (xx != null)
                            {
                                var obj = new DownloadObj();
                                obj.Id = xx.Id;
                                obj.CanDownload = xx.CanDownload;
                                obj.IsDownloaded = xx.IsDownloaded;
                                obj.CourseId = xx.CourseId;
                                obj.Price = xx.Price;
                                obj.RemainingDays = xx.RemainingDays;
                                obj.ValidToDownloadDate = xx.ValidToDownloadDate;
                                items.DownloadObj = obj;
                            }
                            else
                            {
                                var obj = new DownloadObj();
                                obj.Id = 1;
                                obj.CanDownload = items.Content != null ? true : false;
                                obj.IsDownloaded = false;
                                obj.CourseId = items.Id;
                                obj.Price = 55;
                                obj.RemainingDays = 25;
                                obj.ValidToDownloadDate = DateTime.Now.AddDays(25);
                                items.DownloadObj = obj;
                            }
                        }
                        item.Courses = Body;
                    }


                }

                return Header;

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<IEnumerable<MyCourse>> GetDownloadedCourses(long StudentId, int Page)
        {
            try
            {
                Page = Page * 30;
                var conn = _connectionFactory.GetConnection;
                string queryGetHeader = @"select distinct * from 
            (
            select SubCategory.Name as SubCategoryName ,SubCategory.NameLT as SubCategoryNameLT
                        ,[Subject].Id as SubjectId,[Subject].Name as SubjectName,[Subject].NameLT as SubjectNameLT
                        from Subject join Track on Subject.Id=Track.SubjectId
                        join Department on Department.Id=Subject.DepartmentId 
                        join SubCategory on SubCategory.Id=Department.SubCategoryId 
                        join Course on Course.TrackId=Track.Id
            where Course.Id  In (select Distinct CourseId from StudentCourseDownloaded where StudentId=" + StudentId + ") and  Course.Id In (select Distinct CourseId from StudentCourse where StudentId=" + StudentId + ")) NewTable order by NewTable.SubjectId OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY";
                var Header = await conn.QueryAsync<MyCourse>(queryGetHeader);

                foreach (var item in Header)
                {
                    string sqlBody = @"select myCourse.* , (select Name from Teacher where Id=Track.TeacherId) as TeacherName,
(select  Count(StudentContent.Id)  from Content left Join StudentContent  on Content.Id=StudentContent.ContentId 
left join  [Group] on [Group].Id=Content.GroupId left join Course on [Group].CourseId=Course.Id where Course.Id=myCourse.Id  " +
                    " And StudentContent.StudentId=" + StudentId + ") ViewedContent," +
                    "(select  Count(Content.Id)  from Content  left join  [Group] on [Group].Id=Content.GroupId " +
                    " left join Course on [Group].CourseId=Course.Id join StudentCourse on StudentCourse.CourseId=Course.Id  where Course.Id=myCourse.Id   And StudentCourse.StudentId=" + StudentId + ")" +
                    @" as ContentCount from Subject join Track on Subject.Id=Track.SubjectId 
join Department on Department.Id=Subject.DepartmentId  
join SubCategory on SubCategory.Id=Department.SubCategoryId 
join Course as myCourse on myCourse.TrackId=Track.Id            
where myCourse.CourseStatusId=3 And myCourse.Id In (select Distinct CourseId from StudentCourse where StudentId=" + StudentId + "" +
                    " And CourseId Not IN (select Course.Id from TrackSubscription join Course on TrackSubscription.TrackId=Course.TrackId where TrackSubscription.TrackId=Track.Id And StudentId=" + StudentId + " and DurationExpiration< getdate()) ) and myCourse.Id  in (select CourseId from StudentCourseDownloaded where Studentid=" + StudentId+") And [Subject].Id=" + item.SubjectId + "";
                    var Body = await conn.QueryAsync<Course>(sqlBody);
                    if (Body.Count() > 0)
                    {
                        foreach (var items in Body)
                        {
                            var syntaxx = "";
                            if (StudentId != null)
                            {
                                syntaxx = "and StudentCourseDownloaded.studentid=" + StudentId;
                            }
                            var xx = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<DownloadObj>("select course.Id as Id, OfflinePackage.IsShowInMobile as CanDownload,\r\n case when count(StudentCourseDownloaded.StudentId) >= 0 then 1 else 0 end  as IsDownloaded,\r\nOfflinePackage.PackageEndDate  as ValidToDownloadDate,\r\nDATEDIFF(day, getdate(),OfflinePackage.PackageEndDate) AS RemainingDays,\r\n\r\nCourse.CurrentCost as Price,\r\ncourse.Id as CourseId\r\nfrom Course inner join StudentCourseDownloaded \r\non Course.id = StudentCourseDownloaded.CourseId \r\ninner join StudentPackage on StudentCourseDownloaded.StudentId= StudentPackage.StudentId\r\ninner join OfflinePackage on OfflinePackage.Id=StudentPackage.PackageId\r\n\r\nwhere course.id=" + items.Id + syntaxx + "group by course.Id, OfflinePackage.IsShowInMobile ,\r\n OfflinePackage.PackageEndDate  ,\r\nDATEDIFF(day, OfflinePackage.PackageEndDate, getdate()),\r\n\r\nCourse.CurrentCost ,\r\ncourse.Id ");
                            if (xx != null)
                            {
                                var obj = new DownloadObj();
                                obj.Id = xx.Id;
                                obj.CanDownload = xx.CanDownload;
                                obj.IsDownloaded = xx.IsDownloaded;
                                obj.CourseId = xx.CourseId;
                                obj.Price = xx.Price;
                                obj.RemainingDays = xx.RemainingDays;
                                obj.ValidToDownloadDate = xx.ValidToDownloadDate;
                                items.DownloadObj = obj;
                            }
                            else
                            {
                                var obj = new DownloadObj();
                                obj.Id = 1; 
                                obj.CanDownload = items.Content != null ? true : false;
                                obj.IsDownloaded = false;
                                obj.CourseId = items.Id;
                                obj.Price = 55;
                                obj.RemainingDays = 25;
                                obj.ValidToDownloadDate = DateTime.Now.AddDays(25);
                                items.DownloadObj = obj;
                            }
                        }
                        item.Courses = Body;
                    }


                }

                return Header;

            }
            catch (Exception e)
            {
                throw e;
            }
            /*
            Page = Page * 30;
                var conn = _connectionFactory.GetConnection;
                var courses = await _connectionFactory.GetConnection.QueryAsync<Course>("select distinct Course.* from Course inner join StudentCourseDownloaded on Course.Id=StudentCourseDownloaded.CourseId where StudentCourseDownloaded.StudentId="+StudentId+"");
                var res = new List<MyCourse>();
                foreach (var item in courses)
                {

                    res.Add(new MyCourse()
                    {
                        Courses = item
                    }) ;
                }
                return res;
             
             */
        }

        public async Task<IEnumerable<Student>> GetAllStudentForRenewSubscription()
        {
            var Students = await _connectionFactory.GetConnection.QueryAsync<Student>(@"select Student.* from TrackSubscription
 join Student on Student.Id=TrackSubscription.StudentId
 where DATEDIFF(DAY,getdate(),DurationExpiration)=3");

            return Students;
        }
        public async Task<IEnumerable<CourseContentLinks>> GetCourseContentLinks(long courseId)
        {
            var Students = await _connectionFactory.GetConnection.QueryAsync<CourseContentLinks>(@"select  'test' as CourseName,'test' as VideoPath,'OTP' as OTP,'playbackInfo' as PlaybackInfo");

            return Students;
        }

        
    }
}
