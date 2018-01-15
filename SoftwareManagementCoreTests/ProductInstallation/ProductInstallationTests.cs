using ProductInstallationsShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using DateTimeShared;

namespace SoftwareManagementCoreTests.ProductInstallations
{
  [Trait("Category", "ProductInstallation")]
  public class ProductInstallationTests
  {
    // todo: replace fake with mock and test setting the companyEnvironmentGuid etc. being set
    [Fact(DisplayName = "CanCreate")]
    public void CanCreateProductInstallation()
    {
      var sutBuilder = new ProductInstallationSutBuilder();
      var sut = sutBuilder.Build();

      Guid guid = Guid.NewGuid();
      Guid companyGuid = Guid.NewGuid();
      Guid productGuid = Guid.NewGuid();
      Guid ProductInstallationGuid = Guid.NewGuid();
      Guid productVersionGuid = Guid.NewGuid();
      Guid companyEnvironmentGuid = Guid.NewGuid();

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
      Assert.Equal(state.ProductGuid, sut.ProductGuid);
      Assert.Equal(state.ProductVersionGuid, sut.ProductVersionGuid);
      Assert.Equal(state.StartDate, sut.StartDate);
      Assert.Equal(state.EndDate, sut.EndDate);
      Assert.Equal(state.ExternalId, sut.ExternalId);
    }

    [Fact(DisplayName = "ChangeStartDate_UpdatesState")]
    public void ChangeStartDate_UpdatesState()
    {
      var state = new Mock<IProductInstallationState>();
      var repo = new Mock<IProductInstallationStateRepository>();
      var sut = new ProductInstallation(state.Object, repo.Object);

      var dateTime = new DateTime(2017, 1, 1);
      var originalDateTime = new DateTime(2016, 1, 1);

      state.Setup(s => s.StartDate).Returns(originalDateTime);

      sut.ChangeStartDate(dateTime, originalDateTime);

      state.VerifySet(s => s.StartDate = dateTime);
    }

    [Fact(DisplayName = "ChangeEndDate_UpdatesState")]
    public void ChangeEndDate_UpdatesState()
    {
      var state = new Mock<IProductInstallationState>();
      var repo = new Mock<IProductInstallationStateRepository>();
      var sut = new ProductInstallation(state.Object, repo.Object);

      var dateTime = new DateTime(2017, 1, 1);
      var originalDateTime = new DateTime(2016, 1, 1);

      state.Setup(s => s.EndDate).Returns(originalDateTime);

      sut.ChangeEndDate(dateTime, originalDateTime);

      state.VerifySet(s => s.EndDate = dateTime);
    }

    [Fact(DisplayName = "ChangeExternalId_UpdatesState")]
    public void ChangeExternalId_UpdatesState()
    {
      var state = new Mock<IProductInstallationState>();
      var repo = new Mock<IProductInstallationStateRepository>();
      var sut = new ProductInstallation(state.Object, repo.Object);

      var externalId = "new external id";
      var orgExternalId = "original external id";

      state.Setup(s => s.ExternalId).Returns(orgExternalId);

      sut.ChangeExternalId(externalId, orgExternalId);

      state.VerifySet(s => s.ExternalId = externalId);
    }
    public class ProductInstallationSutBuilder
    {
      Mock<IProductInstallationStateRepository> _repo;
      Mock<IDateTimeProvider> _dateTimeProvider;
      public Mock<IProductInstallationStateRepository> RepoMock { get { return _repo; } }
      public ProductInstallationService Build()
      {
        _repo = new Mock<IProductInstallationStateRepository>();
        _dateTimeProvider = new Mock<IDateTimeProvider>();
        _repo.Setup(s => s.CreateProductInstallationState(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new Fakes.ProductInstallationState());
        var sut = new ProductInstallationService(_repo.Object, _dateTimeProvider.Object);
        return sut;
      }
    }
  }
}
