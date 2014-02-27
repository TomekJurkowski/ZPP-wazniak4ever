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
            App.ViewModel.CurrentCourseID = Convert.ToInt32(NavigationContext.QueryString["courseID"]);
        }

        private async void SelectExercise()
        {
            //await App.ViewModel.LoadExercises();
            System.Diagnostics.Debug.WriteLine("Beginning of SelectExercise()");
            await App.ViewModel.PerformTimeConsumingProcess(this, "Loading exercises...", App.ViewModel.LoadExercises);
            System.Diagnostics.Debug.WriteLine("Hello");
            if (App.ViewModel.Solutions.Count <= 0) return;
            System.Diagnostics.Debug.WriteLine("Solutions.Count > 0");
            System.Diagnostics.Debug.WriteLine(App.ViewModel.Solutions[0].Answer.Type);
            string navTo;
            switch (App.ViewModel.Solutions[0].Answer.Type)
            {
                case SolutionType.Open:
                    navTo = string.Format("/ExerciseOpen.xaml?courseName={0}", CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                    break;
                case SolutionType.Multiple:
                    navTo = string.Format("/ExerciseMultipleChoice.xaml?courseName={0}", CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.Relative));
                    break;
            }
        }

        private async void CoursePageOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Option option = CoursePageOptions.SelectedItem as Option;
            CoursePageOptions.SelectedItem = null;

            if (option != null)
            {
                if (option.OnlineOnly && !App.ViewModel.OnlineMode) return;

                switch (option.Type)
                {
                    case OptionType.Start:
                        SelectExercise();
                        break;
                    case OptionType.AddToMyCourses:
                        break;
                    case OptionType.Download:
                        Subject currentCourse = App.ViewModel.AllCourses.Find( x => x.ID == App.ViewModel.CurrentCourseID);
                        App.ViewModel.db.SaveSubjectLocally(currentCourse);
                        System.Diagnostics.Debug.WriteLine("Subject saved locally " + currentCourse.Name);
                        break;
                }
            }
        }
    }
}