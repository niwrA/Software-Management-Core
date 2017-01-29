using SoftwareManagementCoreApi;
using SoftwareManagementCoreApi.Controllers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreApiTests
{
    [Trait("Entity", "Project")]
    public class ProjectsControllerTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;
        public ProjectsControllerTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }
        [Fact(DisplayName = "GetProjects")]
        public void GetProjects_Succeeds()
        {
            // Arrange

            // Act
            var response = _client.GetAsync("/api/projects").Result;

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedSession = response.Content.ReadAsJsonAsync<IEnumerable<ProjectDto>>().Result;
            //Assert.Equal(testGuid, returnedSession.EntityGuid);
        }
    }
}
