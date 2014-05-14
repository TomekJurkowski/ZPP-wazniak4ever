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
            
        }

        private string GetCourseDescription()
        {
            Subject currentSubject;
            if (App.ViewModel.OnlineMode)
            {
                currentSubject = App.ViewModel.AllCourses.Find(course => course.ID == App.ViewModel.CurrentCourseID);
            }
            else
            {
                currentSubject = App.ViewModel.DownloadedCourses.Find(course => course.ID == App.ViewModel.CurrentCourseID);
            }
            return currentSubject.Description;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.ViewModel.CheckForNetworkAvailability();
            CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
            App.ViewModel.CurrentCourseID = Convert.ToInt32(NavigationContext.QueryString["courseID"]);
            App.ViewModel.CorrectAnswers = 0;
            App.ViewModel.GivenAnswers = new List<KeyValuePair<int, bool>>();
            CourseDescription.Text = GetCourseDescription();
            App.ViewModel.LoadCoursePage();
            
        }

        private async void SelectExercise(CourseType type)
        {
            App.ViewModel.CourseType = type;
            await App.ViewModel.PerformTimeConsumingProcess(this, "Loading exercises...", 
                async () => await App.ViewModel.LoadExercises(new List<int>()));
            App.ViewModel.LoadModules();
            if (App.ViewModel.Solutions.Count <= 0) return;

            if (App.ViewModel.CourseType == CourseType.Time)
            {
                App.ViewModel.Timer = new DTimer();
                var timer = App.ViewModel.Timer;
                if (!timer.IsEnabled())
                {
                    timer.HandleTick += App.ViewModel.TimerModeTickHandler;
                    timer.Start(1, 20);
                }
            }
            else if (App.ViewModel.CourseType == CourseType.StudyWithClarifier) App.ViewModel.pickExercises();

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(string.Format("/ModuleSelection.xaml?courseName={0}", CourseName.Text), UriKind.Relative));
        }

        private void NavigateToModuleSelectionPage(CourseType courseType)
        {
            App.ViewModel.LoadModules();
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(string.Format("/ModuleSelection.xaml?courseName={0}&courseType={1}", CourseName.Text, courseType), UriKind.Relative));
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
                        NavigateToModuleSelectionPage(CourseType.Classic);
                        //SelectExercise(CourseType.Classic);
                        break;
                    case OptionType.StudyWithClarifier:
                        //NavigateToModuleSelectionPage(CourseType.StudyWithClarifier);
                        SelectExercise(CourseType.StudyWithClarifier);
                        break;
                    case OptionType.FixedNumber:
                        NavigateToModuleSelectionPage(CourseType.FixedNumber);
                        //SelectExercise(CourseType.FixedNumber);
                        break;
                    case OptionType.Timer:
                        NavigateToModuleSelectionPage(CourseType.Time);
                        //SelectExercise(CourseType.Time);
                        break;
                    case OptionType.AddToMyCourses:
                        await App.ViewModel.PerformTimeConsumingProcess(this, "Adding to My Courses...", App.ViewModel.AddToMyCourses);                                             
                        break;
                    case OptionType.DeleteFromMyCourses:
                        await App.ViewModel.PerformTimeConsumingProcess(this, "Deleting from My Courses...", App.ViewModel.DeleteFromMyCourses);          
                        break;
                    case OptionType.Download:
                        Subject currentCourse = App.ViewModel.AllCourses.Find(x => x.ID == App.ViewModel.CurrentCourseID);
                        await App.ViewModel.PerformTimeConsumingProcess(this, "Downloading course...", async () => { await App.ViewModel.db.SaveSubjectLocally(currentCourse); });
                        App.ViewModel.LoadCoursePage();
                        break;
                    case OptionType.Update:
                        currentCourse = App.ViewModel.AllCourses.Find(x => x.ID == App.ViewModel.CurrentCourseID);
                        await App.ViewModel.PerformTimeConsumingProcess(this, "Updating course...", async () => { await App.ViewModel.db.SyncDownloadedCourse(currentCourse); });
                        break;
                    case OptionType.DeleteFromDownloads:
                        currentCourse = App.ViewModel.DownloadedCourses.Find( x => x.ID == App.ViewModel.CurrentCourseID);
                        await App.ViewModel.PerformTimeConsumingProcess(this, "Deleting from Downloads...", async () => { await App.ViewModel.db.DeleteSubjectFromDownloads(currentCourse); });
                        App.ViewModel.LoadCoursePage();
                        break;
                }
            }
        }
    }
}