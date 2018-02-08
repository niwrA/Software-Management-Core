using CommandsShared;
using Moq;
using ContactsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Contacts
{
    [Trait("Entity", "Contact")]
    public class CompanyCommandsTests
    {
        [Fact(DisplayName = "CreateContactCommand")]
        public void CreateCommand()
        {
            var contactsMock = new Mock<IContactService>();
            var sut = new CommandBuilder<CreateContactCommand>().Build(contactsMock.Object);

            sut.Name = "New Contact";
            sut.Execute();

            contactsMock.Verify(s => s.CreateContact(sut.EntityGuid, sut.Name), Times.Once);
        }

        [Fact(DisplayName = "DeleteContactCommand")]
        public void DeleteCommand()
        {
            var contactsMock = new Mock<IContactService>();
            var sut = new CommandBuilder<DeleteContactCommand>().Build(contactsMock.Object) as DeleteContactCommand;

            sut.Execute();

            contactsMock.Verify(s => s.DeleteContact(sut.EntityGuid), Times.Once);
        }

        [Fact(DisplayName = "RenameContactCommand")]
        public void RenameCommand()
        {
            var sutBuilder = new ContactCommandBuilder<RenameContactCommand>();
            var sut = sutBuilder.Build() as RenameContactCommand;

            sut.Name = "New Name";
            sut.OriginalName = "Original Name";
            sut.Execute();

            sutBuilder.ContactMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }

        [Fact(DisplayName = "ChangeEmailForContactCommand")]
        public void ChangeEmailForContactCommand()
        {
            var sutBuilder = new ContactCommandBuilder<ChangeEmailForContactCommand>();
            var sut = sutBuilder.Build() as ChangeEmailForContactCommand;

            sut.Email = "New";
            sut.OriginalEmail = "Original";
            sut.Execute();

            sutBuilder.ContactMock.Verify(s => s.ChangeEmail(sut.Email, sut.OriginalEmail), Times.Once);
        }
        [Fact(DisplayName = "ChangeAvatarForContactCommand")]
        public void ChangeAvatarForContactCommand()
        {
            var sutBuilder = new ContactCommandBuilder<ChangeAvatarForContactCommand>();
            var sut = sutBuilder.Build() as ChangeAvatarForContactCommand;

            sut.AvatarFileGuid = Guid.NewGuid();
            sut.AvatarUrl = "some/url";
            sut.Execute();

            sutBuilder.ContactMock.Verify(s => s.ChangeAvatar(sut.AvatarFileGuid, sut.OriginalAvatarFileGuid, sut.AvatarUrl), Times.Once);
        }
        [Fact(DisplayName = "ChangeBirthDateOfContactCommand")]
        public void ChangeBirthDateOfContactCommand()
        {
            var sutBuilder = new ContactCommandBuilder<ChangeBirthDateOfContactCommand>();
            var sut = sutBuilder.Build() as ChangeBirthDateOfContactCommand;

            sut.OriginalBirthDate = DateTime.Now;
            sut.BirthDate = DateTime.Now;
            sut.Execute();

            sutBuilder.ContactMock.Verify(s => s.ChangeBirthDate(sut.BirthDate, sut.OriginalBirthDate), Times.Once);
        }

    }

    class ContactCommandBuilder<T> where T : ICommand, new()
    {
        public Mock<IContact> ContactMock { get; set; }
        public ICommand Build()
        {
            var contactsMock = new Mock<IContactService>();
            var contactMock = new Mock<IContact>();
            this.ContactMock = contactMock;

            var sut = new CommandBuilder<T>().Build(contactsMock.Object);

            contactsMock.Setup(s => s.GetContact(sut.EntityGuid)).Returns(contactMock.Object);

            return sut;
        }
    }
}
