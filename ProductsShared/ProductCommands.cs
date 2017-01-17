﻿using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace ProductsShared
{
    public abstract class ProductCommand : CommandBase
    {
    }

    public class CreateProductCommand : ProductCommand
    {
        public string Name { get; set; }
    }

    public class RenameProductCommand : ProductCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var products = new Products(this.EntityRepository as IProductStateRepository);
            var product = products.GetProduct(this.EntityGuid);
            product.Rename(this.Name);

            base.Execute();
        }
    }

}