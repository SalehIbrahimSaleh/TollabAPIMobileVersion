using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
   public class Pattern
    {
        public long? Id { get; set; }
        public long? CountryId { get; set; }
        public long? SectionId { get; set; }
        public long? CategoryId { get; set; }
         public long? DepartmentId { get; set; }
        public long? SubCategoryId { get; internal set; }
    }
}
