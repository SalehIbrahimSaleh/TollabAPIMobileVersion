using DataAccess.Entities;
using DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.QueryResults
{
    public class LiveQueryResult
    {
        public long? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryNameLT { get; set; }
        public long? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryNameLT { get; set; }

        public long LiveId { get; set; }
        public string LiveName { get; set; }
        public long TeacherId { get; set; }
        public string TeacherName { get; set; }

        public int? OrderNumber { get; set; }

        public LiveLinkType LiveLinkType { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal? OldPrice { get; set; }
        public string SKUNumber { get; set; }
        public decimal? CurrentSKUPrice { get; set; }
        public decimal? OldSKUPrice { get; set; }
        public string HostURL { get; set; }
        public string JoinURL { get; set; }
        public string MeetingPassword { get; set; }
        public long? ZoomMeetingId { get; set; }
        public int Duration { get; set; }
        public DateTime LiveDate { get; set; }
        public DateTime LiveAppearanceDate { get; set; }
        public string VideoURL { get; set; }
        public string VideoURI { get; set; }
        public string Image { get; set; }
        public long CountryId { get; set; }
        public long? CourseId { get; set; }
        public long? TrackId { get; set; }
        public int? Enrollment { get; set; }
        public int? SubscriptionCount { get; set; }
        public bool? IsActive { get; set; }
        public IEnumerable<LiveAttachment> LiveAttachments { get; set; }
    }
}
