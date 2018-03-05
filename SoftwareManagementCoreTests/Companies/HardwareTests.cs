using CompaniesShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Companies
{
  [Trait("Entity", "Hardware")]
  public class HardwareTests
  {
    [Fact(DisplayName = "Rename")]
    public void CanRenameHardware()
    {
      var stateMock = new Mock<ICompanyEnvironmentHardwareState>();
      var sut = new CompanyEnvironmentHardware(stateMock.Object);

      stateMock.Setup(s => s.Name).Returns("old");

      sut.Rename("new", "old");

      stateMock.VerifySet(t => t.Name = "new");
    }
    [Fact(DisplayName = "ChangeIpAddress")]
    public void CanChangeIpAddressForHardware()
    {
      var stateMock = new Mock<ICompanyEnvironmentHardwareState>();
      var sut = new CompanyEnvironmentHardware(stateMock.Object);

      stateMock.Setup(s => s.IpAddress).Returns("old");

      sut.ChangeIpAddress("new", "old");

      stateMock.VerifySet(t => t.IpAddress= "new");
    }
    [Fact(DisplayName = "ChangeInternalIpAddress")]
    public void CanChangeInternalIpAddressForHardware()
    {
      var stateMock = new Mock<ICompanyEnvironmentHardwareState>();
      var sut = new CompanyEnvironmentHardware(stateMock.Object);

      stateMock.Setup(s => s.InternalIpAddress).Returns("old");

      sut.ChangeInternalIpAddress("new", "old");

      stateMock.VerifySet(t => t.InternalIpAddress = "new");
    }
  }
}
