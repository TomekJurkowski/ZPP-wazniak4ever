using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace wazniak_forever.Controls
{
    public partial class Exercise : UserControl
    {
        public Exercise()
        {
            InitializeComponent();
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            /*// Hide unnecessary elements
            SubmitAnswer.Visibility = Visibility.Collapsed;
            MultipleChoiceAnswerInput.Visibility = Visibility.Collapsed;
            OpenAnswerInput.Visibility = Visibility.Collapsed;

            // Check the answer
            List<string> choiceList = MultipleChoiceAnswerInput.SelectedItems as List<string>;
            AnswerList<string> ans = new AnswerList<string>(choiceList);
            SingleAnswer<string> textAns = new SingleAnswer<string>(OpenAnswerInput.Text);


            bool[] feedback = App.ViewModel.CurrentSolution.Check(ans);

            //DEBUG
            for (int i = 0; i < feedback.Length; i++)
                System.Diagnostics.Debug.WriteLine(feedback[i]);
            //DEBUG

            // Show the correct answer and explanation, if provided
            StringBuilder headerBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();
            if (feedback.Length == 1)
            {
                System.Diagnostics.Debug.WriteLine("jestem tu");
                if (feedback[0])
                {
                    correctAnswers++;
                    headerBuilder.Append("Correct!");
                    builder.Append("");
                }
                else
                {
                    wrongAnswers++;
                    headerBuilder.Append("You're wrong :-(");
                    builder.Append("Explanation:\n\n")
                        .Append(((OpenExercise) App.ViewModel.CurrentExercise).Solution.Explanation);
                }
            }
            else
            {
                bool temp = true;
                for (int i = 0; i < feedback.Length; ++i)
                {
                    if (!feedback[i])
                    {
                        temp = false;
                        break;
                    }
                }
                if (temp)
                {
                    correctAnswers++;
                    headerBuilder.Append("Correct!");
                    builder.Append("");
                }
                else
                {
                    wrongAnswers++;
                    headerBuilder.Append("You're wrong :-(");
                    builder.Append("Explanation:\n\n");
                    for (int i = 0; i < feedback.Length; ++i)
                    {
                        builder.Append(i + 1);
                        if (feedback[i])
                        {
                            builder.Append("). ok\n");
                        }
                        else
                        {
                            builder.Append("). Wrong:").Append(((MultipleChoiceExercise)App.ViewModel.CurrentExercise).Solutions[i].Explanation);
                            builder.AppendLine();
                        }
                    }
                }
            }
                
            ExplanationPanel.Visibility = Visibility.Visible;
            ExplanationHeader.Text = headerBuilder.ToString();
            Explanation.Text = builder.ToString();

            // Let's show a proper button
            if (App.ViewModel.CurrentQuestionNumber == App.ViewModel.Exercises.Count - 1)
            {
                // We have just finished the test, let's see results
                Finish.Visibility = Visibility.Visible;
            }
            else
            {
                OpenAnswerInput.Text = string.Empty;
                NextQuestion.Visibility = Visibility.Visible;
            }*/
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            /*// Show proper elements
            SubmitAnswer.Visibility = Visibility.Visible;
            NextQuestion.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;

            // Extract next question
            App.ViewModel.CurrentQuestionNumber++;
            App.ViewModel.CurrentExercise = App.ViewModel.Exercises[App.ViewModel.CurrentQuestionNumber];
            App.ViewModel.CurrentSolution = App.ViewModel.Solutions[App.ViewModel.CurrentQuestionNumber];
            if (App.ViewModel.CurrentSolution.Answer.Type == SolutionType.Multiple)
                App.ViewModel.UserChoices = (App.ViewModel.Exercises[App.ViewModel.CurrentQuestionNumber]).Solution.Choices;
            switch (App.ViewModel.CurrentSolution.Answer.Type)
            {
                case SolutionType.Open:
                    MultipleChoiceAnswerInput.Visibility = Visibility.Collapsed;
                    OpenAnswerInput.Visibility = Visibility.Visible;
                    break;
                case SolutionType.Multiple:
                    MultipleChoiceAnswerInput.Visibility = Visibility.Visible;
                    MultipleChoiceAnswerInput.IsSelectionEnabled = true;
                    OpenAnswerInput.Visibility = Visibility.Collapsed;
                    break;
            }*/
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            /*// Hide everything unnecessary and show statistics
            Return.Visibility = Visibility.Visible;
            Finish.Visibility = Visibility.Collapsed;
            QuestionContent.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;
            StatisticTitle.Visibility = Visibility.Visible;

            int total = correctAnswers + wrongAnswers;
            StringBuilder builder = new StringBuilder();
            builder.Append("You have answered ").Append(correctAnswers).Append(" questions correctly out of ").Append(total);

            StatisticContent.Text = builder.ToString();
            StatisticContent.Visibility = Visibility.Visible;*/
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            /*MultipleChoiceAnswerInput.SelectedItems.Clear();
            NavigationService.Navigate(new Uri("/CourseSelection.xaml", UriKind.RelativeOrAbsolute));*/
        }

    }
}
