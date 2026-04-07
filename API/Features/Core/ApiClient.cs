using System.Net.Http;
using System.Text;
using System.Text.Json;

public class PetStoreClient
{
    private readonly HttpClient _client;
    public PetStoreClient()
    {
        _client = new HttpClient { BaseAddress = new Uri("https://petstore.swagger.io/v2/") };
    }

    public async Task<HttpResponseMessage> CreatePetAsync(string name, string status)
    {
        var pet = new { name, status };
        var content = new StringContent(JsonSerializer.Serialize(pet), Encoding.UTF8, "application/json");
        return await _client.PostAsync("pet", content);
    }

    public async Task<HttpResponseMessage> CreatePetInvalidAsync()
    {
        var content = new StringContent("{ invalidJson }", Encoding.UTF8, "application/json");
        return await _client.PostAsync("pet", content);
    }
}