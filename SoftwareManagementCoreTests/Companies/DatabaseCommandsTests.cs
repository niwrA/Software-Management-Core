using CommandsShared;
using CompaniesShared;
using Moq;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Companies
{
  [Trait("Entity", "Database")]
  public class DatabaseCommandsTests
  {
    [Fact(DisplayName = "RenameDatabaseCommand")]
    public void RenameCommand()
    {
      var sutBuilder = new DatabaseCommandBuilder<RenameCompanyEnvironmentDatabaseCommand>();
      var sut = sutBuilder.Build() as RenameCompanyEnvironmentDatabaseCommand;

      sut.Name = "New Name";
      sut.OriginalName = "Original Name";
      sut.Execute();

      sutBuilder.DatabaseMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
    }
    [Fact(DisplayName = "AddDatabaseToCompanyEnvironmentCommand")]
    public void AddDatabaseToCompanyEnvironmentCommand()
    {
      var sutBuilder = new DatabaseCommandBuilder<AddDatabaseToCompanyEnvironmentCommand>();
      var sut = sutBuilder.Build() as AddDatabaseToCompanyEnvironmentCommand;

      sut.DatabaseName = "New Name";
      sut.EnvironmentGuid = Guid.NewGuid();
      sut.DatabaseGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.CompanyEnvironmentMock.Verify(s => s.AddDatabase(sut.DatabaseGuid, sut.DatabaseName), Times.Once);
    }
    [Fact(DisplayName = "AddDatabaseToCompanyEnvironmentCommand")]
    public void RemoveDatabaseFromCompanyEnvironmentCommand()
    {
      var sutBuilder = new DatabaseCommandBuilder<RemoveDatabaseFromCompanyEnvironmentCommand>();
      var sut = sutBuilder.Build() as RemoveDatabaseFromCompanyEnvironmentCommand;

      sut.EnvironmentGuid = Guid.NewGuid();
      sut.DatabaseGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.CompanyEnvironmentMock.Verify(s => s.RemoveDatabase(sut.DatabaseGuid), Times.Once);
    }

  }


  class DatabaseCommandBuilder<T> where T : DatabaseCommand, new()
  {
    public Mock<ICompany> CompanyMock { get; set; }
    public Mock<ICompanyEnvironment> CompanyEnvironmentMock { get; set; }
    public Mock<ICompanyEnvironmentDatabase> DatabaseMock { get; set; }
    public DatabaseCommand Build()
    {
      var companiesMock = new Mock<ICompanyService>();
      var companyMock = new Mock<ICompany>();
      var companyEnvironmentMock = new Mock<ICompanyEnvironment>();
      var databaseMock = new Mock<ICompanyEnvironmentDatabase>();

      this.CompanyMock = companyMock;
      this.CompanyEnvironmentMock = companyEnvironmentMock;
      this.DatabaseMock = databaseMock;

      var sut = new CommandBuilder<T>().Build(companiesMock.Object) as DatabaseCommand;

      companiesMock.Setup(s => s.GetCompany(It.IsAny<Guid>())).Returns(companyMock.Object);
      companyMock.Setup(s => s.GetEnvironment(It.IsAny<Guid>())).Returns(companyEnvironmentMock.Object);
      companyEnvironmentMock.Setup(s => s.GetDatabase(It.IsAny<Guid>())).Returns(databaseMock.Object);

      return sut;
    }
  }
}
