using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{ 
    public class EmotionTag
    {

        public UserTags Anger =  new UserTags();
        public UserTags Happiness =  new UserTags();
        public UserTags Sadness = new UserTags();

    }
    public class UserTags
    {
        public string FirstMainTagName { get; set; }
        public MainTag FirstMainTag = new MainTag();
        public string SecondMainTagName { get; set; }
        public MainTag SecondMainTag = new MainTag();
        public string ThirdMainTagName { get; set; }
        public MainTag ThirdMainTag = new MainTag();
    }

    public class MainTag
    {
        public double Score { get; set; }
        public SubTag SubTags = new SubTag();
    }

    public class SubTag
    {
        public string FirstSubTag { get; set; }
        public double FirstSubTagScore { get; set; }
        public string SecondSubTag { get; set; }
        public double SecondSubTagScore { get; set; }
        public string ThirdSubTag { get; set; }
        public double ThirdSubTagScore { get; set; }
    }
}
