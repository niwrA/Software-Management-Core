using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager;

namespace ProjectsShared
{
    public abstract class ProjectCommand : CommandBase, ICommand
    {
        public ProjectCommand() : base() { }
        public ProjectCommand(ICommandStateRepository repo) : base(repo) { }
        public virtual void Execute() { }
  }

  public class DeleteProjectCommand: ProjectCommand
    {
        public override void Execute()
        {
            ((IProjectService)base.CommandProcessor).DeleteProject(this.EntityGuid);
            base.Execute();
        }

    }

    public class CreateProjectCommand : ProjectCommand
    {
        public string Name { get; set; }
        public override void Execute()
        {
            ((IProjectService)base.CommandProcessor).CreateProject(this.EntityGuid, this.Name);
            base.Execute();
        }
    }

    public class RenameProjectCommand : ProjectCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var product = ((IProjectService)base.CommandProcessor).GetProject(this.EntityGuid);
            product.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }

    public class ChangeStartDateOfProjectCommand : ProjectCommand
    {
        public DateTime? OriginalStartDate { get; set; }
        public DateTime? StartDate { get; set; }
        public override void Execute()
        {
            var project = ((IProjectService)base.CommandProcessor).GetProject(this.EntityGuid);
            project.ChangeStartDate(this.StartDate, this.OriginalStartDate);
            base.Execute();
        }
    }

    public class ChangeEndDateOfProjectCommand : ProjectCommand
    {
        public DateTime? OriginalEndDate { get; set; }
        public DateTime? EndDate { get; set; }
        public override void Execute()
        {
            var project = ((IProjectService)base.CommandProcessor).GetProject(this.EntityGuid);
            project.ChangeEndDate(this.EndDate, this.OriginalEndDate);
            base.Execute();
        }
    }

    public class AddRoleToProjectCommand : ProjectCommand
    {
        public string RoleName { get; set; }
        public Guid RoleGuid { get; set; }
        public override void Execute()
        {
            var project = ((IProjectService)base.CommandProcessor).GetProject(this.EntityGuid);
            project.AddRoleToProject(this.RoleGuid, this.RoleName);
            base.Execute();
        }
    }
    public class RemoveRoleFromProjectCommand : ProjectCommand
    {
        public Guid RoleGuid { get; set; }
        public override void Execute()
        {
            var project = ((IProjectService)base.CommandProcessor).GetProject(this.EntityGuid);
            project.RemoveRoleFromProject(this.RoleGuid);
            base.Execute();
        }
    }
}
