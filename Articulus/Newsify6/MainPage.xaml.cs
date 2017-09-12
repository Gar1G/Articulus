using Newsify6.Common;
using Newsify6.Common.GlobalDatabase;
using Newsify6.Common.LocalDatabase;
using Newsify6.History;
using Newsify6.Interests;
using Newsify6.ReadingList;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Newsify6
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            SetUpPageAnimation();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected void SetUpPageAnimation()
        {
            TransitionCollection collection = new TransitionCollection();
            NavigationThemeTransition theme = new NavigationThemeTransition();

            var info = new ContinuumNavigationTransitionInfo();
            theme.DefaultNavigationTransitionInfo = info;
            collection.Add(theme);
            this.Transitions = collection;
        }


        private async void feed_ItemClick(object sender, ItemClickEventArgs e)
        {
            NewsFeedArticle ClickedArticle = (NewsFeedArticle)e.ClickedItem;

            App.NavService.NavigateTo(typeof(ContentReader.ArticleContentPage), ClickedArticle);

            if ((ClickedArticle.BestSubTags == null || ClickedArticle.BestSubTags.Capacity == 0) && 
                (ClickedArticle.EmbedlyEntities == null || ClickedArticle.EmbedlyEntities.Capacity == 0) &&
                (ClickedArticle.EmbedlyKeywords == null || ClickedArticle.EmbedlyKeywords.Capacity == 0))
            {
                await ClickedArticle.GetSubTags();
                SQLite.Net.SQLiteConnection Conn = LocalDatabaseAPI.OpenLocalDatabaseArticleConnection();
                LocalDatabaseAPI.UpdateAllJsonStrings(Conn, ClickedArticle);
            }
            GlobalDatabaseAPI.POSTArticleData(ClickedArticle);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchButton.Visibility = Visibility.Collapsed;
            Search.Visibility = Visibility.Visible;
            ((MainPageData)this.DataContext).ClearFilter();
        }

        private void CloseSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchButton.Visibility = Visibility.Visible;
            Search.Visibility = Visibility.Collapsed;
        }

        private void feed_PointerEntered(object sender, RoutedEventArgs e)
        {
            var ClickedArticle = sender as Windows.UI.Xaml.Controls.Grid;
            var Hovered_Article = ClickedArticle.DataContext as NewsFeedArticle;  
            
            object value = null;
            ClickedArticle?.Resources.TryGetValue("pointerOver", out value);
            Storyboard story = value as Storyboard;
            story?.Begin();
            
            Hovered_Article.ButtonOpacity = 1;
        }   

        private void feed_PointerExited(object sender, RoutedEventArgs e)
        {
            var ClickedArticle = sender as Windows.UI.Xaml.Controls.Grid;
            var Hovered_Article = ClickedArticle.DataContext as NewsFeedArticle;
            object value = null;
            ClickedArticle?.Resources.TryGetValue("pointerLeave", out value);
            Storyboard story = value as Storyboard;
            story?.Begin();
            Hovered_Article.ButtonOpacity = 0;
        }

        private void Grid_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var ClickedArticle = sender as Windows.UI.Xaml.Controls.Grid;
            var Hovered_Article = ClickedArticle.DataContext as NewsFeedArticle;
            object value = null;
            ClickedArticle?.Resources.TryGetValue("pointerLeave", out value);
            Storyboard story = value as Storyboard;
            story?.Begin();
            Hovered_Article.ButtonOpacity = 0;
        }

        public static T FindParent<T>(DependencyObject element) where T : DependencyObject
        {
            while (element != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(element);
                T candidate = parent as T;
                if (candidate != null)
                {
                    return candidate;
                }
                element = parent;
            }
            return default(T);
        }

        private void sharebutton_Click(object sender, RoutedEventArgs e)
        {
            var story = share.Resources["ShareSheet"] as Storyboard;
            if (story != null)
            {
                share.Visibility = Visibility.Visible;
                story.Begin();
                story.Completed += ShareAnimate_Complete;
            }
        }

        private void ShareAnimate_Complete(object sender, object e)
        {
            share.Visibility = Visibility.Collapsed;
        }

        private void closebutton_Click(object sender, RoutedEventArgs e)
        {
            var Article = ((FrameworkElement)sender).DataContext as NewsFeedArticle;
            ((MainPageData)this.DataContext).DeleteArticle(Article);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            NavSplitView.IsPaneOpen = !NavSplitView.IsPaneOpen;
        }

        private void scrl_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            
            
            if(scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight && ((MainPageData)this.DataContext).count != ((MainPageData)this.DataContext).maxCount )
            {
                
                SeeMoreDown.Visibility = Visibility.Visible;
            }
            else if(scrollViewer.VerticalOffset == 0 &&  ((MainPageData)this.DataContext).count != 0 )
            {
                
                SeeMoreUp.Visibility = Visibility.Visible;
            }
            else
            {
                SeeMoreUp.Visibility = Visibility.Collapsed;
                SeeMoreDown.Visibility = Visibility.Collapsed;
            }

        }

        private void ReadingListButton_Click(object sender, RoutedEventArgs e)
        {
            var story = addedReading.Resources["ReadingList"] as Storyboard;
            if(story != null)
            {
                addedReading.Visibility = Visibility.Visible;
                story.Begin();
                story.Completed += RLAnimate_Complete;
            }
        }

        private void RLAnimate_Complete(object sender, object e)
        {
            addedReading.Visibility = Visibility.Collapsed;
        }


        /* STORYBOARD ANIMATIONS */
        //Duplicate storyboards needed due to unintended function calls


        //User wants to go back up to previous feed
        private void SeeMoreUp_Click(object sender, RoutedEventArgs e)
        {
            var DiscardFeed = feedGrid.Resources["slideDownExit"] as Storyboard;
            if (DiscardFeed != null)
            {
                DiscardFeed.Begin();
                //DiscardFeed.Completed += discardedComplete;
            }
        }

        //Prepare Next feed for entrance with opacity 0
        private void discardedComplete(object sender, object e)
        {
            
            var prepareNewFeed = feedGrid.Resources["PrepareUp"] as Storyboard;
            if (prepareNewFeed != null)
            {
                prepareNewFeed.Begin();
                scrl.ScrollToVerticalOffset(0);
                //prepareNewFeed.Completed += prepareComplete;
                
                
            }
        }

        //Entrance of previous feed fades into view
        private void prepareComplete(object sender, object e)
        {
            ((MainPageData)this.DataContext).PreviousPage();
            var prevFeed = feedGrid.Resources["slideDownEnter"] as Storyboard;
            if (prevFeed != null)
            {
                prevFeed.Begin();
                scrl.ScrollToVerticalOffset(scrl.ScrollableHeight);
                SeeMoreUp.Visibility = Visibility.Collapsed;
            }
        }

        //User wants to see more content that they havent seen yet
        private void SeeMoreDown_Click(object sender, RoutedEventArgs e)
        {
            
            //current feed fades out
            var DiscardFeed = feedGrid.Resources["slideUpExit"] as Storyboard;
            if (DiscardFeed != null)
            {
                DiscardFeed.Begin();
                //DiscardFeed.Completed += discardedComplete2;
               
            }
             
        }

        //Next Feed needs to be placed in position for entrance
        private void discardedComplete2(object sender, object e)
        {
            
            var prepareEntrance = feedGrid.Resources["PrepareDown"] as Storyboard;
            if (prepareEntrance != null)
            {
                prepareEntrance.Begin();
                //prepareEntrance.Completed += prepareComplete2;
                SeeMoreDown.Visibility = Visibility.Collapsed;
            }
        }

        //New Feed slides into view
        private void prepareComplete2(object sender, object e)
        {
            ((MainPageData)this.DataContext).NextPage();
            //feedGrid.Opacity = 1;
            var newFeed = feedGrid.Resources["slideUpEnter"] as Storyboard;
            if (newFeed != null)
            {
                newFeed.Begin();
                scrl.ScrollToVerticalOffset(0);

            }
        }

        private void ptrBox_RefreshInvoked(DependencyObject sender, object args)
        {

        }

        private void PageListBox_ItemClick(object sender, ItemClickEventArgs e)
        {
            var page = e.ClickedItem as StackPanel;
            var page_name = page.Parent as ListViewItem;
            NavSplitView.IsPaneOpen = !NavSplitView.IsPaneOpen;

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

        private void Grid_PointerCanceled(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var ClickedArticle = sender as Windows.UI.Xaml.Controls.Grid;
            var Hovered_Article = ClickedArticle.DataContext as NewsFeedArticle;
            Hovered_Article.Opacity = 0;
            Hovered_Article.ButtonOpacity = 0;
        }

        private void Grid_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            var ClickedArticle = sender as Windows.UI.Xaml.Controls.Grid;
            var Hovered_Article = ClickedArticle.DataContext as NewsFeedArticle;
            object value = null;
            ClickedArticle?.Resources.TryGetValue("pointerEnter", out value);
            Storyboard story = value as Storyboard;
            story?.Begin();
            Hovered_Article.ButtonOpacity = 1;

        }

        private void Grid_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var ClickedArticle = sender as Windows.UI.Xaml.Controls.Grid;
            var Hovered_Article = ClickedArticle.DataContext as NewsFeedArticle;
            

        }

        private void PageListBox_ItemClick_1(object sender, ItemClickEventArgs e)
        {

        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
