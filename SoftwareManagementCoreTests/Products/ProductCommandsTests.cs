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
        [Fact(DisplayName = "CreateCommand")]
        [Trait("Type", "IntegrationTest")]
        public void CanCreateProductWithCommand()
        {
            var commandRepoMock = new Mock<ICommandRepository>();
            var productsMock = new Mock<IProductService>();
            var commandState = new Fakes.CommandState();
            commandRepoMock.Setup(t => t.Create()).Returns(commandState);

            var guid = Guid.NewGuid();

            var sut = new CommandManager(commandRepoMock.Object, new DateTimeProvider());
            var commandConfig = new CommandConfig { Assembly = TestGlobals.Assembly, NameSpace = TestGlobals.Namespace, Name = CommandTypes.Create.ToString(), ProcessorName = TestGlobals.Entity, Processor = productsMock.Object };

            sut.AddConfig(commandConfig);

            var commandDto = new CommandDto { Entity = TestGlobals.Entity, EntityGuid = guid, Name = CommandTypes.Create.ToString() };

            var sutResult = sut.ProcessCommand(commandDto, commandRepoMock.Object);

            productsMock.Verify(v => v.CreateProduct(guid), Times.Once);
        }

        [Fact(DisplayName = "CreateCommand_IProduct")]
        public void CreateProductWithCommand_ImplementsIProduct()
        {
            var productsMock = new Mock<IProductService>();
            var commandRepoMock = new Mock<ICommandRepository>();
            var commandState = new Fakes.CommandState();

            commandRepoMock.Setup(t => t.Create()).Returns(commandState);

            var guid = Guid.NewGuid();
            var sut = new CreateProductCommand(commandRepoMock.Object)
            {
                EntityGuid = guid,
                CommandProcessor = productsMock.Object
            };
            sut.Execute();
            productsMock.Verify(v => v.CreateProduct(It.IsAny<Guid>()), Times.Once);
        }

    }
}
