using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using wazniak_forever.Model;

namespace wazniak_forever.Controls
{
    public partial class CoursePivotItem : UserControl
    {
        public CoursePivotItem()
        {
            InitializeComponent();
        }

        public void setBinding(string name)
        {
            Binding myBinding = new Binding(name);
            myBinding.Source = App.ViewModel;
            BindingOperations.SetBinding(AllCoursesList, LongListSelector.ItemsSourceProperty, myBinding);
        }

        private string MY_COURSES_NAME = "MyCourses";
        private string ALL_COURSES_NAME = "AllCourses";

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Course selectedCourse = AllCoursesList.SelectedItem as Course;

            if (selectedCourse == null) return;
            var courseName = selectedCourse.Name;
            AllCoursesList.SelectedItem = null;
            var navTo = string.Format("/Exercise.xaml?courseName={0}", courseName);
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
        }

        private void CourseSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(CourseSearch.Text);
            List<Course> SearchList = App.ViewModel.DownloadedCourses;
            if (this.Name == MY_COURSES_NAME) SearchList = App.ViewModel.MyCourses;
            else if (this.Name == ALL_COURSES_NAME) SearchList = App.ViewModel.AllCourses;
            AllCoursesList.ItemsSource = SearchList.
                Where(course => course.Name.IndexOf(CourseSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
    }
}
