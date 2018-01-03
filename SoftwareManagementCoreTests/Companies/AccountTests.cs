using CompaniesShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Companies
{
  [Trait("Entity", "Account")]
  public class AccountTests
  {
    [Fact(DisplayName = "Rename")]
    public void CanRenameAccount()
    {
      var stateMock = new Mock<ICompanyEnvironmentAccountState>();
      var sut = new CompanyEnvironmentAccount(stateMock.Object);

      stateMock.Setup(s => s.Name).Returns("old");

      sut.Rename("new", "old");

      stateMock.VerifySet(t => t.Name = "new");
    }
  }
}
