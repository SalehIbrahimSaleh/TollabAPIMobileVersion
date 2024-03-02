using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.ContactUs")]
    public class ContactUs
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
      //  public string ContactNumber { get; set; }
        public DateTime? CreationDate { get; set; }
        public long? StudentId { get; set; }
        public long?  TeacherId { get; set; }
        public bool? Status { get; set; }
    }
}
