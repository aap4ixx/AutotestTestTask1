using System.Globalization;
using Dynatech.Test.Common;
using Dynatech.Test.Common.Exceptions;
using Dynatech.Test.Common.Extensions;
using Dynatech.Test.Common.FixtureFile;
using OpenQA.Selenium;
using Serilog;
using Serilog.Core;

namespace Dynatech.Test.PageObject.SearchResultObjects; 

public class SearchResultBusiness: FixtureSessionBase {
    public record OfferBox(string companyName, string flyPrice, string departureDate);
    private List<OfferBox> lineList;

    public SearchResultBusiness(FixtureSessionBase fixtureSessionBase): base(fixtureSessionBase) {
    }

    public void ReadData() {
        lineList = new List<OfferBox>();

        // take all lines
        var lineListElementXpath = "//div[./button[text()='Show more results']]/div[not(@class)]";
        var lineListElement = lineListElementXpath.FindElements(Session);

        foreach (var item in lineListElement) {
            var companyName = item.FindElement(".//*[@class='oojo-tooltip-target pos-rlt']").GetFullText();
            var flyPrice = item.FindElement(".//div[contains(@class, 'bookingInfo_desktop_pqBookingInfo_')]/div[2]/div[2]").GetFullText();
            var departureDate = item.FindElement(".//*[@data-qa='pqDateFrom']").GetFullText();
            
            lineList.Add(new OfferBox(companyName, flyPrice, departureDate));
        }
        
        Logger.Information($"found: '{lineList.Count}'");
    }

    public void CheckFlightPricePresent() {
        if (lineList.Any(x => x.flyPrice == "")) {
            var errorDescription = "Result contain empty price";
            var uniqueId = ArtifactLogs.CreateArtifactLog("CheckFlightPricePresent'", errorDescription, Session);
            throw new CustomException(uniqueId, errorDescription);
        }
    }

    public string GetPrice(int boxNumber) {
        return lineList[boxNumber-1].flyPrice;
    }

    public void CheckDepartureDate(DateTime checkDate) {
        var checkDateText = checkDate.ToString("ddd, MMM d", CultureInfo.InvariantCulture);

        foreach (var item in lineList) {
            if (item.departureDate != checkDateText) {
                var errorDescription = $"One departure date contain wrong date\r\nExpected:'{checkDateText}'\r\nActual:'{item.departureDate}'";
                var uniqueId = ArtifactLogs.CreateArtifactLog("CheckDepartureDate'", errorDescription, Session);
                throw new CustomException(uniqueId, errorDescription);
            }
        }
    }
}