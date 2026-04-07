using Microsoft.Playwright;
using NUnit.Framework;

namespace SauceDemoTests.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;
        private readonly string _url = "https://www.saucedemo.com/";

        public LoginPage(IPage page) => _page = page;

        // Locators
        private ILocator Username => _page.Locator("#user-name");
        private ILocator Password => _page.Locator("#password");
        private ILocator LoginButton => _page.Locator("#login-button");
        private ILocator ErrorMessage => _page.Locator("[data-test='error']");

        // Actions
        public async Task GoToAsync() => await _page.GotoAsync(_url);

        public async Task LoginAsync(string username, string password)
        {
            await Username.FillAsync(username);
            await Password.FillAsync(password);
            await LoginButton.ClickAsync();
        }

        // Verification
        public async Task VerifyErrorMessageAsync(string expectedMessage)
        {
            string actualMessage = await ErrorMessage.TextContentAsync();
            Assert.AreEqual(expectedMessage, actualMessage);
        }
    }
}