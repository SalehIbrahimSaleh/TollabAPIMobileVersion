using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;

namespace DataAccess.Entities
{
    [Table("TeacherAssistant")]
    public class TeacherAssistant
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long? TeacherId { get; set; }
        public bool? Enabled { get; set; }
        public string IdentityId { get; set; }
        public DateTime RegisterationDate { get; set; }
        [Computed]
        public JArray Token { get; set; }
        [Computed]
        public bool IsTeacher { get; set; }
    }
}
