using Common;
using Newsify6.Common.GlobalDatabase;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;


namespace Newsify6.Login
{
    public sealed partial class LoginPage : Page
    {

        public SolidColorBrush BackgroundColorDark = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 44, 62, 80));
        public SolidColorBrush BackgroundColorLight = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public LoginPage()
        {

            this.InitializeComponent();

            //Set Box/Button Text
            Password.PlaceholderText = "Password";
            PasswordRepeat.PlaceholderText = "Repeat Password";
            FirstName.PlaceholderText = "First Name";
            SecondName.PlaceholderText = "Second Name";
            Email.PlaceholderText = "Email";
            Login.Content = "Login";
            Register.Content = "Register";
            LoginEmail.PlaceholderText = "Email";
            LoginPassword.PlaceholderText = "Password";

            //Set Click Events
            Login.Click += Login_Click;
            Register.Click += Register_Click;
            PasswordRepeat.PasswordChanged += Password_PasswordChanged;
            Password.PasswordChanged += Password_PasswordChanged;
            Email.TextChanged += Email_TextChange;
            Submit.Click += Submit_Data;
            SubmitLogin.Click += Submit_Login;
            LoginEmail.TextChanged += Login_Email_TextChange;

            //Collapsed and Visibility Settings
            RegisterForm.Visibility = Visibility.Collapsed;
            LoginForm.Visibility = Visibility.Visible;
            FormValidationLogin.Visibility = Visibility.Visible;
            FormValidation.Visibility = Visibility.Collapsed;

        }

        #region Styles
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Register.Background = BackgroundColorLight;
            Login.Background = BackgroundColorDark;
            Login.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            Register.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            FormValidation.Visibility = Visibility.Collapsed;
            FormValidationLogin.Visibility = Visibility.Visible;
            RegisterForm.Visibility = Visibility.Collapsed;
            LoginForm.Visibility = Visibility.Visible;
            LoginEmail.Text = string.Empty;
            LoginEmailValidation.Text = string.Empty;
            LoginPassword.Password = string.Empty;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            //Styles
            Login.Background = BackgroundColorLight;
            Register.Background = BackgroundColorDark;
            Register.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            Login.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            //Collapsed and Visibility Settings
            FormValidationLogin.Visibility = Visibility.Collapsed;
            FormValidation.Visibility = Visibility.Visible;
            LoginForm.Visibility = Visibility.Collapsed;
            RegisterForm.Visibility = Visibility.Visible;

            Email.Text = string.Empty;
            EmailValidation.Text = string.Empty;
            FirstName.Text = string.Empty;
            SecondName.Text = string.Empty;
            FirstNameValidation.Text = string.Empty;
            SecondNameValidation.Text = string.Empty;
            Password.Password = string.Empty;
            PasswordRepeat.Password = string.Empty;
            PasswordValidation.Text = string.Empty;
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Password.Password == PasswordRepeat.Password && (Password.Password != "" || PasswordRepeat.Password != ""))
            {
                PasswordValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 46, 204, 113));
                PasswordValidation.Text = "Passwords Match";
            }
            else
            {
                PasswordValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                PasswordValidation.Text = "Passwords Do Not Match";
            }

        }

        private void Email_TextChange(object sender, RoutedEventArgs e)
        {
            bool isEmail = Regex.IsMatch(Email.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (!isEmail)
            {
                EmailValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                EmailValidation.Text = "Please Enter A Valid Email Address";

            }
            else
            {
                EmailValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 46, 204, 113));
                EmailValidation.Text = "Email Address Is Valid";
            }


        }

        private void Login_Email_TextChange(object sender, RoutedEventArgs e)
        {
            bool isEmail = Regex.IsMatch(LoginEmail.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (!isEmail)
            {
                LoginEmailValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                LoginEmailValidation.Text = "Please Enter A Valid Email Address";

            }
            else
            {
                LoginEmailValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 46, 204, 113));
                LoginEmailValidation.Text = "Email Address Is Valid";
            }


        }
        #endregion

        private async void Submit_Data(object sender, RoutedEventArgs e)
        {
            Submit.IsEnabled = false;
            bool error = false;
            Match match = Regex.Match(Password.Password, "[^a-z0-9]", RegexOptions.IgnoreCase);
            FormValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));

            #region Check if Form Input is Valid
            if (FirstName.Text == string.Empty || FirstName.Text == "")
            {
                FormValidation.Text = "Please Fix The Following Errors";
                FirstNameValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                FirstNameValidation.Text = "First Name Cannot Be Empty";
                Submit.IsEnabled = true;
                error = true;

            }
            else
            {
                FirstNameValidation.Text = string.Empty;
            }

            if (SecondName.Text == string.Empty || SecondName.Text == "")
            {
                FormValidation.Text = "Please Fix The Following Errors";
                SecondNameValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                SecondNameValidation.Text = "Second Name Cannot Be Empty";
                error = true;
                Submit.IsEnabled = true;

            }
            else
            {
                SecondNameValidation.Text = string.Empty;

            }

            if (Email.Text == string.Empty)
            {
                EmailValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                EmailValidation.Text = "Email Address Cannot Be Empty";
                FormValidation.Text = "Please Fix The Following Errors";
                error = true;
                Submit.IsEnabled = true;

            }
            else if (EmailValidation.Text == "Please Enter A Valid Email Address")
            {

                FormValidation.Text = "Please Fix The Following Errors";
                error = true;
                Submit.IsEnabled = true;

            }

            if (Password.Password == string.Empty || PasswordRepeat.Password == string.Empty)

            {
                PasswordValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                PasswordValidation.Text = "Passwords Cannot Be Empty";
                FormValidation.Text = "Please Fix The Following Errors";
                error = true;
                Submit.IsEnabled = true;

            }
            else if (PasswordValidation.Text == "Passwords Do Not Match")
            {
                FormValidation.Text = "Please Fix The Following Errors";
                error = true;
                Submit.IsEnabled = true;

            }
            else if (!match.Success)
            {

                PasswordValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                PasswordValidation.Text = "Password Must Contain Special Character";
                error = true;
                Submit.IsEnabled = true;


            }
            else if (!(Password.Password.Any(char.IsDigit)))
            {
                PasswordValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                PasswordValidation.Text = "Password Must Contain Number";
                error = true;
                Submit.IsEnabled = true;

            }
            else if (!Password.Password.Any(c => char.IsUpper(c)) || !Password.Password.Any(c => char.IsLower(c)))
            {
                PasswordValidation.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                PasswordValidation.Text = "Password Must Contain Both Upper and Lower Case";
                error = true;
                Submit.IsEnabled = true;

            }

            #endregion

            #region Register User
            if (!error)
            {
                //Remove any validation errors
                FormValidation.Text = string.Empty;

                var entry = new RegisterBindingModel()
                {

                    Email = Email.Text,
                    Password = Password.Password,
                    ConfirmPassword = PasswordRepeat.Password

                };

                //Post to server details about registration
                var entryJson = JsonConvert.SerializeObject(entry);
                var client = new HttpClient();
                var HttpContent = new StringContent(entryJson);
                HttpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync("http://poweb.azurewebsites.net/api/account/register", HttpContent);

                //if register was successful, redirect to Login
                if (response.IsSuccessStatusCode)
                {
                    FormValidation.Visibility = Visibility.Collapsed;
                    FormValidationLogin.Visibility = Visibility.Visible;
                    FormValidationLogin.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 46, 204, 113));
                    FormValidationLogin.Text = "Registration Successful - Please Login Using Your Credentials";
                    Register.IsEnabled = false;
                    Submit.IsEnabled = false;
                    Login_Click(sender, e);

                    UserTokenInfo.GetInstance.Email = Email.Text;
                    UserTokenInfo.GetInstance.FirstName = FirstName.Text;
                    UserTokenInfo.GetInstance.SecondName = SecondName.Text;
                }
                else
                {
                    FormValidationLogin.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                    FormValidation.Text = "This user already exists, please try logging in";
                    EmailValidation.Text = "";
                    Submit.IsEnabled = true;

                }
            }
            #endregion

        }

        private async void Submit_Login(object sender, RoutedEventArgs e)
        {
            //Disable Login Button
            SubmitLogin.IsEnabled = false;

            #region Get Token Validation for User
            var entry = "username=" + LoginEmail.Text + "&password=" + LoginPassword.Password + "&grant_type=password";
            var client = new HttpClient();
            var HttpContent = new StringContent(entry);
            HttpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            HttpResponseMessage Response = await client.PostAsync("http://poweb.azurewebsites.net/token", HttpContent);
            #endregion

            #region Log User In
            //if error in Login
            if (Response.IsSuccessStatusCode == false)
            {
                FormValidationLogin.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
                FormValidationLogin.Text = "Email/Password is Incorrect";
                SubmitLogin.IsEnabled = true;
            }
            else
            {
                FormValidationLogin.Text = "";
                //Get Token Information
                UserTokenInfoSerialized usertokeninfo = JsonConvert.DeserializeObject<UserTokenInfoSerialized>(await Response.Content.ReadAsStringAsync());

                //Store User Credentials in Singleton Class for use in this session
                UserTokenInfo.GetInstance.access_token = usertokeninfo.access_token;
                UserTokenInfo.GetInstance.expiry = DateTime.Now.AddSeconds(usertokeninfo.expires_in).ToString();
                UserTokenInfo.GetInstance.Email = usertokeninfo.userName;

                bool UserLoginInformationExists = await GlobalDatabaseAPI.UpdateUserTokenInfo(UserTokenInfo.GetInstance.Email);

                if (!UserLoginInformationExists)
                {
                    // These should not be null.
                    Debug.WriteLine(UserTokenInfo.GetInstance.Email);
                    Debug.WriteLine(UserTokenInfo.GetInstance.FirstName);
                    Debug.WriteLine(UserTokenInfo.GetInstance.SecondName);

                    GlobalDatabaseAPI.POSTUserLoginInformation();
                }


                //Store User Credential Locally 
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
                composite["Email"] = UserTokenInfo.GetInstance.Email;
                composite["Token"] = UserTokenInfo.GetInstance.access_token;
                composite["Expires In"] = UserTokenInfo.GetInstance.expiry.ToString();
                localSettings.Values["CurrentUserLogin"] = composite;

                //navigate to page which can then access the api using bearer
                this.Frame.Navigate(typeof(Interests.InterestPage));
            }
            #endregion

        }


        //private async void Submit_Login(object sender, RoutedEventArgs e)
        //{
        //    bool userdatavalid = false;//this will actually be a function to check whether user is valid
        //    if (userdatavalid)
        //    {

        //        SQLite.Net.SQLiteConnection Conn = LocalDatabaseAPI.OpenLocalDatabaseArticleConnection();

        //        List<string> Searchs = new List<string> { "England", "Olympics", "Brazil", "China", "India", "Africa", "Canada" };

        //        for (int i = 0; i < Searchs.Count; i++)
        //        {
        //            Debug.WriteLine("\t\t" + Searchs[i]);
        //            List<SearchedArticle> SearchedArticles = await Util.GetArticles
        //             ("1871e2f9d5e04efcba50692d81ecd7bc", 
        //             "1a98e8cf56c14c5a82e0e92fba941e43",
        //             50, 
        //             Searchs[i],
        //             new List<string> { "bbc.co.uk", "telegraph.co.uk", "independent.co.uk", "huffingtonpost.co.uk", "theguardian.com" },
        //             "en-gb");

        //            List<LocalDatabaseArticle> LocalDatabaseArticles = new List<LocalDatabaseArticle>();

        //            foreach(Article Article in SearchedArticles)
        //            {
        //                LocalDatabaseArticles.Add(new LocalDatabaseArticle(Article));
        //            }

        //            LocalDatabaseAPI.StoreArticlesToLocalDatabase(Conn, LocalDatabaseArticles);
        //        }

        //        App.NavService.NavigateTo(typeof(MainPage), null);
        //        Debug.WriteLine("Done");
        //    }
        //    else
        //    {
        //        FormValidationLogin.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 57, 4));
        //        FormValidationLogin.Text = "Email/Password is Incorrect";
        //    }
        //}

    }
}