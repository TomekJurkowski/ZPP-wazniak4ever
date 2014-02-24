using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace wazniak_forever
{
    public partial class ExerciseOpen : PhoneApplicationPage
    {
        public ExerciseOpen()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
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
        }

        private void AddEvents()
        {
            ExControl.SubmitAnswer.Click += new RoutedEventHandler(SubmitAnswer_Click);
            ExControl.Return.Click += new RoutedEventHandler(Return_Click);
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}