using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class FavouriteRepository:GenericRepository<Favourite>
    {
        public async Task<bool> IsAdded(Favourite favourite)
        {
            var IsFound = await GetAll(" Where CourseId="+favourite.CourseId+" And StudentId="+favourite.StudentId+" ");
            if (IsFound.Count()>0)
            {
                return true;
            }
            return false;
        }
        public async Task<Favourite> GetFavourite(Favourite favourite)
        {
            var result = await GetWhere(" Where CourseId=" + favourite.CourseId + " And StudentId=" + favourite.StudentId + " ");
           
            return result;
        }
        public async Task<IEnumerable<Course>> GetAllFavourite(long StudentId, int Page)
        {
            Page = Page * 30;
            string sql= @"select Course.*,Teacher.Name As TeacherName from Course join Favourite on Favourite.CourseId=Course.Id join Track on Track.Id=Course.TrackId Join Teacher on Track.TeacherId=Teacher.Id
                        where Favourite.StudentId=" + StudentId+"  order by Course.Id OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY";
            var result =await _connectionFactory.GetConnection.QueryAsync<Course>(sql);
            /*MMK if (result != null)
            {
                foreach (var item in result)
                {
                    var syntaxx = "";
                    if (StudentId != null)
                    {
                        syntaxx = "and StudentCourseDownloaded.studentid=" + StudentId;
                    }
                    var xx = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<DownloadObj>("select course.Id as Id, OfflinePackage.IsShowInMobile as CanDownload,\r\n case when count(StudentCourseDownloaded.StudentId) >= 0 then 1 else 0 end  as IsDownloaded,\r\nOfflinePackage.PackageEndDate  as ValidToDownloadDate,\r\nDATEDIFF(day, getdate(),OfflinePackage.PackageEndDate) AS RemainingDays,\r\n\r\nCourse.CurrentCost as Price,\r\ncourse.Id as CourseId\r\nfrom Course inner join StudentCourseDownloaded \r\non Course.id = StudentCourseDownloaded.CourseId \r\ninner join StudentPackage on StudentCourseDownloaded.StudentId= StudentPackage.StudentId\r\ninner join OfflinePackage on OfflinePackage.Id=StudentPackage.PackageId\r\n\r\nwhere course.id=" + item.Id + syntaxx +"\r\ngroup by course.Id, OfflinePackage.IsShowInMobile ,\r\n OfflinePackage.PackageEndDate  ,\r\nDATEDIFF(day, OfflinePackage.PackageEndDate, getdate()),\r\n\r\nCourse.CurrentCost ,\r\ncourse.Id ");

                    if (xx != null)
                    {
                        var obj = new DownloadObj();
                        obj.Id = xx.Id;
                        obj.CanDownload = xx.CanDownload;
                        obj.IsDownloaded = xx.IsDownloaded;
                        obj.CourseId = item.Id;
                        obj.Currency = "EGP";
                        obj.Price = xx.Price;
                        obj.RemainingDays = xx.RemainingDays;
                        obj.ValidToDownloadDate = xx.ValidToDownloadDate;
                        item.DownloadObj = obj;
                    }
                    else
                    {
                        var obj = new DownloadObj();
                        obj.Id = 1;
                        obj.CanDownload = true;
                        obj.IsDownloaded = false;
                        obj.CourseId = item.Id;
                        obj.Currency = "SR";
                        obj.Price = 55;
                        obj.RemainingDays = 25;
                        obj.ValidToDownloadDate = DateTime.Now.AddDays(25);
                        item.DownloadObj = obj;
                    }
                }

            }*/
            return result;
        }

        public async Task<IEnumerable<Content>> GetContentsByCourse(long courseId)
        {
            string sql = @"select Content.*  from Content inner join [Group] on Content.GroupId=[Group].Id inner join Course on [Group].CourseId= Course.Id where Content.ContentTypeId= 1 and Content.ProviderType= 'vdocipher' and Course.id= " + courseId + "";
            var resultss = await _connectionFactory.GetConnection.QueryAsync<Content>(sql);
            return resultss;
        }


        public async Task<IEnumerable<ContentIdPath>> GetContentsByCourseId(long courseId)
        {
            try
            {
                string sql = @"select Content.Id as Id,Content.Path as Path from Content inner join [Group] on Content.GroupId=[Group].Id inner join Course on [Group].CourseId= Course.Id where Content.ContentTypeId= 1 and Content.ProviderType= 'vdocipher' and Course.id= " + courseId + "";

                var result = await _connectionFactory.GetConnection.QueryAsync<ContentIdPath>(sql);
                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
