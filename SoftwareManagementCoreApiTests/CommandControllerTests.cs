using CommandsShared;
using SoftwareManagementCoreApi;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit;

namespace SoftwareManagementCoreApiTests
{
    // todo: why are these tests not detected? 
    [Trait("Entity", "Product")]
    public class CommandControllerTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;
        public CommandControllerTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }
        [Fact(DisplayName = "CanPostCreateProductCommand")]
        public void CreatePostReturnsCreatedCommandAsync()
        {
            // Arrange
            var testGuid = Guid.NewGuid();
            var newIdea = new CommandDto { Entity = "Product", Name = "Create", EntityGuid = testGuid, ParametersJson = @"{ Name: 'Test Project'}" };

            // Act
            var response =  _client.PostAsJsonAsync("/api/commands", newIdea).Result;

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedSession = response.Content.ReadAsJsonAsync<CommandDto>().Result;
            Assert.Equal(testGuid, returnedSession.EntityGuid);
        }
    }
}
