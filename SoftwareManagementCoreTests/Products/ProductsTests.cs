using ProductsShared;
using System;
using Xunit;
using Moq;
using CommandsShared;

namespace SoftwareManagementCoreTests
{
    public class ProductStateMock : IProductState
    {
        public DateTime Created { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
    }

    public class CommandStateMock : ICommandState
    {
        public string CommandTypeId { get; set; }
        public Guid EntityGuid { get; set; }
        public long? ExecutedOn { get; set; }
        public Guid Guid { get; set; }
        public string ParametersJson { get; set; }
        public long? ReceivedOn { get; set; }
        public string UserName { get; set; }
    }
    [Trait("Entity", "Product")]
    public class ProductsTests
    {
        [Fact(DisplayName = "Create")]
        public void CanCreateProduct()
        {
            var repoFake = new Moq.Mock<IProductStateRepository>();
            var sut = new Products(repoFake.Object);
            var stateFake = new Mock<IProductState>();

            repoFake.Setup(t => t.CreateProductState(It.IsAny<Guid>())).Returns(stateFake.Object);

            var guid = Guid.NewGuid();
            var sutResult = sut.CreateProduct(guid);

            Assert.Equal(stateFake.Object.Guid, sutResult.Guid);
        }

        [Fact(DisplayName = "CreateCommand")]
        public void CanCreateProductWithCommand()
        {
            var commandRepoFake = new Moq.Mock<ICommandRepository>();
            var repoFake = new Moq.Mock<IProductStateRepository>();
            var productState = new ProductStateMock();
            var commandState = new CommandStateMock();
            var guid = Guid.NewGuid();
            var commandProcessor = new CommandManager();
            var commandConfig = new CommandConfig { Name = "Create", ProcessorName = "Project", Processor = new Products(repoFake.Object)};
            commandProcessor.AddConfig(commandConfig);

            productState.Guid = guid;

            commandRepoFake.Setup(t => t.Create()).Returns(commandState);
            repoFake.Setup(t => t.CreateProductState(It.IsAny<Guid>())).Returns(productState);

            var sut = new CreateProductCommand { EntityGuid = guid };
            commandProcessor.ProcessCommand(sut);

        }

        [Fact(DisplayName = "Get")]
        public void CanGetProduct()
        {
            var repoFake = new Moq.Mock<IProductStateRepository>();
            var sut = new Products(repoFake.Object);
            var stateFake = new Mock<IProductState>();
            var stateFakeAlt = new Mock<IProductState>();

            stateFake.Object.Guid = Guid.NewGuid();
            stateFakeAlt.Object.Guid = Guid.NewGuid();

            repoFake.Setup(t => t.GetProductState(stateFake.Object.Guid)).Returns(stateFake.Object);
            repoFake.Setup(t => t.GetProductState(stateFakeAlt.Object.Guid)).Returns(stateFakeAlt.Object);

            var sutResult = sut.GetProduct(stateFake.Object.Guid);
            var sutResultAlt = sut.GetProduct(stateFakeAlt.Object.Guid);

            Assert.Equal(stateFake.Object.Guid, sutResult.Guid);
            Assert.Equal(stateFakeAlt.Object.Guid, sutResultAlt.Guid);
        }
    }
}
