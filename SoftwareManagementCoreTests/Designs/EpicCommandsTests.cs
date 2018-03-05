using niwrA.CommandManager;
using DateTimeShared;
using Moq;
using DesignsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.EpicElements
{
    [Trait("Entity", "EpicElement")]
    public class EpicElementCommandsTests
    {
        [Fact(DisplayName = "CreateCommand")]
        public void CreateEpicElementWithCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<CreateEpicElementCommand>().Build(designServiceMock.Object) as CreateEpicElementCommand;
            var designMock = new Mock<IDesign>();

            designServiceMock.Setup(v => v.GetDesign(sut.EntityRootGuid)).Returns(designMock.Object);

            sut.Name = "New EpicElement";
            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.EntityRootGuid), Times.Once);
            designMock.Verify(v => v.AddEpicElement(sut.EntityGuid, sut.Name), Times.Once);
        }

        [Fact(DisplayName = "DeleteCommand")]
        public void DeleteCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<DeleteEpicElementCommand>().Build(designServiceMock.Object) as DeleteEpicElementCommand;
            var designMock = new Mock<IDesign>();

            designServiceMock.Setup(v => v.GetDesign(sut.EntityRootGuid)).Returns(designMock.Object);

            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.EntityRootGuid), Times.Once);
            designMock.Verify(v => v.DeleteEpicElement(sut.EntityGuid), Times.Once);
        }

        [Fact(DisplayName = "RenameCommand")]
        public void RenameCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<RenameEpicElementCommand>().Build(designServiceMock.Object) as RenameEpicElementCommand;
            var designMock = new Mock<IDesign>();
            var epicMock = new Mock<IEpicElement>();

            designServiceMock.Setup(v => v.GetDesign(sut.EntityRootGuid)).Returns(designMock.Object);
            designMock.Setup(v => v.GetEpicElement(sut.EntityGuid)).Returns(epicMock.Object);

            sut.Name = "New name";
            sut.OriginalName = "";
            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.EntityRootGuid), Times.Once);
            epicMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }

        [Fact(DisplayName = "ChangeDescriptionCommand")]
        public void ChangeDescriptionCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<ChangeDescriptionOfEpicElementCommand>().Build(designServiceMock.Object) as ChangeDescriptionOfEpicElementCommand;
            var designMock = new Mock<IDesign>();
            var epicMock = new Mock<IEpicElement>();

            designServiceMock.Setup(v => v.GetDesign(sut.EntityRootGuid)).Returns(designMock.Object);
            designMock.Setup(v => v.GetEpicElement(sut.EntityGuid)).Returns(epicMock.Object);

            sut.Description = "New description";
            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.EntityRootGuid), Times.Once);
            epicMock.Verify(s => s.ChangeDescription(sut.Description), Times.Once);
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
            public static string Namespace = "EpicElementsShared";
            public static string Entity = "EpicElement";
        }
    }
}