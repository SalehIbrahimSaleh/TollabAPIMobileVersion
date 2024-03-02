using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    [Table("TrackPromotion")]
    public class TrackPromotion 
    {
        public TrackPromotion()
        {
            this.TrackPromotionCourses = new List<TrackPromotionCourse>();
        }
        public string Name { get; set; }
        public long? TrackId { get; set; }
        public DateTime PromotionEndDate { get; set; }
        public bool IsShowInMobile { get; set; }
        public string DiscountType { get; set; }
        public float DiscountValue { get; set; }
        public float? ChachedPrice { get; set; }
        public decimal? NewPrice { get; set; }
        public decimal? SkuPrice { get; set; }
        public decimal? SkuNumber { get; set; }
        public string Description { get; set; }

        public Track Track { get; set; }
        public List<TrackPromotionCourse> TrackPromotionCourses { get; set; }
    }
}
