using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using iText.Layout.Element;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using TechTalk.SpecFlow;
using WebDriverManager.DriverConfigs.Impl;

namespace ImportFile
{
    [Binding]
    public class WebActionSteps
    {
        private DateTime _startTime;
        private const int IntervalMinutes = 10;
        private const int DurationHours = 8;

        private readonly string _url;
        private readonly WebDriverManager.DriverManager _driverManager;
        FeatureContext _context;
        private WebDriverWait wait;
        FeatureContext _fc;
        private string downloadFolderPath = @"C:\Users\Abinaya\Downloads";

        public WebActionSteps(FeatureContext context)
        {
            _fc=context;
            _url = TestRunner.configurationRoot["url"];
            _driverManager = new WebDriverManager.DriverManager();
            _context = context;
           
    }

        [Given(@"I have set up the web action")]
        public void GivenIHaveSetUpTheWebAction()
        {
            // Initial setup if needed
            //IWebDriver driver=OpenBrowserAndNavigateToUrl();
        }

        [When(@"I start the periodic action")]
        public async Task WhenIStartThePeriodicAction()
        {

            using (IWebDriver driver = OpenBrowserAndNavigateToUrl())
            {
                PerformWebActions(driver);
            }


        }



        private IWebDriver OpenBrowserAndNavigateToUrl()
        {
            _driverManager.SetUpDriver(new ChromeConfig());
            IWebDriver driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(_url);
            driver.Manage().Timeouts().ImplicitWait= TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            return driver;
        }

        private void PerformWebActions(IWebDriver driver)
        {
           
            Console.WriteLine("Performing web action at: " + DateTime.Now);
            ////div[@class='page-layout']/div/div/div[2]/a
            IList<IWebElement> downloadLinks = driver.FindElements(By.XPath("//body/main/div[2]/div/div/div/a"));
            IList<IWebElement> visibleLinks = downloadLinks.Where(link => link.Displayed).ToList();
            int countVisibleLinks = visibleLinks.Count;
            Console.WriteLine($"Total visible download links found: {countVisibleLinks}");
            for (int i = 2; i <= countVisibleLinks + 1; i++)
            {
                try
                {
                    IWebElement mainLink = driver.FindElement(By.XPath($"//body/main/div[2]/div/div/div[{i}]/a"));
                    string linkHref = mainLink.GetAttribute("href");
                    
                    if (linkHref != null && linkHref.EndsWith(".pdf"))
                    {
                        string linkText = mainLink.Text;
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({ block: 'center' });", mainLink);
                        Console.WriteLine($"Link {i}: Href = {linkHref}, Text = {linkText}");
                        wait.Until(ExpectedConditions.ElementToBeClickable(mainLink));
                        mainLink.Click();
                        Console.WriteLine("Element clicked");
                        bool downloadCompleted = WaitForDownloadToComplete(downloadFolderPath, linkText, TimeSpan.FromMinutes(1));
                        if (downloadCompleted)
                        {
                            Console.WriteLine("Download completed successfully.");
                            _fc["FileName"] = linkText;
                        }
                        else
                        {
                            Console.WriteLine("Download did not complete within the expected time.");
                        }
                        break;
                        
                    }
                    
                }
            catch (NoSuchElementException)
            {
                Console.WriteLine($"No element found for index {i}");
            }
        }

        }

        [Then(@"the action should run every 10 minutes for 8 hours")]
        public void ThenTheActionShouldRunEvery10MinutesFor8Hours()
        {
            // Verification logic if necessary
            
        }
        [Then(@"I obtain file path of downloaded file from local directory")]
        public void ThenIObtainFilePathOfDownloadedFileFromLocalDirectory()
        {
            var filePathInfo = GetLatestDownloadedFilePath().ToList(); // Convert to list to access elements by index
            if (filePathInfo.Count >= 2)
            {
                _fc["FilePath"] = filePathInfo[1]; // Full file path
                _fc["FileNameWExtension"] = filePathInfo[0]; // File name with extension
                Console.WriteLine($"Latest downloaded file path: {filePathInfo[1]}");
                Console.WriteLine($"Latest downloaded file name with extension: {filePathInfo[0]}");
            }
        }

        private IEnumerable<string> GetLatestDownloadedFilePath()
        {
            string[] files = Directory.GetFiles(downloadFolderPath);
            Array.Sort(files, (x, y) => File.GetLastWriteTime(y).CompareTo(File.GetLastWriteTime(x)));
            string latestFilePath = files.Length > 0 ? files[0] : null;
            if (latestFilePath != null)
            {
                yield return Path.GetFileName(latestFilePath);
                yield return latestFilePath;
            }
            else
            {
                yield return null; // or throw an exception, depending on your requirements
            }
        }

        private bool WaitForDownloadToComplete(string downloadDirectory, string expectedFileName, TimeSpan timeout)
        {
            var stopwatch = Stopwatch.StartNew();
            string downloadedFilePath = Path.Combine(downloadDirectory, expectedFileName);
            string incompleteFilePath = downloadedFilePath + ".crdownload";

            while (stopwatch.Elapsed < timeout)
            {
                if (File.Exists(downloadedFilePath) && !File.Exists(incompleteFilePath))
                {
                    return true;
                }
                Thread.Sleep(1000);
            }
            return false;
        }

    }
}
