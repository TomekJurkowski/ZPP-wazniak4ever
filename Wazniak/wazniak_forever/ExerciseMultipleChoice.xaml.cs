using System;
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

        public MultipleChoiceSolution MySolution { get; set; }

        public ExerciseMultipleChoice()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            MySolution = App.ViewModel.Solutions[App.ViewModel.CurrentQuestionNumber] as MultipleChoiceSolution;
            AddEvents(ExControl);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ExControl.CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            //MultipleChoiceAnswerInput.SelectedItems.Clear();
        }

        private void AddEvents(Controls.Exercise ExControl)
        {
            ExControl.SubmitAnswer.Click += new RoutedEventHandler(SubmitAnswer_Click);
            /*ExControl.NextQuestion.Click += new RoutedEventHandler(NextQuestion_Click);
            ExControl.Finish.Click += new RoutedEventHandler(Finish_Click);
            ExControl.Return.Click += new RoutedEventHandler(Return_Click);*/
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            ExControl.MakeVisible();
            List<string> choiceList = MultipleChoiceAnswerInput.SelectedItems as List<string>;
            System.Diagnostics.Debug.WriteLine(choiceList.Count);
            AnswerList<string> ans = new AnswerList<string>(choiceList);
            bool[] feedback = ans.GetFeedback(MySolution.Answer as AnswerList<string>);
            StringBuilder headerBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();
            bool correctAnswer = true;
            for (int i = 0; i < feedback.Length; ++i)
            {
                if (!feedback[i])
                {
                    correctAnswer = false;
                    break;
                }
            }
            if (correctAnswer)
            {
                App.ViewModel.CorrectAnswers++;
                headerBuilder.Append("Correct!");
                builder.Append("");
            }
            else
            {
                headerBuilder.Append("You're wrong :-(");
                builder.Append("Explanation:\n\n");
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
            }
            ExControl.SubmitAnswerClick(headerBuilder, builder);
        }


    }
}