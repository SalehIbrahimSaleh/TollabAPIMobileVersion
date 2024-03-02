using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.QueryResults
{
    public class StudentExamResult
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public string ExamName { get; set; }
        public int StudentResult { get; set; }
        public int ExamDegree { get; set; }
    }
}
