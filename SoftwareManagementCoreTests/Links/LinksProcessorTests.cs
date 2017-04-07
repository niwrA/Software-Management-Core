﻿using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;
using LinksShared;

namespace SoftwareManagementCoreTests.Links
{
    public class LinksProcessorTests
    {
        [Fact]
        public void CanGetLinkDetails()
        {
            var sut = new LinkDetailsProcessor();
            var sutResult = sut.ProcessLinkDetails("https://www.google.com");

            Assert.Equal("Google", sutResult.Title);
            Assert.True(sutResult.ContentLength > 0);
            Assert.Contains("text/html", sutResult.MimeType);
        }

        [Fact]
        public void CanGetImageLink()
        {
            var sut = new LinkDetailsProcessor();
            var sutResult = sut.ProcessLinkDetails("http://www.eurogamer.net/articles/digitalfoundry-2014-alien-isolation-face-off");
            
            Assert.NotNull(sutResult.Image);
            Assert.False(string.IsNullOrWhiteSpace(sutResult.Image.Url));
        }

    }
}
