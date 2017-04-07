using LinksShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreTests.Links.Fakes
{
    public class LinkState : ILinkState
    {
        public LinkState()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
            Guid = Guid.NewGuid();
            Name = "Fake Link";
            EntityGuid = Guid.NewGuid();
            Url = "Fake Url";
            ForGuid = Guid.NewGuid();
            SiteName = "Fake SiteName";
        }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public Guid EntityGuid { get; set; }
        public string Url { get; set; }
        public Guid ForGuid { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string SiteName { get; set; }
    }
}


