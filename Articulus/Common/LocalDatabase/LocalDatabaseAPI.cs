using Newsify6.Common.ArticleSearch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SQLite.Net;
using Newtonsoft.Json;

namespace Newsify6.Common.LocalDatabase
{
    public class LocalDatabaseAPI
    {
        private static string _path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");

        public static bool DbExists() => File.Exists(_path);

        public static SQLite.Net.SQLiteConnection OpenLocalDatabaseArticleConnection()
        {
            return new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), _path);
        }

        public static void CreateLocalDatabaseArticleTable()
        {
            SQLite.Net.SQLiteConnection conn;

            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), _path);
            conn.CreateTable<LocalDatabaseArticle>();

            conn.Close();
        }

        #region Storing Articles
        public static void StoreArticlesToLocalDatabase(SQLiteConnection Conn, LocalDatabaseArticle LocalDatabaseArticles)
        {
            Conn.Insert(LocalDatabaseArticles);
        }

        public static void StoreArticlesToLocalDatabase(SQLiteConnection Conn, List<LocalDatabaseArticle> LocalDatabaseArticles)
        {
            foreach (LocalDatabaseArticle LocalDatabaseArticle in LocalDatabaseArticles)
            {
                Conn.Insert(LocalDatabaseArticle);
            } 
        }
        #endregion

        public static List<NewsFeedArticle> PullAllArticles(SQLite.Net.SQLiteConnection Conn)
        {
            var AllLocalDatabaseArticles = Conn.Query<LocalDatabaseArticle>(
                "SELECT * FROM LocalDatabaseArticle");

            List <NewsFeedArticle> NewsFeedArticles = new List<NewsFeedArticle>();

            foreach (LocalDatabaseArticle LocalDatabaseArticle in AllLocalDatabaseArticles)
            {
                NewsFeedArticles.Add(new NewsFeedArticle(LocalDatabaseArticle));
            }

            return NewsFeedArticles;
        }

        public static void UpdateBestSubTagsJsonString(SQLite.Net.SQLiteConnection Conn, NewsFeedArticle NewsFeedArticle)
        {
            var QueryString = string.Format("UPDATE LocalDatabaseArticle " +
                                            "SET BestSubTagsJsonString = {0} " +
                                            "WHERE Id = {1}", NewsFeedArticle.BestSubTagsJsonString, NewsFeedArticle.Id);

            var Query = Conn.Query<LocalDatabaseArticle>(QueryString);
        }

        public static void UpdateEmbedlyEntitiesJsonString(SQLite.Net.SQLiteConnection Conn, NewsFeedArticle NewsFeedArticle)
        {
            var QueryString = string.Format("UPDATE LocalDatabaseArticle " +
                                            "SET EmbedlyEntitiesJsonString = {0} " +
                                            "WHERE Id = {1}", NewsFeedArticle.EmbedlyEntitiesJsonString, NewsFeedArticle.Id);

            var Query = Conn.Query<LocalDatabaseArticle>(QueryString);
        }

        public static void UpdateEmbedlyKeywordsJsonString(SQLite.Net.SQLiteConnection Conn, NewsFeedArticle NewsFeedArticle)
        {
            var QueryString = string.Format("UPDATE LocalDatabaseArticle " +
                                            "SET EmbedlyKeywordsJsonString = {0} " +
                                            "WHERE Id = {1}", NewsFeedArticle.EmbedlyKeywordsJsonString, NewsFeedArticle.Id);

            var Query = Conn.Query<LocalDatabaseArticle>(QueryString);
        }

        public static void UpdateAllJsonStrings(SQLite.Net.SQLiteConnection Conn, NewsFeedArticle NewsFeedArticle)
        {
            var QueryString = string.Format("UPDATE LocalDatabaseArticle " +
                                            "SET BestSubTagsJsonString = \"{0}\", " +
                                            "EmbedlyEntitiesJsonString = \"{1}\", " +
                                            "EmbedlyKeywordsJsonString = \"{2}\" " +
                                            "WHERE Id = {3}", 
                                            NewsFeedArticle.BestSubTagsJsonString.Replace("\"", "\"\""), 
                                            NewsFeedArticle.EmbedlyEntitiesJsonString.Replace("\"", "\"\""), 
                                            NewsFeedArticle.EmbedlyKeywordsJsonString.Replace("\"", "\"\""),
                                            NewsFeedArticle.Id);

            var Query = Conn.Query<LocalDatabaseArticle>(QueryString);
        }
        
        public static NewsFeedArticle PullUpdatedArticle(SQLite.Net.SQLiteConnection Conn, int Id)
        {
            var QueryString = string.Format("SELECT * FROM LocalDatabaseArticle " +
                                            "WHERE Id = {0}", Id);

            var Query = Conn.Query<LocalDatabaseArticle>(QueryString);

            return Query != null && Query.Capacity != 0 ?
                new NewsFeedArticle(Query[0]) :
                null;
        }

        public static void DeleteDuplicates(SQLite.Net.SQLiteConnection Conn)
        {
            var Query = Conn.Query<LocalDatabaseArticle>(
                "DELETE FROM LocalDatabaseArticle " +
                "WHERE Id NOT IN( " +
                "SELECT Id FROM LocalDatabaseArticle " +
                "AS t1 JOIN( " +
                "SELECT MIN(Id) AS min_Id " +
                "FROM LocalDatabaseArticle " +
                "GROUP BY Title) " +
                "AS t2 ON t1.Id = t2.min_Id)");
        }

        public static void DeleteArticlesWithoutTitles(SQLite.Net.SQLiteConnection Conn)
        {
            var Query = Conn.Query<LocalDatabaseArticle>(
                "DELETE FROM LocalDatabaseArticle " +
                "WHERE Id IN ( " +
                "SELECT Id FROM LocalDatabaseArticle " +
                "WHERE Title IS NULL OR \"\")");
        }

        public static void DeleteArticlesWithoutDescriptions(SQLite.Net.SQLiteConnection Conn)
        {
            var Query = Conn.Query<LocalDatabaseArticle>(
                "DELETE FROM LocalDatabaseArticle " +
                "WHERE Id IN ( " +
                "SELECT Id FROM LocalDatabaseArticle " +
                "WHERE Description IS NULL OR \"\")");
        }

        public static void DeleteArticlesWhereMainTagisNull(SQLite.Net.SQLiteConnection Conn)
        {
            var Query = Conn.Query<LocalDatabaseArticle>(
                   "DELETE FROM LocalDatabaseArticle " +
                   "WHERE Id IN ( " +
                   "SELECT Id FROM LocalDatabaseArticle " +
                   "WHERE MainTag IS NULL)");
        }
    }
}
