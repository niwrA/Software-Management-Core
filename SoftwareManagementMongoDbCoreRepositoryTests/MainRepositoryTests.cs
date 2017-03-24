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

        public MainRepository Build()
        {
            _clientMock = new Mock<IMongoClient>();
            _databaseMock = new Mock<IMongoDatabase>();

            if (EmploymentStateCollection != null)
            {
                _databaseMock.Setup(s => s.GetCollection<EmploymentState>("EmploymentStates", null)).Returns(EmploymentStateCollection.Object);
            }
            if (ProductStateCollection != null)
            {
                _databaseMock.Setup(s => s.GetCollection<ProductState>("ProductStates", null)).Returns(ProductStateCollection.Object);
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
    }
}
