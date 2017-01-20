using ProductsShared;
using System;

namespace SoftwareManagementCoreTests.Fakes
{
    public class ProductState : IProductState
    {
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
    }
}
