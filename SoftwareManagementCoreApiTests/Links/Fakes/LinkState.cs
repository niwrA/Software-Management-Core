using LinksShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreApiTests.Fakes
{
    public class LinkState : ILinkState
    {
        public LinkState()
        {
            Name = "fake link Name";
            Guid = Guid.NewGuid();
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
            EntityGuid = Guid.NewGuid();
            Url = "fake link Url";
            ForGuid = Guid.NewGuid();
            Description = "fake link Description";
            ImageUrl = "fake link ImageUrl";
            SiteName = "fake SiteName";
        }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid EntityGuid { get; set; }
        public string Url { get; set; }
        public Guid ForGuid { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string SiteName { get; set; }
    }
}