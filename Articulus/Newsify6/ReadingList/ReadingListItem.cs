using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.ReadingList
{
    public class ReadingListItem
    {
        public int readingListID { get; set; }
        public string readingListTitle { get; set; }
        public string readingListImage { get; set; }
    }

    public class ReadingListItemManager
    {
        public static List<ReadingListItem> GetBookMarkItem()
        {
            var readingListItems = new List<ReadingListItem>();
            //readingListItems.Add(new ReadingListItem { readingListID = 4, readingListTitle = "Clinton clinches Democratic nomination as Sanders vows to fight on", readingListImage = "Assets/uardianlogo.jpg" });
            //readingListItems.Add(new ReadingListItem { readingListID = 0, readingListTitle = "Boris Johnson warns of EU migration 'risks'", readingListImage = "Assets/BBCNewsImage.png" });
            //readingListItems.Add(new ReadingListItem { readingListID = 1, readingListTitle = "Jordan officers killed in attack at Baqaa camp near Amman,", readingListImage = "Assets/BBCNewsImage.png" });
            //readingListItems.Add(new ReadingListItem { readingListID = 2, readingListTitle = "US election 2016: Hillary Clinton wins Puerto Rico primary", readingListImage = "Assets/BBCNewsImage.png" });
            //readingListItems.Add(new ReadingListItem { readingListID = 3, readingListTitle = "Strike targets University of Winchester open day", readingListImage = "Assets/BBCNewsImage.png" });
            return readingListItems;
        }
    }
}
