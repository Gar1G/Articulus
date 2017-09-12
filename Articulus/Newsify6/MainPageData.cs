using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Popups;
using Newsify6.Common;
using Newsify6.Common.LocalDatabase;
using System.Net;

namespace Newsify6
{
    public class MainPageData : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<NewsFeedArticle> _allArticles = new List<NewsFeedArticle>();
        private List<NewsFeedArticle> _filteredArticles = new List<NewsFeedArticle>();
        private int nextToDisplay = 0;

        public int count = 0;

        private string _filter;
        internal int maxCount;

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Filter)));
                PerformFiltering();
            }
        }

        public ObservableCollection<NewsFeedArticle> FilteredItems { get; set; } = new ObservableCollection<NewsFeedArticle>();

        public ObservableCollection<NewsFeedArticle> BookmarkedItems { get; set; } = new ObservableCollection<NewsFeedArticle>();

        private List<List<NewsFeedArticle>> NewsFeedArticlesHolder { get; set; } = new List<List<NewsFeedArticle>>();
        public ObservableCollection<NewsFeedArticle> NewsFeedArticles { get; set; } = new ObservableCollection<NewsFeedArticle>();

        public MainPageData()
        {
            Load();
        }

        /// <summary>
        /// Initialise load will put 24 Articles into NewsFeedArticles
        /// </summary>
        private async void Load()
        {
            LoadAllArticles();
            FilterArticles();
            Randomise();
            maxCount = _filteredArticles.Count / 24 - 1;

            if (_filteredArticles.Count < 24)
            {
                var Dialog = new MessageDialog("Too few Articles in Database");
                await Dialog.ShowAsync();
            }
            else
            {
                for (int i = 0; i < 24; i++)
                {
                    NewsFeedArticles.Add(_filteredArticles[i]);
                    nextToDisplay++;
                }

                PositionArticles();
                AssignArticleTypes();
                PerformFiltering();

                NewsFeedArticlesHolder.Add(new List<NewsFeedArticle>());

                foreach(NewsFeedArticle Article in NewsFeedArticles)
                {
                    NewsFeedArticlesHolder[0].Add(Article);
                }

            }
        }

        /// <summary>
        /// Loads _fullItems will all Articles in the local Sql database
        /// </summary>
        private void LoadAllArticles()
        {
            SQLite.Net.SQLiteConnection Conn = LocalDatabaseAPI.OpenLocalDatabaseArticleConnection();
            _allArticles = LocalDatabaseAPI.PullAllArticles(Conn);
            Conn.Close();
        }

        /// <summary>
        /// Simply randomise the list of Articles
        /// </summary>
        /// <param name="ListToRandomise">The list that needs to randomnised</param>
        private void Randomise()
        {
            Random rng = new Random();

            int n = _allArticles.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                NewsFeedArticle value = _allArticles[k];
                _allArticles[k] = _allArticles[n];
                _allArticles[n] = value;
            }
        }

        /// <summary>
        /// Performs the filtering process when search bar is clicked
        /// Order results alphabetically
        /// </summary>
        private void PerformFiltering()
        {
            FilteredItems.Clear();

            if (_filter == null)
                _filter = "";

            var lowerCaseFilter = Filter.ToLowerInvariant().Trim();

            var results = NewsFeedArticles.
                Where(d => d.Title.ToLowerInvariant().Contains(lowerCaseFilter)).
                ToList();

            results = results.OrderBy(o => o.Title).ToList();

            foreach (var item in results)
                FilteredItems.Add(item);
        }

        private void FilterArticles()
        {
            _filteredArticles = _allArticles.Where(o => o.MainTag.Contains("")).ToList();
        }

        /// <summary>
        /// Ensures that in a set of 24 Articles, there are at most 4 Articles without Images
        /// </summary>
        private async void RemoveWithoutImages()
        {
            List<int> ArticlesIndex = new List<int>();

            for (int i = 0; i < 24; i++)
                if (NewsFeedArticles[i].Image == null || NewsFeedArticles[i].Image == string.Empty)
                    ArticlesIndex.Add(i);

            while (ArticlesIndex.Count > 4)
            {
                while (_filteredArticles[nextToDisplay].Image == null || _filteredArticles[nextToDisplay].Image == string.Empty)
                {
                    nextToDisplay++;
                    if (nextToDisplay > _filteredArticles.Count - 1)
                    {
                        var Dialog = new MessageDialog("Too few Articles in Database");
                        await Dialog.ShowAsync();
                    }
                }

                NewsFeedArticles.RemoveAt(ArticlesIndex[0]);
                NewsFeedArticles.Insert(ArticlesIndex[0], _filteredArticles[nextToDisplay]);
                nextToDisplay++;
                ArticlesIndex.RemoveAt(0);
            }
        }

        /// <summary>
        /// Ensures that articles without Images are in the correct place
        /// If more than 4 Articles without Images, then RemoveExcessWithoutImages is called
        /// </summary>
        private void PositionArticles()
        {
            List<NewsFeedArticle> ArticleIndex = new List<NewsFeedArticle>();

            foreach (NewsFeedArticle Article in NewsFeedArticles)
                if (Article.Image == null || Article.Image == string.Empty)
                    ArticleIndex.Add(Article);

            if (ArticleIndex.Count > 4)
            {
                RemoveWithoutImages();
                ArticleIndex.Clear();
                foreach (NewsFeedArticle Article in NewsFeedArticles)
                    if (Article.Image == null || Article.Image == string.Empty)
                        ArticleIndex.Add(Article);
            }

            List<int> indexToInsert = IndexToInsert(ArticleIndex.Count);

            for (int i = 0; i < ArticleIndex.Count; i++)
            {
                NewsFeedArticles.Remove(ArticleIndex[i]);
            }

            for (int i = 0; i < indexToInsert.Count; i++)
            {
                NewsFeedArticles.Insert(indexToInsert[i], ArticleIndex[i]);
            }
        }

        /// <summary>
        /// Generates List of indexes to Insert articles without Images
        /// </summary>
        /// <param name="Count"></param>
        /// <returns></returns>
        private static List<int> IndexToInsert(int Count)
        {
            List<int> indexToInsert = new List<int>();

            int count = 0;

            for (int i = 0; i < Count; i++)
            {
                if (indexToInsert.Count == 0 || indexToInsert[indexToInsert.Count - 1] % 24 == 19)
                    indexToInsert.Add(6 + count * 24);
                else if (indexToInsert[indexToInsert.Count - 1] % 24 == 6)
                    indexToInsert.Add(7 + count * 24);
                else if (indexToInsert[indexToInsert.Count - 1] % 24 == 7)
                    indexToInsert.Add(18 + count * 24);
                else if (indexToInsert[indexToInsert.Count - 1] % 24 == 18)
                {
                    indexToInsert.Add(19 + count * 24);
                    count++;
                }
            }

            return indexToInsert;
        }

        /// <summary>
        /// Assign article types so that the articles appear in the correct places
        /// </summary>
        private void AssignArticleTypes()
        {
            int j;
            for (int i = 0; i < NewsFeedArticles.Count; i++)
            {
                j = i % 24;
                switch (j)
                {
                    case 0:
                        NewsFeedArticles[i].ArticleType = ArticleType.Large;
                        break;
                    case 1:
                        NewsFeedArticles[i].ArticleType = ArticleType.Small;
                        break;
                    case 2:
                        NewsFeedArticles[i].ArticleType = ArticleType.Small;
                        break;
                    case 3:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                    case 4:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                    case 5:
                        NewsFeedArticles[i].ArticleType = ArticleType.Large;
                        break;
                    case 6:
                        NewsFeedArticles[i].ArticleType = ArticleType.Mini;
                        break;
                    case 7:
                        NewsFeedArticles[i].ArticleType = ArticleType.Mini;
                        break;
                    case 8:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                    case 9:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                    case 10:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                    case 11:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                    case 12:
                        NewsFeedArticles[i].ArticleType = ArticleType.Small;
                        break;
                    case 13:
                        NewsFeedArticles[i].ArticleType = ArticleType.Small;
                        break;
                    case 14:
                        NewsFeedArticles[i].ArticleType = ArticleType.Large;
                        break;
                    case 15:
                        NewsFeedArticles[i].ArticleType = ArticleType.Large;
                        break;
                    case 16:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                    case 17:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                    case 18:
                        NewsFeedArticles[i].ArticleType = ArticleType.Mini;
                        break;
                    case 19:
                        NewsFeedArticles[i].ArticleType = ArticleType.Mini;
                        break;
                    case 20:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                    case 21:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                    case 22:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                    case 23:
                        NewsFeedArticles[i].ArticleType = ArticleType.Medium;
                        break;
                }
            }
        }

        public void PreviousPage()
        {
            if(count != 0)
            {
                count--;
                NewsFeedArticles.Clear();
                foreach (NewsFeedArticle Article in NewsFeedArticlesHolder[count])
                {
                    NewsFeedArticles.Add(Article);
                }
            }
        }

        public void NextPage()
        {
            count++;
            if(count == NewsFeedArticlesHolder.Count)
            {
                NewsFeedArticles.Clear();

                for (int i = 0; i < 24; i++)
                {
                    NewsFeedArticles.Add(_filteredArticles[nextToDisplay]);
                    nextToDisplay++;
                    NewsFeedArticles[i].Image = WebUtility.HtmlDecode(NewsFeedArticles[i].Image);
                }

                PositionArticles();
                AssignArticleTypes();
                PerformFiltering();

                NewsFeedArticlesHolder.Add(new List<NewsFeedArticle>());
                foreach (NewsFeedArticle Article in NewsFeedArticles)
                {
                    NewsFeedArticlesHolder[NewsFeedArticlesHolder.Count - 1].Add(Article);
                }

                if(_filteredArticles.Count - nextToDisplay < 40)
                {
                    maxCount = count;
                }

            }
            else
            {
                NewsFeedArticles.Clear();
                foreach(NewsFeedArticle Article in NewsFeedArticlesHolder[count])
                {
                    NewsFeedArticles.Add(Article);
                }
            }
            
        }

        /// <summary>
        /// Delete Article from the list
        /// Place new Article in the same position
        /// </summary>
        /// <param name="ClickedArticle"></param>
        public void DeleteArticle(NewsFeedArticle ClickedArticle)
        {
            ArticleType ArticleType = ClickedArticle.ArticleType;
            int Index = NewsFeedArticles.IndexOf(ClickedArticle);

            NewsFeedArticles.Remove(ClickedArticle);

            while((_filteredArticles[nextToDisplay].Image == null || _filteredArticles[nextToDisplay].Image == string.Empty) && ArticleType != ArticleType.Mini)
            {
                nextToDisplay++;
            }

            _filteredArticles[nextToDisplay].ArticleType = ArticleType;
            _filteredArticles[nextToDisplay].Image = WebUtility.HtmlDecode(_filteredArticles[nextToDisplay].Image);
            NewsFeedArticles.Insert(Index, _filteredArticles[nextToDisplay]);
            nextToDisplay++;
        }

        public void ClearFilter() => Filter = null;
    }
}
