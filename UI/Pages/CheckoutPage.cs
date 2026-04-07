using Microsoft.Playwright;
using NUnit.Framework;

namespace SauceDemoTests.Pages
{
    public class CheckoutPage
    {
        private readonly IPage _page;

        public CheckoutPage(IPage page) => _page = page;

        private ILocator FirstName => _page.Locator("#first-name");
        private ILocator LastName => _page.Locator("#last-name");
        private ILocator PostalCode => _page.Locator("#postal-code");
        private ILocator ContinueButton => _page.Locator("#continue");
        private ILocator FinishButton => _page.Locator("#finish");

        // Actions
        public async Task EnterCheckoutInfoAsync(string firstName, string lastName, string postalCode)
        {
            await FirstName.FillAsync(firstName);
            await LastName.FillAsync(lastName);
            await PostalCode.FillAsync(postalCode);
        }

        public async Task ContinueAsync() => await ContinueButton.ClickAsync();
        public async Task FinishAsync() => await FinishButton.ClickAsync();
    }
}