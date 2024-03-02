using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    [Table("dbo.Teacher")]

    public class Teacher
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Bio { get; set; }
        public bool? Gender { get; set; }
        public string Address { get; set; }
        public long? CountryId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? RegisterationDate { get; set; }
        public string FaceBookLink { get; set; }
        public string TwitterLink { get; set; }
        public string Instagram { get; set; }
        public int? TakenPercentage { get; set; }
        public int? LiveTakenPercentage { get; set; }
        public bool? Enabled { get; set; }
        public string IdentityId { get; set; }

        [Computed]
        public long StudentCount { get; internal set; }
        [Computed]
        public long StudentSubscriptionsCount { get;  set; }

        [Computed]
        public int CourseCount { get;  set; }

        [Computed]
        public string Currency { get; internal set; }
        [Computed]
        public string CurrencyLT { get; set; }
        [Computed]
        public IEnumerable<Country> Countries { get; set; }

        [Computed]
        public IEnumerable<TrackWithCourses>   TrackWithCourses { get;  set; }
        [Computed]
        public JArray Token { get; set; }
        [Computed]
        public bool IsTeacher { get; set; }
        [Computed]
        public TeacherAssistant TeacherAssistant { get; set; }
        public bool CanAddLive { get; set; }
    }
}
