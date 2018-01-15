using CommandsShared;
using System;
using ProductInstallationsShared;
using System.Collections.Generic;
using System.Text;

namespace ProductInstallationsShared
{
  public abstract class ProductInstallationCommand : CommandBase
  {
    public ProductInstallationCommand() : base() { }
    public ProductInstallationCommand(ICommandStateRepository repo) : base(repo) { }
  }

  public class CreateProductInstallationCommand : ProductInstallationCommand
  {
    public Guid CompanyGuid { get; set; }
    public Guid ProductGuid { get; set; }
    public Guid? CompanyEnvironmentGuid { get; set; }
    public Guid? ProductVersionGuid{ get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public override void Execute()
    {
      ((IProductInstallationService)base.CommandProcessor).CreateProductInstallation(this.EntityGuid, this.CompanyGuid, this.ProductGuid, this.CompanyEnvironmentGuid, this.ProductVersionGuid, this.StartDate, this.EndDate);
      base.Execute();
    }
  }

  public class DeleteProductInstallationCommand : ProductInstallationCommand
  {
    public override void Execute()
    {
      ((IProductInstallationService)base.CommandProcessor).DeleteProductInstallation(this.EntityGuid);
      base.Execute();
    }
  }

  public class ChangeStartDateOfProductInstallationCommand : ProductInstallationCommand
  {
    public DateTime? OriginalStartDate { get; set; }
    public DateTime? StartDate { get; set; }
    public override void Execute()
    {
      var ProductInstallation = ((IProductInstallationService)base.CommandProcessor).GetProductInstallation(this.EntityGuid);
      ProductInstallation.ChangeStartDate(this.StartDate, this.OriginalStartDate);
      base.Execute();
    }
  }

  public class ChangeEndDateOfProductInstallationCommand : ProductInstallationCommand
  {
    public DateTime? OriginalEndDate { get; set; }
    public DateTime? EndDate { get; set; }
    public override void Execute()
    {
      var ProductInstallation = ((IProductInstallationService)base.CommandProcessor).GetProductInstallation(this.EntityGuid);
      ProductInstallation.ChangeEndDate(this.EndDate, this.OriginalEndDate);
      base.Execute();
    }
  }

  public class ChangeExternalIdOfProductInstallationCommand : ProductInstallationCommand
  {
    public string OriginalExternalId { get; set; }
    public string ExternalId { get; set; }
    public override void Execute()
    {
      var ProductInstallation = ((IProductInstallationService)base.CommandProcessor).GetProductInstallation(this.EntityGuid);
      ProductInstallation.ChangeExternalId(this.ExternalId, this.OriginalExternalId);
      base.Execute();
    }
  }
}
