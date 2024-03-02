using DataAccess.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class StudentLiveHome
    {
        public long? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryNameLT { get; set; }
        public long? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryNameLT { get; set; }
        public IEnumerable<LiveDTO> Lives { get; set; }
    }
}
