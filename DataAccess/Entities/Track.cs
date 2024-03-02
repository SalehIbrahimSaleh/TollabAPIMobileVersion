using Dapper.Contrib.Extensions;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    [Table("dbo.Track")]

    public class Track
    {
        public long Id { get; set; }
        public string TrackSubject { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public string Image { get; set; }
        public string ImageHomeCover { get; set; }
        public long? TeacherId { get; set; }
        public long? SubjectId { get; set; }
        public int?  SubscriptionDuration { get; set; }
        public decimal? SubscriptionCurrentPrice { get; set; }
        public decimal? SubscriptionOldPrice { get; set; }
        public string SKUNumber { get; set; }
        public decimal? SKUPrice { get; set; }
        public decimal? OldSKUPrice { get; set; }

        public int? OrderNumber { get; set; }

        public bool? BySubscription { get; set; }
        public bool? ShowWaterMark  { get; set; }
        public string TrackCode { get; set; }
        public string WhatsupGroupLink { get; set; }
        public decimal? CachedPrice { get; set; }
        public string DiscountType { get; set; }
        public float? DiscountValue { get; set; }
        [Computed]
        public int CountCourse { get; set; }
        [Computed]
        public decimal? TrackAchivementPercentage { get; set; }
        [Computed]
        public decimal TotalCurrentCost { get; set; }
        [Computed]
        public decimal TotalOldCost { get; set; }
        [Computed]
        public string TeacherName { get; set; }
        [Computed]
        public bool IsSubscriped { get; set; }
        public List<TrackPromotion> TrackPromotions { get; set; }
    }
}
