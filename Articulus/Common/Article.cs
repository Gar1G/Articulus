using Newsify6.Common.Embedly;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using Newsify6.Common.LocalDatabase;

namespace Newsify6.Common
{
    public class Article
    {
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string ImageCredit { get; set; } = string.Empty;
        public string MainTag { get; set; } = string.Empty;
        public string PubDate { get; set; } = string.Empty;
        public string SubTagsJsonString { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
