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
            var commandConfig = new CommandConfig { Assembly = TestGlobals.Assembly, NameSpace = TestGlobals.Namespace, Name = CommandTypes.Create.ToString(), ProcessorName = TestGlobals.Entity, Processor = productsMock.Object };

            sut.AddConfig(commandConfig);

            var commandDto = new CommandDto { Entity = TestGlobals.Entity, EntityGuid = guid, Name = CommandTypes.Create.ToString(), ParametersJson = @"{name: '" + name + "'}" };
            var sutResult = sut.ProcessCommand(commandDto);

            productsMock.Verify(v => v.CreateProduct(guid, name), Times.Once);
        }

    }
    class ProductCommandBuilder<T> where T : ICommand, new()
    {
        public Mock<IProduct> ProductMock { get; set; }
        public ICommand Build()
        {
            var productsMock = new Mock<IProductService>();
            var productMock = new Mock<IProduct>();
            this.ProductMock = productMock;

            var sut = new CommandBuilder<T>().Build(productsMock.Object);

            productsMock.Setup(s => s.GetProduct(sut.EntityGuid)).Returns(productMock.Object);

            return sut;
        }
    }
}
