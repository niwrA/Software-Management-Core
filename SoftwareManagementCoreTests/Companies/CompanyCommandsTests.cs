using CommandsShared;
using Moq;
using CompaniesShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Companies
{
  [Trait("Entity", "Company")]
  public class CompanyCommandsTests
  {
    [Fact(DisplayName = "CreateCompanyCommand")]
    public void CreateCommand()
    {
      var companiesMock = new Mock<ICompanyService>();
      var sut = new CommandBuilder<CreateCompanyCommand>().Build(companiesMock.Object) as CreateCompanyCommand;

      sut.Name = "New Company";
      sut.Execute();

      companiesMock.Verify(s => s.CreateCompany(sut.EntityGuid, sut.Name), Times.Once);
    }

    [Fact(DisplayName = "DeleteCompanyCommand")]
    public void DeleteCommand()
    {
      var companiesMock = new Mock<ICompanyService>();
      var sut = new CommandBuilder<DeleteCompanyCommand>().Build(companiesMock.Object) as DeleteCompanyCommand;

      sut.Execute();

      companiesMock.Verify(s => s.DeleteCompany(sut.EntityGuid), Times.Once);
    }

    [Fact(DisplayName = "RenameCompanyCommand")]
    public void RenameCommand()
    {
      var sutBuilder = new CompanyCommandBuilder<RenameCompanyCommand>();
      var sut = sutBuilder.Build() as RenameCompanyCommand;

      sut.Name = "New Name";
      sut.OriginalName = "Original Name";
      sut.Execute();

      sutBuilder.CompanyMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
    }

    [Fact(DisplayName = "AddRoleToCompanyCommand")]
    public void AddRoleToCompanyCommand()
    {
      var sutBuilder = new CompanyCommandBuilder<AddRoleToCompanyCommand>();
      var sut = sutBuilder.Build() as AddRoleToCompanyCommand;

      sut.RoleName = "New Name";
      sut.RoleGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.CompanyMock.Verify(s => s.AddRoleToCompany(sut.RoleGuid, sut.RoleName), Times.Once);
    }

    [Fact(DisplayName = "RemoveRoleFromCompanyCommand")]
    public void RemoveRoleFromCompanyCommand()
    {
      var sutBuilder = new CompanyCommandBuilder<RemoveRoleFromCompanyCommand>();
      var sut = sutBuilder.Build() as RemoveRoleFromCompanyCommand;

      sut.RoleGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.CompanyMock.Verify(s => s.RemoveRoleFromCompany(sut.RoleGuid), Times.Once);
    }

    [Fact(DisplayName = "AddEnvironmentToCompanyCommand")]
    public void AddEnvironmentToCompanyCommand()
    {
      var sutBuilder = new CompanyCommandBuilder<AddEnvironmentToCompanyCommand>();
      var sut = sutBuilder.Build() as AddEnvironmentToCompanyCommand;

      sut.EnvironmentName = "New Name";
      sut.EnvironmentGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.CompanyMock.Verify(s => s.AddEnvironmentToCompany(sut.EnvironmentGuid, sut.EnvironmentName), Times.Once);
    }

    [Fact(DisplayName = "RemoveEnvironmentFromCompanyCommand")]
    public void RemoveEnvironmentFromCompanyCommand()
    {
      var sutBuilder = new CompanyCommandBuilder<RemoveEnvironmentFromCompanyCommand>();
      var sut = sutBuilder.Build() as RemoveEnvironmentFromCompanyCommand;

      sut.EnvironmentGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.CompanyMock.Verify(s => s.RemoveEnvironmentFromCompany(sut.EnvironmentGuid), Times.Once);
    }
  }

  class CompanyCommandBuilder<T> where T : ICommand, new()
  {
    public Mock<ICompany> CompanyMock { get; set; }
    public ICommand Build()
    {
      var companiesMock = new Mock<ICompanyService>();
      var companyMock = new Mock<ICompany>();
      this.CompanyMock = companyMock;

      var sut = new CommandBuilder<T>().Build(companiesMock.Object);

      companiesMock.Setup(s => s.GetCompany(sut.EntityGuid)).Returns(companyMock.Object);

      return sut;
    }
  }

}
