using Core.Api;
using Business.Models;
using RestSharp;

namespace Tests.Api
{
    [TestFixture]
    [Category("API")]
    [Parallelizable(ParallelScope.All)]
    public class UserApiTests
    {
        private ApiClient _apiClient;
        private const string BaseUrl = "https://jsonplaceholder.typicode.com";

        [SetUp]
        public void SetUp()
        {
            _apiClient = new ApiClient(BaseUrl);
        }

        [Test]
        public async Task ValidateListOfUsersCanBeReceivedSuccessfully()
        {
            var request = new RequestBuilder("/users", Method.Get).Build();

            var response = await _apiClient.ExecuteAsync<List<UserModel>>(request);

            Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code is not 200 OK");
            Assert.That(response.ErrorMessage, Is.Null.Or.Empty, "Expected no error messages in response");

            Assert.That(response.Data, Is.Not.Null.And.Not.Empty, "User list is empty or null");

            var firstUser = response.Data.First();
            Assert.Multiple(() =>
            {
                Assert.That(firstUser.Id, Is.GreaterThan(0), "Id is missing or invalid");
                Assert.That(firstUser.Name, Is.Not.Null.And.Not.Empty, "Name is missing");
                Assert.That(firstUser.Username, Is.Not.Null.And.Not.Empty, "Username is missing");
                Assert.That(firstUser.Email, Is.Not.Null.And.Not.Empty, "Email is missing");
                Assert.That(firstUser.Address, Is.Not.Null, "Address object is missing");
                Assert.That(firstUser.Phone, Is.Not.Null.And.Not.Empty, "Phone is missing");
                Assert.That(firstUser.Website, Is.Not.Null.And.Not.Empty, "Website is missing");
                Assert.That(firstUser.Company, Is.Not.Null, "Company object is missing");
            });
        }

        [Test]
        public async Task ValidateResponseHeaderForListOfUsers()
        {
            var request = new RequestBuilder("/users", Method.Get).Build();

            var response = await _apiClient.ExecuteAsync(request);

            Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code is not 200 OK");
            Assert.That(response.ErrorMessage, Is.Null.Or.Empty, "Expected no error messages in response");

            var contentTypeHeader = response.ContentHeaders?.FirstOrDefault(h => h.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));

            Assert.That(contentTypeHeader, Is.Not.Null, "Content-Type header is missing from the response");
            Assert.That(contentTypeHeader.Value?.ToString(), Does.Contain("application/json; charset=utf-8"), "Content-Type header value is incorrect");
        }

        [Test]
        public async Task ValidateResponseContentForListOfUsers()
        {
            var request = new RequestBuilder("/users", Method.Get).Build();

            var response = await _apiClient.ExecuteAsync<List<UserModel>>(request);

            Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code is not 200 OK");
            Assert.That(response.ErrorMessage, Is.Null.Or.Empty, "Expected no error messages in response");

            var users = response.Data;
            Assert.That(users, Is.Not.Null, "User list should not be null");

            Assert.That(users!.Count, Is.EqualTo(10), "Response should contain exactly 10 users");

            var uniqueIdCount = users.Select(u => u.Id).Distinct().Count();
            Assert.That(uniqueIdCount, Is.EqualTo(10), "Not all users have a unique ID");

            Assert.Multiple(() =>
            {
                foreach (var user in users)
                {
                    Assert.That(user.Name, Is.Not.Null.And.Not.Empty, $"User with ID {user.Id} has an empty Name");
                    Assert.That(user.Username, Is.Not.Null.And.Not.Empty, $"User with ID {user.Id} has an empty Username");
                    Assert.That(user.Company, Is.Not.Null, $"User with ID {user.Id} has no Company object");
                    Assert.That(user.Company?.Name, Is.Not.Null.And.Not.Empty, $"User with ID {user.Id} has an empty Company Name");
                }
            });
        }

        [Test]
        public async Task ValidateUserCanBeCreated()
        {
            var newUserPayload = new
            {
                Name = "Test Automation User",
                Username = "auto_user"
            };

            var request = new RequestBuilder("/users", Method.Post)
                .AddBody(newUserPayload)
                .Build();

            var response = await _apiClient.ExecuteAsync<UserModel>(request);

            Assert.That((int)response.StatusCode, Is.EqualTo(201), "Status code is not 201 Created");
            Assert.That(response.ErrorMessage, Is.Null.Or.Empty, "Expected no error messages in response");

            Assert.That(response.Data, Is.Not.Null, "Response data is empty");
            Assert.That(response.Data!.Id, Is.GreaterThan(0), "Response does not contain a valid generated ID");
        }

        [Test]
        public async Task ValidateUserIsNotifiedIfResourceDoesNotExist()
        {
            var request = new RequestBuilder("/invalidendpoint", Method.Get).Build();

            var response = await _apiClient.ExecuteAsync(request);

            Assert.That((int)response.StatusCode, Is.EqualTo(404), "Status code is not 404 Not Found");
            Assert.That(response.ErrorMessage, Is.Null.Or.Empty, "Expected no framework error messages despite the 404 status");
        }
    }
}