using ProjectRoleAssignmentsShared;
using System;
using System.Collections.Generic;
using System.Text;
using ContactsShared;

namespace SoftwareManagementCoreTests.Fakes
{
    public class ProjectRoleAssignmentState : IProjectRoleAssignmentState
    {
        public ProjectRoleAssignmentState()
        {
            Guid = Guid.NewGuid();
            ContactGuid = Guid.NewGuid();
            ProjectRoleGuid = Guid.NewGuid();
            StartDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date.AddYears(1);
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
            ContactName = "Jane Smith";
        }
        public Guid Guid { get; set; }
        public Guid ContactGuid { get; set; }
        public Guid ProjectRoleGuid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ContactName { get; set; }
    }
}
