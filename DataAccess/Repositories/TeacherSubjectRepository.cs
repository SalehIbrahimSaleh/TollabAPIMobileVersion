using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class TeacherSubjectRepository:GenericRepository<TeacherSubject>
    {
        public async Task<IEnumerable<Subject>> GetTeacherSubject(long TeacherId,long CountryId, int Page)
        {
            try
            {
                Page = Page * 30;
                string sql = @" 
  select distinct Subject.* from  TeachersSubject
 join Subject on TeachersSubject.Id=Subject.Id
 join Department on Department.Id=Subject.DepartmentId
 join SubCategory on SubCategory.Id=Department.SubCategoryId
 join Category on Category.Id=SubCategory.CategoryId
 join Section on Section.Id=Category.SectionId
where Section.CountryId=" + CountryId+ " and TeachersSubject.TeacherId =" + TeacherId + " order by Id desc  OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY";
                var result = await _connectionFactory.GetConnection.QueryAsync<Subject>(sql);
                return result;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}
