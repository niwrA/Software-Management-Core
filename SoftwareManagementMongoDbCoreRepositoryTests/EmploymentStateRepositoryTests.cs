using MongoDB.Driver;
using Moq;
using SoftwareManagementMongoDbCoreRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace SoftwareManagementMongoDbCoreRepositoryTests
{
  [Trait("MongoDb", "EmploymentState")]
  public class EmploymentStateRepositoryTests
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

            var state = sut.CreateEmploymentState(guid, contactGuid, companyRoleGuid) as EmploymentState;

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

        public class SutBuilder
        {
            private Mock<IMongoDatabase> _databaseMock;
            private Mock<IMongoClient> _clientMock;

            public Mock<IMongoCollection<EmploymentState>> EmploymentStateCollection { get; set; }
            public Mock<IMongoDatabase> Database { get { return _databaseMock; } }
            public Mock<IMongoClient> Client { get { return _clientMock; } }
            public EmploymentStateRepository Build()
            {
                _clientMock = new Mock<IMongoClient>();
                _databaseMock = new Mock<IMongoDatabase>();

                if (EmploymentStateCollection != null)
                {
                    _databaseMock.Setup(s => s.GetCollection<EmploymentState>("EmploymentStates", null)).Returns(EmploymentStateCollection.Object);
                }

                _clientMock.Setup(s => s.GetDatabase("SoftwareManagement", null)).Returns(_databaseMock.Object);

                var sut = new EmploymentStateRepository(_clientMock.Object);
                return sut;
            }
            public SutBuilder WithEmploymentCollection()
            {
                EmploymentStateCollection = new Mock<IMongoCollection<EmploymentState>>();
                return this;
            }

        }

    }
}
