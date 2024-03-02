using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("dbo.TeacherSubject")]
    public class TeacherSubject
    {
        public long  Id { get; set; }  
        public long? TeacherId { get; set; }
        public long? SubjectId { get; set; }

    }
}
