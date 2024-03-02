using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Views
{
   public class ExaminationSummary
    {
        public long ExamQuestionTypeId { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public float TotalQuestionDegree { get; set; }
        public float TotalAnswerDegree { get; set; }
        public int QuestionCount { get; set; }
        public int CorrectedByTeacher { get; set; }
    }
}
