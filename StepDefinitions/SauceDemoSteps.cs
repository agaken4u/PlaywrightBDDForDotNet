using TechTalk.SpecFlow;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace SauceDemoTests.StepDefinitions
{
    [Binding]
    public class SauceDemoSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private IPage _page;
        private IBrowser _browser;

        public SauceDemoSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        // ======================
        // HOOKS
        // ======================
        [BeforeScenario]
        public async Task Setup()
        {
            var playwright = await Playwright.CreateAsync();
            _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();

            _scenarioContext["PAGE"] = _page;
            _scenarioContext["BROWSER"] = _browser;
        }

        [AfterScenario]
        public async Task TearDown()
        {
            if (_browser != null)
                await _browser.CloseAsync();
        }

        // ======================
        // LOGIN
        // ======================
        [Given(@"I am on the Sauce Demo login page")]
        public async Task GivenIAmOnLoginPage()
        {
            await _page.GotoAsync("https://www.saucedemo.com/");
        }

        [When(@"I enter username ""(.*)""")]
        public async Task WhenIEnterUsername(string username)
        {
            await _page.FillAsync("#user-name", username);
        }

        [When(@"I enter password ""(.*)""")]
        public async Task WhenIEnterPassword(string password)
        {
            await _page.FillAsync("#password", password);
        }

        [When(@"I click the login button")]
        public async Task WhenIClickLogin()
        {
            await _page.ClickAsync("#login-button");
        }

        [Given(@"I am logged in as ""(.*)"" with password ""(.*)""")]
        public async Task GivenIAmLoggedIn(string username, string password)
        {
            await GivenIAmOnLoginPage();
            await WhenIEnterUsername(username);
            await WhenIEnterPassword(password);
            await WhenIClickLogin();
            await _page.WaitForURLAsync("**/inventory.html");
        }

        [Then(@"I should be redirected to the inventory page")]
        public async Task ThenIShouldBeRedirectedToInventory()
        {
            await _page.WaitForURLAsync("**/inventory.html");
            Assert.IsTrue(_page.Url.Contains("inventory"));
        }

        [Then(@"I should see the product list on the inventory page")]
        public async Task ThenIShouldSeeProductList()
        {
            var count = await _page.Locator(".inventory_item").CountAsync();
            Assert.IsTrue(count > 0);
        }

        // ======================
        // PRODUCTS / CART
        // ======================
        [Given(@"I add ""(.*)"" to the cart")]
        [When(@"I add ""(.*)"" to the cart")]
        public async Task AddProductToCart(string productId)
        {
            await _page.ClickAsync($"#add-to-cart-{productId}");
        }

        [When(@"I remove ""(.*)"" from the cart")]
        public async Task WhenIRemoveProductFromCart(string productId)
        {
            await _page.ClickAsync($"#remove-{productId}");
        }

        [When(@"I open the cart")]
        public async Task WhenIOpenCart()
        {
            await _page.ClickAsync(".shopping_cart_link");
        }

        [Then(@"the cart badge should display ""(.*)""")]
        public async Task ThenCartBadgeShouldDisplay(string expectedCount)
        {
            var badge = _page.Locator(".shopping_cart_badge");
            var text = await badge.InnerTextAsync();
            Assert.AreEqual(expectedCount, text);
        }

        [Then(@"I should see ""(.*)"" in the cart")]
        public async Task ThenIShouldSeeProductInCart(string productName)
        {
            var item = _page.Locator(".inventory_item_name", new PageLocatorOptions
            {
                HasTextString = productName
            });
            Assert.IsTrue(await item.IsVisibleAsync());
        }

        // ======================
        // CHECKOUT
        // ======================
        [When(@"I complete checkout with ""(.*)"", ""(.*)"", ""(.*)""")]
        public async Task WhenICompleteCheckout(string firstName, string lastName, string postalCode)
        {
            await WhenIOpenCart();
            await _page.ClickAsync("#checkout");

            await _page.FillAsync("#first-name", firstName);
            await _page.FillAsync("#last-name", lastName);
            await _page.FillAsync("#postal-code", postalCode);

            await _page.ClickAsync("#continue");
            await _page.ClickAsync("#finish");
        }

        [Then(@"I should see the order confirmation message ""(.*)""")]
        public async Task ThenIShouldSeeOrderConfirmation(string expectedMessage)
        {
            var confirmation = _page.Locator(".complete-header");
            var text = await confirmation.InnerTextAsync();
            Assert.AreEqual(expectedMessage, text);
        }

        // ======================
        // LOGOUT
        // ======================
        [When(@"I logout")]
        public async Task WhenILogout()
        {
            await _page.ClickAsync("#react-burger-menu-btn");
            await _page.ClickAsync("#logout_sidebar_link");
        }

        [Then(@"I should be redirected to the login page")]
        public void ThenIShouldBeRedirectedToLogin()
        {
            Assert.IsTrue(_page.Url.Contains("saucedemo.com"));
        }
    }
}