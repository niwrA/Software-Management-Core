using CompaniesShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Companies
{
  [Trait("Entity", "Database")]
  public class DatabaseTests
  {
    [Fact(DisplayName = "Rename")]
    public void CanRenameDatabase()
    {
      var stateMock = new Mock<ICompanyEnvironmentDatabaseState>();
      var sut = new CompanyEnvironmentDatabase(stateMock.Object);

      stateMock.Setup(s => s.Name).Returns("old");

      sut.Rename("new", "old");

      stateMock.VerifySet(t => t.Name = "new");
    }
  }
}
