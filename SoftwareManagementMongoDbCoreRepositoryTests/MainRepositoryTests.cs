using Moq;
using System;
using Xunit;
using MongoDB;
using MongoDB.Driver;
using SoftwareManagementMongoDbCoreRepository;
using System.Collections.Generic;
using System.Threading;
using ProductsShared;
using System.Linq;

namespace SoftwareManagementMongoDbCoreRepositoryTests
{
    [Trait("Entity", "MainRepository_Mongo")]
    public class MainRepositoryTests
    {
        [Fact(DisplayName = "CreateEmploymentState")]
        public void CanCreateEmploymentState()
        {
            var sutBuilder = new SutBuilder();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();
            var companyRoleGuid = Guid.NewGuid();
            var contactGuid = Guid.NewGuid();

            var state = sut.CreateEmploymentState(guid, contactGuid, companyRoleGuid);

            Assert.Equal(guid, state.Guid);
            Assert.Equal(contactGuid, state.ContactGuid);
            Assert.Equal(companyRoleGuid, state.CompanyRoleGuid);
        }

        [Fact(DisplayName = "DeleteEmploymentState")]
        public void CanDeleteEmploymentState()
        {
            var sutBuilder = new SutBuilder();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteEmploymentState(guid);
        }

        [Fact(DisplayName = "PersistCreatedEmploymentState")]
        public void WhenCreateEmploymentState_PersistChanges_InsertsIntoCollection()
        {
            var sutBuilder = new SutBuilder().WithEmploymentCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();
            var companyRoleGuid = Guid.NewGuid();
            var contactGuid = Guid.NewGuid();

            var state = sut.CreateEmploymentState(guid, contactGuid, companyRoleGuid);

            sut.PersistChanges();

            sutBuilder.EmploymentStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<EmploymentState>>(
                l => l.Contains(state) &&
                l.Count == 1)
                , null, CancellationToken.None), Times.Once, "InsertMany was not called correctly");
        }

        [Fact(DisplayName = "PersistDeletedEmploymentState")]
        public void WhenDeleteEmploymentState_PersistChanges_DeletesFromCollection()
        {
            var sutBuilder = new SutBuilder().WithEmploymentCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteEmploymentState(guid);

            sut.PersistChanges();

            sutBuilder.EmploymentStateCollection.Verify(s => s.DeleteOne(It.IsAny<FilterDefinition<EmploymentState>>(), null, CancellationToken.None), Times.Once, "DeleteOne was not called");
        }

        [Fact(DisplayName = "CreateProjectRoleAssignmentState")]
        public void CanCreateProjectRoleAssignmentState()
        {
            var sutBuilder = new SutBuilder();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();
            var projectRoleGuid = Guid.NewGuid();
            var contactGuid = Guid.NewGuid();

            var state = sut.CreateProjectRoleAssignmentState(guid, contactGuid, projectRoleGuid);

            Assert.Equal(guid, state.Guid);
            Assert.Equal(contactGuid, state.ContactGuid);
            Assert.Equal(projectRoleGuid, state.ProjectRoleGuid);
        }


        [Fact(DisplayName = "PersistCreatedLinkState")]
        public void WhenCreateLinkState_PersistChanges_InsertsIntoCollection()
        {
            var sutBuilder = new SutBuilder().WithLinkCollection();
            var sut = sutBuilder.Build();

            var name = "Link name";
            var guid = Guid.NewGuid();

            var state = sut.CreateLinkState(guid, name);

            sut.PersistChanges();

            sutBuilder.LinkStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<LinkState>>(
                l => l.Contains(state) &&
                l.Count == 1)
                , null, CancellationToken.None), Times.Once, "InsertMany was not called correctly");
        }

        [Fact(DisplayName = "PersistDeletedLinkState")]
        public void WhenDeleteLinkState_PersistChanges_DeletesFromCollection()
        {
            var sutBuilder = new SutBuilder().WithLinkCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteLinkState(guid);

            sut.PersistChanges();

            sutBuilder.LinkStateCollection.Verify(s => s.DeleteOne(It.IsAny<FilterDefinition<LinkState>>(), CancellationToken.None), Times.Once, "DeleteOne was not called");
        }


        [Fact(DisplayName = "DeleteProjectRoleAssignmentState")]
        public void CanDeleteProjectRoleAssignmentState()
        {
            var sutBuilder = new SutBuilder();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteProjectRoleAssignmentState(guid);
        }

        [Fact(DisplayName = "PersistCreatedProjectRoleAssignmentState")]
        public void WhenCreateProjectRoleAssignmentState_PersistChanges_InsertsIntoCollection()
        {
            var sutBuilder = new SutBuilder().WithProjectRoleAssignmentCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();
            var projectRoleGuid = Guid.NewGuid();
            var contactGuid = Guid.NewGuid();

            var state = sut.CreateProjectRoleAssignmentState(guid, contactGuid, projectRoleGuid);

            sut.PersistChanges();

            sutBuilder.ProjectRoleAssignmentStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<ProjectRoleAssignmentState>>(
                l => l.Contains(state) &&
                l.Count == 1)
                , null, CancellationToken.None), Times.Once, "InsertMany was not called correctly");
        }

        [Fact(DisplayName = "PersistDeletedProjectRoleAssignmentState")]
        public void WhenDeleteProjectRoleAssignmentState_PersistChanges_DeletesFromCollection()
        {
            var sutBuilder = new SutBuilder().WithProjectRoleAssignmentCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteProjectRoleAssignmentState(guid);

            sut.PersistChanges();

            sutBuilder.ProjectRoleAssignmentStateCollection.Verify(s => s.DeleteOne(It.IsAny<FilterDefinition<ProjectRoleAssignmentState>>(), null, CancellationToken.None), Times.Once, "DeleteOne was not called");
        }

        [Fact(DisplayName = "AddProductVersionState")]
        public void CanAddProductVersionState()
        {
            var sutBuilder = new SutBuilder().WithProductCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var versionGuid = Guid.NewGuid();
            var productState = (ProductState)sut.CreateProductState(guid, "testproductstate");
            var state = sut.CreateProductVersionState(guid, versionGuid, "testversionstate");

            sut.PersistChanges();

            sutBuilder.ProductStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<ProductState>>(
                    l => l.Contains(productState) &&
                    l.Count == 1 &&
                    l.First().ProductVersionStates.Contains(state) &&
                    l.First().ProductVersionStates.Count == 1),
                null, CancellationToken.None), Times.Once,
                "InsertMany was not called with the expected state");
        }

        [Fact(DisplayName = "DeleteProductVersionState", Skip = "In progress")]
        public void CanDeleteProductVersionState()
        {
            var sutBuilder = new SutBuilder().WithProductCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var versionGuid = Guid.NewGuid();
            var productState = (ProductState)sut.CreateProductState(guid, "testproductstate");

            sut.PersistChanges();

        }

        [Fact(DisplayName = "AddCompanyEnvironmentState")]
        public void CanAddCompanyEnvironmentState()
        {
            var sutBuilder = new SutBuilder().WithCompanyCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var environmentGuid = Guid.NewGuid();
            var companyState = (CompanyState)sut.CreateCompanyState(guid, "testcompanystate");
            var state = sut.AddEnvironmentToCompanyState(guid, environmentGuid, "testenvironmentstate");

            sut.PersistChanges();

            sutBuilder.CompanyStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<CompanyState>>(
                    l => l.Contains(companyState) &&
                    l.Count == 1 &&
                    l.First().CompanyEnvironmentStates.Contains(state) &&
                    l.First().CompanyEnvironmentStates.Count == 1),
                null, CancellationToken.None), Times.Once,
                "InsertMany was not called with the expected state");
        }

        [Fact(DisplayName = "DeleteCompanyEnvironmentState", Skip = "In progress")]
        public void CanDeleteCompanyEnvironmentState()
        {
            var sutBuilder = new SutBuilder().WithCompanyCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var environmentGuid = Guid.NewGuid();
            var companyState = (CompanyState)sut.CreateCompanyState(guid, "testcompanystate");

            sut.PersistChanges();

        }

        [Fact(DisplayName = "CanGetEnvironmentState", Skip = "In progress")]
        public void CanGetEnvironmentState()
        {
            var sutBuilder = new SutBuilder().WithCompanyCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var environmentGuid = Guid.NewGuid();

            // todo: setup mock for GetCompanyState

            sut.GetEnvironmentState(guid, environmentGuid);
        }


        [Fact(DisplayName = "CanPersistChanges_WhenNoChanges")]
        public void CanPersistChanges_WhenNoChanges()
        {
            var sutBuilder = new SutBuilder();
            var sut = sutBuilder.Build();

            sut.PersistChanges();
        }
    }

    public class SutBuilder
    {
        private Mock<IMongoDatabase> _databaseMock;
        private Mock<IMongoClient> _clientMock;

        public Mock<IMongoCollection<EmploymentState>> EmploymentStateCollection { get; set; }
        public Mock<IMongoDatabase> Database { get { return _databaseMock; } }
        public Mock<IMongoClient> Client { get { return _clientMock; } }
        public Mock<IMongoCollection<ProductState>> ProductStateCollection { get; private set; }
        public Mock<IMongoCollection<DesignState>> DesignStateCollection { get; private set; }
        public Mock<IMongoCollection<CompanyState>> CompanyStateCollection { get; private set; }
        public Mock<IMongoCollection<ProjectRoleAssignmentState>> ProjectRoleAssignmentStateCollection { get; private set; }
        public Mock<IMongoCollection<LinkState>> LinkStateCollection { get; private set; }

        public MainRepository Build()
        {
            _clientMock = new Mock<IMongoClient>();
            _databaseMock = new Mock<IMongoDatabase>();

            if (EmploymentStateCollection != null)
            {
                _databaseMock.Setup(s => s.GetCollection<EmploymentState>("EmploymentStates", null)).Returns(EmploymentStateCollection.Object);
            }
            if (ProjectRoleAssignmentStateCollection != null)
            {
                _databaseMock.Setup(s => s.GetCollection<ProjectRoleAssignmentState>("ProjectRoleAssignmentStates", null)).Returns(ProjectRoleAssignmentStateCollection.Object);
            }
            if (ProductStateCollection != null)
            {
                _databaseMock.Setup(s => s.GetCollection<ProductState>("ProductStates", null)).Returns(ProductStateCollection.Object);
            }
            if (DesignStateCollection != null)
            {
                _databaseMock.Setup(s => s.GetCollection<DesignState>("DesignStates", null)).Returns(DesignStateCollection.Object);
            }
            if (CompanyStateCollection != null)
            {
                _databaseMock.Setup(s => s.GetCollection<CompanyState>("CompanyStates", null)).Returns(CompanyStateCollection.Object);
            }
            if (LinkStateCollection != null)
            {
                _databaseMock.Setup(s => s.GetCollection<LinkState>("LinkStates", null)).Returns(LinkStateCollection.Object);
            }

            _clientMock.Setup(s => s.GetDatabase("SoftwareManagement", null)).Returns(_databaseMock.Object);

            var sut = new MainRepository(_clientMock.Object);
            return sut;
        }
        public SutBuilder WithEmploymentCollection()
        {
            EmploymentStateCollection = new Mock<IMongoCollection<EmploymentState>>();
            return this;
        }

        public SutBuilder WithProductCollection()
        {
            ProductStateCollection = new Mock<IMongoCollection<ProductState>>();
            return this;
        }

        public SutBuilder WithDesignCollection()
        {
            DesignStateCollection = new Mock<IMongoCollection<DesignState>>();
            return this;
        }

        public SutBuilder WithCompanyCollection()
        {
            CompanyStateCollection = new Mock<IMongoCollection<CompanyState>>();
            return this;
        }

        public SutBuilder WithProjectRoleAssignmentCollection()
        {
            ProjectRoleAssignmentStateCollection = new Mock<IMongoCollection<ProjectRoleAssignmentState>>();
            return this;
        }
        public SutBuilder WithLinkCollection()
        {
            LinkStateCollection = new Mock<IMongoCollection<LinkState>>();
            return this;
        }
    }

    public class CompanyStateBuilder
    {
        private List<CompanyEnvironmentState> _environments = new List<CompanyEnvironmentState>();
        public CompanyState Build()
        {
            var state = new CompanyState();
            foreach (var environmentState in _environments)
            {
                state.CompanyEnvironmentStates.Add(environmentState);
            }
            return state;
        }
         
        public CompanyStateBuilder WithEnvironment(string name)
        {
            var state = new CompanyEnvironmentState { Name = name };
            _environments.Add(state);
            return this;
        }
    }
}
