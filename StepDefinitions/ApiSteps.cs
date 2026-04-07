using TechTalk.SpecFlow;
using FluentAssertions;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AventStack.ExtentReports;

namespace PlaywrightBDDFramework.API.Steps
{
    [Binding]
    public class PetApiStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly HttpClient _client;
        private HttpResponseMessage _response;
        private Stopwatch _stopwatch;
        private ExtentTest _test;

        public PetApiStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;

            _client = new HttpClient
            {
                BaseAddress = new System.Uri("https://petstore.swagger.io/v2/")
            };

            _test = (ExtentTest)_scenarioContext["ExtentTest"];
        }

        // ================================
        // HELPER: Normalize Endpoint
        // ================================
        private string NormalizeEndpoint(string endpoint)
        {
            return endpoint.TrimStart('/');
        }

        // ================================
        // HELPER: Logging
        // ================================
        private async Task LogRequestAndResponse(string stepName, string payload = null)
        {
            var responseBody = await _response.Content.ReadAsStringAsync();

            _test.Info($"🔹 {stepName}");
            if (!string.IsNullOrEmpty(payload))
                _test.Info($"Request Payload:\n{payload}");

            _test.Info($"Response Status: {(int)_response.StatusCode}");
            _test.Info($"Response Body:\n{responseBody}");
        }

        // ================================
        // GIVEN
        // ================================
        [Given(@"the Petstore API is available")]
        public async Task GivenTheApiIsAvailable()
        {
            var endpoint = NormalizeEndpoint("/pet/1");
            var response = await _client.GetAsync(endpoint);
            response.Should().NotBeNull();
            _test.Info($"API Check URL: {_client.BaseAddress}{endpoint}");
        }

        [Given(@"a pet exists with id (.*)")]
        public async Task GivenPetExists(int id)
        {
            var endpoint = NormalizeEndpoint("/pet");
            var payload = new
            {
                id = id,
                name = "TestPet",
                photoUrls = new[] { "https://example.com/photo.jpg" },
                status = "available"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _response = await _client.PostAsync(endpoint, content);
            await LogRequestAndResponse("Create Pet (Setup)", json);
        }

        [Given(@"a pet with id (.*) has been deleted")]
        public async Task GivenPetDeleted(int id)
        {
            var endpoint = NormalizeEndpoint($"/pet/{id}");
            _response = await _client.DeleteAsync(endpoint);
            await LogRequestAndResponse("Delete Pet (Setup)");
        }

        // ================================
        // WHEN
        // ================================
        [When(@"I send a POST request to ""(.*)"" with payload:")]
        public async Task WhenISendPostWithPayload(string endpoint, string payload)
        {
            endpoint = NormalizeEndpoint(endpoint);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            _stopwatch = Stopwatch.StartNew();
            _response = await _client.PostAsync(endpoint, content);
            _stopwatch.Stop();

            _test.Info($"Final URL: {_client.BaseAddress}{endpoint}");
            await LogRequestAndResponse($"POST {endpoint}", payload);
        }

        [When(@"I send a PUT request to ""(.*)"" with payload:")]
        public async Task WhenISendPutWithPayload(string endpoint, string payload)
        {
            endpoint = NormalizeEndpoint(endpoint);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            _response = await _client.PutAsync(endpoint, content);
            await LogRequestAndResponse($"PUT {endpoint}", payload);
        }

        [When(@"I send a GET request to ""(.*)""")]
        public async Task WhenISendGetRequest(string endpoint)
        {
            endpoint = NormalizeEndpoint(endpoint);

            _stopwatch = Stopwatch.StartNew();
            _response = await _client.GetAsync(endpoint);
            _stopwatch.Stop();

            await LogRequestAndResponse($"GET {endpoint}");
        }

        [When(@"I send a DELETE request to ""(.*)""")]
        public async Task WhenISendDeleteRequest(string endpoint)
        {
            endpoint = NormalizeEndpoint(endpoint);
            _response = await _client.DeleteAsync(endpoint);
            await LogRequestAndResponse($"DELETE {endpoint}");
        }

        [When(@"I send a POST request to ""(.*)"" with invalid JSON")]
        public async Task WhenISendInvalidJson(string endpoint)
        {
            endpoint = NormalizeEndpoint(endpoint);
            var invalidJson = "{ invalid json }";
            var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

            _response = await _client.PostAsync(endpoint, content);
            await LogRequestAndResponse($"POST Invalid JSON {endpoint}", invalidJson);
        }

        [When(@"I send a POST request to ""(.*)"" without headers")]
        public async Task WhenISendWithoutHeaders(string endpoint)
        {
            endpoint = NormalizeEndpoint(endpoint);

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            var payload = new
            {
                id = 999,
                name = "NoHeaderPet",
                photoUrls = new[] { "https://example.com/photo.jpg" }
            };

            var json = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(json, Encoding.UTF8);

            _response = await _client.SendAsync(request);
            await LogRequestAndResponse($"POST Without Headers {endpoint}", json);
        }

        // ================================
        // THEN
        // ================================
        [Then(@"the response status should be (.*)")]
        public void ThenStatusShouldBe(int expectedStatus)
        {
            ((int)_response.StatusCode).Should().Be(expectedStatus);
            _test.Pass($"Status code is {expectedStatus}");
        }

        [Then(@"the response status should be one of \[(.*)\]")]
        public void ThenStatusShouldBeOneOf(string statuses)
        {
            var actual = (int)_response.StatusCode;

            var expectedList = statuses.Split(',')
                                       .Select(s => int.Parse(s.Trim()))
                                       .ToList();

            expectedList.Should().Contain(actual);
            _test.Pass($"Status code is {actual} (expected one of {string.Join(",", expectedList)})");
        }

        [Then(@"the response time should be less than (.*) milliseconds")]
        public void ThenResponseTimeShouldBeLessThan(int milliseconds)
        {
            _stopwatch.ElapsedMilliseconds.Should().BeLessThan(milliseconds);
            _test.Info($"Response time: {_stopwatch.ElapsedMilliseconds} ms");
        }

        // ================================
        // RESPONSE VALIDATION
        // ================================
        [Then(@"the response should contain ""(.*)"" as (.*)")]
        public async Task ThenResponseShouldContainInt(string key, int expected)
        {
            var json = await _response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var actual = doc.RootElement.GetProperty(key).GetInt32();

            actual.Should().Be(expected);
            _test.Pass($"{key} = {actual}");
        }

        [Then(@"the response should contain ""(.*)"" as ""(.*)""")]
        public async Task ThenResponseShouldContainString(string key, string expected)
        {
            var json = await _response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var actual = doc.RootElement.GetProperty(key).GetString();

            actual.Should().Be(expected);
            _test.Pass($"{key} = {actual}");
        }

        [Then(@"the response should contain an error message")]
        public async Task ThenTheResponseShouldContainAnErrorMessage()
        {
            var responseBody = await _response.Content.ReadAsStringAsync();
            responseBody.Should().NotBeNullOrEmpty();
            _test.Pass("Error message exists in response");
        }
    }
}