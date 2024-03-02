using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
  public class StudentContentRepository:GenericRepository<StudentContent>
    {

        public async Task<bool> ViewThisContent(long ContentId, long StudentId)
        {
            try
            {
                var IsViewed = await GetWhere(" where ContentId=" + ContentId + " And StudentId=" + StudentId + " ");
                if (IsViewed == null)
                {
                    StudentContent studentContent = new StudentContent { ContentId = ContentId, StudentId = StudentId };
                    var Insert = await Add(studentContent);
                    if (Insert > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;

            }
            catch (Exception e)
            {

                return false;
            }
        }
    }
}
