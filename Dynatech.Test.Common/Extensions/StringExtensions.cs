using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace Dynatech.Test.Common.Extensions {
    public static class StringExtensions {
        /// <summary>
        /// Cut text. Example: aaabbbcccdddeee -> bbb
        /// </summary>
        /// <param name="str">aaabbbcccdddeee</param>
        /// <param name="a">aaa</param>
        /// <param name="b">ccc</param>
        /// <returns>bbb</returns>
        public static string CutText(this string str, string a, string b) {
            var ai = str.IndexOf(a, StringComparison.Ordinal) + a.Length;
            var bi = str.IndexOf(b, StringComparison.Ordinal) - ai;

            return str.Substring(ai, bi);
        }

        public static string CutTextWithInclude(this string str, string a, string b) {
            var ai = str.IndexOf(a, StringComparison.Ordinal);
            var bi = str.IndexOf(b, StringComparison.Ordinal);

            return str.Substring(ai, bi - ai + b.Length);
        }

        public static IWebElement WaitForElement(this string str, IWebDriver session, bool checkOnDisplayed = true, int retries = 10,
            List<string> extraErrorDescriptionToLog = null) {
            return By.XPath(str).WaitForElement(session, checkOnDisplayed, retries, extraErrorDescriptionToLog: extraErrorDescriptionToLog);
        }
            
        public static void WaitElementHide(this string str, IWebDriver session, int retries = 10) {
            By.XPath(str).WaitElementHide(session, retries);
        }

        public static void WaitElementDisappear(this string str, IWebDriver session, int retries = 20, int time = 1, int secDelayOnStart = 0) {
            By.XPath(str).WaitElementDisappear(session, retries, time, secDelayOnStart);
        }
        
        public static void WaitElementVisualDisappear(this string str, IWebDriver session, int retries = 30, int time = 1, int secDelayOnStart = 0) {
            By.XPath(str).WaitElementVisualDisappear(session, retries, time, secDelayOnStart);
        }
        
        public static By ToXpath(this string str) {
            return By.XPath(str);
        }
        
        public static ReadOnlyCollection<IWebElement> FindElements(this string xPath, ISearchContext session) {
            return session.FindElements(By.XPath(xPath));
        }
    }
}