using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Name == MY_COURSES_NAME)
            {
                GradientStopCollection gSC = new GradientStopCollection();
                if (!App.ViewModel.isSorted)
                {
                    App.ViewModel.isSorted = true;
                    App.ViewModel.MyCourses.Sort(App.ViewModel.CompareSubjects);
                    AllCoursesList.ItemsSource = App.ViewModel.MyCourses.ToList();
                }
                App.ViewModel.CalculateBreakingPoint();
                for (int i = 0; i < 4; i++) gSC.Add(new GradientStop());
                gSC[0].Color = (Application.Current.Resources["TileBackgroundColor"] as SolidColorBrush).Color;
                gSC[1].Color = gSC[0].Color;
                gSC[1].Offset = App.ViewModel.BreakingPoint;
                gSC[2].Color = Color.FromArgb(255, 180, 150, 250);
                gSC[2].Offset = gSC[1].Offset + 0.1;
                gSC[3].Color = gSC[2].Color;
                gSC[3].Offset = 1.0;
                LinearGradientBrush lGB = new LinearGradientBrush(gSC, 0.0);
                (sender as Grid).Background = lGB;
            }
        }
    }
}
