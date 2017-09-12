using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Newsify6.ContentReader
{
    class ContentPresenter
    {
        public string cssfilename { get; set; }
        public HtmlNode GetBBCMainBodyNews(HtmlDocument Webpage)
        {
            var findclassesnews = Webpage.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("story-body__inner"));
            var htmlnews = findclassesnews.FirstOrDefault();
            return htmlnews;
        }
        public HtmlNode GetBBCMainBodySport(HtmlDocument Webpage)
        {
            var findclassessport = Webpage.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("id") && d.Attributes["id"].Value.Equals("story-body"));
            var htmlsport = findclassessport.FirstOrDefault();
            return htmlsport;

        }
        public String StripHTMLBBC(HtmlNode WebContent)
        {
            //general
            WebContent.Descendants().Where(n => n.Name == "script").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "link").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "style").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "iframe").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "figcaption").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "aside").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "img").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "figure").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("sp-media-asset")).ToList().ForEach(n => n.Remove());

            //remove video playback
            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("media-player-wrapper")).ToList().ForEach(n => n.Remove());


            //Remove Copyright stuff
            WebContent.Descendants().Where(n => n.Name == "span").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("off-screen")).ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "span").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("story-image-copyright")).ToList().ForEach(n => n.Remove());

            //Make First Paragraph bold
            var replacefirstpara = WebContent.Descendants().Where(n => n.Name == "p").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("story-body__introduction")).FirstOrDefault();
            string value = replacefirstpara.InnerHtml;
            HtmlNode lbl = HtmlNode.CreateNode("lb1");
            lbl.InnerHtml = "<strong>" + value + "</strong>";
            replacefirstpara.ParentNode.ReplaceChild(lbl, replacefirstpara);

            //clean js-delayed-image-load
            foreach (var div in WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("js-delayed-image-load")).ToList())
            {
                div.Attributes.Remove("data-width");
                div.Attributes.Remove("data-height");
                div.Attributes.Remove("data-alt");
                div.Attributes.Remove("class");

                string replaceval = div.Attributes["data-src"].Value;

                replaceval = Regex.Replace(replaceval, "http://ichef.bbci.co.uk/news/320/", "http://ichef.bbci.co.uk/news/624/");
                replaceval = Regex.Replace(replaceval, "http://ichef-1.bbci.co.uk/news/320/", "http://ichef-1.bbci.co.uk/news/624/");


                HtmlNode lb2 = HtmlNode.CreateNode("lb2");
                lb2.InnerHtml = "<img src=\"" + replaceval + "\"/>";
                div.ParentNode.ReplaceChild(lb2, div);


            }

            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("ns_outer_wrapper")).ToList().ForEach(n => n.Remove());//bbc only
            WebContent.Descendants().Where(n => n.Name == "a").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("story-body__link")).ToList().ForEach(n => n.Remove());//bbc only

            string HTMLCode = WebContent.InnerHtml;

            HTMLCode = Regex.Replace(HTMLCode, @"\t|\n|\r", "");
            HTMLCode = Regex.Replace(HTMLCode, "</?(a|A).*?>", ""); //remove hyperlinks
            HTMLCode = Regex.Replace(HTMLCode, "<p.*?>", "<p>");
            HTMLCode = Regex.Replace(HTMLCode, "<div.*?>", "<div>");

            HTMLCode = Regex.Replace(HTMLCode, "<figure.*?>", "<figure>");
            HTMLCode = Regex.Replace(HTMLCode, "<table.*?>", "<table style=\"margin-bottom:40px;\">");

            HTMLCode = Regex.Replace(HTMLCode, "<figure> *<\\/figure>", string.Empty);

            HTMLCode = Regex.Replace(HTMLCode, "<span.*?>", "<span>");
            HTMLCode = Regex.Replace(HTMLCode, "</span>", "</span>");

            HTMLCode = Regex.Replace(HTMLCode, "<h2.*?>", "<h2>");
            HTMLCode = Regex.Replace(HTMLCode, "<ul.*?>", "<ul>");
            HTMLCode = Regex.Replace(HTMLCode, "<li.*?>", "<li>");
            HTMLCode = Regex.Replace(HTMLCode, "<blockquote.*?>", "<blockquote>");

            HTMLCode = Regex.Replace(HTMLCode, "<li> *<\\/li>", string.Empty);

            return HTMLCode;

        }
        public HtmlNode GetGuardianMainBodyNews(HtmlDocument Webpage)
        {
            var findclassesnews = Webpage.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("itemprop") && d.Attributes["itemprop"].Value.Equals("articleBody"));
            var htmlnews = findclassesnews.FirstOrDefault();

            if (htmlnews == null)
            {
                findclassesnews = Webpage.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("itemprop") && d.Attributes["itemprop"].Value.Equals("reviewBody"));
                htmlnews = findclassesnews.FirstOrDefault();

            }
            return htmlnews;
        }
        public String StripHTMLGuardian(HtmlNode WebContent)
        {
            WebContent.Descendants().Where(n => n.Name == "script").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "link").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "style").ToList().ForEach(n => n.Remove());

            WebContent.Descendants().Where(n => n.Name == "video").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "aside").ToList().ForEach(n => n.Remove()); //guardian
            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("block-share")).ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "figure").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("element element-interactive interactive")).ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "iframe").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "svg").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "figure").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("element-video")).ToList().ForEach(n => n.Remove());

            string HTMLCode = WebContent.InnerHtml;

            HTMLCode = Regex.Replace(HTMLCode, "</?(a|A).*?(alt=\"Click to see content).*?>.*</a>", ""); //remove hyperlinks Bbc Only
            HTMLCode = Regex.Replace(HTMLCode, "</?(a|A).*?>", ""); //remove hyperlinks
            HTMLCode = Regex.Replace(HTMLCode, "<p.*?>", "<p>");
            HTMLCode = Regex.Replace(HTMLCode, "<div.*?class=\"content-list-component text\".*?>", "<p>");
            HTMLCode = Regex.Replace(HTMLCode, "<div.*?>", "<div>");
            HTMLCode = Regex.Replace(HTMLCode, "<figure.*?>", "<figure>");
            HTMLCode = Regex.Replace(HTMLCode, "<span.*?>", string.Empty);
            HTMLCode = Regex.Replace(HTMLCode, "</span>", string.Empty);
            HTMLCode = Regex.Replace(HTMLCode, "<figcaption.*?>", "<figcaption>"); //guardian

            HTMLCode = Regex.Replace(HTMLCode, "<h2.*?>", "<h2>");
            HTMLCode = Regex.Replace(HTMLCode, "</h2>", "</h2>");
            HTMLCode = Regex.Replace(HTMLCode, "<ul.*?>", "<ul>");
            HTMLCode = Regex.Replace(HTMLCode, "<li.*?>", "<li>");
            HTMLCode = Regex.Replace(HTMLCode, "<blockquote.*?>", "<blockquote>");

            HTMLCode = Regex.Replace(HTMLCode, "</li>", "</li>");
            HTMLCode = Regex.Replace(HTMLCode, "<li> *<\\/li>", string.Empty);
            HTMLCode = Regex.Replace(HTMLCode, "((<source>)(\n)*)+", "<source>");
            HTMLCode = Regex.Replace(HTMLCode, "((</source>)(\n)*)+", "</source>");

            return HTMLCode;

        }
        public HtmlNode GetHuffingtonMainBodyNews(HtmlDocument Webpage)
        {
            var findclassesnews = Webpage.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("entry__body js-entry-body"));
            var htmlnews = findclassesnews.FirstOrDefault();
            return htmlnews;
        }
        public String StripHTMLHuff(HtmlNode WebContent)
        {
            WebContent.Descendants().Where(n => n.Name == "script").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "link").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "style").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "iframe").ToList().ForEach(n => n.Remove());

            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("top-media")).ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("image__credit js-image-credit")).ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "section").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("extra-content")).ToList().ForEach(n => n.Remove());//huff only
            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("tag-cloud")).ToList().ForEach(n => n.Remove());//huff only
            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("below-entry")).ToList().ForEach(n => n.Remove());//huff only
            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("slideshow")).ToList().ForEach(n => n.Remove());//huff only


            foreach (var div in WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("embed-asset")).ToList())
            {
                foreach (var a in div.Descendants().Where(b => b.Name == "blockquote").Where(b => b.Attributes.Contains("class") && b.Attributes["class"].Value.Equals("twitter-tweet")).ToList())
                {
                    a.Attributes.Remove("align");
                    string replace = a.OuterHtml;
                    HtmlNode lb2 = HtmlNode.CreateNode("lb2");
                    lb2.InnerHtml = "<p>" + replace + "</p>";
                    div.ParentNode.ReplaceChild(lb2, div);

                }
            }
            foreach (var span in WebContent.Descendants().Where(n => n.Name == "span").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("quote")).ToList())
            {
                HtmlNode lb3 = HtmlNode.CreateNode("lb3");
                lb3.InnerHtml = "</br></br>";
                span.ParentNode.InsertAfter(lb3, span);
            }

            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("embed-asset")).ToList().ForEach(n => n.Remove());
            string d = WebContent.OuterHtml;
            string HTMLCode = WebContent.InnerHtml;

            HTMLCode = Regex.Replace(HTMLCode, "<span.*?>", "<span>");
            HTMLCode = Regex.Replace(HTMLCode, @"\t|\n|\r", "");
            HTMLCode = Regex.Replace(HTMLCode, "</?(a|A).*?>", ""); //remove hyperlinks
            HTMLCode = Regex.Replace(HTMLCode, "<p.*?>", "<p>");
            HTMLCode = Regex.Replace(HTMLCode, "<div.*?>", "<div>");
            HTMLCode = Regex.Replace(HTMLCode, "<figure.*?>", "<figure>");
            HTMLCode = Regex.Replace(HTMLCode, "<figcaption.*?>", "<figcaption>"); //guardian

            HTMLCode = Regex.Replace(HTMLCode, "<h2.*?>", "<h2>");
            HTMLCode = Regex.Replace(HTMLCode, "</h2>", "</h2>");
            HTMLCode = Regex.Replace(HTMLCode, "<ul.*?>", "<ul>");
            HTMLCode = Regex.Replace(HTMLCode, "<li.*?>", "<li>");
            HTMLCode = Regex.Replace(HTMLCode, "<blockquote.*?>", "<blockquote>");

            HTMLCode = Regex.Replace(HTMLCode, "</li>", "</li>");
            HTMLCode = Regex.Replace(HTMLCode, "<li> *<\\/li>", string.Empty);
            HTMLCode = Regex.Replace(HTMLCode, "((<source>)(\n)*)+", "<source>");
            HTMLCode = Regex.Replace(HTMLCode, "((</source>)(\n)*)+", "</source>");

            return HTMLCode;

        }
        public HtmlNode GetIndependantMainBodyNews(HtmlDocument Webpage)
        {
            var findclassesnews = Webpage.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("itemprop") && d.Attributes["itemprop"].Value.Equals("articleBody"));
            var htmlnews = findclassesnews.FirstOrDefault();
            return htmlnews;
        }
        public String StripHTMLInde(HtmlNode WebContent)
        {
            WebContent.Descendants().Where(n => n.Name == "script").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "iframe").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "link").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "style").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "meta").ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("inline-block-related")).ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("type-video")).ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("type-gallery")).ToList().ForEach(n => n.Remove());
            WebContent.Descendants().Where(n => n.Name == "ul").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("inline-pipes-list")).ToList().ForEach(n => n.Remove());

            foreach (var div in WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("image")).ToList())
            {
                foreach (var img in WebContent.Descendants().Where(n => n.Name == "img").ToList())
                {

                    img.Attributes.Remove("width");
                    img.Attributes.Remove("height");
                    img.Attributes.Remove("alt");
                    img.Attributes.Remove("title");

                }

            }

            foreach (var div in WebContent.Descendants().Where(n => n.Name == "div").Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Equals("dnd-caption-wrapper")).ToList())
            {

                string replace = div.InnerHtml;
                HtmlNode lb4 = HtmlNode.CreateNode("lb4");
                lb4.InnerHtml = "<figcaption>" + replace + "</figcaption>";
                div.ParentNode.ReplaceChild(lb4, div);
            }



            string HTMLCode = WebContent.InnerHtml;
            HTMLCode = Regex.Replace(HTMLCode, "<span.*?>", "<span>");

            HTMLCode = Regex.Replace(HTMLCode, "<span.*?>", "<span>");
            HTMLCode = Regex.Replace(HTMLCode, @"\t|\n|\r", "");
            HTMLCode = Regex.Replace(HTMLCode, "</?(a|A).*?>", ""); //remove hyperlinks
            HTMLCode = Regex.Replace(HTMLCode, "<p.*?>", "<p>");
            HTMLCode = Regex.Replace(HTMLCode, "<div.*?>", "<div>");
            HTMLCode = Regex.Replace(HTMLCode, "<figure.*?>", "<figure>");
            HTMLCode = Regex.Replace(HTMLCode, "<figcaption.*?>", "<figcaption>"); //guardian

            HTMLCode = Regex.Replace(HTMLCode, "<h2.*?>", "<h2>");
            HTMLCode = Regex.Replace(HTMLCode, "</h2>", "</h2>");
            HTMLCode = Regex.Replace(HTMLCode, "<ul.*?>", "<ul>");
            HTMLCode = Regex.Replace(HTMLCode, "<li.*?>", "<li>");
            HTMLCode = Regex.Replace(HTMLCode, "<blockquote.*?>", "<blockquote>");

            HTMLCode = Regex.Replace(HTMLCode, "</li>", "</li>");
            HTMLCode = Regex.Replace(HTMLCode, "<li> *<\\/li>", string.Empty);
            HTMLCode = Regex.Replace(HTMLCode, "((<source>)(\n)*)+", "<source>");
            HTMLCode = Regex.Replace(HTMLCode, "((</source>)(\n)*)+", "</source>");


            return HTMLCode;

        }


    }
}
