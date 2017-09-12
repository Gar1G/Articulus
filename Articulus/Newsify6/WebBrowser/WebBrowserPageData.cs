using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Newsify6.WebBrowser
{
    public class WebBrowserPageData
    {
        public static void BackButton(ref WebView web)
        {

        }

        public static void ForwardButton(ref WebView web)
        {
            if (web.CanGoForward)
            {
                web.GoForward();
            }
        }

        public static void GoButton(ref WebView web, string value, KeyRoutedEventArgs args)
        {
            if (args.Key == Windows.System.VirtualKey.Enter)
            {
                try
                {
                    web.Navigate(new Uri(value));
                }
                catch
                {

                }
                web.Focus(FocusState.Keyboard);
            }
        }

        public static void LoadPage(ref WebView web, string url)
        {
            try
            {
                web.Navigate(new Uri(url));
            }
            catch
            {

            }
            web.Focus(FocusState.Keyboard);
        }
    }
}
