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
using System;
using CompaniesShared;

namespace SoftwareManagementCoreApiTests
{
  [Trait("Entity", "ContactController")]
  public class ContactsControllerTests
  {
    [Fact(DisplayName = "GetContacts")]
    public void CanGetContacts()
    {
      var repo = new Mock<IContactStateReadOnlyRepository>();
      var contactState = new Fakes.ContactState();

      repo.Setup(s => s.GetContactStates()).Returns(new List<IContactState> { contactState });

      var sut = new ContactsController(repo.Object);
      var sutResult = sut.Get();

      Assert.Single(sutResult);
      Assert.Equal(contactState.Guid, sutResult.First().Guid);
    }
    [Fact(DisplayName = "GetContact")]
    public void CanGetContact()
    {
      var repo = new Mock<IContactStateReadOnlyRepository>();
      var contactState = new Fakes.ContactState();

      repo.Setup(s => s.GetContactState(contactState.Guid)).Returns(contactState);

      var sut = new ContactsController(repo.Object);
      var sutResult = sut.Get(contactState.Guid);

      Assert.Equal(contactState.Guid, sutResult.Guid);
    }
  }
  public class MockRepository : IMainReadOnlyRepository
  {
    private Mock<IMainReadOnlyRepository> _mock;

    public MockRepository()
    {
      _mock = new Mock<IMainReadOnlyRepository>();
    }

    public ICompanyState GetCompanyState(Guid guid)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<ICompanyState> GetCompanyStates()
    {
      throw new NotImplementedException();
    }

    public IContactState GetContactState(Guid guid)
    {
      return new Fakes.ContactState();
    }

    public IEnumerable<IContactState> GetContactStates()
    {
      return new List<IContactState> { new Fakes.ContactState() };
    }
  }

  [Trait("Entity", "ContactController")]
  public class ContactsControllerIntegrationTests : IClassFixture<TestFixture<Startup>>
  {
    private readonly HttpClient _client;
    public ContactsControllerIntegrationTests(TestFixture<Startup> fixture)
    {
      _client = fixture.Client;
      fixture.AddContactStateReadOnlyRepository<MockRepository>();
    }
    [Fact(Skip = "Setup not yet complete (needs Development Environment setup with InMemory db)")]
    public void GetContacts_Succeeds()
    {
      var response = _client.GetAsync("/api/contacts").Result;

      response.EnsureSuccessStatusCode();

      var returnedSession = response.Content.ReadAsJsonAsync<IEnumerable<ContactDto>>().Result;

      Assert.Equal("John", returnedSession.First().Name);
    }
  }
}
