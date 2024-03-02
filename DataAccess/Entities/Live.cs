using DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace DataAccess.Entities
{
    [Table("dbo.Live")]
    public class Live
    {
        public long Id { get; set; }
        public string LiveName { get; set; }
        public long TeacherId { get; set; }

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
        public int Duration { get; set; }
        public DateTime LiveDate { get; set; }
        public DateTime LiveAppearanceDate { get; set; }
        public string VideoURL { get; set; }
        public string VideoURI { get; set; }
        public string Image { get; set; }
        public long CountryId { get; set; }
        public long? CourseId { get; set; }
        public long? TrackId { get; set; }

        public long? ZoomMeetingId { get; set; }

        [Write(false)]
        [Computed] 
        public Teacher Teacher { get; set; }
        [Write(false)]
        [Computed] 
        public Track Track { get; set; }
        [Write(false)]
        [Computed] 
        public Course Course { get; set; }
        [Write(false)]
        [Computed]
        public Country Country { get; set; }
    }
}
