using CommandsShared;
using DateTimeShared;
using Moq;
using ProductsShared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Products
{
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

    [Trait("Entity", "Product")]
    public class ProductCommandsTests
    {
        [Fact(DisplayName = "CreateCommandToRepository")]
        [Trait("Type", "IntegrationTest")]
        public void CanCreateProductWithCommand_CallsRepository()
        {
            var commandRepoMock = new Mock<ICommandRepository>();
            var productsMock = new Mock<IProductService>();
            var commandState = new Fakes.CommandState();
            commandRepoMock.Setup(t => t.CreateCommandState()).Returns(commandState);

            var guid = Guid.NewGuid();

            var sut = new CommandManager(commandRepoMock.Object, new DateTimeProvider());
            var commandConfig = new CommandConfig { Assembly = TestGlobals.Assembly, NameSpace = TestGlobals.Namespace, Name = CommandTypes.Create.ToString(), ProcessorName = TestGlobals.Entity, Processor = productsMock.Object };

            sut.AddConfig(commandConfig);

            var commandDto = new CommandDto { Entity = TestGlobals.Entity, EntityGuid = guid, Name = CommandTypes.Create.ToString() };

            var sutResult = sut.ProcessCommand(commandDto, commandRepoMock.Object);

            productsMock.Verify(v => v.CreateProduct(guid), Times.Once);
        }

        [Fact(DisplayName = "CreateCommand")]
        public void CreateProductWithCommand_ImplementsIProduct()
        {
            var productsMock = new Mock<IProductService>();
            var commandRepoMock = new Mock<ICommandRepository>();
            var commandState = new Fakes.CommandState();

            commandRepoMock.Setup(t => t.CreateCommandState()).Returns(commandState);

            var guid = Guid.NewGuid();
            var sut = new CreateProductCommand(commandRepoMock.Object)
            {
                EntityGuid = guid,
                CommandProcessor = productsMock.Object
            };
            sut.Execute();
            productsMock.Verify(v => v.CreateProduct(It.IsAny<Guid>()), Times.Once);
        }

        [Fact(DisplayName = "DeleteCommand")]
        public void DeleteCommand()
        {
            var commandsRepoMock = new Mock<ICommandRepository>();
            commandsRepoMock.Setup(s => s.CreateCommandState()).Returns(new Fakes.CommandState());

            var productsMock = new Mock<IProductService>();
            var guid = Guid.NewGuid();
            var sut = new DeleteProductCommand
            {
                CommandRepository = commandsRepoMock.Object,
                CommandProcessor = productsMock.Object,
                EntityGuid = guid
            };

            sut.Execute();

            productsMock.Verify(s => s.DeleteProduct(guid), Times.Once);
        }

        [Fact(DisplayName = "RenameCommand")]
        public void RenameCommand()
        {
            var commandsRepoMock = new Mock<ICommandRepository>();
            commandsRepoMock.Setup(s => s.CreateCommandState()).Returns(new Fakes.CommandState());

            var productsMock = new Mock<IProductService>();
            var productMock = new Mock<IProduct>();
            var guid = Guid.NewGuid();
            productsMock.Setup(s => s.GetProduct(guid)).Returns(productMock.Object);
            var sut = new RenameProductCommand
            {
                CommandRepository = commandsRepoMock.Object,
                CommandProcessor = productsMock.Object,
                EntityGuid = guid,
                OriginalName = "Old name",
                Name = "New name"
            };

            sut.Execute();

            productsMock.Verify(s => s.GetProduct(guid), Times.Once);
            productMock.Verify(s => s.Rename(sut.Name), Times.Once);
        }
    }
}
