using niwrA.CommandManager;
using Moq;
using ProductsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Products
{
  [Trait("Entity", "ProductFeature")]
  public class ProductFeatureCommandTests
  {
    [Fact(DisplayName = "ChangeDescriptionCommand")]
    public void ChangeDescriptionOfFeatureCommand()
    {
      var sutBuilder = new ProductFeatureCommandBuilder<ChangeDescriptionOfProductFeatureCommand>();

      var sut = sutBuilder
          .Build() as ChangeDescriptionOfProductFeatureCommand;

      sut.EntityGuid = sutBuilder.ProductFeatureMock.Object.Guid;
      sut.Description = "Description";
      sut.ProductGuid = sutBuilder.ProductMock.Object.Guid;
      sut.Execute();

      sutBuilder.ProductsMock.Verify(s => s.GetProduct(sut.ProductGuid));
      sutBuilder.ProductMock.Verify(s => s.GetFeature(sut.EntityGuid));
      sutBuilder.ProductFeatureMock.Verify(s => s.ChangeDescription(sut.Description), Times.Once);
    }
    [Fact(DisplayName = "RenameFeatureCommand")]
    public void RenameFeatureCommand()
    {
      var sutBuilder = new ProductFeatureCommandBuilder<RenameProductFeatureCommand>();

      var sut = sutBuilder
          .Build() as RenameProductFeatureCommand;

      sut.OriginalName = "Old name";
      sut.Name = "New name";
      sut.ProductGuid = sutBuilder.ProductMock.Object.Guid;
      sut.EntityGuid = sutBuilder.ProductFeatureMock.Object.Guid;
      sut.Execute();

      sutBuilder.ProductsMock.Verify(s => s.GetProduct(sutBuilder.ProductMock.Object.Guid));
      sutBuilder.ProductMock.Verify(s => s.GetFeature(sutBuilder.ProductFeatureMock.Object.Guid));
      sutBuilder.ProductFeatureMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
    }


  }
  class ProductFeatureCommandBuilder<T> where T : ICommand, new()
  {
    public Mock<IProduct> ProductMock = new Mock<IProduct>();
    public Mock<IProductFeature> ProductFeatureMock = new Mock<IProductFeature>();
    public Mock<IProductService> ProductsMock = new Mock<IProductService>();

    public ICommand Build()
    {
      var featureGuid = Guid.NewGuid();
      ProductFeatureMock.Setup(s => s.Guid).Returns(featureGuid);
      ProductMock.Setup(s => s.GetFeature(ProductFeatureMock.Object.Guid)).Returns(ProductFeatureMock.Object);
      var sut = new CommandBuilder<T>().Build(ProductsMock.Object);
      ProductMock.Setup(s => s.Guid).Returns(sut.EntityGuid);
      ProductsMock.Setup(s => s.GetProduct(sut.EntityGuid)).Returns(ProductMock.Object);
      return sut;
    }
    public ProductFeatureCommandBuilder<T> WithProduct(Guid guid)
    {
      ProductsMock.Setup(s => s.GetProduct(guid)).Returns(ProductMock.Object);
      return this;
    }
  }

}
