using Microsoft.Playwright;
using System.Threading.Tasks;
using TechTalk.SpecFlow;


public class ApiClient
{
    private readonly IAPIRequestContext _request;

    public ApiClient(IAPIRequestContext request)
    {
        _request = request;
    }

    public async Task<IAPIResponse> PostAsync(string url, object data)
    {
        return await _request.PostAsync(url, new APIRequestContextOptions
        {
            DataObject = data
        });
    }

    public async Task<IAPIResponse> GetAsync(string url)
    {
        return await _request.GetAsync(url);
    }

    public async Task<IAPIResponse> DeleteAsync(string url)
    {
        return await _request.DeleteAsync(url);
    }
}