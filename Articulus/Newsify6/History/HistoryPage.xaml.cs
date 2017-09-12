using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newsify6.Interests;
using Newsify6.ReadingList;
using System.Collections.ObjectModel;
using Newsify6.Common.GlobalDatabase;
using Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Newsify6.History
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HistoryPage : Page
    {
        private ObservableCollection<HistoryArticle> _allHistoryItems = new ObservableCollection<HistoryArticle>();
        private ObservableCollection<HistoryArticle> HistoryItems = new ObservableCollection<HistoryArticle>();

        public SolidColorBrush BackgroundColorDark = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 44, 62, 80));
        public SolidColorBrush BackgroundColorLight = new SolidColorBrush(Windows.UI.Colors.LightGray);
        public SolidColorBrush ForegroundColorLight = new SolidColorBrush(Windows.UI.Colors.White);
        public SolidColorBrush ForegroundColorDark = new SolidColorBrush(Windows.UI.Colors.Black);

        public HistoryPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            List<GlobalDatabaseHistoryArticle> HistoryList = await GlobalDatabaseAPI.GETUserHistory(UserTokenInfo.GetInstance.Email);
            foreach(GlobalDatabaseHistoryArticle HistoryArticle in HistoryList)
            {
                _allHistoryItems.Add(new HistoryArticle(HistoryArticle));
            }

            List<HistoryArticle> temp = _allHistoryItems.Where(x => x.TimeStamp >= DateTime.Today).ToList();

            foreach(HistoryArticle Article in temp)
            {
                HistoryItems.Add(Article);
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            NavSplitView.IsPaneOpen = !NavSplitView.IsPaneOpen;
        }

        private void PageListBox_ItemClick(object sender, ItemClickEventArgs e)
        {
            var page = e.ClickedItem as StackPanel;
            var page_name = page.Parent as ListViewItem;

            if (page_name.Name == "NewsFeed")
            {
                App.NavService.NavigateTo(typeof(MainPage), null);
            }
            else if (page_name.Name == "Bookmarks")
            {
                App.NavService.NavigateTo(typeof(ReadingListPage), null);
            }
            else if (page_name.Name == "History")
            {
                App.NavService.NavigateTo(typeof(HistoryPage), null);
            }
            else if (page_name.Name == "Preferences")
            {
                App.NavService.NavigateTo(typeof(InterestPage), null);
            }
            else if (page_name.Name == "Settings")
            {

            }
            else
            {

            }
        }


        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeColor(0);
            HistoryItems.Clear();
            List<HistoryArticle> temp = _allHistoryItems.Where(x => x.TimeStamp >= DateTime.Today).ToList();

            foreach (HistoryArticle Article in temp)
            {
                HistoryItems.Add(Article);
            }
        }

        private void YesterdayButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeColor(1);
            HistoryItems.Clear();
            List<HistoryArticle> temp = _allHistoryItems.Where(x => x.TimeStamp >= DateTime.Today.AddDays(-1)).ToList();

            List<HistoryArticle> today = _allHistoryItems.Where(x => x.TimeStamp >= DateTime.Today).ToList();

            foreach(HistoryArticle todayArticle in today)
            {
                temp.Remove(todayArticle);
            }

            foreach (HistoryArticle Article in temp)
            {
                HistoryItems.Add(Article);
            }
        }

        private void PastWeekButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeColor(2);
            HistoryItems.Clear();
            List<HistoryArticle> temp = _allHistoryItems.Where(x => x.TimeStamp >= DateTime.Today.AddDays(-7)).ToList();

            List<HistoryArticle> today = _allHistoryItems.Where(x => x.TimeStamp >= DateTime.Today.AddDays(-1)).ToList();

            foreach (HistoryArticle todayArticle in today)
            {
                temp.Remove(todayArticle);
            }

            foreach (HistoryArticle Article in temp)
            {
                HistoryItems.Add(Article);
            }
        }

        private void PastMonthButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeColor(3);
            HistoryItems.Clear();

            List<HistoryArticle> temp = _allHistoryItems.Where(x => x.TimeStamp >= DateTime.Today.AddDays(-30)).ToList();

            List<HistoryArticle> today = _allHistoryItems.Where(x => x.TimeStamp >= DateTime.Today.AddDays(-7)).ToList();

            foreach (HistoryArticle todayArticle in today)
            {
                temp.Remove(todayArticle);
            }

            foreach (HistoryArticle Article in temp)
            {
                HistoryItems.Add(Article);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ChangeColor(int selection)
        {
            if (selection == 0)
            {
                TodayButton.Background = BackgroundColorDark;
                YesterdayButton.Background = BackgroundColorLight;
                PastWeekButton.Background = BackgroundColorLight;
                PastMonthButton.Background = BackgroundColorLight;
                TodayButton.Foreground = ForegroundColorLight;
                YesterdayButton.Foreground = ForegroundColorDark;
                PastWeekButton.Foreground = ForegroundColorDark;
                PastMonthButton.Foreground = ForegroundColorDark;

            }

            else if (selection == 1)
            {
                TodayButton.Background = BackgroundColorLight;
                YesterdayButton.Background = BackgroundColorDark;
                PastWeekButton.Background = BackgroundColorLight;
                PastMonthButton.Background = BackgroundColorLight;
                TodayButton.Foreground = ForegroundColorDark;
                YesterdayButton.Foreground = ForegroundColorLight;
                PastWeekButton.Foreground = ForegroundColorDark;
                PastMonthButton.Foreground = ForegroundColorDark;
            }

            else if (selection == 2)
            {
                TodayButton.Background = BackgroundColorLight;
                YesterdayButton.Background = BackgroundColorLight;
                PastWeekButton.Background = BackgroundColorDark;
                PastMonthButton.Background = BackgroundColorLight;
                TodayButton.Foreground = ForegroundColorDark;
                YesterdayButton.Foreground = ForegroundColorDark;
                PastWeekButton.Foreground = ForegroundColorLight;
                PastMonthButton.Foreground = ForegroundColorDark;
            }

            else if (selection == 3)
            {
                TodayButton.Background = BackgroundColorLight;
                YesterdayButton.Background = BackgroundColorLight;
                PastWeekButton.Background = BackgroundColorLight;
                PastMonthButton.Background = BackgroundColorDark;
                TodayButton.Foreground = ForegroundColorDark;
                YesterdayButton.Foreground = ForegroundColorDark;
                PastWeekButton.Foreground = ForegroundColorDark;
                PastMonthButton.Foreground = ForegroundColorLight;
            }
        }
    }
}
