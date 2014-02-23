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
    public partial class Course : PhoneApplicationPage
    {
        public Course()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.LoadCoursePage();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
            //CourseDescription.Text = Convert.ToString(NavigationContext.QueryString["courseDescription"]);
        }

        private void CoursePageOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Option option = CoursePageOptions.SelectedItem as Option;
            CoursePageOptions.SelectedItem = null;

            if (option != null)
            {
                if (option.OnlineOnly && !App.ViewModel.OnlineMode) return;

                switch (option.Type)
                {
                    case OptionType.Start:
                        var navTo = string.Format("/Exercise.xaml?courseName={0}", CourseName.Text);
                        (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                        break;
                    case OptionType.AddToMyCourses:
                        break;
                    case OptionType.Download:
                        break;
                }
            }
        }
    }
}