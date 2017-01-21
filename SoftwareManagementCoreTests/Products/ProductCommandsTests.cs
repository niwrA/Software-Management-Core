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
    [Trait("Entity", "Product")]
    public class ProductCommandsTests
    {
        [Fact(DisplayName = "CreateCommand")]
        [Trait("Type", "IntegrationTest")]
        public void CanCreateProductWithCommand()
        {
            var commandRepoMock = new Mock<ICommandRepository>();
            var repoMock = new Mock<IProductStateRepository>();
            var productState = new Fakes.ProductState();
            var commandState = new Fakes.CommandState();
            var guid = Guid.NewGuid();
            var commandProcessor = new CommandManager(commandRepoMock.Object, new DateTimeProvider());
            var commandConfig = new CommandConfig { Name = "Create", ProcessorName = "Product", Processor = new ProductService(repoMock.Object, new DateTimeProvider()) };
            commandProcessor.AddConfig(commandConfig);

            productState.Guid = guid;

            commandRepoMock.Setup(t => t.Create()).Returns(commandState);
            repoMock.Setup(t => t.CreateProductState(It.IsAny<Guid>())).Returns(productState);

            var sut = new CreateProductCommand(commandRepoMock.Object) { EntityGuid = guid };
            sut.Execute();
            //commandProcessor.ProcessCommand(sut);

        }

        [Fact(DisplayName = "CreateCommand_IProduct")]
        public void CreateProductWithCommand_ImplementsIProduct()
        {
            var productsMock = new Mock<IProductService>();

            var commandRepoMock = new Mock<ICommandRepository>();
            var commandState = new Fakes.CommandState();
            var commandProcessor = new CommandManager(commandRepoMock.Object, new DateTimeProvider());
            var commandConfig = new CommandConfig { Name = "Create", ProcessorName = "Product", Processor = productsMock.Object };

            commandProcessor.AddConfig(commandConfig);

            commandRepoMock.Setup(t => t.Create()).Returns(commandState);

            var sut = new CreateProductCommand(commandRepoMock.Object);
            sut.Execute();
            //commandProcessor.ProcessCommand(sut);

            productsMock.Verify(v => v.CreateProduct(It.IsAny<Guid>()), Times.Once);
        }

    }
}
