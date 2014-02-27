﻿using System;
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

namespace wazniak_forever
{
    public partial class ExerciseSingleValue : PhoneApplicationPage
    {
        public ExerciseSingleValue()
        {
            InitializeComponent();
            AddEvents();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ExControl.CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
        }

        private void AddEvents()
        {
            ExControl.SubmitAnswer.Click += new RoutedEventHandler(SubmitAnswer_Click);
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder headerBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();
            if (AnswerBox.Text == (App.ViewModel.CurrentSolution.Answer as SingleAnswer<string>).value) headerBuilder.Append("Correct!");
            else
            {
                headerBuilder.Append("Wrong!");
                builder.Append("You answered: " + AnswerBox.Text + "\n");
                builder.Append("Correct answer is " + (App.ViewModel.CurrentSolution.Answer as SingleAnswer<string>).value);
            }
            ExControl.SubmitAnswerClick(headerBuilder, builder);
        }
    }
}