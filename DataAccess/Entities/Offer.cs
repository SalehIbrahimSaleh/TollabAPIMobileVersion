using Dapper.Contrib.Extensions;
using DataAccess.Enums;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.Offer")]
    public class Offer
    {
        public long Id { get; set; }
        public string Tilte { get; set; }
        public string TitleLT { get; set; }
        public string Image { get; set; }
        public long? CourseId { get; set; }
        public int? OrderNumber { get; set; }
        public OfferTypeEnum? OfferContentTypeId { get; set; }

        public string VideoURL { get; set; }

        public string VideoURI { get; set; }
        public string VideoThumbnail { get; set; }


        public OfferLinkTypeEnum? OfferLinkTypeId { get; set; }
        public long? TrackId { get; set; }

        public string ExternalLink { get; set; }

        public DateTime EndOfferDate { get; set; }
        [Computed]
        public Course Course { get; set; }
        [Computed]
        public Track Track { get; set; }
    }
}
