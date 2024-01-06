using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace StudySpark.WebScraper.Educator {
    public class EducatorWebScraper : WebScraper {

        public EducatorWebScraper(string username, string password) : this(new ScraperOptions { Username = username, Password = password }) {
        }

        public EducatorWebScraper(ScraperOptions scraperOptions) : base(scraperOptions) {
            scraperOptions.URL = "https://educator.windesheim.nl/studyprogress";
        }

        public void Load() {
            SetupDriver();
            WaitForPageLoad();

            HandleLogIn(true);
        }

        private void HandleLogIn(bool with2FA) {
            if (GetElementById("userNameInput") == null) {
                return;
            }

            GetElementById("userNameInput").SendKeys(scraperOptions?.Username);

            GetElementById("passwordInput").SendKeys(scraperOptions?.Password);

            GetElementById("submitButton").Click();

            if (with2FA) {
                WaitForIdLoad("verificationCodeInput");

                GetElementById("verificationCodeInput").SendKeys(scraperOptions?.TwoFACode);

                GetElementById("signInButton").Click();
            }
        }

        public bool TestLoginCredentials() {
            HandleLogIn(false);
            WaitForPageLoad();
            try {
                WaitForIdLoad("verificationCodeInput", 15);
                return true;
            } catch {
                return false;
            }
        }

        public List<StudentGrade> FetchGrades() {
            List<StudentGrade> grades = new List<StudentGrade>();

            WaitForPageLoad();

            foreach (IWebElement item in GetElementsByClassName("studyplanning-unit")) {
                StudentGrade grade = new StudentGrade();

                grade.CourseName = item.FindElement(By.ClassName("exam-unit__name")).FindElement(By.ClassName("btn-link")).Text;
                grade.CourseCode = item.FindElement(By.ClassName("exam-unit__name")).FindElement(By.TagName("small")).Text;
                grade.ECs = item.FindElement(By.ClassName("exam-unit-workload-amount")).Text;
                grade.Semester = item.FindElement(By.ClassName("justify-content-end")).FindElements(By.ClassName("examination-date"))[0].FindElement(By.TagName("dd")).Text;
                grade.TestDate = item.FindElement(By.ClassName("justify-content-end")).FindElements(By.ClassName("examination-date"))[1].FindElement(By.TagName("dd")).Text;
                grade.Grade = item.FindElement(By.ClassName("grade")).Text.Split("\n")[0];

                grades.Add(grade);
            }

            return grades;
        }
    }
}
