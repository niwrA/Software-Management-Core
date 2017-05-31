using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace ContactsShared
{
    public abstract class ContactCommand : CommandBase
    {
        public ContactCommand() : base() { }
        public ContactCommand(ICommandStateRepository repo) : base(repo) { }
    }

    public class DeleteContactCommand : ContactCommand
    {
        public override void Execute()
        {
            ((IContactService)base.CommandProcessor).DeleteContact(this.EntityGuid);
            base.Execute();
        }

    }

    public class CreateContactCommand : ContactCommand
    {
        public string Name { get; set; }
        public override void Execute()
        {
            ((IContactService)base.CommandProcessor).CreateContact(this.EntityGuid, this.Name);
            base.Execute();
        }
    }

    public class RenameContactCommand : ContactCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var product = ((IContactService)base.CommandProcessor).GetContact(this.EntityGuid);
            product.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }

    public class ChangeEmailForContactCommand : ContactCommand
    {
        public string OriginalEmail { get; set; }
        public string Email { get; set; }
        public override void Execute()
        {
            var product = ((IContactService)base.CommandProcessor).GetContact(this.EntityGuid);
            product.ChangeEmail(this.Email, this.OriginalEmail);
            base.Execute();
        }
    }

    public class ChangeAvatarForContactCommand : ContactCommand
    {
        public Guid? OriginalAvatarFileGuid { get; set; }
        public Guid? AvatarFileGuid { get; set; }
        public string AvatarUrl { get; set; }
        public override void Execute()
        {
            var contact = ((IContactService)base.CommandProcessor).GetContact(this.EntityGuid);
            contact.ChangeAvatar(this.AvatarFileGuid, this.OriginalAvatarFileGuid, this.AvatarUrl);
            base.Execute();
        }
    }
    public class ChangeBirthDateOfContactCommand : ContactCommand
    {
        public DateTime? OriginalBirthDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public override void Execute()
        {
            var contact = ((IContactService)base.CommandProcessor).GetContact(this.EntityGuid);
            contact.ChangeBirthDate(this.BirthDate, this.OriginalBirthDate);
            base.Execute();
        }
    }
}
