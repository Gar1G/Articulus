using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.ArticleSearch.Classes.ImageSearch
{
    public class BingImageSearchRootObject
    {
        private string _type;
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private Instrumentation _instrumentation;
        public Instrumentation Instrumentation
        {
            get { return _instrumentation; }
            set { _instrumentation = value; }
        }

        private string _webSearchUrl;
        public string WebSearchUrl
        {
            get { return _webSearchUrl; }
            set { _webSearchUrl = value; }
        }

        private string _totalEstimatedMatches;
        public string TotalEstimatedMatches
        {
            get { return _totalEstimatedMatches; }
            set { _totalEstimatedMatches = value; }
        }

        private List<Value> _value;
        public List<Value> Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private List<QueryExpansion> _queryExpansion;
        public List<QueryExpansion> QueryExpansion
        {
            get { return _queryExpansion; }
            set { _queryExpansion = value; }
        }

        private int _nextOffsetAddCount;
        public int NextOffsetAddCount
        {
            get { return _nextOffsetAddCount; }
            set { _nextOffsetAddCount = value; }
        }

        private List<PivotSuggestion> _pivotSuggestions;
        public List<PivotSuggestion> PivotSuggestions
        {
            get { return _pivotSuggestions; }
            set { _pivotSuggestions = value; }
        }

        private bool _displayShoppingSourcesBadges;
        public bool DisplayShoppingSourcesBadges
        {
            get { return _displayShoppingSourcesBadges; }
            set { _displayShoppingSourcesBadges = value; }
        }

        private bool _displayRecipeSourcesBadges;
        public bool DisplayRecipeSourcesBadges
        {
            get { return _displayRecipeSourcesBadges; }
            set { _displayRecipeSourcesBadges = value; }
        }

    }
}
