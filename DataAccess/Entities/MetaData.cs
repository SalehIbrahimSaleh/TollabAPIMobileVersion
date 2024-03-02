using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class MetaData
    {
        public List<ExamQuestionType> ExamQuestionTypes { get; set; }
        public List<ExamType> ExamTypes { get; set; }
        public List<SolveStatus> SolveStatuses { get; set; }

        public List<Country> Countries { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public List<TermAndCondition> TermAndConditions { get; set; }
        public List<Reference> References { get; set; }
        public List<CourseStatus> CourseStatuses { get; set; }
        public List<AboutUs> AboutUs { get; set; }
        public List<Setting> Settings { get; set; }
        public List<SystemSetting> SystemSettings { get; set; }
    }
}
