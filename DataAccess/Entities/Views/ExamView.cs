using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Views
{
   public class ExamView
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DeadlineDate { get; set; }
        public int OrderNumber { get; set; }
        public int Duration { get; set; }
        public long ExamTypeId { get; set; }
        public long CourseId { get; set; }
        public bool Publish { get; set; }
        public DateTime? EndDate { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameLT { get; set; }
        public string TrackName { get; set; }
        public string TrackNameLT { get; set; }
        public string CourseName { get; set; }
        public string CourseNameLT { get; set; }
        public string CourseImage { get; set; }
        public string ExamTypeName { get; set; }
        public string ExamTypeNameLT { get; set; }
        public int NumberOfStudent { get; set; }
        public int TeacherTransaction { get; set; }
        public int QuestionsCount { get; set; }
        public string TeacherName { get; set; }
        public long   SolveStatusId { get; set; }
        public string SolveStatusName { get; set; }
        public string SolveStatusNameLT { get; set; }
        public string SolveStatusColor { get; set; }
        public string DeadlineDateStatus { get; set; }
        public string DeadlineDateStatusLT { get; set; }

    }
}
