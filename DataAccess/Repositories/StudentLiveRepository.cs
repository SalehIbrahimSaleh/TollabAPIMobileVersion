using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class StudentLiveRepository : GenericRepository<StudentLive>
    {
        public async Task<bool> CheckIfStudentEnrolledInThisLiveBefore(long liveId, long StudentId)
        {
            var result = await GetWhere(" where StudentId=" + StudentId + " And LiveId=" + liveId + " ");
            if (result == null)
            {
                return false;
            }
            return true;
        }
    }
}
