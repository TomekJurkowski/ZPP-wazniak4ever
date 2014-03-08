using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Text;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wazniak_forever.Model;

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

        private void AddEvents()
        {
            ExControl.SubmitAnswer.Click += new RoutedEventHandler(SubmitAnswer_Click);
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder headerBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();
            headerBuilder.Append("Answer:\n");
            builder.Append((App.ViewModel.CurrentSolution.Answer as SingleAnswer<string>).value);
            ExControl.SubmitAnswerClick(headerBuilder, builder);
        }
    }
}