using ProjectsShared;
using SoftwareManagementCoreApi;
using SoftwareManagementCoreApi.Controllers;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;
using Moq;
using SoftwareManagementCoreApiTests.Fakes;
using System.Linq;

namespace SoftwareManagementCoreApiTests
{
    [Trait("Entity", "ProjectController")]
    public class ProjectsControllerTests
    {
        [Fact(DisplayName ="GetProjects")]
        public void CanGetProjects()
        {
            var repo = new Mock<IProjectStateRepository>();
            var projectState = new ProjectState();
            repo.Setup(s => s.GetProjectStates()).Returns(new List<IProjectState> { projectState });
            var sut = new ProjectsController(repo.Object);
            var sutResult = sut.Get();
            Assert.Equal(1, sutResult.Count());
            Assert.Equal(projectState.Guid, sutResult.First().Guid);
        }
        [Fact(DisplayName = "GetProject")]
        public void CanGetProject()
        {
            var repo = new Mock<IProjectStateRepository>();
            var projectState = new ProjectState();
            repo.Setup(s => s.GetProjectState(projectState.Guid)).Returns(projectState);
            var sut = new ProjectsController(repo.Object);
            var sutResult = sut.Get(projectState.Guid);
            Assert.Equal(projectState.Guid, sutResult.Guid);
        }
    }


    [Trait("Entity", "ProjectController")]
    public class ProjectsControllerIntegrationTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;
        public ProjectsControllerIntegrationTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }
        [Fact(DisplayName = "GetProjects_IntegrationTest", Skip ="Setup not yet complete (needs Development Environment setup with InMemory db)")]
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
