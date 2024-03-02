using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("dbo.TermAndCondition")]
    public class TermAndCondition
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string TitleLT { get; set; }
        public string Discription { get; set; }
        public string DiscriptionLT { get; set; }
        public bool   Type { get; set; }
    }
}
