using CompaniesShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreTests.Fakes
{
  public class CompanyState : ICompanyState
  {
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public ICollection<ICompanyRoleState> CompanyRoleStates { get; set; }
    public ICollection<ICompanyEnvironmentState> CompanyEnvironmentStates { get; set; }
    public string ExternalId { get; set; }
    public string Code { get; set; }
  }
}
