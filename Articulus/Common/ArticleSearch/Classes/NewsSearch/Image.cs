using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.ArticleSearch.Classes.NewsSearch
{
    public class Image
    {
        private Thumbnail _thumbnail;
        public Thumbnail Thumbnail
        {
            get { return _thumbnail; }
            set { _thumbnail = value; }
        }
    }  
}
