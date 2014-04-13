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
    public partial class ModuleSelection : PhoneApplicationPage
    {
        public ModuleSelection()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            if (App.ViewModel.CourseType == CourseType.StudyWithClarifier) StartCourse();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
        }

        private void StartCourse()
        {
            string navTo = "";
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