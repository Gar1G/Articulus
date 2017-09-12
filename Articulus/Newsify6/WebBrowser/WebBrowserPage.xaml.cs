using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Media.Capture;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Media.MediaProperties;
using Windows.UI.Popups;
using Newsify6.Common;
using Newsify6.Common.GlobalDatabase;
using Common;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Newsify6.WebBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebBrowserPage : Page
    {

        public WebBrowserPage()
        {
            this.InitializeComponent();
        }

        public Article CurrentArticle;

        EmotionTimeSeries EmotionTimeSeries = new EmotionTimeSeries();
        private MediaCapture _mediaCapture;
        public Emotion[] EmotionResult;
        DateTime Timestamp;
        bool Recording = true;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CurrentArticle = e.Parameter as Article;
            theWebView.Navigate(new Uri(CurrentArticle.Url));

            theWebView.Settings.IsJavaScriptEnabled = true;

            LoadingProcessProgressRing.IsActive = true;
            LoadingProcessProgressRing.Visibility = Visibility.Visible;
            theWebView.Visibility = Visibility.Collapsed;

        }

        private void GoBackNav(object sender, RoutedEventArgs e)
        {
            Recording = false;
            DateTime DateTime = DateTime.Now;
            EmotionInstance EmotionInstance = CalculateEmotionMetric();
            //GlobalDatabaseAPI.POSTUserMainTagEmotionData(CurrentArticle, DateTime, EmotionInstance);
            //GlobalDatabaseAPI.POSTUserSubTagEmotionData(CurrentArticle, DateTime, EmotionInstance);
            App.NavService.NavigateTo(typeof(MainPage), null);

        }

        private async void theWebView_LoadCompleted(object sender, NavigationEventArgs e)
        {

            _mediaCapture = new MediaCapture();
            try
            {
                await _mediaCapture.InitializeAsync();
            }
            catch(UnauthorizedAccessException UnauthorizedAccessException)
            {
                Debug.WriteLine(UnauthorizedAccessException.Message);
                var Dialog = new MessageDialog("Webcam not authorized for Access");
                await Dialog.ShowAsync();

            }

            LoadingProcessProgressRing.IsActive = false;
            LoadingProcessProgressRing.Visibility = Visibility.Collapsed;
            theWebView.Visibility = Visibility.Visible;

            while (Recording)
            {
                try
                {
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
            }

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
            //EmotionInstance.ArticleID = CurrentArticle.GetArticleID();
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

        private EmotionInstance CalculateEmotionMetric()
        {
            EmotionInstance AggregatedScore = new EmotionInstance();

            for(int i = 0; i < EmotionTimeSeries.Count; i++)
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
