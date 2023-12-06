namespace StudySpark.WebScraper {
    public class ScraperOptions {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string TwoFACode { get; set; } = "";

        public string URL { get; set; } = "";
        public string DataDirPath { get; set; } = "";

        public bool Debug { get; set; } = false;
    }
}
