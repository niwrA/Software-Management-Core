using CommandsShared;
using Moq;
using ProductsShared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Commands
{
    [Trait("Entity", "Command")]
    public class CommandConfigTests
    {
        [Fact(DisplayName = "ProcessorConfig_CanSerializeToTypedCommand")]
        public void CanSerializeToTypedCommand_FromConfig()
        {
            var json = @"{'OriginalName': 'oude naam', 'Name': 'nieuwe naam'}";
            var projectsMoq = new Mock<IProductService>();
            var commandConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", Entity = "Product", NameSpace = "ProductsShared", Processor = projectsMoq.Object };
            RenameProductCommand command = commandConfig.GetCommand("Rename", "Product", json) as RenameProductCommand;
            Assert.Equal("oude naam", command.OriginalName);
        }

        [Fact(DisplayName = "CommandConfig_CanSerializeToTypedCommand")]
        public void CanSerializeToTypedCommand_FromCommandConfig()
        {
            var json = @"{'OriginalName': 'oude naam', 'Name': 'nieuwe naam'}";
            var projectsMoq = new Mock<IProductService>();
            var commandConfig = new CommandConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Name = "Rename", ProcessorName = "Product", Processor = projectsMoq.Object };
            RenameProductCommand command = commandConfig.GetCommand(json) as RenameProductCommand;
            Assert.Equal("oude naam", command.OriginalName);
        }
        //todo: check that CommandConfig overrides ProcessorConfig
    }
}
