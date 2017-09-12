using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsify6.Common.GlobalDatabase
{
    /// <summary>
    /// Table will contain information about user's response to a certain article;
    /// Only post to this after the user has finished reading the aritcle, this will ensure that the ArticleID already exists in the database;
    /// In the case that the user presses dislike without when open the article, then post to ArticleData table first, then post to UserArticleData;
    /// Also, if person has not read the article, put duration as 0;
    /// </summary>
    public class UserArticleData
    {
        [Key, Column(Order = 0)]
        public string Email { get; set; } = string.Empty;
        public string ArticleID { get; set; } = string.Empty;
        public string LikeDislike { get; set; } = string.Empty;
        public int Duration { get; set; }
        public DateTime TimeStamp { get; set; }
    }

}
