using Common;
using HtmlAgilityPack;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Newsify6.Common;
using Newsify6.Common.GlobalDatabase;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Newsify6.ContentReader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArticleContentPage : Page
    {
        public ArticleContentPage()
        {
            this.InitializeComponent();
        }
        public int WordCount = 0;
        private MediaCapture _mediaCapture;
        EmotionTimeSeries emotiontimeseries = new EmotionTimeSeries();
        public Emotion[] EmotionResult;
        DateTime TimeStamp;
        bool recording = true;
        bool Like = false;
        bool Dislike = false;
        NewsFeedArticle Article;
        int Duration = 0;
        DateTime Timestamp;
        EmotionTimeSeries EmotionTimeSeries = new EmotionTimeSeries();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Article = e.Parameter as NewsFeedArticle;

            ContentPresenter content = new ContentPresenter();
            titlegridwrap.Visibility = Visibility.Collapsed;

            Title.Text = Article.Title;
            Title.FontSize = 34;
            Title.FontFamily = new FontFamily("Georgia");
            Title.TextAlignment = TextAlignment.Center;
            Title.HorizontalAlignment = HorizontalAlignment.Center;

            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            Debug.WriteLine(Article.Url);

            HtmlWeb htmlWeb = new HtmlWeb();

            try
            {
                document = await htmlWeb.LoadFromWebAsync(Article.Url);
            }
            catch (Exception Exception)
            {
                recording = false;
                var dialog = new MessageDialog(Exception.Message);
                dialog.Title = "Error";
                await dialog.ShowAsync();
                Frame.GoBack();
            }

            HtmlNode WebContent = null;

            if (Article.Url.Contains("bbc.co.uk"))
            {
                setStyle("bbc");
                content.cssfilename = "ContentReader/bbc.css";
                if (Article.Url.Contains("bbc.co.uk/news/"))
                    WebContent = content.GetBBCMainBodyNews(document);
                else if (Article.Url.Contains("bbc.co.uk/sport/"))
                    WebContent = content.GetBBCMainBodySport(document);

            }
            else if (Article.Url.Contains("guardian.com"))
            {
                setStyle("guardian");
                content.cssfilename = "ContentReader/guardian.css";
                WebContent = content.GetGuardianMainBodyNews(document);

            }
            else if (Article.Url.Contains("huffingtonpost.co.uk"))
            {
                setStyle("huff");
                content.cssfilename = "ContentReader/huff.css";
                WebContent = content.GetHuffingtonMainBodyNews(document);

            }
            else if (Article.Url.Contains("independent.co.uk"))
            {
                setStyle("inde");
                content.cssfilename = "ContentReader/inde.css";
                WebContent = content.GetIndependantMainBodyNews(document);
            }
            else
            {
                theWebView.Navigate(new Uri(Article.Url));
                titlegridwrap.Visibility = Visibility.Collapsed;
                Article.WordCount = 1;
            }

            if (WebContent != null)
            {
                string htmllist = null;

                if (Article.Url.Contains("bbc.co.uk"))
                {
                    htmllist = content.StripHTMLBBC(WebContent);
                }
                else if (Article.Url.Contains("guardian.com"))
                {
                    htmllist = content.StripHTMLGuardian(WebContent);
                }
                else if (Article.Url.Contains("huffingtonpost.co.uk"))
                {
                    htmllist = content.StripHTMLHuff(WebContent);
                }
                else if (Article.Url.Contains("independent.co.uk"))
                {
                    htmllist = content.StripHTMLInde(WebContent);
                }

                titlegridwrap.Visibility = Visibility.Visible;
                string image = Article.Image;
                WordCount = GetWordCount(htmllist);
                theWebView.NavigateToString("<html><head><link rel=\"stylesheet\" href=\"ms-appx-web:///" + content.cssfilename + "\" type=\"text/css\" media=\"screen\" /><meta name=\"format-detection\" content=\"telephone=no\"></head><body><div id=\"imgheader\"><img class=\"headerimg\" src=\"" + image + "\"></div>" + "<article>" + htmllist + " </article></body></html>");

            }
            else
            {
                Article.WordCount = 1;
                theWebView.Navigate(new Uri(Article.Url));
                titlegridwrap.Visibility = Visibility.Collapsed;

            }


        }

        private void setStyle(string website)
        {
            switch (website)
            {
                case "bbc":
                    all.Background = new SolidColorBrush(Color.FromArgb(255, 160, 21, 21));
                    Title.Foreground = new SolidColorBrush(Colors.White);
                    all.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 169, 23, 23));
                    all.BorderThickness = new Thickness(0, 0, 0, 5);
                    return;
                case "inde":
                    all.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    Title.Foreground = new SolidColorBrush(Colors.Black);
                    all.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 237, 27, 47));
                    all.BorderThickness = new Thickness(0, 0, 0, 5);
                    return;
                case "guardian":
                    all.Background = new SolidColorBrush(Color.FromArgb(255, 0, 86, 137));
                    all.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 69, 110));
                    all.BorderThickness = new Thickness(0, 0, 0, 5);
                    Title.Foreground = new SolidColorBrush(Colors.White);
                    return;
                case "huff":
                    all.Background = new SolidColorBrush(Color.FromArgb(255, 0, 116, 102));
                    Title.Foreground = new SolidColorBrush(Colors.White);
                    all.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 84, 73));
                    all.BorderThickness = new Thickness(0, 0, 0, 5);
                    return;
                default:
                    return;

            }
        }

        private async void GoBackNav(object sender, RoutedEventArgs e)
        {
            //if the article hasnt been parsed we cannot get the wordcount so we set it to one
            Debug.WriteLine(WordCount);
            recording = false;
            CalculateEmotionMetric();
            App.NavService.NavigateTo(typeof(MainPage), null);
            DateTime DateTime = DateTime.Now;
            EmotionInstance EmotionInstance = CalculateEmotionMetric();
            GlobalDatabaseAPI.POSTUserMainTagEmotionData(Article, DateTime, EmotionInstance);
            GlobalDatabaseAPI.POSTUserSubTagEmotionData(Article, DateTime, EmotionInstance);

            bool? LikeDislike = Like;
            if (Like == false && Dislike == false)
                LikeDislike = null;
            GlobalDatabaseAPI.POSTUserArticleData(Article, LikeDislike, 10);
        }


        private void theWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            LoadingProcessProgressRing.IsActive = true;
            LoadingProcessProgressRing.Visibility = Visibility.Visible;
            theWebView.Visibility = Visibility.Collapsed;
            all.Visibility = Visibility.Collapsed;

        }

        private async Task<bool> cam_Snap()
        {
            // Creat temporary storage file for camera snaps
            StorageFile temp = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("cam_snap.jpg", CreationCollisionOption.ReplaceExisting);

            ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();

            await _mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, temp);

            // Get emotions using API
            EmotionResult = await UploadAndDetectEmotions(temp.Path);

            // Get Timestamp
            Timestamp = DateTime.Now;

            Emotion Emotion = new Emotion();

            if (EmotionResult != null && EmotionResult.Count() != 0)
            {
                Emotion = EmotionResult[0];
            }

            UserArticleEmotionTimeSeries EmotionInstance = new UserArticleEmotionTimeSeries();

            EmotionInstance.Email = UserTokenInfo.GetInstance.Email;
            EmotionInstance.ArticleID = Article.ArticleId;
            EmotionInstance.Timestamp = Timestamp;
            EmotionInstance.AngerScore = Emotion.Scores.Anger;
            EmotionInstance.ContemptScore = Emotion.Scores.Contempt;
            EmotionInstance.DisgustScore = Emotion.Scores.Disgust;
            EmotionInstance.FearScore = Emotion.Scores.Fear;
            EmotionInstance.HappinessScore = Emotion.Scores.Happiness;
            EmotionInstance.NeutralScore = Emotion.Scores.Neutral;
            EmotionInstance.SadnessScore = Emotion.Scores.Sadness;
            EmotionInstance.SurpriseScore = Emotion.Scores.Surprise;

            GlobalDatabaseAPI.POSTUserArticleEmotionTimeSeries(EmotionInstance);

            EmotionTimeSeries.Add(EmotionInstance);

            return true;
        }

        private async Task<Emotion[]> UploadAndDetectEmotions(String path)
        {
            // Need valid subscription key
            string subscriptionKey = "32bd5bb40f534094a06e7c3894b0d6a6";

            EmotionServiceClient emotionServiceClient = new EmotionServiceClient(subscriptionKey);
            try
            {
                Emotion[] emotionResult;

                using (Stream imageFileStream = File.OpenRead(path))
                {
                    emotionResult = await emotionServiceClient.RecognizeAsync(imageFileStream);

                    return emotionResult;
                }

            }
            catch (Exception exception)
            {
                Debug.WriteLine("Exception", exception.ToString());
                return null;

            }


        }

        private async void theWebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            LoadingProcessProgressRing.IsActive = false;
            LoadingProcessProgressRing.Visibility = Visibility.Collapsed;
            all.Visibility = Visibility.Visible;
            theWebView.Visibility = Visibility.Visible;

            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync();

            while (recording)
            {
                try
                {
                    // Take/ process pictures
                    await cam_Snap();
                }
                catch (InvalidOperationException InvalidOperationException)
                {
                    Debug.WriteLine(InvalidOperationException.Message);
                }
                catch (NullReferenceException NullReferenceException)
                {
                    Debug.WriteLine(NullReferenceException.Message);
                }
                catch (UnauthorizedAccessException UnauthorizedAccessException)
                {
                    Debug.WriteLine(UnauthorizedAccessException.Message);
                }
                catch (COMException COMException)
                {
                    recording = false;
                }

            }
        }

        private void AppBarButtonLike_Click(object sender, RoutedEventArgs e)
        {
            Like = true;
            Dislike = false;
            lik.Foreground = new SolidColorBrush(Color.FromArgb(255, 207, 0, 15));
            dis.Foreground = new SolidColorBrush(Color.FromArgb(255, 116, 116, 116));
            var story = liked.Resources["ArticleLiked"] as Storyboard;
            if (story != null)
            {
                liked.Visibility = Visibility.Visible;
                story.Begin();
                story.Completed += LikeAnimate_Complete;
            }

        }

        private void LikeAnimate_Complete(object sender, object e)
        {
            liked.Visibility = Visibility.Collapsed;
            disliked.Visibility = Visibility.Collapsed;
            
        }

        private void AppBarButtonDislike_Click(object sender, RoutedEventArgs e)
        {
            Dislike = true;
            Like = false;
            dis.Foreground = new SolidColorBrush(Color.FromArgb(255,207,0,15) );
            lik.Foreground = new SolidColorBrush(Color.FromArgb(255, 116, 116, 116));
            
            var story = disliked.Resources["ArticleDisliked"] as Storyboard;
            if (story != null)
            {
                disliked.Visibility = Visibility.Visible;
                story.Begin();
                story.Completed += LikeAnimate_Complete;
            }
            
        }

        private int GetWordCount(string document)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(document);

            string docutext = doc.DocumentNode.InnerText;

            MatchCollection collection = Regex.Matches(docutext, @"[\S]+");
            return collection.Count;

        }

        private EmotionInstance CalculateEmotionMetric()
        {
            EmotionInstance AggregatedScore = new EmotionInstance();

            for (int i = 0; i < EmotionTimeSeries.Count; i++)
            {
                AggregatedScore.Anger += EmotionTimeSeries[i].AngerScore;
                AggregatedScore.Contempt += EmotionTimeSeries[i].ContemptScore;
                AggregatedScore.Disgust += EmotionTimeSeries[i].DisgustScore;
                AggregatedScore.Fear += EmotionTimeSeries[i].FearScore;
                AggregatedScore.Happiness += EmotionTimeSeries[i].HappinessScore;
                AggregatedScore.Neutral += EmotionTimeSeries[i].NeutralScore;
                AggregatedScore.Sadness += EmotionTimeSeries[i].SadnessScore;
                AggregatedScore.Surprise += EmotionTimeSeries[i].SurpriseScore;
            }

            AggregatedScore.Anger /= EmotionTimeSeries.Count;
            AggregatedScore.Contempt /= EmotionTimeSeries.Count;
            AggregatedScore.Disgust /= EmotionTimeSeries.Count;
            AggregatedScore.Fear /= EmotionTimeSeries.Count;
            AggregatedScore.Happiness /= EmotionTimeSeries.Count;
            AggregatedScore.Neutral /= EmotionTimeSeries.Count;
            AggregatedScore.Sadness /= EmotionTimeSeries.Count;
            AggregatedScore.Surprise /= EmotionTimeSeries.Count;

            return AggregatedScore;
        }


    }
}
