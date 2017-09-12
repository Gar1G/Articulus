using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.ArticleSearch.Classes.ImageSearch
{
    public class Suggestion
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private string _displayText;
        public string DisplayText
        {
            get { return _displayText; }
            set { _displayText = value; }
        }

        private string _webSearchurl;
        public string WebSearchUrl
        {
            get { return _webSearchurl; }
            set { _webSearchurl = value; }
        }

        private string _searchLink;
        public string SearchLink
        {
            get { return _searchLink; }
            set { _searchLink = value; }
        }

        private ThumbnailUrlC _thumbnail;
        public ThumbnailUrlC Thumbnail
        {
            get { return _thumbnail; }
            set { _thumbnail = value; }
        }
    }
}
