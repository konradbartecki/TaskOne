using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using TaskOne.WindowsPhone.Model;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Interfaces;
using User = TaskOne.WindowsPhone.Model.User;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace TaskOne.WindowsPhone
{ 
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //private MobileServiceUser user;
        private TwitterCredentials appCredentials;
        private MobileServiceCollection<User, User> users;
        private IMobileServiceTable<User> userTable =
            App.TaskOneClient.GetTable<User>();
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }



        private async void button_Click(object sender, RoutedEventArgs e)
        {
            appCredentials = new TwitterCredentials(Config.Twitter.ConsumerKey, Config.Twitter.ConsumerSecret);

            var url = CredentialsCreator.GetAuthorizationURL(appCredentials);
            button.Visibility = Visibility.Collapsed;
            textBox.Visibility = Visibility.Visible;
            button2.Visibility = Visibility.Visible;

            await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
            
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox.Text)) return;
            var userCredentials = CredentialsCreator.GetCredentialsFromVerifierCode(textBox.Text, appCredentials);

            Auth.SetCredentials(userCredentials);

            var loggedUser = Tweetinvi.User.GetLoggedUser();
            var accountSettings = loggedUser.GetAccountSettings();

            ShowUserbar(loggedUser);
            LoginGrid.Visibility = Visibility.Collapsed;
            MainGrid.Visibility = Visibility.Visible;
            Register(loggedUser);
        }

        private void ShowUserbar(ILoggedUser user)
        {
            namebox.Text = user.ScreenName;
            usernamebox.Text = user.Name;
            Uri uri = new Uri(user.ProfileImageUrl);
            ImageSource imgSource = new BitmapImage(uri);          
            imgbox.Source = imgSource;
        }

        private async void Register(ILoggedUser user)
        {
            User myUser = new User {Email = user.ScreenName};
            try
            {
                await userTable.InsertAsync(myUser);
            }
            catch (MobileServiceInvalidOperationException e)
            {
                Debug.WriteLine(e.Response);
            }            
        }
    }
}
