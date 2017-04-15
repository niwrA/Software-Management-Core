using Moq;
using DesignsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using Xunit;

namespace SoftwareManagementCoreTests.CommandElements
{
    [Trait("Entity", "CommandElement")]
    public class CommandElementCommandsTests
    {
        [Fact(DisplayName = "CreateCommand")]
        public void CreateCommandElementWithCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<CreateCommandElementCommand>().Build(designServiceMock.Object) as CreateCommandElementCommand;
            var designMock = new Mock<IDesign>();
            designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);

            var epicMock = new Mock<IEpicElement>();
            designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

            var entityMock = new Mock<IEntityElement>();
            epicMock.Setup(s => s.GetEntityElement(sut.EntityElementGuid)).Returns(entityMock.Object);

            sut.Name = "New CommandElement";
            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
            entityMock.Verify(v => v.AddCommandElement(sut.EntityGuid, sut.Name), Times.Once);
        }

        [Fact(DisplayName = "DeleteCommand")]
        public void DeleteCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<DeleteCommandElementCommand>().Build(designServiceMock.Object) as DeleteCommandElementCommand;

            var designMock = new Mock<IDesign>();
            designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);

            var epicMock = new Mock<IEpicElement>();
            designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

            var entityMock = new Mock<IEntityElement>();
            epicMock.Setup(s => s.GetEntityElement(sut.EntityElementGuid)).Returns(entityMock.Object);

            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
            entityMock.Verify(v => v.DeleteCommandElement(sut.EntityGuid), Times.Once);
        }

        [Fact(DisplayName = "RenameCommand")]
        public void RenameCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<RenameCommandElementCommand>().Build(designServiceMock.Object) as RenameCommandElementCommand;

            sut.DesignGuid = Guid.NewGuid();
            sut.EpicElementGuid = Guid.NewGuid();

            var designMock = new Mock<IDesign>();
            designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);

            var epicMock = new Mock<IEpicElement>();
            designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

            var entityMock = new Mock<IEntityElement>();
            epicMock.Setup(s => s.GetEntityElement(sut.EntityElementGuid)).Returns(entityMock.Object);

            var commandMock = new Mock<ICommandElement>();
            entityMock.Setup(s => s.GetCommandElement(sut.EntityGuid)).Returns(commandMock.Object);

            sut.Name = "New name";
            //            sut.OriginalName = "";
            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
            commandMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }

        [Fact(DisplayName = "ChangeDescriptionCommand")]
        public void ChangeDescriptionCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<ChangeDescriptionOfCommandElementCommand>().Build(designServiceMock.Object) as ChangeDescriptionOfCommandElementCommand;

            sut.DesignGuid = Guid.NewGuid();
            sut.EpicElementGuid = Guid.NewGuid();

            var designMock = new Mock<IDesign>();
            designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);

            var epicMock = new Mock<IEpicElement>();
            designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

            var entityMock = new Mock<IEntityElement>();
            epicMock.Setup(s => s.GetEntityElement(sut.EntityElementGuid)).Returns(entityMock.Object);

            var commandMock = new Mock<ICommandElement>();
            entityMock.Setup(s => s.GetCommandElement(sut.EntityGuid)).Returns(commandMock.Object);

            sut.Description = "New description";
            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
            commandMock.Verify(s => s.ChangeDescription(sut.Description), Times.Once);
        }


    }
}