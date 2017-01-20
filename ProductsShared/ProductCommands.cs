using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace ProductsShared
{
    public abstract class ProductCommand : CommandBase
    {
        public ProductCommand() : base() { }
        public ProductCommand(ICommandRepository repo) : base(repo) { }
    }

    public class CreateProductCommand : ProductCommand
    {
        public CreateProductCommand() : base() { }
        public CreateProductCommand(ICommandRepository repo) : base(repo) { }
        public string Name { get; set; }
        public override void Execute()
        {
            ((IProductService)base.CommandProcessor).CreateProduct(this.EntityGuid);
            base.Execute();
        }
    }

    public class RenameProductCommand : ProductCommand
    {
        public RenameProductCommand() : base() { }
        public RenameProductCommand(ICommandRepository repo) : base(repo) { }
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var product = ((IProductService)base.CommandProcessor).GetProduct(this.EntityGuid);
            product.Rename(this.Name);
            base.Execute();
        }
    }
}
