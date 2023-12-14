using StudySpark.WebScraper;
using StudySpark.WebScraper.Educator;

namespace StudySpark.Tests {
    [TestFixture]
    public class EducatorScraperTests {

        [Test]
        public void TestLoginpageElementDetection() {
            ScraperOptions scraperOptions = new ScraperOptions();
            scraperOptions.URL = "https://educator.windesheim.nl";

            StudySpark.WebScraper.WebScraper webScraper = new StudySpark.WebScraper.WebScraper(scraperOptions);

            webScraper.SetupDriver();
            webScraper.WaitForPageLoad();

            Assert.NotNull(webScraper.GetElementById("userNameInput"));
            Assert.NotNull(webScraper.GetElementById("passwordInput"));
            Assert.NotNull(webScraper.GetElementById("submitButton"));

            webScraper.CloseDriver();
        }

        [Test]
        public void TestCookieStorage() {
            ScraperOptions scraperOptions = new ScraperOptions();
            scraperOptions.URL = "https://www.google.com";
            scraperOptions.Debug = true;
            scraperOptions.DataDirPath = "browserdata";

            if (Directory.Exists(scraperOptions.DataDirPath)) {
                DeleteDirectory(scraperOptions.DataDirPath);
            }

            Directory.CreateDirectory(scraperOptions.DataDirPath);

            StudySpark.WebScraper.WebScraper webScraper = new StudySpark.WebScraper.WebScraper(scraperOptions);

            webScraper.SetupDriver();
            webScraper.WaitForPageLoad();

            webScraper.CloseDriver();

            Assert.IsTrue(Directory.Exists(scraperOptions.DataDirPath));
        }

        private void DeleteDirectory(string target_dir) {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files) {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs) {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}
