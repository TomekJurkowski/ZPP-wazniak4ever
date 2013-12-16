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
    public partial class Downloaded : PhoneApplicationPage
    {
        public Downloaded()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.LoadCourses();
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Course selectedCourse = AllDownloadsList.SelectedItem as Course;

            if (selectedCourse == null) return;

            var courseName = selectedCourse.Name;

            AllDownloadsList.SelectedItem = null;
            var navTo = string.Format("/Exercise.xaml?courseName={0}", courseName);
            NavigationService.Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
        }

        private void CourseSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(CourseSearch.Text);
            AllDownloadsList.ItemsSource = App.ViewModel.AllCourses.
                Where(course => course.Name.IndexOf(CourseSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
    }
}