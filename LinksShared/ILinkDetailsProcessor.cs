using cloudscribe.HtmlAgilityPack;

namespace LinksShared
{
    public interface ILinkDetailsProcessor
    {
        ILinkDetails ProcessLinkDetails(string url);
    }
}