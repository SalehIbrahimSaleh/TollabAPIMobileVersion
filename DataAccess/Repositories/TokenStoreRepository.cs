using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
 public  class TokenStoreRepository:GenericRepository<TokenStore>
    {
        public async Task<bool> InvalidateOldTokens(long StudentId)
        {
            var d=await _connectionFactory.GetConnection.ExecuteAsync("update TokenStore set Valid=0 where StudentId="+StudentId+ " and Valid=1 ");
            if (d>0)
            {
                return true;
            }
            return false;
        }
    }
}
