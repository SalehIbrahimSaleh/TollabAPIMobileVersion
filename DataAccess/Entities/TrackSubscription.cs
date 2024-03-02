using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("dbo.TrackSubscription")]
    public class TrackSubscription
    {
        public long Id { get; set; }
        public long TrackId { get; set; }
        public long StudentId { get; set; }
        public DateTime? DurationExpiration { get; set; }
        public DateTime? CreationDate { get; set; }
        public string ReferenceNumber { get;  set; }
    }
}
