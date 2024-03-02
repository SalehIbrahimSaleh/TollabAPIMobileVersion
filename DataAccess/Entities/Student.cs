using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    [Table("dbo.Student")]
    public class Student
    {

        public long   Id       { get; set; }
        public string Name     { get; set; }
        public string ParentName { get; set; }
        public string ParentName2 { get; set; }
        public string ParentPhone { get; set; }
        public string ParentPhone2 { get; set; }
        public string Email    { get; set; }
        public string PhoneKey { get; set; }
        public string Phone    { get; set; }
        public bool?  Gender    { get; set; }
        public string Photo    { get; set; }
        public string Bio      { get; set; }
        public DateTime? CreationDate { get; set; }
        public string IdentityId { get; set; }
        public long CountryId { get; set; }

        public bool? Enabled { get; set; }
        //[JsonIgnore]
        public int Vcode { get;  set; }
        public DateTime? ExpirationVCodeDate { get;  set; }
        public bool Verified { get;  set; }

        public string PaymentLink     { get; set; }
        public string PaymentKey { get; set; }
        public DateTime? LastSendDate { get; set; }

        public int? ScreenShootCount { get; set; }
        public DateTime? LastTakenScreenshootDate { get; set; }

        public int? NumberMaxLoginCount { get; set; }
        public int? NumberCurrentLoginCount { get; set; }


        [Computed]
        public JArray Token { get;  set; }
        [Computed]
        public int NumberOfCourses { get; set; }

        [Computed]
        public IEnumerable<Interest> Interests { get;  set; }

        [Computed]
        public IEnumerable<Section> Sections { get; set; }
        [Computed]
        public string Currency { get; internal set; }
        [Computed]
        public string CurrencyLT { get;  set; }
        [Computed]
        public string CountryCode { get;  set; }
        [Computed]
        public string UserType { get; set; }
        [Computed]
        public string Password { get; set; }
        [Computed]
        public Package package { get; set; }

    }
    public class Package
    {
        public long? id { get; set; }
        public decimal? price { get; set; }
        public string color { get; set; }
        public int? period { get; set; }
        public string name { get; set; }
        public string details { get; set; }
        public string SKUNumber { get; set; }
        public string PackageExpireDate { get; set; }
    }
}
