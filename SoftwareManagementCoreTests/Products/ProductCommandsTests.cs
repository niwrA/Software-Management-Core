using CommandsShared;
using DateTimeShared;
using Moq;
using ProductsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Products
{
  [Trait("Entity", "Product")]
  public class ProductCommandsTests
  {
    [Fact(DisplayName = "CreateCommand")]
    public void CreateProductWithCommand()
    {
      var productsMock = new Mock<IProductService>();
      var sut = new CommandBuilder<CreateProductCommand>().Build(productsMock.Object) as CreateProductCommand;

      sut.Name = "New Product";
      sut.Execute();

      productsMock.Verify(v => v.CreateProduct(sut.EntityGuid, sut.Name), Times.Once);
    }

    [Fact(DisplayName = "DeleteCommand")]
    public void DeleteCommand()
    {
      var productsMock = new Mock<IProductService>();
      var sut = new CommandBuilder<DeleteProductCommand>().Build(productsMock.Object) as DeleteProductCommand;

      sut.Execute();

      productsMock.Verify(s => s.DeleteProduct(sut.EntityGuid), Times.Once);
    }

    [Fact(DisplayName = "RenameCommand")]
    public void RenameCommand()
    {
      var sutBuilder = new ProductCommandBuilder<RenameProductCommand>();
      var sut = sutBuilder.Build() as RenameProductCommand;

      sut.OriginalName = "Old name";
      sut.Name = "New name";
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
    }

    [Fact(DisplayName = "RemoveFeatureFromProductCommand")]
    public void RemoveFeatureFromProductCommand()
    {
      var sutBuilder = new ProductCommandBuilder<RemoveFeatureFromProductCommand>();
      var sut = sutBuilder.Build() as RemoveFeatureFromProductCommand;

      var guid = Guid.NewGuid();
      var issueGuid = Guid.NewGuid();

      sut.EntityGuid = sutBuilder.ProductMock.Object.Guid;
      sut.ProductFeatureGuid = issueGuid;
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.DeleteFeature(issueGuid));
    }
    [Fact(DisplayName = "RemoveVersionFromProductCommand")]
    public void RemoveVersionFromProductCommand()
    {
      var sutBuilder = new ProductCommandBuilder<RemoveVersionFromProductCommand>();
      var sut = sutBuilder.Build() as RemoveVersionFromProductCommand;

      var guid = Guid.NewGuid();
      var versionGuid = Guid.NewGuid();

      sut.EntityGuid = sutBuilder.ProductMock.Object.Guid;
      sut.ProductVersionGuid = versionGuid;
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.DeleteVersion(versionGuid));
    }
    [Fact(DisplayName = "RemoveIssueFromProductCommand")]
    public void RemoveIssueFromProductCommand()
    {
      var sutBuilder = new ProductCommandBuilder<RemoveIssueFromProductCommand>();
      var sut = sutBuilder.Build() as RemoveIssueFromProductCommand;

      var guid = Guid.NewGuid();
      var issueGuid = Guid.NewGuid();

      sut.EntityGuid = sutBuilder.ProductMock.Object.Guid;
      sut.ProductIssueGuid = issueGuid;
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.DeleteIssue(issueGuid));
    }

    [Fact(DisplayName = "ChangeDescriptionCommand")]
    public void ChangeDescriptionCommand()
    {
      var sutBuilder = new ProductCommandBuilder<ChangeDescriptionOfProductCommand>();
      var sut = sutBuilder.Build() as ChangeDescriptionOfProductCommand;

      sut.Description = "New description";
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.ChangeDescription(sut.Description), Times.Once);
    }

    [Fact(DisplayName = "ChangeBusinessCaseCommand")]
    public void ChangeBusinessCaseCommand()
    {
      var sutBuilder = new ProductCommandBuilder<ChangeBusinessCaseOfProductCommand>();
      var sut = sutBuilder.Build() as ChangeBusinessCaseOfProductCommand;

      sut.BusinessCase = "New business case";
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.ChangeBusinessCase(sut.BusinessCase), Times.Once);
    }

    [Fact(DisplayName = "AddVersionToProductCommand")]
    public void AddVersionToProductCommand()
    {
      var sutBuilder = new ProductCommandBuilder<AddVersionToProductCommand>();
      var sut = sutBuilder.Build() as AddVersionToProductCommand;

      sut.ProductVersionGuid = Guid.NewGuid();
      sut.Name = "New name";
      sut.Major = 1;
      sut.Minor = 2;
      sut.Revision = 3;
      sut.Build = 4;
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.AddVersion(sut.ProductVersionGuid, sut.Name, sut.Major, sut.Minor, sut.Revision, sut.Build), Times.Once);
    }

    [Fact(DisplayName = "AddFeatureToProductCommand")]
    public void AddFeatureToProductCommand()
    {
      var sutBuilder = new ProductCommandBuilder<AddFeatureToProductCommand>();
      var sut = sutBuilder.Build() as AddFeatureToProductCommand;

      sut.ProductFeatureGuid = Guid.NewGuid();
      sut.FirstVersionGuid = Guid.NewGuid();
      sut.Name = "New name";
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.AddFeature(sut.ProductFeatureGuid, sut.Name, sut.FirstVersionGuid), Times.Once);
    }
    [Fact(DisplayName = "AddIssueToProductCommand")]
    public void AddIssueToProductCommand()
    {
      var sutBuilder = new ProductCommandBuilder<AddIssueToProductCommand>();
      var sut = sutBuilder.Build() as AddIssueToProductCommand;

      sut.ProductIssueGuid = Guid.NewGuid();
      sut.FirstVersionGuid = Guid.NewGuid();
      sut.Name = "New name";
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.AddIssue(sut.ProductIssueGuid, sut.Name, sut.FirstVersionGuid), Times.Once);
    }
    [Fact(DisplayName = "AddConfigToProductCommand")]
    public void AddConfigToProductCommand()
    {
      var sutBuilder = new ProductCommandBuilder<AddConfigOptionToProductCommand>();
      var sut = sutBuilder.Build() as AddConfigOptionToProductCommand;

      sut.EntityGuid = sutBuilder.ProductMock.Object.Guid;
      sut.ConfigGuid = Guid.NewGuid();
      sut.FeatureGuid = Guid.NewGuid();
      sut.Name = "New name";
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.AddConfigOption(sut.FeatureGuid, sut.ConfigGuid, sut.Name, null), Times.Once);
    }
    [Fact(DisplayName = "RemoveConfigFromFeatureCommand")]
    public void RemoveConfigFromFeatureCommand()
    {
      var sutBuilder = new ProductCommandBuilder<RemoveConfigOptionFromProductCommand>();
      var sut = sutBuilder.Build() as RemoveConfigOptionFromProductCommand;

      sut.EntityGuid = sutBuilder.ProductMock.Object.Guid;
      sut.ConfigGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.DeleteConfigOption(sut.ConfigGuid), Times.Once);
    }
    [Fact]
    public void RequestFeatureForProductCommand()
    {
      var sutBuilder = new ProductCommandBuilder<RequestFeatureForProductCommand>();
      var sut = sutBuilder.Build() as RequestFeatureForProductCommand;

      sut.ProductFeatureGuid = Guid.NewGuid();
      sut.RequestedForVersionGuid = Guid.NewGuid();
      sut.Name = "New name";
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.RequestFeature(sut.ProductFeatureGuid, sut.Name, sut.RequestedForVersionGuid), Times.Once);
    }
    [Fact(DisplayName = "RenameIssueCommand")]
    public void RenameIssueCommand()
    {
      var sutBuilder = new ProductIssueCommandBuilder<RenameProductIssueCommand>();
      var guid = Guid.NewGuid();
      var issueGuid = Guid.NewGuid();
      var issueMock = new Mock<IProductIssue>();
      issueMock.Setup(s => s.Guid).Returns(issueGuid);

      var sut = sutBuilder
          .WithProduct(guid)
          .WithProductIssue(issueMock.Object)
          .Build() as RenameProductIssueCommand;

      sut.EntityGuid = issueGuid;
      sut.OriginalName = "Old name";
      sut.Name = "New name";
      sut.ProductGuid = guid;
      sut.Execute();

      sutBuilder.ProductsMock.Verify(s => s.GetProduct(guid));
      sutBuilder.ProductMock.Verify(s => s.GetIssue(issueGuid));
      issueMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
    }

    [Fact(DisplayName = "ChangeDescriptionCommand")]
    public void ChangeDescriptionOfProductCommand()
    {
      var sutBuilder = new ProductCommandBuilder<ChangeDescriptionOfProductCommand>();
      var sut = sutBuilder.Build() as ChangeDescriptionOfProductCommand;

      sut.Description = "New description";
      sut.Execute();

      sutBuilder.ProductMock.Verify(s => s.ChangeDescription(sut.Description), Times.Once);
    }

    // todo: move to another file, along with the globals?
    public enum CommandTypes
    {
      Create,
      Rename
    }
    public static class TestGlobals
    {
      public static string Assembly = "SoftwareManagementCore";
      public static string Namespace = "ProductsShared";
      public static string Entity = "Product";
    }

    [Fact(DisplayName = "CreateCommandToRepository")]
    [Trait("Type", "IntegrationTest")]
    public void CanCreateProductWithCommand_CallsRepository()
    {
      var commandRepoMock = new Mock<ICommandStateRepository>();
      var productsMock = new Mock<IProductService>();
      var commandState = new Fakes.CommandState();
      commandRepoMock.Setup(t => t.CreateCommandState()).Returns(commandState);

      var guid = Guid.NewGuid();
      var name = "New Project";

      var sut = new CommandService(commandRepoMock.Object, new DateTimeProvider());
      var commandConfig = new CommandConfig { Assembly = TestGlobals.Assembly, NameSpace = TestGlobals.Namespace, CommandName = CommandTypes.Create.ToString(), Entity = TestGlobals.Entity, Processor = productsMock.Object };

      sut.AddCommandConfigs(new List<ICommandConfig> { commandConfig });

      var commandDto = new CommandDto { Entity = TestGlobals.Entity, EntityGuid = guid, Name = CommandTypes.Create.ToString(), ParametersJson = @"{name: '" + name + "'}" };
      var sutResult = sut.ProcessCommand(commandDto);

      productsMock.Verify(v => v.CreateProduct(guid, name), Times.Once);
    }

  }
  class ProductCommandBuilder<T> where T : ICommand, new()
  {
    public Mock<IProduct> ProductMock = new Mock<IProduct>();
    public Mock<IProductService> ProductsMock = new Mock<IProductService>();

    public T Build()
    {
      T sut = new CommandBuilder<T>().Build(ProductsMock.Object);

      this.ProductMock.Setup(s => s.Guid).Returns(sut.EntityGuid);

      ProductsMock.Setup(s => s.GetProduct(sut.EntityGuid)).Returns(ProductMock.Object);

      return sut;
    }
  }

  class ProductIssueCommandBuilder<T> where T : ICommand, new()
  {
    public Mock<IProduct> ProductMock = new Mock<IProduct>();
    public Mock<IProductIssue> ProductIssueMock { get; set; }
    public Mock<IProductFeature> ProductFeatureMock { get; set; }
    public Mock<IProductService> ProductsMock = new Mock<IProductService>();

    public ICommand Build()
    {
      var sut = new CommandBuilder<T>().Build(ProductsMock.Object);

      return sut;
    }
    public ProductIssueCommandBuilder<T> WithProduct(Guid guid)
    {
      ProductsMock.Setup(s => s.GetProduct(guid)).Returns(ProductMock.Object);
      return this;
    }
    public ProductIssueCommandBuilder<T> WithProductIssue(IProductIssue issue)
    {
      ProductMock.Setup(s => s.GetIssue(issue.Guid)).Returns(issue);
      return this;
    }
  }

}
