using SQLite.Net.Attributes;
using System;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;
using System.Net;
using System.Diagnostics;

namespace Newsify6.Common.LocalDatabase
{
    public class LocalDatabaseArticle : Article
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string ArticleId { get; set; } = string.Empty;
        public string BestSubTagsJsonString { get; set; } = string.Empty;
        public string EmbedlyEntitiesJsonString { get; set; } = string.Empty;
        public string EmbedlyKeywordsJsonString { get; set; } = string.Empty;
        public int WordCount { get; set; }

        #region Constructors
        public LocalDatabaseArticle() { }

        public LocalDatabaseArticle(Article Article)
        {
            foreach (PropertyInfo Property in Article.GetType().GetProperties())
            {
                if(GetType().GetProperty(Property.Name) != null)
                {
                    GetType().GetProperty(Property.Name).SetValue(this, Property.GetValue(Article));
                }    
            }
            
            CorrectUrl();
            SetArticleID();
            Author = WebUtility.HtmlDecode(Author);
            Description = WebUtility.HtmlDecode(Description);
            Image = WebUtility.HtmlDecode(Image);
            ImageCredit = WebUtility.HtmlDecode(ImageCredit);
            Title = WebUtility.HtmlDecode(Title);
            Title = Regex.Replace(Title, "&#039;", @"'");
            Title = Regex.Replace(Title, "&#x27;", @"'");
            Title = Title.Trim();
            SubTagsJsonString = WebUtility.HtmlDecode(SubTagsJsonString);
        }
        #endregion

        /// <summary>
        /// Edits the url returned by the Bing Search into the actual url
        /// </summary>
        protected void CorrectUrl()
        {
            Regex Regex = new Regex("1&r=https*.*&p=DevEx");
            string MatchedText = Regex.Match(Url).Value;
            MatchedText = Regex.Replace(MatchedText, "%3a", ":");
            MatchedText = Regex.Replace(MatchedText, "%2f", "/");
            MatchedText = Regex.Split(MatchedText, "&p=DevEx").First();
            MatchedText = Regex.Split(MatchedText, "1&r=").Last();
            Url = MatchedText;
        }

        #region Hashing
        public void SetArticleID() => ArticleId = Hash(Title);

        /// <summary>
        /// Returns the hashed version of the input string
        /// </summary>
        /// <param name="inputString">The string the needs to be hased</param>
        /// <returns></returns>
        private string Hash(string inputString)
        {
            IBuffer passwordBuffUtf8 = CryptographicBuffer.
                ConvertStringToBinary(inputString, BinaryStringEncoding.Utf8);

            HashAlgorithmProvider hashAlgorithmProvider =
                HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);

            IBuffer hasedInputString = hashAlgorithmProvider.HashData(passwordBuffUtf8);

            if (hasedInputString.Length != hashAlgorithmProvider.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }

            return CryptographicBuffer.EncodeToBase64String(hasedInputString);
        }
        #endregion

    }
}
