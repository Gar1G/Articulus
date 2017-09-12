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
using Common;
namespace ProjectOxfordWebServ.Controllers
{
    public class UserTopicsPreferredController : ApiController
    {
        private ProjectOxfordWebServContext db = new ProjectOxfordWebServContext();

        public IHttpActionResult GetUserTopicsPreferred(string Email)
        {
            var EmotionTags =   FindTags(Email);
             
            return Ok(EmotionTags);
        }


        private EmotionTag FindTags(string Email)
        {

            var usertags = new EmotionTag();
            //get the top 3 main tags and store in UserTags
            //anger
            var Top3Anger = (from a in db.Ranking_Main_Tag
                             where a.User_Id == Email
                             orderby a.AngerEmotion descending
                             select a).Take(3).ToList();

            string Top3AngerString;

            if (Top3Anger.Count() > 0)
            {

                //Main Tag Name
                usertags.Anger.FirstMainTagName = Top3Anger[0].Tag;
                //Main Tag Score
                usertags.Anger.FirstMainTag.Score = Top3Anger[0].AngerEmotion;

                Top3AngerString = Top3Anger[0].Tag;
                var Top3SubAngerFirst = (from a in db.Ranking_Sub_Tag
                                         where a.User_Id == Email && a.Tag == Top3AngerString
                                         orderby a.AngerEmotion descending
                                         select a).Take(3).ToList();

                if(Top3SubAngerFirst.Count() > 0)
                {
                    usertags.Anger.FirstMainTag.SubTags.FirstSubTag = Top3SubAngerFirst[0].Sub_Tag;
                    usertags.Anger.FirstMainTag.SubTags.FirstSubTagScore = Top3SubAngerFirst[0].AngerEmotion;
                }
                if(Top3SubAngerFirst.Count() > 1)
                {
                    usertags.Anger.FirstMainTag.SubTags.SecondSubTag = Top3SubAngerFirst[1].Sub_Tag;
                    usertags.Anger.FirstMainTag.SubTags.SecondSubTagScore = Top3SubAngerFirst[1].AngerEmotion;
                }
                if(Top3SubAngerFirst.Count() > 2)
                {
                    usertags.Anger.FirstMainTag.SubTags.ThirdSubTag = Top3SubAngerFirst[2].Sub_Tag;
                    usertags.Anger.FirstMainTag.SubTags.ThirdSubTagScore = Top3SubAngerFirst[2].AngerEmotion;

                }

            }
            if (Top3Anger.Count() > 1)
            {

                usertags.Anger.SecondMainTagName = Top3Anger[1].Tag;
                usertags.Anger.SecondMainTag.Score = Top3Anger[1].AngerEmotion;
                Top3AngerString = Top3Anger[1].Tag;

                var Top3SubAngerSecond = (from a in db.Ranking_Sub_Tag
                                          where a.User_Id == Email && a.Tag == Top3AngerString
                                          orderby a.AngerEmotion descending
                                          select a).Take(3).ToList();

                if (Top3SubAngerSecond.Count() > 0)
                {
                    usertags.Anger.SecondMainTag.SubTags.FirstSubTag = Top3SubAngerSecond[0].Sub_Tag;
                    usertags.Anger.SecondMainTag.SubTags.FirstSubTagScore = Top3SubAngerSecond[0].AngerEmotion;
                }
                if (Top3SubAngerSecond.Count() > 1)
                {
                    usertags.Anger.SecondMainTag.SubTags.SecondSubTag = Top3SubAngerSecond[1].Sub_Tag;
                    usertags.Anger.SecondMainTag.SubTags.SecondSubTagScore = Top3SubAngerSecond[1].AngerEmotion;
                }
                if (Top3SubAngerSecond.Count() > 2)
                {
                    usertags.Anger.SecondMainTag.SubTags.ThirdSubTag = Top3SubAngerSecond[2].Sub_Tag;
                    usertags.Anger.SecondMainTag.SubTags.ThirdSubTagScore = Top3SubAngerSecond[2].AngerEmotion;
                }


            }
            if (Top3Anger.Count() > 2)
            {
                usertags.Anger.ThirdMainTagName = Top3Anger[2].Tag;
                usertags.Anger.ThirdMainTag.Score = Top3Anger[2].AngerEmotion;

                Top3AngerString = Top3Anger[2].Tag;

                var Top3SubAngerThird = (from a in db.Ranking_Sub_Tag
                                         where a.User_Id == Email && a.Tag == Top3AngerString
                                         orderby a.AngerEmotion descending
                                         select a).Take(3).ToList();

                if (Top3SubAngerThird.Count() > 0)
                {
                    usertags.Anger.ThirdMainTag.SubTags.FirstSubTag = Top3SubAngerThird[0].Sub_Tag;
                    usertags.Anger.ThirdMainTag.SubTags.FirstSubTagScore = Top3SubAngerThird[0].AngerEmotion;
                }
                if (Top3SubAngerThird.Count() > 1)
                {
                    usertags.Anger.ThirdMainTag.SubTags.SecondSubTag = Top3SubAngerThird[1].Sub_Tag;
                    usertags.Anger.ThirdMainTag.SubTags.SecondSubTagScore = Top3SubAngerThird[1].AngerEmotion;
                }
                if (Top3SubAngerThird.Count() > 2)
                {
                    usertags.Anger.ThirdMainTag.SubTags.ThirdSubTag = Top3SubAngerThird[2].Sub_Tag;
                    usertags.Anger.ThirdMainTag.SubTags.ThirdSubTagScore = Top3SubAngerThird[2].AngerEmotion;
                }

            }



            var Top3Happiness = (from a in db.Ranking_Main_Tag
                                 where a.User_Id == Email
                                 orderby a.HappinessEmotion descending
                                 select a).Take(3).ToList();

            string Top3HappinessString;

            if (Top3Happiness.Count() > 0)
            {

                //Main Tag Name
                usertags.Happiness.FirstMainTagName = Top3Happiness[0].Tag;
                //Main Tag Score
                usertags.Happiness.FirstMainTag.Score = Top3Happiness[0].HappinessEmotion;

                Top3HappinessString = Top3Happiness[0].Tag;
                var Top3SubHappinessFirst = (from a in db.Ranking_Sub_Tag
                                             where a.User_Id == Email && a.Tag == Top3HappinessString
                                             orderby a.HappinessEmotion descending
                                             select a).Take(3).ToList();

                if (Top3SubHappinessFirst.Count() > 0)
                {
                    usertags.Happiness.FirstMainTag.SubTags.FirstSubTag = Top3SubHappinessFirst[0].Sub_Tag;
                    usertags.Happiness.FirstMainTag.SubTags.FirstSubTagScore = Top3SubHappinessFirst[0].HappinessEmotion;
                }
                if (Top3SubHappinessFirst.Count() > 1)
                {
                    usertags.Happiness.FirstMainTag.SubTags.SecondSubTag = Top3SubHappinessFirst[1].Sub_Tag;
                    usertags.Happiness.FirstMainTag.SubTags.SecondSubTagScore = Top3SubHappinessFirst[1].HappinessEmotion;
                }
                if (Top3SubHappinessFirst.Count() > 2)
                {
                    usertags.Happiness.FirstMainTag.SubTags.ThirdSubTag = Top3SubHappinessFirst[2].Sub_Tag;
                    usertags.Happiness.FirstMainTag.SubTags.ThirdSubTagScore = Top3SubHappinessFirst[2].HappinessEmotion;

                }

            }
            if (Top3Happiness.Count() > 1)
            {

                usertags.Happiness.SecondMainTagName = Top3Happiness[1].Tag;
                usertags.Happiness.SecondMainTag.Score = Top3Happiness[1].HappinessEmotion;
                Top3HappinessString = Top3Happiness[1].Tag;

                var Top3SubHappinessSecond = (from a in db.Ranking_Sub_Tag
                                              where a.User_Id == Email && a.Tag == Top3HappinessString
                                              orderby a.HappinessEmotion descending
                                              select a).Take(3).ToList();

                if (Top3SubHappinessSecond.Count() > 0)
                {
                    usertags.Happiness.SecondMainTag.SubTags.FirstSubTag = Top3SubHappinessSecond[0].Sub_Tag;
                    usertags.Happiness.SecondMainTag.SubTags.FirstSubTagScore = Top3SubHappinessSecond[0].HappinessEmotion;
                }
                if (Top3SubHappinessSecond.Count() > 1)
                {
                    usertags.Happiness.SecondMainTag.SubTags.SecondSubTag = Top3SubHappinessSecond[1].Sub_Tag;
                    usertags.Happiness.SecondMainTag.SubTags.SecondSubTagScore = Top3SubHappinessSecond[1].HappinessEmotion;
                }
                if (Top3SubHappinessSecond.Count() > 2)
                {
                    usertags.Happiness.SecondMainTag.SubTags.ThirdSubTag = Top3SubHappinessSecond[2].Sub_Tag;
                    usertags.Happiness.SecondMainTag.SubTags.ThirdSubTagScore = Top3SubHappinessSecond[2].HappinessEmotion;
                }


            }
            if (Top3Happiness.Count() > 2)
            {
                usertags.Happiness.ThirdMainTagName = Top3Happiness[2].Tag;
                usertags.Happiness.ThirdMainTag.Score = Top3Happiness[2].HappinessEmotion;

                Top3HappinessString = Top3Happiness[2].Tag;

                var Top3SubHappinessThird = (from a in db.Ranking_Sub_Tag
                                             where a.User_Id == Email && a.Tag == Top3HappinessString
                                             orderby a.HappinessEmotion descending
                                             select a).Take(3).ToList();

                if (Top3SubHappinessThird.Count() > 0)
                {
                    usertags.Happiness.ThirdMainTag.SubTags.FirstSubTag = Top3SubHappinessThird[0].Sub_Tag;
                    usertags.Happiness.ThirdMainTag.SubTags.FirstSubTagScore = Top3SubHappinessThird[0].HappinessEmotion;
                }
                if (Top3SubHappinessThird.Count() > 1)
                {
                    usertags.Happiness.ThirdMainTag.SubTags.SecondSubTag = Top3SubHappinessThird[1].Sub_Tag;
                    usertags.Happiness.ThirdMainTag.SubTags.SecondSubTagScore = Top3SubHappinessThird[1].HappinessEmotion;
                }
                if (Top3SubHappinessThird.Count() > 2)
                {
                    usertags.Happiness.ThirdMainTag.SubTags.ThirdSubTag = Top3SubHappinessThird[2].Sub_Tag;
                    usertags.Happiness.ThirdMainTag.SubTags.ThirdSubTagScore = Top3SubHappinessThird[2].HappinessEmotion;
                }

            }
            /*****************************/
            var Top3Sadness = (from a in db.Ranking_Main_Tag
                               where a.User_Id == Email
                               orderby a.SadnessEmotion descending
                               select a).Take(3).ToList();

            string Top3SadnessString;

            if (Top3Sadness.Count() > 0)
            {

                //Main Tag Name
                usertags.Sadness.FirstMainTagName = Top3Sadness[0].Tag;
                //Main Tag Score
                usertags.Sadness.FirstMainTag.Score = Top3Sadness[0].SadnessEmotion;

                Top3SadnessString = Top3Sadness[0].Tag;
                var Top3SubSadnessFirst = (from a in db.Ranking_Sub_Tag
                                           where a.User_Id == Email && a.Tag == Top3SadnessString
                                           orderby a.SadnessEmotion descending
                                           select a).Take(3).ToList();

                if (Top3SubSadnessFirst.Count() > 0)
                {
                    usertags.Sadness.FirstMainTag.SubTags.FirstSubTag = Top3SubSadnessFirst[0].Sub_Tag;
                    usertags.Sadness.FirstMainTag.SubTags.FirstSubTagScore = Top3SubSadnessFirst[0].SadnessEmotion;
                }
                if (Top3SubSadnessFirst.Count() > 1)
                {
                    usertags.Sadness.FirstMainTag.SubTags.SecondSubTag = Top3SubSadnessFirst[1].Sub_Tag;
                    usertags.Sadness.FirstMainTag.SubTags.SecondSubTagScore = Top3SubSadnessFirst[1].SadnessEmotion;
                }
                if (Top3SubSadnessFirst.Count() > 2)
                {
                    usertags.Sadness.FirstMainTag.SubTags.ThirdSubTag = Top3SubSadnessFirst[2].Sub_Tag;
                    usertags.Sadness.FirstMainTag.SubTags.ThirdSubTagScore = Top3SubSadnessFirst[2].SadnessEmotion;

                }

            }
            if (Top3Sadness.Count() > 1)
            {

                usertags.Sadness.SecondMainTagName = Top3Sadness[1].Tag;
                usertags.Sadness.SecondMainTag.Score = Top3Sadness[1].SadnessEmotion;
                Top3SadnessString = Top3Sadness[1].Tag;

                var Top3SubSadnessSecond = (from a in db.Ranking_Sub_Tag
                                            where a.User_Id == Email && a.Tag == Top3SadnessString
                                            orderby a.SadnessEmotion descending
                                            select a).Take(3).ToList();

                if (Top3SubSadnessSecond.Count() > 0)
                {
                    usertags.Sadness.SecondMainTag.SubTags.FirstSubTag = Top3SubSadnessSecond[0].Sub_Tag;
                    usertags.Sadness.SecondMainTag.SubTags.FirstSubTagScore = Top3SubSadnessSecond[0].SadnessEmotion;
                }
                if (Top3SubSadnessSecond.Count() > 1)
                {
                    usertags.Sadness.SecondMainTag.SubTags.SecondSubTag = Top3SubSadnessSecond[1].Sub_Tag;
                    usertags.Sadness.SecondMainTag.SubTags.SecondSubTagScore = Top3SubSadnessSecond[1].SadnessEmotion;
                }
                if (Top3SubSadnessSecond.Count() > 2)
                {
                    usertags.Sadness.SecondMainTag.SubTags.ThirdSubTag = Top3SubSadnessSecond[2].Sub_Tag;
                    usertags.Sadness.SecondMainTag.SubTags.ThirdSubTagScore = Top3SubSadnessSecond[2].SadnessEmotion;
                }


            }
            if (Top3Sadness.Count() > 2)
            {
                usertags.Sadness.ThirdMainTagName = Top3Sadness[2].Tag;
                usertags.Sadness.ThirdMainTag.Score = Top3Sadness[2].SadnessEmotion;

                Top3SadnessString = Top3Sadness[2].Tag;

                var Top3SubSadnessThird = (from a in db.Ranking_Sub_Tag
                                           where a.User_Id == Email && a.Tag == Top3SadnessString
                                           orderby a.SadnessEmotion descending
                                           select a).Take(3).ToList();

                if (Top3SubSadnessThird.Count() > 0)
                {
                    usertags.Sadness.ThirdMainTag.SubTags.FirstSubTag = Top3SubSadnessThird[0].Sub_Tag;
                    usertags.Sadness.ThirdMainTag.SubTags.FirstSubTagScore = Top3SubSadnessThird[0].SadnessEmotion;
                }
                if (Top3SubSadnessThird.Count() > 1)
                {
                    usertags.Sadness.ThirdMainTag.SubTags.SecondSubTag = Top3SubSadnessThird[1].Sub_Tag;
                    usertags.Sadness.ThirdMainTag.SubTags.SecondSubTagScore = Top3SubSadnessThird[1].SadnessEmotion;
                }
                if (Top3SubSadnessThird.Count() > 2)
                {
                    usertags.Sadness.ThirdMainTag.SubTags.ThirdSubTag = Top3SubSadnessThird[2].Sub_Tag;
                    usertags.Sadness.ThirdMainTag.SubTags.ThirdSubTagScore = Top3SubSadnessThird[2].SadnessEmotion;
                }

            }
            /*****************************/
            return usertags;
        }

    }
}
