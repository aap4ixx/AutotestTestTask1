using System.Collections.ObjectModel;
using System.Text;
using Dynatech.Test.Common.Exceptions;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;


namespace Dynatech.Test.Common.Extensions {
    public static class WebElementExtensions {
        public static void CheckElementIsClickable(this IWebElement element, IWebDriver session, int time = 10) {
            try {
                var wait = new WebDriverWait(session, TimeSpan.FromSeconds(time));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element));
            }
            catch (Exception e) {
                var uniqueId = ArtifactLogs.CreateArtifactLog("CheckElementIsClickable", $"retries: '{time}'\r\n{e}", session);
                throw new ElementNotClickableException(uniqueId, time, e.ToString());
            }
        }

        public static void MoveFocusToElement(this IWebElement element, IWebDriver session, int retries = 10) {
            var attempt = 0;
            do {
                try {
                    var actions = new OpenQA.Selenium.Interactions.Actions(session);
                    actions.MoveToElement(element);
                    actions.Perform();
                    
                    return;
                }
                catch (Exception e) {
                    attempt++;
                    if (attempt >= retries) {
                        var elementSize = element.Size;
                        var elementLocation = element.Location;

                        var errorDescription =
                            $"Move focus to element failed\r\nRetry: {retries}\r\n{e}\r\nElement location:{elementLocation.X}; {elementLocation.Y}\r\nElement size: {elementSize.Height}; {elementSize.Width}";

                        var uniqueId = ArtifactLogs.CreateArtifactLog("MoveFocusToElement", errorDescription, session);
                        throw new FocusToElementFailedException(uniqueId, errorDescription);
                    }

                    Thread.Sleep(200);
                }
            } while (true);
        }

        public static void ClickWrapper(this IWebElement element, IWebDriver session) {
            try {
                element.CheckElementIsClickable(session);
                element.Click();
                return;
            }
            catch (ElementClickInterceptedException) {
                element.CheckElementNotMove(session);
            }
            catch (Exception e) {
                var uniqueId = ArtifactLogs.CreateArtifactLog("ClickWrapper_1", $"Error description:\r\n{e}", session);
                throw new ElementNotClickableException(uniqueId, e.ToString());
            }

            try {
                element.Click();
            }
            catch (Exception e) {
                var uniqueId = ArtifactLogs.CreateArtifactLog("ClickWrapper_2", $"Error description:\r\n{e}", session);
                throw new ElementNotClickableException(uniqueId, e.ToString());
            }
        }

        public static string GetHtmlText(this IWebElement webElement, HtmlTextType htmlTextType = HtmlTextType.SHORT) {
            switch (htmlTextType) {
                case HtmlTextType.ORIGINAL: {
                    return webElement.GetAttribute("innerHTML");
                }
                case HtmlTextType.SHORT: {
                    var result = webElement.GetAttribute("innerHTML");
                    return result.Replace("\r\n", " ");
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(htmlTextType), htmlTextType, null);
            }
        }

        public static string GetFullText(this IWebElement webElement, string splitText = "") {
            var innerHtml = webElement.GetAttribute("innerHTML");

            var listText = new List<string>();
            var fullText = new StringBuilder();

            var flagText = true;
            foreach (var oneChar in innerHtml) {
                if (oneChar == '<') {
                    flagText = false;

                    if (fullText.Length != 0) {
                        listText.Add(fullText.ToString());
                        fullText.Clear();
                    }

                    continue;
                }

                if (oneChar == '>') {
                    flagText = true;
                    continue;
                }

                if (flagText) {
                    fullText.Append(oneChar);
                }
            }

            if (fullText.Length != 0) {
                listText.Add(fullText.ToString());
                fullText.Clear();
            }

            return string.Join(splitText, listText);
        }

        public static ReadOnlyCollection<IWebElement> FindElements(this IWebElement webElement, string xPath) {
            return webElement.FindElements(By.XPath(xPath));
        }

        public static IWebElement FindElement(this IWebElement webElement, string xPath) {
            return webElement.FindElement(By.XPath(xPath));
        }

        public static void CheckElementNotMove(this IWebElement webElement, IWebDriver session, int retries = 3) {
            var attempt = 0;

            var elementLocation = webElement.Location;
            Thread.Sleep(100);

            do {
                var newElementLocation = webElement.Location;

                if (elementLocation.X == newElementLocation.X && elementLocation.Y == newElementLocation.Y) {
                    return;
                }

                elementLocation = newElementLocation;

                attempt++;
                if (attempt >= retries) {
                    var errorDescription = new StringBuilder();
                    errorDescription.Append("Element not stop\r\n");
                    errorDescription.Append($"retries: {retries}");

                    var uniqueId = ArtifactLogs.CreateArtifactLog("CheckElementNotMove", errorDescription.ToString(), session);
                    throw new ElementIsNotStopException(uniqueId, errorDescription.ToString());
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            } while (true);
        }

        /// <summary>
        /// Check element is Displayed. If no then error
        /// </summary>
        /// <param name="element"></param>
        /// <param name="session"></param>
        /// <param name="errorMessage"></param>
        /// <exception cref="StaleElementReferenceException"></exception>
        public static void AssertDisplayedShouldBeTrue(this IWebElement element, IWebDriver session, string errorMessage = "") {
            var attempt = 0;
            while (true) {
                try {
                    element.Displayed.Should().BeTrue(errorMessage);
                    break;
                }
                catch (Exception ex) {
                    attempt++;
                    if (attempt >= 10) {
                        var uniqueId = ArtifactLogs.CreateArtifactLog("AssertDisplayedShouldBeTrue",
                            $"{attempt.ToString()},\r\n Exception thrown: '{ex.Message}'", session);
                        throw new ElementFoundButNotVisibleException(uniqueId, "No Xpath", errorMessage);
                    }

                    Thread.Sleep(500);
                }
            }
        }
    }
}