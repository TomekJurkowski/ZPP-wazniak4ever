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
    public partial class CourseSelection : PhoneApplicationPage
    {
        public CourseSelection()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            if (App.ViewModel.OnlineCourses)
            {
                coursesPivot.Header = "My courses";
                App.ViewModel.LoadCourses();
            }
            else
            {
                coursesPivot.Header = "Downloads";
                App.ViewModel.LoadDownloadedCourses();
            }
        }

        /*private void Pivot_LoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            if (e.Item == AllCourses)
                ApplicationBar.IsVisible = true;
            else
                ApplicationBar.IsVisible = false; 
        }*/

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Course selectedCourse = AllCoursesList.SelectedItem as Course;

            if (selectedCourse == null) return;

            var courseName = selectedCourse.Name;

            AllCoursesList.SelectedItem = null;
            var navTo = string.Format("/Exercise.xaml?courseName={0}", courseName);
            NavigationService.Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
        }

        private void CourseSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(CourseSearch.Text);
            List<Course> searchCourses;
            if (App.ViewModel.OnlineCourses) searchCourses = App.ViewModel.AllCourses;
            else searchCourses = App.ViewModel.DownloadedCourses;
            AllCoursesList.ItemsSource = searchCourses.
                Where(course => course.Name.IndexOf(CourseSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
    }
}