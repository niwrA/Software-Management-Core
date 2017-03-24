using Moq;
using ProductsShared;
using SoftwareManagementCoreApi.Controllers;
using SoftwareManagementCoreApiTests.Fakes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreApiTests.Products
{
    public class ProductsControllerTests
    {
        [Fact(DisplayName = "GetProduct")]
        public void CanGetProduct()
        {
            var repo = new Mock<IProductStateRepository>();
            var productState = new ProductState();
            repo.Setup(s => s.GetProductState(productState.Guid)).Returns(productState);
            var sut = new ProductsController(repo.Object);
            var sutResult = sut.Get(productState.Guid);
            Assert.Equal(productState.Guid, sutResult.Guid);
            Assert.Equal(productState.Name, sutResult.Name);
            Assert.Equal(productState.Description, sutResult.Description);
            Assert.Equal(productState.BusinessCase, sutResult.BusinessCase);
            Assert.Equal(productState.ProductVersionStates.Count, sutResult.ProductVersions.Count);
        }
    }
}
