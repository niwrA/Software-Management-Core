using CompaniesShared;
using SoftwareManagementCoreApi;
using SoftwareManagementCoreApi.Controllers;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;
using Moq;
using SoftwareManagementCoreApiTests.Fakes;
using System.Linq;
using SoftwareManagementEFCoreRepository;

namespace SoftwareManagementCoreApiTests
{
    [Trait("Entity", "CompanyController")]
    public class CompaniesControllerTests
    {
        [Fact(DisplayName = "GetCompanies")]
        public void CanGetCompanies()
        {
            var repo = new Mock<ICompanyStateRepository>();
            var companyState = new Fakes.CompanyState();
            repo.Setup(s => s.GetCompanyStates()).Returns(new List<ICompanyState> { companyState });
            var sut = new CompaniesController(repo.Object);
            var sutResult = sut.Get();
            Assert.Equal(1, sutResult.Count());
            Assert.Equal(companyState.Guid, sutResult.First().Guid);
        }
        [Fact(DisplayName = "GetCompany")]
        public void CanGetCompany()
        {
            var repo = new Mock<ICompanyStateRepository>();
            var companyState = new Fakes.CompanyState();
            repo.Setup(s => s.GetCompanyState(companyState.Guid)).Returns(companyState);
            var sut = new CompaniesController(repo.Object);
            var sutResult = sut.Get(companyState.Guid);
            Assert.Equal(companyState.Guid, sutResult.Guid);
        }
    }

    [Trait("Entity", "CompanyController")]
    public class CompaniesControllerIntegrationTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;
        public CompaniesControllerIntegrationTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }
        [Fact(DisplayName = "GetCompanies_IntegrationTest", Skip = "Setup not yet complete (needs Development Environment setup with InMemory db)")]
        public void GetCompanies_Succeeds()
        {
            // Arrange

            // Act
            var response = _client.GetAsync("/api/companies").Result;

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedSession = response.Content.ReadAsJsonAsync<IEnumerable<CompanyDto>>().Result;
            //Assert.Equal(testGuid, returnedSession.EntityGuid);
        }
    }
}
