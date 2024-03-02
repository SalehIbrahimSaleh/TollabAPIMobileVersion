using Dapper;
using DataAccess.DTOs;
using DataAccess.Entities;
using DataAccess.QueryResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class LiveRepository : GenericRepository<Live>
    {
        public async Task<IEnumerable<LiveQueryResult>> GetTopLives(long countryId, long studentId)
        {
            var courseLives = await GetCourseLives(countryId, studentId);
            var trackLives = await GetTrackLives(countryId, studentId);
            var otherLives = await GetOtherLives(countryId, studentId);
            return courseLives.Concat(trackLives).Concat(otherLives)
                .OrderByDescending(a => a.SubCategoryId).ThenByDescending(a => a.LiveAppearanceDate).Take(4);
        }

        public async Task<IEnumerable<LiveQueryResult>> GetLivesForTeacher(long countryId, long teacherId, int page = 0)
        {
            var courseLives = await GetCourseLivesForTeacher(countryId, teacherId);
            var trackLives = await GetTrackLivesForTeacher(countryId, teacherId);
            var otherLives = await GetOtherLivesForTeacher(countryId, teacherId);
            return courseLives.Concat(trackLives).Concat(otherLives)
                .OrderByDescending(a => a.SubCategoryId).ThenByDescending(a => a.LiveAppearanceDate).Skip(page * 4).Take(4);
        }

        public async Task<IEnumerable<LiveQueryResult>> GetLives(long countryId, long studentId, int page = 0)
        {
            var courseLives = await GetCourseLives(countryId, studentId);
            var trackLives = await GetTrackLives(countryId, studentId);
            var otherLives = await GetOtherLives(countryId, studentId);
            return courseLives.Concat(trackLives).Concat(otherLives)
                .OrderByDescending(a => a.SubCategoryId).ThenByDescending(a => a.LiveAppearanceDate).Skip(page * 4).Take(4);
        }

        public async Task<LiveQueryResult> GetLive(long id, long studentId)
        {
            string sql = @"
select Live.* , Live.Id as LiveId, Teacher.Name  as TeacherName , 
IsNull((select top 1 '1'  from StudentLive where StudentId = " + studentId + @" And LiveId = " + id + @"),'0' ) as Enrollment,
(select count(*) from StudentLive where LiveId = Live.Id) as SubscriptionCount
from Live inner join Teacher on Live.TeacherId = Teacher.Id
where  Live.IsActive=1 and Live.Id =" + id;

            var lives = await _connectionFactory.GetConnection.QueryAsync<LiveQueryResult>(sql);
            string liveAttachmentsSQl = "select Id , Name, Path, LiveId, OrderNumber from LiveAttachment where LiveId =" + id + " order by OrderNumber";

            var liveAttachemnts = await _connectionFactory.GetConnection.QueryAsync<LiveAttachment>(liveAttachmentsSQl);


            var liveEntity = lives.FirstOrDefault();

            if (liveEntity != null)
                liveEntity.LiveAttachments = liveAttachemnts;

            return liveEntity;
        }

        public async Task<LiveQueryResult> GetLiveForTeacher(long id, long teacherId)
        {
            string sql = @"
select Live.* , Live.Id as LiveId, Teacher.Name  as TeacherName , 
(select count(*) from StudentLive where LiveId = Live.Id) as SubscriptionCount
from Live inner join Teacher on Live.TeacherId = Teacher.Id
where  Live.IsActive=1 and Live.Id =" + id + " and Live.TeacherId = " + teacherId;

            var lives = await _connectionFactory.GetConnection.QueryAsync<LiveQueryResult>(sql);
            string liveAttachmentsSQl = "select Id , Name, Path, LiveId, OrderNumber from LiveAttachment where LiveId =" + id + " order by OrderNumber";

            var liveAttachemnts = await _connectionFactory.GetConnection.QueryAsync<LiveAttachment>(liveAttachmentsSQl);


            var liveEntity = lives.FirstOrDefault();

            if (liveEntity != null)
                liveEntity.LiveAttachments = liveAttachemnts;

            return liveEntity;
        }

        #region private methods
        private async Task<IEnumerable<LiveQueryResult>> GetCourseLives(long countryId, long studentId)
        {
            string sql = @"
select Live.Id as LiveId, Live.*, Teacher.Name  as TeacherName , 
Category.Id as CategoryId , Category.Name as CategoryName, Category.NameLT  as CategoryNameLT ,
SubCategory.Id as SubCategoryId , SubCategory.Name as SubCategoryName , SubCategory.NameLT as SubCategoryNameLT ,
IsNull((select top 1 '1'  from StudentLive where StudentId = " + studentId + @" And LiveId = Live.Id),'0' ) as Enrollment,
(select count(*) from StudentLive where LiveId = Live.Id) as SubscriptionCount
from Live inner join Teacher on Live.TeacherId = Teacher.Id join Course  on Live.CourseId = Course.Id 
join Track on Track.Id=Course.TrackId 
join Subject on Track.SubjectId=Subject.Id 
join Department on Department.Id=Subject.DepartmentId
join SubCategory on SubCategory.Id=Department.SubCategoryId join Category on Category.Id=SubCategory.CategoryId
join Section on Section.Id=Category.SectionId where Live.IsActive=1 and Live.LiveLinkType = 2  And (Section.CountryId=" + countryId + @" or Live.CountryId  = " + countryId + @")
and Live.LiveAppearanceDate <=  (select DATEADD(HOUR," + GetTimeZoneOffset(countryId) + @", (select getutcDate()))  ) and (select DATEADD(HOUR," + GetTimeZoneOffset(countryId) + @", (select getutcDate()))  ) <=  (select DATEADD(MINUTE,Live.Duration, Live.LiveDate)) order by Live.LiveAppearanceDate DESC;
";

            var lives = await _connectionFactory.GetConnection.QueryAsync<LiveQueryResult>(sql);
            return lives;
        }


        private async Task<IEnumerable<LiveQueryResult>> GetTrackLives(long countryId, long studentId)
        {
            string sql = @"
select Live.Id as LiveId, Live.*, Teacher.Name  as TeacherName , 
Category.Id as CategoryId , Category.Name as CategoryName, Category.NameLT  as CategoryNameLT ,
SubCategory.Id as SubCategoryId , SubCategory.Name as SubCategoryName , SubCategory.NameLT as SubCategoryNameLT ,
IsNull((select top 1 '1'  from StudentLive where StudentId = " + studentId + @" And LiveId = Live.Id),'0' ) as Enrollment,
(select count(*) from StudentLive where LiveId = Live.Id) as SubscriptionCount
from Live inner join Teacher on Live.TeacherId = Teacher.Id join Track  on Live.TrackId = Track.Id 
join Subject on Track.SubjectId=Subject.Id 
join Department on Department.Id=Subject.DepartmentId
join SubCategory on SubCategory.Id=Department.SubCategoryId join Category on Category.Id=SubCategory.CategoryId
join Section on Section.Id=Category.SectionId where Live.IsActive=1 and Live.LiveLinkType = 1  And (Section.CountryId=" + countryId + " or Live.CountryId  = " + countryId + @")
and Live.LiveAppearanceDate <=  (select DATEADD(HOUR," + GetTimeZoneOffset(countryId) + @", (select getutcDate()))  ) and (select DATEADD(HOUR," + GetTimeZoneOffset(countryId) + @", (select getutcDate()))  ) <=  (select DATEADD(MINUTE,Live.Duration, Live.LiveDate)) order by Live.LiveAppearanceDate DESC;
";

            var lives = await _connectionFactory.GetConnection.QueryAsync<LiveQueryResult>(sql);
            return lives;
        }


        private async Task<IEnumerable<LiveQueryResult>> GetOtherLives(long countryId, long studentId)
        {
            string sql = @"
select Live.Id as LiveId, Live.* , Teacher.Name  as TeacherName , -1 as CategoryId ,  
N'اخري' as CategoryName , N'Other' as CategoryNameLT , 
-1 as SubCategoryId , N'اخري' as SubCategoryName, N'Other' as SubCategoryNameLT ,
IsNull((select top 1 '1'  from StudentLive where StudentId = " + studentId + @" And LiveId = Live.Id),'0' ) as Enrollment,
(select count(*) from StudentLive where LiveId = Live.Id) as SubscriptionCount
from Live inner join Teacher on Live.TeacherId = Teacher.Id
where Live.IsActive=1 and Live.LiveLinkType  = 3 and Live.CountryId = " + countryId + @" and Live.LiveAppearanceDate <=  (select DATEADD(HOUR," + GetTimeZoneOffset(countryId) + @", (select getutcDate()))  ) and (select DATEADD(HOUR," + GetTimeZoneOffset(countryId) + @", (select getutcDate()))  ) <=  (select DATEADD(MINUTE,Live.Duration, Live.LiveDate))
order by Live.LiveAppearanceDate DESC;
";

            var lives = await _connectionFactory.GetConnection.QueryAsync<LiveQueryResult>(sql);
            return lives;
        }


        private async Task<IEnumerable<LiveQueryResult>> GetCourseLivesForTeacher(long countryId, long teacherId)
        {
            string sql = @"
select Live.Id as LiveId, Live.*, Teacher.Name  as TeacherName , 
Category.Id as CategoryId , Category.Name as CategoryName, Category.NameLT  as CategoryNameLT ,
SubCategory.Id as SubCategoryId , SubCategory.Name as SubCategoryName , SubCategory.NameLT as SubCategoryNameLT ,
(select count(*) from StudentLive where LiveId = Live.Id) as SubscriptionCount
from Live inner join Teacher on Live.TeacherId = Teacher.Id join Course  on Live.CourseId = Course.Id 
join Track on Track.Id=Course.TrackId 
join Subject on Track.SubjectId=Subject.Id 
join Department on Department.Id=Subject.DepartmentId
join SubCategory on SubCategory.Id=Department.SubCategoryId join Category on Category.Id=SubCategory.CategoryId
join Section on Section.Id=Category.SectionId where Live.IsActive=1 and Live.LiveLinkType = 2 and Live.TeacherId = " + teacherId + @" 
 and (select DATEADD(HOUR," + GetTimeZoneOffset(countryId) + @", (select getutcDate()))  ) <=  (select DATEADD(MINUTE,Live.Duration, Live.LiveDate)) order by Live.LiveAppearanceDate DESC;
";

            var lives = await _connectionFactory.GetConnection.QueryAsync<LiveQueryResult>(sql);
            return lives;
        }


        private async Task<IEnumerable<LiveQueryResult>> GetTrackLivesForTeacher(long countryId, long teacherId)
        {
            string sql = @"
select Live.Id as LiveId, Live.*, Teacher.Name  as TeacherName , 
Category.Id as CategoryId , Category.Name as CategoryName, Category.NameLT  as CategoryNameLT ,
SubCategory.Id as SubCategoryId , SubCategory.Name as SubCategoryName , SubCategory.NameLT as SubCategoryNameLT ,
(select count(*) from StudentLive where LiveId = Live.Id) as SubscriptionCount
from Live inner join Teacher on Live.TeacherId = Teacher.Id join Track  on Live.TrackId = Track.Id 
join Subject on Track.SubjectId=Subject.Id 
join Department on Department.Id=Subject.DepartmentId
join SubCategory on SubCategory.Id=Department.SubCategoryId join Category on Category.Id=SubCategory.CategoryId
join Section on Section.Id=Category.SectionId where Live.IsActive=1 and Live.LiveLinkType = 1 and Live.TeacherId = " + teacherId + @" 
 and (select DATEADD(HOUR," + GetTimeZoneOffset(countryId) + @", (select getutcDate()))  ) <=  (select DATEADD(MINUTE,Live.Duration, Live.LiveDate)) order by Live.LiveAppearanceDate DESC;
";

            var lives = await _connectionFactory.GetConnection.QueryAsync<LiveQueryResult>(sql);
            return lives;
        }


        private async Task<IEnumerable<LiveQueryResult>> GetOtherLivesForTeacher(long countryId, long teacherId)
        {
            string sql = @"
select Live.Id as LiveId, Live.* , Teacher.Name  as TeacherName , -1 as CategoryId ,  
N'اخري' as CategoryName , N'Other' as CategoryNameLT , 
-1 as SubCategoryId , N'اخري' as SubCategoryName, N'Other' as SubCategoryNameLT ,
(select count(*) from StudentLive where LiveId = Live.Id) as SubscriptionCount
from Live inner join Teacher on Live.TeacherId = Teacher.Id
where Live.IsActive=1 and Live.LiveLinkType  = 3 and Live.TeacherId = " + teacherId + @" and (select DATEADD(HOUR," + GetTimeZoneOffset(countryId) + @", (select getutcDate()))  ) <=  (select DATEADD(MINUTE,Live.Duration, Live.LiveDate))
order by Live.LiveAppearanceDate DESC;
";

            var lives = await _connectionFactory.GetConnection.QueryAsync<LiveQueryResult>(sql);
            return lives;
        }


        private int GetTimeZoneOffset(long countryId)
        {
            DateTime? countryNow = null;
            DateTime timeUtc = DateTime.UtcNow;
            switch (countryId)
            {
                case 20011:
                    var EgyptZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                    countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, EgyptZone);
                    return countryNow.Value.Subtract(timeUtc).Hours;
                case 3:
                    var KuwaitZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                    countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, KuwaitZone);
                    return countryNow.Value.Subtract(timeUtc).Hours;
                case 20012:
                    var JordanZone = TimeZoneInfo.FindSystemTimeZoneById("Jordan Standard Time");
                    countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, JordanZone);
                    return countryNow.Value.Subtract(timeUtc).Hours;
                case 20013:
                    var QatarZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                    countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, QatarZone);
                    return countryNow.Value.Subtract(timeUtc).Hours;
                case 20015:
                    var OmanZone = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
                    countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, OmanZone);
                    return countryNow.Value.Subtract(timeUtc).Hours;
            }
            return 0;
        }
        #endregion


    }
}
