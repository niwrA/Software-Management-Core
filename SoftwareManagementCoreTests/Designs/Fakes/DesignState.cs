using DesignsShared;
using System;
using System.Collections.Generic;

namespace SoftwareManagementCoreTests.Fakes
{
    public class DesignState : IDesignState
    {
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
