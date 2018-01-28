using Moq;
using DesignsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using Xunit;

namespace SoftwareManagementCoreTests.EntityElements
{
  [Trait("Entity", "EntityElement")]
  public class EntityElementCommandsTests
  {
    [Fact(DisplayName = "CreateCommand")]
    public void CreateEntityElementWithCommand()
    {
      var designServiceMock = new Mock<IDesignService>();
      var sut = new CommandBuilder<CreateEntityElementCommand>().Build(designServiceMock.Object) as CreateEntityElementCommand;
      var designMock = new Mock<IDesign>();
      designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);

      var epicMock = new Mock<IEpicElement>();
      designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

      sut.Name = "New EntityElement";
      sut.Execute();

      designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
      epicMock.Verify(v => v.AddEntityElement(sut.EntityGuid, sut.Name, null), Times.Once);
    }

    [Fact(DisplayName = "DeleteCommand")]
    public void DeleteCommand()
    {
      var designServiceMock = new Mock<IDesignService>();
      var sut = new CommandBuilder<DeleteEntityElementCommand>().Build(designServiceMock.Object) as DeleteEntityElementCommand;

      var designMock = new Mock<IDesign>();
      designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);

      var epicMock = new Mock<IEpicElement>();
      designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

      sut.Execute();

      designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
      epicMock.Verify(v => v.DeleteEntityElement(sut.EntityGuid), Times.Once);
    }

    [Fact(DisplayName = "RenameCommand")]
    public void RenameCommand()
    {
      var designServiceMock = new Mock<IDesignService>();
      var sut = new CommandBuilder<RenameEntityElementCommand>().Build(designServiceMock.Object) as RenameEntityElementCommand;
      var designMock = new Mock<IDesign>();
      var epicMock = new Mock<IEpicElement>();
      var entityMock = new Mock<IEntityElement>();

      sut.DesignGuid = Guid.NewGuid();
      sut.EpicElementGuid = Guid.NewGuid();
      designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);
      designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);
      epicMock.Setup(v => v.GetEntityElement(sut.EntityGuid)).Returns(entityMock.Object);

      sut.Name = "New name";
      //            sut.OriginalName = "";
      sut.Execute();

      designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
      entityMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
    }

    [Fact(DisplayName = "ChangeDescriptionCommand")]
    public void ChangeDescriptionCommand()
    {
      var designServiceMock = new Mock<IDesignService>();
      var sut = new CommandBuilder<ChangeDescriptionOfEntityElementCommand>().Build(designServiceMock.Object) as ChangeDescriptionOfEntityElementCommand;
      var designMock = new Mock<IDesign>();
      var epicMock = new Mock<IEpicElement>();
      var entityMock = new Mock<IEntityElement>();

      sut.DesignGuid = Guid.NewGuid();
      designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);
      designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);
      epicMock.Setup(v => v.GetEntityElement(sut.EntityGuid)).Returns(entityMock.Object);

      sut.Description = "New description";
      sut.Execute();

      designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
      entityMock.Verify(s => s.ChangeDescription(sut.Description), Times.Once);
    }

    [Fact(DisplayName = "ChangePluralNameCommand")]
    public void ChangePluralNameCommand()
    {
      var designServiceMock = new Mock<IDesignService>();
      var sut = new CommandBuilder<ChangePluralNameOfEntityElementCommand>().Build(designServiceMock.Object);
      var designMock = new Mock<IDesign>();
      var epicMock = new Mock<IEpicElement>();
      var entityMock = new Mock<IEntityElement>();

      sut.DesignGuid = Guid.NewGuid();
      sut.EpicElementGuid = Guid.NewGuid();
      designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);
      designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);
      epicMock.Setup(v => v.GetEntityElement(sut.EntityGuid)).Returns(entityMock.Object);

      sut.PluralName = "New name";
      sut.OriginalPluralName = "Orignial name";
      sut.Execute();

      designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
      entityMock.Verify(s => s.ChangePluralName(sut.PluralName, sut.OriginalPluralName), Times.Once);
    }

    [Fact(DisplayName = "ChangeIsCollectionCommand")]
    public void ChangeIsCollectionCommand()
    {
      var designServiceMock = new Mock<IDesignService>();
      var sut = new CommandBuilder<ChangeIsCollectionForEntityElementCommand>().Build(designServiceMock.Object);
      var designMock = new Mock<IDesign>();
      var epicMock = new Mock<IEpicElement>();
      var entityMock = new Mock<IEntityElement>();

      sut.DesignGuid = Guid.NewGuid();
      designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);
      designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);
      epicMock.Setup(v => v.GetEntityElement(sut.EntityGuid)).Returns(entityMock.Object);

      sut.IsCollection = true;
      sut.Execute();

      designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
      entityMock.Verify(s => s.ChangeIsCollection(sut.IsCollection), Times.Once);
    }

    [Fact(DisplayName = "AddChildToEntityElementCommand")]
    public void AddChildToEntityElementCommand()
    {
      var designServiceMock = new Mock<IDesignService>();
      var sut = new CommandBuilder<AddChildToEntityElementCommand>().Build(designServiceMock.Object);
      var designMock = new Mock<IDesign>();
      designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);

      var epicMock = new Mock<IEpicElement>();
      designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

      sut.Name = "New EntityElement";
      sut.ParentGuid = Guid.NewGuid();
      sut.Execute();

      designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
      epicMock.Verify(v => v.AddEntityElement(sut.EntityGuid, sut.Name, sut.ParentGuid), Times.Once);
    }
    [Fact(DisplayName = "RemoveChildFromEntityElementCommand")]
    public void RemoveChildFromEntityElementCommand()
    {
      var designServiceMock = new Mock<IDesignService>();
      var sut = new CommandBuilder<RemoveChildFromEntityElementCommand>().Build(designServiceMock.Object);

      var designMock = new Mock<IDesign>();
      designServiceMock.Setup(v => v.GetDesign(sut.DesignGuid)).Returns(designMock.Object);

      var epicMock = new Mock<IEpicElement>();
      designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

      sut.Execute();

      designServiceMock.Verify(v => v.GetDesign(sut.DesignGuid), Times.Once);
      epicMock.Verify(v => v.DeleteEntityElement(sut.EntityGuid), Times.Once);
    }

  }
}