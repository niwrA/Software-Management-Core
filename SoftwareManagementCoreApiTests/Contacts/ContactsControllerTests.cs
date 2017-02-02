using ContactsShared;
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
    [Trait("Entity", "ContactController")]
    public class ContactsControllerTests
    {
        [Fact(DisplayName = "GetContacts")]
        public void CanGetContacts()
        {
            var repo = new Mock<IContactStateRepository>();
            var contactState = new Fakes.ContactState();
            repo.Setup(s => s.GetContactStates()).Returns(new List<IContactState> { contactState });
            var sut = new ContactsController(repo.Object);
            var sutResult = sut.Get();
            Assert.Equal(1, sutResult.Count());
            Assert.Equal(contactState.Guid, sutResult.First().Guid);
        }
        [Fact(DisplayName = "GetContact")]
        public void CanGetContact()
        {
            var repo = new Mock<IContactStateRepository>();
            var contactState = new Fakes.ContactState();
            repo.Setup(s => s.GetContactState(contactState.Guid)).Returns(contactState);
            var sut = new ContactsController(repo.Object);
            var sutResult = sut.Get(contactState.Guid);
            Assert.Equal(contactState.Guid, sutResult.Guid);
        }
    }

    [Trait("Entity", "ContactController")]
    public class ContactsControllerIntegrationTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;
        public ContactsControllerIntegrationTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }
        [Fact(DisplayName = "GetContacts_IntegrationTest", Skip = "Setup not yet complete (needs Development Environment setup with InMemory db)")]
        public void GetContacts_Succeeds()
        {
            // Arrange

            // Act
            var response = _client.GetAsync("/api/contacts").Result;

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedSession = response.Content.ReadAsJsonAsync<IEnumerable<ContactDto>>().Result;
            //Assert.Equal(testGuid, returnedSession.EntityGuid);
        }
    }
}
