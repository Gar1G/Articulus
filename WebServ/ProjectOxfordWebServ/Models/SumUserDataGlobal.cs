using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectOxfordWebServ.Models
{
    public class SumUserDataGlobal
    {

        public int Id { get; set; }
        public string userId { get; set; }
        public double AngerEmotionNt { get; set; }
        public double ContemptEmotionNt { get; set; }
        public double DisgustEmotionNt { get; set; }
        public double FearEmotionNt { get; set; }
        public double HappinessEmotionNt { get; set; }
        public double NeutraEmotionNt { get; set; }
        public double SadnessEmotionNt { get; set; }
        public double SurpriseEmotionNt { get; set; }

    }
}