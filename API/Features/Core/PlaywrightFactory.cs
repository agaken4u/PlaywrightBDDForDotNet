using Microsoft.Playwright;
using System.Threading.Tasks;

public class PlaywrightFactory
{
    public static async Task<IAPIRequestContext> CreateApiContext()
    {
        var playwright = await Playwright.CreateAsync();

        return await playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = "https://petstore.swagger.io/v2"
        });
    }
}