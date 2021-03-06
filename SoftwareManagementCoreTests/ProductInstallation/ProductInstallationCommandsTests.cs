﻿using niwrA.CommandManager;
using Moq;
using ProductInstallationsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using niwrA.CommandManager.Contracts;

namespace SoftwareManagementCoreTests.ProductInstallations
{
  [Trait("Entity", "ProductInstallation")]
  public class ProductInstallationCommandsTests
  {
    [Fact(DisplayName = "CreateProductInstallationCommand")]
    public void CreateCommand()
    {
      var ProductInstallationsMock = new Mock<IProductInstallationService>();
      var sut = new CommandBuilder<CreateProductInstallationCommand>().Build(ProductInstallationsMock.Object);

      sut.CompanyGuid = Guid.NewGuid();
      sut.ProductGuid = Guid.NewGuid();
      sut.CompanyEnvironmentGuid = Guid.NewGuid();
      sut.ProductVersionGuid = Guid.NewGuid();
      sut.StartDate = DateTime.Now.Date;
      sut.EndDate = DateTime.Now.Date.AddYears(1);
      sut.Execute();

      ProductInstallationsMock.Verify(s => s.CreateProductInstallation(sut.EntityGuid, sut.CompanyGuid, sut.ProductGuid, sut.CompanyEnvironmentGuid, sut.ProductVersionGuid, sut.StartDate, sut.EndDate), Times.Once);
    }

    // todo: duplicate check on ProductInstallations

    [Fact(DisplayName = "DeleteProductInstallationCommand")]
    public void DeleteCommand()
    {
      var ProductInstallationsMock = new Mock<IProductInstallationService>();
      var sut = new CommandBuilder<DeleteProductInstallationCommand>().Build(ProductInstallationsMock.Object);

      sut.Execute();

      ProductInstallationsMock.Verify(s => s.DeleteProductInstallation(sut.EntityGuid), Times.Once);
    }

    [Fact(DisplayName = "ChangeStartDateOfProductInstallationCommand")]
    public void ChangeProductInstallationOfStartDateCommand()
    {
      var sutBuilder = new ProductInstallationCommandBuilder<ChangeStartDateOfProductInstallationCommand>();
      var sut = sutBuilder.Build();

      sut.OriginalStartDate = DateTime.Now;
      sut.StartDate = DateTime.Now;
      sut.Execute();

      sutBuilder.ProductInstallationMock.Verify(s => s.ChangeStartDate(sut.StartDate, sut.OriginalStartDate), Times.Once);
    }

    [Fact(DisplayName = "ChangeEndDateOfProductInstallationCommand")]
    public void ChangeEndDateOfProductInstallationCommand()
    {
      var sutBuilder = new ProductInstallationCommandBuilder<ChangeEndDateOfProductInstallationCommand>();
      var sut = sutBuilder.Build();

      sut.OriginalEndDate = DateTime.Now;
      sut.EndDate = DateTime.Now;
      sut.Execute();

      sutBuilder.ProductInstallationMock.Verify(s => s.ChangeEndDate(sut.EndDate, sut.OriginalEndDate), Times.Once);
    }

    [Fact(DisplayName = "ChangeExternalIdOfProductInstallationCommand")]
    public void ChangeExternalIdOfProductInstallationCommand()
    {
      var sutBuilder = new ProductInstallationCommandBuilder<ChangeExternalIdOfProductInstallationCommand>();
      var sut = sutBuilder.Build();

      sut.OriginalExternalId = "org external id";
      sut.ExternalId = "new external id";
      sut.Execute();

      sutBuilder.ProductInstallationMock.Verify(s => s.ChangeExternalId(sut.ExternalId, sut.OriginalExternalId), Times.Once);
    }
  }

  class ProductInstallationCommandBuilder<T> where T : ICommand, new()
  {
    public Mock<IProductInstallation> ProductInstallationMock { get; set; }
    public T Build()
    {
      var ProductInstallationsMock = new Mock<IProductInstallationService>();
      var ProductInstallationMock = new Mock<IProductInstallation>();
      this.ProductInstallationMock = ProductInstallationMock;

      var sut = new CommandBuilder<T>().Build(ProductInstallationsMock.Object);

      ProductInstallationsMock.Setup(s => s.GetProductInstallation(Guid.Parse(sut.EntityGuid))).Returns(ProductInstallationMock.Object);

      return sut;
    }
  }
}
