using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class StudentPromoCodeRepository:GenericRepository<StudentPromoCode>
    {
        public async Task<bool> checkIfStudentUsedThisPromoCodeBefore(long StudentId,long PromoCodeId)
        {
          var  IsFound = await GetWhere(" Where StudentId=" + StudentId + " And PromoCodeId=" + PromoCodeId + "");
            if (IsFound==null)
            {
                return false;
            }
            return true;
        }
    }
}
