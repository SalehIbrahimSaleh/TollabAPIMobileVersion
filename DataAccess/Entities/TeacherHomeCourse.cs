using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
  public  class TeacherHomeCourse
    {
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameLT { get; set; }
        public string SubjectImage { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryNameLT { get; set; }
        public string CategoryName { get; set; }
        public string CategoryNameLT { get; set; }
        public IEnumerable<Course> Courses { get; set; }
    }
}
