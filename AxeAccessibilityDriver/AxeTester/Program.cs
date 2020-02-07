using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace AxeTester
{
    class Program
    {
        static string LogSaveFileLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\log";

        static SeleniumDriver seleniumDriver;
        static void Main(string[] args)
        {
            SeleniumDriver.Browser browser = SeleniumDriver.Browser.Chrome;
            TimeSpan timeOutThreshold = TimeSpan.FromSeconds(3);
            string url = "https://www.google.ca/";
            string screenshotSaveLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\pic";
            string environment = "test env";
            string loadingSpinner = "//*[@id='loadingspinner']";
            string errorContainer = "//div[@class='alert alert-danger']";

            seleniumDriver = new SeleniumDriver(browser, timeOutThreshold, environment, url, screenshotSaveLocation)
            {
                ErrorContainer = errorContainer,
                LoadingSpinner = loadingSpinner,
            };

            seleniumDriver.NavigateToURL("https://www.google.ca/");
            seleniumDriver.RunAODA("Google");

            SaveAODA();
        }

        /// <summary>
        /// Runs AODA If needed.
        /// </summary>
        public static void SaveAODA()
        {
            Console.WriteLine("Saving AODA");
            string tempFolder = $"{LogSaveFileLocation}\\temp\\";

            // Delete temp folder if exist and recreate
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }

            Directory.CreateDirectory(tempFolder);

            // Generate AODA Results
            seleniumDriver.GenerateAODAResults(tempFolder);

            // Zip all the contents up & Timestamp it
            string zipFileName = $"AODA_Results_{DateTime.Now:MM_dd_yyyy_hh_mm_ss_tt}.zip";
            ZipFile.CreateFromDirectory(tempFolder, $"{LogSaveFileLocation}\\{zipFileName}");

            // Remove all remaining contents.
            Directory.Delete(tempFolder, true);

            seleniumDriver.Quit();
        }
    }
}
