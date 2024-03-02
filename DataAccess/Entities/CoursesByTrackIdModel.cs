using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public  class ContentIdPath
    {
        public int Id { get; set; }
        public string MPath { get; set; }
    }
   public class CoursesByTrackIdModel
    {
        public string CategoryName { get; set; }
        public string CategoryNameLT { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryNameLT { get; set; }
        public string TrackName { get; set; }
        public string TrackNameLT { get; set; }
        public string TrackImage { get; set; }
        public string SKUNumber { get; set; }
        public decimal? SKUPrice { get; set; }
        public decimal? OldSKUPrice { get; set; }
        public decimal? SubscriptionCurrentPrice { get; set; }
        public decimal? SubscriptionOldPrice { get; set; }
        public string TeacherName { get; set; }
        public string WhatsupGroupLink { get; set; }
        public bool IsEnrolled { get; set; }
        public bool IsAllowToShow { get; set; }
        public long? ViewsCount { get; set; }
        public decimal? CourseAchivementPercentage { get; set; }
        public bool HasOffers { get; set; }
        
        public IEnumerable<Course> Courses { get; set; }
    }
}
