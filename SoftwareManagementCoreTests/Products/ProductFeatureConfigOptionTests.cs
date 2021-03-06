﻿
using Moq;
using ProductsShared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Products
{
  [Trait("Entity", "ProductFeatureConfigOption")]
  public class ProductFeatureConfigOptionTests
  {
    [Fact(DisplayName = "ChangeDefaultValue implements IRepository")]
    public void CanChangeDefaultValue()
    {
      var repoMock = new Mock<IProductStateRepository>();
      var stateMock = new Mock<IProductConfigOptionState>();
      var sut = new ProductConfigOption(stateMock.Object, repoMock.Object);
      const string value = "new";
      const string orgvalue = "old";

      stateMock.Setup(s => s.DefaultValue).Returns(orgvalue);

      sut.ChangeDefaultValue(value, orgvalue);

      stateMock.VerifySet(t => t.DefaultValue = value);
    }

    [Fact]
    public void CanChangeDescription()
    {
      var repoMock = new Mock<IProductStateRepository>();
      var stateMock = new Mock<IProductConfigOptionState>();
      var sut = new ProductConfigOption(stateMock.Object, repoMock.Object);
      const string value = "new";

      sut.ChangeDescription(value);

      stateMock.VerifySet(t => t.Description = value);
    }

    [Fact(DisplayName = "ChangePath implements IRepository")]
    public void CanChangePath()
    {
      var sutBuilder = new ProductConfigOptionSutBuilder();
      const string value = "new";
      const string orgvalue = "old";
      var sut = sutBuilder
        .WithPath(value, orgvalue)
        .Build();

      sut.ChangePath(value, orgvalue);

      sutBuilder.StateMock.VerifySet(t => t.Path = value);
    }

    [Fact(DisplayName = "MakeDefaultOption implements IRepository")]
    public void CanMakeDefaultOption()
    {
      var sutBuilder = new ProductConfigOptionSutBuilder();
      var sut = sutBuilder.Build();

      sut.MakeDefaultOption();

      sutBuilder.RepoMock.Verify(t => t.MakeDefaultConfigOptionState(sutBuilder.StateMock.Object), Times.Exactly(1));
    }
    [Fact(DisplayName = "MoveToParent implements IRepository")]
    public void CanMoveToParent()
    {
      var sutBuilder = new ProductConfigOptionSutBuilder();
      var guid = Guid.NewGuid();
      var originalGuid = Guid.NewGuid();
      var sut = sutBuilder.WithParentGuid(originalGuid).Build();

      sut.MoveToParent(guid, originalGuid);

      sutBuilder.RepoMock.Verify(t => t.MoveProductConfigOption(sutBuilder.StateMock.Object, guid), Times.Exactly(1));
    }
    [Fact(DisplayName = "AddConfigChildToProductCommand")]
    public void AddConfigChildToProductCommand()
    {
      var sutBuilder = new ProductCommandBuilder<AddChildToProductConfigOptionCommand>();
      var sut = sutBuilder.Build();

      sut.EntityGuid = Guid.NewGuid();
      sut.EntityRootGuid = sutBuilder.ProductMock.Object.Guid;
      sut.ParentGuid = Guid.NewGuid();
      sut.FeatureGuid = Guid.NewGuid();
      sut.Name = "New name";
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.AddConfigOption(sut.FeatureGuid, sut.EntityGuid, sut.Name, sut.ParentGuid), Times.Once);
    }
    [Fact(DisplayName = "RemoveConfigChildFromProducteCommand")]
    public void RemoveConfigChildFromProducteCommand()
    {
      var sutBuilder = new ProductCommandBuilder<RemoveChildFromProductConfigOptionCommand>();
      var sut = sutBuilder.Build();

      sut.EntityGuid = Guid.NewGuid();
      sut.EntityRootGuid = sutBuilder.ProductMock.Object.Guid;
      sut.ChildGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.DeleteConfigOption(sut.ChildGuid), Times.Once);
    }
  }

  class ProductConfigOptionSutBuilder
  {
    public Mock<IProductStateRepository> RepoMock = new Mock<IProductStateRepository>();
    public Mock<IProductConfigOptionState> StateMock = new Mock<IProductConfigOptionState>();
    public IProductConfigOption Build()
    {
      var sut = new ProductConfigOption(StateMock.Object, RepoMock.Object);
      return sut;
    }
    public ProductConfigOptionSutBuilder WithPath(string value, string orgvalue)
    {
      StateMock.Setup(s => s.Path).Returns(orgvalue);
      return this;
    }

    public ProductConfigOptionSutBuilder WithParentGuid(Guid originalGuid)
    {
      StateMock.Setup(s => s.ParentGuid).Returns(originalGuid);
      return this;
    }
  }
}
