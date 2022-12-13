using Dynatech.Test.Common.FixtureFile;
using Dynatech.Test.PageObject;
using Xunit;

namespace Dynatech.Test.Configs {
    [CollectionDefinition(NAME)]
    public class CollectionTests: ICollectionFixture<FixtureTests> {
        public const string NAME = "Dynatech Test";
    }

    public class FixtureTests: BrowserSessionFixture {
        public FixtureTests() {
            var browserSessionFixtureConfig = new BrowserSessionFixtureConfig();
            CreateSession(browserSessionFixtureConfig);
            
            // go to search page
            Session.Navigate().GoToUrl("https://www.oojo.com/result/NYC-LHE/2023-01-07/business");
            
            AcceptCookiesPage.AcceptAllCookies(Session); 
            MemberSubscriptionPage.ClickOnCross(Session);
        }
    }
}    