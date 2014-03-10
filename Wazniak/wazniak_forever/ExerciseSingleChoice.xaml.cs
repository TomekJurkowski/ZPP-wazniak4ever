﻿using System;
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
        string choice = "";

        public ExerciseSingleChoice()
        {
            InitializeComponent(); DataContext = App.ViewModel;
            //AddRowDefinitions();
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
            base.OnBackKeyPress(e);
            if (NavigationService.CanGoBack)
            {
                e.Cancel = true;
                NavigationService.RemoveBackEntry();
                NavigationService.GoBack();
            }
        }

        private void AddChoices()
        {
            foreach (string s in App.ViewModel.CurrentSolution.Choices)
            {
                RadioButton radioButton1 = new RadioButton();
                radioButton1.Content = s;
                radioButton1.GroupName = "SingleChoices";
                radioButton1.Foreground = Application.Current.Resources["PageNameColor"] as System.Windows.Media.Brush;
                radioButton1.Checked += RadioButton_Checked;
                SingleChoicePanel.Children.Add(radioButton1);
            }
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
                headerBuilder.Append("Correct!\n");
                ExControl.CorrectAnswerMediaElement.Play();
            }
            else
            {
                headerBuilder.Append("Wrong!\n");
                builder.Append("You answered: " + choice + "\nCorrect answer is: " + (App.ViewModel.CurrentSolution.Answer as SingleAnswer<string>).value);
                ExControl.WrongAnswerMediaElement.Play();
            }
            SingleChoicePanel.Visibility = Visibility.Collapsed;
            ExControl.SubmitAnswerClick(headerBuilder, builder);
        }
    }
}