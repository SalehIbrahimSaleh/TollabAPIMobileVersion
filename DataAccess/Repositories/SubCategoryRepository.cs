using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class SubCategoryRepository:GenericRepository<SubCategory>
    {
        public async Task<IEnumerable<SubCategory>> GetSubCategoriesByCategoryId(long CategoryId,int Page)
        {
            Page = Page * 30;

            var result = await GetAll(" Where CategoryId=" + CategoryId + " order by Id  OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY");
            return result;

        }
    }
}
