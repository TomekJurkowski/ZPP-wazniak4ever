using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wazniak_forever.Model;
using Task = System.Threading.Tasks.Task;

namespace wazniak_forever
{
    public partial class ModuleSelection : PhoneApplicationPage
    {
        public ModuleSelection()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
            if (App.ViewModel.CourseType == CourseType.StudyWithClarifier)
            {
                LayoutRoot.Visibility = Visibility.Collapsed;
                StartCourse();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NavigationService.RemoveBackEntry();
        }

        private async Task SelectExercise(CourseType type, IEnumerable<int> selectedModuleIdList)
        {
            App.ViewModel.CourseType = type;
            await App.ViewModel.PerformTimeConsumingProcess(this, "Loading exercises...",
                async () => await App.ViewModel.LoadExercises(selectedModuleIdList));
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
        }

        private async void StartCourse()
        {
            if (App.ViewModel.CourseType == CourseType.StudyWithClarifier) App.ViewModel.pickExercises();
            else
            {
                var type = (CourseType)Enum.Parse(typeof(CourseType), NavigationContext.QueryString["courseType"], true);
                if (ModuleSelector.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one module!");
                    return;
                }
                var selectedModuleIdList = (from object module in ModuleSelector.SelectedItems
                                            select module as Module
                                                into m
                                                select m.ID).ToList();
                await SelectExercise(type, selectedModuleIdList);
                ModuleSelector.SelectedItems.Clear();
            }
            string navTo = "";
            if (App.ViewModel.Solutions.Count == 0) return;
            switch (App.ViewModel.Solutions[0].Answer.Type)
            {
                case SolutionType.Open:
                    navTo = string.Format("/ExerciseOpen.xaml?courseName={0}", CourseName.Text);
                    break;
                case SolutionType.Value:
                    navTo = string.Format("/ExerciseSingleValue.xaml?courseName={0}", CourseName.Text);
                    break;
                case SolutionType.Multiple:
                    navTo = string.Format("/ExerciseMultipleChoice.xaml?courseName={0}", CourseName.Text);
                    break;
                case SolutionType.Single:
                    navTo = string.Format("/ExerciseSingleChoice.xaml?courseName={0}", CourseName.Text);
                    break;
            }
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.Relative));
        }

        private void SubmitModules_Click(object sender, RoutedEventArgs e)
        {
            StartCourse();
        }
    }
}