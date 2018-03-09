using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsShared
{
  public class AddProductIssueCommand : ProductCommand
  {
    public string Name { get; set; }
    public Guid FirstVersionGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityRootGuid);
      product.AddIssue(EntityGuid, Name, FirstVersionGuid);
      base.Execute();
    }
  }
  public class RemoveProductIssueCommand : ProductCommand
  {
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityRootGuid);
      product.DeleteIssue(EntityGuid);
      base.Execute();
    }
  }
  public class RenameProductIssueCommand : ProductCommand
  {
    public string OriginalName { get; set; }
    public string Name { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityRootGuid);
      IProductIssue issue = product.GetIssue(this.EntityGuid);
      issue.Rename(this.Name, this.OriginalName);
      base.Execute();
    }
  }

  public class ChangeDescriptionOfProductIssueCommand : ProductCommand
  {
    public string Description { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityRootGuid);
      IProductIssue issue = product.GetIssue(this.EntityGuid);
      issue.ChangeDescription(this.Description);
      base.Execute();
    }
  }
  public class ResolveProductIssueCommand : ProductCommand
  {
    public Guid ResolvedVersionGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityRootGuid);
      IProductIssue issue = product.GetIssue(this.EntityGuid);
      issue.Resolve(this.ResolvedVersionGuid);
      base.Execute();
    }
  }

}
