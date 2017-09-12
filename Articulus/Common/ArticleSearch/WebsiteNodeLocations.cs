using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.ArticleSearch
{
    public class WebsiteNodeLocations
    {
        #region Properties
        private List<string> _title;
        public List<string> Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _titleReplace;
        public string TitleReplace
        {
            get { return _titleReplace; }
            set { _titleReplace = value; }
        }

        private List<string> _image;
        public List<string> Image
        {
            get { return _image; }
            set { _image = value; }
        }

        private List<string> _imageCredit;
        public List<string> ImageCredit
        {
            get { return _imageCredit; }
            set { _imageCredit = value; }
        }

        private List<string> _mainTag;
        public List<string> MainTag
        {
            get { return _mainTag; }
            set { _mainTag = value; }
        }

        private List<string> _author;
        public List<string> Author
        {
            get { return _author; }
            set { _author = value; }
        }

        private List<string> _text;
        public List<string> Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private List<string> _description;
        public List<string> Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private List<string> _pubDate;
        public List<string> PubDate
        {
            get { return _pubDate; }
            set { _pubDate = value; }
        }

        private List<string> _keywords;
        public List<string> Keywords
        {
            get { return _keywords; }
            set { _keywords = value; }
        }
        #endregion

        #region Methods
        // Outputs structure with locations of all the nodes for the webScraper
        public static WebsiteNodeLocations GetNodeLocations(string url)
        {
            WebsiteNodeLocations WebsiteNodeLocations = new WebsiteNodeLocations();

            if (url.Contains("bbc.co.uk"))
            {
                if (url.Contains("sport"))
                    BBCSportNodeLocations(WebsiteNodeLocations);
                else if (url.Contains("news"))
                    BBCNewsNodeLocations(WebsiteNodeLocations);
            }
            else if (url.Contains("telegraph.co.uk"))
                TelegraphNodeLocations(WebsiteNodeLocations);
            else if (url.Contains("independent.co.uk"))
                IndependentNodeLocations(WebsiteNodeLocations);
            else if (url.Contains("huffingtonpost.co.uk"))
                HuffingtonPostNodeLocations(WebsiteNodeLocations);
            else if (url.Contains("theguardian.com"))
                GuardianNodeLocations(WebsiteNodeLocations);
            else
                return null;

            return WebsiteNodeLocations;
        }

        private static void GuardianNodeLocations(WebsiteNodeLocations WebsiteNodeLocations)
        {
            WebsiteNodeLocations.Title = new List<string> { "meta", "property", "og:title" };
            WebsiteNodeLocations.TitleReplace = null;
            WebsiteNodeLocations.Description = new List<string> { "meta", "property", "og:description" };
            WebsiteNodeLocations.MainTag = new List<string> { "meta", "property", "article:section" };
            WebsiteNodeLocations.Image = new List<string> { "meta", "name", "twitter:image" };
            WebsiteNodeLocations.Keywords = new List<string> { "meta", "name", "keywords" };
            WebsiteNodeLocations.Author = new List<string> { "meta", "name", "author" };
            WebsiteNodeLocations.PubDate = new List<string> { "time", "itemprop", "datePublished" };
            WebsiteNodeLocations.Text = new List<string> { "div", "itemprop", "articleBody", "p" };
            WebsiteNodeLocations.ImageCredit = new List<string> { "figcaption", "itemprop", "description" };
        }

        private static void HuffingtonPostNodeLocations(WebsiteNodeLocations WebsiteNodeLocations)
        {
            WebsiteNodeLocations.Title = new List<string> { "meta", "property", "og:title" };
            WebsiteNodeLocations.TitleReplace = null;
            WebsiteNodeLocations.Description = new List<string> { "meta", "name", "description" };
            WebsiteNodeLocations.Keywords = new List<string> { "meta", "name", "keywords" };
            WebsiteNodeLocations.Author = new List<string> { "a", "class", "author-card__details__name" };
            WebsiteNodeLocations.Image = new List<string> { "meta", "property", "og:image" };
            WebsiteNodeLocations.MainTag = new List<string> { "meta", "property", "article:section" };
            WebsiteNodeLocations.ImageCredit = new List<string> { "div", "class", "image__credit js-image-credit" };
            WebsiteNodeLocations.Text = new List<string> { "div", "class", "content-list-component text" };
            WebsiteNodeLocations.PubDate = new List<string> { "meta", "property", "article:published_time" };
        }

        private static void IndependentNodeLocations(WebsiteNodeLocations WebsiteNodeLocations)
        {
            WebsiteNodeLocations.Title = new List<string> { "meta", "property", "og:title" };
            WebsiteNodeLocations.TitleReplace = null;
            WebsiteNodeLocations.Description = new List<string> { "meta", "property", "og:description" };
            WebsiteNodeLocations.Keywords = new List<string> { "meta", "name", "keywords" };
            WebsiteNodeLocations.Author = new List<string> { "meta", "name", "article:author_name" };
            WebsiteNodeLocations.Image = new List<string> { "meta", "property", "og:image" };
            WebsiteNodeLocations.MainTag = new List<string> { "meta", "property", "article:section" };
            WebsiteNodeLocations.ImageCredit = new List<string> { "span", "class", "copyright" };
            WebsiteNodeLocations.Text = new List<string> { "div", "itemprop", "articleBody", "p", "class", "credits" };
            WebsiteNodeLocations.PubDate = new List<string> { "meta", "property", "article:published_time" };
        }

        private static void TelegraphNodeLocations(WebsiteNodeLocations WebsiteNodeLocations)
        {
            WebsiteNodeLocations.Title = new List<string> { "meta", "property", "og:title" };
            WebsiteNodeLocations.TitleReplace = null;
            WebsiteNodeLocations.Image = new List<string> { "meta", "property", "og:image" };
            WebsiteNodeLocations.ImageCredit = new List<string> { "span", "class", "lead-asset-copyright" };
            WebsiteNodeLocations.MainTag = new List<string> { "meta", "name", "tmgads.channel" };
            WebsiteNodeLocations.Author = new List<string> { "meta", "name", "DCSext.author" };
            WebsiteNodeLocations.Text = new List<string> { "article", "itemprop", "articleBody", "p" };
            WebsiteNodeLocations.Description = new List<string> { "meta", "property", "og:description" };
            WebsiteNodeLocations.PubDate = new List<string> { "meta", "name", "DCSext.articleFirstPublished" };
            WebsiteNodeLocations.Keywords = new List<string> { "meta", "name", "tmgads.keywords" };
        }

        private static void BBCNewsNodeLocations(WebsiteNodeLocations WebsiteNodeLocations)
        {
            WebsiteNodeLocations.Title = new List<string> { "meta", "property", "og:title" };
            WebsiteNodeLocations.TitleReplace = " - BBC News";
            WebsiteNodeLocations.Image = new List<string> { "img", "class", "js-image-replace" };
            WebsiteNodeLocations.ImageCredit = new List<string> { "span", "class", "story-image-copyright" };
            WebsiteNodeLocations.MainTag = new List<string> { "meta", "property", "og:article:section" };
            WebsiteNodeLocations.Author = new List<string> { "span", "class", "byline__name" };
            WebsiteNodeLocations.Text = new List<string> { "div", "property", "articleBody", "p" };
            WebsiteNodeLocations.Description = new List<string> { "meta", "property", "og:description" };
            WebsiteNodeLocations.PubDate = null;
            WebsiteNodeLocations.Keywords = null;
        }

        private static void BBCSportNodeLocations(WebsiteNodeLocations WebsiteNodeLocations)
        {
            WebsiteNodeLocations.Title = new List<string> { "meta", "property", "og:title" };
            WebsiteNodeLocations.TitleReplace = " - BBC News";
            WebsiteNodeLocations.Image = new List<string> { "div", "class", "sp-media-asset__image gel-responsive-image", "img" };
            WebsiteNodeLocations.ImageCredit = new List<string> { "span", " class", "story-image-copyright" };
            WebsiteNodeLocations.MainTag = new List<string> { "meta", "property", "article:section" };
            WebsiteNodeLocations.Author = new List<string> { "p", "class", "gel-long-primer" };
            WebsiteNodeLocations.Text = new List<string> { "div", "id", "story-body", "p" };
            WebsiteNodeLocations.Description = new List<string> { "meta", "property", "og:description" };
            WebsiteNodeLocations.PubDate = null;
            WebsiteNodeLocations.Keywords = null;
        }
        #endregion
    }

}
