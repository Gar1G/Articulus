using Newsify6.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Newsify6
{
    public class TemplateSelector : DataTemplateSelector
    {
        public DataTemplate LargeTemp { get; set; }
        public DataTemplate MediumTemp { get; set; }
        public DataTemplate SmallTemp { get; set; }
        public DataTemplate MiniTemp { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var dataItem = item as NewsFeedArticle;

            if (dataItem.ArticleType == ArticleType.Large)
                return LargeTemp;
            else if (dataItem.ArticleType == ArticleType.Medium)
                return MediumTemp;
            else if (dataItem.ArticleType == ArticleType.Small)
                return SmallTemp;
            else
                return MiniTemp;
        }

    }
}
