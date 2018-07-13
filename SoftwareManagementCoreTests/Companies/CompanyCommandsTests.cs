using niwrA.CommandManager;
using Moq;
using CompaniesShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using niwrA.CommandManager.Contracts;

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
            var sut = sutBuilder.Build();

            sut.Name = "New Name";
            sut.OriginalName = "Original Name";
            sut.Execute();

            sutBuilder.CompanyMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }
        [Fact(DisplayName = "ChangeCodeForCompanyCommand")]
        public void ChangeCodeForCommand()
        {
            var sutBuilder = new CompanyCommandBuilder<ChangeCodeForCompanyCommand>();
            var sut = sutBuilder.Build();

            sut.Code = "New Code";
            sut.OriginalCode = "Original Code";
            sut.Execute();

            sutBuilder.CompanyMock.Verify(s => s.ChangeCode(sut.Code, sut.OriginalCode), Times.Once);
        }
        [Fact(DisplayName = "ChangeExternalIdForCompanyCommand")]
        public void ChangeExternalIdForCommand()
        {
            var sutBuilder = new CompanyCommandBuilder<ChangeExternalIdForCompanyCommand>();
            var sut = sutBuilder.Build();

            sut.ExternalId = "New ExternalId";
            sut.OriginalExternalId = "Original ExternalId";
            sut.Execute();

            sutBuilder.CompanyMock.Verify(s => s.ChangeExternalId(sut.ExternalId, sut.OriginalExternalId), Times.Once);
        }

        [Fact(DisplayName = "AddRoleToCompanyCommand")]
        public void AddRoleToCompanyCommand()
        {
            var sutBuilder = new CompanyCommandBuilder<AddCompanyRoleCommand>();
            var sut = sutBuilder.Build();

            sut.RoleName = "New Name";
            sut.Execute();

            sutBuilder.CompanyMock.Verify(s => s.AddRoleToCompany(sut.EntityGuid, sut.RoleName), Times.Once);
        }

        [Fact(DisplayName = "RemoveRoleFromCompanyCommand")]
        public void RemoveRoleFromCompanyCommand()
        {
            var sutBuilder = new CompanyCommandBuilder<RemoveCompanyRoleCommand>();
            var sut = sutBuilder.Build();

            sut.Execute();

            sutBuilder.CompanyMock.Verify(s => s.RemoveRoleFromCompany(sut.EntityGuid), Times.Once);
        }

        [Fact(DisplayName = "AddEnvironmentToCompanyCommand")]
        public void AddEnvironmentToCompanyCommand()
        {
            var sutBuilder = new CompanyCommandBuilder<AddCompanyEnvironmentCommand>();
            var sut = sutBuilder.Build();

            sut.EnvironmentName = "New Name";
            sut.Execute();

            sutBuilder.CompanyMock.Verify(s => s.AddEnvironmentToCompany(sut.EntityGuid, sut.EnvironmentName), Times.Once);
        }

        [Fact(DisplayName = "RemoveEnvironmentFromCompanyCommand")]
        public void RemoveEnvironmentFromCompanyCommand()
        {
            var sutBuilder = new CompanyCommandBuilder<RemoveCompanyEnvironmentCommand>();
            var sut = sutBuilder.Build();

            sut.Execute();

            sutBuilder.CompanyMock.Verify(s => s.RemoveEnvironmentFromCompany(sut.EntityGuid), Times.Once);
        }
    }

    class CompanyCommandBuilder<T> where T : ICommand, new()
    {
        public Mock<ICompany> CompanyMock { get; set; }
        public T Build()
        {
            var companiesMock = new Mock<ICompanyService>();
            var companyMock = new Mock<ICompany>();
            this.CompanyMock = companyMock;

            var sut = new CommandBuilder<T>().Build(companiesMock.Object);

            companiesMock.Setup(s => s.GetCompany(Guid.Parse(sut.EntityRootGuid))).Returns(companyMock.Object);

            return sut;
        }
    }

}
