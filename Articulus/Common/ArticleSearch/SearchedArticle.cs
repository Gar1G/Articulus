using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newsify6.Common.LocalDatabase;
using Newsify6.Common.ArticleSearch.Classes.ImageSearch;

namespace Newsify6.Common.ArticleSearch
{
    public class SearchedArticle : Article
    {
        public HtmlDocument HtmlDoc { get; set; }
        public WebsiteNodeLocations NodeLocations { get; set; }

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

        #region Private Helper Functions For Parse Article
        /// <summary>
        /// Extracts the SubTags from Meta Data
        /// </summary>
        private void ExtractSubTags()
        {
            HtmlNode Parser = NodeLocations.Keywords != null ?
                HtmlDoc.DocumentNode.Descendants(NodeLocations.Keywords[0]).
                Where(x => x.GetAttributeValue(NodeLocations.Keywords[1], null) ==
                NodeLocations.Keywords[2]).FirstOrDefault() :
                null;

            SubTags = Parser != null && Parser.Attributes["content"] != null ? 
                Parser.Attributes["content"].Value.Replace("-", " ").Split(',').ToList() : SubTags;
        }

        /// <summary>
        /// Extracts the Publishing Date from Meta Data
        /// </summary>
        private void ExtractPubDate()
        {
            HtmlNode Parser = NodeLocations.PubDate != null ?
                HtmlDoc.DocumentNode.Descendants(NodeLocations.PubDate[0]).
                Where(x => x.GetAttributeValue(NodeLocations.PubDate[1], null) ==
                NodeLocations.PubDate[2]).FirstOrDefault() :
                null;

            PubDate = Parser != null && Parser.Attributes["content"] != null ? 
                Parser.Attributes["content"].Value : 
                string.Empty;
        }

        /// <summary>
        /// Extracts the Author from the Meta Data
        /// </summary>
        private void ExtractAuthor()
        {
            HtmlNode Parser = NodeLocations.Author != null ?
                HtmlDoc.DocumentNode.Descendants(NodeLocations.Author[0]).
                Where(x => x.GetAttributeValue(NodeLocations.Author[1], null) ==
                NodeLocations.Author[2]).FirstOrDefault() :
                null;

            if (Parser != null)
            {
                Author = NodeLocations.Author.Contains("meta") && Parser.Attributes["content"] != null ?
                    Parser.Attributes["content"].Value :
                    Parser.InnerText.Replace("By ", "");

                Author = Author == String.Empty || Author == " " ? string.Empty : Author;
            }
        }

        /// <summary>
        /// Extracts the Main Tag from the Meta Data
        /// </summary>
        private void ExtractMainTag()
        {
            HtmlNode Parser = NodeLocations.MainTag != null ?
                HtmlDoc.DocumentNode.Descendants(NodeLocations.MainTag[0]).
                Where(x => x.GetAttributeValue(NodeLocations.MainTag[1], null) ==
                NodeLocations.MainTag[2]).FirstOrDefault() :
                null;

            MainTag = Parser != null && Parser.Attributes["content"] != null ?
                Parser.Attributes["content"].Value :
                string.Empty;
        }

        /// <summary>
        /// Extracts Image Credits from Meta Data
        /// </summary>
        private void ExtractImageCredits()
        {
            HtmlNode Parser = NodeLocations.ImageCredit != null ?
                HtmlDoc.DocumentNode.Descendants(NodeLocations.ImageCredit[0]).
                Where(x => x.GetAttributeValue(NodeLocations.ImageCredit[1], null)
                == NodeLocations.ImageCredit[2]).FirstOrDefault() :
                null;

            ImageCredit = Parser != null && Parser.InnerHtml != null ?
                Regex.Replace(Parser.InnerText, @"\t|\n|\r", "") :
                string.Empty;
            
            TelegraphImageCreditFix(Parser);
        }

        /// <summary>
        /// Fix for Telegraph Image Credits
        /// </summary>
        /// <param name="Parser"></param>
        private void TelegraphImageCreditFix(HtmlNode Parser) =>
            ImageCredit = ImageCredit != null && Url.Contains("telegraph.co.uk") ?
            ImageCredit.Replace(Parser.FirstChild.InnerText, "").Replace("Credit:", "") :
            ImageCredit;

        /// <summary>
        /// Extract Image from Meta Data
        /// </summary>
        private void ExtractImageFromPage()
        {
            HtmlNode Parser = NodeLocations.Image != null ?
                HtmlDoc.DocumentNode.Descendants(NodeLocations.Image[0]).
                Where(x => x.GetAttributeValue(NodeLocations.Image[1], null)
                == NodeLocations.Image[2]).FirstOrDefault() :
                null;

            if (Parser != null)
            {
                if (NodeLocations.Image[0].Contains("meta") && Parser.Attributes["content"] != null)
                {
                    Image = NodeLocations.Image.Count() > 3 ?
                        Parser.Descendants(NodeLocations.Image[3]).
                        FirstOrDefault().Attributes["content"].Value :
                        Parser.Attributes["content"].Value;
                }
                else
                {
                    Image = Parser.Attributes["src"] != null ?
                        Parser.Attributes["src"].Value :
                        string.Empty;

                    Image = Parser.Attributes["width"] != null && Url.Contains("bbc.co.uk") && !Url.Contains("sport") ?
                        Image.Replace("/320/", "/" + Parser.Attributes["width"].Value + "/") :
                        string.Empty;
                }

                HuffingtonPostImageFix();
            }
        }

        /// <summary>
        /// Fix for HuffingtonPost Images
        /// </summary>
        private void HuffingtonPostImageFix() => 
            Image = Image != null && Image.Contains("DEFAULT") ? 
            string.Empty : 
            Image;

        /// <summary>
        /// Extract the Title from the MetaData
        /// </summary>
        private void ExtractTitleFromPage()
        {
            HtmlNode Parser = HtmlDoc.DocumentNode.Descendants
                (NodeLocations.Title[0]).Where(x =>
                x.GetAttributeValue(NodeLocations.Title[1], null)
                == NodeLocations.Title[2]).FirstOrDefault();

            if (Parser != null && Parser.Attributes["content"] != null)
                Title = NodeLocations.TitleReplace != null ?
                    Parser.Attributes["content"].Value.ToString().Replace(NodeLocations.TitleReplace, "") :
                    Parser.Attributes["content"].Value.ToString();
            else
                Title = string.Empty;
        }
        #endregion

        public async Task<bool> ParseArticle(string SearchAccountKey, string TextAccountKey)
        {
            if (Url == null)
                return false;

            // Get locations of the nodes to scrape
            NodeLocations = WebsiteNodeLocations.GetNodeLocations(Url);

            if (NodeLocations == null)
                return false;

            // Load HTML
            HtmlWeb HtmlWeb = new HtmlWeb();

            HtmlDoc = await HtmlWeb.LoadFromWebAsync(Url);

            ExtractTitleFromPage();

            ExtractImageFromPage();

            if(Image != null)
                ExtractImageCredits();

            ExtractMainTag();

            UpdateMainTag();

            ExtractSubTags();

            ExtractAuthor();

            ExtractPubDate();

            return true;
        }

        #region Update MainTags
        private List<String> MainTagList = new List<string>
        {
            "us",
            "world",
            "money",
            "sport",
            "politics",
            "opinion",
            "entertainment",
            "education",
            "arts",
            "books",
            "australia",
            "media",
            "scienceandtechnology",
            "music",
            "uk",
            "travel",
            "film",
            "business",
            "health",
            "fashion",
            "environment",
            "europe",
            "lifestyle",
            "americas",
            "eu referendum",
            "trending",
            "scotland",
            "asia",
            "people",
            "crime",
            "us elections",
            "reviews",
            "england",
            "wales",
            "northern ireland",
            "culture",
            "middle east"
        };

        /// <summary>
        /// Ensures that the Main Tag is one of the approved Main Tags
        /// </summary>
        private void UpdateMainTag()
        {
            if (MainTag == null || MainTag == string.Empty)
                FindMainTagInSubTag();
            else if (MainTagList.Contains(MainTag.ToLower()))
                MainTag = MainTagList[MainTagList.IndexOf(MainTag.ToLower())];
            else if (MainTag.ToLower().Contains("world") || MainTag.ToLower().Contains("global") || MainTag.ToLower().Contains("news"))
                MainTag = MainTagList[MainTagList.IndexOf("world")];
            else if (MainTag.ToLower().Contains("life") || MainTag.ToLower().Contains("style") || MainTag.ToLower().Contains("lifestyle"))
                MainTag = MainTagList[MainTagList.IndexOf("lifestyle")];
            else if (MainTag.ToLower().Contains("television") || MainTag.ToLower().Contains("radio") || MainTag.ToLower().Contains("stage"))
                MainTag = MainTagList[MainTagList.IndexOf("entertainment")];
            else if (MainTag.ToLower().Contains("science") || MainTag.ToLower().Contains("tech") || MainTag.ToLower().Contains("technology"))
                MainTag = MainTagList[MainTagList.IndexOf("scienceandtechnology")];
            else if (MainTag.ToLower().Contains("films"))
                MainTag = MainTagList[MainTagList.IndexOf("film")];
            else if (MainTag.ToLower().Contains("health"))
                MainTag = MainTagList[MainTagList.IndexOf("health")];
            else if (MainTag.ToLower().Contains("features") || MainTag.ToLower().Contains("on-demand"))
                MainTag = MainTagList[MainTagList.IndexOf("trending")];
            else if (MainTag.ToLower().Contains("voices"))
                MainTag = MainTagList[MainTagList.IndexOf("people")];
            else if (MainTag.ToLower().Contains("sports") || 
                MainTag.ToLower().Contains("football") || 
                MainTag.ToLower().Contains("boxing") ||
                MainTag.ToLower().Contains("cricket") ||
                MainTag.ToLower().Contains("rugby"))
                MainTag = MainTagList[MainTagList.IndexOf("sport")];
            else if (MainTag.ToLower().Contains("uk"))
                MainTag = MainTagList[MainTagList.IndexOf("uk")];
            else if (MainTag.ToLower().Contains("scotland"))
                MainTag = MainTagList[MainTagList.IndexOf("scotland")];
            else if (MainTag.ToLower().Contains("us"))
                MainTag = MainTagList[MainTagList.IndexOf("us")];
            else if (MainTag.ToLower().Contains("magazine"))
                MainTag = MainTagList[MainTagList.IndexOf("entertainment")];
            else if (MainTag.ToLower().Contains("wales"))
                MainTag = MainTagList[MainTagList.IndexOf("wales")];
            else if (MainTag.ToLower().Contains("books") || MainTag.ToLower().Contains("book"))
                MainTag = MainTagList[MainTagList.IndexOf("books")];
            else if (MainTag.ToLower().Contains("money") || MainTag.ToLower().Contains("finances") || MainTag.ToLower().Contains("finance"))
                MainTag = MainTagList[MainTagList.IndexOf("money")];
            else if (MainTag.ToLower().Contains("art"))
                MainTag = MainTagList[MainTagList.IndexOf("arts")];
            else if (MainTag.ToLower().Contains("culture"))
                MainTag = MainTagList[MainTagList.IndexOf("culture")];
            else if (MainTag.ToLower().Contains("teacher"))
                MainTag = MainTagList[MainTagList.IndexOf("education")];
            else if (MainTag.ToLower().Contains("society"))
                MainTag = MainTagList[MainTagList.IndexOf("people")];
            else if (MainTag.ToLower().Contains("fashion"))
                MainTag = MainTagList[MainTagList.IndexOf("fashion")];
            else if (MainTag.ToLower().Contains("middle east"))
                MainTag = MainTagList[MainTagList.IndexOf("middle east")];
            else if (MainTag.ToLower().Contains("australia"))
                MainTag = MainTagList[MainTagList.IndexOf("australia")];
            else
                FindMainTagInSubTag();
        }

        /// <summary>
        /// Tries to find an approved Main Tag within the Subtags Obtained
        /// </summary>
        private void FindMainTagInSubTag()
        {
            if (SubTags == null || SubTags.Count == 0)
                MainTag = string.Empty;
            else
            {
                for (int i = 0; i < SubTags.Count; i++)
                {
                    foreach (var VerifiedMainTag in MainTagList)
                    {
                        if (SubTags[i].ToLower().Contains(VerifiedMainTag))
                        {
                            MainTag = VerifiedMainTag;
                            return;
                        }
                    }
                }

                MainTag = string.Empty;
                return;
            }
        }
        #endregion
    }
}
