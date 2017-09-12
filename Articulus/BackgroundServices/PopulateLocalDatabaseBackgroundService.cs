using Newsify6.Common.ArticleSearch;
using Newsify6.Common.LocalDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;
using Newsify6.Common;
using Newsify6.Common.GlobalDatabase;
using Common;

namespace BackgroundServices
{
    public sealed class PopulateLocalDatabaseBackgroundService : IBackgroundTask
    {
        private static BackgroundTaskDeferral _deferral;
        private static uint _time = 30;

        public static async void Register()
        {
            if (!LocalDatabaseAPI.DbExists())
                LocalDatabaseAPI.CreateLocalDatabaseArticleTable();

            // Check if there are any registered tasks with the name MyBackgroundTask
            var isRegistered = BackgroundTaskRegistration.AllTasks.Values.Any(
                t => t.Name == nameof(PopulateLocalDatabaseBackgroundService));

            if (isRegistered)
                return;

            // Requesting access for application to run background tasks
            if (await BackgroundExecutionManager.RequestAccessAsync() ==
                BackgroundAccessStatus.Denied)
                return;

            RegisterNewTask();
        }

        private static void RegisterNewTask()
        {
            // Register name and entry point of background task
            // Entry point will define where the Run method can be found
            var builder = new BackgroundTaskBuilder
            {
                Name = nameof(PopulateLocalDatabaseBackgroundService),
                TaskEntryPoint = $"{nameof(BackgroundServices)}.{nameof(PopulateLocalDatabaseBackgroundService)}"
            };

            builder.SetTrigger(new TimeTrigger(_time, false));
            builder.Register();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
           

            UserTags UserTags = await GlobalDatabaseAPI.GETUserTags(UserTokenInfo.GetInstance.Email);
            int numArticles = 50;

            List<string> Searches = await Util.GetTrendingKeyWords(10, "en-gb");

            SQLite.Net.SQLiteConnection Conn = LocalDatabaseAPI.OpenLocalDatabaseArticleConnection();



            for (int i = 0; i < Searches.Count; i++)
            {
                List<SearchedArticle> SearchedArticles = await Util.GetArticles
                     ("1871e2f9d5e04efcba50692d81ecd7bc",
                     "1a98e8cf56c14c5a82e0e92fba941e43",
                     numArticles,
                     Searches[i],
                     new List<string> { "bbc.co.uk", "telegraph.co.uk", "independent.co.uk", "huffingtonpost.co.uk", "theguardian.com" },
                     "en-gb");

                List<LocalDatabaseArticle> LocalDatabaseArticles = new List<LocalDatabaseArticle>();

                foreach (Article Article in SearchedArticles)
                {
                    LocalDatabaseArticles.Add(new LocalDatabaseArticle(Article));
                }

                LocalDatabaseAPI.StoreArticlesToLocalDatabase(Conn, LocalDatabaseArticles);
            }

            LocalDatabaseAPI.DeleteDuplicates(Conn);
            LocalDatabaseAPI.DeleteArticlesWithoutTitles(Conn);
            LocalDatabaseAPI.DeleteArticlesWithoutDescriptions(Conn);

            Conn.Close();

            _deferral.Complete();
        }
    }
}
