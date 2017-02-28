using Moq;
using System;
using Xunit;
using MongoDB;
using MongoDB.Driver;
using SoftwareManagementMongoDbCoreRepository;
using System.Collections.Generic;
using System.Threading;

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

            sut.DeleteCompanyState(guid);
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

            sutBuilder.EmploymentStateCollection.Verify(s => s.InsertMany(It.IsAny<IEnumerable<EmploymentState>>(), null, CancellationToken.None), Times.Once, "InsertMany was not called");
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
        public MainRepository Build()
        {
            _clientMock = new Mock<IMongoClient>();
            _databaseMock = new Mock<IMongoDatabase>();

            if (EmploymentStateCollection != null)
            {
                _databaseMock.Setup(s => s.GetCollection<EmploymentState>("EmploymentStates", null)).Returns(EmploymentStateCollection.Object);
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
    }
}
