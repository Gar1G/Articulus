using Newsify6.Common.GlobalDatabase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Common.GlobalDatabase
{ 
    public class HistoryArticle : NewsFeedArticle
    {
        public DateTime TimeStamp { get; set; }

        public HistoryArticle(GlobalDatabaseHistoryArticle GlobalDatabaseHistoryArticle)
        {
            foreach (PropertyInfo Property in GlobalDatabaseHistoryArticle.ArticleD.GetType().GetProperties())
            {
                if (GetType().GetProperty(Property.Name) != null)
                {
                    GetType().GetProperty(Property.Name).SetValue(this, Property.GetValue(GlobalDatabaseHistoryArticle.ArticleD));
                }
            }

            SubTagsJsonString = GlobalDatabaseHistoryArticle.ArticleD.SubTagJsonString;
            TimeStamp = GlobalDatabaseHistoryArticle.TimeStamp;

            if(GlobalDatabaseHistoryArticle.ArticleD.BestSubTagsJsonString != null)
                BestSubTags = JsonConvert.DeserializeObject<List<string>>(GlobalDatabaseHistoryArticle.ArticleD.BestSubTagsJsonString);
            if (GlobalDatabaseHistoryArticle.ArticleD.EmbedlyEntitiesJsonString != null)
                EmbedlyEntities = JsonConvert.DeserializeObject<List<string>>(GlobalDatabaseHistoryArticle.ArticleD.EmbedlyEntitiesJsonString);
            if (GlobalDatabaseHistoryArticle.ArticleD.EmbedlyKeywordsJsonString != null)
                EmbedlyKeywords = JsonConvert.DeserializeObject<List<string>>(GlobalDatabaseHistoryArticle.ArticleD.EmbedlyKeywordsJsonString);
            if (GlobalDatabaseHistoryArticle.ArticleD.SubTagJsonString != null)
                SubTags = JsonConvert.DeserializeObject<List<string>>(GlobalDatabaseHistoryArticle.ArticleD.SubTagJsonString);
        }
    }

    public class GlobalDatabaseHistoryArticle
    {
        public ArticleData ArticleD { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
