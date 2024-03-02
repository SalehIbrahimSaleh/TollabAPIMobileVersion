using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PromoCodeRepository : GenericRepository<PromoCode>
    {
        public async Task<PromoCode> IsThisPromoCodeValid(string PromoCodeText)
        {
            try
            {
                PromoCode promoCode = null;
                promoCode = await GetOneByQuery("select * from PromoCode where Active=1 and PromoCodeText like '" + PromoCodeText + "' And Cast(ExpirationDate as date)> cast( (select GETUTCDATE()) as date ) And ISNULL(UsedCount,0)<=Count");
                return promoCode;

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<bool> StudentValidToThisPromocode(long studentId, PromoCode promoCode)
        {
            var conn = _connectionFactory.GetConnection;
            string sql =
@"select Student.Id, Student.CountryId,Section.Id as SectionId,Category.Id as CategoryId,SubCategory.Id as SubCategoryId,
StudentDepartment.DepartmentId from Student  join StudentDepartment on Student.Id=StudentDepartment.StudentId
join Section on Section.CountryId=Student.CountryId
join Category on Category.SectionId=Section.Id
join SubCategory on SubCategory.CategoryId=Category.Id
where Student.Id="+studentId+" ";

            string pattern = "";
            if (promoCode.CountryId!=null)
            {
                sql = sql + "  And Student.CountryId=" + promoCode.CountryId + "";
            }
            if (promoCode.SectionId!=null)
            {
                sql = sql + " and Section.Id="+promoCode.SectionId+"";
            }
            if (promoCode.CategoryId!=null)
            {

                sql = sql + " and Category.Id=" + promoCode.CategoryId + "";
            }
            if (promoCode.SubCategoryId!=null)
            {
                sql = sql + " and SubCategory.Id=" + promoCode.SubCategoryId + "";
            }
            if (promoCode.DepartmentId!=null)
            {
                sql = sql + " and StudentDepartment.DepartmentId=" + promoCode.DepartmentId + "";
            }
            var patternModel = conn.QueryFirstOrDefault<Pattern>(sql);
            if (patternModel != null)
            {
              string studentPattern=  GetPatten(patternModel,promoCode);

                if (studentPattern==promoCode.Pattern)
                {
                    return true;
                }
            }

            return false;
        }


        private string GetPatten(Pattern pattern,PromoCode promoCode)
        {
            string patterntext = "";
            if (promoCode.CountryId == null)
            {
                patterntext = "0";
            }
            if (promoCode.CountryId > 0)
            {
                patterntext = pattern.CountryId.ToString();
            }
            if (promoCode.SectionId == null)
            {
                patterntext = patterntext + "0";
            }
            if (promoCode.SectionId > 0)
            {
                patterntext = patterntext + pattern.SectionId.ToString();
            }
            if (promoCode.CategoryId == null)
            {
                patterntext = patterntext + "0";
            }
            if (promoCode.CategoryId > 0)
            {
                patterntext = patterntext + pattern.CategoryId.ToString();
            }
            if (promoCode.SubCategoryId == null)
            {
                patterntext = patterntext + "0";
            }
            if (promoCode.SubCategoryId > 0)
            {
                patterntext = patterntext + pattern.SubCategoryId.ToString();
            }
            if (promoCode.DepartmentId == null)
            {
                patterntext = patterntext + "0";
            }
            if (promoCode.DepartmentId > 0)
            {
                patterntext = patterntext + pattern.DepartmentId.ToString();
            }
            return patterntext;
        }

    }
}
