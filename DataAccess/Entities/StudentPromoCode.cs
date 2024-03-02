using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("dbo.StudentPromoCode")]
    public   class StudentPromoCode
    {
        public long Id { get; set; }
        public long PromoCodeId { get; set; }
        public long StudentId { get; set; }
        
    }
}
