using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class DepartmentRepository:GenericRepository<Department>
    {
        public async Task<IEnumerable<Department>> GetDepartmentsBySubCategoryId(long SubCategoryId, int Page)
        {
            Page = Page * 30;

            var result = await GetAll(" Where SubCategoryId=" + SubCategoryId + " order by Id  OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY");
            return result;

        }

        public async Task<IEnumerable<Department>> GetDepartmentsByStudentId(long StudentId)
        {
            var result = await GetAllByQuery("Select Department.* from Department join StudentDepartment on Department.Id=StudentDepartment.DepartmentId where StudentDepartment.StudentId=" + StudentId + "");
            return result;
        }


        public async Task<IEnumerable<Interest>> GetInterests(long StudentId)
        {
            try
            {
                List<Interest> result = new List<Interest>();
                string query = @"select Section.Id as SectionId,Section.Image as SectionImage,Section.Name as SectionName,Section.NameLT as SectionNameLT,
                               Category.Name as CategoryName,Category.NameLT as CategoryNameLT,SubCategory.Name SubCategoryName ,
                            SubCategory.NameLT SubCategoryNameLT,SubCategory.Id as SubCategoryId from Category join SubCategory on Category.Id=SubCategory.CategoryId
                            join Department on SubCategory.Id=Department.SubCategoryId join StudentDepartment On
                            Department.Id=StudentDepartment.DepartmentId join Section on Category.SectionId=Section.Id
                            Where StudentDepartment.StudentId=" + StudentId + "";
                var list = await _connectionFactory.GetConnection.QueryAsync<Interest>(query);
                List<long> SubIds = new List<long>();
                foreach (var item in list)
                {

                    if (!SubIds.Contains(item.SubCategoryId))
                    {
                        result.Add(item);
                        SubIds.Add(item.SubCategoryId);
                    }
                }
                return result;

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<IEnumerable<Category>> GetInterestsBeforeEdit(long StudentId)
        {
            try
            {
                string categoriesQuery = @"select distinct newdata.* from (SELECT    dbo.Category.Id , dbo.Category.Name , dbo.Category.NameLT 
                         FROM dbo.Category INNER JOIN dbo.SubCategory ON dbo.Category.Id = dbo.SubCategory.CategoryId INNER JOIN
                         dbo.Department ON dbo.Department.SubCategoryId = dbo.SubCategory.Id INNER JOIN
                         dbo.StudentDepartment ON dbo.StudentDepartment.DepartmentId = dbo.Department.Id where StudentDepartment.StudentId="+StudentId+") newdata";
                var CategoryDtat =await _connectionFactory.GetConnection.QueryAsync<Category>(categoriesQuery);
                foreach (var item in CategoryDtat)
                {

                    var subCategoryList =await _connectionFactory.GetConnection.QueryAsync<SubCategory>("select * from SubCategory where CategoryId=" + item.Id + "");
                    List<SubCategory> subCategoriesList = new List<SubCategory>();
                    foreach (var subcategory in subCategoryList)
                    {
                        string departmentQuery = @"select Department.* from Department join StudentDepartment on Department.Id=StudentDepartment.DepartmentId where Department.SubCategoryId=" + subcategory.Id + " And StudentDepartment.StudentId="+StudentId+"";
                        var departmentList = await _connectionFactory.GetConnection.QueryAsync<Department>(departmentQuery);
                        if (departmentList.Count()>0)
                        {
                            subcategory.Departments = departmentList;
                            subCategoriesList.Add(subcategory);
                        }
                        if (subCategoriesList.Count()>0)
                        {
                            item.SubCategories = subCategoriesList;
                        }
                    }

                }
                return CategoryDtat;
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        public async Task<IEnumerable<StudentHomeCourse>> GetHomeCoursesByStudentInterests(long StudentId,long CountryId ,long Page)
        {
            try
            {
                Page = Page * 3;
                // string ListHeaderQuery = @"select Distinct  * from StudentDepartmentsView where StudentId=" + StudentId + " order by CategoryId  OFFSET " + Page + " Rows FETCH Next 3 Rows ONLY ";

                //eng:mansour 
                string ListHeaderQuery = @"select distinct * from (
                        SELECT   dbo.Category.Id AS CategoryId, dbo.Category.Name AS CategoryName,
                        dbo.Category.NameLT AS CategoryNameLT, dbo.SubCategory.Id AS SubCategoryId, 
                        dbo.SubCategory.Name AS SubCategoryName, dbo.SubCategory.NameLT AS SubCategoryNameLT
                        FROM   dbo.Category INNER JOIN
                        dbo.SubCategory ON dbo.Category.Id = dbo.SubCategory.CategoryId INNER JOIN
                        dbo.Department ON dbo.Department.SubCategoryId = dbo.SubCategory.Id 
						Inner join Section on Section.Id=Category.SectionId
						where Section.CountryId="+CountryId+") as NewTable order by CategoryId  OFFSET " + Page + " Rows FETCH Next 3 Rows ONLY";
                var ListHeaderData = _connectionFactory.GetConnection.Query<StudentHomeCourse>(ListHeaderQuery);
                foreach (var item in ListHeaderData)
                {
                    // string TrackIdsQuery = "EXEC TrackIds " + StudentId + ","+item.SubCategoryId+"";

                    //eng:mansour 
                    string TrackIdsQuery = @"select Track.Id as TrackId,SubCategory.Id as SubCategoryId from Track  join [Subject] on Track.SubjectId=[Subject].Id join 
                    Department on Department.Id =[Subject].DepartmentId
                    join SubCategory on SubCategory.Id = Department.SubCategoryId where Track.IsActive=1 and SubCategory.Id = " + item.SubCategoryId + "";
                    var TrackIds = _connectionFactory.GetConnection.Query<TrackAndSUbCategoryIds>(TrackIdsQuery);
                    string TrackIdsString = null;
                    foreach (var id in TrackIds)
                    {
                        TrackIdsString += id.TrackId.ToString()+",";
                    }
                    if (!string.IsNullOrEmpty(TrackIdsString))
                    {
                        TrackIdsString = TrackIdsString.Remove(TrackIdsString.Length - 1, 1);
                        //string ListBodyQuery = @" select   Teacher.Id as TeacherId, Teacher.Name as TeacherName,StudentDepartment.Id as StudentDepartmentId,
                        //Track.Id as TrackId, Track.Name as TrackName,Track.NameLT as TrackNameLT,Track.Image as TrackImage,
                        //Department.Id as DepartmentId,Subject.Id as SubjectId ,
                        //  (select Count(Course.Id) from Course where Course.TrackId=Track.Id And Course.CourseStatusId=3) as CountCourses,
                        //(select Sum(Course.CurrentCost) from Course where Course.TrackId=Track.Id And Course.CourseStatusId=3) as TotalPrice
                        //  from Category  Inner join SubCategory on Category.Id=SubCategory.CategoryId Inner join Department on SubCategory.Id=Department.SubCategoryId
                        // Inner  join Subject on Subject.DepartmentId=Department.Id Inner join Track on Track.SubjectId=Subject.Id Inner join Teacher on Track.TeacherId=Teacher.Id
                        // Inner join StudentDepartment on  StudentDepartment.DepartmentId=Department.Id
                        //  where  Track.Id IN (" + TrackIdsString + ") and StudentDepartment.StudentId=" + StudentId + " Order By Track.OrderNumber ";


                        //eng:mansour 
                        //MMK Replace Track.Image with New ImageHomeCover
                        string ListBodyQuery = @" select   Teacher.Id as TeacherId, Teacher.Name as TeacherName,
                        Track.Id as TrackId, Track.Name as TrackName,Track.NameLT as TrackNameLT, COALESCE(NULLIF(Track.ImageHomeCover,''), Track.Image) as TrackImage,
                        Department.Id as DepartmentId,Subject.Id as SubjectId ,
                          (select Count(Course.Id) from Course where Course.TrackId=Track.Id And Course.CourseStatusId=3) as CountCourses,
                        (select Sum(Course.CurrentCost) from Course where Course.TrackId=Track.Id And Course.CourseStatusId=3) as TotalPrice
                          from Category  Inner join SubCategory on Category.Id=SubCategory.CategoryId Inner join Department on SubCategory.Id=Department.SubCategoryId
                         Inner  join Subject on Subject.DepartmentId=Department.Id Inner join Track on Track.SubjectId=Subject.Id Inner join Teacher on Track.TeacherId=Teacher.Id
                          
                          where  Track.IsActive=1  and Track.Id IN (" + TrackIdsString + ") Order By Track.OrderNumber ";

                        var ListBodyData = _connectionFactory.GetConnection.Query<SubjectCourse>(ListBodyQuery);
                        if (ListBodyData.Count() > 0)
                        {
                            item.SubjectCourses = ListBodyData;
                        }
                    }
                }
                return ListHeaderData;

            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}



//public async Task<IEnumerable<HomeCourse>> GetInterestsList(long StudentId)
//{
//    try
//    {
//        string query = @"select Category.Name as CategoryName,Category.NameLT as CategoryNameLT,SubCategory.Name SubCategoryName ,
//                    SubCategory.NameLT SubCategoryNameLT,Department.Name as DepartmentName,Department.NameLT as DepartmentNameLT,Department.Id as DepartmentId
//                    from Category join SubCategory on Category.Id=SubCategory.CategoryId join Department on SubCategory.Id=Department.SubCategoryId
//                    join StudentDepartment On Department.Id=StudentDepartment.DepartmentId Where StudentDepartment.StudentId=" + StudentId + "";
//        var list = await _connectionFactory.GetConnection.QueryAsync<HomeCourse>(query);

//        return list;

//    }
//    catch (Exception e)
//    {

//        throw e;
//    }
//}