using System;

namespace Newsify6.Common.GlobalDatabase
{
    /// <summary>
    /// This table will contain information about the article that the user has just read;
    /// Post to this table after the reader has finished reading the article;
    /// Each of the scores is calculated based on Junchen's formula;
    /// The calculations are completly independent and thus a new algorithm can be implemented very easily;
    /// </summary>
    public class UserMainTagEmotionData
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string ArticleID { get; set; }
        public string MainTag { get; set; }
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
