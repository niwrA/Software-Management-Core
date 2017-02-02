using ContactsShared;
using System;
using Xunit;
using Moq;
using DateTimeShared;

namespace SoftwareManagementCoreTests
{
    [Trait("Entity", "Contact")]
    public class ContactsTests
    {
        [Fact(DisplayName = "Create")]
        public void CreateContact_ImplementsIRepository()
        {
            var repoMock = new Moq.Mock<IContactStateRepository>();
            var sut = new ContactService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<IContactState>();

            repoMock.Setup(t => t.CreateContactState(It.IsAny<Guid>(), It.IsAny<string>())).Returns(stateMock.Object);

            var guid = Guid.NewGuid();
            var name = "New Contact";
            sut.CreateContact(guid, name);

            repoMock.Verify(s => s.CreateContactState(guid, name), Times.Once);
        }

        [Fact(DisplayName = "Delete")]
        public void CanDeleteContact()
        {
            var repoMock = new Mock<IContactStateRepository>();
            var sut = new ContactService(repoMock.Object, new DateTimeProvider());
            var guid = Guid.NewGuid();

            sut.DeleteContact(guid);

            repoMock.Verify(s => s.DeleteContactState(guid));
        }

        [Fact(DisplayName = "Get")]
        public void CanGetContact()
        {
            var repoMock = new Mock<IContactStateRepository>();
            var sut = new ContactService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<IContactState>();
            var stateMockAlt = new Mock<IContactState>();

            var stateGuid = Guid.NewGuid();
            var altStateGuid = Guid.NewGuid();

            repoMock.Setup(t => t.GetContactState(stateGuid)).Returns(stateMock.Object);
            repoMock.Setup(t => t.GetContactState(altStateGuid)).Returns(stateMockAlt.Object);

            sut.GetContact(stateGuid);
            repoMock.Verify(t => t.GetContactState(stateGuid));

            sut.GetContact(altStateGuid);
            repoMock.Verify(t => t.GetContactState(altStateGuid));
        }

        [Fact(DisplayName = "Rename")]
        public void CanRenameContact()
        {
            var stateMock = new Mock<IContactState>();
            var sut = new Contact(stateMock.Object);

            stateMock.Setup(s => s.Name).Returns("old");

            sut.Rename("new", "old");

            stateMock.VerifySet(t => t.Name = "new");
        }
    }
}
