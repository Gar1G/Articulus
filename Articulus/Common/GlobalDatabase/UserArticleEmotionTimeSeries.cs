using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsify6.Common.GlobalDatabase
{
    /// <summary>
    /// Table will contain information about user's response to a certain article;
    /// Will post to this table while the user is reading the article;
    /// The number of posts per second will depend on how fast the EmotionAPI is able to return information
    /// Again, before information is posted to this table, the ArticleData table will already contain information about the Article.
    /// </summary>
    public class UserArticleEmotionTimeSeries
    {
        [Key, Column(Order = 0)]
        public string Email { get; set; }
        [Key, Column(Order = 1)]
        public string ArticleID { get; set; }
        [Key, Column(Order = 2)]
        public DateTime Timestamp { get; set; }
        public double AngerScore { get; set; }
        public double ContemptScore { get; set; }
        public double DisgustScore { get; set; }
        public double FearScore { get; set; }
        public double HappinessScore { get; set; }
        public double NeutralScore { get; set; }
        public double SadnessScore { get; set; }
        public double SurpriseScore { get; set; }
    }
}
