using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.ArticleSearch.Classes.ImageSearch
{
    public class PivotSuggestion
    {
        private string _pivot;
        public string Pivot
        {
            get { return _pivot; }
            set { _pivot = value; }
        }

        private List<Suggestion> _suggestions;
        public List<Suggestion> Suggestions
        {
            get { return _suggestions; }
            set { _suggestions = value; }
        }
  
    }
}
