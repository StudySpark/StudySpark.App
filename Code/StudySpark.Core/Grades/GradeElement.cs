﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.Core.Grades {
    public class GradeElement {
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public string? TestDate { get; set; }
        public string? Semester { get; set; }
        public string? ECs { get; set; }
        private string? _grade;
        public string? Grade { get { return _grade; } set { _grade = value?.Replace("Voldaan", "V"); } }
        public string? GradeColor {
            get {

                if (double.TryParse(Grade, out double numericGrade)) {
                    if (numericGrade >= 5.5) {
                        return "#FF158221"; // Green
                    } else {
                        return "#FF821521"; // Magenta
                    }
                } else {
                    return Grade.StartsWith("V") ? "#FF158221" : "#828282"; // Green : Dark Gray
                }
            }
        }

        public override string ToString() {
            return $"{CourseName} ({CourseCode}) - TestDate: {TestDate} | Semester: {Semester} | ECs: {ECs} | Grade: {Grade} | GradeColor: {GradeColor}";
        }
    }
}
