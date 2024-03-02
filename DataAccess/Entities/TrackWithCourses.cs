using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
   public class TrackWithCourses
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public string Image { get; set; }
        public int? SubscriptionDuration { get; set; }
        public decimal? SubscriptionCurrentPrice { get; set; }
        public decimal? SubscriptionOldPrice { get; set; }

        public bool? BySubscription { get; set; }

        public long? DepartmentId { get; set; }
        public int CountCourse { get; set; }
        public decimal? TotalCurrentCost { get; set; }
        public decimal? TotalOldCost { get; set; }

    }
}
