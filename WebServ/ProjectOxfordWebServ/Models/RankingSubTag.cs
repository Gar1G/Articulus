using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectOxfordWebServ.Models
{
    public class RankingSubTag
    {

        [Key, Column(Order = 0)]
        public string User_Id { get; set; }
        [Key, Column(Order = 1)]
        public string Tag { get; set; }
        [Key, Column(Order = 2)]
        public string Sub_Tag { get; set; }
        public double AngerEmotion { get; set; }
        public double ContemptEmotion { get; set; }
        public double DisgustEmotion { get; set; }
        public double FearEmotion { get; set; }
        public double HappinessEmotion { get; set; }
        public double NeutraEmotion { get; set; }
        public double SadnessEmotion { get; set; }
        public double SurpriseEmotion { get; set; }

    }
}