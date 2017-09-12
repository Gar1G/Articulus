using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.ArticleSearch.Classes.ImageSearch
{
    public class InsightsSourcesSummary
    {
        private int _shoppingSourcesCount;
        public int ShoppingSourcesCount
        {
            get { return _shoppingSourcesCount; }
            set { _shoppingSourcesCount = value; }
        }

        private int _recipeSourcesCount;
        public int RecipeSourcesCount
        {
            get { return _recipeSourcesCount; }
            set { _recipeSourcesCount = value; }
        }

    }
}
