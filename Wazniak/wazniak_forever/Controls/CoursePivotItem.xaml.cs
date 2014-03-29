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

        private const string MY_COURSES_NAME = "MyCourses";
        private const string ALL_COURSES_NAME = "AllCourses";
        private const string NEW_COURSES_NAME = "New";

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Subject selectedCourse = AllCoursesList.SelectedItem as Subject;

            if (selectedCourse == null) return;
            var courseName = selectedCourse.Name;
            var courseID = selectedCourse.ID;
            var courseDescription = selectedCourse.Description;
            AllCoursesList.SelectedItem = null;
            var navTo = string.Format("/Course.xaml?courseName={0}&courseID={1}", courseName, courseID);
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
        }

        private void CourseSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            List<Subject> SearchList = App.ViewModel.DownloadedCourses;
            switch (this.Name)
            {
                case MY_COURSES_NAME:
                    SearchList = App.ViewModel.MyCourses;
                    break;
                case ALL_COURSES_NAME:
                    SearchList = App.ViewModel.AllCourses;
                    break;
                case NEW_COURSES_NAME:
                    SearchList = App.ViewModel.NewCourses;
                    break;
            }
            AllCoursesList.ItemsSource = SearchList.
                Where(course => course.Name.IndexOf(CourseSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
    }
}
