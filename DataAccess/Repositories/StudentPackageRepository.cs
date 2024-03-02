using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class StudentPackageRepository : GenericRepository<StudentPackage>
    {
        public async Task<bool> CheckIfStudentEnrolledInThisPackageBefore(long PackageId, long StudentId)
        {
            var result = await GetWhere(" where StudentId=" + StudentId + " And PackageId=" + PackageId + " ");
            if (result == null)
            {
                return false;
            }
            return true;
        }
    }
}