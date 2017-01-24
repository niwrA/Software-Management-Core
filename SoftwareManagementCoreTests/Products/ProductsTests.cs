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
        [Fact(DisplayName = "Create implements IRepository")]
        public void CanCreateProduct()
        {
            var repoMock = new Moq.Mock<IProductStateRepository>();
            var sut = new ProductService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<IProductState>();

            repoMock.Setup(t => t.CreateProductState(It.IsAny<Guid>())).Returns(stateMock.Object);

            var guid = Guid.NewGuid();
            sut.CreateProduct(guid);

            repoMock.Verify(s => s.CreateProductState(guid), Times.Once);
        }

        [Fact(DisplayName = "Get Implements IRepository")]
        public void CanGetProduct()
        {
            var repoMock = new Mock<IProductStateRepository>();
            var sut = new ProductService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<IProductState>();
            var stateMockAlt = new Mock<IProductState>();

            var stateGuid = Guid.NewGuid();
            var altStateGuid = Guid.NewGuid();

            repoMock.Setup(t => t.GetProductState(stateGuid)).Returns(stateMock.Object);
            repoMock.Setup(t => t.GetProductState(altStateGuid)).Returns(stateMockAlt.Object);

            sut.GetProduct(stateGuid);
            sut.GetProduct(altStateGuid);

            repoMock.Verify(t => t.GetProductState(stateGuid));
            repoMock.Verify(t => t.GetProductState(altStateGuid));
        }
    }
}
