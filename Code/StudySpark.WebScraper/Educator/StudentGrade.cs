namespace StudySpark.WebScraper.Educator {
    public class StudentGrade {
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string Semester { get; set; }
        public string TestDate { get; set; }
        public string ECs { get; set; }
        public string Grade { get; set; }

        public override string ToString() {
            return $"CourseName: {CourseName}, CourseCode: {CourseCode}, Semester: {Semester}, TestDate: {TestDate}, ECs: {ECs}, Grade: {Grade}";
        }
    }
}