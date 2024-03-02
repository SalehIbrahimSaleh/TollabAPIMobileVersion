using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
  public  class TeacherAssistantRepository:GenericRepository<TeacherAssistant>
    {
        public async Task<TeacherAssistant> GetTeacherAssistantByIdentityId(string IdentityId)
        {
            try
            {
                TeacherAssistant teacherAssistant = null;
                var data = await GetAll(" Where IdentityId=N'" + IdentityId + "'");
                if (data.Count() > 0)
                {
                    teacherAssistant = data.FirstOrDefault();
                }

                return teacherAssistant;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

    }
}
