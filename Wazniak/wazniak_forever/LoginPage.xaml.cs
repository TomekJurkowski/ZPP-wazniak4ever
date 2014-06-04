using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wazniak_forever.Model;

namespace wazniak_forever
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.LoadAuthenticationProviders();
        }

        private async void AuthenticationProviders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var provider = AuthenticationProviders.SelectedItem as AuthenticationProvider;
            AuthenticationProviders.SelectedItem = null;
            if (provider != null)
            {
                switch (provider.Type)
                {
                    case AuthenticationProviderType.Microsoft:
                        await App.ViewModel.Authenticate(Microsoft.WindowsAzure
                            .MobileServices.MobileServiceAuthenticationProvider.MicrosoftAccount);
                        break;
                    case AuthenticationProviderType.Facebook:
                        await App.ViewModel.Authenticate(Microsoft.WindowsAzure
                            .MobileServices.MobileServiceAuthenticationProvider.Facebook);
                        break;
                    case AuthenticationProviderType.Google:
                        await App.ViewModel.Authenticate(Microsoft.WindowsAzure
                            .MobileServices.MobileServiceAuthenticationProvider.Google);
                        break;
                    case AuthenticationProviderType.Twitter:
                        await App.ViewModel.Authenticate(Microsoft.WindowsAzure
                            .MobileServices.MobileServiceAuthenticationProvider.Twitter);
                        break;
                }
                NavigationService.Navigate(new Uri("/MainMenu.xaml", UriKind.RelativeOrAbsolute));
            }
            
        }
    }
}