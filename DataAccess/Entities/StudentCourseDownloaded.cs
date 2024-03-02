﻿using Dapper.Contrib.Extensions;
using System;

namespace DataAccess.Entities
{
    [Table("dbo.StudentCourseDownloaded")]
    public class StudentCourseDownloaded
    {
        public long Id { get; set; }
        public long? StudentId { get; set; }
        public long? CourseId { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public int? CompletionPercentage { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }
    }
}
