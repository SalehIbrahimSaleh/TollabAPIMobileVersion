using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
   public class SubjectCourse
    {

        public int    CountCourses { get; set;}
        public decimal TotalPrice { get; set; }
        public string TeacherName { get; set;}
        public long TrackId { get; set; }
        public string TrackName { get; set; }
        public string TrackNameLT { get; set; }
        public string TrackImage { get; set; }

    }
}
