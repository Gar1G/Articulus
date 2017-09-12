using Newsify6.Common.Embedly;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Diagnostics;
using Newsify6.Common.LocalDatabase;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace Newsify6.Common
{
    public class NewsFeedArticle : LocalDatabaseArticle, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ArticleType ArticleType { get; set; }

        private List<string> _bestSubTags = new List<string>();
        public List<string> BestSubTags
        {
            get { return _bestSubTags; }
            set
            {
                _bestSubTags = value;
                BestSubTagsJsonString = _bestSubTags != null ?
                    JsonConvert.SerializeObject(_bestSubTags) :
                    string.Empty;
            }
        }

        private double b_o_value;
        public double ButtonOpacity
        {
            get { return this.b_o_value; }
            set { this.b_o_value = value; NotifyPropertyChanged(); }
        }

        private List<string> _embedlyEntities = new List<string>();
        public List<string> EmbedlyEntities
        {
            get { return _embedlyEntities; }
            set
            {
                _embedlyEntities = value;
                EmbedlyEntitiesJsonString = _embedlyEntities != null ?
                    JsonConvert.SerializeObject(_embedlyEntities) :
                    string.Empty;
            }
        }

        private List<string> _embedlyKeywords = new List<string>();
        public List<string> EmbedlyKeywords
        {
            get { return _embedlyKeywords; }
            set
            {
                _embedlyKeywords = value;
                EmbedlyKeywordsJsonString = _embedlyKeywords != null ?
                    JsonConvert.SerializeObject(_embedlyKeywords) :
                    string.Empty;
            }
        }

        private double o_value;
        public double Opacity
        {
            get { return this.o_value; }
            set { this.o_value = value; NotifyPropertyChanged(); }
        }

        private List<string> _subTags = new List<string>();
        public List<string> SubTags
        {
            get { return _subTags; }
            set
            {
                _subTags = value;
                SubTagsJsonString = _subTags != null ?
                    JsonConvert.SerializeObject(_subTags) :
                    string.Empty;
            }
        }

        private DateTime _dateClicked;
        public DateTime DateClicked
        {
            get { return _dateClicked; }
            set { _dateClicked = value; }
        }

        #region GetSubTags
        /// <summary>
        /// Generate appropriate list Subtags for the Article
        /// The updated list of Subtags will be stored in 
        /// </summary>
        public async Task<bool> GetSubTags()
        {
            if (Url.Contains("bing"))
                CorrectUrl();
            HttpClient HttpClient = new HttpClient();
            try
            {
                if (SubTags != null && SubTags.Count != 0)
                    SubTags = SubTags.Select(d => d.ToLower().Trim()).ToList();

                var JsonResponse = await HttpClient.GetStringAsync(
                     "https://api.embedly.com/1/extract?key=1c12c0c51f564711a3e56b03414941f4&url=" +
                     Url +
                     "&format=json");

                EmbedlyArticle EmbedlyArticle = JsonConvert.DeserializeObject<EmbedlyArticle>(JsonResponse);

                EmbedlyKeywords = GetEmbedlyKeywords(EmbedlyArticle);

                EmbedlyEntities = GetEmbedlyEntities(EmbedlyArticle);

                List<string> IntersectionAllThree = new List<string>();

                if (SubTags != null && SubTags.Capacity != 0 &&
                    EmbedlyKeywords != null && EmbedlyKeywords.Capacity != 0 &&
                    EmbedlyKeywords != null && EmbedlyKeywords.Capacity != 0)
                    IntersectionAllThree = SubTags.Intersect(EmbedlyKeywords).ToList().Intersect(EmbedlyEntities).ToList();

                List<string> IntersectionEmbedlyEntitiesKeywords = new List<string>();

                if (EmbedlyEntities != null && EmbedlyEntities.Count != 0 &&
                    EmbedlyKeywords != null && EmbedlyKeywords.Capacity != 0)
                    IntersectionEmbedlyEntitiesKeywords = EmbedlyEntities.Intersect(EmbedlyKeywords).ToList();

                List<string> IntersectionInSubTags = GetSubTagsWithIntersection(IntersectionEmbedlyEntitiesKeywords);
                List<string> SubTagsWithKeywords = GetSubTagsWithKeywords();
                List<string> SubTagsWithEntities = GetSubTagsWithEntities();

                var allProducts = IntersectionInSubTags
                                    .Concat(SubTagsWithEntities)
                                    .Concat(SubTagsWithKeywords)
                                    .ToList();

                List<string> MostCommonlyOccuring = null;

                try
                {
                    if (allProducts != null)
                        MostCommonlyOccuring = (from i in allProducts
                                                group i by i into grp
                                                orderby grp.Count() descending
                                                select grp.Key).ToList();
                }
                catch (ArgumentException ArgumentException)
                {
                    Debug.WriteLine(ArgumentException.Message);
                    Debug.WriteLine(Url);
                }

                List<string> TempBestSubTags = new List<string>();

                if (MostCommonlyOccuring != null && MostCommonlyOccuring.Capacity != 0)
                    TempBestSubTags.Add(MostCommonlyOccuring[0]);
                if (IntersectionEmbedlyEntitiesKeywords != null && IntersectionEmbedlyEntitiesKeywords.Capacity != 0)
                    TempBestSubTags.Add(IntersectionEmbedlyEntitiesKeywords[0]);
                if (IntersectionAllThree != null && IntersectionAllThree.Capacity != 0)
                    TempBestSubTags.Add(IntersectionAllThree[0]);

                if (TempBestSubTags.Count < 3 && MostCommonlyOccuring != null &&
                    MostCommonlyOccuring.Capacity != 0 && MostCommonlyOccuring.Count > 2)
                    TempBestSubTags.Add(MostCommonlyOccuring[1]);

                if (TempBestSubTags != null && TempBestSubTags.Capacity != 0)
                    DecodeSubTags(TempBestSubTags);

                BestSubTags = TempBestSubTags;

                return true;

            }
            catch (HttpRequestException HttpRequestExeption)
            {
                Debug.WriteLine(HttpRequestExeption.Message);
                Debug.WriteLine(Url);

                return false;
            }
        }

        /// <summary>
        /// Turns subtags that have Html codes into proper strings
        /// </summary>
        private void DecodeSubTags(List<string> TempBestSubTags)
        {
            foreach (string EmbedlySubTag in TempBestSubTags)
            {
                EmbedlySubTag.Replace("&amp", "&");
            }
        }

        /// <summary>
        /// Finds the subtags that are similar to EmbedlyEntities
        /// </summary>
        /// <returns>List of string contain subtags that are similar to EmbedlyEntities</returns>
        private List<string> GetSubTagsWithEntities()
        {
            List<string> EntitiesInSubtags = new List<string>();

            if (SubTags != null && SubTags.Capacity != 0 && EmbedlyEntities != null && EmbedlyEntities.Capacity != 0)
                foreach (var Entity in EmbedlyEntities)
                    foreach (var Subtag in SubTags)
                        if (Subtag.Contains(Entity))
                            EntitiesInSubtags.Add(Subtag);

            return EntitiesInSubtags;
        }

        /// <summary>
        /// Finds the subtags that are similar to EmbedlyKeywords
        /// </summary>
        /// <returns>List of string contain subtags that are similar to EmbedlyKeywords</returns>
        private List<string> GetSubTagsWithKeywords()
        {
            List<string> KeywordsInSubTags = new List<string>();

            if (SubTags != null && SubTags.Capacity != 0 && EmbedlyKeywords != null && EmbedlyKeywords.Capacity != 0)
                foreach (var Keyword in EmbedlyKeywords)
                    foreach (var Subtag in SubTags)
                        if (Subtag.Contains(Subtag))
                            KeywordsInSubTags.Add(Subtag);

            return KeywordsInSubTags;
        }

        /// <summary>
        /// Finds the subtags that are similar to both EmbedlyEntities and EmbedlyKeywords
        /// </summary>
        /// <param name="IntersectionEmbedlyEntitiesKeywords">List of string that contains words that are part of both EmbedlyKeywords and EmebedlyEntities</param>
        /// <returns>List of string contains subtags that are similar to both EmbedlyEntities and EmbedlyKeywords</returns>
        private List<string> GetSubTagsWithIntersection(List<string> IntersectionEmbedlyEntitiesKeywords)
        {
            List<string> IntersectionInSubTags = new List<string>();

            if (SubTags != null && SubTags.Count != 0)
            {
                foreach (var intersectionWord in IntersectionEmbedlyEntitiesKeywords)
                {
                    foreach (var Subtag in SubTags)
                    {
                        if (Subtag.Contains(intersectionWord))
                            IntersectionInSubTags.Add(Subtag);
                    }
                }
            }

            return IntersectionInSubTags;
        }

        /// <summary>
        /// Extracts Embedly Entities from Embedly Article
        /// </summary>
        /// <param name="EmbedlyArticle">The Embedly Article from which entities have to extracted</param>
        /// <returns></returns>
        private static List<string> GetEmbedlyEntities(EmbedlyArticle EmbedlyArticle)
        {
            List<string> EmbedlyEntities = new List<string>();

            if (EmbedlyArticle.Entities != null && EmbedlyArticle.Entities.Count != 0)
            {
                for (int i = 0; i < EmbedlyArticle.Entities.Count; i++)
                {
                    EmbedlyEntities.Add(EmbedlyArticle.Entities[i].Name.ToLower());
                }
            }

            return EmbedlyEntities;
        }

        /// <summary>
        /// Extracts Embedly Keywords from Embedly Article
        /// </summary>
        /// <param name="EmbedlyArticle">The Embedly Article from which keywords have to be extracted</param>
        /// <returns></returns>
        private static List<string> GetEmbedlyKeywords(EmbedlyArticle EmbedlyArticle)
        {
            List<string> EmbedlyKeywords = new List<string>();

            if (EmbedlyArticle.Keywords != null && EmbedlyArticle.Keywords.Count != 0)
            {
                for (int i = 0; i < EmbedlyArticle.Keywords.Count; i++)
                {
                    EmbedlyKeywords.Add(EmbedlyArticle.Keywords[i].Name.ToLower());
                }
            }

            return EmbedlyKeywords;
        }


        #endregion


        #region Constructors
        public NewsFeedArticle() { }

        public NewsFeedArticle(LocalDatabaseArticle LocalDatabaseArticle)
        {
            foreach (PropertyInfo Property in LocalDatabaseArticle.GetType().GetProperties())
                GetType().GetProperty(Property.Name).SetValue(this, Property.GetValue(LocalDatabaseArticle));

            BestSubTags = JsonConvert.DeserializeObject<List<string>>(BestSubTagsJsonString);
            EmbedlyEntities = JsonConvert.DeserializeObject<List<string>>(EmbedlyEntitiesJsonString);
            EmbedlyKeywords = JsonConvert.DeserializeObject<List<string>>(EmbedlyKeywordsJsonString);
            SubTags = JsonConvert.DeserializeObject<List<string>>(SubTagsJsonString);
        }
        #endregion
    }
}
