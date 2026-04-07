using Microsoft.Playwright;
using NUnit.Framework;

namespace SauceDemoTests.Pages
{
    public class CartPage
    {
        private readonly IPage _page;

        public CartPage(IPage page) => _page = page;

        private ILocator CheckoutButton => _page.Locator("#checkout");
        private ILocator CartItems => _page.Locator(".cart_item");

        // Actions
        public async Task CheckoutAsync() => await CheckoutButton.ClickAsync();

        // Verification
        public async Task VerifyProductInCartAsync(string productName)
        {
            Assert.IsTrue(await _page.Locator($"text={productName}").IsVisibleAsync());
        }

        public async Task VerifyCartCountAsync(int expectedCount)
        {
            Assert.AreEqual(expectedCount, await CartItems.CountAsync());
        }
    }
}