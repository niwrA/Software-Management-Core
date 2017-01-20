using CommandsShared;
using Moq;
using ProductsShared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Commands
{
    public class CommandConfigTests
    {
        [Fact]
        public void CanSerializeCommand()
        {
            var json = @"{'OriginalName': 'oude naam', 'Name': 'nieuwe naam'}";
            var projectsMoq = new Mock<IProductService>();
            var commandConfig = new CommandConfig { Name = "Rename", ProcessorName = "Product", Processor = projectsMoq.Object };
            RenameProductCommand command = commandConfig.GetCommand(json) as RenameProductCommand;
            Assert.Equal("oude naam", command.OriginalName);
        }
    }
}
