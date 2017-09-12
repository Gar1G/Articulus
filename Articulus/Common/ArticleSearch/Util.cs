using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.UI.Popups;
using System.Diagnostics;
using Newsify6.Common.ArticleSearch.Classes.NewsSearch;
using Newsify6.Common.ArticleSearch.Classes.ImageSearch;

namespace Newsify6.Common.ArticleSearch
{
    public static class Util
    {
        #region Constants
        private const string _googleTrendingWebite = "http://www.google.com/trends/hottrends/atom/feed?pn=p";
        private const string _azureBaseUrl = "https://bingapis.azure-api.net/";
        private const string _bingNewsSearchAPI = "https://bingapis.azure-api.net/api/v5/news/search/?q=";
        private const string _bingImageSearchAPI = "https://bingapis.azure-api.net/api/v5/images/search?q=";
        private const string _cognitiveServicesBaseUrl = "https://westus.api.cognitive.microsoft.com/";
        private const string _keyPhrasesUri = "text/analytics/v2.0/keyPhrases";
        #endregion

        #region Private Utility Function
        /// <summary>
        /// Extracts trending data from google XML document
        /// </summary>
        /// <param name="NumberOfTrendingWords"></param>
        /// <param name="TrendsXML"></param>
        /// <returns></returns>
        private static async Task<List<string>> ExtractTrendingWordsFromXMLDoc(int NumberOfTrendingWords, string TrendsXML)
        {
            try
            {
                var doc = XDocument.Parse(TrendsXML);

                // Extract first n (number) keywords from JSON data
                List<string> tags = new List<string>();
                foreach (var item in doc.Root.Elements("channel").
                    Descendants("item").Take(NumberOfTrendingWords))
                {
                    tags.Add((string)item.Element("title"));
                }

                return tags;
            }
            catch (Exception)
            {
                var dialog = new MessageDialog("Error Fetching Trend Data.");
                await dialog.ShowAsync();
                return null;
            }
        }

        /// <summary>
        /// Returns the integer value for each area code. Interger Value appended to google trending website
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        private static int GetAreaCode(string Location)
        {
            return 9;

        }

        /// <summary>
        /// Uses Bing Search API to find articles in the given list of Websites
        /// </summary>
        /// <param name="SearchAccountKey"></param>
        /// <param name="NumOfArticles"></param>
        /// <param name="Sources"></param>
        /// <param name="SearchPhrase"></param>
        /// <param name="Freshness"></param>
        /// <param name="Location"></param>
        /// <param name="SafeSearch"></param>
        /// <returns></returns>
        private static async Task<List<BingNewsSearchRootObject>> SearchSources
            (string SearchAccountKey, int NumOfArticles,
            List<string> Sources, string SearchPhrase, string Freshness, string Location, string SafeSearch)
        {
            // BingNewsSearchRootObject returns the result of 1 search
            // Need a list of these because we will be doing multiple searches
            List<BingNewsSearchRootObject> RootNewsObjectList = new List<BingNewsSearchRootObject>();


            // Share article gathrering between sources
            int NumOfArticlesForSource = (int)(NumOfArticles / Sources.Count());
            int AddtionalArticlesForLastSource = NumOfArticles % Sources.Count;

            // Initialise Object that will be used for all searches
            // Prevents object creation on each iteration of the for loop
            BingNewsSearchRootObject RootNewsObject;

            // Get articles for all sources except 1
            for (int i = 0; i < Sources.Count - 1; i++)
            {
                string Query = "Site:" + Sources[i] + " " + SearchPhrase;
                RootNewsObject = await SearchRequest(SearchAccountKey, Query, Freshness, NumOfArticlesForSource, 0, Location, SafeSearch);
                RootNewsObjectList.Add(RootNewsObject);
            }

            // Get articles from the last source
            string LastQuery = "Site:" + Sources[Sources.Count - 1] + " " + SearchPhrase;
            RootNewsObject = await SearchRequest(SearchAccountKey, LastQuery, Freshness, NumOfArticlesForSource + AddtionalArticlesForLastSource, 0, Location, SafeSearch);
            RootNewsObjectList.Add(RootNewsObject);

            return RootNewsObjectList;
        }

        /// <summary>
        /// Turns the BingNewsSearchRootObject into a List of Searched Articles
        /// </summary>
        /// <param name="SearchAccountKey"></param>
        /// <param name="TextAccountKey"></param>
        /// <param name="RootNewsObjectList"></param>
        /// <returns></returns>
        private static async Task<List<SearchedArticle>> GenerateArticleList
            (string SearchAccountKey, string TextAccountKey, 
            List<BingNewsSearchRootObject> RootNewsObjectList)
        {
            // Initialise the output list
            List<SearchedArticle> SearchedArticleOutputList = new List<SearchedArticle>();

            // Loop through all search response
            for (int j = 0; j < RootNewsObjectList.Count; j++)
            {
                // Sometimes RootNewsObject does not contain anything, i.e. the Search Failed.
                if (RootNewsObjectList[j].Value == null)
                    continue;

                for (int k = 0; k < RootNewsObjectList[j].Value.Count; k++)
                {
                    SearchedArticle SearchedArticle = new SearchedArticle();

                    // The way that the BingSearchAPI returns a result means that if the Property does not exist
                    // it will be set to null
                    SearchedArticle.Url = RootNewsObjectList[j].Value[k].Url;
                    SearchedArticle.Title = RootNewsObjectList[j].Value[k].Name;
                    SearchedArticle.MainTag = RootNewsObjectList[j].Value[k].Category;
                    SearchedArticle.Description = RootNewsObjectList[j].Value[k].Description;

                    try
                    {
                        // After all the information from the BingSearchAPI has been assigned, the
                        await SearchedArticle.ParseArticle(SearchAccountKey, TextAccountKey);

                        // Add the Article to output list
                        SearchedArticleOutputList.Add(SearchedArticle);
                    }
                    catch(NullReferenceException NullReferenceExpection)
                    {
                        Debug.WriteLine(NullReferenceExpection.Message);
                        Debug.WriteLine("Error in Parsing at: " + SearchedArticle.Url);
                    }
                    catch(HttpRequestException HttpRequestException)
                    {
                        Debug.WriteLine(HttpRequestException.Message);
                        Debug.WriteLine("Error in Parsing at: " + SearchedArticle.Url);
                    }
                }
            }

            return SearchedArticleOutputList;
        }

        /// <summary>
        /// Main searching function, returns a RootNewsObject which contains List of Values
        /// </summary>
        /// <param name="AccountKey"></param>
        /// <param name="Query"></param>
        /// <param name="Freshness"></param>
        /// <param name="NumberOfArticles"></param>
        /// <param name="Offset"></param>
        /// <param name="Market"></param>
        /// <param name="SafeSearch"></param>
        /// <returns></returns>
        private static async Task<BingNewsSearchRootObject> SearchRequest
            (string AccountKey, string Query, string Freshness, int NumberOfArticles,
            int Offset, string Market, string SafeSearch)
        {
            using (HttpClient HttpClient = new HttpClient())
            {
                HttpClient.BaseAddress = new Uri(_azureBaseUrl);
                HttpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AccountKey);

                byte[] ByteData = Encoding.UTF8.GetBytes("");

                var Uri = (_bingNewsSearchAPI + WebUtility.HtmlEncode(Query) +
                    "&Freshness=" + Freshness + "&count=" + NumberOfArticles.ToString() +
                    "&offset=" + Offset.ToString() + "&mkt=" + Market + "&safesearch=" + SafeSearch);

                var Response = await HttpClient.GetAsync(Uri);

                var ResponseResult = Response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<BingNewsSearchRootObject>(ResponseResult.Result);
            }
        }
        #endregion

        /// <summary>
        /// Main function called to obtain articles
        /// </summary>
        /// <param name="SearchAccountKey"></param>
        /// <param name="TextAccountKey"></param>
        /// <param name="NumOfArticles"></param>
        /// <param name="MainTopic"></param>
        /// <param name="SubTopic"></param>
        /// <param name="Sources"></param>
        /// <param name="Location"></param>
        /// <returns></returns>
        public static async Task<List<SearchedArticle>> GetArticles
            (string SearchAccountKey, string TextAccountKey, 
            int NumOfArticles, string SearchPhrase, List<string> Sources, string Location)
        {

            // Exit if invalid search
            if (SearchPhrase == null || NumOfArticles == 0|| Sources == null || Sources.Capacity == 0)
                return null;

            // How far back to go to look for articles
            // Default setting is a week
            string Freshness = "week";        
            string SafeSearch = "moderate";  

            List<BingNewsSearchRootObject> RootNewsObjectList = await SearchSources(SearchAccountKey, NumOfArticles, Sources, SearchPhrase, Freshness, Location, SafeSearch);

            return await GenerateArticleList(SearchAccountKey, TextAccountKey, RootNewsObjectList);
        }

        /// <summary>
        /// Return list of trending keywords based on your Location
        /// </summary>
        /// <param name="NumberOfTrendingWords"></param>
        /// <param name="Location"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetTrendingKeyWords(int NumberOfTrendingWords, string Location)
        {
            using (HttpClient HttpClient = new HttpClient())
            {
                // get area code from location
                int areaCode = GetAreaCode(Location);
                // set max limit number of articles
                NumberOfTrendingWords = (NumberOfTrendingWords > 30) ? 30 : NumberOfTrendingWords;
                string url = _googleTrendingWebite + areaCode.ToString();
                string TrendsXML = await HttpClient.GetStringAsync(url);
                return await ExtractTrendingWordsFromXMLDoc(NumberOfTrendingWords, TrendsXML);
            }
        }
    }

}
