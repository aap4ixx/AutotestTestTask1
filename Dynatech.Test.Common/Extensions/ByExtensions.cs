using System.Collections.Concurrent;
using Dynatech.Test.Common.Exceptions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Dynatech.Test.Common.Extensions {
    public static class ByExtensions {
        private static readonly ConcurrentDictionary<(IWebDriver, int, int), WebDriverWait>
            waitCache = new ConcurrentDictionary<(IWebDriver, int, int), WebDriverWait>();
        
        public static void CheckElementIsVisible(this By by, IWebDriver session, int time) {
            try {
                var wait = new WebDriverWait(session, TimeSpan.FromSeconds(time));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
            }
            catch (Exception) {
                var errorDescription = $"Time (Retry): {time}\r\n";
                var uniqueId = ArtifactLogs.CreateArtifactLog("CheckElementIsVisible", errorDescription, session, by.ToString());
                throw new ElementIsVisibleException(uniqueId, errorDescription);
            }
        }

        public static void CheckElementIsInvisible(this By by, IWebDriver session, int time) {
            try {
                var wait = new WebDriverWait(session, TimeSpan.FromSeconds(time));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(by));
            }
            catch (Exception e) {
                var uniqueId = ArtifactLogs.CreateArtifactLog("CheckElementIsInvisible", e.ToString(), session, by.ToString());
                throw new  ElementIsInvisibleException(uniqueId, e.ToString());
            }
        }

        public static void WaitElementHide(this By by, IWebDriver session, int time = 10, int secDelayOnStart = 0) {
            if (secDelayOnStart != 0) {
                Thread.Sleep(TimeSpan.FromSeconds(secDelayOnStart));
            }
            
            try {
                var wait = new WebDriverWait(session, TimeSpan.FromSeconds(time));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(by));
            }
            catch (Exception e) {
                var uniqueId = ArtifactLogs.CreateArtifactLog("WaitElementHide", e.ToString(), session, by.ToString());
                throw new  ElementIsInvisibleException(uniqueId, e.ToString());
            }
        }
        
        public static IWebElement WaitForElement(this By by, IWebDriver session, bool checkOnDisplayed = true, int retries = 10, int time = 1,
            List<string> extraErrorDescriptionToLog = null) {
            var key = (session, retries, time);
            var wait = waitCache.GetOrAdd(key, _ => new WebDriverWait(session, TimeSpan.FromSeconds(time) * retries));
            
            IWebElement WaitForElementIml(IWebDriver web) {
                var e = web.FindElement(by);
                if (checkOnDisplayed) {
                    try {
                        if (e.Displayed) {
                            return e;
                        }
                    }
                    catch (StaleElementReferenceException) {
                        return default;
                    }
                }
                else {
                    return e;
                }
                return default;
            }

            try {
                var element = wait.Until(WaitForElementIml);
                return element;
            } catch (WebDriverTimeoutException) {
                try {
                    var dataTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss-fff");
                    session.FindElement(by);
                    var errorDescription = $"Element found but not visible\r\nretries: {retries}\r\nDate time: {dataTime}";
                    var uniqueId = ArtifactLogs.CreateArtifactLog("WaitForElement", errorDescription, session, by.ToString(), extraErrorDescriptionToLog);
                    throw new ElementFoundButNotVisibleException(uniqueId, by);
                } catch (NoSuchElementException) {
                    var dataTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss-fff");
                    var errorDescription = $"Element not found\r\nretries: {retries}\r\nDate time: {dataTime}";
                    var uniqueId = ArtifactLogs.CreateArtifactLog("WaitForElement", errorDescription, session, by.ToString(), extraErrorDescriptionToLog);
                    throw new ElementNotFoundException(uniqueId, retries, by);
                }
            }
        }

        /// <summary>
        /// try find element.
        /// if element not found, then ok and no any exception.
        /// if element found, then wait when is Disappear or after 20 sec call exception
        /// Note: Displayed -  status not check.
        /// </summary>
        /// <param name="by">XPath or other select</param>
        /// <param name="session"></param>
        /// <param name="retries">How many retry (</param>
        /// <param name="time">How many delay after any retry</param>
        /// <param name="secDelayOnStart">Delay before starting checking</param>
        /// <exception cref="Exception"></exception>
        public static void WaitElementDisappear(this By by, IWebDriver session, int retries = 10, int time = 1, int secDelayOnStart = 0) {
            // delay on start
            Thread.Sleep(secDelayOnStart);

            // check element is found or not.
            try {
                session.FindElement(by);
            }
            catch (Exception) {
                // element not found, all good
                return;
            }
            
            //Element found, wait when element is lost.
            var attempt = 0;
            do {
                try {
                    session.FindElement(by);
                }
                catch (Exception) {
                    return;
                }

                attempt++;
                if (attempt >= retries) {
                    var uniqueId = ArtifactLogs.CreateArtifactLog($"WaitElementDisappear", $"\r\nThe element will find and not disappear", session, by.ToString());
                    throw new ElementFoundBytNotDisappear(uniqueId, by, retries);
                }

                Thread.Sleep(TimeSpan.FromSeconds(time));
            } while (true);
        }

        /// <summary>
        /// try find element and check is Displayed
        /// if element (Displayed true) not found, then ok and no any exception.
        /// if element (Displayed true) found, then wait when is Disappear, Displayed true => false or after 20 sec call exception
        /// Note: Displayed -  status check.
        /// </summary>
        /// <param name="by"></param>
        /// <param name="session"></param>
        /// <param name="retries"></param>
        /// <param name="time"></param>
        /// <param name="secDelayOnStart"></param>
        public static void WaitElementVisualDisappear(this By by, IWebDriver session, int retries = 30, int time = 1, int secDelayOnStart = 0) {
            // delay on start
            Thread.Sleep(secDelayOnStart);
            IWebElement element;
            
            // check element is found or not.
            try {
                element = session.FindElement(by);
                if (element.Displayed == false) {
                    return;
                }
            }
            catch (Exception) {
                // element not found, all good
                return;
            }
            
            //Element found, wait when element is lost.
            var attempt = 0;
            do {
                try {
                    element = session.FindElement(by);
                    if (element.Displayed == false) {
                        return;
                    }
                }
                catch (Exception) {
                    return;
                }

                attempt++;
                if (attempt >= retries) {
                    var uniqueId = ArtifactLogs.CreateArtifactLog($"WaitElementDisappear", "FaceErrorDescription: \r\n{fakeErrorDescription}\r\nThe element will find and not disappear", session, by.ToString());
                    throw new ElementFoundBytNotVisualDisappear(uniqueId, by);
                }

                Thread.Sleep(TimeSpan.FromSeconds(time));
            } while (true);
        }
    }
}