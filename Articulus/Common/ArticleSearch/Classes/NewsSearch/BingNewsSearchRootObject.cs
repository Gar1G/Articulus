using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.ArticleSearch.Classes.NewsSearch
{
    public class BingNewsSearchRootObject
    {
        private string _type;
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _readLink;
        public string ReadLink
        {
            get { return _readLink; }
            set { _readLink = value; }
        }

        private string _totalEstimatedMatches;
        public string TotalEstimatedMatches
        {
            get { return _totalEstimatedMatches; }
            set { _totalEstimatedMatches = value; }
        }

        private List<BingNewsSearchArticle> _value;
        public List<BingNewsSearchArticle> Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
