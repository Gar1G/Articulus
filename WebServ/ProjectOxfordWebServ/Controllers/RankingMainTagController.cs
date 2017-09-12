using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ProjectOxfordWebServ.Models;
using System.Diagnostics;

namespace ProjectOxfordWebServ.Controllers
{
    public class RankingMainTagController : ApiController
    {
        private ProjectOxfordWebServContext db = new ProjectOxfordWebServContext();

        public IQueryable<RankingMainTag> GetRanking_Main_Tag()
        {
            return db.Ranking_Main_Tag;
        }

        // GET: api/Ranking_Main_Tag/5
        public IHttpActionResult GetRanking_Main_Tag(int id)
        {
            switch (id)
            {

                case 0:

                    DateTime time30minsbefore = DateTime.Now.AddMinutes(-30); //Find the time 30 mins before
                    Dictionary<string, double[]> tag_emotion_current_news = calculate_d_t(time30minsbefore);

                    CalculateAndStoreRank(tag_emotion_current_news);
                                     
                    return Ok();

                default:
                    return BadRequest();

            }

        }

        private Dictionary<string, double[]> calculate_d_t(DateTime t)
        {
            Dictionary<string, double[]> Global_Distribution_Tag_Emotions = new Dictionary<string, double[]>(); //Dictionary which stores D(t) - Global_Distribution_Tag_Emotions[tag] = D(t)_Value

            ///Calculates all clicks from users over specific time period
            var Global_Data_Past_t_Anger = db.GlobalTagDataAllUsers.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.AngerEmotion);
            var Global_Data_Past_t_Contempt = db.GlobalTagDataAllUsers.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.ContemptEmotion);
            var Global_Data_Past_t_Disgust = db.GlobalTagDataAllUsers.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.DisgustEmotion);
            var Global_Data_Past_t_Fear = db.GlobalTagDataAllUsers.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.FearEmotion);
            var Global_Data_Past_t_Happiness = db.GlobalTagDataAllUsers.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.HappinessEmotion);
            var Global_Data_Past_t_Neutral = db.GlobalTagDataAllUsers.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.NeutraEmotion);
            var Global_Data_Past_t_Sadness = db.GlobalTagDataAllUsers.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.SadnessEmotion);
            var Global_Data_Past_t_Surprise = db.GlobalTagDataAllUsers.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.SurpriseEmotion);

            ///Calculates click for all users in specific tags
            var Global_Tag_Data_Past_t = from GlobalTagDataAllUsers in db.GlobalTagDataAllUsers
                                         where GlobalTagDataAllUsers.Timestamp >= t
                                         where GlobalTagDataAllUsers.Timestamp <= DateTime.Now
                                         group GlobalTagDataAllUsers by GlobalTagDataAllUsers.MainTag into taggroup
                                         select taggroup;

            //// calculates d(t)
            foreach (var taggroup in Global_Tag_Data_Past_t)
            {
                var sumAnger = taggroup.Sum(x => x.AngerEmotion);
                var sumContempt = taggroup.Sum(x => x.ContemptEmotion);
                var sumDisgust = taggroup.Sum(x => x.DisgustEmotion);
                var sumFear = taggroup.Sum(x => x.FearEmotion);
                var sumHappiness = taggroup.Sum(x => x.HappinessEmotion);
                var sumNeutral = taggroup.Sum(x => x.NeutraEmotion);
                var sumSadness = taggroup.Sum(x => x.SadnessEmotion);
                var sumSurpise = taggroup.Sum(x => x.SurpriseEmotion);

                sumAnger = sumAnger / Global_Data_Past_t_Anger;
                sumContempt = sumContempt / Global_Data_Past_t_Contempt;
                sumDisgust = sumDisgust / Global_Data_Past_t_Disgust;
                sumFear = sumFear / Global_Data_Past_t_Fear;
                sumHappiness = sumHappiness / Global_Data_Past_t_Happiness;
                sumNeutral = sumNeutral / Global_Data_Past_t_Neutral;
                sumSadness = sumSadness / Global_Data_Past_t_Sadness;
                sumSurpise = sumSurpise / Global_Data_Past_t_Surprise;

                double[] StoreValues = new double[8];
                StoreValues[0] = sumAnger;
                StoreValues[1] = sumContempt;
                StoreValues[2] = sumDisgust;
                StoreValues[3] = sumFear;
                StoreValues[4] = sumHappiness;
                StoreValues[5] = sumNeutral;
                StoreValues[6] = sumSadness;
                StoreValues[7] = sumSurpise;

                Global_Distribution_Tag_Emotions.Add(taggroup.Key, StoreValues); //stores d(t) values 

            }
            return Global_Distribution_Tag_Emotions;
        }

        private void CalculateAndStoreRank(Dictionary<string, double[]> tag_emotion_current_news)
        {
            var query = from a in db.D_ut_by_d_t_Time_Period_Update
                        group a by new { a.User_Id, a.Tag } into useridtaggroup
                        select useridtaggroup;


            foreach (var useridtaggroup in query)
            {
                UserByGlobalDistributionUpdate Rowstore = useridtaggroup.First();
                int G = 10;

                var query_Nt = from b in db.sum_nt_g
                               where b.userId == useridtaggroup.Key.User_Id
                               select b;

                SumUserDataGlobal RowStoreNt = query_Nt.First();
                double[] emotionarray = new double[8];

                if (tag_emotion_current_news.ContainsKey(useridtaggroup.Key.Tag))
                {
                    emotionarray = tag_emotion_current_news[useridtaggroup.Key.Tag];

                }
                else
                {
                    for (int i = 0; i < emotionarray.Length; i++)
                    {
                        emotionarray[i] = 0.1;
                    }
                }

                var rankscoreanger = (emotionarray[0] * ((G + (Rowstore.AngerEmotion)) / (RowStoreNt.AngerEmotionNt + G)));
                var rankscorecontempt = (emotionarray[1] * ((G + (Rowstore.ContemptEmotion)) / (RowStoreNt.ContemptEmotionNt + G)));
                var rankscoredisgust = (emotionarray[2] * ((G + (Rowstore.DisgustEmotion)) / (RowStoreNt.DisgustEmotionNt + G)));
                var rankscorefear = (emotionarray[3] * ((G + (Rowstore.FearEmotion)) / (RowStoreNt.FearEmotionNt + G)));
                var rankscorehappiness = (emotionarray[4] * ((G + (Rowstore.HappinessEmotion)) / (RowStoreNt.HappinessEmotionNt + G)));
                var rankscoreneutral = (emotionarray[5] * ((G + (Rowstore.NeutraEmotion)) / (RowStoreNt.NeutraEmotionNt + G)));
                var rankscoresaddness = (emotionarray[6] * ((G + (Rowstore.SadnessEmotion)) / (RowStoreNt.SadnessEmotionNt + G)));
                var rankscoresurprise = (emotionarray[7] * ((G + (Rowstore.SurpriseEmotion)) / (RowStoreNt.SurpriseEmotionNt + G)));

                //need to save this data in the ranking table, if this user id and tag already exists then update, if not then create new

                var querycheck = (from a in db.Ranking_Main_Tag
                                  where a.User_Id == useridtaggroup.Key.User_Id && a.Tag == useridtaggroup.Key.Tag
                                  select a).SingleOrDefault();

                if (querycheck == null)
                {

                    querycheck = new RankingMainTag();
                    querycheck.User_Id = useridtaggroup.Key.User_Id;
                    querycheck.Tag = useridtaggroup.Key.Tag;
                    querycheck.AngerEmotion = rankscoreanger;
                    querycheck.ContemptEmotion = rankscorecontempt;
                    querycheck.DisgustEmotion = rankscoredisgust;
                    querycheck.FearEmotion = rankscorefear;
                    querycheck.HappinessEmotion = rankscorehappiness;
                    querycheck.NeutraEmotion = rankscoreneutral;
                    querycheck.SadnessEmotion = rankscoresaddness;
                    querycheck.SurpriseEmotion = rankscoresurprise;
                    db.Ranking_Main_Tag.Add(querycheck);
                }
                else
                {
                    querycheck.AngerEmotion = rankscoreanger;
                    querycheck.ContemptEmotion = rankscorecontempt;
                    querycheck.DisgustEmotion = rankscoredisgust;
                    querycheck.FearEmotion = rankscorefear;
                    querycheck.HappinessEmotion = rankscorehappiness;
                    querycheck.NeutraEmotion = rankscoreneutral;
                    querycheck.SadnessEmotion = rankscoresaddness;
                    querycheck.SurpriseEmotion = rankscoresurprise;
                }


            }
            db.SaveChanges();
        }
    }
}