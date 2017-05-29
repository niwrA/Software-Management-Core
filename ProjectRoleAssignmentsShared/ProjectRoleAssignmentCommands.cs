using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace ProjectRoleAssignmentsShared
{
    public abstract class ProjectRoleAssignmentCommand : CommandBase
    {
        public ProjectRoleAssignmentCommand() : base() { }
        public ProjectRoleAssignmentCommand(ICommandStateRepository repo) : base(repo) { }
    }

    public class CreateProjectRoleAssignmentCommand : ProjectRoleAssignmentCommand
    {
        public Guid ContactGuid { get; set; }
        public Guid ProjectGuid { get; set; }
        public Guid ProjectRoleGuid { get; set; } 
        public string ContactName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public override void Execute()
        {
            ((IProjectRoleAssignmentService)base.CommandProcessor).CreateProjectRoleAssignment(this.EntityGuid, this.ContactGuid, this.ProjectGuid, this.ProjectRoleGuid, this.StartDate, this.EndDate, this.ContactName);
            base.Execute();
        }
    }
    
    public class DeleteProjectRoleAssignmentCommand : ProjectRoleAssignmentCommand
    {
        public override void Execute()
        {
            ((IProjectRoleAssignmentService)base.CommandProcessor).DeleteProjectRoleAssignment(this.EntityGuid);
            base.Execute();
        }
    }

    public class ChangeStartDateOfProjectRoleAssignmentCommand : ProjectRoleAssignmentCommand
    {
        public DateTime? OriginalStartDate { get; set; }
        public DateTime? StartDate { get; set; }
        public override void Execute()
        {
            var projectroleassignment = ((IProjectRoleAssignmentService)base.CommandProcessor).GetProjectRoleAssignment(this.EntityGuid);
            projectroleassignment.ChangeStartDate(this.StartDate, this.OriginalStartDate);
            base.Execute();
        }
    }

    public class ChangeEndDateOfProjectRoleAssignmentCommand : ProjectRoleAssignmentCommand
    {
        public DateTime? OriginalEndDate { get; set; }
        public DateTime? EndDate { get; set; }
        public override void Execute()
        {
            var projectroleassignment = ((IProjectRoleAssignmentService)base.CommandProcessor).GetProjectRoleAssignment(this.EntityGuid);
            projectroleassignment.ChangeEndDate(this.EndDate, this.OriginalEndDate);
            base.Execute();
        }
    }    
}
