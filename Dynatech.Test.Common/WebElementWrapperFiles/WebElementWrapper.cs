using Dynatech.Test.Common.Exceptions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Dynatech.Test.Common.WebElementWrapperFiles {
    public class WebElementWrapper {
        private string xPath;
        private readonly IWebDriver session;
        private IWebElement webElement;

        private int getElementRetries;
        private int getElementTime;
        private bool getElementCheckOnDisplayed;

        private readonly bool artifactEnable;

        public WebElementWrapper(IWebDriver session, bool artifactEnable = true) {
            this.session = session;
            this.artifactEnable = artifactEnable;
        }

        public IWebElement GetIWebElement() {
            return webElement;
        }

        public string GetAttribute(string attributeText) {
            return webElement.GetAttribute(attributeText);
        }

        /// <summary>
        /// try find element in browser
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ElementFoundButNotVisibleException"></exception>
        /// <exception cref="ElementNotFoundException"></exception>
        private IWebElement GetElementByXPath() {
            var attempt = 0;
            IWebElement element = null;
            while (true) {
                try {
                    element = session.FindElement(By.XPath(xPath));
                    if (getElementCheckOnDisplayed) {
                        if (element.Displayed) {
                            return element;
                        }
                    }
                    else {
                        return element;
                    }
                }
                catch (NoSuchElementException) {
                }
                catch (Exception) {
                    // ignored
                }

                if (attempt >= getElementRetries) {
                    var errorDescription = element != null ? "Element found but not visible" : "Element not found";
                    var uniqueId = -1;

                    if (artifactEnable) {
                        uniqueId = ArtifactLogs.CreateArtifactLog("WaitForElementWrapper", errorDescription, session, xPath);
                    }

                    if (element != null) {
                        throw new ElementFoundButNotVisibleException(uniqueId, xPath, "");
                    }

                    throw new ElementNotFoundException(uniqueId, getElementRetries, xPath);
                }

                attempt++;
                Thread.Sleep(TimeSpan.FromSeconds(getElementTime));
            } //while (true)
        }

        public void WaitForElementWrapper(string xPathText, int retriesCount = 10, int timeSecond = 1, bool checkOnDisplayedFlag = true) {
            xPath = xPathText;

            getElementRetries = retriesCount;
            getElementTime = timeSecond;
            getElementCheckOnDisplayed = checkOnDisplayedFlag;

            webElement = GetElementByXPath();
        }

        private void CheckElementIsClickable(int time = 10) {
            try {
                var wait = new WebDriverWait(session, TimeSpan.FromSeconds(time));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(webElement));
            }
            catch (Exception e) {
                var uniqueId = ArtifactLogs.CreateArtifactLog("CheckElementIsClickable", $"retries: '{time}'\r\n{e}", session);
                throw new ElementNotClickableException(uniqueId, time, e.ToString());
            }
        }

        /// <summary>
        /// Click on element
        /// 1. Check element is clickable
        /// 2. If element miffing, try find again.
        /// </summary>
        /// <param name="verificationXPath"></param>
        /// <param name="retriesCount"></param>
        /// <param name="innerErrorDescription"></param>
        /// <exception cref="ElementNotClickableException"></exception>
        public void Click(int retriesCount = 10, string verificationXPath = null, string innerErrorDescription = null) {
            var attempt = 0;

            CheckElementIsClickable();

            do {
                var exceptionMessage = "";
                try {
                    webElement.Click();
                    
                    if (verificationXPath != null) {
                        // check verificationElement
                        var checkVerificationElement = new WebElementWrapper(session, false);
                        checkVerificationElement.WaitForElementWrapper(verificationXPath, 3);
                    }
                    
                    return;
                }

                //OpenQA.Selenium.StaleElementReferenceException: stale element reference: element is not attached to the page document
                catch (StaleElementReferenceException e) {
                    exceptionMessage = e.ToString();
                    webElement = GetElementByXPath();
                }

                catch (Exception e) {
                    exceptionMessage = e.ToString();
                }

                if (attempt >= retriesCount) {
                    var errorDescription = $"retries: '{retriesCount}'\r\nxPath: '{xPath}'\r\n{exceptionMessage}";
                    var errorDescriptionForLogs = errorDescription;

                    if (innerErrorDescription != null) {
                        errorDescriptionForLogs += $"\r\ninner error description:\r\n{innerErrorDescription}";
                    }

                    var uniqueId = -1;
                    if (artifactEnable) {
                        uniqueId = ArtifactLogs.CreateArtifactLog("WebElementWrapper.Click", errorDescriptionForLogs, session);
                    }

                    throw new ElementNotClickableException(uniqueId, retriesCount, errorDescription);
                }

                attempt++;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            } while (true);
        }

        /// <summary>
        /// Move mouse to element.
        /// </summary>
        /// <param name="retries"></param>
        /// <param name="verificationXPath">If XPath not null, then after moving the mouse to element, the verificationXPath is checked (They are trying to find a verification element by XPath) </param>
        /// <param name="checkAfterMoveIsVisible">after move mouse to element check, element is visible or not. if not then again move mouse to element.</param>
        /// <exception cref="FocusToElementFailedException"></exception>
        public void MoveFocusToElement(int retries = 3, string verificationXPath = null, bool checkAfterMoveIsVisible = false) {
            var attempt = 0;

            do {
                var errorDescription = "";
                try {
                    var actions = new Actions(session);
                    actions.MoveToElement(webElement);
                    actions.Perform();

                    if (verificationXPath != null) {
                        // check verificationElement
                        var checkVerificationElement = new WebElementWrapper(session, false);
                        checkVerificationElement.WaitForElementWrapper(verificationXPath, 3);
                    }

                    if (checkAfterMoveIsVisible) {
                        if (webElement.Displayed == false) {
                            throw new ElementNotVisibleException();
                        }
                    }

                    return;
                }

                //OpenQA.Selenium.StaleElementReferenceException: stale element reference: element is not attached to the page document
                catch (StaleElementReferenceException) {
                    errorDescription = "Stale element reference: element is not attached to the page document";
                    webElement = GetElementByXPath();
                }

                catch (ElementNotFoundException) {
                    errorDescription = "Verification element not found";
                }

                catch (ElementFoundButNotVisibleException) {
                    errorDescription = "Verification element found but not visible.";
                }

                catch (Exception e) {
                    errorDescription = e.ToString();
                }

                if (attempt >= retries) {
                    errorDescription += $"\r\nXpath: '{xPath}'\r\nverificationXPath:{verificationXPath}\r\nretries{retries}";
                    var uniqueId = -1;
                    if (artifactEnable) {
                        uniqueId = ArtifactLogs.CreateArtifactLog("MoveFocusToElement",
                            $"Move focus to element failed.\r\nError description:\r\n'{errorDescription}'", session);
                    }

                    throw new FocusToElementFailedException(uniqueId, errorDescription);
                }

                attempt++;
                Thread.Sleep(500);
            } while (true);
        }
  
        public void RightClickWithCheckResult(string verificationXPath, int retriesCount = 10, string innerErrorDescription = null) {
            var attempt = 0;

            CheckElementIsClickable();

            do {
                var exceptionMessage = "";
                try {
                    // Right mouse click.
                    var action = new Actions(session);
                    action.ContextClick(webElement).Perform();

                    var webElementWrapper = new WebElementWrapper(session, false);
                    webElementWrapper.WaitForElementWrapper(verificationXPath, 4);
                    return;
                }

                //OpenQA.Selenium.StaleElementReferenceException: stale element reference: element is not attached to the page document
                catch (StaleElementReferenceException e) {
                    exceptionMessage = e.ToString();
                    webElement = GetElementByXPath();
                }

                catch (Exception e) {
                    exceptionMessage = e.ToString();
                }

                if (attempt >= retriesCount) {
                    var errorDescription = $"retries: '{retriesCount}'\r\nxPath: '{xPath}'\r\n{exceptionMessage}";
                    var errorDescriptionForLogs = errorDescription;

                    if (innerErrorDescription != null) {
                        errorDescriptionForLogs += $"inner error description:\r\n{errorDescription}";
                    }

                    var uniqueId = -1;
                    if (artifactEnable) {
                        uniqueId = ArtifactLogs.CreateArtifactLog("WebElementWrapper.ClickWithCheckResult", errorDescriptionForLogs, session);
                    }

                    throw new ElementNotClickableException(uniqueId, retriesCount, errorDescription);
                }

                attempt++;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            } while (true);
        }

        public void SendKeys(string keysString, int retriesCount = 10, string verificationXPath = null, string innerErrorDescription = null) {
            var attempt = 0;

            do {
                var exceptionMessage = "";

                try {
                    webElement.SendKeys(keysString);

                    if (verificationXPath != null) {
                        var webElementWrapper = new WebElementWrapper(session, false);
                        webElementWrapper.WaitForElementWrapper(verificationXPath, 4);
                    }

                    return;
                }
                catch (Exception e) {
                    exceptionMessage = e.ToString();
                }

                if (attempt >= retriesCount) {
                    var errorDescription = $"retries: '{retriesCount}'\r\nxPath: '{xPath}'\r\n{exceptionMessage}";
                    var errorDescriptionForLogs = errorDescription;

                    if (innerErrorDescription != null) {
                        errorDescriptionForLogs += $"inner error description:\r\n{errorDescription}";
                    }

                    var uniqueId = -1;
                    if (artifactEnable) {
                        uniqueId = ArtifactLogs.CreateArtifactLog("WebElementWrapper.SendKeys", errorDescriptionForLogs, session);
                    }

                    throw new ElementNotClickableException(uniqueId, retriesCount, errorDescription);
                }

                attempt++;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            } while (true);
        }

        public void IsDisplayed(string errorDescription) {
            if (webElement.Displayed != true) {
            }
        }
    }
}