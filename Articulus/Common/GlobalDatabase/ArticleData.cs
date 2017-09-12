using Newsify6.Common.LocalDatabase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Newsify6.Common.GlobalDatabase
{
    /// <summary>
    /// Table will hold data about all articles that have been read by all users;
    /// Only post to this table after an article has been opened by a specific user;
    /// If article already exists in the database, a new entry will not be added;
    /// ArticeID: Hash of the Url;
    /// </summary>
    public class ArticleData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ArticleId { get; set; }
        public string Author { get; set; }
        public string BestSubTagsJsonString { get; set; }
        public string Description { get; set; }
        public string EmbedlyEntitiesJsonString { get; set; }
        public string EmbedlyKeywordsJsonString { get; set; }
        public string Image { get; set; }
        public string ImageCredit { get; set; }
        public string MainTag { get; set; }
        public string PubDate { get; set; }
        public string SubTagJsonString { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int WordCount { get; set; }

        #region Constructors
        public ArticleData() { }

        public ArticleData(LocalDatabaseArticle LocalDatabaseArticle)
        { 
            foreach (PropertyInfo Property in LocalDatabaseArticle.GetType().GetProperties())
            {
                if (GetType().GetProperty(Property.Name) != null)
                {
                    GetType().GetProperty(Property.Name).SetValue(this, Property.GetValue(LocalDatabaseArticle));
                }
            }
        }
        #endregion
    }
}
