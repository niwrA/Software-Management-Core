using ProductsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreApiTests.Fakes
{
    public class ProductVersionState : IProductVersionState
    {
        public ProductVersionState()
        {
            Name = "1.1.1.1";
            Major = 1;
            Minor = 1;
            Revision = 1;
            Build = 1;
            ProductGuid = Guid.NewGuid();
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public int Build { get; set; }
        public Guid ProductGuid { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    public class ProductFeatureState : IProductFeatureState
    {
        public Guid ProductGuid { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    public class ProductState : IProductState
    {
        public ProductState()
        {
            this.Description = "Product description";
            this.BusinessCase = "Product businesscase";
            this.CreatedOn = DateTime.Now;
            this.Guid = Guid.NewGuid();
            this.Name = "Product name";
            this.ProductVersionStates = new List<IProductVersionState> { new ProductVersionState() };
            this.ProductFeatureStates = new List<IProductFeatureState> { new ProductFeatureState() };
        }

        public string Description { get; set; }
        public string BusinessCase { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public ICollection<IProductVersionState> ProductVersionStates { get; set; }
        public ICollection<IProductFeatureState> ProductFeatureStates { get; set; }
    }
}
