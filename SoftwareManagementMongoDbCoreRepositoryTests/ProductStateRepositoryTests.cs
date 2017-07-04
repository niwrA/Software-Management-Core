using MongoDB.Driver;
using Moq;
using SoftwareManagementMongoDbCoreRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace SoftwareManagementMongoDbCoreRepositoryTests
{
    public class ProductStateRepositoryTests
    {
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

        [Fact(DisplayName = "AddProductFeatureState")]
        public void CanAddProductFeatureState()
        {
            var sutBuilder = new SutBuilder().WithProductCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var featureGuid = Guid.NewGuid();
            var productState = (ProductState)sut.CreateProductState(guid, "testproductstate");
            var state = sut.CreateProductFeatureState(guid, featureGuid, "testfeaturestate");

            sut.PersistChanges();

            sutBuilder.ProductStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<ProductState>>(
                    l => l.Contains(productState) &&
                    l.Count == 1 &&
                    l.First().ProductFeatureStates.Contains(state) &&
                    l.First().ProductFeatureStates.Count == 1),
                null, CancellationToken.None), Times.Once,
                "InsertMany was not called with the expected state");
        }

        [Fact(DisplayName = "DeleteProductFeatureState", Skip = "In progress")]
        public void CanDeleteProductFeatureState()
        {
            var sutBuilder = new SutBuilder().WithProductCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var featureGuid = Guid.NewGuid();
            var productState = (ProductState)sut.CreateProductState(guid, "testproductstate");

            sut.PersistChanges();

        }

        public class SutBuilder
        {
            private Mock<IMongoDatabase> _databaseMock;
            private Mock<IMongoClient> _clientMock;

            public Mock<IMongoDatabase> Database { get { return _databaseMock; } }
            public Mock<IMongoClient> Client { get { return _clientMock; } }
            public Mock<IMongoCollection<ProductState>> ProductStateCollection { get; private set; }

            public ProductStateRepository Build()
            {
                _clientMock = new Mock<IMongoClient>();
                _databaseMock = new Mock<IMongoDatabase>();

                if (ProductStateCollection != null)
                {
                    _databaseMock.Setup(s => s.GetCollection<ProductState>("ProductStates", null)).Returns(ProductStateCollection.Object);
                }

                _clientMock.Setup(s => s.GetDatabase("SoftwareManagement", null)).Returns(_databaseMock.Object);

                var sut = new ProductStateRepository(_clientMock.Object);
                return sut;
            }
            public SutBuilder WithProductCollection()
            {
                ProductStateCollection = new Mock<IMongoCollection<ProductState>>();
                return this;
            }

        }


    }
}
