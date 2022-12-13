using System.Drawing;
using Dynatech.Test.Common.Common;
using Dynatech.Test.Common.Config;
using Dynatech.Test.Common.EnumType;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Serilog;

namespace Dynatech.Test.Common.FixtureFile {
    public class BrowserSessionFixtureConfig {
        public Size BrowserSize = new Size(1800, 1000);
        public readonly MonitorPosition BrowserMonitorPosition = AutotestConfigs.MonitorPosition;
        public bool EnableHeadLess = AutotestConfigs.EnableHeadless;
        public string GoToUrlString = "";
    }

    public class BrowserSessionFixture: FixtureSessionBase, IDisposable {
        private BrowserSessionFixtureConfig browserSessionFixtureConfig;

        public void Dispose() {
            Session?.Quit();
            Session?.Dispose();
        }

        public BrowserSessionFixture() {
            TestInfo = new TestInfo();
        }

        public void CreateSession(BrowserSessionFixtureConfig browserSessionFixtureConfigValue) {
            browserSessionFixtureConfig = browserSessionFixtureConfigValue;

            var config = new LoggerConfiguration();
            Logger = config.CreateLogger().ForContext(GetType());

            Session = GetChromeSession();

            // Prepare Browser start position
            Session.Manage().Window.Position = browserSessionFixtureConfig.BrowserMonitorPosition switch {
                MonitorPosition.LEFT => new Point(-1900, 25),
                MonitorPosition.CENTER => new Point(25, 25),
                MonitorPosition.RIGHT => new Point(1945, 25),
                _ => throw new ArgumentOutOfRangeException()
            };

            Session.Manage().Window.Size = browserSessionFixtureConfig.BrowserSize;
        }

        private IWebDriver GetChromeSession() {
            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            if (browserSessionFixtureConfig.EnableHeadLess) {
                options.AddArgument("--headless");
            }

            options.AddArgument("--disable-extensions");
            options.AddArguments("--allow-running-insecure-content");
            options.AddArguments("--disable-web-security");
            options.AddArgument("--enable-javascript");
            options.AddArgument("--disable-dev-shm-usage");

            options.SetLoggingPreference(LogType.Browser, LogLevel.All);

            var settingForHeadlessDownloads = new Dictionary<string, object> {
                { "behavior", "allow" },
            };

            var service = ChromeDriverService.CreateDefaultService(AutotestConfigs.ChromeDriverUrl);
            service.WhitelistedIPAddresses = " ";

            var driver = new ChromeDriver(service, options);
            if (browserSessionFixtureConfig.EnableHeadLess) {
                //driver.ExecuteChromeCommand("Page.setDownloadBehavior", settingForHeadlessDownloads);
                driver.ExecuteCdpCommand("Page.setDownloadBehavior", settingForHeadlessDownloads);
            }

            return driver;
        }
    }
}