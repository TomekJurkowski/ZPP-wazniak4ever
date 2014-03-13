using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wazniak_forever.Model;

namespace wazniak_forever
{
    public partial class ExerciseSingleChoice : PhoneApplicationPage
    {
        string choice = "";

        public ExerciseSingleChoice()
        {
            InitializeComponent(); DataContext = App.ViewModel;
            AddChoices();
            AddEvents();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ExControl.CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (!App.ViewModel.HandleCourseExit(e)) return;
            base.OnBackKeyPress(e);
        }

        private void AddChoices()
        {
            StackPanel SingleChoicePanel = new StackPanel();
            SingleChoicePanel.Visibility = Visibility.Visible;
            SingleChoicePanel.VerticalAlignment = VerticalAlignment.Top;
            foreach (string s in App.ViewModel.CurrentSolution.Choices)
            {
                ScrollViewer SingleChoiceViewer = new ScrollViewer();
                SingleChoiceViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                SingleChoiceViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                RadioButton radioButton1 = new RadioButton();
                radioButton1.Content = s;
                radioButton1.FontSize = 20;
                radioButton1.GroupName = "SingleChoices";
                radioButton1.Foreground = Application.Current.Resources["MenuItemColor"] as System.Windows.Media.Brush;
                radioButton1.Checked += RadioButton_Checked;
                SingleChoiceViewer.Content = radioButton1;
                SingleChoicePanel.Children.Add(SingleChoiceViewer);
            }
            ExControl.AddElement(SingleChoicePanel);
        }

        private void AddEvents()
        {
            ExControl.SubmitAnswer.Click += new RoutedEventHandler(SubmitAnswer_Click);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            choice = (sender as RadioButton).Content as string;
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder headerBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();
            SingleAnswer<string> ans = new SingleAnswer<string>(choice);
            if (ans.Equals(App.ViewModel.CurrentSolution.Answer as SingleAnswer<string>))
            {
                App.ViewModel.CorrectAnswers++;
                headerBuilder.Append("Correct!\n");
                ExControl.CorrectAnswerMediaElement.Play();
            }
            else
            {
                headerBuilder.Append("Wrong!\n");
                builder.Append("You answered: " + choice + "\nCorrect answer is: " + (App.ViewModel.CurrentSolution.Answer as SingleAnswer<string>).value);
                ExControl.WrongAnswerMediaElement.Play();
            }
            ExControl.SubmitAnswerClick(headerBuilder, builder);
        }
    }
}