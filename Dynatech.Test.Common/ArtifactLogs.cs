using System.Text;
using Dynatech.Test.Common.Extensions;
using OpenQA.Selenium;

namespace Dynatech.Test.Common {
    public static class ArtifactLogs {
        private const string PATH_FOR_LOG_FILE = "ArtifactLogs";
        private static int uniqueIdStat;

        public static int GetNextUniqueId() {
            uniqueIdStat++;
            return uniqueIdStat;
        }
        
        private static void CreateLogFileWithHtml(string logDescription, string fileSuffix, string textContent, int uniqueId) {
            CheckArtifactDirectory();
            
            //HTML 2020-06-17 14-16-46-408 - Begin NOT red.txt
            var dataTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss-fff");
            var fileName = $"{PATH_FOR_LOG_FILE}{Path.DirectorySeparatorChar}HTML {dataTime} - {fileSuffix}.{uniqueId}.log";
            
            File.WriteAllText(fileName,textContent);
        }

        /// <summary>
        /// Create Screenshot with suffix. Example: 'HTML 2020-09-16 06-41-53-202 - WaitForElement.png'
        /// </summary>
        /// <param name="logDescription"></param>
        /// <param name="fileSuffix">'WaitForElement'</param>
        /// <param name="webDriver"></param>
        /// <param name="uniqueId"></param>
        private static void CreateScreenshot(string logDescription, string fileSuffix, IWebDriver webDriver, int uniqueId) {
            CheckArtifactDirectory();
            
            var dataTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss-fff");
            var fileName = $"{PATH_FOR_LOG_FILE}{Path.DirectorySeparatorChar}HTML {dataTime} - {fileSuffix}.{uniqueId}.png";

            var screenshot = ((ITakesScreenshot)webDriver).GetScreenshot();
            screenshot.SaveAsFile(fileName, ScreenshotImageFormat.Png);    
        }

        private static void CheckArtifactDirectory() {
            if (!Directory.Exists(PATH_FOR_LOG_FILE)) {
                Directory.CreateDirectory(PATH_FOR_LOG_FILE);
            }
        }

        public static int CreateArtifactLog(string fileSuffix, string logDescription, IWebDriver session, string elementXpath = "", List<string> extraLogDescription = null, bool enableCreatScreenShots = true) {
            return CreateArtifactLog(-1, fileSuffix, logDescription, session, elementXpath, extraLogDescription, enableCreatScreenShots);
        }

        public static int CreateArtifactLog(int uniqueId, string fileSuffix,  string logDescription, IWebDriver session, string elementXpath = "", List<string> extraLogDescription = null, bool enableCreatScreenShots = true) {
            if (uniqueId == -1) {
                uniqueId = GetNextUniqueId();
            }
            
            var htmlCode = session.GetHtmlText(); 
            
            var errorMessage = new StringBuilder();
            errorMessage.AppendLine($"File: '{fileSuffix}'");
            errorMessage.AppendLine($"Error: '{logDescription}'");
            errorMessage.AppendLine($"XPath: '{elementXpath}'");
            errorMessage.AppendLine("--- HTML Code 1 --------------------------------------------------");
            errorMessage.AppendLine(htmlCode);

            if (extraLogDescription != null) {
                foreach (var extraLog in extraLogDescription) {
                    errorMessage.AppendLine();
                    errorMessage.AppendLine();
                    errorMessage.AppendLine(extraLog);
                }
            }
            
            errorMessage.AppendLine();

            var errorDesc = $"{logDescription}\nXPath:{elementXpath}";
            CreateLogFileWithHtml(errorDesc, fileSuffix, errorMessage.ToString(), uniqueId);

            if (enableCreatScreenShots) {
                CreateScreenshot(errorDesc, fileSuffix, session, uniqueId);
            }
            
            // Create artifact logs from chrome logs
            SaveBrowserLogToArtifact(uniqueId, fileSuffix, session);
            return uniqueId;
        }

        public static void SaveBrowserLogToArtifact(int uniqueId, string fileSuffix, IWebDriver session) {
            CheckArtifactDirectory();
            var dataTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss-fff");
            
            // get loge from chrome driver
            var logsBrowser = session.Manage().Logs.GetLog(LogType.Browser);
            if (logsBrowser != null) {
                var logsBrowserTextToFile = new StringBuilder();
                foreach (var item in logsBrowser) {
                    logsBrowserTextToFile.AppendLine(item.ToString());
                }
                
                var fileNameBrowser = $"{PATH_FOR_LOG_FILE}{Path.DirectorySeparatorChar}HTML {dataTime} - {fileSuffix} - Browser .{uniqueId}.log";
                File.WriteAllText(fileNameBrowser,logsBrowserTextToFile.ToString());
            }

            var logsDriver = session.Manage().Logs.GetLog(LogType.Driver);
            if (logsDriver != null) {
                var logsDriverTextToFile = new StringBuilder();
                foreach (var item in logsDriver) {
                    logsDriverTextToFile.AppendLine(item.ToString());
                }
           
                var fileNameDriver = $"{PATH_FOR_LOG_FILE}{Path.DirectorySeparatorChar}HTML {dataTime} - {fileSuffix} - Driver .{uniqueId}.log";
                File.WriteAllText(fileNameDriver,logsDriverTextToFile.ToString());
            }
        }
    }
}