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
    // todo: this test tests the state property mapping as well. Perhaps this should be in a different test,
    // and perhaps it should be done in/add a test for all tests that affect state properties
    [Fact(DisplayName = "AddConfigOption Implements IRepository")]
    public void CanAddConfigOption()
    {
      var repoMock = new Mock<IProductStateRepository>();
      var productStateMock = new Mock<IProductState>();
      var sut = new Product(productStateMock.Object, repoMock.Object);
      var stateMock = new Mock<IProductConfigOptionState>();
      const string name = "new";

      var guid = Guid.NewGuid();
      var productGuid = Guid.NewGuid();
      var featureGuid = Guid.NewGuid();
      var parentGuid = Guid.NewGuid();

      productStateMock.Setup(s => s.Guid).Returns(productGuid);
      stateMock.Setup(s => s.Guid).Returns(guid);
      stateMock.Setup(s => s.Name).Returns(name);
      stateMock.Setup(s => s.ProductGuid).Returns(productGuid);
      stateMock.Setup(s => s.ProductFeatureGuid).Returns(featureGuid);
      stateMock.Setup(s => s.ParentGuid).Returns(parentGuid);
      repoMock.Setup(t => t.CreateProductConfigOptionState(productStateMock.Object, featureGuid, parentGuid, guid, name)).Returns(stateMock.Object);

      var result = sut.AddConfigOption(featureGuid, guid, name, parentGuid);

      repoMock.Verify(t => t.CreateProductConfigOptionState(productStateMock.Object, featureGuid, parentGuid, guid, name), Times.Exactly(1));

      Assert.Equal(guid, result.Guid);
      Assert.Equal(name, result.Name);
      Assert.Equal(productGuid, result.ProductGuid);
      Assert.Equal(featureGuid, result.ProductFeatureGuid);
      Assert.Equal(parentGuid, result.ParentGuid);
    }
    [Fact(DisplayName = "DeleteConfigOption Implements IRepository")]
    public void CanDeleteConfigOption()
    {
      var repoMock = new Mock<IProductStateRepository>();
      var productStateMock = new Mock<IProductState>();
      var sut = new Product(productStateMock.Object, repoMock.Object);
      var stateMock = new Mock<IProductConfigOptionState>();

      var guid = Guid.NewGuid();
      var productFeatureGuid = Guid.NewGuid();

      productStateMock.Setup(s => s.Guid).Returns(productFeatureGuid);
      stateMock.Setup(s => s.Guid).Returns(guid);
      stateMock.Setup(s => s.ProductFeatureGuid).Returns(productFeatureGuid);

      sut.DeleteConfigOption(guid);

      repoMock.Verify(t => t.DeleteProductConfigOptionState(productStateMock.Object, guid), Times.Exactly(1));
    }
  }
}
