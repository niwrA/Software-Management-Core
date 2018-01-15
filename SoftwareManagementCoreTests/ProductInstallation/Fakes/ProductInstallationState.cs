using ProductInstallationsShared;
using System;
using System.Collections.Generic;
using System.Text;
using ContactsShared;

namespace SoftwareManagementCoreTests.Fakes
{
  public class ProductInstallationState : IProductInstallationState
  {
    public ProductInstallationState()
    {
      Guid = Guid.NewGuid();
      CompanyGuid = Guid.NewGuid();
      ProductGuid = Guid.NewGuid();
      CompanyEnvironmentGuid = Guid.NewGuid();
      ProductVersionGuid = Guid.NewGuid();
      StartDate = DateTime.Now.Date;
      EndDate = DateTime.Now.Date.AddYears(1);
      CreatedOn = DateTime.Now;
      UpdatedOn = DateTime.Now;
      ExternalId = "123abc";
    }
    public Guid Guid { get; set; }
    public Guid CompanyGuid { get; set; }
    public Guid ProductGuid { get; set; }
    public Guid? CompanyEnvironmentGuid { get; set; }
    public Guid? ProductVersionGuid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ExternalId { get; set; }
  }
}
