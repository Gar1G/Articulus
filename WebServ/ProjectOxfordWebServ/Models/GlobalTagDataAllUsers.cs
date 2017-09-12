using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectOxfordWebServ.Models
{
    public class GlobalTagDataAllUsers
    {

        public int Id { get; set; }
        public string UserId { get; set; }
        public double AngerEmotion { get; set; }
        public double ContemptEmotion { get; set; }
        public double DisgustEmotion { get; set; }
        public double FearEmotion { get; set; }
        public double HappinessEmotion { get; set; }
        public double NeutraEmotion { get; set; }
        public double SadnessEmotion { get; set; }
        public double SurpriseEmotion { get; set; }
        public string MainTag { get; set; }
        public DateTime Timestamp { get; set; }

    }
}