using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TollabAPI.Utils
{
    public class ImageModel
    {
        public long  RecordId { get; set; }
        public string Table { get; set; }
        public int ImageType { get; set; }
        public string Image { get; set; }
        public string CoulmnName { get;  set; }
    }
}