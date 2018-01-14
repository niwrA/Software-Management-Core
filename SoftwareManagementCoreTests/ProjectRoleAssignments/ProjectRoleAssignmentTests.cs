using ProductInstallationsShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.ProductInstallations
{
  [Trait("Category", "ProductInstallation")]
  public class ProductInstallationTests
  {
    [Fact(DisplayName = "CanCreate")]
    public void CanCreateProductInstallation()
    {
      var sutBuilder = new ProductInstallationSutBuilder();
      var sut = sutBuilder.Build();

      Guid guid = Guid.NewGuid();
      Guid companyGuid = Guid.NewGuid();
      Guid productGuid = Guid.NewGuid();
      Guid companyEnvironmentGuid = Guid.NewGuid();
      Guid productVersionGuid = Guid.NewGuid();

      var startDate = DateTime.Now.Date as DateTime?;
      var ProductInstallation = sut.CreateProductInstallation(guid, companyGuid, productGuid, companyEnvironmentGuid, productVersionGuid, startDate, null);

      sutBuilder.RepoMock.Verify(v => v.CreateProductInstallationState(guid, companyGuid, productGuid), Times.Once);
    }

    [Fact(DisplayName = "ReflectsState")]
    public void ProductInstallationReflectsState()
    {
      var state = new Fakes.ProductInstallationState();
      var sut = new ProductInstallation(state);

      Assert.Equal(state.Guid, sut.Guid);
      Assert.Equal(state.CompanyGuid, sut.CompanyGuid);
      Assert.Equal(state.CompanyEnvironmentGuid, sut.CompanyEnvironmentGuid);
      Assert.Equal(state.ProductVersionGuid, sut.ProductVersionGuid);
      Assert.Equal(state.StartDate, sut.StartDate);
      Assert.Equal(state.EndDate, sut.EndDate);
    }

    public class ProductInstallationSutBuilder
    {
      Mock<IProductInstallationStateRepository> _repo;
      public Mock<IProductInstallationStateRepository> RepoMock { get { return _repo; } }
      public ProductInstallationService Build()
      {
        _repo = new Mock<IProductInstallationStateRepository>();
        _repo.Setup(s => s.CreateProductInstallationState(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new Fakes.ProductInstallationState());
        var sut = new ProductInstallationService(_repo.Object);
        return sut;
      }
    }
  }
}
