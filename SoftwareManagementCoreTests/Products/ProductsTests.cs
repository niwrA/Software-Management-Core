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
        public void CreateProduct_ImplementsIRepository()
        {
            var repoMock = new Moq.Mock<IProductStateRepository>();
            var sut = new ProductService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<IProductState>();

            repoMock.Setup(t => t.CreateProductState(It.IsAny<Guid>(), It.IsAny<string>())).Returns(stateMock.Object);

            var guid = Guid.NewGuid();
            var name = "New Product";
            sut.CreateProduct(guid, name);

            repoMock.Verify(s => s.CreateProductState(guid, name), Times.Once);
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
            repoMock.Verify(t => t.GetProductState(stateGuid));

            sut.GetProduct(altStateGuid);
            repoMock.Verify(t => t.GetProductState(altStateGuid));
        }

        [Fact(DisplayName = "Get Implements IRepository")]
        public void CanRenameProduct()
        {
            var stateMock = new Mock<IProductState>();
            var repoMock = new Mock<IProductStateRepository>();
            var sut = new Product(stateMock.Object, repoMock.Object);

            sut.Rename("new", "old");

            stateMock.Setup(s => s.Name).Returns("old");
            sut.Rename("new", "old");
            stateMock.VerifySet(t => t.Name = "new");
        }

        [Fact(DisplayName = "AddVersion Implements IRepository")]
        public void CanAddVersion()
        {
            var repoMock = new Mock<IProductStateRepository>();
            var productStateMock = new Mock<IProductState>();
            var sut = new Product(productStateMock.Object, repoMock.Object);
            var stateMock = new Mock<IProductVersionState>();
            const string name = "new";

            var guid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();

            productStateMock.Setup(s => s.Guid).Returns(productGuid);
            repoMock.Setup(t => t.CreateProductVersionState(productGuid, guid, name)).Returns(stateMock.Object);

            var result = sut.AddVersion(guid, name, 1, 2, 3, 4);

            stateMock.VerifySet(t => t.Major = 1);
            stateMock.VerifySet(t => t.Minor = 2);
            stateMock.VerifySet(t => t.Revision = 3);
            stateMock.VerifySet(t => t.Build = 4);
            stateMock.VerifySet(t => t.ProductGuid = productGuid);
        }

    }
}
