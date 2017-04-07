using System.Collections.Generic;

namespace LinksShared
{
    public interface ILinkDetails
    {
        long ContentLength { get; set; }
        string Description { get; set; }
        string Email { get; set; }
        string FaxNumber { get; set; }
        IImageLink Image { get; set; }
        List<IImageLink> Images { get; set; }
        int LinkType { get; set; }
        string MimeType { get; set; }
        string PhoneNumber { get; set; }
        string SiteName { get; set; }
        string Title { get; set; }
        string Type { get; set; }
        string Url { get; set; }
    }
}