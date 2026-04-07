using TechTalk.SpecFlow;
using SauceDemoTests.Pages;
using Microsoft.Playwright;
using NUnit.Framework;

namespace SauceDemoTests.StepDefinitions
{
    [Binding]
    public class SauceDemoNegativeSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private LoginPage _loginPage;
        private InventoryPage _inventoryPage;
        private CartPage _cartPage;
        private CheckoutPage _checkoutPage;

        public SauceDemoNegativeSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            var page = _scenarioContext.Get<IPage>("PAGE");
            _loginPage = new LoginPage(page);
            _inventoryPage = new InventoryPage(page);
            _cartPage = new CartPage(page);
            _checkoutPage = new CheckoutPage(page);
        }

        // ============================
        // LOGIN NEGATIVE STEPS
        // ============================
        [Then(@"I should see the login error message ""(.*)""")]
        public async Task ThenIShouldSeeLoginErrorMessageAsync(string expectedMessage)
        {
            await _loginPage.VerifyErrorMessageAsync(expectedMessage);
        }

        // ============================
        // CART NEGATIVE STEPS
        // ============================
        [When(@"I go to the cart page")]
        public async Task WhenIGoToCartPageAsync() => await _inventoryPage.GoToCartAsync();

        [Then(@"I should see the checkout page error message ""(.*)""")]
        public async Task ThenIShouldSeeCheckoutPageErrorMessageAsync(string expectedMessage)
        {
            // Sauce Demo does not allow checkout if cart is empty, simulate error
            var page = _scenarioContext.Get<IPage>("PAGE");
            bool isButtonDisabled = await page.IsDisabledAsync("#checkout");
            Assert.IsTrue(isButtonDisabled, expectedMessage);
        }

        // ============================
        // CHECKOUT NEGATIVE STEPS
        // ============================
        [When(@"I enter checkout info with ""(.*)"" ""(.*)"" ""(.*)""")]
        public async Task WhenIEnterCheckoutInfoAsync(string firstName, string lastName, string postalCode)
        {
            await _checkoutPage.EnterCheckoutInfoAsync(firstName, lastName, postalCode);
        }

        [When(@"I click the continue button")]
        public async Task WhenIClickContinueButtonAsync() => await _checkoutPage.ContinueAsync();

        [Then(@"I should see the checkout error message ""(.*)""")]
        public async Task ThenIShouldSeeCheckoutErrorMessageAsync(string expectedMessage)
        {
            var page = _scenarioContext.Get<IPage>("PAGE");
            var errorLocator = page.Locator("[data-test='error']");
            Assert.AreEqual(expectedMessage, await errorLocator.TextContentAsync());
        }
    }
}