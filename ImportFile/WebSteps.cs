using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using WebDriverManager.DriverConfigs.Impl;

namespace ImportFile
{
    [Binding]
    public class WebSteps
    {
        private readonly IWebDriver _driver;
        private string downloadFolderPath = @"C:\Users\Abinaya\Downloads";
        FeatureContext _featureContext;
        public WebSteps(FeatureContext _fc) {
            new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig());
           _driver = new ChromeDriver();
            _featureContext = _fc;
        }


        [Given(@"I am in webpage")]
        public void GivenIAmInWebpage()
        {
            _driver.Navigate().GoToUrl(TestRunner.configurationRoot["url"]);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _driver.Close();
        }

        [When(@"I click on Downloadable Link")]
        public void WhenIClickOnDownloadableLink()
        {
            //a[<data-testid]
            IList<IWebElement> downloadLinks = _driver.FindElements(By.CssSelector(".row div a"));
            foreach (IWebElement downloadLink in downloadLinks) {
                string link = downloadLink.GetAttribute("value");
                if (link != null && link.EndsWith(".pdf")) {
                    downloadLink.Click();
                    break;
                }

            }
        }

        [Then(@"I read contents from downloaded PDF File")]
        public void ThenIReadContentsFromDownloadedPDFFile()
            {
                string latestFilePath = GetLatestDownloadedFilePath();
                Console.WriteLine($"Latest downloaded file path: {latestFilePath}");
                _featureContext["filePath"]= latestFilePath;
            }

        private string GetLatestDownloadedFilePath()
        {
            string[] files = Directory.GetFiles(downloadFolderPath);
            Array.Sort(files, (x, y) => File.GetLastWriteTime(y).CompareTo(File.GetLastWriteTime(x)));
            string latestFilePath = files.Length > 0 ? files[0] : null;
            return latestFilePath;
        }






    }
        }
