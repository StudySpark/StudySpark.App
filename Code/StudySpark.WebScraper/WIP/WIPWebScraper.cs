using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.WebScraper.WIP
{
    public class WIPWebScraper : WebScraper
    {

        public WIPWebScraper(ScraperOptions scraperOptions) : base(scraperOptions)
        {
            scraperOptions.URL = "https://wip.windesheim.nl";
        }

        public void Load()
        {
            SetupDriver();
            WaitForPageLoad();

            HandleLogIn(true);
        }

        private void HandleLogIn(bool with2FA)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("i0116")));

            var email = driver.FindElement(By.Id("i0116"));
            var subButton = driver.FindElement(By.Id("idSIButton9"));

            email.SendKeys(scraperOptions?.Username);
            subButton.Click();

            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("passwordInput")));

            var password = driver.FindElement(By.Id("passwordInput"));
            subButton = driver.FindElement(By.Id("submitButton"));

            password.SendKeys(scraperOptions?.Password);
            subButton.Click();

            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("idTxtBx_SAOTCC_OTC")));

            var twoFA = driver.FindElement(By.Id("idTxtBx_SAOTCC_OTC"));
            subButton = driver.FindElement(By.Id("idSubmit_SAOTCC_Continue"));

            if (with2FA)
            {
                twoFA.SendKeys(scraperOptions?.TwoFACode);
                subButton.Click();

                try {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("idSIButton9")));

                    subButton = driver.FindElement(By.Id("idSIButton9"));

                    subButton.Click();
                } catch (WebDriverTimeoutException wdte) {
                    throw wdte;
                }
            }
            else
            {
                return;
            }
        }

        public bool TestLoginCredentials()
        {
            HandleLogIn(false);
            WaitForPageLoad();

            try
            {
                WaitForIdLoad("idTxtBx_SAOTCC_OTC", 15);
                return true;
            } catch
            {
                try
                {
                    WaitForIdLoad("DeltaPlaceHolderMain", 15);
                    return true;
                }
                catch 
                {
                    return false;
                }               
            }

        }

        public List<ScheduleActivity> FetchSchedule()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("activitiesupcoming-link")));

            var moveToButton = driver.FindElement(By.ClassName("activitiesupcoming-link"));
            moveToButton.Click();

            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("dhx_cal_next_button")));

            moveToButton = driver.FindElement(By.ClassName("dhx_cal_next_button"));
            moveToButton.Click();

            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("dhx_cal_event")));

            List<IWebElement> list = driver.FindElements(By.XPath(".//div//div[contains(@class,'dhx_scale_holder')]/div[contains(@class,'cal_event')]")).ToList();
            List<ScheduleActivity> result = new List<ScheduleActivity>();
            foreach (IWebElement element in list)
            {
                element.Click();

                List<IWebElement> scheduleInfo = driver.FindElements(By.XPath(".//div[contains(@class, 'dlwo-row')]/div[contains(@class, 'dlwo-col')]")).ToList();

                ScheduleActivity schedAct = new ScheduleActivity();

                foreach (IWebElement info in scheduleInfo)
                {
                    int index = scheduleInfo.IndexOf(info);

                    Debug.WriteLine(index + " : " + info.Text);

                    if (index >= 0 && index % 2 == 0)
                    {

                        switch (scheduleInfo[index].Text)
                        {
                            case "Class":
                                schedAct.Class = scheduleInfo[index + 1].Text;
                                break;
                            case "Room":
                                schedAct.Classroom = scheduleInfo[index + 1].Text;
                                break;
                            case "Teacher(s)":
                                schedAct.Teacher = scheduleInfo[index + 1].Text;
                                break;
                            case "OE/EvL":
                                int cut = scheduleInfo[index + 1].Text.IndexOf("|");
                                string coursename = scheduleInfo[index + 1].Text.Substring(0, cut - 1);
                                string coursecode = scheduleInfo[index + 1].Text.Substring(cut + 2);

                                schedAct.CourseName = coursename;
                                schedAct.CourseCode = coursecode;
                                break;
                            default:
                                schedAct.CourseName = "Niet gevonden";
                                schedAct.CourseCode = "Niet gevonden";
                                schedAct.Class = "Niet gevonden";
                                schedAct.Classroom = "Niet gevonden";
                                schedAct.Teacher = "Niet gevonden";
                                break;
                        }
                    }

                    string timeInfo = driver.FindElement(By.ClassName("dlwo-event-date")).Text;
                    int firstCut = timeInfo.IndexOf("-");
                    int lastCut = timeInfo.IndexOf(" ", firstCut + 2);

                    string start = timeInfo.Substring(0, firstCut - 1);
                    string end = timeInfo.Substring(firstCut + 2, lastCut - (firstCut + 2));
                    string date = timeInfo.Substring(lastCut + 1);

                    string format = "HH:mm d MMMM yyyy";

                    schedAct.StartTime = DateTime.ParseExact(start + " " + date, format, CultureInfo.InvariantCulture);
                    schedAct.EndTime = DateTime.ParseExact(end + " " + date, format, CultureInfo.InvariantCulture);
                }

                Debug.WriteLine("ScheduleActivity = " + schedAct.ToString());
                Debug.WriteLine("--------------");

                result.Add(schedAct);

                var exitButton = driver.FindElement(By.XPath(".//div/div[contains(@class, 'dhx_cancel_btn_set')]"));
                exitButton.Click();
            }

            return result;
        }
    }
}
