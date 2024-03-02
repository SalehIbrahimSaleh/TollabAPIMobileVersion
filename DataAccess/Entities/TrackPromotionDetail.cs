using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
   public class TrackPromotionDetail
    {
        public long Id { get; set; }
        public long TrackId{ get; set; }
        public string Name { get; set; }
        public DateTime PromotionStartDate { get; set; }
        public DateTime PromotionEndDate { get; set; }
        public bool IsShowInMobile { get; set; }
        public string DiscountType { get; set; }
        public float? NewPrice { get; set; }
        public decimal? SkuPrice { get; set; }
        public decimal? SkuNumber { get; set; }
        public string Description { get; set; }
        public string TrackImage { get; set; }
        public string Image { get; set; }
        public bool? IsEnroll { get; set; }

        public IEnumerable<Course> Courses { get; set; }
    }
}
