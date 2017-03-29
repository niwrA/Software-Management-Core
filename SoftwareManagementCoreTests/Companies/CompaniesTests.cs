using CompaniesShared;
using System;
using Xunit;
using Moq;
using DateTimeShared;

namespace SoftwareManagementCoreTests
{
    [Trait("Entity", "Company")]
    public class CompaniesTests
    {
        [Fact(DisplayName = "Create")]
        public void CreateCompany_ImplementsIRepository()
        {
            var repoMock = new Moq.Mock<ICompanyStateRepository>();
            var sut = new CompanyService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<ICompanyState>();

            repoMock.Setup(t => t.CreateCompanyState(It.IsAny<Guid>(), It.IsAny<string>())).Returns(stateMock.Object);

            var guid = Guid.NewGuid();
            var name = "New Company";
            sut.CreateCompany(guid, name);

            repoMock.Verify(s => s.CreateCompanyState(guid, name), Times.Once);
        }

        [Fact(DisplayName = "Delete")]
        public void CanDeleteCompany()
        {
            var repoMock = new Mock<ICompanyStateRepository>();
            var sut = new CompanyService(repoMock.Object, new DateTimeProvider());
            var guid = Guid.NewGuid();

            sut.DeleteCompany(guid);

            repoMock.Verify(s => s.DeleteCompanyState(guid));
        }

        [Fact(DisplayName = "Get")]
        public void CanGetCompany()
        {
            var repoMock = new Mock<ICompanyStateRepository>();
            var sut = new CompanyService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<ICompanyState>();
            var stateMockAlt = new Mock<ICompanyState>();

            var stateGuid = Guid.NewGuid();
            var altStateGuid = Guid.NewGuid();

            repoMock.Setup(t => t.GetCompanyState(stateGuid)).Returns(stateMock.Object);
            repoMock.Setup(t => t.GetCompanyState(altStateGuid)).Returns(stateMockAlt.Object);

            sut.GetCompany(stateGuid);
            repoMock.Verify(t => t.GetCompanyState(stateGuid));

            sut.GetCompany(altStateGuid);
            repoMock.Verify(t => t.GetCompanyState(altStateGuid));
        }

        [Fact(DisplayName = "Rename")]
        public void CanRenameCompany()
        {
            var stateMock = new Mock<ICompanyState>();
            var sut = new Company(stateMock.Object);

            stateMock.Setup(s => s.Name).Returns("old");

            sut.Rename("new", "old");

            stateMock.VerifySet(t => t.Name = "new");
        }
        [Fact(DisplayName = "CanAddRoleToCompany")]
        public void CanAddRoleToCompany()
        {
            var repoMock = new Mock<ICompanyStateRepository>();
            var stateMock = new Fakes.CompanyState { Guid = Guid.NewGuid() };
            var sut = new Company(stateMock, repoMock.Object);

            var roleGuid = Guid.NewGuid();
            var roleName = "Tester";

            sut.AddRoleToCompany(roleGuid, roleName);

            repoMock.Verify(s => s.AddRoleToCompanyState(stateMock.Guid, roleGuid, roleName), Times.Once);
        }

        [Fact(DisplayName = "CanRemoveRoleFromCompany")]
        public void CanRemoveRoleFromCompany()
        {
            var repoMock = new Mock<ICompanyStateRepository>();
            var stateMock = new Fakes.CompanyState { Guid = Guid.NewGuid() };
            var sut = new Company(stateMock, repoMock.Object);

            var roleGuid = Guid.NewGuid();

            sut.RemoveRoleFromCompany(roleGuid);

            repoMock.Verify(s => s.RemoveRoleFromCompanyState(stateMock.Guid, roleGuid), Times.Once);
        }
        [Fact(DisplayName = "CanAddEnvironmentToCompany")]
        public void CanAddEnvironmentToCompany()
        {
            var repoMock = new Mock<ICompanyStateRepository>();
            var stateMock = new Fakes.CompanyState { Guid = Guid.NewGuid() };
            var sut = new Company(stateMock, repoMock.Object);

            var EnvironmentGuid = Guid.NewGuid();
            var EnvironmentName = "Tester";

            sut.AddEnvironmentToCompany(EnvironmentGuid, EnvironmentName);

            repoMock.Verify(s => s.AddEnvironmentToCompanyState(stateMock.Guid, EnvironmentGuid, EnvironmentName), Times.Once);
        }

        [Fact(DisplayName = "CanRemoveEnvironmentFromCompany")]
        public void CanRemoveEnvironmentFromCompany()
        {
            var repoMock = new Mock<ICompanyStateRepository>();
            var stateMock = new Fakes.CompanyState { Guid = Guid.NewGuid() };
            var sut = new Company(stateMock, repoMock.Object);

            var EnvironmentGuid = Guid.NewGuid();

            sut.RemoveEnvironmentFromCompany(EnvironmentGuid);

            repoMock.Verify(s => s.RemoveEnvironmentFromCompanyState(stateMock.Guid, EnvironmentGuid), Times.Once);
        }
    }
}
