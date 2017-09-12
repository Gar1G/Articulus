using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.ArticleSearch.Classes.NewsSearch
{
    public class About
    {
        private string _readLink;
        public string ReadLink
        {
            get { return _readLink; }
            set { _readLink = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
