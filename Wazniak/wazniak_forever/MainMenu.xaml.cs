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

namespace wazniak_forever
{
    public partial class MainMenu : PhoneApplicationPage
    {
        public MainMenu()
        {
            InitializeComponent();

            DataContext = App.ViewModel;
            App.ViewModel.LoadMenu();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void MainOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Option option = MainOptions.SelectedItem as Option;
            MainOptions.SelectedItem = null;

            if (option != null)
            {
                switch (option.Type)
                {
                    case OptionType.StartCourse:
                        NavigationService.Navigate(new Uri("/CourseSelection.xaml", UriKind.RelativeOrAbsolute));
                        break;
                }
            }
        }
    }
}