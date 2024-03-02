using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class GroupRepository : GenericRepository<Group>
    {
        public async Task<long?> GetCourseIdByContentId(long? contentId)
        {
            var result = await GetOneByQuery("select [Group].CourseId from Content join [Group] on [Group].Id=Content.GroupId where Content.Id=" + contentId + "");
            if (result!=null)
            {
                return result.CourseId;
            }
            return null;
        }

        public async Task<string> GetAlbumByGroupId(long? groupId)
        {
            string sql = @"select Course.AlbumUri from Course join [Group] on Course.Id=[Group].CourseId 
            where [Group].Id=" + groupId + "";
            var Album =await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync(sql);
            return Album;
        }
    }
}
