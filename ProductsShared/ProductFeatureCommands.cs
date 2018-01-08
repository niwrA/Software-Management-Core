using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsShared
{
  public class RenameProductFeatureCommand : ProductCommand
  {
    public string OriginalName { get; set; }
    public string Name { get; set; }
    public Guid ProductGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.ProductGuid);
      IProductFeature feature = product.GetFeature(this.EntityGuid);
      feature.Rename(this.Name, this.OriginalName);
      base.Execute();
    }
  }
  public class ChangeDescriptionOfProductFeatureCommand : ProductCommand
  {
    public string Description { get; set; }
    public Guid ProductGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.ProductGuid);
      IProductFeature feature = product.GetFeature(this.EntityGuid);
      feature.ChangeDescription(this.Description);
      base.Execute();
    }
  }
  public class AddConfigOptionToProductFeatureCommand : ProductCommand
  {
    public string Name { get; set; }
    public Guid FeatureGuid { get; set; }
    public Guid ConfigGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      var feature = product.GetFeature(this.FeatureGuid);
      feature.AddConfigOption(ConfigGuid, Name);
      base.Execute();
    }
  }
  public class RemoveConfigOptionFromProductFeatureCommand : ProductCommand
  {
    public Guid FeatureGuid { get; set; }
    public Guid ConfigGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      var feature = product.GetFeature(this.FeatureGuid);
      feature.DeleteConfigOption(ConfigGuid);
      base.Execute();
    }
  }

}
