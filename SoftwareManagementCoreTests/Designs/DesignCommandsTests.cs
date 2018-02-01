using CommandsShared;
using DateTimeShared;
using Moq;
using DesignsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace SoftwareManagementCoreTests.Designs
{
  [Trait("Entity", "Design")]
  public class DesignCommandsTests
  {
    [Fact(DisplayName = "CreateCommand")]
    public void CreateDesignWithCommand()
    {
      var designsMock = new Mock<IDesignService>();
      var sut = new CommandBuilder<CreateDesignCommand>().Build(designsMock.Object) as CreateDesignCommand;

      sut.Name = "New Design";
      sut.Execute();

      designsMock.Verify(v => v.CreateDesign(sut.EntityGuid, sut.Name), Times.Once);
    }

    [Fact(DisplayName = "DeleteCommand")]
    public void DeleteCommand()
    {
      var designsMock = new Mock<IDesignService>();
      var sut = new CommandBuilder<DeleteDesignCommand>().Build(designsMock.Object) as DeleteDesignCommand;

      sut.Execute();

      designsMock.Verify(s => s.DeleteDesign(sut.EntityGuid), Times.Once);
    }

    [Fact(DisplayName = "RenameCommand")]
    public void RenameCommand()
    {
      var sutBuilder = new DesignCommandBuilder<RenameDesignCommand>();
      var sut = sutBuilder.Build() as RenameDesignCommand;

      sut.OriginalName = "Old name";
      sut.Name = "New name";
      sut.Execute();

      sutBuilder.DesignMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
    }

    [Fact(DisplayName = "ChangeDescriptionCommand")]
    public void ChangeDescriptionCommand()
    {
      var sutBuilder = new DesignCommandBuilder<ChangeDescriptionOfDesignCommand>();
      var sut = sutBuilder.Build() as ChangeDescriptionOfDesignCommand;

      sut.Description = "New description";
      sut.Execute();

      sutBuilder.DesignMock.Verify(s => s.ChangeDescription(sut.Description), Times.Once);
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
      public static string Namespace = "DesignsShared";
      public static string Entity = "Design";
    }

    [Fact(DisplayName = "CreateCommandToRepository")]
    [Trait("Type", "IntegrationTest")]
    public void CanCreateDesignWithCommand_CallsRepository()
    {
      var commandRepoMock = new Mock<ICommandStateRepository>();
      var designsMock = new Mock<IDesignService>();
      var commandState = new Fakes.CommandState();
      commandRepoMock.Setup(t => t.CreateCommandState()).Returns(commandState);

      var guid = Guid.NewGuid();
      var name = "New Project";

      var sut = new CommandService(commandRepoMock.Object, new DateTimeProvider());
      var commandConfig = new CommandConfig { Assembly = TestGlobals.Assembly, NameSpace = TestGlobals.Namespace, CommandName = CommandTypes.Create.ToString(), Entity = TestGlobals.Entity, Processor = designsMock.Object };

      sut.AddCommandConfigs(new List<ICommandConfig>() { commandConfig });

      var commandDto = new CommandDto { Entity = TestGlobals.Entity, EntityGuid = guid, Name = CommandTypes.Create.ToString(), ParametersJson = @"{name: '" + name + "'}" };
      var sutResult = sut.ProcessCommand(commandDto);

      designsMock.Verify(v => v.CreateDesign(guid, name), Times.Once);
      Assert.Equal(commandDto.Entity, sutResult.First().Entity);
    }

  }
  class DesignCommandBuilder<T> where T : ICommand, new()
  {
    public Mock<IDesign> DesignMock { get; set; }
    public ICommand Build()
    {
      var designsMock = new Mock<IDesignService>();
      var designMock = new Mock<IDesign>();
      this.DesignMock = designMock;

      var sut = new CommandBuilder<T>().Build(designsMock.Object);

      designsMock.Setup(s => s.GetDesign(sut.EntityGuid)).Returns(designMock.Object);

      return sut;
    }
  }
}
