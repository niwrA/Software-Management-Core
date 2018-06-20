using DesignsShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Designs
{
    [Trait("Entity", "PropertyElement")]
    public class PropertiesTests
    {
        [Fact(DisplayName = "Can Rename PropertyElement")]
        public void CanRenameProperty()
        {
            var stateMock = new Mock<IPropertyElementState>();
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new PropertyElement(stateMock.Object, repoMock.Object);

            sut.Rename("new", "old");

            stateMock.Setup(s => s.Name).Returns("old");
            sut.Rename("new", "old");
            stateMock.VerifySet(t => t.Name = "new");
        }

        [Fact(DisplayName = "Can Change Description of PropertyElement")]
        public void CanChangeDescriptionOfPropertyElement()
        {
            var stateMock = new Mock<IPropertyElementState>();
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new PropertyElement(stateMock.Object, repoMock.Object);

            sut.ChangeDescription("new");

            stateMock.VerifySet(t => t.Description = "new");
        }

        [Fact(DisplayName = "Can Change DataType of PropertyElement")]
        public void CanChangeDataTypeOfPropertyElement()
        {
            var stateMock = new Mock<IPropertyElementState>();
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new PropertyElement(stateMock.Object, repoMock.Object);

            stateMock.Setup(s => s.DataType).Returns("old");
            sut.ChangeDataType("new", "old");

            stateMock.VerifySet(t => t.DataType = "new");
        }
        [Fact(DisplayName = "Can Change IsState of PropertyElement")]
        public void CanChangeIsStateOfPropertyElement()
        {
            var stateMock = new Mock<IPropertyElementState>();
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new PropertyElement(stateMock.Object, repoMock.Object);

            sut.ChangeIsState(true);

            stateMock.VerifySet(t => t.IsState = true);
        }
    }
}
