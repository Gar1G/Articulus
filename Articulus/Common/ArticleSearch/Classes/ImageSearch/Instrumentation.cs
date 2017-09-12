using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.ArticleSearch.Classes.ImageSearch
{
    public class Instrumentation
    {
        private string _pageLoadPingUrl;
        public string PageLoadPingUrl
        {
            get { return _pageLoadPingUrl; }
            set { _pageLoadPingUrl = value; }
        }
    }
}
