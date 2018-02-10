using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager;

namespace ProductsShared
{
  public abstract class ProductCommand : CommandBase, ICommand
  {
    public ProductCommand() : base() { }
    public ProductCommand(ICommandStateRepository repo) : base(repo) { }
    public virtual void Execute() { }
  }

  public class CreateProductCommand : ProductCommand
  {
    public CreateProductCommand() : base() { }
    public CreateProductCommand(ICommandStateRepository repo) : base(repo) { }
    public string Name { get; set; }
    public override void Execute()
    {
      ((IProductService)base.CommandProcessor).CreateProduct(this.EntityGuid, this.Name);
      base.Execute();
    }
  }

  public class RenameProductCommand : ProductCommand
  {
    public string OriginalName { get; set; }
    public string Name { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      product.Rename(this.Name, this.OriginalName);
      base.Execute();
    }
  }

  public class ChangeDescriptionOfProductCommand : ProductCommand
  {
    public string Description { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      product.ChangeDescription(this.Description);
      base.Execute();
    }
  }

  public class ChangeBusinessCaseOfProductCommand : ProductCommand
  {
    public string BusinessCase { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      product.ChangeBusinessCase(this.BusinessCase);
      base.Execute();
    }
  }

  public class DeleteProductCommand : ProductCommand
  {
    public string Name { get; set; }
    public override void Execute()
    {
      ((IProductService)base.CommandProcessor).DeleteProduct(this.EntityGuid);
      base.Execute();
    }
  }

  public class AddIssueToProductCommand : ProductCommand
  {
    public string Name { get; set; }
    public Guid ProductIssueGuid { get; set; }
    public Guid FirstVersionGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      product.AddIssue(ProductIssueGuid, Name, FirstVersionGuid);
      base.Execute();
    }

  }

  public class AddProductVersionCommand : ProductCommand
  {
    public string Name { get; set; }
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Revision { get; set; }
    public int Build { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityRootGuid);
      product.AddVersion(this.EntityGuid, Name, Major, Minor, Revision, Build);
      base.Execute();
    }
  }

  public class AddFeatureToProductCommand : ProductCommand
  {
    public string Name { get; set; }
    public Guid ProductFeatureGuid { get; set; }
    public Guid FirstVersionGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      product.AddFeature(ProductFeatureGuid, Name, FirstVersionGuid);
      base.Execute();
    }
  }
  public class RequestFeatureForProductCommand : ProductCommand
  {
    public string Name { get; set; }
    public Guid ProductFeatureGuid { get; set; }
    public Guid RequestedForVersionGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      product.RequestFeature(ProductFeatureGuid, Name, RequestedForVersionGuid);
      base.Execute();
    }

  }

  public class RenameProductIssueCommand : ProductCommand
  {
    public string OriginalName { get; set; }
    public string Name { get; set; }
    public Guid ProductGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.ProductGuid);
      IProductIssue issue = product.GetIssue(this.EntityGuid);
      issue.Rename(this.Name, this.OriginalName);
      base.Execute();
    }
  }
  public class ChangeDescriptionOfProductIssueCommand : ProductCommand
  {
    public string Description { get; set; }
    public Guid ProductGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.ProductGuid);
      IProductIssue issue = product.GetIssue(this.EntityGuid);
      issue.ChangeDescription(this.Description);
      base.Execute();
    }
  }
  public class RemoveFeatureFromProductCommand : ProductCommand
  {
    public Guid ProductFeatureGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      product.DeleteFeature(ProductFeatureGuid);
      base.Execute();
    }
  }
  public class RemoveVersionFromProductCommand : ProductCommand
  {
    public Guid ProductVersionGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      product.DeleteVersion(ProductVersionGuid);
      base.Execute();
    }
  }
  public class RemoveIssueFromProductCommand : ProductCommand
  {
    public Guid ProductIssueGuid { get; set; }
    public override void Execute()
    {
      var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
      product.DeleteIssue(ProductIssueGuid);
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
      product.AddConfigOption(this.FeatureGuid, ConfigGuid, Name, null);
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
