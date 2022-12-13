using Dynatech.Test.Common.Common;
using OpenQA.Selenium;
using Serilog;
using Xunit.Abstractions;

namespace Dynatech.Test.Common.FixtureFile {
    public class FixtureSessionBase {
        public IWebDriver Session { get; set; }
        public ILogger Logger { get;  set; }
        public ILogger LoggerSeq { get;  set; }
        public TestInfo TestInfo { get;  set; }

        public FixtureSessionBase() {
        }
        
        public FixtureSessionBase(FixtureSessionBase fixtureSessionBase) {
            Logger = fixtureSessionBase.Logger;
            LoggerSeq = fixtureSessionBase.LoggerSeq;
            Session = fixtureSessionBase.Session;
            TestInfo = fixtureSessionBase.TestInfo;
        }
        
        public FixtureSessionBase(IWebDriver session, TestInfo testInfo, ILogger logger) {
            Logger = logger;
            Session = session;
            TestInfo = testInfo;
        }
        
        public FixtureSessionBase(BrowserSessionFixture fixture, ITestOutputHelper output) {
            var config = new LoggerConfiguration().WriteTo.TestOutput(output);
            Logger = config.CreateLogger().ForContext(GetType());
            //output.WithReportPortal();
            
            LoggerSeq = new LoggerConfiguration()
                .WriteTo.TestOutput(output)
              //  .WriteTo.Seq(CommonTestConfigs.SEQ_SERVER_ADDRESS_AUTO)
                .CreateLogger();
            
            Session = fixture.Session;
            TestInfo = fixture.TestInfo;
        }

        public FixtureSessionBase(BrowserSessionFixture browserSessionFixture) {
            Session = browserSessionFixture.Session;
            TestInfo = browserSessionFixture.TestInfo;
            Logger = browserSessionFixture.Logger;
        }
    }
}