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
    public class UserSubTagEmotionDatasController : ApiController
    {
        private ProjectOxfordWebServContext db = new ProjectOxfordWebServContext();

        // GET: api/UserSubTagEmotionDatas
        public IQueryable<UserSubTagEmotionData> GetUserSubTagEmotionDatas()
        {
            return db.UserSubTagEmotionDatas;
        }

        // GET: api/UserSubTagEmotionDatas/5
        [ResponseType(typeof(UserSubTagEmotionData))]
        public IHttpActionResult GetUserSubTagEmotionData(int id)
        {

            DateTime Time30MinsBefore = DateTime.Now.AddMinutes(-30);

            // Calculate D(U/T) time period is 30mins
            //calculate N for each user

            Dictionary<string, double[]> User_Emotion_Total = Individual_Emotion_Data_Sum(Time30MinsBefore);

            //calculate sum[N_t] 
            StoreAccumulationSum(User_Emotion_Total);

            //calculate Ni 
            //function return d(u,t)

            Dictionary<Tuple<string,string,string>,double[]> User_Tag_Emotion_Total = Individual_Emotion_Tag_Data(User_Emotion_Total, Time30MinsBefore);

            StoreD_UT(User_Tag_Emotion_Total);

            // Calculate P_0(category) every 30 mins
            Dictionary < Tuple<string, string> , double[]> tag_emotion_current_news = calculate_d_t(Time30MinsBefore);

            CalculateAndStoreRank(tag_emotion_current_news);

            return Ok();

        }


        private Dictionary<string,double[]> Individual_Emotion_Data_Sum(DateTime Timebefore)
        {

            //calculates scores in a specific timeperiod for each user
            ///Query table to give back user id and emotion data for a specific time period
            ///

            var User_Specific_Data_Past_t = from a in db.UserSubTagEmotionDatas
                                            where a.Timestamp >= Timebefore && a.Timestamp <= DateTime.Now
                                            group a by a.UserId into useridgroup
                                            select useridgroup;

            Dictionary<string, double[]> User_Emotion_Total = new Dictionary<string, double[]>();

            foreach (var useridgroup in User_Specific_Data_Past_t)
            {
                var sumAnger = useridgroup.Sum(x => x.AngerEmotion);
                var sumContempt = useridgroup.Sum(x => x.ContemptEmotion);
                var sumDisgust = useridgroup.Sum(x => x.DisgustEmotion);
                var sumFear = useridgroup.Sum(x => x.FearEmotion);
                var sumHappiness = useridgroup.Sum(x => x.HappinessEmotion);
                var sumNeutral = useridgroup.Sum(x => x.NeutraEmotion);
                var sumSadness = useridgroup.Sum(x => x.SadnessEmotion);
                var sumSurpise = useridgroup.Sum(x => x.SurpriseEmotion);

                double[] StoreTuple = new double[10];
                StoreTuple[0] = sumAnger;
                StoreTuple[1] = sumContempt;
                StoreTuple[2] = sumDisgust;
                StoreTuple[3] = sumFear;
                StoreTuple[4] = sumHappiness;
                StoreTuple[5] = sumNeutral;
                StoreTuple[6] = sumSadness;
                StoreTuple[7] = sumSurpise;

                User_Emotion_Total.Add(useridgroup.Key, StoreTuple); //get a users specific emotion sum for that time period User_Emotion_Total.Add[userid]
            }

            return User_Emotion_Total;

        }


        private Dictionary<Tuple<string,string,string>, double[]> Individual_Emotion_Tag_Data(Dictionary<string, double[]> User_Emotion_Total, DateTime daybefore)
        {

            //query table to give back user id and emotion data for a specific time period and subtag
            var User_Specific_Tag_Data_Past_t = from a in db.UserSubTagEmotionDatas
                                                where a.Timestamp >= daybefore
                                                where a.Timestamp <= DateTime.Now
                                                group a by new { a.UserId, a.MainTag,a.SubTag } into useridtaggroup
                                                select useridtaggroup;


            Dictionary<Tuple<string, string,string>, double[]> User_Tag_Emotion_Total = new Dictionary<Tuple<string,string, string>, double[]>();

            foreach (var useridtaggroup in User_Specific_Tag_Data_Past_t)
            {
                var sumAnger = useridtaggroup.Sum(x => x.AngerEmotion);
                var sumContempt = useridtaggroup.Sum(x => x.ContemptEmotion);
                var sumDisgust = useridtaggroup.Sum(x => x.DisgustEmotion);
                var sumFear = useridtaggroup.Sum(x => x.FearEmotion);
                var sumHappiness = useridtaggroup.Sum(x => x.HappinessEmotion);
                var sumNeutral = useridtaggroup.Sum(x => x.NeutraEmotion);
                var sumSadness = useridtaggroup.Sum(x => x.SadnessEmotion);
                var sumSurpise = useridtaggroup.Sum(x => x.SurpriseEmotion);

                double[] StoreTuple = User_Emotion_Total[useridtaggroup.Key.UserId];

                sumAnger = sumAnger / StoreTuple[0];
                sumContempt = sumContempt / StoreTuple[1];
                sumDisgust = sumDisgust / StoreTuple[2];
                sumFear = sumFear / StoreTuple[3];
                sumHappiness = sumHappiness / StoreTuple[4];
                sumNeutral = sumNeutral / StoreTuple[5];
                sumSadness = sumSadness / StoreTuple[6];
                sumSurpise = sumSurpise / StoreTuple[7];


                double[] StoreValues = new double[8];
                StoreValues[0] = sumAnger;
                StoreValues[1] = sumContempt;
                StoreValues[2] = sumDisgust;
                StoreValues[3] = sumFear;
                StoreValues[4] = sumHappiness;
                StoreValues[5] = sumNeutral;
                StoreValues[6] = sumSadness;
                StoreValues[7] = sumSurpise;
                User_Tag_Emotion_Total.Add(new Tuple<string,string, string>(useridtaggroup.Key.UserId, useridtaggroup.Key.MainTag, useridtaggroup.Key.SubTag), StoreValues); //stores d(u,t) values

            }

            return User_Tag_Emotion_Total;


        }

        private Dictionary<Tuple<string,string>, double[]> calculate_d_t(DateTime t)
        {

            Dictionary<Tuple<string, string>, double[]> Global_Distribution_Tag_Emotions = new Dictionary<Tuple<string, string>, double[]>();
            ///Calculates all clicks from users over specific time period
            var Global_Data_Past_t_Anger = db.UserSubTagEmotionDatas.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.AngerEmotion);
            var Global_Data_Past_t_Contempt = db.UserSubTagEmotionDatas.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.ContemptEmotion);
            var Global_Data_Past_t_Disgust = db.UserSubTagEmotionDatas.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.DisgustEmotion);
            var Global_Data_Past_t_Fear = db.UserSubTagEmotionDatas.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.FearEmotion);
            var Global_Data_Past_t_Happiness = db.UserSubTagEmotionDatas.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.HappinessEmotion);
            var Global_Data_Past_t_Neutral = db.UserSubTagEmotionDatas.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.NeutraEmotion);
            var Global_Data_Past_t_Sadness = db.UserSubTagEmotionDatas.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.SadnessEmotion);
            var Global_Data_Past_t_Surprise = db.UserSubTagEmotionDatas.AsEnumerable().Where(x => x.Timestamp >= t && x.Timestamp <= DateTime.Now).Sum(x => x.SurpriseEmotion);

            ///Calculates click for all users in specific tags
            var Global_Tag_Data_Past_t = from a in db.UserSubTagEmotionDatas
                                         where a.Timestamp >= t
                                         where a.Timestamp <= DateTime.Now
                                         group a by new { a.MainTag , a.SubTag} into taggroup
                                         select taggroup;

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

                Global_Distribution_Tag_Emotions.Add(new Tuple <string,string>(taggroup.Key.MainTag,taggroup.Key.SubTag), StoreValues); //stores d(t) values 

            }

            return Global_Distribution_Tag_Emotions;
        }

        private void StoreAccumulationSum(Dictionary<string, double[]> User_Emotion_Total)
        {

            foreach (var item in User_Emotion_Total)
            {

                //query to check whether user exists in sum_nt_sub_tag table
                var query = (from a in db.sum_nt_sub_tag
                             where a.userId == item.Key
                             select a).SingleOrDefault();

                if (query == null)
                {
                    query = new SumUserDataSubTag();
                    query.userId = item.Key;
                    query.AngerEmotionNt = item.Value[0];
                    query.ContemptEmotionNt = item.Value[1];
                    query.DisgustEmotionNt = item.Value[2];
                    query.FearEmotionNt = item.Value[3];
                    query.HappinessEmotionNt = item.Value[4];
                    query.NeutraEmotionNt = item.Value[5];
                    query.SadnessEmotionNt = item.Value[6];
                    query.SurpriseEmotionNt = item.Value[7];
                    db.sum_nt_sub_tag.Add(query);
                }

                else
                {
                    query.AngerEmotionNt = item.Value[0] + query.AngerEmotionNt;
                    query.ContemptEmotionNt = item.Value[1] + query.ContemptEmotionNt;
                    query.DisgustEmotionNt = item.Value[2] + query.DisgustEmotionNt;
                    query.FearEmotionNt = item.Value[3] + query.FearEmotionNt;
                    query.HappinessEmotionNt = item.Value[4] + query.HappinessEmotionNt;
                    query.NeutraEmotionNt = item.Value[5] + query.NeutraEmotionNt;
                    query.SadnessEmotionNt = item.Value[6] + query.SadnessEmotionNt;
                    query.SurpriseEmotionNt = item.Value[7] + query.SurpriseEmotionNt;

                }

            }

            db.SaveChanges();

        }

        private void StoreD_UT(Dictionary<Tuple<string, string, string>, double[]> User_Tag_Emotion_Total)
        {
            foreach (var item in User_Tag_Emotion_Total)
            {


                var checkintable = (from e in db.D_ut_by_d_t_Time_Period_Update_sub_tags
                                    where e.User_Id == item.Key.Item1 && e.Tag == item.Key.Item2 && e.SubTag == item.Key.Item3
                                    select e).SingleOrDefault();


                if (checkintable == null)
                {
                    checkintable = new UserDistributionUpdate();
                    checkintable.User_Id = item.Key.Item1;
                    checkintable.Tag = item.Key.Item2;
                    checkintable.SubTag = item.Key.Item3;
                    checkintable.AngerEmotion = item.Value[0];
                    checkintable.ContemptEmotion = item.Value[1];
                    checkintable.DisgustEmotion = item.Value[2];
                    checkintable.FearEmotion = item.Value[3];
                    checkintable.HappinessEmotion = item.Value[4];
                    checkintable.NeutraEmotion = item.Value[5];
                    checkintable.SadnessEmotion = item.Value[6];
                    checkintable.SurpriseEmotion = item.Value[7];
                    checkintable.Timestamp = DateTime.Now;
                    db.D_ut_by_d_t_Time_Period_Update_sub_tags.Add(checkintable);
                }

                else
                {
                    checkintable.AngerEmotion = item.Value[0] + checkintable.AngerEmotion;
                    checkintable.ContemptEmotion = item.Value[1] + checkintable.ContemptEmotion;
                    checkintable.DisgustEmotion = item.Value[2] + checkintable.DisgustEmotion;
                    checkintable.FearEmotion = item.Value[3] + checkintable.FearEmotion;
                    checkintable.HappinessEmotion = item.Value[4] + checkintable.HappinessEmotion;
                    checkintable.NeutraEmotion = item.Value[5] + checkintable.NeutraEmotion;
                    checkintable.SadnessEmotion = item.Value[6] + checkintable.SadnessEmotion;
                    checkintable.SurpriseEmotion = item.Value[7] + checkintable.SurpriseEmotion;
                    checkintable.Timestamp = DateTime.Now;
                }

                db.SaveChanges();

            }
        }

        private void CalculateAndStoreRank(Dictionary<Tuple<string, string>, double[]> tag_emotion_current_news)
        {
            //Store ranking data
            var query_Sub = from a in db.D_ut_by_d_t_Time_Period_Update_sub_tags
                            group a by new { a.User_Id, a.Tag, a.SubTag } into useridsubtaggroup
                            select useridsubtaggroup;

            foreach (var item in query_Sub)
            {

                UserDistributionUpdate Rowstore = item.First();

                var query_Nt = from b in db.sum_nt_sub_tag
                               where b.userId == item.Key.User_Id
                               select b;

                SumUserDataSubTag RowStoreNt = query_Nt.First();

                double[] emotionarray = new double[8];

                if (tag_emotion_current_news.ContainsKey(new Tuple<string, string>(item.Key.Tag, item.Key.SubTag)))
                {
                    emotionarray = tag_emotion_current_news[new Tuple<string, string>(item.Key.Tag, item.Key.SubTag)];

                }
                else
                {
                    for (int i = 0; i < emotionarray.Length; i++)
                    {
                        emotionarray[i] = 0.2; //arbitary value to scale down a tag change this to find a good value
                    }
                }


                var rankscoreanger = (emotionarray[0] * ((Rowstore.AngerEmotion)) / RowStoreNt.AngerEmotionNt);
                var rankscorecontempt = (emotionarray[1] * ((Rowstore.ContemptEmotion)) / RowStoreNt.ContemptEmotionNt);
                var rankscoredisgust = (emotionarray[2] * ((Rowstore.DisgustEmotion)) / RowStoreNt.DisgustEmotionNt);
                var rankscorefear = (emotionarray[3] * ((Rowstore.FearEmotion)) / RowStoreNt.FearEmotionNt);
                var rankscorehappiness = (emotionarray[4] * ((Rowstore.HappinessEmotion)) / RowStoreNt.HappinessEmotionNt);
                var rankscoreneutral = (emotionarray[5] * ((Rowstore.NeutraEmotion)) / RowStoreNt.NeutraEmotionNt);
                var rankscoresaddness = (emotionarray[6] * ((Rowstore.SadnessEmotion)) / RowStoreNt.SadnessEmotionNt);
                var rankscoresurprise = (emotionarray[7] * ((Rowstore.SurpriseEmotion)) / RowStoreNt.SurpriseEmotionNt);

                //need to save this data in the ranking table, if this user id and tag already exists then update, if not then create new

                var querycheck = (from a in db.Ranking_Sub_Tag
                                  where a.User_Id == item.Key.User_Id && a.Tag == item.Key.Tag && a.Sub_Tag == item.Key.SubTag
                                  select a).SingleOrDefault();

                if (querycheck == null)
                {

                    querycheck = new RankingSubTag();
                    querycheck.User_Id = item.Key.User_Id;
                    querycheck.Tag = item.Key.Tag;
                    querycheck.Sub_Tag = item.Key.SubTag;
                    querycheck.AngerEmotion = rankscoreanger;
                    querycheck.ContemptEmotion = rankscorecontempt;
                    querycheck.DisgustEmotion = rankscoredisgust;
                    querycheck.FearEmotion = rankscorefear;
                    querycheck.HappinessEmotion = rankscorehappiness;
                    querycheck.NeutraEmotion = rankscoreneutral;
                    querycheck.SadnessEmotion = rankscoresaddness;
                    querycheck.SurpriseEmotion = rankscoresurprise;
                    db.Ranking_Sub_Tag.Add(querycheck);
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

        // POST: api/UserSubTagEmotionDatas
        [ResponseType(typeof(UserSubTagEmotionData))]
        public IHttpActionResult PostUserSubTagEmotionData(UserSubTagEmotionData userSubTagEmotionData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //check if the tag exists in the Table
            var querysearchexists = (from a in db.UserSubTagEmotionDatas
                                     where a.MainTag == userSubTagEmotionData.MainTag && a.SubTag == userSubTagEmotionData.SubTag && a.UserId == userSubTagEmotionData.UserId
                                     select a).ToList();

            //if it does not exist
            if (querysearchexists.Count == 0)
            {
                //count how many tags currently exist, if less than 100 then add to table else delete LRU and add

                //int querylessthan100 = db.UserSubTagEmotionDatas.Distinct().Where(a => a.UserId == userSubTagEmotionData.UserId && a.MainTag == userSubTagEmotionData.MainTag).Distinct().Count();

                var querylessthan100 = db.UserSubTagEmotionDatas.Where(a => a.UserId == userSubTagEmotionData.UserId && a.MainTag == userSubTagEmotionData.MainTag).GroupBy(x => x.SubTag).Select(x => x.Distinct().Count());
                int count = 0;

                foreach (var item in querylessthan100)
                {
                    count++;
                }
                
                if (count < 100)
                {
                    db.UserSubTagEmotionDatas.Add(userSubTagEmotionData);
                }
                else
                {

                    var oldest = db.UserSubTagEmotionDatas.OrderBy(x => x.Timestamp).First();
                    db.UserSubTagEmotionDatas.Remove(oldest);

                    var querysearchothertable = (from a in db.D_ut_by_d_t_Time_Period_Update_sub_tags
                                                where a.User_Id == oldest.UserId && a.Tag == oldest.MainTag && a.SubTag == oldest.SubTag
                                                select a).Single();

                    var querysearchothertable1 = (from a in db.Ranking_Sub_Tag
                                                  where a.User_Id == oldest.UserId && a.Tag == oldest.MainTag && a.Sub_Tag == oldest.SubTag
                                                  select a).Single();

                    if (querysearchothertable != null)
                    {

                        db.D_ut_by_d_t_Time_Period_Update_sub_tags.Remove(querysearchothertable);

                    }

                    if (querysearchothertable1 != null)
                    {

                        db.Ranking_Sub_Tag.Remove(querysearchothertable1);

                    }

                    db.UserSubTagEmotionDatas.Add(userSubTagEmotionData);
                }
            }
            else
            {

                db.UserSubTagEmotionDatas.Add(userSubTagEmotionData);


            }

            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = userSubTagEmotionData.Id }, userSubTagEmotionData);
        }

    }
}