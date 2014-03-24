using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wazniak_forever.Model;

namespace wazniak_forever
{
    public partial class ExerciseMultipleChoice : PhoneApplicationPage
    {

        public ExerciseMultipleChoice()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            AddEvents();
            LayoutRoot.Children.Remove(MultipleChoiceAnswerInput);
            ExControl.AddElement(MultipleChoiceAnswerInput);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ExControl.CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NavigationService.RemoveBackEntry();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (!App.ViewModel.HandleCourseExit(e)) return;
            base.OnBackKeyPress(e);
            MultipleChoiceAnswerInput.SelectedItems.Clear();
        }

        private void AddEvents()
        {
            ExControl.SubmitAnswer.Click += new RoutedEventHandler(SubmitAnswer_Click);
        }

        private List<bool> parseToList(IList toBeParsed)
        {
            List<bool> result = new List<bool>();
            List<string> choices = App.ViewModel.UserChoices;
            int i = 0;
            foreach (string s in choices)
            {
                if (i < toBeParsed.Count && s.Equals(toBeParsed[i].ToString()))
                {
                    result.Add(true);
                    i++;
                }
                else result.Add(false);
            }
            return result;
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            ExControl.NextExerciseVisible();
            List<bool> choiceList = parseToList(MultipleChoiceAnswerInput.SelectedItems);
            MultipleChoiceAnswerInput.SelectedItems.Clear();
            if (choiceList.Count == 0) return;
            AnswerList<bool> ans = new AnswerList<bool>(choiceList);
            bool correctAnswer = true;
            bool[] feedback = ans.GetFeedback(App.ViewModel.CurrentSolution.Answer as AnswerList<bool>);
            StringBuilder headerBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < feedback.Length; ++i)
            {
                if (!feedback[i])
                {
                    correctAnswer = false;
                    break;
                }
            }

            var currentExerciseId = App.ViewModel.Exercises[App.ViewModel.CurrentQuestionNumber].ID;
            
            if (correctAnswer)
            {
                App.ViewModel.AddAnswer(currentExerciseId, true);
                App.ViewModel.CorrectAnswers++;
                headerBuilder.Append("Correct!");
                builder.Append("");
                ExControl.CorrectAnswerMediaElement.Play();
            }
            else
            {
                App.ViewModel.AddAnswer(currentExerciseId, false);
                headerBuilder.Append("You're wrong :-(");
                builder.Append("Explanation:\n");
                for (int i = 0; i < feedback.Length; ++i)
                {
                    builder.Append(i + 1);
                    if (feedback[i])
                    {
                        builder.Append("). OK\n");
                    }
                    else
                    {
                        builder.Append("). Wrong!");
                        builder.AppendLine();
                    }
                }
                ExControl.WrongAnswerMediaElement.Play();
            }
            MultipleChoiceAnswerInput.Visibility = Visibility.Collapsed;
            ExControl.SubmitAnswerClick(headerBuilder, builder);
        }
    }
}