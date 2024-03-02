using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.ContentCourseTrackUser")]
    public class ContentCourseTrackUser
    {
        public long Id { get; set; }
        public long? ContentId { get; set; }
        public long? CourseId { get; set; }
        public long? TrackId { get; set; }
        public long? UserId { get; set; }
        public double? Percentage { get; set; }
        public int? MinuteCount{ get; set; }
        public int? SecoundsCount { get; set; }
    }
}
