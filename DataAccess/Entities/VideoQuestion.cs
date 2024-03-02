using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    [Table("dbo.VideoQuestion")]
    public class VideoQuestion
    {
        public long Id { get; set; }
        public string Question { get; set; }
        public float? Minute { get; set; }
        public string Image { get; set; }
        public string Voice { get; set; }

        public bool? ViewMyAccount { get; set; }
        public long? ContentId { get; set; }
        public long? LiveId { get; set; }
        public DateTime? CreationDate { get; set; }
        public long? StudentId { get; set; }
        [Computed]
        public string StudentName { get; set; }
        [Computed]
        public string StudentPhoto { get; set; }
        [Computed]
        public string ContentName { get; set; }
        [Computed]
        public string ContentNameLT { get; set; }
        [Computed]
        public IEnumerable<Reply> Replies { get;  set; }
    }
}
