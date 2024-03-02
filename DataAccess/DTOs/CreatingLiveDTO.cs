using DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs
{
    public class CreatingLiveDTO
    {
        public string LiveName { get; set; }
        public long TeacherId { get; set; }
        public int Duration { get; set; }
        public LiveLinkType LiveLinkType { get; set; }
        public long? CourseId { get; set; }
        public long? TrackId { get; set; }
        public long CountryId { get; set; }
        public DateTime LiveDate { get; set; }
        public DateTime LiveAppearanceDate { get; set; }
        public string Image { get; set; }
    }
}
