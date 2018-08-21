using System;
using Xunit;
using Altairis.HtmlFilter;

namespace Altairis.HtmlFilter.Test {
    public class SafeHtmlTests {

        [Fact]
        public void RemoveUnsupportedElements() {
            var f = new SafeHtmlFilter();
            var s = f.FilterHtml("<p>test <b>bold</b> <u>underline</u> </p>");
            Assert.Equal("<p>test <b>bold</b> <span>underline</span> </p>", s);
        }

        [Fact]
        public void RemoveUnsupportedAttributes() {
            var f = new SafeHtmlFilter();
            var s = f.FilterHtml("<p><a href=\"http://somewhere\" target=\"_blank\">aaa</a></p>");
            Assert.Equal("<p><a href=\"http://somewhere\">aaa</a></p>", s);
        }

        [Fact]
        public void RemoveUnsupportedHrefValues() {
            var f = new SafeHtmlFilter();
            var s = f.FilterHtml("<p><a href=\"javascript:void();\">aaa</a></p>");
            Assert.Equal("<p><a>aaa</a></p>", s);
        }

        [Fact]
        public void AllowLocalHref() {
            var f = new SafeHtmlFilter();
            var s = f.FilterHtml("<p><a href=\"/local\">aaa</a></p>");
            Assert.Equal("<p><a href=\"/local\">aaa</a></p>", s);
        }

        [Fact]
        public void AllowHashtagHref() {
            var f = new SafeHtmlFilter();
            var s = f.FilterHtml("<p><a href=\"#local\">aaa</a></p>");
            Assert.Equal("<p><a href=\"#local\">aaa</a></p>", s);
        }

    }
}
