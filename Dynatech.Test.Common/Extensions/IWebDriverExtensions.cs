using Dynatech.Test.Common.Exceptions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Dynatech.Test.Common.Extensions {
    public enum UrlAddressText {
        CLIENT_LOGIN,
        CLIENT_ANALYSIS,
        CLIENT_ARCHIVE,
        CLIENT_HOME,
        CLIENT_TASKS,

        PORTAL_LOGIN,
        PORTAL_PROJECT,
        PORTAL_MEMBERS,
        PORTAL_MEMBERS_ROLES_MANAGEMENT,
        PORTAL_USER_MANAGEMENT,
        PORTAL_PROJECT_ASSIGNMENT,
        PORTAL_ADMINISTRATION,
        PORTAL_ADMINISTRATION_INDEXING,
        PORTAL_ADMINISTRATION_CODE_SNIPPETS,
        
        D3V_PROJECTS,
        D3V_AUDIT
    }

    public enum HtmlTextType {
        ORIGINAL,
        SHORT,
    }

    public static class WebDriverExtensions {
        /// <summary>
        /// Return full HTML code from browser
        /// </summary>
        /// <param name="session">IWebDriver</param>
        /// <param name="htmlTextType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string GetHtmlText(this IWebDriver session, HtmlTextType htmlTextType = HtmlTextType.SHORT) {
            switch (htmlTextType) {
                case HtmlTextType.ORIGINAL: {
                    return session.PageSource;
                }
                case HtmlTextType.SHORT: {
                    var result = session.PageSource;
                    var headText = result.CutTextWithInclude("<html", "</head>");
                    headText = headText.Replace(Environment.NewLine, " ");

                    var bodyText = result.CutTextWithInclude("<body", "</html>");
                    bodyText = bodyText.Replace("</html>", $"{Environment.NewLine}</html>");

                    return $"{headText}\r\n{bodyText}";
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(htmlTextType), htmlTextType, null);
            }
        }

        public static void GoToPage(this IWebDriver session, string url) {
            session.Navigate().GoToUrl(url);
        }

        public static void WaitWhenLoadSpinnerDisappear(this IWebDriver session, int delaySeconds = 0) {
            "//div[@class='ant-spin ant-spin-lg ant-spin-spinning']".WaitElementVisualDisappear(session, 120, secDelayOnStart: delaySeconds);
        }

        /// <summary>
        /// This method try to find difference between two xPath
        /// </summary>
        /// <param name="session"></param>
        /// <param name="xPath1"></param>
        /// <param name="xPath2"></param>
        /// <param name="retries"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="ElementFromGroupNotFoundException"></exception>
        public static int FindElementFromGroup(this IWebDriver session, string xPath1, string xPath2, int retries = 10, int time = 1) {
            var attempt = 0;
            while (true) {
                try {
                    session.FindElement(xPath1.ToXpath());
                    return 1;
                }
                catch (Exception) {
                    // ignored
                }

                try {
                    session.FindElement(xPath2.ToXpath());
                    return 2;
                }
                catch (Exception) {
                    // ignored
                }

                if (attempt >= retries) {
                    var errorDescription = $"In attempt to find error, error was not found\r\nxPath1:'{xPath1}'\r\nxPath2:'{xPath2}'";
                    var uniqueId = ArtifactLogs.CreateArtifactLog("FindFromGroup", errorDescription, session, "xPath1 or xPath2");

                    throw new ElementFromGroupNotFoundException(uniqueId, xPath1, xPath2);
                }

                attempt++;
                Thread.Sleep(TimeSpan.FromSeconds(time));
            }
        }

        public static void RunJavaScript(this IWebDriver session, string javaScriptCode) {
            var js = (IJavaScriptExecutor)session;
            js.ExecuteScript(javaScriptCode);
        }

        public static void DeleteChromeAllCookies(this IWebDriver session) {
            ((ChromeDriver)session).ExecuteCdpCommand("Storage.clearCookies", new Dictionary<string, object>());
        }
    }
}