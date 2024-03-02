using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Views
{
   public class StudentAnswersWithStudentExamId
    {
        public long StudentExamId { get; set; }
        public List<StudentAnswer>  StudentAnswers { get; set; }
    }
}
