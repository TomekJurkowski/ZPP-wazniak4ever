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
    public partial class ExerciseSingleChoice : PhoneApplicationPage
    {
        public ExerciseSingleChoice()
        {
            InitializeComponent(); DataContext = App.ViewModel;
            //AddRowDefinitions();
            AddEvents();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ExControl.CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            // BUG?
        }

        private void AddRowDefinitions()
        {
            foreach (RowDefinition row in ExControl.LayoutRoot.RowDefinitions)
            {
                LayoutRoot.RowDefinitions.Add(row);
            }
        }

        private void AddEvents()
        {
            ExControl.SubmitAnswer.Click += new RoutedEventHandler(SubmitAnswer_Click);
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder headerBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();
            string choice = SingleChoiceAnswerInput.SelectedItem.ToString();
            SingleAnswer<string> ans = new SingleAnswer<string>(choice);
            if (ans.Equals(App.ViewModel.CurrentSolution.Answer as SingleAnswer<string>))
            {
                headerBuilder.Append("Correct!\n");
                ExControl.CorrectAnswerMediaElement.Play();
            }
            else
            {
                headerBuilder.Append("Wrong!\n");
                builder.Append("You answered: " + choice + "\n Correct answer is: " + (App.ViewModel.CurrentSolution.Answer as SingleAnswer<string>).value);
                ExControl.WrongAnswerMediaElement.Play();
            }
            SingleChoiceAnswerInput.Visibility = Visibility.Collapsed;
            ExControl.SubmitAnswerClick(headerBuilder, builder);
        }
    }
}