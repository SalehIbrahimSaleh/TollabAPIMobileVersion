using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CourseRepository : GenericRepository<Course>
    {
       

        public async Task<Course> GetCoursesById(long CourseId, long StudentId)
        {
            try
            {
                string query =
               @"select Distinct * from (
select OuterCourse.*,IsNull((select (select sum(ContentCourseTrackUser.minutecount) from ContentCourseTrackUser where UserId= " + StudentId + " and courseId="+CourseId+") * 100/(SELECT  sum ([Duration])  FROM [dbo].[Content] inner join  [Group] g on Content.GroupId=g.Id inner join course on g.courseid= course.Id where course.id="+CourseId+ ") ),'0') as CourseAchivementPercentage, IsNull((select ShowWaterMark from Track where Id=OuterCourse.TrackId),'0' ) as TrackShowWaterMark ,IsNull((select top 1 '1' from Favourite where StudentId=" + StudentId + " And CourseId=OuterCourse.Id),'0' ) as IsFavourite, "+
"IsNull((select top 1 '1'  from StudentCourse where StudentId = " + StudentId+" And CourseId = OuterCourse.Id),'0' ) as Enrollment, "+
@"(select Count(Content.Id) from Content join [Group] on [Group].Id = Content.GroupId join Course on [Group].CourseId = Course.Id where Content.ContentTypeId = 1 and Course.Id = OuterCourse.Id) as VideoCount, 
0 as ShouldAnswerExam,
(select Count(Content.Id) from Content join [Group] on [Group].Id = Content.GroupId join Course on [Group].CourseId = Course.Id where Content.ContentTypeId = 2 and Course.Id = OuterCourse.Id ) as FilesCount, 
(select IsNull(CAST(Sum(Content.Duration) as decimal) / 60, 0) from Content join [Group] on [Group].Id = Content.GroupId join Course on [Group].CourseId = Course.Id where Content.ContentTypeId = 1 and Course.Id = OuterCourse.Id ) as HoursCount,
(select Teacher.Name from Teacher join Track on Teacher.Id = Track.TeacherId join Course on Track.Id = Course.TrackId where Track.Isactive=1 and Course.Id = OuterCourse.Id) as TeacherName ,
(select Teacher.Id from Teacher join Track on Teacher.Id = Track.TeacherId join Course on Track.Id = Course.TrackId where Track.Isactive=1 and Course.Id = OuterCourse.Id) AS TeacherId
,(select BySubscription from Track join Course on  Track.Id=Course.TrackId where Track.Isactive=1 and Course.Id=OuterCourse.Id) as BySubscription
 from Course as OuterCourse left join[Group] on OuterCourse.Id =[Group].CourseId left join Content On[Group].Id = Content.GroupId where OuterCourse.Id = " + CourseId + ") newTable";
                var CourseData = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<Course>(query);

                if (CourseData != null)
                {
                    if(CourseData.AnswerExam == true)
                    {
                        var examObj = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<ExamsPerCourse>("SELECT(select count(*) from exam where courseid="+CourseData.Id+ " and ExamTypeId=1 and Publish=1 and enddate>startdate and DeadlineDate>GETDATE()) AS ActualExams, (select count(*) from StudentExam where StudentExam.ExamId in (select exam.Id from exam where CourseId=" + CourseData.Id+ " and ExamTypeId=1 and Publish=1 and enddate>startdate and DeadlineDate>GETDATE()) and SolveStatusId <>0 and studentid=" + StudentId+") AS SolvedExams\r\n");
                        if (examObj.ActualExams != examObj.SolvedExams)
                        {
                            CourseData.ShouldAnswerExam = true;
                        }
                        else
                        {
                            CourseData.ShouldAnswerExam = false;
                        }
                    }
                    var syntaxx = "";
                    if (StudentId != null)
                    {
                        syntaxx = "and StudentCourseDownloaded.studentid=" + StudentId;
                    }
                    var xx = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<DownloadObj>("select course.Id as Id, OfflinePackage.IsShowInMobile as CanDownload,\r\n case when count(StudentCourseDownloaded.StudentId) >= 0 then 1 else 0 end  as IsDownloaded,\r\nOfflinePackage.PackageEndDate  as ValidToDownloadDate,\r\nDATEDIFF(day, getdate(),OfflinePackage.PackageEndDate) AS RemainingDays,\r\n\r\nCourse.CurrentCost as Price,\r\ncourse.Id as CourseId\r\nfrom Course inner join StudentCourseDownloaded \r\non Course.id = StudentCourseDownloaded.CourseId \r\ninner join StudentPackage on StudentCourseDownloaded.StudentId= StudentPackage.StudentId\r\ninner join OfflinePackage on OfflinePackage.Id=StudentPackage.PackageId\r\n\r\nwhere course.id="+CourseData.Id + syntaxx + "\r\ngroup by course.Id, OfflinePackage.IsShowInMobile ,\r\n OfflinePackage.PackageEndDate  ,\r\nDATEDIFF(day, OfflinePackage.PackageEndDate, getdate()),\r\n\r\nCourse.CurrentCost ,\r\ncourse.Id ");
                    if (xx != null)
                    {
                        var obj = new DownloadObj();
                        obj.Id = xx.Id;
                        obj.CanDownload = xx.CanDownload;
                        obj.IsDownloaded = xx.IsDownloaded;
                        obj.CourseId = CourseId;
                        obj.Currency = "EGP";
                        obj.Price = xx.Price;
                        obj.RemainingDays = xx.RemainingDays;
                        obj.ValidToDownloadDate = xx.ValidToDownloadDate;
                        CourseData.DownloadObj = obj;
                    }
                    else
                    {
                        var obj = new DownloadObj();
                        obj.Id = 1;
                        obj.CanDownload = CourseData.Content != null ? true : false;
                        obj.IsDownloaded = false;
                        obj.CourseId = CourseId;
                        obj.Currency = "SR";
                        obj.Price = 55;
                        obj.RemainingDays = 25;
                        obj.ValidToDownloadDate = DateTime.Now.AddDays(25);
                        CourseData.DownloadObj = obj;
                    }
                }

                return CourseData;

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<Course> GetCoursesWithContentById(long CourseId, long StudentId,long ContentId)
        {
            try
            {
                string query =
               @"select Distinct * from (
select OuterCourse.* ,IsNull((select '1' from Favourite where StudentId=" + StudentId + " And CourseId=OuterCourse.Id),'0' ) as IsFavourite, " +
"IsNull((select top 1 '1'  from StudentCourse where StudentId = " + StudentId + " And CourseId = OuterCourse.Id),'0' ) as Enrollment, " +
@"(select Count(Content.Id) from Content join [Group] on [Group].Id = Content.GroupId join Course on [Group].CourseId = Course.Id where Content.ContentTypeId = 1 and Course.Id = OuterCourse.Id) as VideoCount, 
(select Count(Content.Id) from Content join [Group] on [Group].Id = Content.GroupId join Course on [Group].CourseId = Course.Id where Content.ContentTypeId = 2 and Course.Id = OuterCourse.Id ) as FilesCount, 
(select IsNull(CAST(Sum(Content.Duration) as decimal) / 60, 0) from Content join [Group] on [Group].Id = Content.GroupId join Course on [Group].CourseId = Course.Id where Content.ContentTypeId = 1 and Course.Id = OuterCourse.Id ) as HoursCount,
(select Teacher.Name from Teacher join Track on Teacher.Id = Track.TeacherId join Course on Track.Id = Course.TrackId where Track.Isactive=1 and Course.Id = OuterCourse.Id) as TeacherName ,
(select Teacher.Id from Teacher join Track on Teacher.Id = Track.TeacherId join Course on Track.Id = Course.TrackId where Track.Isactive=1 and Course.Id = OuterCourse.Id) AS TeacherId
,(select BySubscription from Track join Course on  Track.Id=Course.TrackId where Track.Isactive=1 and Course.Id=OuterCourse.Id) as BySubscription
 from Course as OuterCourse left join[Group] on OuterCourse.Id =[Group].CourseId left join Content On[Group].Id = Content.GroupId where OuterCourse.Id = " + CourseId + ") newTable";
                var CourseData = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<Course>(query);
                if (CourseData != null)
                {
                    var ContentData = _connectionFactory.GetConnection.QueryFirstOrDefault<Content>("select * from Content where Id=" + ContentId + "");

                    CourseData.Content = ContentData;
                }


                return CourseData;

            }
            catch (Exception e)
            {

                throw e;
            }
        }


        public async Task<CoursesByTrackIdModel> GetCoursesByTrackId(long TrackId, long studentId)
        {
            try
            {
                var query =
                    @"select case when (select COUNT(*) from StudentCourse sc inner join Course c on sc.CourseId = c.Id  where  c.TrackId = " +
                    TrackId + @" and sc.StudentId = " + studentId + @") > 0 then 1 else 0 end as IsEnrolled, 1 as IsAllowToShow ,
                                 Track.WhatsupGroupLink,
                                 Category.Name as CategoryName,Category.NameLT as CategoryNameLT,SubCategory.Name SubCategoryName ,
                            SubCategory.NameLT SubCategoryNameLT
							,Track.Id as TrackId,Track.Name as TrackName,,Track.SKUNumber,Track.SKUPrice,Track.OldSKUPrice,Track.NameLT as TrackNameLT,Track.ImageHomeCover as TrackImage, Track.SubscriptionCurrentPrice, Track.SubscriptionOldPrice,
                         (select Name from Teacher where Id=Track.TeacherId) as TeacherName
                           from Subject join Track on Subject.Id=Track.SubjectId
                           join Department on Department.Id=Subject.DepartmentId 
						   join SubCategory on SubCategory.Id=Department.SubCategoryId 
						   join Category on Category.Id=SubCategory.CategoryId
                           where Track.Isactive=1 and Track.Id=" + TrackId + " ";
                var result =
                    await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<CoursesByTrackIdModel>(query);
                if (result != null)
                {
                    //                (select Count(StudentId) from StudentCourse where CourseId=OuterCourse.Id) CountStudentEnrolled,

                    var courses = await GetAllByQuery(@"
                select Distinct * from ( 
                select  OuterCourse.*,
              ( select Count(StudentCourse.StudentId) as StudentCount from StudentCourse join Course on  StudentCourse.CourseId=Course.Id join Track on Track.Id=Course.TrackId where Track.Isactive=1 and StudentCourse.CourseId=OuterCourse.Id) as CountStudentEnrolled,
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3) as VideoCount, 
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=2 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3 ) as FilesCount, 
                (select IsNull( CAST(Sum(Content.Duration) as decimal )/60,0) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3) as HoursCount
                from Course as OuterCourse left join [Group] on [Group].CourseId=OuterCourse.Id where OuterCourse.TrackId=" +
                                                      TrackId +
                                                      " And OuterCourse.CourseStatusId=3  ) newTable  Order By OrderNumber");
                    result.Courses = courses;
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<CoursesByTrackIdModel> GetCoursesByTrackId(long TrackId,string studentIdenId=null,long? studentId=null)

        {
            try
            {
                var query = "";
                if(studentIdenId == null)
                {
                    query =
                    @"select  
                           0 as HasOffers,  Track.WhatsupGroupLink, Category.Name as CategoryName,Category.NameLT as CategoryNameLT,SubCategory.Name SubCategoryName ,
                            SubCategory.NameLT SubCategoryNameLT
							,Track.Id as TrackId,Track.SKUNumber,Track.SKUPrice,Track.OldSKUPrice, Track.Name as TrackName,Track.NameLT as TrackNameLT,Track.ImageHomeCover as TrackImage,Track.SubscriptionCurrentPrice, Track.SubscriptionOldPrice,
                         (select Name from Teacher where Id=Track.TeacherId) as TeacherName
                           from Subject join Track on Subject.Id=Track.SubjectId
                           join Department on Department.Id=Subject.DepartmentId 
						   join SubCategory on SubCategory.Id=Department.SubCategoryId 
						   join Category on Category.Id=SubCategory.CategoryId
                           where Track.Isactive=1 and Track.Id=" + TrackId + " ";
                }
                else
                {
                    query =
                    @"select 0 as HasOffers, case when (select COUNT(*) from StudentCourse sc inner join Course c on sc.CourseId = c.Id  inner join student s on sc.studentid=s.id where  c.TrackId = " +
                    TrackId + @" and s.IdentityId = '" + studentIdenId + @"') > 0 then 1 else 0 end as IsEnrolled,  1 as IsAllowToShow,
(select  Count(StudentContent.Id)   from Content inner Join StudentContent  on Content.Id=StudentContent.ContentId  inner join  [Group] on [Group].Id=Content.GroupId  
inner join Course on [Group].CourseId=Course.Id  where Course.Id IN (select id from course where TrackId="+TrackId+" ) " +
") as ViewsCount, (select (select sum(ContentCourseTrackUser.minutecount) from ContentCourseTrackUser  inner join student s on ContentCourseTrackUser.UserId=s.id where s.IdentityId= '" + studentIdenId +"' and trackid=" + TrackId + ") * 100/(SELECT  sum ([Duration])  FROM [dbo].[Content] inner join  [Group] g on Content.GroupId=g.Id inner join course on g.courseid= course.Id   inner join Track on Course.TrackId= track.Id where track.id=" + TrackId + ") ) as CourseAchivementPercentage, Track.WhatsupGroupLink, Category.Name as CategoryName," +
"Category.NameLT as CategoryNameLT,SubCategory.Name SubCategoryName , SubCategory.NameLT SubCategoryNameLT ,Track.Id as TrackId,Track.SKUNumber,Track.SKUPrice,Track.OldSKUPrice," +
"Track.Name as TrackName,Track.NameLT as TrackNameLT,Track.ImageHomeCover as TrackImage, (select Name from Teacher where Id=Track.TeacherId) as TeacherName from Subject join Track on Subject.Id=Track.SubjectId join Department on Department.Id=Subject.DepartmentId join SubCategory on SubCategory.Id=Department.SubCategoryId join Category on Category.Id=SubCategory.CategoryId where Track.Isactive=1 and Track.Id=" + TrackId + "";

                }
                var result =
                    await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<CoursesByTrackIdModel>(query);
                if (result != null)
                {
                    //                (select Count(StudentId) from StudentCourse where CourseId=OuterCourse.Id) CountStudentEnrolled,

                    
                    if(studentIdenId == null)
                    {
                        var courses = await GetAllByQuery(@"
                select Distinct * from ( 
                select  OuterCourse.*,
              ( select Count(StudentCourse.StudentId) as StudentCount from StudentCourse join Course on  StudentCourse.CourseId=Course.Id join Track on Track.Id=Course.TrackId where Track.Isactive=1 and StudentCourse.CourseId=OuterCourse.Id) as CountStudentEnrolled,
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3) as VideoCount, 
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=2 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3 ) as FilesCount, 
                (select IsNull( CAST(Sum(Content.Duration) as decimal )/60,0) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3) as HoursCount
                from Course as OuterCourse left join [Group] on [Group].CourseId=OuterCourse.Id where OuterCourse.TrackId=" + TrackId + "   ) newTable  Order By OrderNumber");
                        result.Courses = courses;
                       
                    }
                
                    else
                    {
                       var courses= await GetAllByQuery(@"
                select Distinct * from ( 
                select  OuterCourse.*,0 as 'IsShowingNow' ,
              ( select Count(StudentCourse.StudentId) as StudentCount from StudentCourse join Course on  StudentCourse.CourseId=Course.Id join Track on Track.Id=Course.TrackId where Track.Isactive=1 and StudentCourse.CourseId=OuterCourse.Id) as CountStudentEnrolled,
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3) as VideoCount, 
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=2 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3 ) as FilesCount, 
                (select  Count(StudentContent.Id)   from Content inner Join StudentContent  on Content.Id=StudentContent.ContentId  inner join  [Group] on [Group].Id=Content.GroupId  inner join Course on [Group].CourseId=Course.Id where Course.Id=OuterCourse.Id )  as ViewsCount,(select IsNull( CAST(Sum(Content.Duration) as decimal )/60,0) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3) as HoursCount " +
                "from Course as OuterCourse left join [Group] on [Group].CourseId=OuterCourse.Id where OuterCourse.TrackId=" + TrackId + " And OuterCourse.CourseStatusId=3  ) newTable  Order By OrderNumber");
                        result.Courses = courses;
                    }
                 }
               if (result.Courses != null )
                {
                    foreach (var item in result.Courses)
                    {
                        var syntaxx = "";
                        if(studentIdenId != null)
                        {
                            syntaxx= "and StudentCourseDownloaded.studentid=" + studentId;
                        }
                        var xx = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<DownloadObj>("select course.Id as Id, OfflinePackage.IsShowInMobile as CanDownload,\r\n case when count(StudentCourseDownloaded.StudentId) >= 0 then 1 else 0 end  as IsDownloaded,\r\nOfflinePackage.PackageEndDate  as ValidToDownloadDate,\r\nDATEDIFF(day, getdate(),OfflinePackage.PackageEndDate) AS RemainingDays,\r\n\r\nCourse.CurrentCost as Price,\r\ncourse.Id as CourseId\r\nfrom Course inner join StudentCourseDownloaded \r\non Course.id = StudentCourseDownloaded.CourseId \r\ninner join StudentPackage on StudentCourseDownloaded.StudentId= StudentPackage.StudentId\r\ninner join OfflinePackage on OfflinePackage.Id=StudentPackage.PackageId\r\n\r\nwhere course.id=" + item.Id + syntaxx+ "group by course.Id, OfflinePackage.IsShowInMobile ,\r\n OfflinePackage.PackageEndDate  ,\r\nDATEDIFF(day, OfflinePackage.PackageEndDate, getdate()),\r\n\r\nCourse.CurrentCost ,\r\ncourse.Id ");
                        if (xx != null)
                        {
                            var obj = new DownloadObj();
                            obj.Id = xx.Id;
                            obj.CanDownload = xx.CanDownload;
                            obj.IsDownloaded = xx.IsDownloaded; 
                            obj.CourseId = xx.CourseId;
                            obj.Currency = "EGP";
                            obj.Price = xx.Price;
                            obj.RemainingDays = xx.RemainingDays;
                            obj.ValidToDownloadDate = xx.ValidToDownloadDate;
                            item.DownloadObj=obj;
                        }
                        else
                        {
                            var obj = new DownloadObj();
                            obj.Id = 1;
                            obj.CanDownload = item.Content != null ? true : false;
                            obj.IsDownloaded = false;
                            obj.CourseId = item.Id;
                            obj.Currency = "SR";
                            obj.Price = 55;
                            obj.RemainingDays = 25;
                            obj.ValidToDownloadDate = DateTime.Now.AddDays(25);
                            item.DownloadObj = obj;
                        }
                    }

                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<CoursesByTrackIdModel> GetAllCoursesByTrackIdForTeacher(long TrackId)
        {
            try
            {
                string query = @"select Category.Name as CategoryName,Category.NameLT as CategoryNameLT,SubCategory.Name SubCategoryName ,
                            SubCategory.NameLT SubCategoryNameLT
							,Track.Id as TrackId,Track.Name as TrackName,Track.NameLT as TrackNameLT,Track.ImageHomeCover as TrackImage,Track.SubscriptionCurrentPrice, Track.SubscriptionOldPrice,
                         (select Name from Teacher where Id=Track.TeacherId) as TeacherName
                           from Subject join Track on Subject.Id=Track.SubjectId
                           join Department on Department.Id=Subject.DepartmentId 
						   join SubCategory on SubCategory.Id=Department.SubCategoryId 
						   join Category on Category.Id=SubCategory.CategoryId
                           where Track.Isactive=1 and Track.Id=" + TrackId + " ";
                var result = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<CoursesByTrackIdModel>(query);
                if (result != null)
                {
                    //                (select Count(StudentId) from StudentCourse where CourseId=OuterCourse.Id) CountStudentEnrolled,

                    var courses = await GetAllByQuery(@"
                select Distinct * from ( 
                select  OuterCourse.*,
              ( select Count(StudentCourse.StudentId) as StudentCount from StudentCourse join Course on  StudentCourse.CourseId=Course.Id join Track on Track.Id=Course.TrackId where Track.Isactive=1 and StudentCourse.CourseId=OuterCourse.Id) as CountStudentEnrolled,
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3) as VideoCount, 
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=2 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3 ) as FilesCount, 
                (select IsNull( CAST(Sum(Content.Duration) as decimal )/60,0) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id And OuterCourse.CourseStatusId=3) as HoursCount
                from Course as OuterCourse left join [Group] on [Group].CourseId=OuterCourse.Id where OuterCourse.TrackId=" + TrackId + "   ) newTable  Order By OrderNumber");
                    result.Courses = courses;
                }
                return result;

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<CoursesByTrackIdModel> GetCoursesByTrackIdForTeacher(long TrackId)
        {
            try
            {
                string query = @"select Category.Name as CategoryName,Category.NameLT as CategoryNameLT,SubCategory.Name SubCategoryName ,
                            SubCategory.NameLT SubCategoryNameLT
							,Track.Id as TrackId,Track.Name as TrackName,Track.NameLT as TrackNameLT,Track.ImageHomeCover as TrackImage,
                         (select Name from Teacher where Id=Track.TeacherId) as TeacherName
                           from Subject join Track on Subject.Id=Track.SubjectId
                           join Department on Department.Id=Subject.DepartmentId 
						   join SubCategory on SubCategory.Id=Department.SubCategoryId 
						   join Category on Category.Id=SubCategory.CategoryId
                           where Track.Isactive=1 and Track.Id=" + TrackId + "";
                var result = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<CoursesByTrackIdModel>(query);
                if (result != null)
                {
                    var courses = await GetAllByQuery(@"
              select Distinct * from ( select  OuterCourse.*,
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id And Course.CourseStatusId!=3) as VideoCount, 
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=2 and  Course.Id=OuterCourse.Id And Course.CourseStatusId!=3) as FilesCount, 
                (select IsNull( CAST(Sum(Content.Duration) as decimal )/60,0) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id And Course.CourseStatusId!=3 ) as HoursCount
                from Course as OuterCourse left join [Group] on [Group].CourseId=OuterCourse.Id where OuterCourse.TrackId=" + TrackId + " And OuterCourse.CourseStatusId!=3  ) newTable");
                    result.Courses = courses;
                }
                return result;

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<List<Group>> GetGroupsWithContentsByCourseIdForStudent(long CourseId, long StudentId, int Page,long ContentId)
        {
            try
            {
                string AndContent = "";
                if (ContentId>0)
                {
                    AndContent = " And NewContent.Id=" + ContentId + "";
                }
                Page = Page * 3;
                if (Page > 0)
                {
                    var GroupList = new List<Group>();
                    return GroupList;
                }

                //    string sql1 = "select *,(select '1' from StudentContent where StudentContent.ContentId=Content.Id And StudentContent.StudentId=" + StudentId + ") as IsViewed  from  [Group]  join Content on Content.GroupId=[Group].Id  where [Group].CourseId=" + CourseId + "";
                string sql = @"
BEGIN
    DECLARE @enrolled INT;
    SELECT 
        @enrolled= Isnull(Student.Id,0)
    FROM
       Student  
        INNER JOIN StudentCourse   ON StudentCourse.StudentId = Student.Id where StudentCourse.StudentId="+StudentId+" And StudentCourse.CourseId="+CourseId+""+
       @" IF @enrolled > 0
      BEGIN
select [Group].*, case when (select Count(*) from StudentExam se inner join Exam e on se.ExamId = e.Id 
where e.LockCourseContent = 1 and se.SolveStatusId = 1
and se.StudentId = " + StudentId + @" 
and e.CourseId = [Group].CourseId ) > 0 then 1 else 0 end as LockCourseContent, case when (select Count(*)  from course inner join Exam e on course.id = e.courseid left join StudentExam se on se.ExamId = e.Id
where  
 se.StudentId = " + StudentId + @" 
and e.CourseId = [Group].CourseId and se.Id is null and course.AnswerExam=1) > 0 then 0 else 1 end as AnswerExam, NewContent.Id,NewContent.ProviderType, NewContent.Path,NewContent.Name,NewContent.NameLT,NewContent.GroupId,NewContent.Duration,NewContent.ContentTypeId,'true' as IsFree ,(select  top 1 '1' from StudentContent where StudentContent.ContentId=NewContent.Id And StudentContent.StudentId=" +
                          StudentId + ") as IsViewed " +
                          " from  [Group]  join Content as NewContent on NewContent.GroupId=[Group].Id where [Group].CourseId=" +
                          CourseId + " Order By NewContent.OrderNumber " +
                          @" END
    ELSE
    BEGIN
	select [Group].*,NewContent.Id, NewContent.Name,NewContent.NameLT,NewContent.GroupId,NewContent.Duration,NewContent.ProviderType,NewContent.ContentTypeId,NewContent.IsFree,(select top 1 '1' from StudentContent where StudentContent.ContentId=NewContent.Id And StudentContent.StudentId=" + StudentId + ") as IsViewed,"+
    @"(select top 1 Content.Path from Content where IsFree=1 and GroupId=[Group].Id And Id=NewContent.Id ) as Path
  from  [Group]  join Content as NewContent on NewContent.GroupId=[Group].Id 
  where [Group].CourseId=" + CourseId + ""+AndContent+" Order By NewContent.OrderNumber " +
  @" END
    END";







                var GroupDictionary = new Dictionary<long, Group>();
                var list = _connectionFactory.GetConnection.Query<Group, Content, Group>(
                    sql,
                    (group, content) =>
                    {
                        Group groupEntry;

                        if (!GroupDictionary.TryGetValue(group.Id, out groupEntry))
                        {
                            groupEntry = group;
                            groupEntry.Contents = new List<Content>();
                            GroupDictionary.Add(groupEntry.Id, groupEntry);
                        }
                        groupEntry.Contents.Add(content);

                        return groupEntry;
                    },
                    splitOn: "Id")
                    .OrderBy(i=>i.OrderNumber)
                .Distinct()//.Skip(Page)
                .ToList();

                return list;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<IEnumerable<Course>> SearchAsync(string word,long studentCountry, int Page)
        {
            Page = Page * 30;

            string sql = @"select Teacher.Name as TeacherName, Course.* from Course join Track on 
            Track.Id=Course.TrackId join Teacher on Teacher.Id=Track.TeacherId
             where  Track.Isactive=1 and Course.CourseStatusId=3 And Teacher.CountryId=" + studentCountry+ " And ( Course.Name LIKE N'%" + word + "%' or Course.NameLT LIKE N'%" + word + "%' or Teacher.Name LIKE N'%" + word + "%' or Track.Name LIKE N'%" + word + "%' or Track.NameLT LIKE N'%" + word + "%') order by Course.Id  OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY";
            var result = await GetAllByQuery(sql);
            return result;
        }
      
        public async Task<IEnumerable<Course>> GetCoursesByDepartmentId(long DepartmentId, int Page)
        {
            Page = Page * 30;

            string sql = @"select Teacher.Name as TeacherName,Course.* from Course join Track on Course.TrackId=Track.Id
 join Subject on Track.SubjectId=Subject.Id join Department on Subject.DepartmentId=Department.Id join Teacher on Teacher.Id=Track.TeacherId
 where Track.Isactive=1 and Course.CourseStatusId=3 And Department.Id=" + DepartmentId + " order by Course.Id  OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY";
            var result = await GetAllByQuery(sql);
            return result;
        }

        public async Task<List<Group>> GetGroupsWithContentsByCourseId(long CourseId, int Page,long ContentId)
        {
            try
            {
                Page = Page * 3;
                if (Page>0)
                {
                    var GroupList = new List<Group>();
                    return GroupList;
                }
                string sql = "select *  from  [Group] left join Content on Content.GroupId=[Group].Id  where [Group].CourseId=" + CourseId + " ";
                if (ContentId>0)
                {
                    sql = sql + " And Content.Id=" + ContentId + "";
                }
                sql = sql + " order by Content.OrderNumber";
                var GroupDictionary = new Dictionary<long, Group>();
                var list = _connectionFactory.GetConnection.Query<Group, Content, Group>(
                    sql,
                    (group, content) =>
                    {
                        Group groupEntry;

                        if (!GroupDictionary.TryGetValue(group.Id, out groupEntry))
                        {
                            groupEntry = group;
                            groupEntry.Contents = new List<Content>();
                            GroupDictionary.Add(groupEntry.Id, groupEntry);
                        }

                        groupEntry.Contents.Add(content);

                        return groupEntry;
                    },
                    splitOn: "Id")
                .Distinct()//.Skip(Page)
                .ToList();

                return list;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<Course> GetCourseByIdForTeacher(long courseId, long TeacherId)
        {

            
            string sql = @" select Distinct * from ( select  OuterCourse.*,
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id) as VideoCount, 
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=2 and  Course.Id=OuterCourse.Id ) as FilesCount, 
                (select IsNull( CAST(Sum(Content.Duration) as decimal )/60,0) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id ) as HoursCount
             , ( select Count(StudentCourse.StudentId) as StudentCount from StudentCourse join Course on  StudentCourse.CourseId=Course.Id join Track on Track.Id=Course.TrackId where StudentCourse.CourseId=OuterCourse.Id And Track.Isactive=1 and Track.TeacherId=" + TeacherId + ") as CountStudentEnrolled " +
            " ,(select isnull( sum(Amount),0) from TeacherTransaction where CourseId=OuterCourse.Id And TeacherId=" + TeacherId + ") GainedMoney " +
             "  from Course as OuterCourse left join [Group] on [Group].CourseId=OuterCourse.Id where OuterCourse.Id=" + courseId + " ) newTable";
            var CourseData = await GetOneByQuery(sql);
            
            return CourseData;
        }

        public async  Task<Course> GetCourseByIdForTeacher(long courseId ,long TeacherId,long ContentId)
        {
            string sql = @" select Distinct * from ( select  OuterCourse.*,
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id) as VideoCount, 
                (select Count(Content.Id) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=2 and  Course.Id=OuterCourse.Id ) as FilesCount, 
                (select IsNull( CAST(Sum(Content.Duration) as decimal )/60,0) from Content join [Group] on [Group].Id=Content.GroupId join Course on [Group].CourseId=Course.Id where Content.ContentTypeId=1 and  Course.Id=OuterCourse.Id ) as HoursCount
             , ( select Count(StudentCourse.StudentId) as StudentCount from StudentCourse join Course on  StudentCourse.CourseId=Course.Id join Track on Track.Id=Course.TrackId where StudentCourse.CourseId=OuterCourse.Id And Track.Isactive=1 and Track.TeacherId=" + TeacherId + ") as CountStudentEnrolled " +
            " ,(select isnull( sum(Amount),0) from TeacherTransaction where CourseId=OuterCourse.Id And TeacherId="+ TeacherId + ") GainedMoney "+
	         "  from Course as OuterCourse left join [Group] on [Group].CourseId=OuterCourse.Id where OuterCourse.Id="+ courseId + " ) newTable";
            var CourseData = await GetOneByQuery(sql);
            if (CourseData != null)
            {
                var ContentData = _connectionFactory.GetConnection.QueryFirstOrDefault<Content>("select * from Content where Id=" + ContentId + "");

                CourseData.Content = ContentData;
            }
            return CourseData;
        }


        public async Task<bool> SubmitToReview(long CourseId)
        {
            var oldCourse = await Get(CourseId);
            if (oldCourse!=null)
            {
                oldCourse.CourseStatusId = 2;//submit to review 
                var update = await Update(oldCourse);
                return update;
            }
            return false;
        }

        public async Task<Course> AddCourseFullDescription(Course course)
        {
            var oldCourse = await Get(course.Id);
            if (oldCourse != null)
            {
                oldCourse.FullDescription = course.FullDescription;
                var update = await Update(oldCourse);
                if (update)
                {
                    return oldCourse;
                }
            }
            return null;
        }

    }
}
