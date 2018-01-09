﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsShared
{
  public abstract class ProductFeatureCommand : ProductCommand
  {
    public Guid ProductGuid { get; set; }
  }
  public class RenameProductFeatureCommand : ProductFeatureCommand
  {
    public string OriginalName { get; set; }
    public string Name { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.ProductGuid);
      IProductFeature feature = product.GetFeature(this.EntityGuid);
      feature.Rename(this.Name, this.OriginalName);
      base.Execute();
    }
  }
  public class ChangeDescriptionOfProductFeatureCommand : ProductFeatureCommand
  {
    public string Description { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.ProductGuid);
      IProductFeature feature = product.GetFeature(this.EntityGuid);
      feature.ChangeDescription(this.Description);
      base.Execute();
    }
  }
  public class AddConfigOptionToProductCommand : ProductCommand
  {
    public string Name { get; set; }
    public Guid ConfigGuid { get; set; }
    public Guid? FeatureGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      product.AddConfigOption(this.FeatureGuid, ConfigGuid, Name);
      base.Execute();
    }
  }
  public class RemoveConfigOptionFromProductCommand : ProductCommand
  {
    public Guid ConfigGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      product.DeleteConfigOption(ConfigGuid);
      base.Execute();
    }
  }

}
