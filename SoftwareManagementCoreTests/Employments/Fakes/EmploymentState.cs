using EmploymentsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreTests.Fakes
{
    public class EmploymentState : IEmploymentState
    {
        public EmploymentState()
        {
            Guid = Guid.NewGuid();
            ContactGuid = Guid.NewGuid();
            CompanyRoleGuid = Guid.NewGuid();
            StartDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date.AddYears(1);
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;

        }
        public Guid Guid { get; set; }
        public Guid ContactGuid { get; set; }
        public Guid CompanyRoleGuid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
