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
using System.Diagnostics;
using ProjectOxfordWebServ.Models;

namespace ProjectOxfordWebServ.Controllers
{
    public class GlobalTagDataAllUsersController : ApiController
    {
        private ProjectOxfordWebServContext db = new ProjectOxfordWebServContext();

        // GET: api/GlobalTagDataAllUsers
        public IQueryable<GlobalTagDataAllUsers> GetGlobalTagDataAllUsers()
        {
            return db.GlobalTagDataAllUsers;
        }

        // GET: api/GlobalTagDataAllUsers/5
        public IHttpActionResult GetGlobalTagDataAllUsers(int id)
        {
            switch (id)
            {
                //Calculate this every day on a scheduler

                case 0:  //calculate sum [N_t * p(category = ci | click) / p(category = ci)]

                    DateTime daybefore = DateTime.Now.AddDays(-1); //Find day before - we choose day as this is our specified time period

                    //calculate D(u,t) = Ni/N

                    ///calculate N for each user
                    ///Dictionary to store users total emotions over time period
                    Dictionary<string, double[]> User_Emotion_Total = Indivdual_Emotion_Data_Sum(daybefore);

                    //Calculate sum[N_t] 
                    StoreAccumulationSum(User_Emotion_Total);

                    ///Calculate Ni for each user
                    ///Function returns d(U,T)

                    //dictionary to store individual user tag data
                    Dictionary<Tuple<string, string>, double[]> User_Tag_Emotion_Total = Individual_Emotion_Tag_Data(User_Emotion_Total, daybefore);

                    //calculate D(t)
                    Dictionary<string, double[]> Global_Distribution_Tag_Emotions = calculate_d_t(daybefore);

                    //Need to store/update data in new table which the client can access from his end using their user-id; TABLE: _Userid | _Tag | D(u,t)/D(t) 
                    StoreUserDistributionByGlobalDistributionByUsersClicksMainTagS(User_Tag_Emotion_Total, Global_Distribution_Tag_Emotions, User_Emotion_Total);

                    return Ok();

                default:
                    return BadRequest();

            }


        }

        // POST: api/GlobalTagDataAllUsers
        [ResponseType(typeof(GlobalTagDataAllUsers))]
        public IHttpActionResult PostGlobalTagDataAllUsers(GlobalTagDataAllUsers globalTagDataAllUsers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GlobalTagDataAllUsers.Add(globalTagDataAllUsers);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = globalTagDataAllUsers.Id }, globalTagDataAllUsers);
        }


        private Dictionary<string, double[]> Indivdual_Emotion_Data_Sum(DateTime t)
        {

            ////Calculates clicks in a specific timeperiod for each user
            ///// query table to give back user id and emotion data from specific time period
            var User_Specific_Data_Past_t = from GlobalTagDataAllUsers in db.GlobalTagDataAllUsers
                                            where GlobalTagDataAllUsers.Timestamp >= t && GlobalTagDataAllUsers.Timestamp <= DateTime.Now
                                            group GlobalTagDataAllUsers by GlobalTagDataAllUsers.UserId into useridgroup
                                            select useridgroup;

            Dictionary<string, double[]> User_Emotion_Total = new Dictionary<string,double[]>();

            /////sum each users clicks 
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
        private Dictionary<Tuple<string, string>, double[]> Individual_Emotion_Tag_Data(Dictionary<string, double[]> User_Emotion_Total, DateTime daybefore)
        {

            ////query table to give back user id and emotion data from specific time period and tag
            var User_Specific_Tag_Data_Past_t = from GlobalTagDataAllUsers in db.GlobalTagDataAllUsers
                                                where GlobalTagDataAllUsers.Timestamp >= daybefore
                                                where GlobalTagDataAllUsers.Timestamp <= DateTime.Now
                                                group GlobalTagDataAllUsers by new { GlobalTagDataAllUsers.UserId, GlobalTagDataAllUsers.MainTag } into useridtaggroup
                                                select useridtaggroup;


            //dictionary to store individual user tag data
            Dictionary<Tuple<string, string>, double[]> User_Tag_Emotion_Total = new Dictionary<Tuple<string, string>, double[]>();

            ////sum each user clicks in each seperate tag and calculate d(u,t)
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
                User_Tag_Emotion_Total.Add(new Tuple<string, string>(useridtaggroup.Key.UserId, useridtaggroup.Key.MainTag), StoreValues); //stores d(u,t) values

            }

            return User_Tag_Emotion_Total;

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

                double[] StoreValues = new double[10];
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
        private void StoreAccumulationSum(Dictionary<string, double[]> UserGlobalTagData)
        {

            foreach (var item in UserGlobalTagData)
            {

                var query = (from a in db.sum_nt_g
                             where a.userId == item.Key
                             select a).SingleOrDefault();

                if (query == null)
                {
                    query = new SumUserDataGlobal();
                    query.userId = item.Key;
                    query.AngerEmotionNt = item.Value[0];
                    query.ContemptEmotionNt = item.Value[1];
                    query.DisgustEmotionNt = item.Value[2];
                    query.FearEmotionNt = item.Value[3];
                    query.HappinessEmotionNt = item.Value[4];
                    query.NeutraEmotionNt = item.Value[5];
                    query.SadnessEmotionNt = item.Value[6];
                    query.SurpriseEmotionNt = item.Value[7];
                    db.sum_nt_g.Add(query);
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
        private void StoreUserDistributionByGlobalDistributionByUsersClicksMainTagS(Dictionary<Tuple<string, string>, double[]> User_Tag_Emotion_Total, Dictionary<string, double[]> Global_Distribution_Tag_Emotions, Dictionary<string, double[]> User_Emotion_Total)
        {
            foreach (var item in User_Tag_Emotion_Total)
            {
                //Calculate  [N_t * p(category = ci | click) / p(category = ci)]
                double[] StoreTuple = User_Emotion_Total[item.Key.Item1];
                double[] StoreTuple1 = Global_Distribution_Tag_Emotions[item.Key.Item2];


                double AngerEmotion = StoreTuple[0] * (item.Value[0]) / StoreTuple1[0];
                double ContemptEmotion = StoreTuple[1] * (item.Value[1]) / StoreTuple1[1];
                double DisgustEmotion = StoreTuple[2] * (item.Value[2]) / StoreTuple1[2];
                double FearEmotion = StoreTuple[3] * (item.Value[3]) / StoreTuple1[3];
                double HappinessEmotion = StoreTuple[4] * (item.Value[4]) / StoreTuple1[4];
                double NeutraEmotion = StoreTuple[5] * (item.Value[5]) / StoreTuple1[5];
                double SadnessEmotion = StoreTuple[6] * (item.Value[6]) / StoreTuple1[6];
                double SurpriseEmotion = StoreTuple[7] * (item.Value[7]) / StoreTuple1[7];


                var store = (from e in db.D_ut_by_d_t_Time_Period_Update
                             where e.User_Id == item.Key.Item1
                             where e.Tag == item.Key.Item2
                             select e).SingleOrDefault();

                if (store == null)
                {
                    store = new UserByGlobalDistributionUpdate();
                    store.User_Id = item.Key.Item1;
                    store.Tag = item.Key.Item2;
                    store.AngerEmotion = AngerEmotion;
                    store.ContemptEmotion = ContemptEmotion;
                    store.DisgustEmotion = DisgustEmotion;
                    store.FearEmotion = FearEmotion;
                    store.HappinessEmotion = HappinessEmotion;
                    store.NeutraEmotion = NeutraEmotion;
                    store.SadnessEmotion = SadnessEmotion;
                    store.SurpriseEmotion = SurpriseEmotion;
                    store.Timestamp = DateTime.Now;
                    db.D_ut_by_d_t_Time_Period_Update.Add(store);
                }

                else
                {
                    store.AngerEmotion = AngerEmotion + store.AngerEmotion;
                    store.ContemptEmotion = ContemptEmotion + store.ContemptEmotion;
                    store.DisgustEmotion = DisgustEmotion + store.DisgustEmotion;
                    store.FearEmotion = FearEmotion + store.FearEmotion;
                    store.HappinessEmotion = HappinessEmotion + store.HappinessEmotion;
                    store.NeutraEmotion = NeutraEmotion + store.NeutraEmotion;
                    store.SadnessEmotion = SadnessEmotion + store.SadnessEmotion;
                    store.SurpriseEmotion = SurpriseEmotion + store.SurpriseEmotion;
                    store.Timestamp = DateTime.Now;
                }
                db.SaveChanges();
            }

        }

    }

}