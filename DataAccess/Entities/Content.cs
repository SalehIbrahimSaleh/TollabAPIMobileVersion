using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.Content")]
    public class Content
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameLT { get; set; }
        public string Path { get; set; }
        public long? GroupId { get; set; }
        public double? Duration { get; set; }
        public long? ContentTypeId { get; set; }
        public bool? IsFree { get; set; }
        public string VideoUri { get; set; }
        public string ProviderType { get; set; }

        public string YoutubeLink { get; set; }
        public int? OrderNumber { get; set; }
        public long? ViewsCount { get; set; }

        [Computed]
        public int IsViewed { get; set; }
        [Computed]
        public StudentContent StudentContent  { get; set; }
        [Computed]
        public string VideoLink { get; set; }
    }
}
