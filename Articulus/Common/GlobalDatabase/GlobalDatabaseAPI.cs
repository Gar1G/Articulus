using Common;
using Newsify6.Common.LocalDatabase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Newsify6.Common.GlobalDatabase
{
    public static class GlobalDatabaseAPI
    {
        private const string _baseUrl = "http://poweb.azurewebsites.net/";
        private const string _articleData = "api/ArticleDatas";
        private const string _userArticleData = "api/UserArticleInfos";
        private const string _userArticleHistory = "api/UserArticleInfos?Email=";
        private const string _userSubTagEmotionData = "api/UserSubTagEmotionDatas";
        private const string _userMainTagEmotionData = "api/UserMainTagEmotionDatas";
        private const string _userArticleEmotionTimeSeries = "api/UserArticleEmotionTimeSeries";
        private const string _userLoginInformationGet = "api/UserLoginInformations?Email=";
        private const string _userLoginInformationPost = "api/UserLoginInformations";
        private const string _userTagGet = "api/GetUserTopicsPreferred?Email=";


        //public static void Logout()
        //{
        //    Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        //    Windows.Storage.ApplicationDataCompositeValue composite = (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["CurrentUserLogin"];
        //    localSettings.Values.Remove("CurrentUserLogin");
        //    //App.NavService.NavigateTo(typeof(Login.LoginPage), null);
        //}

        public static async void POSTArticleData(LocalDatabaseArticle LocalDatabaseArticle)
        {
            if (true)
            {
                HttpClient HttpClient = new HttpClient();
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserTokenInfo.GetInstance.access_token);
                StringContent HttpContent = new StringContent(JsonConvert.SerializeObject(new ArticleData(LocalDatabaseArticle)));
                HttpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage HttpResponseMessage = await HttpClient.PostAsync(_baseUrl + _articleData, HttpContent);
            }
            else
            {
                //Logout();
            }
        }

        public static async void POSTUserArticleData(NewsFeedArticle Article, bool? LikeDislike, int Duration)
        {
            if (UserTokenInfo.GetInstance.isTokenValid())
            {
                HttpClient HttpClient = new HttpClient();
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserTokenInfo.GetInstance.access_token);

                UserArticleData UserArticleData = new UserArticleData()
                {
                    Email = UserTokenInfo.GetInstance.Email,
                    ArticleID = Article.ArticleId,
                    Duration = Duration,
                    LikeDislike = JsonConvert.SerializeObject(LikeDislike),
                    TimeStamp = DateTime.Now
                };

                StringContent HttpContent = new StringContent(JsonConvert.SerializeObject(UserArticleData));
                HttpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage HttpResponseMessage = await HttpClient.PostAsync(_baseUrl + _userArticleData, HttpContent);
            }
            else
            {
                //Logout();
            }
        }

        public static async void POSTUserMainTagEmotionData(NewsFeedArticle Article, DateTime DateTime, EmotionInstance AggregatedScore)
        {
            if (UserTokenInfo.GetInstance.isTokenValid())
            {
                HttpClient HttpClient = new HttpClient();
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserTokenInfo.GetInstance.access_token);

                UserMainTagEmotionData UserMainTagEmotionData = new UserMainTagEmotionData()
                {
                    Email = UserTokenInfo.GetInstance.Email,
                    ArticleID = Article.ArticleId,
                    MainTag = Article.MainTag,
                    Timestamp = DateTime
                };

                UserMainTagEmotionData.AngerScore = AggregatedScore.Anger;
                UserMainTagEmotionData.ContemptScore = AggregatedScore.Contempt;
                UserMainTagEmotionData.DisgustScore = AggregatedScore.Disgust;
                UserMainTagEmotionData.FearScore = AggregatedScore.Fear;
                UserMainTagEmotionData.HappinessScore = AggregatedScore.Happiness;
                UserMainTagEmotionData.NeutralScore = AggregatedScore.Neutral;
                UserMainTagEmotionData.SadnessScore = AggregatedScore.Sadness;
                UserMainTagEmotionData.SurpriseScore = AggregatedScore.Surprise;

                StringContent HttpContent = new StringContent(JsonConvert.SerializeObject(UserMainTagEmotionData));
                HttpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage HttpResponseMessage = await HttpClient.PostAsync(_baseUrl + _userMainTagEmotionData, HttpContent);
            }
            else
            {
                //Logout();
            }
        }

        public static async void POSTUserSubTagEmotionData(NewsFeedArticle Article, DateTime DateTime, EmotionInstance AggregatedScore)
        {
            if (UserTokenInfo.GetInstance.isTokenValid())
            {
                foreach(string subtag in Article.BestSubTags)
                {
                    HttpClient HttpClient = new HttpClient();
                    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserTokenInfo.GetInstance.access_token);

                    UserSubTagEmotionData UserSubTagEmotionData = new UserSubTagEmotionData()
                    {
                        Email = UserTokenInfo.GetInstance.Email,
                        ArticleID = Article.ArticleId,
                        MainTag = Article.MainTag,
                        SubTag = subtag,
                        Timestamp = DateTime
                    };

                    UserSubTagEmotionData.AngerScore = AggregatedScore.Anger;
                    UserSubTagEmotionData.ContemptScore = AggregatedScore.Contempt;
                    UserSubTagEmotionData.DisgustScore = AggregatedScore.Disgust;
                    UserSubTagEmotionData.FearScore = AggregatedScore.Fear;
                    UserSubTagEmotionData.HappinessScore = AggregatedScore.Happiness;
                    UserSubTagEmotionData.NeutralScore = AggregatedScore.Neutral;
                    UserSubTagEmotionData.SadnessScore = AggregatedScore.Sadness;
                    UserSubTagEmotionData.SurpriseScore = AggregatedScore.Surprise;

                    StringContent HttpContent = new StringContent(JsonConvert.SerializeObject(UserSubTagEmotionData));
                    HttpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage HttpResponseMessage = await HttpClient.PostAsync(_baseUrl + _userSubTagEmotionData, HttpContent);
                }  
            }
            else
            {
                //Logout();
            }
        }

        public static async void POSTUserArticleEmotionTimeSeries(UserArticleEmotionTimeSeries UserArticleEmotionTimeSeries)
        {
            if (UserTokenInfo.GetInstance.isTokenValid())
            {
                HttpClient HttpClient = new HttpClient();
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserTokenInfo.GetInstance.access_token);

                StringContent HttpContent = new StringContent(JsonConvert.SerializeObject(UserArticleEmotionTimeSeries));
                HttpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage HttpResponseMessage = await HttpClient.PostAsync(_baseUrl + _userArticleEmotionTimeSeries, HttpContent);
            }
            else
            {
                //Logout();
            }
        }

        public static async Task<bool> UpdateUserTokenInfo(string Email)
        {
            try
            {
                UserLoginInformation UserLoginInformation = await GETUserLoginInformation(Email);

                UserTokenInfo.GetInstance.FirstName = UserLoginInformation.FirstName;
                UserTokenInfo.GetInstance.SecondName = UserLoginInformation.SecondName;

                if (UserLoginInformation.TopicJsonStringList != null)
                    UserTokenInfo.GetInstance.Topic = JsonConvert.DeserializeObject<List<string>>(UserLoginInformation.TopicJsonStringList);
                else
                    UserTokenInfo.GetInstance.Topic = new List<string>();

                if (UserLoginInformation.WebsiteJsonStringList != null)
                    UserTokenInfo.GetInstance.Website = JsonConvert.DeserializeObject<List<string>>(UserLoginInformation.WebsiteJsonStringList);
                else
                    UserTokenInfo.GetInstance.Website = new List<string>();
            }
            catch (HttpRequestException HttpRequestException)
            {
                Debug.WriteLine(HttpRequestException.Message);
                return false;
            }
            
            return true;
        }

        private static async Task<UserLoginInformation> GETUserLoginInformation(string Email)
        {
            if (UserTokenInfo.GetInstance.isTokenValid())
            {
                HttpClient HttpClient = new HttpClient();
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserTokenInfo.GetInstance.access_token);

                string JsonResponse = await HttpClient.GetStringAsync(_baseUrl + _userLoginInformationGet + Email);

                return JsonConvert.DeserializeObject<UserLoginInformation>(JsonResponse);
            }
            else
            {
                //Logout();
                return null;
            }
        }

        public static async Task<UserTags> GETUserTags(string Email)
        {
            if (UserTokenInfo.GetInstance.isTokenValid())
            {
                HttpClient HttpClient = new HttpClient();
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserTokenInfo.GetInstance.access_token);

                string JsonResponse = await HttpClient.GetStringAsync(_baseUrl + _userTagGet + Email);

                return JsonConvert.DeserializeObject<UserTags>(JsonResponse);
            }
            else
            {
                //Logout();
                return null;
            }
        }


        public static async Task<List<GlobalDatabaseHistoryArticle>> GETUserHistory(string Email)
        {
            if (UserTokenInfo.GetInstance.isTokenValid())
            {
                HttpClient HttpClient = new HttpClient();
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserTokenInfo.GetInstance.access_token);

                string JsonResponse = await HttpClient.GetStringAsync(_baseUrl + _userArticleHistory + Email);

                return JsonConvert.DeserializeObject<List<GlobalDatabaseHistoryArticle>>(JsonResponse);
            }
            else
            {
                //Logout();
                return null;
            }
        }

        public static async void POSTUserLoginInformation()
        {
            if (UserTokenInfo.GetInstance.isTokenValid())
            {
                UserLoginInformation UserLoginInformation = new UserLoginInformation();
                UserLoginInformation.FirstName = UserTokenInfo.GetInstance.FirstName;
                UserLoginInformation.SecondName = UserTokenInfo.GetInstance.SecondName;
                UserLoginInformation.Email = UserTokenInfo.GetInstance.Email;

                if (UserTokenInfo.GetInstance.Topic != null && UserTokenInfo.GetInstance.Topic.Capacity != 0)
                    UserLoginInformation.TopicJsonStringList = JsonConvert.SerializeObject(UserTokenInfo.GetInstance.Topic);
                if (UserTokenInfo.GetInstance.Website != null && UserTokenInfo.GetInstance.Website.Capacity != 0)
                    UserLoginInformation.WebsiteJsonStringList = JsonConvert.SerializeObject(UserTokenInfo.GetInstance.Website);

                HttpClient HttpClient = new HttpClient();
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserTokenInfo.GetInstance.access_token);

                StringContent HttpContent = new StringContent(JsonConvert.SerializeObject(UserLoginInformation));
                HttpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage HttpResponseMessage = await HttpClient.PostAsync(_baseUrl + _userLoginInformationPost, HttpContent);
            }
            else
            {
                //Logout();
            }
        }
    }
}
