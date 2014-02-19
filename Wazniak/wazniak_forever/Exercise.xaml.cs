using System.Text;
using Microsoft.Phone.Controls;
using System;
using System.IO;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using wazniak_forever.Model;
using Windows.Storage;

namespace wazniak_forever
{
    public partial class Exercise : PhoneApplicationPage
    {
        private int correctAnswers;
        private int wrongAnswers;

        public Exercise()
        {
            InitializeComponent();
            correctAnswers = 0;
            wrongAnswers = 0;
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

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            MultipleChoiceAnswerInput.SelectedItems.Clear();
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

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            // Hide unnecessary elements
            SubmitAnswer.Visibility = Visibility.Collapsed;
            MultipleChoiceAnswerInput.Visibility = Visibility.Collapsed;
            OpenAnswerInput.Visibility = Visibility.Collapsed;

            // Check the answer
            var choices = MultipleChoiceAnswerInput.SelectedItems;
            Answer ans = new Answer();
            ans.Choices = new bool[MultipleChoiceAnswerInput.ItemsSource.Count];
            int selectedIndex;
            for (int i = 0; i < choices.Count; i++)
            {
                selectedIndex = MultipleChoiceAnswerInput.ItemsSource.IndexOf(choices[i]);
                ans.Choices[selectedIndex] = true;
            }

            ans.Text = OpenAnswerInput.Text;
            bool[] feedback = App.ViewModel.CurrentExercise.Check(ans);

            //DEBUG
            for (int i = 0; i < feedback.Length; i++)
                System.Diagnostics.Debug.WriteLine(feedback[i]);
            //DEBUG

            // Show the correct answer and explanation, if provided
            StringBuilder headerBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();
            if (feedback.Length == 1)
            {
                System.Diagnostics.Debug.WriteLine("jestem tu");
                if (feedback[0])
                {
                    correctAnswers++;
                    headerBuilder.Append("Correct!");
                    builder.Append("");
                }
                else
                {
                    wrongAnswers++;
                    headerBuilder.Append("You're wrong :-(");
                    builder.Append("Explanation:\n\n")
                        .Append(((OpenExercise) App.ViewModel.CurrentExercise).Solution.Explanation);
                }
            }
            else
            {
                bool temp = true;
                for (int i = 0; i < feedback.Length; ++i)
                {
                    if (!feedback[i])
                    {
                        temp = false;
                        break;
                    }
                }
                if (temp)
                {
                    correctAnswers++;
                    headerBuilder.Append("Correct!");
                    builder.Append("");
                }
                else
                {
                    wrongAnswers++;
                    headerBuilder.Append("You're wrong :-(");
                    builder.Append("Explanation:\n\n");
                    for (int i = 0; i < feedback.Length; ++i)
                    {
                        builder.Append(i + 1);
                        if (feedback[i])
                        {
                            builder.Append("). ok\n");
                        }
                        else
                        {
                            builder.Append("). Wrong:").Append(((MultipleChoiceExercise)App.ViewModel.CurrentExercise).Solutions[i].Explanation);
                            builder.AppendLine();
                        }
                    }
                }
            }
                
            ExplanationPanel.Visibility = Visibility.Visible;
            ExplanationHeader.Text = headerBuilder.ToString();
            Explanation.Text = builder.ToString();

            // Let's show a proper button
            if (App.ViewModel.CurrentQuestionNumber == App.ViewModel.Exercises.Count - 1)
            {
                // We have just finished the test, let's see results
                Finish.Visibility = Visibility.Visible;
            }
            else
            {
                OpenAnswerInput.Text = string.Empty;
                NextQuestion.Visibility = Visibility.Visible;
            }
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            // Show proper elements
            SubmitAnswer.Visibility = Visibility.Visible;
            NextQuestion.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;

            // Extract next question
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
                    MultipleChoiceAnswerInput.IsSelectionEnabled = true;
                    OpenAnswerInput.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            // Hide everything unnecessary and show statistics
            Return.Visibility = Visibility.Visible;
            Finish.Visibility = Visibility.Collapsed;
            QuestionContent.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;
            StatisticTitle.Visibility = Visibility.Visible;

            int total = correctAnswers + wrongAnswers;
            StringBuilder builder = new StringBuilder();
            builder.Append("You have answered ").Append(correctAnswers).Append(" questions correctly out of ").Append(total);

            StatisticContent.Text = builder.ToString();
            StatisticContent.Visibility = Visibility.Visible;
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CourseSelection.xaml", UriKind.RelativeOrAbsolute));
        }

    }
}