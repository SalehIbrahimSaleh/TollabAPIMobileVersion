using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Z.BulkOperations;

namespace DataAccess.Repositories
{
   public class ContentRepository:GenericRepository<Content>
    {
        public async Task<long> SaveViewPerContent(long studentId,long videoId, long courseId, long trackId, double percentage)
        {
            try
            {
                var content = await GetOneByQuery("Select * from Content Where Id=" + videoId + "");
                content.ViewsCount = content.ViewsCount + 1;
                await Update(content);
                return content.ViewsCount.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       

    }
}
