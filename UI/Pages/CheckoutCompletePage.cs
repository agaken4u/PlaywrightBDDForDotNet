using Microsoft.Playwright;
using NUnit.Framework;

namespace SauceDemoTests.Pages
{
    public class CheckoutCompletePage
    {
        private readonly IPage _page;
        private ILocator CompleteHeader => _page.Locator(".complete-header");

        public CheckoutCompletePage(IPage page) => _page = page;

        public async Task VerifyOrderCompleteAsync(string expectedMessage)
        {
            string actual = await CompleteHeader.TextContentAsync();
            Assert.AreEqual(expectedMessage, actual);
        }
    }
}