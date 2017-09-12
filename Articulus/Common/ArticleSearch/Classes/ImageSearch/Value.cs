using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.ArticleSearch.Classes.ImageSearch
{
    public class Value
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _webSearchUrl;
        public string WebSearchUrl
        {
            get { return _webSearchUrl; }
            set { _webSearchUrl = value; }
        }  

        private string _thumbnailUrl;
        public string ThumbnailUrl
        {
            get { return _thumbnailUrl; }
            set { _thumbnailUrl = value; }
        }

        private string _datePublished;
        public string DatePublished
        {
            get { return _datePublished; }
            set { _datePublished = value; }
        }

        private string _contentUrl;
        public string ContentUrl
        {
            get { return _contentUrl; }
            set { _contentUrl = value; }
        }

        private string _hostedPageUrl;
        public string HostedPageUrl
        {
            get { return _hostedPageUrl; }
            set { _hostedPageUrl = value; }
        }

        private string _contentSize;
        public string ContentSize
        {
            get { return _contentSize; }
            set { _contentSize = value; }
        }

        private string _encodingFormat;
        public string EncodingFormat
        {
            get { return _encodingFormat; }
            set { _encodingFormat = value; }
        }

        private string _hostPageDisplayUrl;
        public string MyProperty
        {
            get { return _hostPageDisplayUrl; }
            set { _hostPageDisplayUrl = value; }
        }

        private int _width;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        private Thumbnail _thumbnail;
        public Thumbnail Thumbnail
        {
            get { return _thumbnail; }
            set { _thumbnail = value; }
        }

        private string _imagesInsightsToken;
        public string ImagesInsightsToken
        {
            get { return _imagesInsightsToken; }
            set { _imagesInsightsToken = value; }
        }

        private InsightsSourcesSummary _insightsSourcesSummary;
        public InsightsSourcesSummary InsightsSourcesSummary
        {
            get { return _insightsSourcesSummary; }
            set { _insightsSourcesSummary = value; }
        }

        private string _imageId;
        public string ImageId
        {
            get { return _imageId; }
            set { _imageId = value; }
        }

        private string _accentColor;
        public string AccentColor
        {
            get { return _accentColor; }
            set { _accentColor = value; }
        }

    }
}
