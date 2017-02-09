using CommandsShared;
using ContactsShared;
using DateTimeShared;
using Moq;
using SoftwareManagementEventSourceRepository;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Commands
{
    public class CommandManagerTests
    {
        // todo: consider creating test specific commands
        [Fact(DisplayName = "CanMergeCommands_WithNoExistingState")] // same as processcommands
        [Trait("Category", "IntegrationTests")]
        public void CommandManager_CanMergeCommandsWithNoExistingState()
        {
            Guid entityGuid = Guid.NewGuid();
            var existingCreateCommand = new Fakes.CommandState { CommandTypeId = "CreateContactCommand", EntityGuid = entityGuid, Guid = Guid.NewGuid(), CreatedOn = DateTime.Now, ExecutedOn = DateTime.Now, ReceivedOn = DateTime.Now, ParametersJson = "{'Name': 'John Smith'}" };
            var existingChangeEmailCommand = new Fakes.CommandState { CommandTypeId = "ChangeEmailForContactCommand", EntityGuid = entityGuid, Guid = Guid.NewGuid(), CreatedOn = DateTime.Now, ExecutedOn = DateTime.Now, ReceivedOn = DateTime.Now, ParametersJson = "{'Email': 'john@smith.net'}" };
            var existingCommands = new List<ICommandState> { existingCreateCommand, existingChangeEmailCommand };
            DateTimeProvider dateTimeProvider = new DateTimeProvider();
            var repo = new Mock<ICommandStateRepository>();
            repo.Setup(s => s.CreateCommandState()).Returns(new Fakes.CommandState());
            repo.Setup(s => s.GetCommandStates(entityGuid)).Returns(existingCommands);
            var contactRepo = new EventSourcedMainRepository();
            var processor = new ContactService(contactRepo, dateTimeProvider);
            // todo: make builder
            var sut = new CommandManager(repo.Object, dateTimeProvider);

            var processorConfig = new ProcessorConfig();
            processorConfig.Assembly = "SoftwareManagementCore";
            processorConfig.NameSpace = "ContactsShared";
            processorConfig.Entity = "Contact";
            processorConfig.Processor = processor;
            sut.AddConfig(processorConfig);

            // todo: make builder
            var updateEmailCommandDto = new CommandDto { CreatedOn = DateTime.Now.AddTicks(1), Entity = "Contact", EntityGuid = entityGuid, Guid = Guid.NewGuid(), Name = "ChangeEmailFor", ParametersJson = @"{Email: 'john@smith.com', OriginalEmail:'john@smith.net'}" };
            //            var updateNameCommandDto = new CommandDto { CreatedOn = DateTime.Now.AddTicks(2), Entity = "Contact", EntityGuid = entityGuid, Guid = Guid.NewGuid(), Name = "Rename", ParametersJson = @"{ Name: 'James Smith', OriginalName: 'John Smith'}" };
            var commandDtos = new List<CommandDto> { updateEmailCommandDto };

            sut.MergeCommands(commandDtos);

            var contact = processor.GetContact(entityGuid);

            Assert.Equal("John Smith", contact.Name);
            Assert.Equal("john@smith.com", contact.Email);
        }
    }
}
