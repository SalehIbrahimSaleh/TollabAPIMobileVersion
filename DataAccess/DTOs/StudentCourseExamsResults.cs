using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs
{
    public class StudentCourseExamsResults
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public int StudentTotalResult { get; set; }
        public int TotalExamsDegree { get; set; }
        public List<ExamResult> ExamResults { get; set; }
    }
}
