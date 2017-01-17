using ProductsShared;
using System;
using Xunit;
using Moq;

namespace SoftwareManagementCoreTests
{
    [Trait("Entity", "Product")]
    public class ProductsTests
    {
        [Fact(DisplayName = "Create")]
        public void CanCreateProduct()
        {
            var repoFake = new Moq.Mock<IProductStateRepository>();
            var sut = new Products(repoFake.Object);
            var stateFake = new Mock<IProductState>();

            repoFake.Setup(t => t.CreateProductState(It.IsAny<Guid>())).Returns(stateFake.Object);

            var guid = Guid.NewGuid();
            var sutResult = sut.CreateProduct(guid);

            Assert.Equal(stateFake.Object.Guid, sutResult.Guid);
        }

        [Fact(DisplayName = "Get")]
        public void CanGetProduct()
        {
            var repoFake = new Moq.Mock<IProductStateRepository>();
            var sut = new Products(repoFake.Object);
            var stateFake = new Mock<IProductState>();
            var stateFakeAlt = new Mock<IProductState>();

            stateFake.Object.Guid = Guid.NewGuid();
            stateFakeAlt.Object.Guid = Guid.NewGuid();

            repoFake.Setup(t => t.GetProductState(stateFake.Object.Guid)).Returns(stateFake.Object);
            repoFake.Setup(t => t.GetProductState(stateFakeAlt.Object.Guid)).Returns(stateFakeAlt.Object);

            var sutResult = sut.GetProduct(stateFake.Object.Guid);
            var sutResultAlt = sut.GetProduct(stateFakeAlt.Object.Guid);

            Assert.Equal(stateFake.Object.Guid, sutResult.Guid);
            Assert.Equal(stateFakeAlt.Object.Guid, sutResultAlt.Guid);
        }
    }
}
