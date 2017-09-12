using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;
using Newsify6.Common.GlobalDatabase;
using Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Newsify6.Websites
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebsitePage : Page
    {
        public List<WebsiteElement> WebsiteList = new List<WebsiteElement>()
        {
            new WebsiteElement { WebsiteId = 0, WebsiteName = "BBC", setWebsiteImage = "/Assets/bbcImage.jpg", OriginalImage="/Assets/bbcImage.jpg" , BlurredImage = "/Assets/bbcImageblur.jpg"  },
            new WebsiteElement { WebsiteId = 1, WebsiteName = "Huffington Post", setWebsiteImage = "/Assets/huffingtonImage.jpg", OriginalImage="/Assets/huffingtonImage.jpg" , BlurredImage = "/Assets/huffingtonImageblur.jpg"},
            new WebsiteElement { WebsiteId = 2, WebsiteName = "The Guardian", setWebsiteImage = "/Assets/guardianImage.jpg", OriginalImage="/Assets/guardianImage.jpg" , BlurredImage = "/Assets/guardianImageblur.jpg"},
            new WebsiteElement { WebsiteId = 3, WebsiteName = "The Telegraph", setWebsiteImage = "/Assets/telegraphImage.jpg", OriginalImage="/Assets/telegraphImage.jpg" , BlurredImage = "/Assets/telegraphImageblur.jpg"},
            new WebsiteElement { WebsiteId = 4, WebsiteName = "The Independant", setWebsiteImage = "/Assets/independentImage.jpg" ,OriginalImage="/Assets/independentImage.jpg" , BlurredImage = "/Assets/independentImageblur.jpg"}
        };

        List<WebsiteElement> SelectedItems = new List<WebsiteElement>();

        public WebsitePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            //Add text to textblockinlines
            Run run = new Run();
            run.Text = "Your Preferences";
            run.FontSize = 24;
            run.FontWeight = Windows.UI.Text.FontWeights.SemiBold;
            run.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 44, 62, 80));
            guidetext.Inlines.Add(run);
            guidetext.Inlines.Add(new LineBreak());
            Run run1 = new Run();
            run1.FontSize = 12;
            run1.Text = "Please choose at least 1 website";
            run1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 44, 62, 80));
            guidetext.Inlines.Add(run1);

            Next.IsEnabled = false;

            WebsiteGridView.IsItemClickEnabled = true;
            WebsiteGridView.SelectionMode = ListViewSelectionMode.Single;
            WebsiteGridView.IsMultiSelectCheckBoxEnabled = false;
            WebsiteGridView.ItemClick += ItemClick;

            Next.Click += Next_Click;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SelectedItems.Clear();
            foreach (string Website in UserTokenInfo.GetInstance.Website)
            {
                WebsiteElement SelectedWebsite = WebsiteList.FirstOrDefault(o => o.WebsiteName == Website);
                SelectedItems.Add(SelectedWebsite);
                SelectedWebsite.Checked = 1;
                SelectedWebsite.setWebsiteImage = SelectedWebsite.BlurredImage;
            }

            if (SelectedItems.Count >= 1)
                Next.IsEnabled = true;
            else
                Next.IsEnabled = false;
        }

        private void ItemClick(object sender, ItemClickEventArgs e)
        {
            WebsiteElement WebsiteElement = (WebsiteElement)e.ClickedItem;
            WebsiteElement.Checked = WebsiteElement.Checked == 1 ? 0 : 1;
            WebsiteElement.setWebsiteImage = WebsiteElement.Checked == 1 ? WebsiteElement.BlurredImage : WebsiteElement.OriginalImage;
            if (WebsiteElement.Checked == 1)
                SelectedItems.Add(WebsiteElement);
            else
                SelectedItems.Remove(WebsiteElement);

            if (SelectedItems.Count >= 1)
                Next.IsEnabled = true;
            else
                Next.IsEnabled = false;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            App.NavService.NavigateTo(typeof(MainPage), null);

            UserTokenInfo.GetInstance.Website.Clear();

            foreach (WebsiteElement Website in SelectedItems)
            {
                UserTokenInfo.GetInstance.Website.Add(Website.WebsiteName);
            }

            GlobalDatabaseAPI.POSTUserLoginInformation();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            App.NavService.NavigateTo(typeof(Interests.InterestPage), null);

            UserTokenInfo.GetInstance.Website.Clear();

            foreach (WebsiteElement Website in SelectedItems)
            {
                UserTokenInfo.GetInstance.Website.Add(Website.WebsiteName);
            }

            GlobalDatabaseAPI.POSTUserLoginInformation();
        }

    }
}
