using Moq;
using DesignsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using Xunit;

namespace SoftwareManagementCoreTests.PropertyElements
{
    [Trait("Entity", "PropertyElement")]
    public class PropertyElementCommandsTests
    {
        [Fact(DisplayName = "CreateCommand")]
        public void CreatePropertyElementWithCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<CreatePropertyElementCommand>().Build(designServiceMock.Object) as CreatePropertyElementCommand;
            var designMock = new Mock<IDesign>();
            designServiceMock.Setup(v => v.GetDesign(sut.EntityRootGuid)).Returns(designMock.Object);

            var epicMock = new Mock<IEpicElement>();
            designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

            var entityMock = new Mock<IEntityElement>();
            epicMock.Setup(s => s.GetEntityElement(sut.EntityElementGuid)).Returns(entityMock.Object);

            sut.Name = "New PropertyElement";
            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.EntityRootGuid), Times.Once);
            entityMock.Verify(v => v.AddPropertyElement(sut.EntityGuid, sut.Name), Times.Once);
        }

        [Fact(DisplayName = "DeleteCommand")]
        public void DeleteCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<DeletePropertyElementCommand>().Build(designServiceMock.Object) as DeletePropertyElementCommand;

            var designMock = new Mock<IDesign>();
            designServiceMock.Setup(v => v.GetDesign(sut.EntityRootGuid)).Returns(designMock.Object);

            var epicMock = new Mock<IEpicElement>();
            designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

            var entityMock = new Mock<IEntityElement>();
            epicMock.Setup(s => s.GetEntityElement(sut.EntityElementGuid)).Returns(entityMock.Object);

            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.EntityRootGuid), Times.Once);
            entityMock.Verify(v => v.DeletePropertyElement(sut.EntityGuid), Times.Once);
        }

        [Fact(DisplayName = "RenameCommand")]
        public void RenameCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<RenamePropertyElementCommand>().Build(designServiceMock.Object);

            sut.EpicElementGuid = Guid.NewGuid();

            var designMock = new Mock<IDesign>();
            designServiceMock.Setup(v => v.GetDesign(sut.EntityRootGuid)).Returns(designMock.Object);

            var epicMock = new Mock<IEpicElement>();
            designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

            var entityMock = new Mock<IEntityElement>();
            epicMock.Setup(s => s.GetEntityElement(sut.EntityElementGuid)).Returns(entityMock.Object);

            var propertyMock = new Mock<IPropertyElement>();
            entityMock.Setup(s => s.GetPropertyElement(sut.EntityGuid)).Returns(propertyMock.Object);

            sut.Name = "New name";
            //            sut.OriginalName = "";
            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.EntityRootGuid), Times.Once);
            propertyMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }

        [Fact(DisplayName = "ChangeDescriptionCommand")]
        public void ChangeDescriptionCommand()
        {
            var designServiceMock = new Mock<IDesignService>();
            var sut = new CommandBuilder<ChangeDescriptionOfPropertyElementCommand>().Build(designServiceMock.Object) as ChangeDescriptionOfPropertyElementCommand;

            sut.EpicElementGuid = Guid.NewGuid();

            var designMock = new Mock<IDesign>();
            designServiceMock.Setup(v => v.GetDesign(sut.EntityRootGuid)).Returns(designMock.Object);

            var epicMock = new Mock<IEpicElement>();
            designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

            var entityMock = new Mock<IEntityElement>();
            epicMock.Setup(s => s.GetEntityElement(sut.EntityElementGuid)).Returns(entityMock.Object);

            var propertyMock = new Mock<IPropertyElement>();
            entityMock.Setup(s => s.GetPropertyElement(sut.EntityGuid)).Returns(propertyMock.Object);

            sut.Description = "New description";
            sut.Execute();

            designServiceMock.Verify(v => v.GetDesign(sut.EntityRootGuid), Times.Once);
            propertyMock.Verify(s => s.ChangeDescription(sut.Description), Times.Once);
        }

    [Fact(DisplayName = "ChangeDataTypeCommand")]
    public void ChangeDataTypeCommand()
    {
      var designServiceMock = new Mock<IDesignService>();
      var sut = new CommandBuilder<ChangeDataTypeOfPropertyElementCommand>().Build(designServiceMock.Object);

      sut.EpicElementGuid = Guid.NewGuid();

      var designMock = new Mock<IDesign>();
      designServiceMock.Setup(v => v.GetDesign(sut.EntityRootGuid)).Returns(designMock.Object);

      var epicMock = new Mock<IEpicElement>();
      designMock.Setup(s => s.GetEpicElement(sut.EpicElementGuid)).Returns(epicMock.Object);

      var entityMock = new Mock<IEntityElement>();
      epicMock.Setup(s => s.GetEntityElement(sut.EntityElementGuid)).Returns(entityMock.Object);

      var propertyMock = new Mock<IPropertyElement>();
      entityMock.Setup(s => s.GetPropertyElement(sut.EntityGuid)).Returns(propertyMock.Object);

      sut.DataType = "New type";
      sut.OriginalDataType = "Old type";
      sut.Execute();

      designServiceMock.Verify(v => v.GetDesign(sut.EntityRootGuid), Times.Once);
      propertyMock.Verify(s => s.ChangeDataType(sut.DataType, sut.OriginalDataType), Times.Once);
    }


  }
}