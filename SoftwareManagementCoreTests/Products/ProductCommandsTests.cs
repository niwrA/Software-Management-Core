﻿using niwrA.CommandManager;
using DateTimeShared;
using Moq;
using ProductsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using niwrA.CommandManager.Contracts;

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
            var sut = new CommandBuilder<DeleteProductCommand>().Build(productsMock.Object);

            sut.Execute();

            productsMock.Verify(s => s.DeleteProduct(sut.EntityRootGuid), Times.Once);
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
            var sutBuilder = new ProductCommandBuilder<RemoveProductFeatureCommand>();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();
            var issueGuid = Guid.NewGuid();

            //sut.EntityGuid = sutBuilder.ProductMock.Object.Guid;
            sut.ProductFeatureGuid = issueGuid;
            sut.Execute();

            sutBuilder.ProductMock.Verify(s => s.DeleteFeature(issueGuid));
        }
        [Fact(DisplayName = "RemoveVersionFromProductCommand")]
        public void RemoveVersionFromProductCommand()
        {
            var sutBuilder = new ProductCommandBuilder<RemoveProductVersionCommand>();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();
            var versionGuid = Guid.NewGuid();

            //sut.EntityGuid = sutBuilder.ProductMock.Object.Guid;
            sut.ProductVersionGuid = versionGuid;
            sut.Execute();

            sutBuilder.ProductMock.Verify(s => s.DeleteVersion(versionGuid));
        }
        [Fact(DisplayName = "RemoveIssueFromProductCommand")]
        public void RemoveIssueFromProductCommand()
        {
            var sutBuilder = new ProductCommandBuilder<RemoveProductIssueCommand>();
            var sut = sutBuilder.Build();

            sut.Execute();

            sutBuilder.ProductMock.Verify(s => s.DeleteIssue(sut.EntityGuid));
        }


        [Fact(DisplayName = "ChangeBusinessCaseOfProductCommand")]
        public void ChangeBusinessCaseOfProductCommand()
        {
            var sutBuilder = new ProductCommandBuilder<ChangeBusinessCaseOfProductCommand>();
            var sut = sutBuilder.Build();

            sut.BusinessCase = "New business case";
            sut.Execute();

            sutBuilder.ProductMock.Verify(s => s.ChangeBusinessCase(sut.BusinessCase), Times.Once);
        }

        [Fact(DisplayName = "AddVersionToProductCommand")]
        public void AddVersionToProductCommand()
        {
            var sutBuilder = new ProductCommandBuilder<AddProductVersionCommand>();
            var sut = sutBuilder.Build();

            sut.Name = "New name";
            sut.Major = 1;
            sut.Minor = 2;
            sut.Revision = 3;
            sut.Build = 4;
            sut.Execute();

            sutBuilder.ProductMock.Verify(s => s.AddVersion(sut.EntityGuid, sut.Name, sut.Major, sut.Minor, sut.Revision, sut.Build), Times.Once);
        }

        [Fact(DisplayName = "AddFeatureToProductCommand")]
        public void AddFeatureToProductCommand()
        {
            var sutBuilder = new ProductCommandBuilder<AddProductFeatureCommand>();
            var sut = sutBuilder.Build();

            sut.ProductFeatureGuid = Guid.NewGuid();
            sut.FirstVersionGuid = Guid.NewGuid();
            sut.Name = "New name";
            sut.Execute();

            sutBuilder.ProductMock.Verify(s => s.AddFeature(sut.ProductFeatureGuid, sut.Name, sut.FirstVersionGuid), Times.Once);
        }
        [Fact(DisplayName = "AddIssueToProductCommand")]
        public void AddIssueToProductCommand()
        {
            var sutBuilder = new ProductCommandBuilder<AddProductIssueCommand>();
            var sut = sutBuilder.Build();

            sut.FirstVersionGuid = Guid.NewGuid();
            sut.Name = "New name";
            sut.Execute();

            sutBuilder.ProductMock.Verify(s => s.AddIssue(sut.EntityGuid, sut.Name, sut.FirstVersionGuid), Times.Once);
        }
        [Fact(DisplayName = "AddConfigToProductCommand")]
        public void AddConfigToProductCommand()
        {
            var sutBuilder = new ProductCommandBuilder<AddProductConfigOptionCommand>();
            var sut = sutBuilder.Build();

            sut.FeatureGuid = Guid.NewGuid();
            sut.Name = "New name";
            sut.Execute();

            sutBuilder.ProductMock.Verify(s => s.AddConfigOption(sut.FeatureGuid, sut.EntityGuid, sut.Name, null), Times.Once);
        }
        [Fact(DisplayName = "RemoveConfigFromFeatureCommand")]
        public void RemoveConfigFromFeatureCommand()
        {
            var sutBuilder = new ProductCommandBuilder<RemoveProductConfigOptionCommand>();
            var sut = sutBuilder.Build();

            sut.Execute();

            sutBuilder.ProductMock.Verify(s => s.DeleteConfigOption(sut.EntityGuid), Times.Once);
        }
        [Fact]
        public void RequestFeatureForProductCommand()
        {
            var sutBuilder = new ProductCommandBuilder<RequestProductFeatureCommand>();
            var sut = sutBuilder.Build();

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

            var sut = sutBuilder
                .WithProduct()
                .WithProductIssue()
                .Build();

            sut.OriginalName = "Old name";
            sut.Name = "New name";
            sut.Execute();

            sutBuilder.ProductIssueMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }

        [Fact(DisplayName = "ResolveProductIssueCommand")]
        public void ResolveProductIssueCommand()
        {
            var sutBuilder = new ProductIssueCommandBuilder<ResolveProductIssueCommand>();

            var sut = sutBuilder
                .WithProduct()
                .WithProductIssue()
                .Build();

            sut.ResolvedVersionGuid = Guid.NewGuid();
            sut.Execute();

            sutBuilder.ProductIssueMock.Verify(s => s.Resolve(sut.ResolvedVersionGuid), Times.Once);
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
            var guid = Guid.NewGuid();

            commandRepoMock.Setup(t => t.CreateCommandState(It.IsAny<Guid>())).Returns(commandState);

            var name = "New Project";

            var sut = new CommandManager(commandRepoMock.Object, new DateTimeProvider());
            var commandConfig = new CommandConfig(assembly: TestGlobals.Assembly, nameSpace: TestGlobals.Namespace, commandName: CommandTypes.Create.ToString(), entity: TestGlobals.Entity, processor: productsMock.Object);

            sut.AddCommandConfigs(new List<ICommandConfig> { commandConfig });

            var commandDto = new CommandDto { Entity = TestGlobals.Entity, EntityGuid = guid.ToString(), Command = CommandTypes.Create.ToString(), ParametersJson = @"{name: '" + name + "'}" };
            sut.ProcessCommands(new List<CommandDto> { commandDto });

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

            this.ProductMock.Setup(s => s.Guid).Returns(Guid.Parse(sut.EntityRootGuid));

            ProductsMock.Setup(s => s.GetProduct(Guid.Parse(sut.EntityRootGuid))).Returns(ProductMock.Object);

            return sut;
        }
    }

    class ProductIssueCommandBuilder<T> where T : ICommand, new()
    {
        public Mock<IProduct> ProductMock = new Mock<IProduct>();
        public Mock<IProductIssue> ProductIssueMock { get; set; }
        public Mock<IProductFeature> ProductFeatureMock { get; set; }
        public Mock<IProductService> ProductsMock = new Mock<IProductService>();

        public T Build()
        {
            var sut = new CommandBuilder<T>().Build(ProductsMock.Object);

            return sut;
        }
        public ProductIssueCommandBuilder<T> WithProduct()
        {
            ProductsMock.Setup(s => s.GetProduct(It.IsAny<Guid>())).Returns(ProductMock.Object);
            return this;
        }
        public ProductIssueCommandBuilder<T> WithProductIssue()
        {
            ProductIssueMock = new Mock<IProductIssue>();
            ProductMock.Setup(s => s.GetIssue(It.IsAny<Guid>())).Returns(ProductIssueMock.Object);
            return this;
        }
    }

}
