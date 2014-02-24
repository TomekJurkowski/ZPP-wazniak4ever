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
        }

        public void MakeVisible()
        {
            SubmitAnswer.Visibility = Visibility.Visible;
            NextQuestion.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;
        }

        public void SubmitAnswerClick(StringBuilder headerBuilder, StringBuilder builder)
        {
            ExplanationPanel.Visibility = Visibility.Visible;
            ExplanationHeader.Text = headerBuilder.ToString();
            Explanation.Text = builder.ToString();

            // Let's show a proper button
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
            // Extract next question
            App.ViewModel.CurrentQuestionNumber++;
            if (App.ViewModel.CurrentSolution.Answer.Type == SolutionType.Multiple)
                App.ViewModel.UserChoices = (App.ViewModel.Exercises[App.ViewModel.CurrentQuestionNumber]).Solution.Choices;
            string navTo;
            switch (App.ViewModel.CurrentSolution.Answer.Type)
            {
                case SolutionType.Open:
                    navTo = string.Format("/ExerciseOpen.xaml?courseName={0}", CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                    break;
                case SolutionType.Multiple:
                    navTo = string.Format("/ExerciseMultipleChoice.xaml?courseName={0}", CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                    break;
            }
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            // Hide everything unnecessary and show statistics
            Return.Visibility = Visibility.Visible;
            Finish.Visibility = Visibility.Collapsed;
            QuestionContent.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;
            StatisticTitle.Visibility = Visibility.Visible;

            int total = App.ViewModel.CurrentQuestionNumber + 1;
            StringBuilder builder = new StringBuilder();
            builder.Append("You have answered ").Append(App.ViewModel.CorrectAnswers).Append(" questions correctly out of ").Append(total);

            StatisticContent.Text = builder.ToString();
            StatisticContent.Visibility = Visibility.Visible;
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            /*MultipleChoiceAnswerInput.SelectedItems.Clear();
            NavigationService.Navigate(new Uri("/CourseSelection.xaml", UriKind.RelativeOrAbsolute));*/
        }

    }
}
