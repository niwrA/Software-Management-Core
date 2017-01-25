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
            var name = "New Project";

            var sut = new CommandManager(commandRepoMock.Object, new DateTimeProvider());
            var commandConfig = new CommandConfig { Assembly = TestGlobals.Assembly, NameSpace = TestGlobals.Namespace, Name = CommandTypes.Create.ToString(), ProcessorName = TestGlobals.Entity, Processor = productsMock.Object };

            sut.AddConfig(commandConfig);

            var commandDto = new CommandDto { Entity = TestGlobals.Entity, EntityGuid = guid, Name = CommandTypes.Create.ToString(), ParametersJson = @"{name: '" + name + "'}" };

            var sutResult = sut.ProcessCommand(commandDto, commandRepoMock.Object);

            productsMock.Verify(v => v.CreateProduct(guid, name), Times.Once);
        }

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
            var productsMock = new Mock<IProductService>();
            var productMock = new Mock<IProduct>();
            var guid = Guid.NewGuid();
            productsMock.Setup(s => s.GetProduct(guid)).Returns(productMock.Object);

            var sut = new CommandBuilder<RenameProductCommand>().Build(productsMock.Object) as RenameProductCommand;
            sut.EntityGuid = guid;
            sut.OriginalName = "Old name";
            sut.Name = "New name";

            sut.Execute();

            productMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }

        public class CommandBuilder<T> where T : ICommand, new()
        {
            public ICommand Build(ICommandProcessor processor)
            {
                var commandRepoMock = new Mock<ICommandRepository>();
                var commandState = new Fakes.CommandState();
                commandRepoMock.Setup(t => t.CreateCommandState()).Returns(commandState);
                ICommand cmd = new T();
                cmd.CommandRepository = commandRepoMock.Object;
                cmd.EntityGuid = Guid.NewGuid();
                cmd.CommandProcessor = processor;
                return cmd;
            }
        }
    }
}
