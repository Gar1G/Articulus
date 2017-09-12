using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Embedly
{
    public class EmbedlyArticle
    {
        public string Content { get; set; }
        public List<Keyword> Keywords { get; set; }
        public List<Entity> Entities { get; set; }
    }
}
