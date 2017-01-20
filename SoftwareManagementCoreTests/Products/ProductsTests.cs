using ProductsShared;
using System;
using Xunit;
using Moq;
using DateTimeShared;

namespace SoftwareManagementCoreTests
{
    [Trait("Entity", "Product")]
    public class ProductsTests
    {
        [Fact(DisplayName = "Create")]
        public void CanCreateProduct()
        {
            var repoMock = new Moq.Mock<IProductStateRepository>();
            var sut = new ProductService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<IProductState>();

            repoMock.Setup(t => t.CreateProductState(It.IsAny<Guid>())).Returns(stateMock.Object);

            var guid = Guid.NewGuid();
            var sutResult = sut.CreateProduct(guid);

            Assert.Equal(stateMock.Object.Guid, sutResult.Guid);
        }

        [Fact(DisplayName = "Get")]
        public void CanGetProduct()
        {
            var repoMock = new Moq.Mock<IProductStateRepository>();
            var sut = new ProductService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<IProductState>();
            var stateMockAlt = new Mock<IProductState>();

            stateMock.Object.Guid = Guid.NewGuid();
            stateMockAlt.Object.Guid = Guid.NewGuid();

            repoMock.Setup(t => t.GetProductState(stateMock.Object.Guid)).Returns(stateMock.Object);
            repoMock.Setup(t => t.GetProductState(stateMockAlt.Object.Guid)).Returns(stateMockAlt.Object);

            var sutResult = sut.GetProduct(stateMock.Object.Guid);
            var sutResultAlt = sut.GetProduct(stateMockAlt.Object.Guid);

            Assert.Equal(stateMock.Object.Guid, sutResult.Guid);
            Assert.Equal(stateMockAlt.Object.Guid, sutResultAlt.Guid);
        }
    }
}
