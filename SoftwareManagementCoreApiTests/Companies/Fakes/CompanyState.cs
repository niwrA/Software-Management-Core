using CompaniesShared;
using System;
using System.Collections.Generic;

namespace SoftwareManagementCoreApiTests.Fakes
{
    public class CompanyState : ICompanyState
    {
        public CompanyState()
        {
            Guid = Guid.NewGuid();
            Name = "Company Name";
            CompanyRoleStates = new List<ICompanyRoleState>();
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<ICompanyRoleState> CompanyRoleStates { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
