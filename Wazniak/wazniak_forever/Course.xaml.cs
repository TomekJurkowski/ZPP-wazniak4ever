using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace wazniak_forever
{
    public partial class Course : PhoneApplicationPage
    {
        public Course()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
            //CourseDescription.Text = Convert.ToString(NavigationContext.QueryString["courseDescription"]);
        }

        private void StartCourse_Click(object sender, RoutedEventArgs e)
        {
            var navTo = string.Format("/Exercise.xaml?courseName={0}", CourseName.Text);
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
        }
    }
}