using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newsify6.ReadingList;
using Newsify6.Interests;
using Newsify6.History;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Newsify6.ReadingList
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReadingListPage : Page
    {
        private List<ReadingListItem> ReadingListItems;
        public ReadingListPage()
        {
            this.InitializeComponent();
            ReadingListItems = ReadingListItemManager.GetBookMarkItem();
        }

       
        private void ReadingList_ItemClick(object sender, ItemClickEventArgs e)
        {

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
    }
}
