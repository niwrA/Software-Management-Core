using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace ProductsShared
{
    public abstract class ProductCommand : CommandBase
    {
        public ProductCommand() : base() { }
        public ProductCommand(ICommandStateRepository repo) : base(repo) { }
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
    public class AddVersionToProductCommand : ProductCommand
    {
        public string Name { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public int Build { get; set; }
        public Guid ProductVersionGuid { get; set; }
        public override void Execute()
        {
            var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
            product.AddVersion(ProductVersionGuid, Name, Major, Minor, Revision, Build);
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
}
