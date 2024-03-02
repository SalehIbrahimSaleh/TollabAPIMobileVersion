using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace DataAccess.Entities
{
    [Table("OfflinePackage")]
    public class OfflinePackage
    {
        public long Id{ get; set; }
        public string Name { get; set; }
        public DateTime PackageEndDate { get; set; }
        public bool IsShowInMobile { get; set; }
        public string DiscountType { get; set; }
        public float DiscountValue { get; set; }
        public float? ChachedPrice { get; set; }
        public decimal? NewPrice { get; set; }
        public decimal? SkuPrice { get; set; }
        public string SkuNumber { get; set; }
        public string Description { get; set; }
        public int Period { get; set; }
        public string Color { get; set; }
    }
}
