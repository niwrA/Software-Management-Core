using ContactsShared;
using System;
using System.Collections.Generic;

namespace SoftwareManagementCoreApiTests.Fakes
{
    public class ContactState : IContactState
    {
        public ContactState()
        {
            Guid = Guid.NewGuid();
            Name = "Contact Name";
            Email = "some@email.adr";
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
            BirthDate = DateTime.Now.Date;
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime? BirthDate{ get; set; }
        public Guid? AvatarFileGuid { get; set; }
        public string AvatarUrl { get; set; }
    }
}
