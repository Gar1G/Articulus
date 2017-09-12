using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.ArticleSearch.Classes.NewsSearch
{
    public class ClusteredArticle
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _url;
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private List<About> _about;
        public List<About> About
        {
            get { return _about; }
            set { _about = value; }
        }

        private string _datePublished;
        public string DatePublished
        {
            get { return _datePublished; }
            set { _datePublished = value; }
        }

        private string _category;
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }
    }
}
