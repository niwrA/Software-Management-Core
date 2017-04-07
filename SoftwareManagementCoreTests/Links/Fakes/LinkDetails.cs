using LinksShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreTests.Links.Fakes
{
    public class ImageLink : IImageLink
    {
        public ImageLink()
        {
            Url = "Fake image url";
        }
        public int Height { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
    }
    public class LinkDetails : ILinkDetails
    {
        public LinkDetails()
        {
            Title = "Fake title";
            Description = "Fake description";
            SiteName = "Fake sitename";
            Url = "Fake url";
            MimeType = "text/html";
            Image = new ImageLink();
        }
        public long ContentLength { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string FaxNumber { get; set; }
        public IImageLink Image { get; set; }
        public List<IImageLink> Images { get; set; }
        public int LinkType { get; set; }
        public string MimeType { get; set; }
        public string PhoneNumber { get; set; }
        public string SiteName { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
    }
}
