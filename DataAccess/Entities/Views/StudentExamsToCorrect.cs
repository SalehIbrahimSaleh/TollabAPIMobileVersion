using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Views
{
   public class StudentExamsToCorrect
    {
        public long Id { get; set; }
        public long? StudentId { get; set; }
        public long? CourseId { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public int? CompletionPercentage { get; set; }
        public long ExamId { get; set; }
        public string ExamName { get; set; }
        public bool Done { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public string CourseNameLT { get; set; }
        public string TrackName { get; set; }
        public string TrackNameLT { get; set; }
        public long SolveStatusId { get; set; }
        public string SolveStatusName { get; set; }
        public string SolveStatusNameLT { get; set; }
        public string SolveStatusColor { get; set; }
        public string DeadlineDateStatus { get; set; }
        public string DeadlineDateStatusLT { get; set; }

    }
}
