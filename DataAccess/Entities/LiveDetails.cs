using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class LiveDetails
    {
        public long Id { get; set; }
        public string LiveName { get; set; }
        public string TeacherName { get; set; }
        public int? SubscriptionCount { get; set; }
        public double Duration { get; set; }
        public decimal CurrentCost { get; set; }
        public decimal? PreviousCost { get; set; }
        public bool IsShowingNow
        {
            get
            {
                var timeUtc = DateTime.UtcNow;
                DateTime? countryNow = null;
                switch (CountryId)
                {
                    case 20011:
                        var EgyptZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                        countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, EgyptZone);
                        return countryNow >= MeetingDate;
                    case 3:
                        var KuwaitZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                        countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, KuwaitZone);
                        return countryNow >= MeetingDate;
                    case 20012:
                        var JordanZone = TimeZoneInfo.FindSystemTimeZoneById("Jordan Standard Time");
                        countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, JordanZone);
                        return countryNow >= MeetingDate;
                    case 20013:
                        var QatarZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                        countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, QatarZone);
                        return countryNow >= MeetingDate;
                }
                return false;
            }
        }
        public string LiveRemainingTime
        {
            get
            {
                if (IsShowingNow)
                    return string.Empty;
                var timeUtc = DateTime.UtcNow;
                DateTime? countryNow = null;
                TimeSpan? subtractionValue = null;
                switch (CountryId)
                {
                    case 20011:
                        var EgyptZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                        countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, EgyptZone);
                        subtractionValue = MeetingDate.Subtract(countryNow.Value);
                        return string.Format("يعرض بعد {0} ايام و {1} ساعات و {2} دقائق", subtractionValue.Value.Days, subtractionValue.Value.Hours, subtractionValue.Value.Minutes);
                    case 3:
                        var KuwaitZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                        countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, KuwaitZone);
                        subtractionValue = MeetingDate.Subtract(countryNow.Value);
                        return string.Format("يعرض بعد {0} ايام و {1} ساعات و {2} دقائق", subtractionValue.Value.Days, subtractionValue.Value.Hours, subtractionValue.Value.Minutes);
                    case 20012:
                        var JordanZone = TimeZoneInfo.FindSystemTimeZoneById("Jordan Standard Time");
                        countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, JordanZone);
                        subtractionValue = MeetingDate.Subtract(countryNow.Value);
                        return string.Format("يعرض بعد {0} ايام و {1} ساعات و {2} دقائق", subtractionValue.Value.Days, subtractionValue.Value.Hours, subtractionValue.Value.Minutes);
                    case 20013:
                        var QatarZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                        countryNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, QatarZone);
                        subtractionValue = MeetingDate.Subtract(countryNow.Value);
                        return string.Format("يعرض بعد {0} ايام و {1} ساعات و {2} دقائق", subtractionValue.Value.Days, subtractionValue.Value.Hours, subtractionValue.Value.Minutes);
                }
                return string.Empty;
            }
        }
        public string HostURL { get; set; }
        public string JoinURL { get; set; }
        public long? MeetingId { get; set; }
        public string MeetingPassword { get; set; }
        public string CoverImage { get; set; }
        public string VideoURL { get; set; }
        public string VideoURI { get; set; }
        public DateTime MeetingDate { get; set; }
        public long CountryId { get; set; }
        public int? Enrollment { get; set; }
        public string SKUNumber { get; set; }
        public decimal? OldSKUPrice { get; set; }
        public decimal? SKUPrice { get; set; }
        public IEnumerable<LiveAttachment> Attachments { get; set; }
    }
}
