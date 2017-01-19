using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace ProductsShared
{
    public abstract class ProductCommand : CommandBase
    {
        internal IProducts _products;
        public IProducts CommandProcessor { get; set; }
    }

    public class CreateProductCommand : ProductCommand
    {
        public string Name { get; set; }
        public override void Execute()
        {
            _products.CreateProduct(this.EntityGuid);
            base.Execute();
        }
    }

    public class RenameProductCommand : ProductCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var product = _products.GetProduct(this.EntityGuid);
            product.Rename(this.Name);
            base.Execute();
        }
    }
}
