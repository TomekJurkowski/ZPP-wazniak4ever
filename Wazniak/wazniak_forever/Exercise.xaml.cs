using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Storage;
using System.IO;
using wazniak_forever.Model;
using System.Windows.Data;

namespace wazniak_forever
{
    public partial class Exercise : PhoneApplicationPage
    {
        public Exercise()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.LoadExercises();
            var currentExercise = App.ViewModel.CurrentExercise;
            switch (currentExercise.Type)
            {
                case ExerciseType.Open:
                    MultipleChoiceAnswerInput.Visibility = Visibility.Collapsed;
                    OpenAnswerInput.Visibility = Visibility.Visible;
                    break;
                case ExerciseType.MultipleChoice:
                    MultipleChoiceAnswerInput.Visibility = Visibility.Visible;
                    OpenAnswerInput.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CourseName.Text = Convert.ToString(NavigationContext.QueryString["courseName"]);
        }

        private async void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            StorageFile sFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\Math\web.html");

            var fileStream = await sFile.OpenStreamForReadAsync();
            string webString;
            string newString = @"NEW CONTENT: $$\int_{i=0}^{\pi} xdx$$";
            using (var streamReader = new StreamReader(fileStream))
            {
                webString = streamReader.ReadToEnd();
            }
            webString = webString.Insert(webString.IndexOf("<body>") + "<body>".Length, newString);
            System.Diagnostics.Debug.WriteLine(webString);
            WebBrowser.NavigateToString(webString);
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            var choices = MultipleChoiceAnswerInput.SelectedItems;
            Answer ans = new Answer();
            ans.Choices = new bool[MultipleChoiceAnswerInput.ItemsSource.Count];
            for (int i = 0; i < choices.Count; i++)
            {
                int selectedIndex = MultipleChoiceAnswerInput.ItemsSource.IndexOf(choices[i]);
                ans.Choices[selectedIndex] = true;
            }

            ans.Text = OpenAnswerInput.Text;
            bool[] feedback = App.ViewModel.CurrentExercise.Check(ans);

            for (int i = 0; i < feedback.Length; i++) 
                System.Diagnostics.Debug.WriteLine(feedback[i]);

            if (App.ViewModel.CurrentQuestionNumber == App.ViewModel.Exercises.Count - 1)
            {
                return;
            }

            App.ViewModel.CurrentQuestionNumber++;
            App.ViewModel.CurrentExercise = App.ViewModel.Exercises[App.ViewModel.CurrentQuestionNumber];
            var currentExercise = App.ViewModel.CurrentExercise;
            if (App.ViewModel.CurrentExercise.Type == ExerciseType.MultipleChoice)
                App.ViewModel.UserChoices = ((MultipleChoiceExercise)App.ViewModel.Exercises[App.ViewModel.CurrentQuestionNumber]).Choices;
            switch (currentExercise.Type)
            {
                case ExerciseType.Open:
                    MultipleChoiceAnswerInput.Visibility = Visibility.Collapsed;
                    OpenAnswerInput.Visibility = Visibility.Visible;
                    break;
                case ExerciseType.MultipleChoice:
                    MultipleChoiceAnswerInput.Visibility = Visibility.Visible;
                    OpenAnswerInput.Visibility = Visibility.Collapsed;
                    break;
            }
            
            
        }
    }
}