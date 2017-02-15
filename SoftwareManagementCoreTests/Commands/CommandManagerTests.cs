﻿using CommandsShared;
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
        [Fact(DisplayName = "CanMergeCommands_WithExistingStateAndApply")] 
        // basically we are testing full event/commandsourcing here, restoring state
        // from existing commands and applying new commands to that state
        [Trait("Category", "IntegrationTests")]
        public void CommandManager_CanMergeCommandsWithExistingStateAndApply()
        {
            Guid entityGuid = Guid.NewGuid();

            // create some existing commands
            var existingCreateCommand = new Fakes.CommandState { CommandTypeId = "CreateContactCommand", EntityGuid = entityGuid, Guid = Guid.NewGuid(), CreatedOn = DateTime.Now, ExecutedOn = DateTime.Now, ReceivedOn = DateTime.Now, ParametersJson = "{'Name': 'John Smith'}" };
            var existingChangeEmailCommand = new Fakes.CommandState { CommandTypeId = "ChangeEmailForContactCommand", EntityGuid = entityGuid, Guid = Guid.NewGuid(), CreatedOn = DateTime.Now, ExecutedOn = DateTime.Now, ReceivedOn = DateTime.Now, ParametersJson = "{'Email': 'john@smith.net'}" };
            var existingCommands = new List<ICommandState> { existingCreateCommand, existingChangeEmailCommand };
            DateTimeProvider dateTimeProvider = new DateTimeProvider();

            // mock that these are returned by the repository
            var repo = new Mock<ICommandStateRepository>();
            repo.Setup(s => s.CreateCommandState()).Returns(new Fakes.CommandState());
            repo.Setup(s => s.GetCommandStates(entityGuid)).Returns(existingCommands);

            // create a real contactservice, with an eventsourcing version of the main repository,
            // which basically only holds state in memory
            // todo: make builder
            var contactRepo = new EventSourcedMainRepository();
            var processor = new ContactService(contactRepo, dateTimeProvider);
            var sut = new CommandService(repo.Object, dateTimeProvider);

            // setup configuration for all Contact commands to use the service above
            // for command processing
            var processorConfig = new ProcessorConfig();
            processorConfig.Assembly = "SoftwareManagementCore";
            processorConfig.NameSpace = "ContactsShared";
            processorConfig.Entity = "Contact";
            processorConfig.Processor = processor;
            sut.AddConfig(processorConfig);

            // create a new command to merge/apply
            // todo: make builder
            var updateEmailCommandDto = new CommandDto { CreatedOn = DateTime.Now.AddTicks(1), Entity = "Contact", EntityGuid = entityGuid, Guid = Guid.NewGuid(), Name = "ChangeEmailFor", ParametersJson = @"{Email: 'john@smith.com', OriginalEmail:'john@smith.net'}" };
            var commandDtos = new List<CommandDto> { updateEmailCommandDto };

            // perform the actual merge
            sut.MergeCommands(commandDtos);

            // retrieve the contact from in-memory state to check if state is as expected
            var contact = processor.GetContact(entityGuid);

            Assert.Equal("John Smith", contact.Name);
            Assert.Equal("john@smith.com", contact.Email);
        }
    }
}
