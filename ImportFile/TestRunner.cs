using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace ImportFile
{
    [Binding]
    public class TestRunner
    {
        public static IConfigurationRoot configurationRoot;
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            string path = "C:\\Users\\Abinaya\\source\\repos\\ImportFile\\ImportFile\\configuration.json";
            ConfigurationBuilder ConfigBuilder=new ConfigurationBuilder();
            configurationRoot=ConfigBuilder.AddJsonFile(path).AddEnvironmentVariables().Build();
        }

        
    }
}
