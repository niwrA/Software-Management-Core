using ProjectsShared;
using System;
using System.Collections.Generic;

namespace SoftwareManagementCoreApiTests.Fakes
{
    public class ProjectState : IProjectState
    {
        public ProjectState()
        {
            Guid = Guid.NewGuid();
            Name = "Project Name";
            ProjectRoleStates = new List<IProjectRoleState>();
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<IProjectRoleState> ProjectRoleStates { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
