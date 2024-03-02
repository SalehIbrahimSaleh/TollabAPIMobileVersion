using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class TeacherPushTokenRepository:GenericRepository<TeacherPushToken>
    {

        public async Task<bool> DeleteAllTokensAsync(long TeacherId)
        {
            string sql = @"Delete from TeacherPushToken where TeacherId=" + TeacherId + "";
            var result = await _connectionFactory.GetConnection.ExecuteAsync(sql);
            if (result >= 0)
            {
                return true;
            }
            return false;
        }

    }
}
