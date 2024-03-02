using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("TrackPromotionCourse")]
    public class TrackPromotionCourse 
    {
        public long? TrackPromotionId { get; set; }
        public long? CourseId { get; set; }
        public Course Course { get; set; }
        public TrackPromotion TrackPromotion { get; set; }
    }
}
