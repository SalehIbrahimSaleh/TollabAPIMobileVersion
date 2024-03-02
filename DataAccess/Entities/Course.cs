using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    [Table("dbo.Course")]

    public class Course
    {
        public long Id { get; set; }
        public string CourseTrack { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public long? TrackId { get; set; }
        public decimal? CurrentCost { get; set; }
        public decimal? PreviousCost { get; set; }
        public string SKUNumber { get; set; }
        public decimal? SKUPrice { get; set; }
        public decimal? OldSKUPrice { get; set; }

        public DateTime? CreationDate { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public long? SubscriptionCount { get; set; }
        public string Image { get; set; }
        public string IntroVideo { get; set; }
        public string AlbumUri { get; set; }
        [Computed]
        public bool? IsShowingNow { get; set; }
        [Computed]
        public bool? AnswerExam { get; set; }
        [Computed]
        public bool? BySubscription { get; set; }
        public bool? ShowWaterMark { get; set; }
        public bool? NeedParent { get; set; }
        [Computed]
        public bool? TrackShowWaterMark { get; set; }
        public string ProviderType { get; set; }

        public int? OrderNumber { get; set; }

        public long? CourseStatusId { get; set; }

        [Computed]
        public long? ViewsCount { get; set; }
        
        public bool? IsShowViewsCount { get; set; }

        
        [Computed]
        public decimal? CourseAchivementPercentage { get; set; }
        [Computed]
        public string TeacherName { get; set; }
        [Computed]
        public long TeacherId { get; set; }
        [Computed]
        public long? VideoCount { get; set; }
        [Computed]
        public long? FilesCount { get; set; }
        [Computed]
        public double? HoursCount { get; set; }
        [Computed]
        public int IsFavourite { get; set; }
        [Computed]
        public int Enrollment { get; set; }
        [Computed]
        public int ContentCount { get; set; }
        [Computed]
        public int ViewedContent { get; set; }
        [Computed]
        public int CountStudentEnrolled { get; set; }
        [Computed]
        public decimal GainedMoney { get; set; }
        [Computed]
        public bool ShouldAnswerExam { get; set; }
        [Computed]
        public List<Group> Groups { get;  set; }
        [Computed]
        public IEnumerable<CourseTags> CourseTags { get;  set; }
        public string IntroVideoUri { get; set; }
        public string CourseCode { get;  set; }
        [Computed]
        public IEnumerable<VideoQuestion>  VideoQuestions { get; set; }
        [Computed]
        public Content Content { get; set; }
        [Computed]
        public DownloadObj DownloadObj { get; set; }
    }
}
