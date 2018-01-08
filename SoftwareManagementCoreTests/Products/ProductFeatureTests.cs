using Moq;
using ProductsShared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Products
{
  [Trait("Entity", "ProductFeature")]
  public class ProductFeatureTests
  {
    [Fact(DisplayName = "AddConfigOption Implements IRepository")]
    public void CanAddConfigOption()
    {
      var repoMock = new Mock<IProductStateRepository>();
      var productFeatureStateMock = new Mock<IProductFeatureState>();
      var sut = new ProductFeature(productFeatureStateMock.Object, repoMock.Object);
      var stateMock = new Mock<IProductFeatureConfigOptionState>();
      const string name = "new";

      var guid = Guid.NewGuid();
      var productFeatureGuid = Guid.NewGuid();

      productFeatureStateMock.Setup(s => s.Guid).Returns(productFeatureGuid);
      stateMock.Setup(s => s.Guid).Returns(guid);
      stateMock.Setup(s => s.Name).Returns(name);
      stateMock.Setup(s => s.ProductFeatureGuid).Returns(productFeatureGuid);
      repoMock.Setup(t => t.CreateProductFeatureConfigOptionState(productFeatureStateMock.Object, guid, name)).Returns(stateMock.Object);

      var result = sut.AddConfigOption(guid, name);

      repoMock.Verify(t => t.CreateProductFeatureConfigOptionState(productFeatureStateMock.Object, guid, name), Times.Exactly(1));

      Assert.Equal(guid, result.Guid);
      Assert.Equal(name, result.Name);
      Assert.Equal(productFeatureGuid, result.ProductFeatureGuid);
    }
    [Fact(DisplayName = "DeleteConfigOption Implements IRepository")]
    public void CanDeleteConfigOption()
    {
      var repoMock = new Mock<IProductStateRepository>();
      var productFeatureStateMock = new Mock<IProductFeatureState>();
      var sut = new ProductFeature(productFeatureStateMock.Object, repoMock.Object);
      var stateMock = new Mock<IProductFeatureConfigOptionState>();

      var guid = Guid.NewGuid();
      var productFeatureGuid = Guid.NewGuid();

      productFeatureStateMock.Setup(s => s.Guid).Returns(productFeatureGuid);
      stateMock.Setup(s => s.Guid).Returns(guid);
      stateMock.Setup(s => s.ProductFeatureGuid).Returns(productFeatureGuid);

      sut.DeleteConfigOption(guid);

      repoMock.Verify(t => t.DeleteProductFeatureConfigOptionState(productFeatureStateMock.Object, guid), Times.Exactly(1));
    }
  }
}
