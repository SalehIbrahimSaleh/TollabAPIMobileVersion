using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
   public class MyCourse
    {
        public string SubCategoryName { get; set; }
        public string SubCategoryNameLT { get; set; }
        public long   SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameLT { get; set; }
        public IEnumerable<Course> Courses { get; set; }
    }
}
