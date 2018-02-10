using niwrA.CommandManager;
using CompaniesShared;
using Moq;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Companies
{
    [Trait("Entity", "Environment")]
    public class EnvironmentCommandsTests
    {
        [Fact(DisplayName = "RenameEnvironmentCommand")]
        public void RenameCommand()
        {
            var sutBuilder = new EnvironmentCommandBuilder<RenameCompanyEnvironmentCommand>();
            var sut = sutBuilder.Build() as RenameCompanyEnvironmentCommand;

            sut.Name = "New Name";
            sut.OriginalName = "Original Name";
            sut.Execute();

            sutBuilder.EnvironmentMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }
    }

    class EnvironmentCommandBuilder<T> where T : EnvironmentCommand, new()
    {
        public Mock<ICompany> CompanyMock { get; set; }
        public Mock<ICompanyEnvironment> EnvironmentMock { get; set; }
        public EnvironmentCommand Build()
        {
            var companiesMock = new Mock<ICompanyService>();
            var companyMock = new Mock<ICompany>();
            var environmentMock = new Mock<ICompanyEnvironment>();

            this.CompanyMock = companyMock;
            this.EnvironmentMock = environmentMock;

            var sut = new CommandBuilder<T>().Build(companiesMock.Object) as EnvironmentCommand;

            companiesMock.Setup(s => s.GetCompany(It.IsAny<Guid>())).Returns(companyMock.Object);
            companyMock.Setup(s => s.GetEnvironment(It.IsAny<Guid>())).Returns(environmentMock.Object);

            return sut;
        }
    }
}
