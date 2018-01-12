using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsShared
{
  public abstract class ProductConfigOptionCommand : ProductCommand
  {
    public Guid ProductGuid { get; set; }
  }
  public class RenameProductConfigOptionCommand : ProductConfigOptionCommand
  {
    public string OriginalName { get; set; }
    public string Name { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.ProductGuid);
      IProductConfigOption configOption = product.GetConfigOption(this.EntityGuid);
      configOption.Rename(this.Name, this.OriginalName);
      base.Execute();
    }
  }
  public class ChangeDefaultValueForProductConfigOptionCommand : ProductConfigOptionCommand
  {
    public string OriginalDefaultValue { get; set; }
    public string DefaultValue { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.ProductGuid);
      IProductConfigOption configOption = product.GetConfigOption(this.EntityGuid);
      configOption.ChangeDefaultValue(this.DefaultValue, this.OriginalDefaultValue);
      base.Execute();
    }
  }

  public class ChangeDescriptionOfProductConfigOptionCommand : ProductConfigOptionCommand
  {
    public string Description { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.ProductGuid);
      IProductConfigOption configOption = product.GetConfigOption(this.EntityGuid);
      configOption.ChangeDescription(this.Description);
      base.Execute();
    }
  }
  public class AddChildToProductConfigOptionCommand : ProductConfigOptionCommand
  {
    public string Name { get; set; }
    public Guid ParentGuid { get; set; }
    public Guid? FeatureGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.ProductGuid);
      product.AddConfigOption(this.FeatureGuid, this.EntityGuid, Name, this.ParentGuid);
      base.Execute();
    }
  }
  public class RemoveChildFromProductConfigOptionCommand : ProductConfigOptionCommand
  {
    public Guid ChildGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.ProductGuid);
      product.DeleteConfigOption(ChildGuid);
      base.Execute();
    }
  }

}
