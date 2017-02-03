using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace EmploymentsShared
{
    public abstract class EmploymentCommand : CommandBase
    {
        public EmploymentCommand() : base() { }
        public EmploymentCommand(ICommandStateRepository repo) : base(repo) { }
    }

    public class CreateEmploymentCommand : EmploymentCommand
    {
        public Guid ContactGuid { get; set; }
        public Guid CompanyRoleGuid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public override void Execute()
        {
            ((IEmploymentService)base.CommandProcessor).CreateEmployment(this.EntityGuid, this.ContactGuid, this.CompanyRoleGuid, this.StartDate, this.EndDate);
            base.Execute();
        }
    }
    
    public class DeleteEmploymentCommand : EmploymentCommand
    {
        public override void Execute()
        {
            ((IEmploymentService)base.CommandProcessor).DeleteEmployment(this.EntityGuid);
            base.Execute();
        }
    }

    public class ChangeStartDateOfEmploymentCommand : EmploymentCommand
    {
        public DateTime? OriginalStartDate { get; set; }
        public DateTime? StartDate { get; set; }
        public override void Execute()
        {
            var employment = ((IEmploymentService)base.CommandProcessor).GetEmployment(this.EntityGuid);
            employment.ChangeStartDate(this.StartDate, this.OriginalStartDate);
            base.Execute();
        }
    }

    public class ChangeEndDateOfEmploymentCommand : EmploymentCommand
    {
        public DateTime? OriginalEndDate { get; set; }
        public DateTime? EndDate { get; set; }
        public override void Execute()
        {
            var employment = ((IEmploymentService)base.CommandProcessor).GetEmployment(this.EntityGuid);
            employment.ChangeEndDate(this.EndDate, this.OriginalEndDate);
            base.Execute();
        }
    }    
}
