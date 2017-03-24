using ProductsShared;
using System;
using System.Collections.Generic;

namespace SoftwareManagementCoreTests.Fakes
{
    public class ProductState : IProductState
    {
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BusinessCase { get; set; }
        public ICollection<IProductVersionState> ProductVersionStates { get; set; }
    }
}
