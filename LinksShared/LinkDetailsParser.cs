using cloudscribe.HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net;

namespace LinksShared
{
    // todo: this needs to move out to a separate project and eventually probably a nuget package
    public class LinkDetailsProcessor : ILinkDetailsProcessor
    {
        // source https://webcmd.wordpress.com/2011/02/16/c-webservice-for-getting-link-details-like-facebook/
        public ILinkDetails ProcessLinkDetails(string url)
        {
            ILinkDetails linkDetails = new LinkDetails();
            //http://htmlagilitypack.codeplex.com/

            linkDetails.Title = url;
            linkDetails.Url = url;
            linkDetails.MimeType = "";
            linkDetails = GetHeaders(linkDetails.Url, linkDetails);

            if (linkDetails.MimeType.ToLower().Contains("text/html"))
            {
                linkDetails.LinkType = 1;
                HtmlDocument htmlDocument = new HtmlDocument();
                var webClient = new HttpClient();

                string download = webClient.GetStringAsync(linkDetails.Url).Result;

                htmlDocument.LoadHtml(download);
                HtmlNode htmlNode = htmlDocument.DocumentNode.SelectSingleNode("html/head");

                linkDetails = GetStandardInfo(linkDetails, htmlNode);

                linkDetails = GetOpenGraphInfo(linkDetails, htmlNode);

                if (linkDetails.Image == null)
                {
                    linkDetails = GuessImage(htmlDocument, linkDetails);
                }
            }

            if (linkDetails.MimeType.ToLower().Contains("image/"))
            {
                linkDetails.LinkType = 2;
                linkDetails.Image = new ImageLink(linkDetails.Image.Url, linkDetails.Url);
            }

            return linkDetails;
        }
        //get info off of basic meta tags
        public ILinkDetails GetStandardInfo(ILinkDetails linkDetails, HtmlNode head)
        {
            foreach (HtmlNode headNode in head.ChildNodes)
            {
                switch (headNode.Name.ToLower())
                {
                    case "link": break;
                    case "title":
                        linkDetails.Title = WebUtility.HtmlDecode(headNode.InnerText);
                        break;
                    case "meta":
                        if (headNode.Attributes["name"] != null && headNode.Attributes["content"] != null)
                        {
                            switch (headNode.Attributes["name"].Value.ToLower())
                            {
                                case "description":
                                    linkDetails.Description = WebUtility.HtmlDecode(headNode.Attributes["content"].Value);
                                    break;
                            }
                        }
                        break;
                }

            }
            // look for apple touch icon in header
            HtmlNode imageNode = head.SelectSingleNode("link[@rel='apple-touch-icon']");
            if (imageNode != null)
            {
                if (imageNode.Attributes["href"] != null) { linkDetails.Image = new ImageLink(imageNode.Attributes["href"].Value, linkDetails.Url); }
                if (imageNode.Attributes["src"] != null) { linkDetails.Image = new ImageLink(imageNode.Attributes["src"].Value, linkDetails.Url); }
            }
            //look for link image in header
            imageNode = head.SelectSingleNode("link[@rel='image_src']");
            if (imageNode != null)
            {
                if (imageNode.Attributes["href"] != null) { linkDetails.Image = new ImageLink(imageNode.Attributes["href"].Value, linkDetails.Url); }
                if (imageNode.Attributes["src"] != null) { linkDetails.Image = new ImageLink(imageNode.Attributes["src"].Value, linkDetails.Url); }
            }

            return linkDetails;
        }

        //get info using open graph
        public ILinkDetails GetOpenGraphInfo(ILinkDetails linkDetails, HtmlNode head)
        {
            foreach (HtmlNode headNode in head.ChildNodes)
            {
                switch (headNode.Name.ToLower())
                {
                    case "link": break;

                    case "meta":
                        if (headNode.Attributes["property"] != null && headNode.Attributes["content"] != null)
                        {
                            switch (headNode.Attributes["property"].Value.ToLower())
                            {
                                case "og:title":
                                    linkDetails.Title = WebUtility.HtmlDecode(headNode.Attributes["content"].Value);
                                    break;
                                case "og:type":
                                    linkDetails.Type = headNode.Attributes["content"].Value;
                                    break;
                                case "og:url":
                                    linkDetails.Url = headNode.Attributes["content"].Value;
                                    break;
                                case "og:image":
                                    linkDetails.Image = new ImageLink(headNode.Attributes["content"].Value, linkDetails.Url);
                                    break;
                                case "og:site_name":
                                    linkDetails.SiteName = WebUtility.HtmlDecode(headNode.Attributes["content"].Value);
                                    break;
                                case "og:description":
                                    linkDetails.Description = WebUtility.HtmlDecode(headNode.Attributes["content"].Value);
                                    break;
                                case "og:email":
                                    linkDetails.Email = WebUtility.HtmlDecode(headNode.Attributes["content"].Value);
                                    break;
                                case "og:phone_number":
                                    linkDetails.PhoneNumber = WebUtility.HtmlDecode(headNode.Attributes["content"].Value);
                                    break;
                                case "og:fax_number":
                                    linkDetails.FaxNumber = WebUtility.HtmlDecode(headNode.Attributes["content"].Value);
                                    break;

                            }
                        }
                        break;
                }

            }
            return linkDetails;
        }

        //try to guess at the images
        private ILinkDetails GuessImage(HtmlDocument htmlDocument, ILinkDetails linkDetails)
        {
            HtmlNodeCollection imageNodes = htmlDocument.DocumentNode.SelectNodes("//img");
            string h1 = string.Empty;
            HtmlNode h1Node = htmlDocument.DocumentNode.SelectSingleNode("//h1");
            if (h1Node != null)
            {
                h1 = h1Node.InnerText;
            }
            int bestScore = -1;
            if (imageNodes != null)
            {
                foreach (HtmlNode imageNode in imageNodes)
                {
                    if (imageNode != null && imageNode.Attributes["src"] != null && imageNode.Attributes["alt"] != null)
                    {
                        ImageLink imageLink = new ImageLink(imageNode.Attributes["src"].Value, linkDetails.Url);
                        if (!(imageLink.Width > 0 && imageLink.Width < 50)) //if we don't have a width go with it but if we know width is less than 50 don't use it
                        {
                            int myScore = ScoreImage(imageNode.Attributes["alt"].Value, linkDetails.Title);
                            myScore += ScoreImage(imageNode.Attributes["alt"].Value, h1);

                            if (myScore > bestScore)
                            {
                                linkDetails.Image = imageLink;
                                bestScore = myScore;
                            }

                            if (!linkDetails.Images.Contains(imageLink)) { linkDetails.Images.Add(imageLink); }
                        }
                    }
                }
            }

            return linkDetails;
        }
        //score the image based on matches in comparing alt to title and h1 tag
        private int ScoreImage(string text, string compare)
        {
            text = text.Replace("\r\n", string.Empty).Replace("\t", string.Empty);
            compare = compare.Replace("\r\n", string.Empty).Replace("\t", string.Empty);
            int score = 0;
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(compare))
            {
                string[] c = compare.Split(' ');

                foreach (string test in c)
                {
                    if (text.Contains(test)) { score++; }
                }
            }
            return score;
        }

        public ILinkDetails GetHeaders(string link, ILinkDetails linkDetails)
        {
            var wc = new HttpClient();
            var result = wc.GetAsync(link).Result;
            if (result.IsSuccessStatusCode)
            {
                var content = result.Content;
                linkDetails.ContentLength = Convert.ToInt64(content.Headers.ContentLength);
                linkDetails.MimeType = content.Headers.ContentType.ToString();
            }

            return linkDetails;
        }

        public class LinkDetails : ILinkDetails
        {
            public LinkDetails()
            {
                Images = new List<IImageLink>();
            }
            public string Title { get; set; }
            public string Url { get; set; }
            public string Type { get; set; }
            public IImageLink Image { get; set; }
            public List<IImageLink> Images { get; set; }
            public string SiteName { get; set; }
            public string Description { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string FaxNumber { get; set; }
            public Int64 ContentLength { get; set; }
            public string MimeType { get; set; }
            public int LinkType { get; set; } // 0=bad, 1=html, 2=image

        }

        public class ImageLink : IImageLink
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public string Url { get; set; }

            public ImageLink()
            {
            }

            public ImageLink(string url, string siteUrl)
            {
                var imgUrl = FullyQualifiedImage(url, siteUrl);
                var uri = new Uri(imgUrl);
                var size = ImageUtilities.GetWebDimensions(uri);
                this.Url = imgUrl;
                //                this.Height = size.;
            }
            //get the image url if it beings with / instead of // if it's a relative url I'm too lazy to make it work
            private string FullyQualifiedImage(string imageUrl, string siteUrl)
            {
                if (imageUrl.Contains("http:") || imageUrl.Contains("https:"))
                {
                    return imageUrl;
                }

                if (imageUrl.IndexOf("//") == 0)
                {
                    return "http:" + imageUrl;
                }
                try
                {
                    string baseurl = siteUrl.Replace("http://", string.Empty).Replace("https://", string.Empty);
                    baseurl = baseurl.Split('/')[0];
                    return string.Format("http://{0}/{1}", baseurl, imageUrl);

                }
                catch { }

                return imageUrl;

            }
        }
    }

}