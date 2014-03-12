using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wazniak_forever.Model;

namespace wazniak_forever.Controls
{
    public partial class Exercise : UserControl
    {
        public Exercise()
        {
            InitializeComponent();
            DataContext = App.ViewModel;

            if (App.ViewModel != null && App.ViewModel.CourseType == CourseType.Time)
            {
                TimerTextBlock.Visibility = Visibility.Visible;
                App.ViewModel.HandleTimesUp += HandleTimesUpAction;
            }
        }

        public void NextExerciseVisible()
        {
            SubmitAnswer.Visibility = Visibility.Visible;
            NextQuestion.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;
        }

        public void SubmitAnswerClick(StringBuilder headerBuilder, StringBuilder builder)
        {
            SubmitAnswer.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Visible;
            ExplanationHeader.Text = headerBuilder.ToString();
            Explanation.Text = builder.ToString();

            if (App.ViewModel.CurrentQuestionNumber == App.ViewModel.Exercises.Count - 1)
            {
                Finish.Visibility = Visibility.Visible;
            }
            else
            {
                NextQuestion.Visibility = Visibility.Visible;
            }
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            NextExerciseVisible();
            // Extract next question
            App.ViewModel.CurrentQuestionNumber++;
            App.ViewModel.CurrentExercise = App.ViewModel.Exercises[App.ViewModel.CurrentQuestionNumber];
            App.ViewModel.CurrentSolution = App.ViewModel.Solutions[App.ViewModel.CurrentQuestionNumber];
            SolutionType NextType = App.ViewModel.CurrentSolution.Answer.Type;
            if (NextType == SolutionType.Multiple || NextType == SolutionType.Single)
                App.ViewModel.UserChoices = (App.ViewModel.Exercises[App.ViewModel.CurrentQuestionNumber]).Solution.Choices;
            string navTo;
            switch (App.ViewModel.CurrentSolution.Answer.Type)
            {
                case SolutionType.Open:
                    navTo = string.Format("/ExerciseOpen.xaml?courseName={0}&id=" + App.ViewModel.CurrentQuestionNumber, CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                    break;
                case SolutionType.Value:
                    navTo = string.Format("/ExerciseSingleValue.xaml?courseName={0}&id=" + App.ViewModel.CurrentQuestionNumber, CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                    break;
                case SolutionType.Multiple:
                    navTo = string.Format("/ExerciseMultipleChoice.xaml?courseName={0}&id=" + App.ViewModel.CurrentQuestionNumber, CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                    break;
                case SolutionType.Single:
                    navTo = string.Format("/ExerciseSingleChoice.xaml?courseName={0}&id=" + App.ViewModel.CurrentQuestionNumber, CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                    break;
            }
        }

        public void HandleTimesUpAction()
        {
            var navTo = string.Format("/TimedModeResults.xaml?courseName={0}", CourseName.Text);
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
        }

        public void HandleFinishAction() 
        {
            SubmitAnswer.Visibility = Visibility.Collapsed;
            Return.Visibility = Visibility.Visible;
            Finish.Visibility = Visibility.Collapsed;
            QuestionContent.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;
            if (App.ViewModel.CourseType != CourseType.Classic)
            {
                StatisticTitle.Visibility = Visibility.Visible;
                StatisticContent.Visibility = Visibility.Visible;
            }

            if (App.ViewModel.CourseType == CourseType.Time)
            {
                var timer = App.ViewModel.Timer;
                timer.Stop();
            }

            int total = App.ViewModel.CurrentQuestionNumber + 1;
            StringBuilder builder = new StringBuilder();
            builder.Append("You have answered ").Append(App.ViewModel.CorrectAnswers).Append(" questions correctly out of ").Append(total);

            StatisticContent.Text = builder.ToString();
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            HandleFinishAction();
        }

        public void Return_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/CourseSelection.xaml", UriKind.RelativeOrAbsolute));
        }

    }
}
