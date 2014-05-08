using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wazniak_forever.Model;

namespace wazniak_forever
{
    public partial class ExerciseSingleValue : PhoneApplicationPage
    {
        public ExerciseSingleValue()
        {
            InitializeComponent();
            AddEvents();
            LayoutRoot.Children.Remove(AnswerBox);
            ExControl.AddElement(AnswerBox);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ExControl.CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
            ExControl.AdjustQuestionBox();
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
        }

        private void AddEvents()
        {
            ExControl.SubmitAnswer.Click += new RoutedEventHandler(SubmitAnswer_Click);
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder headerBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();

            var currentExerciseId = App.ViewModel.Exercises[App.ViewModel.CurrentQuestionNumber].ID;

            if (AnswerBox.Text == (App.ViewModel.CurrentSolution.Answer as SingleAnswer<string>).value)
            {
                App.ViewModel.AddAnswer(currentExerciseId, true);
                App.ViewModel.CorrectAnswers++;
                headerBuilder.Append("Correct!");
                ExControl.CorrectAnswerMediaElement.Play();
            }
            else
            {
                App.ViewModel.AddAnswer(currentExerciseId, false);
                headerBuilder.Append("Wrong!");
                builder.Append("You answered: " + AnswerBox.Text + "\n");
                builder.Append("Correct answer is: " + (App.ViewModel.CurrentSolution.Answer as SingleAnswer<string>).value);
                ExControl.WrongAnswerMediaElement.Play();
            }
            AnswerBox.Visibility = Visibility.Collapsed;
            ExControl.SubmitAnswerClick(headerBuilder, builder);
        }
    }
}
