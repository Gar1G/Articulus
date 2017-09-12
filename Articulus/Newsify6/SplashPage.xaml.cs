using Common;
using Newsify6.Common.GlobalDatabase;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Newsify6
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashPage : Page
    {
        public SplashPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //Access Local Storage Settings that has information on users token information
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue composite = (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["CurrentUserLogin"];

            //Check if setting exists for a valid user
            if (composite == null)
            {
                // No user to be autosigned in, direct user to Login/SignUp page
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.Frame.Navigate(typeof(Login.LoginPage)));
            }
            else
            {
                //if user exists in local settings then pull there data
                #region Check User Token is Valid
                string Email = composite["Email"].ToString();
                string Token = composite["Token"].ToString();
                DateTime Expiry = Convert.ToDateTime(composite["Expires In"]);
                //if user has valid token then allow them to access the app
                if (Expiry >= DateTime.Now)
                {

                    UserTokenInfo.GetInstance.access_token = Token;
                    UserTokenInfo.GetInstance.Email = Email;
                    UserTokenInfo.GetInstance.expiry = Expiry.ToString();
                    await GlobalDatabaseAPI.UpdateUserTokenInfo(Email);

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.Frame.Navigate(typeof(Interests.InterestPage)));

                }
                //otherwise redirect them to sign in
                else
                {
                    localSettings.Values.Remove("CurrentUserLogin");
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.Frame.Navigate(typeof(Login.LoginPage)));
                }
                #endregion
            }
        }

    }
}
