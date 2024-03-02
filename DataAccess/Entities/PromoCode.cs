using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    [Table("dbo.PromoCode")]
    public class PromoCode
    {
        public long Id { get; set; }

        public string Pattern { get; set; }

        public long? Count { get; set; }
        public long? UsedCount { get; set; }

        public string PromoCodeText { get; set; }

        public decimal? PromoCodeValue { get; set; }

        public DateTime? ExpirationDate
        {
            get;

            set;
        }


        //pattern
         public long? CountryId { get; set; }

         public long? SectionId { get; set; }

         public long? CategoryId { get; set; }

         public long? SubCategoryId { get; set; }


         public long? DepartmentId { get; set; }

        public DateTime? CreationDate { get; set; }

        public bool Active { get; set; }
    }
}
