using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newsify6.Common.GlobalDatabase;
using Common;

namespace Newsify6.Interests
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InterestPage : Page
    {
        public List<InterestElement> InterestList = new List<InterestElement>()
        {
            new InterestElement { InterestId = 0, InterestType = "Sport", setInterestImage = "/Assets/sportImage.jpg", OriginalImage="/Assets/sportImage.jpg" , BlurredImage = "/Assets/sportImageblur.jpg"  },
            new InterestElement { InterestId = 1, InterestType = "Business", setInterestImage = "/Assets/business.jpg", OriginalImage="/Assets/business.jpg" , BlurredImage = "/Assets/businessblur.jpg"},
            new InterestElement { InterestId = 2, InterestType = "Politics", setInterestImage = "/Assets/politicsImage.jpg", OriginalImage="/Assets/politicsImage.jpg" , BlurredImage = "/Assets/politicsImageblur.jpg"},
            new InterestElement { InterestId = 3, InterestType = "Technology", setInterestImage = "/Assets/technologyImage.jpg", OriginalImage="/Assets/technologyImage.jpg" , BlurredImage = "/Assets/technologyImageblur.jpg"},
            new InterestElement { InterestId = 4, InterestType = "Health", setInterestImage = "/Assets/healthImage.jpg" ,OriginalImage="/Assets/healthImage.jpg" , BlurredImage = "/Assets/healthImageblur.jpg"},
            new InterestElement { InterestId = 5, InterestType = "Education", setInterestImage = "/Assets/educationImage.jpg" ,OriginalImage="/Assets/educationImage.jpg" , BlurredImage = "/Assets/educationImageblur.jpg"},
            new InterestElement { InterestId = 6, InterestType = "World", setInterestImage = "/Assets/worldImage.jpg", OriginalImage="/Assets/worldImage.jpg" , BlurredImage = "/Assets/worldImageblur.jpg"  },
            new InterestElement { InterestId = 7, InterestType = "Money", setInterestImage = "/Assets/moneyImage.jpg", OriginalImage="/Assets/moneyImage.jpg" , BlurredImage = "/Assets/moneyImageblur.jpg"},
            new InterestElement { InterestId = 8, InterestType = "Opinion", setInterestImage = "/Assets/opinionImage.jpg", OriginalImage="/Assets/opinionImage.jpg" , BlurredImage = "/Assets/opinionImageblur.jpg"},
            new InterestElement { InterestId = 9, InterestType = "Entertainment", setInterestImage = "/Assets/entertainmentImage.jpg", OriginalImage="/Assets/entertainmentImage.jpg" , BlurredImage = "/Assets/entertainmentImageblur.jpg"},
            new InterestElement { InterestId = 10, InterestType = "Arts", setInterestImage = "/Assets/artsImage.jpg" ,OriginalImage="/Assets/artsImage.jpg" , BlurredImage = "/Assets/artsImageblur.jpg"},
            new InterestElement { InterestId = 11, InterestType = "Books", setInterestImage = "/Assets/booksImage.jpg" ,OriginalImage="/Assets/booksImage.jpg" , BlurredImage = "/Assets/booksImageblur.jpg"},
            new InterestElement { InterestId = 12, InterestType = "Australia", setInterestImage = "/Assets/australiaImage.jpg", OriginalImage="/Assets/australiaImage.jpg" , BlurredImage = "/Assets/australiaImageblur.jpg"  },
            new InterestElement { InterestId = 13, InterestType = "Music", setInterestImage = "/Assets/musicImage.jpg", OriginalImage="/Assets/musicImage.jpg" , BlurredImage = "/Assets/musicImageblur.jpg"},
            new InterestElement { InterestId = 14, InterestType = "UK", setInterestImage = "/Assets/ukImage.jpg", OriginalImage="/Assets/ukImage.jpg" , BlurredImage = "/Assets/ukImageblur.jpg"},
            new InterestElement { InterestId = 15, InterestType = "USA", setInterestImage = "/Assets/usaImage.jpg", OriginalImage="/Assets/usaImage.jpg" , BlurredImage = "/Assets/usaImageblur.jpg"},
            new InterestElement { InterestId = 16, InterestType = "Travel", setInterestImage = "/Assets/travelImage.jpg" ,OriginalImage="/Assets/travelImage.jpg" , BlurredImage = "/Assets/travelImageblur.jpg"},
            new InterestElement { InterestId = 17, InterestType = "Film", setInterestImage = "/Assets/filmImage.jpg" ,OriginalImage="/Assets/filmImage.jpg" , BlurredImage = "/Assets/filmImageblur.jpg"},
            new InterestElement { InterestId = 18, InterestType = "Fashion", setInterestImage = "/Assets/fashionImage.jpg", OriginalImage="/Assets/fashionImage.jpg" , BlurredImage = "/Assets/fashionImageblur.jpg"  },
            new InterestElement { InterestId = 19, InterestType = "Environment", setInterestImage = "/Assets/environmentImage.jpg", OriginalImage="/Assets/environmentImage.jpg" , BlurredImage = "/Assets/environmentImageblur.jpg"},
            new InterestElement { InterestId = 20, InterestType = "Europe", setInterestImage = "/Assets/europeImage.jpg", OriginalImage="/Assets/europeImage.jpg" , BlurredImage = "/Assets/europeImageblur.jpg"},
            new InterestElement { InterestId = 21, InterestType = "Lifestyle", setInterestImage = "/Assets/lifestyleImage.jpg", OriginalImage="/Assets/lifestyleImage.jpg" , BlurredImage = "/Assets/lifestyleImageblur.jpg"},
            new InterestElement { InterestId = 22, InterestType = "Americas", setInterestImage = "/Assets/americasImage.jpg" ,OriginalImage="/Assets/americasImage.jpg" , BlurredImage = "/Assets/americasImageblur.jpg"},
            new InterestElement { InterestId = 23, InterestType = "Scotland", setInterestImage = "/Assets/scotlandImage.jpg" ,OriginalImage="/Assets/scotlandImage.jpg" , BlurredImage = "/Assets/scotlandImageblur.jpg"},
            new InterestElement { InterestId = 24, InterestType = "Asia", setInterestImage = "/Assets/asiaImage.jpg", OriginalImage="/Assets/asiaImage.jpg" , BlurredImage = "/Assets/asiaImageblur.jpg"  },
            new InterestElement { InterestId = 25, InterestType = "People", setInterestImage = "/Assets/peopleImage.jpg", OriginalImage="/Assets/peopleImage.jpg" , BlurredImage = "/Assets/peopleImageblur.jpg"},
            new InterestElement { InterestId = 26, InterestType = "Crime", setInterestImage = "/Assets/crimeImage.jpg", OriginalImage="/Assets/crimeImage.jpg" , BlurredImage = "/Assets/crimeImageblur.jpg"},
            new InterestElement { InterestId = 27, InterestType = "Culture", setInterestImage = "/Assets/cultureImage.jpg", OriginalImage="/Assets/cultureImage.jpg" , BlurredImage = "/Assets/cultureImageblur.jpg"},
            new InterestElement { InterestId = 28, InterestType = "Middle East", setInterestImage = "/Assets/middleImage.jpg" ,OriginalImage="/Assets/middleImage.jpg" , BlurredImage = "/Assets/middleImageblur.jpg"}
        };

        List<InterestElement> SelectedItems = new List<InterestElement>();

        public InterestPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            Run run = new Run();
            run.Text = "Your Preferences";
            run.FontSize = 24;
            run.FontWeight = Windows.UI.Text.FontWeights.SemiBold;
            run.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 44, 62, 80));
            guidetext.Inlines.Add(run);
            guidetext.Inlines.Add(new LineBreak());
            Run run1 = new Run();
            run1.FontSize = 12;
            run1.Text = "Please choose at least 5 interests";
            run1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 44, 62, 80));
            guidetext.Inlines.Add(run1);

            Next.IsEnabled = false;

            InterestGridView.IsItemClickEnabled = true;
            InterestGridView.SelectionMode = ListViewSelectionMode.Single;
            //InterestGridView.SelectionChanged += InterestGridView_SelectionChanged;
            InterestGridView.IsMultiSelectCheckBoxEnabled = false;
            InterestGridView.ItemClick += ItemClick;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SelectedItems.Clear();
            foreach(string Interest in UserTokenInfo.GetInstance.Topic)
            {
                InterestElement SelectedInterest = InterestList.FirstOrDefault(o => o.InterestType == Interest);
                SelectedItems.Add(SelectedInterest);
                SelectedInterest.Checked = 1;
                SelectedInterest.setInterestImage = SelectedInterest.BlurredImage;
            }

            if (SelectedItems.Count >= 5)
                Next.IsEnabled = true;
            else
                Next.IsEnabled = false;

        }

        private void ItemClick(object sender, ItemClickEventArgs e)
        {
            InterestElement InterestElement = (InterestElement)e.ClickedItem;
            InterestElement.Checked = InterestElement.Checked == 1 ? 0 : 1;
            InterestElement.setInterestImage = InterestElement.Checked == 1 ? InterestElement.BlurredImage : InterestElement.OriginalImage;
            if (InterestElement.Checked == 1)
                SelectedItems.Add(InterestElement);
            else
                SelectedItems.Remove(InterestElement);

            if (SelectedItems.Count >= 5)
                Next.IsEnabled = true;
            else
                Next.IsEnabled = false;

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            App.NavService.NavigateTo(typeof(Websites.WebsitePage), null);

            UserTokenInfo.GetInstance.Topic.Clear();

            foreach (InterestElement Topic in SelectedItems)
            {
                UserTokenInfo.GetInstance.Topic.Add(Topic.InterestType);
            }

            GlobalDatabaseAPI.POSTUserLoginInformation();
        }

    }
}
