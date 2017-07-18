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

        [Fact(DisplayName = "AddFeature Implements IRepository")]
        public void CanAddFeature()
        {
            var repoMock = new Mock<IProductStateRepository>();
            var productStateMock = new Mock<IProductState>();
            var sut = new Product(productStateMock.Object, repoMock.Object);
            var stateMock = new Mock<IProductFeatureState>();
            const string name = "new";

            var guid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();
            var firstVersionGuid = Guid.NewGuid();

            productStateMock.Setup(s => s.Guid).Returns(productGuid);
            repoMock.Setup(t => t.CreateProductFeatureState(productGuid, guid, name)).Returns(stateMock.Object);

            var result = sut.AddFeature(guid, name, firstVersionGuid);

            stateMock.VerifySet(t => t.FirstVersionGuid = firstVersionGuid);
        }

        [Fact(DisplayName = "AddIssue Implements IRepository")]
        public void CanAddIssue()
        {
            var repoMock = new Mock<IProductStateRepository>();
            var productStateMock = new Mock<IProductState>();
            var sut = new Product(productStateMock.Object, repoMock.Object);
            var stateMock = new Mock<IProductIssueState>();
            const string name = "new";

            var guid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();
            var firstVersionGuid = Guid.NewGuid();

            productStateMock.Setup(s => s.Guid).Returns(productGuid);
            repoMock.Setup(t => t.CreateProductIssueState(productGuid, guid, name)).Returns(stateMock.Object);

            var result = sut.AddIssue(guid, name, firstVersionGuid);

            stateMock.VerifySet(t => t.FirstVersionGuid = firstVersionGuid);
        }


        [Fact(DisplayName = "RequestFeature Implements IRepository")]
        public void RequestFeature()
        {
            var repoMock = new Mock<IProductStateRepository>();
            var productStateMock = new Mock<IProductState>();
            var sut = new Product(productStateMock.Object, repoMock.Object);
            var stateMock = new Mock<IProductFeatureState>();
            const string name = "new";

            var guid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();
            var requestedForVersionGuid = Guid.NewGuid();

            productStateMock.Setup(s => s.Guid).Returns(productGuid);
            repoMock.Setup(t => t.CreateProductFeatureState(productGuid, guid, name)).Returns(stateMock.Object);

            var result = sut.RequestFeature(guid, name, requestedForVersionGuid);

            stateMock.VerifySet(t => t.RequestedForVersionGuid = requestedForVersionGuid);
            stateMock.VerifySet(t => t.IsRequest = true);
        }

        [Fact(DisplayName = "AddFeature Implements IRepository")]
        public void CanDeleteFeature()
        {
            var repoMock = new Mock<IProductStateRepository>();
            var productStateMock = new Mock<IProductState>();
            var sut = new Product(productStateMock.Object, repoMock.Object);
            var stateMock = new Mock<IProductFeatureState>();
            
            var guid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();

            repoMock.Setup(s => s.GetProductState(productGuid)).Returns(productStateMock.Object);

            productStateMock.Setup(s => s.Guid).Returns(productGuid);

            sut.DeleteFeature(guid);

            repoMock.Verify(s => s.DeleteProductFeatureState(productGuid, guid), Times.Once);
        }

        public void CanDeleteVersion()
        {
            var repoMock = new Mock<IProductStateRepository>();
            var productStateMock = new Mock<IProductState>();
            var sut = new Product(productStateMock.Object, repoMock.Object);
            var stateMock = new Mock<IProductFeatureState>();

            var guid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();

            repoMock.Setup(s => s.GetProductState(productGuid)).Returns(productStateMock.Object);

            productStateMock.Setup(s => s.Guid).Returns(productGuid);

            sut.DeleteVersion(guid);

            repoMock.Verify(s => s.DeleteProductVersionState(productGuid, guid), Times.Once);
        }
        public void CanDeleteIssue()
        {
            var repoMock = new Mock<IProductStateRepository>();
            var productStateMock = new Mock<IProductState>();
            var sut = new Product(productStateMock.Object, repoMock.Object);
            var stateMock = new Mock<IProductFeatureState>();

            var guid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();

            repoMock.Setup(s => s.GetProductState(productGuid)).Returns(productStateMock.Object);

            productStateMock.Setup(s => s.Guid).Returns(productGuid);

            sut.DeleteIssue(guid);

            repoMock.Verify(s => s.DeleteProductIssueState(productGuid, guid), Times.Once);
        }

    }
}
