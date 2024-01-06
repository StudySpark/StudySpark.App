using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.WebScraper.WIP
{
    public class ScheduleActivity
    {

        public string CourseName { get; set; }

        public string CourseCode { get; set; }

        public string Class { get; set; }

        public string Classroom { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Teacher { get; set; }

        public override string ToString()
        {
            return $"{CourseName}, {CourseCode}, {Class}, {Classroom}, {Teacher}";
        }

    }
}
