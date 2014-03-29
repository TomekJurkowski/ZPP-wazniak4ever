using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wazniak_forever.Model;
using Microsoft.Phone.Net.NetworkInformation;

namespace wazniak_forever
{
    public partial class MainMenu : PhoneApplicationPage
    {
        public MainMenu()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.LoadMenu();
            //App.ViewModel.db.Drop();
            App.ViewModel.db.Initialize();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.ViewModel.CheckForNetworkAvailability();
        }

        private void MainOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Option option = MainOptions.SelectedItem as Option;
            MainOptions.SelectedItem = null;

            if (option != null)
            {
                if (option.OnlineOnly && !App.ViewModel.OnlineMode) return;

                switch (option.Type)
                {
                    case OptionType.MyCourses:
                        App.ViewModel.AreDownloads = false;
                        NavigationService.Navigate(new Uri("/CourseSelection.xaml", UriKind.RelativeOrAbsolute));
                        break;
                    case OptionType.Downloads:
                        App.ViewModel.AreDownloads = true;
                        NavigationService.Navigate(new Uri("/CourseSelection.xaml", UriKind.RelativeOrAbsolute));
                        break;
                    case OptionType.Login:
                        NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.RelativeOrAbsolute));
                        break;
                    case OptionType.Logout:
                        App.ViewModel.Logout();
                        NavigationService.Navigate(new Uri("/MainMenu.xaml?Refresh=true", UriKind.RelativeOrAbsolute));
                        break;
                }
            }
        }
    }
}