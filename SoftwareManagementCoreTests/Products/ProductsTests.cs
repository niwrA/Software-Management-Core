using ProductsShared;
using System;
using Xunit;
using Moq;

namespace SoftwareManagementCoreTests
{
    public class ProductsTests
    {
        [Fact]
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
    }
}
