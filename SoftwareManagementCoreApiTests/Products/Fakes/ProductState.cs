using ProductsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreApiTests.Fakes
{
    public class ProductState : IProductState
    {
        public ProductState()
        {
            this.Description = "Product description";
            this.BusinessCase = "Product businesscase";
            this.CreatedOn = DateTime.Now;
            this.Guid = Guid.NewGuid();
            this.Name = "Product name";
        }

        public string Description { get; set; }
        public string BusinessCase { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
