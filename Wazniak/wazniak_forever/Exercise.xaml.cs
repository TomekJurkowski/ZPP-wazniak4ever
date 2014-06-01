﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wazniak_forever.Model;
using Task = wazniak_forever.Model.Task;

namespace wazniak_forever.Controls
{
    public partial class Exercise : UserControl
    {
        public Exercise()
        {
            InitializeComponent();
            DataContext = App.ViewModel;

            if (App.ViewModel != null && App.ViewModel.CourseType == CourseType.Time)
            {
                TimerTextBlock.Visibility = Visibility.Visible;
                App.ViewModel.HandleTimesUp += HandleTimesUpAction;
            }

            
        }

        public async System.Threading.Tasks.Task AdjustQuestionBox(DependencyObject page)
        {
            if (App.ViewModel != null
                && App.ViewModel.CurrentExercise.TaskDiscriminator == "MathematicalTask")
            {
                MathQuestionBox.Visibility = Visibility.Visible;
                QuestionContent.Visibility = Visibility.Collapsed;
                await App.ViewModel.PerformTimeConsumingProcess(page, "Loading question...", async () =>
                {
                    await FillRichTextBox(App.ViewModel.CurrentExercise.ModifiedText);
                });

            }
            else
            {
                MathQuestionBox.Visibility = Visibility.Collapsed;
                QuestionContent.Visibility = Visibility.Visible;
            }
        }

        public void ShowImageAttachment()
        {
            if (App.ViewModel != null
                && !string.IsNullOrEmpty(App.ViewModel.CurrentExercise.ImageUrl))
            {
                ImageAttachment.Visibility = Visibility.Visible;
                SetImageAttachment(App.ViewModel.CurrentExercise.ImageUrl);
            } 
            else
            {
                ImageAttachment.Visibility = Visibility.Collapsed;
            }
        }

        private void SetImageAttachment(string imageUrl)
        {
            


            var imageSource = new ImageSource(imageUrl);
            var myBinding = new Binding("BitmapImage")
            {
                Source = imageSource
            };

            ImageAttachment.SetBinding(Image.SourceProperty, myBinding);

            imageSource.LoadImage(imageUrl);
        }

        private async System.Threading.Tasks.Task FillRichTextBox(string text)
        {
            var start = 0;
            var regex = new Regex("(\\$[^\\$]+?\\$)|(\\$\\$[^\\$]+?\\$\\$)");
            var paragraph = new Paragraph();
            foreach (Match match in regex.Matches(text))
            {
                var matchVal = match.Value;
                var notInline = matchVal.StartsWith("$$");
                var imageId = notInline
                    ? match.Value.Substring(3, match.Value.Length - 6)
                    : match.Value.Substring(2, match.Value.Length - 4);
                
                paragraph.AddText(start, match.Index, text);

                if (notInline)
                {
                    MathQuestionBox.Blocks.Add(paragraph);
                    var imgParagraph = new Paragraph();
                    await imgParagraph.ReplaceLabelWithLargeImage(imageId);
                    MathQuestionBox.Blocks.Add(imgParagraph);
                    paragraph = new Paragraph();
                }
                else
                {
                    await paragraph.ReplaceLabelWithImage(imageId); 
                }                
                start = match.Index + match.Length;
            }
            paragraph.AddText(start, text.Length, text);
            //App.ViewModel.Blocks.Add(paragraph);
            MathQuestionBox.Blocks.Add(paragraph);
        }

        public void AddElement(FrameworkElement element)
        {
            Grid.SetRow(element, 4);
            ContentPanel.Children.Add(element);
        }

        public void NextExerciseVisible()
        {
            SubmitAnswer.Visibility = Visibility.Visible;
            NextQuestion.Visibility = Visibility.Collapsed;
            Finish.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;
        }

        public void SubmitAnswerClick(StringBuilder headerBuilder, StringBuilder builder)
        {
            foreach (FrameworkElement element in ContentPanel.Children)
            {
                if (Grid.GetRow(element) == 3 || Grid.GetRow(element) == 4) element.Visibility = Visibility.Collapsed;
            }

            SubmitAnswer.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Visible;
            ExplanationHeader.Text = headerBuilder.ToString();
            Explanation.Text = builder.ToString();

            if (App.ViewModel.CurrentQuestionNumber == App.ViewModel.Exercises.Count - 1)
            {
                Finish.HorizontalAlignment = HorizontalAlignment.Right;
                Finish.Visibility = Visibility.Visible;
                if (App.ViewModel.CourseType != CourseType.Classic) Finish.Content = "My results";
            }
            else
            {
                if (App.ViewModel.CourseType == CourseType.StudyWithClarifier) Finish.Visibility = Visibility.Visible;
                NextQuestion.Visibility = Visibility.Visible;
            }
        }

        private void ChangeQuestion(int next)
        {
            App.ViewModel.CurrentQuestionNumber += next;
            App.ViewModel.CurrentExercise = App.ViewModel.Exercises[App.ViewModel.CurrentQuestionNumber];
            App.ViewModel.CurrentSolution = App.ViewModel.Solutions[App.ViewModel.CurrentQuestionNumber];
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            // Extract next question
            ChangeQuestion(1);

            if (App.ViewModel.CourseType == CourseType.StudyWithClarifier)
            {
                App.ViewModel.CurrentModule = App.ViewModel.SubjectModules.Find(module => module.ID == App.ViewModel.CurrentExercise.ModuleID);
                if (App.ViewModel.CurrentModule != null) App.ViewModel.CurrentModuleIndex = App.ViewModel.CurrentModule.SequenceNo;
                else
                {
                    MessageBox.Show("No modules defined for next exercises");
                    ChangeQuestion(-1);
                    return;
                }
            }

            NextExerciseVisible();
            SolutionType NextType = App.ViewModel.CurrentSolution.Answer.Type;
            if (NextType == SolutionType.Multiple || NextType == SolutionType.Single)
                App.ViewModel.UserChoices = (App.ViewModel.Exercises[App.ViewModel.CurrentQuestionNumber]).Solution.Choices;
            string navTo;
            switch (App.ViewModel.CurrentSolution.Answer.Type)
            {
                case SolutionType.Open:
                    navTo = string.Format("/ExerciseOpen.xaml?courseName={0}&id=" + App.ViewModel.CurrentQuestionNumber, CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                    break;
                case SolutionType.Value:
                    navTo = string.Format("/ExerciseSingleValue.xaml?courseName={0}&id=" + App.ViewModel.CurrentQuestionNumber, CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                    break;
                case SolutionType.Multiple:
                    navTo = string.Format("/ExerciseMultipleChoice.xaml?courseName={0}&id=" + App.ViewModel.CurrentQuestionNumber, CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                    break;
                case SolutionType.Single:
                    navTo = string.Format("/ExerciseSingleChoice.xaml?courseName={0}&id=" + App.ViewModel.CurrentQuestionNumber, CourseName.Text);
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
                    break;
            }
        }

        public void HandleTimesUpAction()
        {
            var navTo = string.Format("/TimedModeResults.xaml?courseName={0}", CourseName.Text);
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
        }

        public async void HandleFinishAction() 
        {
            SubmitAnswer.Visibility = Visibility.Collapsed;
            NextQuestion.Visibility = Visibility.Collapsed;
            Finish.Visibility = Visibility.Collapsed;
            Return.Visibility = Visibility.Visible;
            QuestionContent.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;
            MathQuestionBox.Visibility = Visibility.Collapsed;
            ImageAttachment.Visibility = Visibility.Collapsed;

            if (App.ViewModel.CourseType != CourseType.Classic)
            {
                StatisticTitle.Visibility = Visibility.Visible;
                StatisticContent.Visibility = Visibility.Visible;
            }

            if (App.ViewModel.CourseType == CourseType.Time)
            {
                var timer = App.ViewModel.Timer;
                timer.Stop();
            }

            int total = App.ViewModel.CurrentQuestionNumber + 1;
            StringBuilder builder = new StringBuilder();
            builder.Append("You have answered ").Append(App.ViewModel.CorrectAnswers).Append(" questions correctly out of ").Append(total);

            StatisticContent.Text = builder.ToString();

            if (App.ViewModel.db.User != null
                && App.ViewModel.CourseType == CourseType.StudyWithClarifier)
            {
                await App.ViewModel.PerformTimeConsumingProcess(this, "Sending your results...", async () =>
                {
                    await App.ViewModel.SendMyResults(App.ViewModel.CorrectAnswers, total);
                });
            }
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            HandleFinishAction();
        }

        public void Return_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/CourseSelection.xaml", UriKind.RelativeOrAbsolute));
        }

    }

    public static class RichTextBoxExtensions
    {
        private const string BLOB_URL = "http://clarifierblob.blob.core.windows.net/clarifiermathimages/";
        public static void AddText(this Paragraph paragraph, int start, int end, string text)
        {
            var run = new Run
            {
                Text = text.Substring(start, end - start)
            };
            paragraph.Inlines.Add(run);
        }

        public static async Task<byte[]> LoadImageFromUrl(string imageUrl)
        {        
            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, imageUrl);
                var responseMessage = await client.SendAsync(requestMessage);
                var a = await responseMessage.Content.ReadAsByteArrayAsync();

                return await responseMessage.Content.ReadAsByteArrayAsync();
            }
        }


        public static BitmapImage LoadBitmapImage(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                var image = new BitmapImage();
                image.SetSource(ms);
                return image;
            }
        }

        public async static System.Threading.Tasks.Task ReplaceLabelWithImage(this Paragraph paragraph, string id)
        {
            var requestUri = BLOB_URL + id;
            var imageData = App.ViewModel.OnlineMode
                ? await LoadImageFromUrl(requestUri)
                : await App.ViewModel.db.LoadMathImageOfflineByName(id);
            var image = new Image
            {
                Source = LoadBitmapImage(imageData),
                //Source = new BitmapImage(new Uri("/Assets/CodeCogsEqnInline.png", UriKind.RelativeOrAbsolute)),
                MaxHeight = 25
            };
            paragraph.TextAlignment = TextAlignment.Left;
            var uiImage = new InlineUIContainer { Child = image };
            paragraph.Inlines.Add(uiImage);
        }

        public async static System.Threading.Tasks.Task ReplaceLabelWithLargeImage(this Paragraph paragraph, string id)
        {
            var requestUri = BLOB_URL + id;

            var imageData = App.ViewModel.OnlineMode
                ? await LoadImageFromUrl(requestUri)
                : await App.ViewModel.db.LoadMathImageOfflineByName(id);
            var image = new Image
            {
                Source = LoadBitmapImage(imageData),
                //Source = new BitmapImage(new Uri("/Assets/CodeCogsEqnInline.png", UriKind.RelativeOrAbsolute)),
                MaxHeight = 50
            };

            var uiImage = new InlineUIContainer { Child = image };

            paragraph.TextAlignment = TextAlignment.Center;
            paragraph.Inlines.Add(uiImage);
        }
    }

    public class ImageSource : INotifyPropertyChanged
    {
        private string _imageUrl;

        private BitmapImage _bitmapImage;

        public ImageSource() { }

        public ImageSource(string imageUrl)
        {
            _imageUrl = imageUrl;
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                OnPropertyChanged("ImageUrl");
            }
        }

        public BitmapImage BitmapImage
        {
            get { return _bitmapImage; }
            set
            {
                _bitmapImage = value;
                OnPropertyChanged("BitmapImage");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        public async void LoadImage(string imageUrl)
        {
            var imageBytes = App.ViewModel.OnlineMode
                ? await RichTextBoxExtensions.LoadImageFromUrl(imageUrl)
                : await App.ViewModel.db.LoadImageAttachmentOffline(App.ViewModel.CurrentExercise.ID);
            BitmapImage = RichTextBoxExtensions.LoadBitmapImage(imageBytes);
        }
    }
}
