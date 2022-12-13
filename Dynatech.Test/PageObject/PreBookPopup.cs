using Dynatech.Test.Common;
using Dynatech.Test.Common.Exceptions;
using Dynatech.Test.Common.Extensions;
using Dynatech.Test.Common.FixtureFile;

namespace Dynatech.Test.PageObject; 

public class PreBookPopup: FixtureSessionBase {
    public PreBookPopup(FixtureSessionBase fixtureSessionBase): base(fixtureSessionBase) {
    }

    private HtmlSelect RootPage => new(Session, "//*[contains(@class, 'MuiPaper-root')]");
    private HtmlSelect PageTitleText => RootPage.AppendXpath("//div[text()='Your trip to ']");
    private HtmlSelect BookFlightButton => RootPage.AppendXpath("//button[./div[text()='Book Flight']]");
    
    public void WaitPageLoad() {
        RootPage.AssertDisplayedShouldBeTrue("Check pre-book popup is open");
        PageTitleText.AssertDisplayedShouldBeTrue("Check page 'pre-book popup' contain head text");
    }

    public void ComparePriceMatches(string expectedPrice) {
        var actualPrice = RootPage.AppendXpath("//div[contains(@class, 'price')]").GetElement().GetFullText();

        if (expectedPrice != actualPrice) {
            var errorDescription = $"Price in result page do not match with price in pre-book popup.\r\nExpected:'{expectedPrice}'\r\nActual:'{actualPrice}'";
            var uniqueId = ArtifactLogs.CreateArtifactLog("ComparePriceMatches", errorDescription, Session);
            throw new ValueCompareException(uniqueId, errorDescription);
        }
    }

    public void ClickOnBookFlight() {
        BookFlightButton.ClickWrapper();
    }
}