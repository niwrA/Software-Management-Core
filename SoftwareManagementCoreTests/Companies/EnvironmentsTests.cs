using CompaniesShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Companies
{
  [Trait("Entity", "Environment")]
  public class EnvironmentsTests
  {
    [Fact(DisplayName = "Rename")]
    public void CanRenameEnvironment()
    {
      var stateMock = new Mock<ICompanyEnvironmentState>();
      var sut = new CompanyEnvironment(stateMock.Object);

      stateMock.Setup(s => s.Name).Returns("old");

      sut.Rename("new", "old");

      stateMock.VerifySet(t => t.Name = "new");
    }
    [Fact(DisplayName = "CanAddHardware")]
    public void CanAddHardware()
    {
      var stateMock = new Mock<ICompanyEnvironmentState>();
      var repoMock = new Mock<ICompanyStateRepository>();

      var sut = new CompanyEnvironment(stateMock.Object, repoMock.Object);
      var guid = Guid.NewGuid();
      const string hardwareName = "name";
      sut.AddHardware(guid, hardwareName);

      repoMock.Verify(t => t.AddHardwareToEnvironmentState(stateMock.Object, guid, hardwareName), Times.Once);
    }
    [Fact(DisplayName = "CanRemoveHardware")]
    public void CanRemoveHardware()
    {
      var stateMock = new Mock<ICompanyEnvironmentState>();
      var repoMock = new Mock<ICompanyStateRepository>();

      var sut = new CompanyEnvironment(stateMock.Object, repoMock.Object);
      var guid = Guid.NewGuid();
      sut.RemoveHardware(guid);

      repoMock.Verify(t => t.RemoveHardwareFromEnvironmentState(stateMock.Object, guid), Times.Once);
    }
  }
}
