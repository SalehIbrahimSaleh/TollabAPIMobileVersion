using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class SearchWordRepository : GenericRepository<SearchWord>
    {
        public async  Task<IEnumerable<SearchWord>> GetSearchWords(long StudentId)
        {
            var result = await GetAll(" Where StudentId="+StudentId+ "");

            return result;
        }
        public async Task<SearchWord> GetSearchWord(long StudentId,string Word)
        {
            var result = await GetWhere(" Where StudentId=" + StudentId + " And Word like N'"+Word+"'");
            return result;
        }
    }
}
