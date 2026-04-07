using Microsoft.Playwright;
using NUnit.Framework;

namespace SauceDemoTests.Pages
{
    public class InventoryPage
    {
        private readonly IPage _page;

        public InventoryPage(IPage page) => _page = page;

        private ILocator ProductList => _page.Locator(".inventory_item");
        private ILocator CartBadge => _page.Locator(".shopping_cart_badge");
        private ILocator MenuButton => _page.Locator("#react-burger-menu-btn");
        private ILocator LogoutLink => _page.Locator("#logout_sidebar_link");

        // Actions
        public async Task AddProductToCartAsync(string productId) =>
            await _page.ClickAsync($"#add-to-cart-{productId}");

        public async Task GoToCartAsync() =>
            await _page.ClickAsync(".shopping_cart_link");

        public async Task LogoutAsync()
        {
            await MenuButton.ClickAsync();
            await LogoutLink.ClickAsync();
        }

        // Verification
        public async Task VerifyProductListVisibleAsync()
        {
            Assert.IsTrue(await ProductList.IsVisibleAsync());
        }

        public async Task VerifyCartBadgeAsync(int expectedCount)
        {
            string badgeText = await CartBadge.TextContentAsync();
            Assert.AreEqual(expectedCount.ToString(), badgeText);
        }
    }
}