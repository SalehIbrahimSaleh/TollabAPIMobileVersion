using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class DownloadObj
    {
        public long Id { get; set; }
        public bool CanDownload { get; set; } //Package
        public bool IsDownloaded { get; set; } //Course
        public DateTime ValidToDownloadDate { get; set; }
        public int RemainingDays { get; set; }
        public float Price { get; set; }
        public string Currency { get; set; }
        public long? CourseId { get; set; }

    }
    public class ExamsPerCourse
    {
        public int ActualExams { get; set; }
        public int SolvedExams { get; set; }

    }
}