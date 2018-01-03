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
  [Trait("Entity", "Account")]
  public class AccountCommandsTests
  {
    [Fact(DisplayName = "RenameAccountCommand")]
    public void RenameCommand()
    {
      var sutBuilder = new AccountCommandBuilder<RenameCompanyEnvironmentAccountCommand>();
      var sut = sutBuilder.Build() as RenameCompanyEnvironmentAccountCommand;

      sut.Name = "New Name";
      sut.OriginalName = "Original Name";
      sut.Execute();

      sutBuilder.AccountMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
    }
    [Fact(DisplayName = "AddAccountToCompanyEnvironmentCommand")]
    public void AddAccountToCompanyEnvironmentCommand()
    {
      var sutBuilder = new AccountCommandBuilder<AddAccountToCompanyEnvironmentCommand>();
      var sut = sutBuilder.Build() as AddAccountToCompanyEnvironmentCommand;

      sut.AccountName = "New Name";
      sut.EnvironmentGuid = Guid.NewGuid();
      sut.AccountGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.CompanyEnvironmentMock.Verify(s => s.AddAccount(sut.AccountGuid, sut.AccountName), Times.Once);
    }
    [Fact(DisplayName = "AddAccountToCompanyEnvironmentCommand")]
    public void RemoveAccountFromCompanyEnvironmentCommand()
    {
      var sutBuilder = new AccountCommandBuilder<RemoveAccountFromCompanyEnvironmentCommand>();
      var sut = sutBuilder.Build() as RemoveAccountFromCompanyEnvironmentCommand;

      sut.EnvironmentGuid = Guid.NewGuid();
      sut.AccountGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.CompanyEnvironmentMock.Verify(s => s.RemoveAccount(sut.AccountGuid), Times.Once);
    }

  }


  class AccountCommandBuilder<T> where T : AccountCommand, new()
  {
    public Mock<ICompany> CompanyMock { get; set; }
    public Mock<ICompanyEnvironment> CompanyEnvironmentMock { get; set; }
    public Mock<ICompanyEnvironmentAccount> AccountMock { get; set; }
    public AccountCommand Build()
    {
      var companiesMock = new Mock<ICompanyService>();
      var companyMock = new Mock<ICompany>();
      var companyEnvironmentMock = new Mock<ICompanyEnvironment>();
      var accountMock = new Mock<ICompanyEnvironmentAccount>();

      this.CompanyMock = companyMock;
      this.CompanyEnvironmentMock = companyEnvironmentMock;
      this.AccountMock = accountMock;

      var sut = new CommandBuilder<T>().Build(companiesMock.Object) as AccountCommand;

      companiesMock.Setup(s => s.GetCompany(It.IsAny<Guid>())).Returns(companyMock.Object);
      companyMock.Setup(s => s.GetEnvironment(It.IsAny<Guid>())).Returns(companyEnvironmentMock.Object);
      companyEnvironmentMock.Setup(s => s.GetAccount(It.IsAny<Guid>())).Returns(accountMock.Object);

      return sut;
    }
  }
}
